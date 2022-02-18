namespace Ludos.Engine.Tmx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FuncWorks.XNA.XTiled;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using RectangleF = System.Drawing.RectangleF;

    public class TMXManager
    {
        private readonly List<Map> _maps;
        private readonly List<TMXMapInfo> _mapsInfo;
        private int _currentLevelIndex;
        private Dictionary<string, int> _layerIndexInfo;
        private Map _currentMap;

        public TMXManager(ContentManager content, List<TMXMapInfo> mapsInfo)
        {
            _maps = new List<Map>();
            _mapsInfo = mapsInfo;

            LoadTmxFiles(content);
            LoadMap(tmxMapIndex: 0);
        }

        public string CurrentMapName { get => System.IO.Path.GetFileName(_mapsInfo[_currentLevelIndex].TmxFilePath).Replace(".tmx", string.Empty); }
        public List<MovingPlatform> MovingPlatforms { get; private set; }

        public void LoadMap(int tmxMapIndex)
        {
            _currentLevelIndex = tmxMapIndex;
            _currentMap = _maps[_currentLevelIndex];
            PopulateLayerNames();
            AssignTileLayerIdexes();
            AssignObjectLayerIdexes();
            LoadMovingPlatforms();
        }

        public void LoadMap(string tmxMapName)
        {
            var map = _mapsInfo.Where(x => x.TmxFilePath.ToLower().Contains(tmxMapName.ToLower())).FirstOrDefault();

            if (map.Equals(default(TMXMapInfo)))
            {
                throw new Exception(string.Format("Tmx file named {0} could not be found.", tmxMapName));
            }

            LoadMap(_mapsInfo.IndexOf(map));
        }

        public void Update(GameTime gameTime)
        {
            foreach (var movingPlatform in MovingPlatforms)
            {
                movingPlatform.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }
        }

        public int GetLayerIndex(string layerName)
        {
            return _layerIndexInfo[layerName];
        }

        public Rectangle GetCurrentMapBounds()
        {
            return _currentMap.Bounds;
        }

        public IEnumerable<MapObject> GetObjectsInRegion(string layerName, RectangleF region)
        {
            return _currentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
        }

        public IEnumerable<MapObject> GetObjectsInRegion(string layerName, RectangleF region, KeyValuePair<string, string> property)
        {
            var objectsInRegion = _currentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
            return objectsInRegion.Any() ? objectsInRegion.Where(x => x.Properties.ContainsKey(property.Key) && x.Properties[property.Key].Value == property.Value) : new List<MapObject>();
        }

        public IEnumerable<MapObject> GetObjectsInRegion(string layerName, Rectangle region)
        {
            return _currentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
        }

        public void DrawTileLayers(SpriteBatch spriteBatch, RectangleF region, float layerDepth)
        {
            for (int i = 0; i < _maps[_currentLevelIndex].TileLayers.Count; i++)
            {
                _currentMap.DrawLayer(spriteBatch, i, region, layerDepth);
            }
        }

        public void DrawTileLayer(SpriteBatch spriteBatch, string layerName, RectangleF region, float layerDepth)
        {
            _currentMap.DrawLayer(spriteBatch, _layerIndexInfo[layerName], region, layerDepth);
        }

        public void DrawObjectLayer(SpriteBatch spriteBatch, string layerName, Rectangle region, float layerDepth)
        {
            _currentMap.DrawObjectLayer(spriteBatch, _layerIndexInfo[layerName], region, layerDepth);
        }

        private void LoadTmxFiles(ContentManager content)
        {
            foreach (var info in _mapsInfo)
            {
                _maps.Add(TMXContentProcessor.LoadTMX(info.TmxFilePath, info.ResourcePath, content));
            }
        }

        private void PopulateLayerNames()
        {
            var unassignedIndex = -1;

            _layerIndexInfo = new Dictionary<string, int>
            {
                { TMXDefaultLayerInfo.ObjectLayerWorld, unassignedIndex },
                { TMXDefaultLayerInfo.ObjectLayerWater, unassignedIndex },
                { TMXDefaultLayerInfo.ObjectLayerInteractableObjects, unassignedIndex },
                { TMXDefaultLayerInfo.ObjectLayerParticles, unassignedIndex },
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

        private void AssignTileLayerIdexes()
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

        private void AssignObjectLayerIdexes()
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

        private void LoadMovingPlatforms()
        {
            MovingPlatforms = new List<MovingPlatform>();

            foreach (var mapObject in _currentMap.ObjectLayers[TMXDefaultLayerInfo.ObjectLayerWorld].MapObjects.Where(x => x.Polyline != null))
            {
                MovingPlatforms.Add(new MovingPlatform(mapObject.Polyline, _mapsInfo[_currentLevelIndex].MovingPlatformSize));
            }
        }
    }
}
