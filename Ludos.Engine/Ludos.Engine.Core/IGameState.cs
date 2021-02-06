namespace Ludos.Engine.Core
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public interface IGameState
    {
        bool IsActive { get; set; }

        void Update(GameTime gameTime);

        void PostUpdate(GameTime gameTime);

        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
