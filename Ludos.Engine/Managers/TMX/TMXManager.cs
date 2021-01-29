namespace Ludos.Engine.Managers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using FuncWorks.XNA.XTiled;
    using Ludos.Engine.Model.World;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Rectangle = Microsoft.Xna.Framework.Rectangle;

    public class TMXManager
    {
        private readonly List<Map> _maps;
        private readonly List<TMXMapInfo> _mapsInfo;
        private int _currentLevelIndex;
        private Dictionary<string, int> _layerIndexInfo;

        public TMXManager(ContentManager content, List<TMXMapInfo> mapsInfo)
        {
            _maps = new List<Map>();
            _mapsInfo = mapsInfo;

            LoadTmxFiles(content);
            LoadMap(mapsInfo.First().Name);
        }

        public Map CurrentMap { get => _maps[_currentLevelIndex]; }
        public TMXMapInfo CurrentMapInfo { get => _mapsInfo[_currentLevelIndex]; }
        public List<MovingPlatform> MovingPlatforms { get; private set; }

        public void LoadMap(string mapName)
        {
            var map = _mapsInfo.Where(x => x.Name == mapName).FirstOrDefault();
            _currentLevelIndex = _mapsInfo.IndexOf(map);
            PopulateLayerNames();
            AssignObjectLayers();
            LoadMovingPlatforms();
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

        public IEnumerable<MapObject> GetObjectsInRegion(string layerName, RectangleF region)
        {
            return CurrentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
        }

        public IEnumerable<MapObject> GetObjectsInRegion(string layerName, RectangleF region, KeyValuePair<string, string> property)
        {
            var objectsInRegion = CurrentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
            return objectsInRegion.Any() ? objectsInRegion.Where(x => x.Properties.ContainsKey(property.Key) && x.Properties[property.Key].Value == property.Value) : new List<MapObject>();
        }

        public IEnumerable<MapObject> GetObjectsInRegion(string layerName, Rectangle region)
        {
            return CurrentMap.GetObjectsInRegion(_layerIndexInfo[layerName], region);
        }

        private void LoadTmxFiles(ContentManager content)
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

        private void LoadMovingPlatforms()
        {
            MovingPlatforms = new List<MovingPlatform>();

            foreach (var mapObject in CurrentMap.ObjectLayers[DefaultLayerInfo.GROUND_COLLISION].MapObjects.Where(x => x.Polyline != null))
            {
                MovingPlatforms.Add(new MovingPlatform(mapObject.Polyline, _mapsInfo[_currentLevelIndex].MovingPlatformSize));
            }
        }
    }
}
