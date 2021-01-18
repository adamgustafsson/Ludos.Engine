using FuncWorks.XNA.XTiled;
using Ludos.Engine.Model.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Linq;

namespace Ludos.Engine.Managers
{
    public class TMXManager
    {
        public Map CurrentMap { get => _maps [_currentLevelIndex]; }
        public List<MovingPlatform> MovingPlatforms { get; private set; } = new List<MovingPlatform>();

        private int _currentLevelIndex;
        private readonly List<Map> _maps;
        private readonly List<TmxMapInfo> _mapsInfo;
        private Dictionary<string, int> _layerIndexInfo;

        public TMXManager(ContentManager content, List<TmxMapInfo> mapsInfo)
        {
            _maps = new List<Map>();
            _mapsInfo = mapsInfo;
            _currentLevelIndex = 0;

            LoadMaps(content);
            PopulateLayerNames();
            AssignObjectLayers();

            foreach (var mapObject in CurrentMap.ObjectLayers[DefaultLayerInfo.GROUND_COLLISION].MapObjects.Where(x => x.Polyline != null))
            {
                MovingPlatforms.Add(new MovingPlatform(mapObject.Polyline, mapsInfo[_currentLevelIndex].MovingPlatformSize));
            }
        }

        public void LoadMap(int mapIndex)
        {
            _currentLevelIndex = mapIndex;
            PopulateLayerNames();
            AssignObjectLayers();
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

        public IEnumerable<MapObject> GetObjectsInRegion(string layerName, System.Drawing.RectangleF region)
        {
            return CurrentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
        }

        public IEnumerable<MapObject> GetObjectsInRegion(string layerName, System.Drawing.RectangleF region, KeyValuePair<string, string> property)
        {
            var objectsInRegion = CurrentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
            return objectsInRegion.Any() ? objectsInRegion.Where(x => x.Properties.ContainsKey(property.Key) && x.Properties[property.Key].Value == property.Value) : new List<MapObject>();
        }

        public IEnumerable<MapObject> GetObjectsInRegion(string layerName, Rectangle region)
        {
            return CurrentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
        }

        private void LoadMaps(ContentManager content)
        {
            foreach (var info in _mapsInfo)
            {
                _maps.Add(TMXContentProcessor.LoadTMX(info.Path + info.Name, info.ResourcePath, content));
            }
        }
        
        private void PopulateLayerNames()
        {
            var tempIndexVal = 0;

            _layerIndexInfo = new Dictionary<string, int>();
            _layerIndexInfo.Add(DefaultLayerInfo.GROUND_COLLISION, tempIndexVal);
            _layerIndexInfo.Add(DefaultLayerInfo.WATER_COLLISION, tempIndexVal);
            _layerIndexInfo.Add(DefaultLayerInfo.INTERACTABLE_OBJECTS, tempIndexVal);

            if (_mapsInfo[_currentLevelIndex].NonDefaultLayerNames != null)
            {
                foreach (var name in _mapsInfo[_currentLevelIndex].NonDefaultLayerNames)
                {
                    _layerIndexInfo.Add(name, tempIndexVal);
                }
            }
        }
        
        private void AssignObjectLayers()
        {
            for (int i = 0; i < _maps[_currentLevelIndex].ObjectLayers.Count; i++)
            {
                var layerName = _maps[_currentLevelIndex].ObjectLayers[i].Name;
                
                if (_layerIndexInfo.ContainsKey(layerName))
                {
                    _layerIndexInfo[layerName] = i;
                }
            }
        }
    }

    public struct TmxMapInfo
    {
        public string Path;
        public string ResourcePath;
        public string Name;
        public List<string> NonDefaultLayerNames;
        public Point MovingPlatformSize;
    }
}
