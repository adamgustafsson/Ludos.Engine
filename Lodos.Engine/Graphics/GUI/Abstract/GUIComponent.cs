using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ludos.Engine.Graphics
{
    public abstract class GUIComponent
    {
        public abstract Rectangle Rectangle { get; set; }
        public abstract Vector2 Position { get; set; }
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
