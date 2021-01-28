using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ludos.Engine.Model
{
    public class Actor
    {
        public Vector2 Velocity;
        public RectangleF Bounds;
        public RectangleF BottomDetectBounds;
        protected bool _onGround;

        private Direction _previousDirection;
        public Vector2 Position
        {
            get => new Vector2(Bounds.X, Bounds.Y);
            set => Bounds.Location = new PointF(value.X, value.Y);
        }
        public float Gravity { get; set; }
        public Vector2 Speed { get; set; } = Vector2.Zero;
        public State CurrentState { get; private set; }
        public Direction CurrentDirection { get; private set; } = Direction.Right;
        public bool OnLadder { get; set; }
        public List<IAbility> Abilities { get; set; } = new List<IAbility>();

        public enum State
        {
            Jumping = 0,
            Falling = 1,
            Idle = 3,
            WallClinging = 4,
            //MovingRight = 5,
            //MovingLeft = 6,
            Running = 5,
            Climbing = 6,
            //ClimbingDown = 7,
            ClimbingIdle = 8
        };

        public enum Direction
        {
            Left = 0,
            Right =1
        }

        public void SetState()
        {
            if (Velocity.Y != 0 && OnLadder)
                CurrentState = State.Climbing;
            //else if (Velocity.Y > 0 && OnLadder)
            //    CurrentState = State.ClimbingDown;
            else if (OnLadder && !_onGround)
                CurrentState = State.ClimbingIdle;
            else if (Velocity.Y < 0)
                CurrentState = State.Jumping;
            else if (GetAbility<WallJump>()?.IsWallClinging ?? false)
                CurrentState = State.WallClinging;
            else if (Velocity.Y > 0)
                CurrentState = State.Falling;
            else if (Velocity.X != 0)
                CurrentState = State.Running;
            //else if (Velocity.X < 0)
            //    CurrentState = State.MovingLeft;
            else
                CurrentState = State.Idle;
        }

        public void SetDirection()
        {
            _previousDirection = CurrentDirection;

            if (Velocity.X > 0)
                CurrentDirection = Direction.Right;
            else if (Velocity.X < 0 )
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
