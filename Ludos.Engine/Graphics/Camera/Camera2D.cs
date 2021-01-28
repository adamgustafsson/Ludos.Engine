﻿using Ludos.Engine.Model;
using Ludos.Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ludos.Engine.Graphics
{
    public class Camera2D
    {
        private System.Drawing.RectangleF _cameraBounds;
        private System.Drawing.RectangleF _movementBounds;
        private Viewport _viewPort;
        private LudosPlayer _player;
        private readonly float _scale;


        public Camera2D(GraphicsDevice graphicsDevice, LudosPlayer player, float cameraScale)
        {
            _viewPort = graphicsDevice.Viewport;
            _cameraBounds = new System.Drawing.RectangleF(graphicsDevice.Viewport.Bounds.X, graphicsDevice.Viewport.Bounds.Y, graphicsDevice.Viewport.Bounds.Width, graphicsDevice.Viewport.Bounds.Height);
            _player = player;
            _scale = cameraScale;
            SetupCameraBounds();
        }

        public void Update()
        {

            if (_player.Bounds.Left < _movementBounds.Left)
            {
                _movementBounds.X = _player.Bounds.X;
            }
            else if (_player.Bounds.Right > _movementBounds.Right)
            {
                _movementBounds.X = (_player.Bounds.Right - _movementBounds.Width);
            }

            if (_player.Bounds.Top < _movementBounds.Top)
            {
                _movementBounds.Y = _player.Bounds.Top;
            }
            else if (_player.Bounds.Bottom > _movementBounds.Bottom)
            {
                _movementBounds.Y = (_player.Bounds.Bottom - _movementBounds.Height);
            }

            if (_movementBounds.Center() != _player.Bounds.Center() && _player.Velocity.X == 0)
            {
                var dir = _player.Bounds.Center().X - _movementBounds.Center().X;
                _movementBounds.X += dir * 0.05f;
            }

            var cameraWidth = _cameraBounds.Width / _scale;
            var cameraHeight = _cameraBounds.Height / _scale;
            _cameraBounds.X = _movementBounds.Center().X - (cameraWidth / 2f);
            _cameraBounds.Y = _movementBounds.Center().Y - (cameraHeight / 2f);

            if (_cameraBounds.X < 0)
            {
                _cameraBounds.X = 0;
            }

            if (_cameraBounds.Y < 0)
            {
                _cameraBounds.Y = 0;
            }
        }

        public void SetupCameraBounds()
        {
            var cameraWidth = _cameraBounds.Width / _scale;
            var cameraHeight = _cameraBounds.Height / _scale;

            var cameraCenter = new Vector2(_cameraBounds.Left + cameraWidth / 2f, _cameraBounds.Top + cameraHeight / 2f);
            var cameraCenterVisualized = VisualizeCordinates(cameraCenter);

            var movementBoundsWidth = 75;
            var movementBoundsHeight = 200;

            _movementBounds = new System.Drawing.RectangleF(_player.Bounds.Center().X - (movementBoundsWidth / 2), cameraCenterVisualized.Y - 100, movementBoundsWidth, movementBoundsHeight);
        }

        public System.Drawing.RectangleF CameraBounds { get => _cameraBounds; set => _cameraBounds = value; }
        public System.Drawing.RectangleF MovementBounds { get => _movementBounds; set => _movementBounds = value; }

        public Vector2 VisualizeCordinates(Vector2 cordinates)
        {
            return new Vector2(cordinates.X - _cameraBounds.X, cordinates.Y - _cameraBounds.Y);
        }
        public Vector2 VisualizeCordinates(System.Drawing.RectangleF recF)
        {
            return new Vector2(recF.X - _cameraBounds.X, recF.Y - _cameraBounds.Y);
        }
        public Vector2 VisualizeCordinates(Rectangle rec)
        {
            return new Vector2(rec.X - _cameraBounds.X, rec.Y - _cameraBounds.Y);
        }

        public System.Drawing.RectangleF VisualizeRectangle(System.Drawing.RectangleF recF)
        {
            recF.X -= _cameraBounds.X;
            recF.Y -= _cameraBounds.Y;
            return recF;
        }
        public Rectangle VisualizeRectangle(Rectangle rec)
        {
            rec.X -= _cameraBounds.X.ToInt32();
            rec.Y -= _cameraBounds.Y.ToInt32();
            return rec;
        }

    }
}
