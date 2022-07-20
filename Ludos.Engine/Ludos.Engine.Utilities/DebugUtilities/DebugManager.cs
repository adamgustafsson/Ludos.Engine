namespace Ludos.Engine.Utilities
{
    using System.Collections.Generic;
    using Ludos.Engine.Actors;
    using Ludos.Engine.Graphics;
    using Ludos.Engine.Input;
    using Ludos.Engine.Level;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using RectangleF = System.Drawing.RectangleF;

    public class DebugManager
    {
        private readonly FpsCounter _fpsCounter;
        private readonly SpriteFont _fpsFont;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly InputManager _inputManager;
        private readonly Camera2D _camera;
        private readonly LevelManager _levelManager;
        private readonly LudosPlayer _player;
        private readonly Point _viewport;

        private readonly Texture2D _debugPanelContainer;
        private readonly Texture2D _infoContainer;
        private readonly List<ProceduralTexture> _checkBoxes;

        private bool _drawCollision;
        private bool _drawCameraMovementBounds;
        private bool _drawDebugInfo = true;
        private bool _drawPlayerCollision;

        public DebugManager(GameServiceContainer services, Camera2D camera, LudosPlayer player, SpriteFont debugTextFont)
        {
            _fpsCounter = new FpsCounter();
            _fpsFont = debugTextFont;
            _graphicsDevice = services.GetService<ContentManager>().GetGraphicsDevice();
            _inputManager = services.GetService<InputManager>();
            _levelManager = services.GetService<LevelManager>();
            _camera = camera;
            _player = player;
            _viewport = new Point(_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);

            _debugPanelContainer = Utilities.CreateTexture2D(_graphicsDevice, new Point(265, 89), Color.Black, 0.50f);
            _infoContainer = Utilities.CreateTexture2D(_graphicsDevice, new Point(265, 190), Color.Black, 0.50f);

            var proceduralCheckBox = new ProceduralTexture(_graphicsDevice, new Rectangle(_viewport.X - 36, 12, 19, 16))
            {
                TextureColors = new Color[] { Color.Black },
                Transparancy = 0.7f,
                BorderColor = Color.White,
                BorderTransparancy = 0.7f,
                BorderWidth = 2,
            };

            _checkBoxes = new List<ProceduralTexture>()
            {
                proceduralCheckBox,
                proceduralCheckBox.Clone() as ProceduralTexture,
                proceduralCheckBox.Clone() as ProceduralTexture,
                proceduralCheckBox.Clone() as ProceduralTexture,
            };

            var chkBoxMargin = 19;
            _checkBoxes[1].Position += new Vector2(0, chkBoxMargin);
            _checkBoxes[2].Position += new Vector2(0, chkBoxMargin * 2);
            _checkBoxes[3].Position += new Vector2(0, chkBoxMargin * 3);
        }

        public static void DrawRectancgle(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, int width, int height, Vector2 position, Color? color = null, float transparancy = 1)
        {
            var r = Utilities.CreateTexture2D(graphicsDevice, new Point(width, height), color == null ? Color.White : (Color)color, transparancy);
            spriteBatch.Draw(r, position, Color.White);
        }

        public void DrawRectancgle(SpriteBatch spriteBatch, Rectangle rectangle, Color? color = null, float transparancy = 1, bool visualize = true)
        {
            var position = visualize ? _camera.VisualizeCordinates(rectangle) : new Vector2(rectangle.X, rectangle.Y);
            DrawRectancgle(_graphicsDevice, spriteBatch, rectangle.Width, rectangle.Height, position, color, transparancy);
        }

        public void DrawRectancgle(SpriteBatch spriteBatch, RectangleF rectangle, Color? color = null, float transparancy = 1, bool visualize = true)
        {
            var position = visualize ? _camera.VisualizeCordinates(rectangle) : new Vector2(rectangle.X, rectangle.Y);
            DrawRectancgle(_graphicsDevice, spriteBatch, rectangle.Width.ToInt32(), rectangle.Height.ToInt32(), position, color, transparancy);
        }

        public void Update(GameTime gameTime)
        {
            _fpsCounter.Update(gameTime);
        }

        public void DrawDebugPanel(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var textStartPosX = _viewport.X - 259;
            var textStartPosY = 12;
            var textTopMargin = 19;

            spriteBatch.Draw(_debugPanelContainer, new Vector2(_viewport.X - 269, 5), Color.White);
            spriteBatch.DrawString(_fpsFont, "Show collision", new Vector2(textStartPosX, textStartPosY), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "Show camera movement bounds", new Vector2(textStartPosX, textStartPosY + textTopMargin), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "Show debug info", new Vector2(textStartPosX, textStartPosY + (textTopMargin * 2)), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "Show player collision", new Vector2(textStartPosX, textStartPosY + (textTopMargin * 3)), Color.LightGray);

            if (_inputManager.LeftClicked(_checkBoxes[0].Rectangle))
            {
                _drawCollision = !_drawCollision;
            }

            if (_inputManager.LeftClicked(_checkBoxes[1].Rectangle))
            {
                _drawCameraMovementBounds = !_drawCameraMovementBounds;
            }

            if (_inputManager.LeftClicked(_checkBoxes[2].Rectangle))
            {
                _drawDebugInfo = !_drawDebugInfo;
            }

            if (_inputManager.LeftClicked(_checkBoxes[3].Rectangle))
            {
                _drawPlayerCollision = !_drawPlayerCollision;
            }

            _checkBoxes[0].Transparancy = _drawCollision ? 0.5f : 1;
            _checkBoxes[1].Transparancy = _drawCameraMovementBounds ? 0.5f : 1;
            _checkBoxes[2].Transparancy = _drawDebugInfo ? 0.5f : 1;
            _checkBoxes[3].Transparancy = _drawPlayerCollision ? 0.5f : 1;

            if (_drawDebugInfo)
            {
                DrawDebugInfo(spriteBatch, _player);
            }

            foreach (var chkBox in _checkBoxes)
            {
                chkBox.Draw(gameTime, spriteBatch);
            }
        }

        public void DrawDebugInfo(SpriteBatch spriteBatch, LudosPlayer player)
        {
            var textStartPos = _viewport.X - 259;
            var textStartPosY = 187;
            var textTopMargin = 16;

            spriteBatch.Draw(_infoContainer, new Vector2(_viewport.X - 269, 100), Color.White);
            var vectorStringFormat = "{0}: x {1}, y {2}";

            _fpsCounter.DrawFps(spriteBatch, _fpsFont, new Vector2(_viewport.X - 262, 107f), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "__________________________________________", new Vector2(textStartPos, 167f), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, string.Format(vectorStringFormat, "Velocity", player.Velocity.X.ToString("0.00"), player.Velocity.Y.ToString("0.00")), new Vector2(textStartPos, textStartPosY), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, string.Format(vectorStringFormat, "Position", player.Position.X.ToString("0.00"), player.Position.Y.ToString("0.00")), new Vector2(textStartPos, textStartPosY + textTopMargin), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, string.Format(vectorStringFormat, "Camera velocity", _camera.Velocity.X.ToString("0.00"), _camera.Velocity.Y.ToString("0.00")), new Vector2(textStartPos, textStartPosY + (textTopMargin * 2)), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "State: " + player.CurrentState, new Vector2(textStartPos, textStartPosY + (textTopMargin * 3)), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "Direction: " + player.CurrentDirection, new Vector2(textStartPos, textStartPosY + (textTopMargin * 4)), Color.LightGray);
        }

        public void DrawString(SpriteBatch spriteBatch, string text)
        {
            spriteBatch.DrawString(_fpsFont, text, new Vector2(1661, 293f), Color.LightGray);
        }

        public void DrawScaledContent(SpriteBatch spriteBatch)
        {
            if (_drawCameraMovementBounds)
            {
                DrawRectancgle(spriteBatch, _camera.MovementBounds, transparancy: 0.50f);
            }

            if (_drawCollision)
            {
                _levelManager.DrawObjectLayer(spriteBatch, TMXDefaultLayerInfo.ObjectLayerWorld, _camera.CameraBounds.Round(), 0f);

                foreach (var platform in _levelManager.MovingPlatforms)
                {
                    DrawRectancgle(spriteBatch, platform.DetectionBounds, color: Color.Green);
                }
            }

            if (_drawPlayerCollision)
            {
                DrawRectancgle(spriteBatch, _player.Bounds, color: Color.Blue, transparancy: 0.50f);
                DrawRectancgle(spriteBatch, _player.BottomDetectBounds, color: Color.Red, transparancy: 0.50f);
            }
        }
    }
}
