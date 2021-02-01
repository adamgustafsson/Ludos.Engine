namespace Ludos.Engine.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using RectangleF = System.Drawing.RectangleF;

    public abstract class Actor
    {
        private Direction _previousDirection;

        public enum State
        {
            Jumping = 0,
            Falling = 1,
            Idle = 3,
            WallClinging = 4,
            Running = 5,
            Climbing = 6,
            ClimbingIdle = 8,
        }

        public enum Direction
        {
            Left = 0,
            Right = 1,
        }

        public abstract Vector2 Position { get; set; }
        public abstract Vector2 Velocity { get; }
        public abstract RectangleF Bounds { get; }
        public abstract Point Size { set; }

        public RectangleF BottomDetectBounds { get; set; }
        public Vector2 Speed { get; set; } = Vector2.Zero;
        public State CurrentState { get; private set; }
        public Direction CurrentDirection { get; private set; } = Direction.Right;
        public float Gravity { get; set; }
        public bool OnLadder { get; set; }
        public List<IAbility> Abilities { get; set; } = new List<IAbility>();
        protected bool OnGround { get; set; }

        public void SetState()
        {
            if (Velocity.Y != 0 && OnLadder)
                CurrentState = State.Climbing;
            else if (OnLadder && !OnGround)
                CurrentState = State.ClimbingIdle;
            else if (Velocity.Y < 0)
                CurrentState = State.Jumping;
            else if (GetAbility<WallJump>()?.IsWallClinging ?? false)
                CurrentState = State.WallClinging;
            else if (Velocity.Y > 0)
                CurrentState = State.Falling;
            else if (Velocity.X != 0)
                CurrentState = State.Running;
            else
                CurrentState = State.Idle;
        }

        public void SetDirection()
        {
            _previousDirection = CurrentDirection;

            if (Velocity.X > 0)
                CurrentDirection = Direction.Right;
            else if (Velocity.X < 0)
                CurrentDirection = Direction.Left;
            else if (Velocity.X == 0)
                CurrentDirection = _previousDirection;
            else
                CurrentDirection = Direction.Right;
        }

        public T GetAbility<T>()
        {
            return (T)Abilities.Where(x => x.GetType() == typeof(T) && x.AbilityEnabled == true).FirstOrDefault();
        }
    }
}
