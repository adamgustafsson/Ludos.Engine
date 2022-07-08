﻿namespace Ludos.Engine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
        private LevelManager _levelManager;

        public GameObject(float gravity, Vector2 position, Point size, LevelManager levelManager)
        {
            Gravity = gravity;
            Position = position;
            Size = size;

            _collisionInfo = default;
            _levelManager = levelManager;
        }

        public enum CollisionLayers
        {
            Ground,
            Actors,
            ObjectsAndItems,
        }

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
        }

        public virtual void CalculateTileCollision()
        {
            _collisionInfo = default;
            var collisionRects = _levelManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerWorld, _bounds.Round()).Where(x => x.Type != "platform");

            foreach (var collisionRect in collisionRects)
            {
                _collisionInfo.IsGroundCollision = _lastPosition.Bottom.ToInt32() <= collisionRect.Bounds.Top && _bounds.Bottom.ToInt32() >= collisionRect.Bounds.Top;
                _collisionInfo.IsRoofCollision = _lastPosition.Top.ToInt32() >= collisionRect.Bounds.Bottom && _bounds.Top.ToInt32() < collisionRect.Bounds.Bottom;
                _collisionInfo.IsRightCollision = _lastPosition.Right.ToInt32() <= collisionRect.Bounds.Left && _bounds.Right.ToInt32() >= collisionRect.Bounds.Left;
                _collisionInfo.IsLeftCollision = _lastPosition.Left.ToInt32() >= collisionRect.Bounds.Right && _bounds.Left.ToInt32() <= collisionRect.Bounds.Right;

                if (_collisionInfo.IsGroundCollision && !OnGround)
                {
                     SetGrounded(new PointF(_lastPosition.X, collisionRect.Bounds.Top - _bounds.Height));
                }
                else if (_collisionInfo.IsRoofCollision)
                {
                    _velocity.Y = 0;
                    _bounds.Location = new PointF(_lastPosition.X, collisionRect.Bounds.Bottom);
                }
                else if (_collisionInfo.IsRightCollision)
                {
                    _bounds.X = collisionRect.Bounds.Left - _bounds.Width;
                    ResetVelocity();
                }
                else if (_collisionInfo.IsLeftCollision)
                {
                    _bounds.X = collisionRect.Bounds.Right;
                    _velocity.X = 0;
                    ResetVelocity();
                }
            }

            // If no ordinary collisions are detected - do an additional check with a +1 inflated rectancle in
            // order to determine if the actor is positioned immediately next to a collision.
            if (!collisionRects.Any())
            {
                var colDetectionRect = _bounds;
                colDetectionRect.Inflate(0.2f, 0.2f);
                var collisionRectsInflateOne = _levelManager.GetObjectsInRegion(TMXDefaultLayerInfo.ObjectLayerWorld, colDetectionRect).Where(x => x.Type != "platform");

                if (!collisionRectsInflateOne.Any(x => (x.Bounds.Top == _bounds.Bottom)))
                {
                    OnGround = false;
                }

                var topDetectBound = _bounds;
                topDetectBound.Inflate(-0.2f, 0.2f);
                _collisionInfo.ImidiateTopCollisionExists = collisionRectsInflateOne.Any(x => x.Bounds.Bottom == _bounds.Top.ToInt32() && x.Bounds.ToRectangleF().IntersectsWith(topDetectBound));

                var leftRightDetectBound = _bounds;
                leftRightDetectBound.Inflate(0.2f, -0.2f);

                _collisionInfo.ImidiateRightCollisionExists = collisionRectsInflateOne.Any(x => x.Bounds.Left == _bounds.Right.ToInt32() && x.Bounds.ToRectangleF().IntersectsWith(leftRightDetectBound));
                _collisionInfo.ImidiateLeftCollisionExists = collisionRectsInflateOne.Any(x => x.Bounds.Right == _bounds.Left.ToInt32() && x.Bounds.ToRectangleF().IntersectsWith(leftRightDetectBound));
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