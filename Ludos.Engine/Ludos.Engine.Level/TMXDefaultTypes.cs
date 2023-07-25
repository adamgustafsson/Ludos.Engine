namespace Ludos.Engine.Level
{
    public static class TMXDefaultTypes
    {
        public static string Platforms { get; set; } = "platform";
        public static string Ladders { get; set; } = "ladder";
        public static string Crates { get; set; } = "crate";
        public static string CameraTransitions { get; set; } = "transition-slide";
        public static string CameraMinXLimit { get; set; } = "camera-min-x-limit";
        public static string CameraMaxXLimit { get; set; } = "camera-max-x-limit";
        public static string CameraMinYLimit { get; set; } = "camera-min-y-limit";
        public static string CameraMaxYLimit { get; set; } = "camera-max-y-limit";
        public static string PlayerStratPosition { get; set; } = "player-start";
    }
}
