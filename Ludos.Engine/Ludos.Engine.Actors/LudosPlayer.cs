﻿namespace Ludos.Engine.Actors
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using FuncWorks.XNA.XTiled;
    using Ludos.Engine.Core;
    using Ludos.Engine.Input;
    using Ludos.Engine.Level;
    using Ludos.Engine.Utilities;
    using Microsoft.Xna.Framework;
    using PointF = System.Drawing.PointF;
    using RectangleF = System.Drawing.RectangleF;

    public class LudosPlayer : Actor
    {
        private const float INITIALACCELERATION = 0.001f;
        private const float DEFAULTDECELERATIONSPEED = 4.5f;

        private Vector2 _startPositon;
        private Vector2 _prevVelocity;

        private bool _jumpButtonPressedDown = false;
        private bool _jumpInitiated;

        private bool _ladderIsAvailable;
        private bool _onTopOfLadder;
        private bool _onMovingPlatform;
        private MapObject _mostRecentLadder;

        private float _currentAcceleration = INITIALACCELERATION;
        private float _decelerateSpeed = DEFAULTDECELERATIONSPEED;

        public LudosPlayer(Vector2 position, Point size)
            : base(gravity: 600, position, size)
        {
            UseDefaultGravity = false;
            Position = position;
            Size = size;
            Speed = new Vector2(11, Bounds.Size.Height > 16 ? 225 : 200);

            Velocity = new Vector2(0, 0);
            _startPositon = position;
            LevelManager.GlobalGameObjects.Add(this);
        }

        public void ResetToStartPosition()
        {
            Position = _startPositon;
        }

        public override void Update(float elapsedTime)
        {
            if (!IsActive)
            {
                return;
            }

            var direction = GetDirection();
            Accelerate(ref direction);
            var prevVelocity = Velocity;
            Velocity = CalculateMoveVelocity(Velocity, direction, Speed, elapsedTime);
            Decelirate(prevVelocity, direction);

            AdjustVelocityOnPreviousCollision();

            // Basic world collision.
            base.Update(elapsedTime);
            HandleEventsOnCollision();

            CalculateLadderCollision();
            CalculateWaterCollision(elapsedTime);
            CalculateMovingPlatformCollision();
            SetState();
            SetDirection();

            BottomDetectBounds = new RectangleF(Bounds.X, Bounds.Y + (Bounds.Height * 0.90f), Bounds.Width, Bounds.Height * 0.20f);

            if (AbilityIsActive<DoubleJump>())
            {
                if ((CurrentState == State.Jumping || CurrentState == State.Falling) && !GetAbility<DoubleJump>().DoubleJumpUsed)
                {
                    GetAbility<DoubleJump>().DoubleJumpAvailable = true;
                }
            }

            if (AbilityIsActive<GrabAndThrow>())
            {
                GetAbility<GrabAndThrow>().Update(elapsedTime, this);

                if (InputManager.IsInputDown(InputName.ActionButton2))
                {
                    GetAbility<GrabAndThrow>().InitiateThrow(throwingActor: this);
                }
            }

            _prevVelocity = Velocity;
        }

        public override void OnGameObjectIsColliding(object sender, System.EventArgs e)
        {
            var collisionObject = sender as GameObject;

            if (AbilityIsActive<GrabAndThrow>() && collisionObject?.IsGrabbable == true)
            {
                if (InputManager.IsInputDown(InputName.ActionButton2))
                {
                    GetAbility<GrabAndThrow>().TryToGrab(collisionObject);
                }
            }

            base.OnGameObjectIsColliding(sender, e);
        }

        private void AdjustVelocityOnPreviousCollision()
        {
            var changedDirection = (Velocity.X > 0 && _prevVelocity.X < 0) || (Velocity.X < 0 && _prevVelocity.X > 0);
            var rightCollisionNextFrame = CollisionInfo.ImidiateRightCollisionExists && Velocity.X > 0;
            var leftCollisionNextFrame = CollisionInfo.ImidiateLeftCollisionExists && Velocity.X < 0;

            if (changedDirection)
            {
                _currentAcceleration = 0.01f;
            }

            if (((OnGround && !_onTopOfLadder) || GetAbility<Swimming>()?.IsInWater == true) && (rightCollisionNextFrame || leftCollisionNextFrame))
            {
                ResetVelocity();
                _currentAcceleration = INITIALACCELERATION;
            }
        }

        private void HandleEventsOnCollision()
        {
            if (CollisionInfo.IsGroundCollision)
            {
                GetAbility<WallJump>()?.ResetAbility();
                GetAbility<DoubleJump>()?.ResetAbility();
            }
            else if (CollisionInfo.IsRoofCollision)
            {
                GetAbility<WallJump>()?.ResetAbility();
            }
            else if (CollisionInfo.IsRightCollision)
            {
                _currentAcceleration = INITIALACCELERATION;

                if (AbilityIsActive<WallJump>() && !OnGround)
                {
                    GetAbility<WallJump>().InitiateWallclinging(direction: WallJump.ClingDir.Right);
                    GetAbility<DoubleJump>()?.ResetAbility();
                }
            }
            else if (CollisionInfo.IsLeftCollision)
            {
                _currentAcceleration = INITIALACCELERATION;

                if (AbilityIsActive<WallJump>() && !OnGround)
                {
                    GetAbility<WallJump>().InitiateWallclinging(direction: WallJump.ClingDir.Left);
                    GetAbility<DoubleJump>()?.ResetAbility();
                }
            }
            else
            {
                if (AbilityIsActive<WallJump>())
                {
                    if (GetAbility<WallJump>().IsWallClinging && !CollisionInfo.ImidiateLeftCollisionExists && !CollisionInfo.ImidiateRightCollisionExists)
                    {
                        GetAbility<WallJump>().ResetWallClinging();
                    }
                }
            }
        }

        private void CalculateLadderCollision()
        {
            var ladderDetectionBounds = Bounds;
            ladderDetectionBounds.Inflate(-(Bounds.Width * 0.60f), 0f);

            _onTopOfLadder = false;

            if (_mostRecentLadder != null)
            {
                _onTopOfLadder = (ladderDetectionBounds.Right <= _mostRecentLadder.Bounds.Right) &&
                    (ladderDetectionBounds.Left >= _mostRecentLadder.Bounds.Left) &&
                    (LastPosition.Bottom.ToInt32() <= _mostRecentLadder.Bounds.Top && Bounds.Bottom.ToInt32() >= _mostRecentLadder.Bounds.Top);

                if (_onTopOfLadder)
                {
                    SetGrounded(new PointF(Bounds.X, _mostRecentLadder.Bounds.Top - Bounds.Height));
                    GetAbility<WallJump>()?.ResetAbility();
                    GetAbility<DoubleJump>()?.ResetAbility();
                }
            }

            var ladders = LevelManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerInteractableObjects, ladderDetectionBounds).Where(x => x.Type == "ladder");
            _ladderIsAvailable = ladders.Any();

            if (!_ladderIsAvailable)
            {
                OnLadder = false;
            }
            else if (OnLadder)
            {
                _mostRecentLadder = ladders.ToList()[0];

                if (!(OnGround && !_onTopOfLadder))
                {
                    Position = new Vector2(_mostRecentLadder.Bounds.X, Position.Y);
                }

                if (_onTopOfLadder)
                {
                    Position = new Vector2(Position.X, Bounds.Y + (Bounds.Height / 4));
                }
            }
        }

        private void CalculateWaterCollision(float elapsedTime)
        {
            if (!AbilityIsActive<Swimming>())
            {
                return;
            }

            var water = LevelManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerWater, Bounds);
            var velocityRef = Velocity;

            GetAbility<Swimming>().Update(
                this,
                ref velocityRef,
                LastPosition.Y,
                _jumpInitiated,
                water,
                elapsedTime,
                InputManager.IsInputDown(InputName.MoveDown));

            Velocity = velocityRef;
        }

        private void CalculateMovingPlatformCollision()
        {
            _onMovingPlatform = false;

            foreach (var mp in LevelManager.MovingPlatforms)
            {
                var platformBounds = mp.Bounds;

                // The small adjustment at the end is done if the platform is moving vertically in order to ensure collision when
                // doing small jumps on a plafrorm moving upwards.
                var collisionFromAbove = LastPosition.Bottom <= mp.Bounds.Top + (mp.Change.Y < 0 ? 2 : 0);

                if (platformBounds.Intersects(Bounds))
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

            _onMovingPlatform = LevelManager.MovingPlatforms.Any(x => x.Passenger != null);

            if (_onMovingPlatform)
            {
                Velocity = new Vector2(Velocity.X, Velocity.Y > 0 ? 0 : Velocity.Y);
            }
        }

        private Vector2 CalculateMoveVelocity(Vector2 currentVelocity, Vector2 direction, Vector2 speed, float elapsedTime)
        {
            var newVelocity = currentVelocity;

            var jumpCanceled = newVelocity.Y < 0 && !_jumpButtonPressedDown && !(GetAbility<WallJump>()?.IsWallJumping ?? false);
            var gravity = jumpCanceled ? Gravity * 3 : Gravity;
            var defaultVelocityY = newVelocity.Y += (OnGround ? 0 : gravity) * elapsedTime;

            if (AbilityIsActive<WallJump>())
            {
                var useDefaultYVelocity = false;
                newVelocity = GetAbility<WallJump>().CalculatVelocity(newVelocity, speed, _jumpInitiated, ref direction, ref _currentAcceleration, ref useDefaultYVelocity, wallJumpVelocity: Bounds.Height > 16 ? 50 : 25);
                newVelocity.Y = useDefaultYVelocity ? defaultVelocityY : newVelocity.Y;
            }
            else
            {
                newVelocity.X = speed.X * direction.X;
                newVelocity.Y = defaultVelocityY;
            }

            // Standard single jump.
            if (_jumpInitiated && !CollisionInfo.ImidiateTopCollisionExists && !(GetAbility<WallJump>()?.IsWallClinging ?? false))
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
            var movingLeft = InputManager.IsInputDown(InputName.MoveLeft) ? -Speed.X * _currentAcceleration : 0;
            var movingRight = InputManager.IsInputDown(InputName.MoveRight) ? -Speed.X * _currentAcceleration : 0;

            var climbingUp = InputManager.IsInputDown(InputName.MoveUp) && _ladderIsAvailable ? -Speed.X : 0;
            var climbingDown = InputManager.IsInputDown(InputName.MoveDown) && _ladderIsAvailable ? -Speed.X : 0;

            var jumpIsAvailableFromPlatform = OnGround || OnLadder || _onMovingPlatform || (AbilityIsActive<WallJump>() && GetAbility<WallJump>().IsWallClinging);
            var jumpIsAvailableFromWater = AbilityIsActive<Swimming>() && GetAbility<Swimming>().IsInWater && !GetAbility<Swimming>().IsSubmerged;
            var doubleJumpIsAvailable = AbilityIsActive<DoubleJump>() && GetAbility<DoubleJump>().DoubleJumpAvailable;

            var jumpQueueIsOk = JumpQueueIsOk();
            _jumpInitiated = jumpQueueIsOk && (jumpIsAvailableFromPlatform || jumpIsAvailableFromWater || doubleJumpIsAvailable);

            if (_jumpInitiated && !jumpIsAvailableFromPlatform && !jumpIsAvailableFromWater && doubleJumpIsAvailable)
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

            if (InputManager.IsInputDown(InputName.Jump) && !_jumpButtonPressedDown)
            {
                _jumpButtonPressedDown = true;
                jumpAvailable = true;
            }

            if (InputManager.IsInputUp(InputName.Jump))
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
                _currentAcceleration = INITIALACCELERATION;
            }
        }

        private void Decelirate(Vector2 prevVelocity, Vector2 direction)
        {
            // Decelaration - room for improvement
            if (DecelerationIsActive)
            {
                IsDecelerating = direction.X == 0 && prevVelocity.X != 0;

                if (IsDecelerating)
                {
                    var dirRight = prevVelocity.X > 0;
                    var dirLeft = prevVelocity.X < 0;
                    var newVelocity = Velocity;

                    newVelocity.X = dirRight ? prevVelocity.X -= _decelerateSpeed : prevVelocity.X += _decelerateSpeed;

                    if ((dirRight && newVelocity.X <= 0) || (dirLeft && newVelocity.X >= 0))
                    {
                        IsDecelerating = false;
                        _decelerateSpeed = DEFAULTDECELERATIONSPEED;
                        newVelocity.X = 0;
                    }

                    Velocity = newVelocity;
                }
            }
        }
    }
}