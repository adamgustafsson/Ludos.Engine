﻿namespace Ludos.Engine.Actors
{
    using System.Collections.Generic;
    using System.Linq;
    using Ludos.Engine.Core;
    using Ludos.Engine.Level;
    using Microsoft.Xna.Framework;
    using RectangleF = System.Drawing.RectangleF;

    public abstract class Actor : GameObject
    {
        private Direction _previousDirection;

        public Actor(float gravity, Vector2 position, Point size)
            : base(gravity, position, size)
        {
        }

        public enum State
        {
            Jumping,
            JumpingAndCarrying,
            Falling,
            FallingAndCarrying,
            Idle,
            IdleAndCarrying,
            WallClinging,
            Running,
            RunningAndCarrying,
            Climbing,
            ClimbingIdle,
            Swimming,
            Diving,
            Throwing,
        }

        public enum Direction
        {
            Left = 0,
            Right = 1,
        }

        public RectangleF BottomDetectBounds { get; set; }
        public Vector2 Speed { get; set; } = Vector2.Zero;
        public State CurrentState { get; private set; }
        public State PreviousState { get; private set; }
        public Direction CurrentDirection { get; private set; } = Direction.Right;
        public List<IAbility> Abilities { get; set; } = new List<IAbility>();
        public float HorizontalAcceleration { get; set; } = 0.15f;
        public bool DecelerationIsActive { get; set; } = true;
        public bool IsDecelerating { get; set; }
        protected bool OnLadder { get; set; }

        public void SetState()
        {
            PreviousState = CurrentState;
            var isCarryingObject = GetAbility<GrabAndThrow>()?.CurrentGrabbedObject != null;

            if (Velocity.Y != 0 && OnLadder)
            {
                CurrentState = State.Climbing;
            }
            else if (OnLadder && !OnGround)
            {
                CurrentState = State.ClimbingIdle;
            }
            else if ((GetAbility<Swimming>()?.IsInWater ?? false) && Velocity.Y >= 0)
            {
                CurrentState = GetAbility<Swimming>().IsSubmerged ? State.Diving : State.Swimming;
            }
            else if (GetAbility<WallJump>()?.IsWallClinging ?? false)
            {
                CurrentState = State.WallClinging;
            }
            else if (GetAbility<GrabAndThrow>()?.IsThrowing ?? false)
            {
                CurrentState = State.Throwing;
            }
            else if (Velocity.Y < 0)
            {
                CurrentState = isCarryingObject ? State.JumpingAndCarrying : State.Jumping;
            }
            else if (Velocity.Y > 0)
            {
                CurrentState = isCarryingObject ? State.FallingAndCarrying : State.Falling;
            }
            else if (Velocity.X != 0)
            {
                CurrentState = isCarryingObject ? State.RunningAndCarrying : State.Running;
            }
            else
            {
                CurrentState = isCarryingObject ? State.IdleAndCarrying : State.Idle;
            }
        }

        public void SetDirection()
        {
            _previousDirection = CurrentDirection;

            if (Velocity.X > 0)
            {
                CurrentDirection = Direction.Right;
            }
            else if (Velocity.X < 0)
            {
                CurrentDirection = Direction.Left;
            }
            else if (Velocity.X == 0)
            {
                CurrentDirection = _previousDirection;
            }
            else
            {
                CurrentDirection = Direction.Right;
            }
        }

        public T GetAbility<T>()
        {
            return (T)Abilities.Where(x => x.GetType() == typeof(T) && ((x.AbilityTemporarilyDisabled && !x.AbilityEnabled) || x.AbilityEnabled)).FirstOrDefault();
        }

        public bool AbilityIsActive<T>()
        {
            var ability = GetAbility<T>();
            return ability != null && (ability as IAbility).AbilityEnabled;
        }

        public void TemporarilyDisabledAbility<T>()
        {
            var ability = GetAbility<T>();

            if (ability != null && (ability as IAbility).AbilityEnabled)
            {
                (ability as IAbility).AbilityTemporarilyDisabled = true;
                (ability as IAbility).ResetAbility();
                (ability as IAbility).AbilityEnabled = false;
            }
        }

        public void EnableTemporarilyDisabledAbility<T>()
        {
            var ability = GetAbility<T>();

            if (ability != null && (ability as IAbility).AbilityTemporarilyDisabled)
            {
                (ability as IAbility).AbilityEnabled = true;
                (ability as IAbility).AbilityTemporarilyDisabled = false;
            }
        }
    }
}
