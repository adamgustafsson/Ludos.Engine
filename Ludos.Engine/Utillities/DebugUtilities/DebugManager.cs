﻿using Ludos.Engine.Model;
using Ludos.Engine.Model.World;
using Ludos.Engine.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ludos.Engine.Utilities.Debug
{
    public class DebugManager
    {
        private FpsCounter _fpsCounter;
        private SpriteFont _fpsFont;
        private GraphicsDevice _graphicsDevice;
        private InputManager _inputManager;
        private Texture2D _debugPanel;
        private Camera2D _camera;
        private TMXManager _tmxManager;
        private Player _player;

        private bool _drawCollision;
        private bool _drawCameraMovementBounds;
        private bool _drawDebugInfo = true;

        public DebugManager(ContentManager content, GraphicsDevice graphicsDevice, InputManager inputManager, Camera2D camera, TMXManager tmxManager, Player player)
        {
            _fpsCounter = new FpsCounter();
            _fpsFont = content.Load<SpriteFont>("Fonts/Segoe");
            _debugPanel = content.Load<Texture2D>("Assets/Debug/debugpanel");
            _graphicsDevice = graphicsDevice;
            _inputManager = inputManager;
            _camera = camera;
            _tmxManager = tmxManager;
            _player = player;
        }

        public void Update(GameTime gameTime)
        {
            _fpsCounter.Update(gameTime);
        }

        public void DrawRectancgle(SpriteBatch spriteBatch, Rectangle rectangle, Color? color = null, float transparancy = 1, bool visualize = true)
        {
            var position = visualize ? _camera.VisualizeCordinates(rectangle) : new Vector2(rectangle.X, rectangle.Y);
            DrawRectancgle(_graphicsDevice, spriteBatch, rectangle.Width, rectangle.Height, position, color, transparancy);
        }

        public void DrawRectancgle(SpriteBatch spriteBatch, System.Drawing.RectangleF rectangle, Color? color = null, float transparancy = 1, bool visualize = true)
        {
            var position = visualize ? _camera.VisualizeCordinates(rectangle) : new Vector2(rectangle.X, rectangle.Y);
            DrawRectancgle(_graphicsDevice, spriteBatch, rectangle.Width.ToInt32(), rectangle.Height.ToInt32(), position, color, transparancy);
        }

        public static void DrawRectancgle(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, int width, int height, Vector2 position, Color? color = null, float transparancy = 1)
        {
            var r = GenerateScreenRectangle(graphicsDevice, width, height, color == null ? Color.White : (Color)color, transparancy);
            spriteBatch.Draw(r, position, Color.White);
        }

        public void DrawDebugInfo(GameTime gameTime, SpriteBatch spriteBatch, Player player)
        {

            var container = GenerateScreenRectangle(_graphicsDevice, 265, 180, Color.Black, 0.50f);
            //spriteBatch.Draw(container, new Vector2(3,3), Color.White);
            spriteBatch.Draw(container, new Vector2(1651, 100), Color.White);



            _fpsCounter.DrawFps(spriteBatch, _fpsFont, new Vector2(1658f, 107f), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "__________________________________________", new Vector2(1661, 167f), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "Velocity X: " + player.Velocity.X, new Vector2(1661, 187f), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "Velocity Y: " + player.Velocity.Y, new Vector2(1661, 203f), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "Position X: " + player.GetPositionV().X, new Vector2(1661, 218f), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "Position Y: " + player.GetPositionV().Y, new Vector2(1661, 233f), Color.LightGray);
            spriteBatch.DrawString(_fpsFont, "State: " + player.CurrentState, new Vector2(1661, 248f), Color.LightGray);
        }

        public void DrawDebugPanel(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_debugPanel, new Vector2(1651, 5), null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

            var showCollisionChkbox = new Rectangle(1884, 12, 19, 16);
            var showCameraMovementBoundsChkbox = new Rectangle(1884, 31, 19, 16);
            var showDebugInfoChkbox = new Rectangle(1884, 50, 19, 16);

            if (_inputManager.DidGetTargetedByLeftClick(showCollisionChkbox))
                _drawCollision = !_drawCollision;

            if (_inputManager.DidGetTargetedByLeftClick(showCameraMovementBoundsChkbox))
                _drawCameraMovementBounds = !_drawCameraMovementBounds;

            if (_inputManager.DidGetTargetedByLeftClick(showDebugInfoChkbox))
                _drawDebugInfo = !_drawDebugInfo;

            if (_drawDebugInfo)
            {
                DrawRectancgle(spriteBatch, showDebugInfoChkbox, Color.White, transparancy: 0.50f, visualize: false);
                DrawDebugInfo(gameTime, spriteBatch, _player);
            }

            if (_drawCameraMovementBounds)
            {
                DrawRectancgle(spriteBatch, showCameraMovementBoundsChkbox, Color.White, transparancy: 0.50f, visualize: false);
            }

            if (_drawCollision)
            {
                DrawRectancgle(spriteBatch, showCollisionChkbox, Color.White, transparancy: 0.50f, visualize: false);
            }

        }

        public void DrawScaledContent(SpriteBatch spriteBatch)
        {
            if (_drawCameraMovementBounds)
                DrawRectancgle(spriteBatch, _camera.MovementBounds, transparancy: 0.50f);

            if (_drawCollision)
                _tmxManager.CurrentMap.DrawObjectLayer(spriteBatch, 0, Utilities.Round(_camera.CameraBounds), 0f);
        }

        private static Texture2D GenerateScreenRectangle(GraphicsDevice graphicsDevice, int recWidth, int recHeight, Color color, float transparency)
        {
            Texture2D r = new Texture2D(graphicsDevice, recWidth, recHeight);
            Color[] data = new Color[recWidth * recHeight];
            for (int i = 0; i < data.Length; ++i) data[i] = color * transparency;
            r.SetData(data);

            return r;
        }
    }
}
