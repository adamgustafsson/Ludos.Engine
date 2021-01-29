namespace Ludos.Engine.Core
{
    using Ludos.Engine.Managers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class GameState
    {
        public GameState(LudosGame game, GraphicsDevice graphicsDevice, ContentManager content, InputManager inputManager)
        {
            Game = game;
            Graphics = graphicsDevice;
            Content = content;
            InputManager = inputManager;
        }

        protected ContentManager Content { get; }
        protected GraphicsDevice Graphics { get; }
        protected LudosGame Game { get; }
        protected InputManager InputManager { get; }

        public abstract void Update(GameTime gameTime);

        public abstract void PostUpdate(GameTime gameTime);

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
