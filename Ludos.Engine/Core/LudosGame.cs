using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Ludos.Engine.Core
{
    public abstract class LudosGame : Game
    {
        public static int GraphicsScale = 1;

        protected GraphicsDeviceManager _graphics;
        protected SpriteBatch _spriteBatch;

        private RenderTarget2D _offScreenRenderTarget;
        private float _aspectRatio;
        private Point _oldWindowSize;

        public bool GameIsPaused { get; set; }
        public GameState[] GameStates { get; set; }

        public LudosGame(int graphicsScale)
        : this()
        {
            GraphicsScale = graphicsScale;
        }
        public LudosGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }
        public void OnResize(Object sender, EventArgs e)
        {
            // Remove this event handler, so we don't call it when we change the window size in here
            Window.ClientSizeChanged -= OnResize;

            if (Window.ClientBounds.Width != _oldWindowSize.X)
            { // We're changing the width
                // Set the new backbuffer size
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = (int)(Window.ClientBounds.Width / _aspectRatio);
            }
            if (Window.ClientBounds.Height != _oldWindowSize.Y)
            { // we're changing the height
                // Set the new backbuffer size
                _graphics.PreferredBackBufferWidth = (int)(Window.ClientBounds.Height * _aspectRatio);
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            }

            _graphics.ApplyChanges();

            // Update the old window size with what it is currently
            _oldWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

            // add this event handler back
            Window.ClientSizeChanged += OnResize;

        }
        public abstract void ChangeState(int gameStateIndex);
        public abstract void LoadMap(string mapName);
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _aspectRatio = GraphicsDevice.Viewport.AspectRatio;
            _oldWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            _offScreenRenderTarget = new RenderTarget2D(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);
        }
        protected override bool BeginDraw()
        {
            GraphicsDevice.SetRenderTarget(_offScreenRenderTarget);
            return base.BeginDraw();
        }
        protected override void EndDraw()
        {
            GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_offScreenRenderTarget, GraphicsDevice.Viewport.Bounds, Color.White);
            _spriteBatch.End();
            base.EndDraw();
        }
    }
}
