namespace Ludos.Engine.Actors
{
    using Microsoft.Xna.Framework;

    public class GrabObject : IAbility
    {
        public GrabObject()
        {
        }

        public GrabObject(Vector2 throwVelocity, float throwDelay)
        {
            ThrowVelocity = throwVelocity;
            ThrowDelay = throwDelay;
        }

        public Vector2 ThrowVelocity { get; set; } = Vector2.Zero;
        public float ThrowDelay { get; set; } = 0f;
        public bool AbilityEnabled { get; set; } = true;
        public bool AbilityTemporarilyDisabled { get; set; }
        public void ResetAbility()
        {
        }

        public Vector2 GetThrowVelocity(Actor actor)
        {
            return new Vector2(actor.CurrentDirection == Actor.Direction.Left ? actor.Velocity.X - ThrowVelocity.X : actor.Velocity.X + ThrowVelocity.X, ThrowVelocity.Y);
        }
    }
}