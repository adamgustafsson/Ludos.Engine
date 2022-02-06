namespace Ludos.Engine.Graphics
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RectangleF = System.Drawing.RectangleF;

    public class Texture : TextureComponent
    {
        private Texture2D _texture;
        public Texture(Texture2D texture)
        {
            _texture = texture;
        }

        public float Layer { get; set; } = 0;

        public override Vector2 Position { get; set; }

        public override Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height); }
            set { Rectangle = value; }
        }

        public RectangleF RectangleF
        {
            get { return new RectangleF(Position.X, Position.Y, _texture.Width, _texture.Height); }
            set { RectangleF = value; }
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, Layer);
        }
    }
}
