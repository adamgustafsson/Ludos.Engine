namespace Ludos.Engine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ludos.Engine.Actors;
    using Ludos.Engine.Level;
    using Ludos.Engine.Utilities;
    using Microsoft.Xna.Framework;
    using PointF = System.Drawing.PointF;
    using RectangleF = System.Drawing.RectangleF;
    using SizeF = System.Drawing.SizeF;

    public class GameObject
    {
        private RectangleF _bounds;
        private Vector2 _velocity;
        private RectangleF _lastPosition;
        private CollisionInformation _collisionInfo;

        public GameObject(float gravity, Vector2 position, Point size, params CollisionLayers[] collidingLayers)
            : this(gravity, position, size)
        {
            CollidingLayers.AddRange(collidingLayers);
        }

        public GameObject(float gravity, Vector2 position, Point size)
        {
            Id = Guid.NewGuid();
            Gravity = gravity;
            Position = position;
            Size = size;

            _collisionInfo = default;
            OnCollision += OnGameObjectIsColliding;
        }

        public event EventHandler OnCollision;

        public enum CollisionLayers
        {
            Ground,
            Actors,
            ObjectsAndItems,
        }

        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsGrabbable { get; set; } = false;
        public bool IsThrowable { get; set; } = false;
        public bool IsBounceable { get; set; } = false;
        public bool IsStationary { get; set; } = false;
        public bool UseDefaultGravity { get; set; } = true;
        public virtual RectangleF Bounds { get => _bounds; }
        public virtual Point Size { set => _bounds.Size = new SizeF(value.X, value.Y); }
        public virtual Vector2 Position { get => new Vector2(_bounds.X, _bounds.Y); set => _bounds.Location = new PointF(value.X, value.Y); }
        public virtual Vector2 Velocity { get => _velocity; set => _velocity = value; }
        public virtual float Gravity { get; set; }
        public virtual bool OnGround { get; set; }
        public virtual RectangleF LastPosition { get => _lastPosition; }
        public CollisionInformation CollisionInfo { get => _collisionInfo; set => _collisionInfo = value; }
        public List<CollisionLayers> CollidingLayers { get; set; } = new List<CollisionLayers>() { CollisionLayers.Ground };
        public List<GameObject> AdditionalCollisionObjects { get; set; } = new List<GameObject>();

        public virtual void Update(float elapsedTime)
        {
            _lastPosition = _bounds;

            var currentPosition = new Vector2(_bounds.X, _bounds.Y);

            if (UseDefaultGravity && !OnGround)
            {
                _velocity.Y += Gravity * elapsedTime;
            }

            currentPosition += _velocity * elapsedTime;

            _bounds.X = currentPosition.X;
            _bounds.Y = currentPosition.Y;

            CalculateTileCollision();

            if (IsStationary && _velocity.X != 0)
            {
                if (_velocity.X > 0)
                {
                    _velocity.X -= elapsedTime * 200;
                    _velocity.X = _velocity.X < 0 ? 0 : _velocity.X;
                }
                else
                {
                    _velocity.X += elapsedTime * 200;
                    _velocity.X = _velocity.X > 0 ? 0 : _velocity.X;
                }

                if (_collisionInfo.IsRightCollision && _velocity.X > 0)
                {
                    _velocity.X = -_velocity.X;
                }
            }
        }

        public virtual void CalculateTileCollision()
        {
            _collisionInfo = default;
            var collisionRects = LevelManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerWorld, _bounds.Round()).Where(x => x.Type != "platform").Select(x => x.Bounds).ToList();

            if (AdditionalCollisionObjects != null)
            {
                collisionRects.AddRange(AdditionalCollisionObjects.Where(x => x.Bounds.IntersectsWith(_bounds)).Select(x => x.Bounds.Round()));
            }

            foreach (var collisionRect in collisionRects)
            {
                _collisionInfo.IsGroundCollision = _lastPosition.Bottom.ToInt32() <= collisionRect.Top && _bounds.Bottom.ToInt32() >= collisionRect.Top;
                _collisionInfo.IsRoofCollision = _lastPosition.Top.ToInt32() >= collisionRect.Bottom && _bounds.Top.ToInt32() < collisionRect.Bottom;
                _collisionInfo.IsRightCollision = _lastPosition.Right.ToInt32() <= collisionRect.Left && _bounds.Right.ToInt32() >= collisionRect.Left;
                _collisionInfo.IsLeftCollision = _lastPosition.Left.ToInt32() >= collisionRect.Right && _bounds.Left.ToInt32() <= collisionRect.Right;

                if (_collisionInfo.IsGroundCollision && !OnGround)
                {
                    if (IsBounceable && _velocity.Y > 45)
                    {
                        _bounds.Location = new PointF(_lastPosition.X, collisionRect.Top - _bounds.Height);
                        OnGround = true;
                        _velocity.Y *= -0.5f;
                    }
                    else
                    {
                        SetGrounded(new PointF(_lastPosition.X, collisionRect.Top - _bounds.Height));
                    }
                }
                else if (_collisionInfo.IsRoofCollision)
                {
                    _velocity.Y = 0;
                    _bounds.Location = new PointF(_lastPosition.X, collisionRect.Bottom);
                }
                else if (_collisionInfo.IsRightCollision)
                {
                    _bounds.X = collisionRect.Left - _bounds.Width;

                    if (IsBounceable)
                    {
                        HorizontalBounce();
                    }
                    else
                    {
                        ResetVelocity();
                    }
                }
                else if (_collisionInfo.IsLeftCollision)
                {
                    _bounds.X = collisionRect.Right;

                    if (IsBounceable)
                    {
                        HorizontalBounce();
                    }
                    else
                    {
                        ResetVelocity();
                    }
                }
            }

            // If no ordinary collisions are detected - do an additional check with a +1 inflated rectancle in
            // order to determine if the actor is positioned immediately next to a collision.
            if (!collisionRects.Any())
            {
                var colDetectionRect = _bounds;
                colDetectionRect.Inflate(0.2f, 0.2f);
                var collisionRectsInflateOne = LevelManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerWorld, colDetectionRect).Where(x => x.Type != "platform").Select(x => x.Bounds).ToList();

                if (AdditionalCollisionObjects != null)
                {
                    collisionRectsInflateOne.AddRange(AdditionalCollisionObjects.Where(x => x.Bounds.IntersectsWith(colDetectionRect)).Select(x => x.Bounds.Round()));
                }

                if (!collisionRectsInflateOne.Any(x => (x.Top == _bounds.Bottom)))
                {
                    OnGround = false;
                }

                var topDetectBound = _bounds;
                topDetectBound.Inflate(-0.2f, 0.2f);
                _collisionInfo.ImidiateTopCollisionExists = collisionRectsInflateOne.Any(x => x.Bottom == _bounds.Top.ToInt32() && x.ToRectangleF().IntersectsWith(topDetectBound));

                var leftRightDetectBound = _bounds;
                leftRightDetectBound.Inflate(0.2f, -0.2f);

                _collisionInfo.ImidiateRightCollisionExists = collisionRectsInflateOne.Any(x => x.Left == _bounds.Right.ToInt32() && x.ToRectangleF().IntersectsWith(leftRightDetectBound));
                _collisionInfo.ImidiateLeftCollisionExists = collisionRectsInflateOne.Any(x => x.Right == _bounds.Left.ToInt32() && x.ToRectangleF().IntersectsWith(leftRightDetectBound));
            }
        }

        public void SetGrounded(PointF currentPosition)
        {
            _bounds.Location = currentPosition;
            OnGround = true;
            _velocity.Y = _velocity.Y > 0 ? 0 : _velocity.Y;
        }

        public void ResetVelocity()
        {
            _velocity.X = 0f;
        }

        public void HorizontalBounce()
        {
            var currentVelocity = _velocity;
            _velocity.X = _velocity.X > 0 ? (_velocity.X * -0.85f) : Math.Abs(_velocity.X * 0.85f);
        }

        public void VericalBounce()
        {
            _velocity.Y = _velocity.Y * -0.5f;
        }

        public void InvokeCollision(GameObject gameObject)
        {
            OnCollision?.Invoke(gameObject, new EventArgs());
        }

        public virtual void OnGameObjectIsColliding(object sender, EventArgs e)
        {
            var collisionObject = sender as GameObject;
        }

        public struct CollisionInformation
        {
            public bool IsGroundCollision;
            public bool IsRoofCollision;
            public bool IsRightCollision;
            public bool IsLeftCollision;
            public bool ImidiateTopCollisionExists;
            public bool ImidiateLeftCollisionExists;
            public bool ImidiateRightCollisionExists;
        }
    }
}
