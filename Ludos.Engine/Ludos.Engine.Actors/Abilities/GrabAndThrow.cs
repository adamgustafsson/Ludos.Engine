namespace Ludos.Engine.Actors
{
    using Ludos.Engine.Core;
    using Microsoft.Xna.Framework;
    using static Ludos.Engine.Actors.Actor;

    public class GrabAndThrow : IAbility
    {
        private float _defaultThrowToGrabDelay = 0.25f;
        private float _defaultGrabToThowDelay = 0.5f;
        private float _defaultThrowDuration = 0.5f;
        private float _thorwStateLinger = 0.10f;
        private bool _linger;
        private bool _throwInitiated;
        private Actor _throwingActor;

        public GrabAndThrow()
        {
            ThrowVelocity = new Vector2(250, -100);
            ThrowDuration = _defaultThrowDuration;
            ThrowToGrabDelay = _defaultThrowToGrabDelay;
            GrabToThrowDelay = _defaultGrabToThowDelay;
            ThorwStateLinger = _thorwStateLinger;
        }

        public GrabAndThrow(float throwToGrabDelay, float grabToThrowDelay, float throwDuration, float throwStateLinger)
        {
            _defaultThrowToGrabDelay = throwToGrabDelay;
            _defaultGrabToThowDelay = grabToThrowDelay;
            _defaultThrowDuration = throwDuration;
            _thorwStateLinger = throwStateLinger;
            ThrowToGrabDelay = _defaultThrowToGrabDelay;
            GrabToThrowDelay = _defaultGrabToThowDelay;
            ThorwStateLinger = _thorwStateLinger;
        }

        public Vector2 ThrowVelocity { get; set; }
        public float ThrowDuration { get; set; }
        public float ThrowToGrabDelay { get; set; }
        public float GrabToThrowDelay { get; set; }
        public float ThorwStateLinger { get; set; }
        public GameObject CurrentGrabbedObject { get; set; }
        public bool AbilityEnabled { get; set; } = true;
        public bool AbilityTemporarilyDisabled { get; set; }
        public bool AllowGrabbingMovingObjects { get; set; }
        public bool IsThrowing { get => _throwInitiated || _linger; }

        public void Update(float elapsedTime, Actor actor)
        {
            if (_linger)
            {
                ThorwStateLinger -= elapsedTime;
            }

            if (ThorwStateLinger <= 0)
            {
                _linger = false;
                ThorwStateLinger = _thorwStateLinger;
            }

            if (CurrentGrabbedObject != null)
            {
                CurrentGrabbedObject.Velocity = Vector2.Zero;

                // As default the grabbed object will be placed 1/3 of its width inwards and 1/3 of its height upwards in regards to the players bounds.
                var directionLeftPosition = new Vector2(actor.Position.X + actor.Size.X - (CurrentGrabbedObject.Size.X / 3), actor.Position.Y - (CurrentGrabbedObject.Size.Y / 3));
                var directionRightPosition = new Vector2(actor.Position.X - CurrentGrabbedObject.Size.X + (CurrentGrabbedObject.Size.X / 3), actor.Position.Y - (CurrentGrabbedObject.Size.Y / 3));
                CurrentGrabbedObject.Position = actor.CurrentDirection == Direction.Left ? directionLeftPosition : directionRightPosition;
                GrabToThrowDelay -= elapsedTime;
            }
            else
            {
                ThrowToGrabDelay -= elapsedTime;
            }

            if (_throwInitiated && ThrowDuration <= 0)
            {
                Throw();
                _linger = true;
            }
            else if (_throwInitiated)
            {
                ThrowDuration -= elapsedTime;
            }
        }

        public void ResetAbility()
        {
            ResetThrowToGrabDelay();
            ResetGrabToThrowDelay();
            ResetThrowDelay();
            CurrentGrabbedObject = null;
            _throwInitiated = false;
            _throwingActor = null;
        }

        public void ResetThrowToGrabDelay()
        {
            ThrowToGrabDelay = _defaultThrowToGrabDelay;
        }

        public void ResetThrowDelay()
        {
            ThrowDuration = _defaultThrowDuration;
        }

        public void ResetGrabToThrowDelay()
        {
            GrabToThrowDelay = _defaultGrabToThowDelay;
        }

        public Vector2 GetThrowVelocity(Actor actor)
        {
            return new Vector2(actor.CurrentDirection == Actor.Direction.Left ? actor.Velocity.X - ThrowVelocity.X : actor.Velocity.X + ThrowVelocity.X, ThrowVelocity.Y);
        }

        public void InitiateThrow(Actor throwingActor)
        {
            if (CurrentGrabbedObject != null && GrabToThrowDelay <= 0)
            {
                _throwInitiated = true;
                _throwingActor = throwingActor;
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

        private void Throw()
        {
            CurrentGrabbedObject.Velocity = GetThrowVelocity(_throwingActor);
            ResetAbility();
        }
    }
}