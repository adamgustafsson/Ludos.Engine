namespace Ludos.Engine.Core
{
    using Ludos.Engine.Managers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class GameState
    {
        protected ContentManager _content;
        protected GraphicsDevice _graphicsDevice;
        protected LudosGame _game;
        protected InputManager _inputManager;
        public GameState(LudosGame game, GraphicsDevice graphicsDevice, ContentManager content, InputManager inputManager)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _content = content;
            _inputManager = inputManager;
        }
        public abstract void Update(GameTime gameTime);
        public abstract void PostUpdate(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
