namespace Ludos.Engine.Level
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using FuncWorks.XNA.XTiled;
    using Ludos.Engine.Actors;
    using Ludos.Engine.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using RectangleF = System.Drawing.RectangleF;

    public static class LevelManager
    {
        private static List<Map> _maps;
        private static List<TMXMapInfo> _mapsInfo;
        private static int _currentLevelIndex;
        private static Dictionary<string, int> _layerIndexInfo;
        private static Map _currentMap;

        public static string CurrentMapName { get => System.IO.Path.GetFileName(_mapsInfo[_currentLevelIndex].TmxFilePath).Replace(".tmx", string.Empty); }
        public static List<MovingPlatform> MovingPlatforms { get; private set; } = new();
        public static List<GameObject> GlobalGameObjects { get; set; } = new();

        public static void Init(ContentManager content, List<TMXMapInfo> mapsInfo)
        {
            _maps = new List<Map>();
            _mapsInfo = mapsInfo;
            LoadTmxFiles(content);
            LoadMap(tmxMapIndex: 0);
        }

        public static void LoadMap(int tmxMapIndex)
        {
            _currentLevelIndex = tmxMapIndex;
            _currentMap = _maps[_currentLevelIndex];
            PopulateLayerNames();
            AssignTileLayerIdexes();
            AssignObjectLayerIdexes();
            LoadMovingPlatforms();
        }

        public static void LoadMap(string tmxMapName)
        {
            var map = _mapsInfo.Where(x => x.TmxFilePath.ToLower().Contains(tmxMapName.ToLower())).FirstOrDefault();

            if (map.Equals(default(TMXMapInfo)))
            {
                throw new Exception(string.Format("Tmx file named {0} could not be found.", tmxMapName));
            }

            LoadMap(_mapsInfo.IndexOf(map));
        }

        public static void Update(float elapsedTime)
        {
            foreach (var movingPlatform in MovingPlatforms)
            {
                movingPlatform.Update(elapsedTime);
            }

            foreach (var globalObject in GlobalGameObjects)
            {
                if (globalObject is Actor)
                {
                    foreach (var otherObject in GlobalGameObjects.Where(x => x.Id != globalObject.Id && x.CollidingLayers.Contains(GameObject.CollisionLayers.Actors) && x.Bounds.IntersectsWith(globalObject.Bounds)))
                    {
                        globalObject.InvokeCollision(otherObject);
                    }
                }
                else
                {
                    globalObject.Update(elapsedTime);
                }
            }
        }

        public static Vector2 GetPlayerStartPosition()
        {
            var playerStartObject = GetAllLayerObjects(TMXDefaultLayerInfo.ObjectLayerCamera).Where(x => x.Type == TMXDefaultTypes.PlayerStratPosition).FirstOrDefault();
            return playerStartObject == null ? Vector2.Zero : playerStartObject.Bounds.Location.ToVector2();
        }

        public static int GetLayerIndex(string layerName)
        {
            return _layerIndexInfo[layerName];
        }

        public static Rectangle GetCurrentMapBounds()
        {
            return _currentMap.Bounds;
        }

        public static IEnumerable<MapObject> GetObjectsInRegion(string layerName, RectangleF region)
        {
            return _currentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
        }

        public static IEnumerable<MapObject> GetObjectsInRegion(string layerName, RectangleF region, KeyValuePair<string, string> property)
        {
            var objectsInRegion = _currentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
            return objectsInRegion.Any() ? objectsInRegion.Where(x => x.Properties.ContainsKey(property.Key) && x.Properties[property.Key].Value == property.Value) : new List<MapObject>();
        }

        public static IEnumerable<MapObject> GetObjectsInRegion(string layerName, Rectangle region)
        {
            return _currentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
        }

        public static IEnumerable<MapObject> GetObjectsInRegion(string layerName, Rectangle region, string type)
        {
            return _currentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region).Where(x => x.Type.ToLower().Equals(type.ToLower()));
        }

        public static IEnumerable<MapObject> GetAllLayerObjects(string layer)
        {
            return _currentMap.ObjectLayers[layer].MapObjects;
        }

        public static void DrawTileLayers(SpriteBatch spriteBatch, RectangleF region, float layerDepth)
        {
            for (int i = 0; i < _maps[_currentLevelIndex].TileLayers.Count; i++)
            {
                _currentMap.DrawLayer(spriteBatch, i, region, layerDepth);
            }
        }

        public static void DrawTileLayer(SpriteBatch spriteBatch, string layerName, RectangleF region, float layerDepth)
        {
            _currentMap.DrawLayer(spriteBatch, _layerIndexInfo[layerName], region, layerDepth);
        }

        public static void DrawObjectLayer(SpriteBatch spriteBatch, string layerName, Rectangle region, float layerDepth)
        {
            if (_layerIndexInfo[layerName] == -1)
            {
                throw new Exception("Objectlayer: '" + layerName + "' does not exist.");
            }

            _currentMap.DrawObjectLayer(spriteBatch, _layerIndexInfo[layerName], region, layerDepth);
        }

        private static void LoadTmxFiles(ContentManager content)
        {
            foreach (var info in _mapsInfo)
            {
                _maps.Add(TMXContentProcessor.LoadTMX(info.TmxFilePath, info.ResourcePath, content));
            }
        }

        private static void PopulateLayerNames()
        {
            var unassignedIndex = -1;

            _layerIndexInfo = new Dictionary<string, int>
            {
                { TMXDefaultLayerInfo.ObjectLayerWorld, unassignedIndex },
                { TMXDefaultLayerInfo.ObjectLayerWater, unassignedIndex },
                { TMXDefaultLayerInfo.ObjectLayerInteractableObjects, unassignedIndex },
                { TMXDefaultLayerInfo.ObjectLayerParticles, unassignedIndex },
                { TMXDefaultLayerInfo.ObjectLayerCamera, unassignedIndex },
                { TMXDefaultLayerInfo.TileLayerForeground, unassignedIndex },
                { TMXDefaultLayerInfo.TileLayerWorld, unassignedIndex },
                { TMXDefaultLayerInfo.TileLayerBackGround, unassignedIndex },
            };

            if (_mapsInfo[_currentLevelIndex].NonDefaultLayerNames != null)
            {
                foreach (var name in _mapsInfo[_currentLevelIndex].NonDefaultLayerNames)
                {
                    _layerIndexInfo.Add(name, unassignedIndex);
                }
            }
        }

        private static void AssignTileLayerIdexes()
        {
            for (int i = 0; i < _maps[_currentLevelIndex].TileLayers.Count; i++)
            {
                var layerName = _maps[_currentLevelIndex].TileLayers[i].Name;

                if (_layerIndexInfo.ContainsKey(layerName))
                {
                    _layerIndexInfo[layerName] = i;
                }
            }
        }

        private static void AssignObjectLayerIdexes()
        {
            for (int i = 0; i < _maps[_currentLevelIndex].ObjectLayers.Count; i++)
            {
                var layerName = _maps[_currentLevelIndex].ObjectLayers[i].Name;

                if (_layerIndexInfo.ContainsKey(layerName))
                {
                    if (_layerIndexInfo[layerName] != -1)
                    {
                        throw new Exception(string.Format("A layer named '{0}' has already been added.", layerName));
                    }

                    _layerIndexInfo[layerName] = i;
                }
            }
        }

        private static void LoadMovingPlatforms()
        {
            MovingPlatforms = new List<MovingPlatform>();

            foreach (var mapObject in GetAllLayerObjects(TMXDefaultLayerInfo.ObjectLayerWorld).Where(x => x.Type?.ToLower() == TMXDefaultTypes.Platforms.ToLower()))
            {
                MovingPlatforms.Add(new MovingPlatform(mapObject.Polyline, _mapsInfo[_currentLevelIndex].MovingPlatformSize, _mapsInfo[_currentLevelIndex].MovingPlatformSpeed));
            }
        }
    }
}
