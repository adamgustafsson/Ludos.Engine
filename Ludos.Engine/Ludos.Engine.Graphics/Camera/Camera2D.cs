namespace Ludos.Engine.Graphics
{
    using Ludos.Engine.Actors;
    using Ludos.Engine.Utilities;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using static Ludos.Engine.Graphics.Camera2D.CameraTransition;
    using RectangleF = System.Drawing.RectangleF;

    public class Camera2D
    {
        private readonly LudosPlayer _player;
        private RectangleF _cameraBounds;
        private RectangleF _movementBounds;
        private Viewport _viewPort;
        private Vector2 _velocity;
        private Vector2 _lastPosition;
        private Vector2 _movementBoundsSizePct;
        private CameraTransition _transition;
        private bool _autoCenteringIsActive = true;

        public Camera2D(GraphicsDevice graphicsDevice, LudosPlayer player, float cameraScale)
        : this(graphicsDevice, player, cameraScale, new Vector2(0.15f, 0.75f))
        {
        }

        public Camera2D(GraphicsDevice graphicsDevice, LudosPlayer player, float cameraScale, Vector2 movementBoundsSizePct)
        {
            _viewPort = graphicsDevice.Viewport;
            _cameraBounds = new RectangleF(graphicsDevice.Viewport.Bounds.X, graphicsDevice.Viewport.Bounds.Y, graphicsDevice.Viewport.Bounds.Width / cameraScale, graphicsDevice.Viewport.Bounds.Height / cameraScale);
            _player = player;
            _movementBoundsSizePct = movementBoundsSizePct;
            SetMovementBounds();
        }

        public RectangleF CameraBounds { get => _cameraBounds; set => _cameraBounds = value; }
        public float MovementBoundsHeight { get => _movementBounds.Height; set => _movementBounds.Height = value; }
        public float MovementBoundsWidth { get => _movementBounds.Width; set => _movementBounds.Width = value; }
        public RectangleF MovementBounds { get => _movementBounds; set => _movementBounds = value; }
        public float? CameraAxisYLock { get; set; } = null;
        public float? CameraAxisXLock { get; set; } = null;
        public CameraTransition Transition { get => _transition; set => _transition = value; }

        public Vector2 Velocity { get => _velocity; set => _velocity = value; }
        public void Update(GameTime gameTime)
        {
            if (_transition.TransitionInProgress)
            {
                _autoCenteringIsActive = false;

                var cameraPos = new Vector2(_cameraBounds.Location.X, _cameraBounds.Location.Y);
                var newPos = Utilities.MoveTowardsPosition(cameraPos, _transition.TransitionDestination, 3f);

                _cameraBounds.X = newPos.X;
                _cameraBounds.Y = newPos.Y;

                if (_transition.Type == CameraTransitionType.VerticalSlide)
                {
                    _transition.TransitionComplete = (_transition.Direction == 1 && cameraPos.Y >= _transition.TransitionDestination.Y) || (_transition.Direction == -1 && cameraPos.Y <= _transition.TransitionDestination.Y);
                }

                _transition.TransitionInProgress = !_transition.TransitionComplete;
                _lastPosition = new Vector2(_cameraBounds.X, _cameraBounds.Y);

                if (_transition.PlayerTransitionDestination.ToPoint() != _player.Position.ToPoint())
                {
                    _player.Position = Utilities.MoveTowardsPosition(_player.Position, _transition.PlayerTransitionDestination, 0.5f);
                }

                return;
            }
            else if (_transition.TransitionComplete)
            {
                _transition = default;
                _player.IsActive = true;

                if (CameraAxisYLock != null)
                {
                    CameraAxisYLock = _cameraBounds.Y;
                }

                return;
            }

            if (_player.Bounds.Left < _movementBounds.Left)
            {
                _movementBounds.X = _player.Bounds.X;
                _autoCenteringIsActive = true;
            }
            else if (_player.Bounds.Right > _movementBounds.Right)
            {
                _movementBounds.X = _player.Bounds.Right - _movementBounds.Width;
                _autoCenteringIsActive = true;
            }

            if (_player.Bounds.Top < _movementBounds.Top)
            {
                _movementBounds.Y = _player.Bounds.Top;
            }
            else if (_player.Bounds.Bottom > _movementBounds.Bottom)
            {
                _movementBounds.Y = _player.Bounds.Bottom - _movementBounds.Height;
            }

            if (Vector2.Distance(_movementBounds.Center(), _player.Bounds.Center()) > 0 && (_player.Velocity.X == 0 || _player.IsDecelerating) && _autoCenteringIsActive)
            {
                var dir = _player.Bounds.Center().X - _movementBounds.Center().X;
                _movementBounds.X += dir * ((float)gameTime.ElapsedGameTime.TotalSeconds * 2);
            }

            _cameraBounds.X = _movementBounds.Center().X - (_cameraBounds.Width / 2f);
            _cameraBounds.Y = CameraAxisYLock == null ? (_movementBounds.Center().Y - (_cameraBounds.Height / 2f)) : (float)CameraAxisYLock;

            if (_cameraBounds.X < 0)
            {
                _cameraBounds.X = 0;
            }

            if (_cameraBounds.Y < 0)
            {
                _cameraBounds.Y = 0;
            }

            _velocity = new Vector2((_cameraBounds.X - _lastPosition.X) / (float)gameTime.ElapsedGameTime.TotalSeconds, (_cameraBounds.Y - _lastPosition.Y) / (float)gameTime.ElapsedGameTime.TotalSeconds);

            // Fixes an issue where velocity becomes infinity on initial loop for some reason.
            _velocity.X = double.IsInfinity(_velocity.X) || double.IsNaN(_velocity.X) ? 0 : _velocity.X;
            _velocity.Y = double.IsInfinity(_velocity.Y) || double.IsNaN(_velocity.Y) ? 0 : _velocity.Y;

            _lastPosition = new Vector2(_cameraBounds.X, _cameraBounds.Y);
        }

        public Vector2 VisualizeCordinates(Vector2 cordinates)
        {
            return new Vector2(cordinates.X - _cameraBounds.X, cordinates.Y - _cameraBounds.Y);
        }

        public Vector2 VisualizeCordinates(RectangleF recF)
        {
            return new Vector2(recF.X - _cameraBounds.X, recF.Y - _cameraBounds.Y);
        }

        public Vector2 VisualizeCordinates(Rectangle rec)
        {
            return new Vector2(rec.X - _cameraBounds.X, rec.Y - _cameraBounds.Y);
        }

        public RectangleF VisualizeRectangle(RectangleF recF)
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

        public bool IsOnScreen(Vector2 cordinates)
        {
            return _cameraBounds.Contains(cordinates.X, cordinates.Y);
        }

        public bool IsOnScreen(RectangleF rec)
        {
            return _cameraBounds.IntersectsWith(rec);
        }

        public void InitiateCameraTransition(CameraTransitionType transType, Rectangle transitionTriggerBounds)
        {
            _player.IsActive = false;
            _transition = default;
            _transition.TransitionInProgress = true;
            _transition.Type = transType;

            if (_transition.Type == CameraTransitionType.VerticalSlide || _transition.Type == CameraTransitionType.HorizontalSlide)
            {
                _transition.Direction = _transition.Type == CameraTransitionType.VerticalSlide ? (_player.Velocity.Y > 0 ? 1 : -1) : (_player.Velocity.X > 0 ? 1 : -1);
            }

            var playerDestination = _player.Velocity.Y < 0 ? new Vector2(_player.Position.X, transitionTriggerBounds.Y - (_player.Bounds.Height + 10)) : new Vector2(_player.Position.X, transitionTriggerBounds.Y + transitionTriggerBounds.Height);
            _transition.PlayerTransitionDestination = playerDestination;

            switch (transType)
            {
                case CameraTransitionType.VerticalSlide:
                    _transition.TransitionDestination = _player.Velocity.Y < 0
                        ? new Vector2(_cameraBounds.X, transitionTriggerBounds.Y - _cameraBounds.Height)
                        : new Vector2(_cameraBounds.X, transitionTriggerBounds.Y + transitionTriggerBounds.Height);
                    break;
            }
        }

        public void SetMovementBounds()
        {
            var movementBoundsWidth = _cameraBounds.Width * _movementBoundsSizePct.X;
            var movementBoundsHeight = _cameraBounds.Height * _movementBoundsSizePct.Y;

            _movementBounds = new RectangleF(_player.Bounds.Center().X - (movementBoundsWidth / 2), _player.Bounds.Center().Y - (movementBoundsHeight / 2), movementBoundsWidth, movementBoundsHeight);
        }

        public struct CameraTransition
        {
            public bool TransitionInProgress;
            public bool TransitionComplete;
            public bool PlayterTransitionInProgress;
            public Vector2 TransitionDestination;
            public Vector2 PlayerTransitionDestination;
            public float Direction;
            public CameraTransitionType Type;

            public enum CameraTransitionType
            {
                VerticalSlide,
                HorizontalSlide,
                Blink,
            }
        }
    }
}
