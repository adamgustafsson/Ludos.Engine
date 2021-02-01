namespace Ludos.Engine.Model.World
{
    public static class TmxDefaultLayerInfo
    {
        public static string ObjectLayerWorld { get; set; } = "GroundCollision";
        public static string ObjectLayerWater { get; set; } = "Water";
        public static string ObjectLayerInteractableObjects { get; set; } = "Interactable";
        public static string TileLayerForeground { get; set; } = "Foreground";
        public static string TileLayerWorld { get; set; } = "GroundTiles";
        public static string TileLayerBackGround { get; set; } = "Background";
    }
}
