namespace Ludos.Engine.Tmx
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public struct TMXMapInfo
    {
        public string TmxFilePath;
        public string ResourcePath;
        public List<string> NonDefaultLayerNames;
        public Point MovingPlatformSize;
    }
}
