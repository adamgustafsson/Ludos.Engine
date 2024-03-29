﻿namespace Ludos.Engine.Level
{
    public static class TMXDefaultLayerInfo
    {
        public static string ObjectLayerWorld { get; set; } = "GroundCollision";
        public static string ObjectLayerWater { get; set; } = "Water";
        public static string ObjectLayerInteractableObjects { get; set; } = "Interactable";
        public static string ObjectLayerParticles { get; set; } = "Particles";
        public static string ObjectLayerCamera { get; set; } = "Camera";
        public static string TileLayerForeground { get; set; } = "Foreground";
        public static string TileLayerWorld { get; set; } = "GroundTiles";
        public static string TileLayerBackGround { get; set; } = "Background";
    }
}
