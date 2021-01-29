namespace Ludos.Engine.Core
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class LudosGame : Game
    {
        private RenderTarget2D _offScreenRenderTarget;
        private float _aspectRatio;
        private Point _oldWindowSize;

        public LudosGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }

        public LudosGame(int graphicsScale)
        : this()
        {
            GraphicsScale = graphicsScale;
        }

        public static int GraphicsScale { get; set; } = 1;
        public bool GameIsPaused { get; set; }
        public GameState[] GameStates { get; set; }
        protected SpriteBatch SpriteBatch { get; private set; }
        protected GraphicsDeviceManager Graphics { get; }

        public void OnResize(object sender, EventArgs e)
        {
            // Remove this event handler, so we don't call it when we change the window size in here
            Window.ClientSizeChanged -= OnResize;

            if (Window.ClientBounds.Width != _oldWindowSize.X)
            { // We're changing the width
                // Set the new backbuffer size
                Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                Graphics.PreferredBackBufferHeight = (int)(Window.ClientBounds.Width / _aspectRatio);
            }

            if (Window.ClientBounds.Height != _oldWindowSize.Y)
            { // we're changing the height
                // Set the new backbuffer size
                Graphics.PreferredBackBufferWidth = (int)(Window.ClientBounds.Height * _aspectRatio);
                Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            }

            Graphics.ApplyChanges();

            // Update the old window size with what it is currently
            _oldWindowSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

            // add this event handler back
            Window.ClientSizeChanged += OnResize;
        }

        public abstract void ChangeState(int gameStateIndex);

        public abstract void LoadMap(string mapName);

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
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
            SpriteBatch.Begin();
            SpriteBatch.Draw(_offScreenRenderTarget, GraphicsDevice.Viewport.Bounds, Color.White);
            SpriteBatch.End();
            base.EndDraw();
        }
    }
}
