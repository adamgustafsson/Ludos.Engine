namespace Ludos.Engine.Actors
{
    using System.Diagnostics;
    using Microsoft.Xna.Framework;

    internal class Swimming : IAbility
    {
        public Swimming(float defaultActorGravity, Vector2 defaultActorSpeed)
        {
            DefaultGravity = defaultActorGravity;
            DefaultSpeed = defaultActorSpeed;
            AbilityEnabled = true;
        }

        public bool AbilityEnabled { get; set; }
        public bool IsInWater { get; set; }
        public bool IsSubmerged { get; set; }
        public float DefaultGravity { get; set; }
        public Vector2 DefaultSpeed { get; set; }

        public void ResetAbility()
        {
            IsInWater = false;
            IsSubmerged = false;
        }
    }
}