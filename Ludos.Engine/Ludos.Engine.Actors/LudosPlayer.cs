namespace Ludos.Engine.Actors
{
    using System.Collections.Generic;
    using System.Linq;
    using FuncWorks.XNA.XTiled;
    using Ludos.Engine.Input;
    using Ludos.Engine.Tmx;
    using Ludos.Engine.Utilities;
    using Microsoft.Xna.Framework;
    using PointF = System.Drawing.PointF;
    using RectangleF = System.Drawing.RectangleF;
    using SizeF = System.Drawing.SizeF;

    public class LudosPlayer : Actor
    {
        private TMXManager _tmxManager;
        private InputManager _inputManager;

        private RectangleF _lastPosition;
        private Vector2 _startPositon;
        private Vector2 _velocity;
        private RectangleF _bounds;

        private bool _imidiateTopCollisionExists;
        private bool _jumpButtonPressedDown = false;
        private bool _jumpInitiated;

        private bool _ladderIsAvailable;
        private MapObject _mostRecentLadder;

        private float _currentAcceleration = 0.001f;
        private bool _onMovingPlatform;

        public LudosPlayer(Vector2 position, Point size, GameServiceContainer services)
            : this(position, size, services.GetService<TMXManager>(), services.GetService<InputManager>())
        {
        }

        public LudosPlayer(Vector2 position, Point size, TMXManager tmxManager, InputManager inputManager)
        {
            Gravity = 600;
            Position = position;
            Size = size;
            Speed = new Vector2(10, _bounds.Y > 16 ? 225 : 200);

            _velocity = new Vector2(0, 0);
            _tmxManager = tmxManager;
            _inputManager = inputManager;
            _startPositon = position;

            Abilities.AddRange(new List<IAbility>() { new WallJump(), new DoubleJump(), new Swimming(Gravity, Speed) });
            //GetAbility<DoubleJump>().AbilityEnabled = false;
            //GetAbility<Swimming>().AbilityEnabled = false;
        }

        public float HorizontalAcceleration { get; set; } = 0.15f;
        public override Vector2 Velocity { get => _velocity; }
        public override RectangleF Bounds { get => _bounds; }
        public override Vector2 Position { get => new Vector2(_bounds.X, _bounds.Y); set => _bounds.Location = new PointF(value.X, value.Y); }
        public override Point Size { set => _bounds.Size = new SizeF(value.X, value.Y); }

        public virtual void Update(float elapsedTime)
        {
            _lastPosition = _bounds;

            var direction = GetDirection();
            Accelerate(ref direction);
            _velocity = CalculateMoveVelocity(_velocity, direction, Speed, elapsedTime);

            var currentPosition = new Vector2(_bounds.X, _bounds.Y);
            currentPosition += _velocity * elapsedTime;

            _bounds.X = currentPosition.X;
            _bounds.Y = currentPosition.Y;

            CalculateCollision();
            CalculateLadderCollision();
            CalculateWaterCollision(elapsedTime);
            CalculateMovingPlatformCollision();
            SetState();
            SetDirection();

            BottomDetectBounds = new RectangleF(_bounds.X, _bounds.Y + (_bounds.Height * 0.90f), _bounds.Width, _bounds.Height * 0.20f);

            var test = GetAbility<Swimming>().IsSubmerged;

            if ((GetAbility<DoubleJump>()?.AbilityEnabled ?? false) && !test)
            {
                if ((CurrentState == State.Jumping || CurrentState == State.Falling) && !GetAbility<DoubleJump>().DoubleJumpUsed)
                {
                    GetAbility<DoubleJump>().DoubleJumpAvailable = true;
                }
            }
        }

        public void ResetToStartPosition()
        {
            Position = _startPositon;
        }

        private void CalculateCollision()
        {
            var collisionRects = _tmxManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerWorld, _bounds.Round()).Where(x => x.Type != "platform");

            foreach (var collisionRect in collisionRects)
            {
                var isGroundCollision = _lastPosition.Bottom.ToInt32() <= collisionRect.Bounds.Top && _bounds.Bottom.ToInt32() >= collisionRect.Bounds.Top;
                var isRoofCollision = _lastPosition.Top.ToInt32() >= collisionRect.Bounds.Bottom && _bounds.Top.ToInt32() < collisionRect.Bounds.Bottom;
                var isRightCollision = _lastPosition.Right.ToInt32() <= collisionRect.Bounds.Left && _bounds.Right.ToInt32() >= collisionRect.Bounds.Left;
                var isLeftCollision = _lastPosition.Left.ToInt32() >= collisionRect.Bounds.Right && _bounds.Left.ToInt32() <= collisionRect.Bounds.Right;

                if (isGroundCollision && !OnGround)
                {
                    SetGrounded(new PointF(_lastPosition.X, collisionRect.Bounds.Top - _bounds.Height));
                }
                else if (isRoofCollision)
                {
                    _velocity.Y = 0;
                    _bounds.Location = new PointF(_lastPosition.X, collisionRect.Bounds.Bottom);

                    GetAbility<WallJump>()?.ResetAbility();
                }
                else if (isRightCollision)
                {
                    _bounds.X = collisionRect.Bounds.Left - _bounds.Width;

                    if ((GetAbility<WallJump>()?.AbilityEnabled ?? false) && !OnGround)
                    {
                        GetAbility<WallJump>().InitiateWallclinging(direction: WallJump.ClingDir.Right);
                        GetAbility<DoubleJump>()?.ResetAbility();
                    }
                }
                else if (isLeftCollision)
                {
                    _bounds.X = collisionRect.Bounds.Right;

                    if ((GetAbility<WallJump>()?.AbilityEnabled ?? false) && !OnGround)
                    {
                        GetAbility<WallJump>().InitiateWallclinging(direction: WallJump.ClingDir.Left);
                        GetAbility<DoubleJump>()?.ResetAbility();
                    }
                }
            }

            // If no ordinary collisions are detected - do an additional check with a +1 inflated rectancle in
            // order to determine if the actor is positioned immediately next to a collision.
            if (!collisionRects.Any())
            {
                var colDetectionRect = _bounds;
                colDetectionRect.Inflate(0.2f, 0.2f);
                var collisionRectsInflateOne = _tmxManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerWorld, colDetectionRect);

                if (!collisionRectsInflateOne.Any(x => (x.Bounds.Top == _bounds.Bottom)))
                {
                    OnGround = false;
                }

                if (GetAbility<WallJump>()?.AbilityEnabled ?? false)
                {
                    if (GetAbility<WallJump>().IsWallClinging && (!collisionRectsInflateOne.Any(x => x.Bounds.Left == _bounds.Right) && !collisionRectsInflateOne.Any(x => x.Bounds.Right == _bounds.Left)))
                    {
                        GetAbility<WallJump>().ResetWallClinging();
                    }
                }

                _imidiateTopCollisionExists = collisionRectsInflateOne.Any(x => (x.Bounds.Bottom == _bounds.Top)); // Object bottom is colliding.
            }
        }

        private void CalculateLadderCollision()
        {
            var ladderDetectionBounds = _bounds;
            ladderDetectionBounds.Inflate(-(_bounds.Width * 0.60f), 0f);

            var onTopOfLadder = false;

            if (_mostRecentLadder != null)
            {
                onTopOfLadder = (ladderDetectionBounds.Right <= _mostRecentLadder.Bounds.Right) &&
                    (ladderDetectionBounds.Left >= _mostRecentLadder.Bounds.Left) &&
                    (_lastPosition.Bottom.ToInt32() <= _mostRecentLadder.Bounds.Top && _bounds.Bottom.ToInt32() >= _mostRecentLadder.Bounds.Top);

                if (onTopOfLadder)
                {
                    SetGrounded(new PointF(_bounds.X, _mostRecentLadder.Bounds.Top - _bounds.Height));
                }
            }

            var ladders = _tmxManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerInteractableObjects, ladderDetectionBounds).Where(x => x.Type == "ladder");
            _ladderIsAvailable = ladders.Any();

            if (!_ladderIsAvailable)
            {
                OnLadder = false;
            }
            else if (OnLadder)
            {
                _mostRecentLadder = ladders.ToList()[0];

                if (!(OnGround && !onTopOfLadder))
                {
                    _bounds.X = _mostRecentLadder.Bounds.X;
                }

                if (onTopOfLadder)
                {
                    _bounds.Y = _bounds.Y + (_bounds.Height / 4);
                }
            }
        }

        private void CalculateWaterCollision(float elapsedTime)
        {
            if ((GetAbility<Swimming>()?.AbilityEnabled ?? false) == false)
            {
                return;
            }

            var water = _tmxManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerWater, _bounds);
            var isCollidingWithWater = water.Any();

            if (!GetAbility<Swimming>().IsInWater && isCollidingWithWater)
            {
                GetAbility<Swimming>().IsInWater = true;
                Speed = new Vector2(Speed.X * 0.85f, Speed.Y);

                if (GetAbility<WallJump>()?.AbilityEnabled ?? false)
                {
                    GetAbility<WallJump>().AbilityTemporarilyDisabled = true;
                }
            }
            else if ((_jumpInitiated && isCollidingWithWater && !GetAbility<Swimming>().IsSubmerged) || (GetAbility<Swimming>().IsInWater && !isCollidingWithWater))
            {
                GetAbility<Swimming>().IsInWater = false;
                Speed = GetAbility<Swimming>().DefaultSpeed;
                GetAbility<DoubleJump>()?.ResetAbility();

                if (GetAbility<WallJump>()?.AbilityTemporarilyDisabled ?? false)
                {
                    GetAbility<WallJump>().AbilityTemporarilyDisabled = false;
                }
            }

            if (GetAbility<Swimming>().IsInWater)
            {
                var waterObjectBounds = water.First().Bounds;
                GetAbility<Swimming>().IsSubmerged = _bounds.Top > waterObjectBounds.Top;

                if (GetAbility<Swimming>().IsDiving)
                {
                    Gravity = 100;
                }
                else if (_bounds.Center().Y > waterObjectBounds.Top)
                {
                    _velocity.Y = 0;
                    Gravity -= elapsedTime * (GetAbility<Swimming>().IsSubmerged ? 2000 : 1000);
                }
                else if (_lastPosition.Y > Position.Y && (_bounds.Center().Y < waterObjectBounds.Top))
                {
                    if (Gravity < 500)
                    {
                        Gravity = 500;
                    }
                    else
                    {
                        Gravity += elapsedTime * 1000;
                    }
                }

                GetAbility<Swimming>().IsDiving = _inputManager.IsInputDown(InputName.ActionButton1);

                if (GetAbility<Swimming>().IsSubmerged && OnGround && !GetAbility<Swimming>().IsDiving)
                {
                    Position = new Vector2(Position.X, Position.Y - 0.85f);
                }
            }
            else
            {
                GetAbility<Swimming>().IsSubmerged = false;
                Gravity = GetAbility<Swimming>().DefaultGravity;
            }

            if (GetAbility<Swimming>().IsSubmerged && (GetAbility<DoubleJump>()?.AbilityEnabled ?? false))
            {
                GetAbility<DoubleJump>().JumpUsedOrCanceled();
            }
        }

        private void CalculateMovingPlatformCollision()
        {
            _onMovingPlatform = false;

            foreach (var mp in _tmxManager.MovingPlatforms)
            {
                var platformBounds = mp.Bounds;

                // The small adjustment at the end is done if the platform is moving vertically in order to ensure collision when
                // doing small jumps on a plafrorm moving upwards.
                var collisionFromAbove = _lastPosition.Bottom <= mp.Bounds.Top + (mp.Change.Y < 0 ? 2 : 0);

                if (platformBounds.Intersects(_bounds))
                {
                    if (mp.Passenger == null && collisionFromAbove)
                    {
                        mp.Passenger = this;
                        GetAbility<WallJump>()?.ResetAbility();
                        GetAbility<DoubleJump>()?.ResetAbility();
                    }
                }
                else if (!platformBounds.Intersects(BottomDetectBounds) && mp.Passenger != null)
                {
                    mp.Passenger = null;
                }

                if (_jumpInitiated)
                {
                    mp.Passenger = null;
                }
            }

            _onMovingPlatform = _tmxManager.MovingPlatforms.Any(x => x.Passenger != null);

            if (_onMovingPlatform)
            {
                _velocity.Y = _velocity.Y > 0 ? 0 : _velocity.Y;
            }
        }

        private void SetGrounded(PointF currentPosition)
        {
            _bounds.Location = currentPosition;
            OnGround = true;
            _velocity.Y = _velocity.Y > 0 ? 0 : _velocity.Y;

            GetAbility<WallJump>()?.ResetAbility();
            GetAbility<DoubleJump>()?.ResetAbility();
        }

        private Vector2 CalculateMoveVelocity(Vector2 linearVelocity, Vector2 direction, Vector2 speed, float elapsedTime)
        {
            var newVelocity = linearVelocity;

            var jumpCanceled = newVelocity.Y < 0 && !_jumpButtonPressedDown && !(GetAbility<WallJump>()?.IsWallJumping ?? false);
            var gravity = jumpCanceled ? Gravity * 3 : Gravity;
            var defaultVelocityY = newVelocity.Y += (OnGround ? 0 : gravity) * elapsedTime;

            if (GetAbility<WallJump>()?.AbilityEnabled ?? false)
            {
                var useDefaultYVelocity = false;
                newVelocity = GetAbility<WallJump>().CalculatVelocity(newVelocity, speed, _jumpInitiated, ref direction, ref _currentAcceleration, ref useDefaultYVelocity, wallJumpVelocity: _bounds.Height > 16 ? 50 : 25);
                newVelocity.Y = useDefaultYVelocity ? defaultVelocityY : newVelocity.Y;
            }
            else
            {
                newVelocity.X = speed.X * direction.X;
                newVelocity.Y = defaultVelocityY;
            }

            // Standard single jump.
            if (_jumpInitiated && !_imidiateTopCollisionExists && !(GetAbility<WallJump>()?.IsWallClinging ?? false))
            {
                newVelocity.Y = speed.Y * direction.Y;
                OnGround = false;
                OnLadder = false;
            }

            if (OnLadder)
            {
                newVelocity.Y = speed.X * direction.Y;

                if (OnGround && newVelocity.Y > 0)
                {
                    newVelocity.Y = 0;
                }
            }

            return newVelocity;
        }

        private Vector2 GetDirection()
        {
            var movingLeft = _inputManager.IsInputDown(InputName.MoveLeft) ? -Speed.X * _currentAcceleration : 0;
            var movingRight = _inputManager.IsInputDown(InputName.MoveRight) ? -Speed.X * _currentAcceleration : 0;

            var climbingUp = _inputManager.IsInputDown(InputName.MoveUp) && _ladderIsAvailable ? -Speed.X : 0;
            var climbingDown = _inputManager.IsInputDown(InputName.MoveDown) && _ladderIsAvailable ? -Speed.X : 0;

            var jumpFromWaterAvailable = (GetAbility<Swimming>()?.AbilityEnabled ?? false) && (GetAbility<Swimming>()?.IsInWater ?? false) && !(GetAbility<Swimming>()?.IsSubmerged ?? false);

            var jumpQueueIsOk = JumpQueueIsOk();
            _jumpInitiated = jumpQueueIsOk && (OnGround || OnLadder || _onMovingPlatform || jumpFromWaterAvailable || (GetAbility<WallJump>()?.IsWallClinging ?? false) || (GetAbility<DoubleJump>()?.DoubleJumpAvailable ?? false));

            if (_jumpInitiated && !OnGround && !OnLadder && !_onMovingPlatform && !(GetAbility<WallJump>()?.IsWallClinging ?? false))
            {
                GetAbility<DoubleJump>()?.JumpUsedOrCanceled();
            }

            var climbingDirection = climbingUp - climbingDown;

            if (!OnLadder && climbingDirection != 0)
            {
                OnLadder = true;
                GetAbility<DoubleJump>()?.ResetAbility();
            }

            var isSubmergedInWateraAndGrounded = OnGround && (GetAbility<Swimming>()?.IsSubmerged ?? false);
            var jumpForce = isSubmergedInWateraAndGrounded ? 0.5f : 1;

            return new Vector2(
                movingLeft - movingRight,
                _jumpInitiated ? -jumpForce : climbingDirection);
        }

        private bool JumpQueueIsOk()
        {
            var jumpAvailable = false;

            if (_inputManager.IsInputDown(InputName.Jump) && !_jumpButtonPressedDown)
            {
                _jumpButtonPressedDown = true;
                jumpAvailable = true;
            }

            if (_inputManager.IsInputUp(InputName.Jump))
            {
                _jumpButtonPressedDown = false;
            }

            return jumpAvailable;
        }

        private void Accelerate(ref Vector2 direction)
        {
            direction.X = direction.X > Speed.X ? Speed.X : direction.X;
            direction.X = direction.X < -Speed.X ? -Speed.X : direction.X;

            var acceleratingRight = direction.X > 0 && _currentAcceleration < 1;
            var acceleratingLeft = direction.X < 0 && _currentAcceleration < 1;

            if (acceleratingRight || acceleratingLeft)
            {
                _currentAcceleration += HorizontalAcceleration;
            }
            else if (direction.X == 0)
            {
                _currentAcceleration = 0.001f;
            }
        }
    }
}