namespace Ludos.Engine.Managers
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public struct TMXMapInfo
    {
        public string Path;
        public string ResourcePath;
        public string Name;
        public List<string> NonDefaultLayerNames;
        public Point MovingPlatformSize;
    }
}
