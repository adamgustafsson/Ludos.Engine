namespace Ludos.Engine.Actors
{
    using Ludos.Engine.Core;
    using Microsoft.Xna.Framework;
    using static Ludos.Engine.Actors.Actor;

    public class GrabAndThrow : IAbility
    {
        private float _defaultThrowToGrabDelay = 0.25f;
        private float _defaultGrabToThowDelay = 0.5f;

        public GrabAndThrow()
        {
            ThrowVelocity = new Vector2(250, -100);
            ThrowToGrabDelay = _defaultThrowToGrabDelay;
            GrabToThrowDelay = _defaultGrabToThowDelay;
        }

        public GrabAndThrow(float throwToGrabDelay, float grabToThrowDelay)
        {
            _defaultThrowToGrabDelay = throwToGrabDelay;
            _defaultGrabToThowDelay = grabToThrowDelay;
            ThrowToGrabDelay = _defaultThrowToGrabDelay;
            GrabToThrowDelay = _defaultGrabToThowDelay;
        }

        public Vector2 ThrowVelocity { get; set; }
        public float ThrowToGrabDelay { get; set; }
        public float GrabToThrowDelay { get; set; }
        public GameObject CurrentGrabbedObject { get; set; }
        public bool AbilityEnabled { get; set; } = true;
        public bool AbilityTemporarilyDisabled { get; set; }
        public bool AllowGrabbingMovingObjects { get; set; }

        public void Update(float elapsedTime, Actor actor)
        {
            if (CurrentGrabbedObject != null)
            {
                CurrentGrabbedObject.Velocity = Vector2.Zero;
                CurrentGrabbedObject.Position = actor.CurrentDirection == Direction.Left ? actor.Position + new Vector2(7, -2) : actor.Position + new Vector2(-7, -2);
                GrabToThrowDelay -= elapsedTime;
            }
            else
            {
                ThrowToGrabDelay -= elapsedTime;
            }
        }

        public void ResetAbility()
        {
        }

        public void ResetThrowToGrabDelay()
        {
            ThrowToGrabDelay = _defaultThrowToGrabDelay;
        }

        public void ResetGrabToThrowDelay()
        {
            GrabToThrowDelay = _defaultGrabToThowDelay;
        }

        public Vector2 GetThrowVelocity(Actor actor)
        {
            return new Vector2(actor.CurrentDirection == Actor.Direction.Left ? actor.Velocity.X - ThrowVelocity.X : actor.Velocity.X + ThrowVelocity.X, ThrowVelocity.Y);
        }

        public void Throw(Actor throwingActor)
        {
            if (CurrentGrabbedObject != null && GrabToThrowDelay <= 0)
            {
                ResetGrabToThrowDelay();
                CurrentGrabbedObject.Velocity = GetThrowVelocity(throwingActor);
                CurrentGrabbedObject = null;
            }
        }

        public void TryToGrab(GameObject objectToGrab)
        {
            if (CurrentGrabbedObject == null && ThrowToGrabDelay <= 0 && (AllowGrabbingMovingObjects ? true : objectToGrab.Velocity == Vector2.Zero))
            {
                ResetThrowToGrabDelay();
                CurrentGrabbedObject = objectToGrab;
            }
        }
    }
}