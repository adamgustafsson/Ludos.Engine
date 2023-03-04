namespace Ludos.Engine.Graphics
{
    using System;
    using Ludos.Engine.Input;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Button : GUIButton, ICloneable
    {
        private readonly Texture2D[] _textures;

        public Button(Texture2D texture, SpriteFont font)
        : this(new Texture2D[] { texture }, font)
        {
        }

        public Button(Texture2D[] textures, SpriteFont font)
             : base(font)
        {
            _textures = textures;
            TextColor = Color.White;
        }

        public override Vector2 Position { get; set; }

        public override Rectangle Rectangle
        {
            get => new Rectangle((int)Position.X, (int)Position.Y, _textures[0].Width, _textures[0].Height);
            set => Rectangle = value;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = Color.White;

            if (IsHovering)
            {
                color = Color.Gray;
            }

            foreach (Texture2D texture in _textures)
            {
                spriteBatch.Draw(texture, Rectangle, color);
            }

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (Font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (Font.MeasureString(Text).Y / 2);

                if (UseFontShading && !IsHovering)
                {
                    spriteBatch.DrawString(Font, Text, new Vector2(x, y + 1), Color.Black);
                }

                spriteBatch.DrawString(Font, Text, new Vector2(x, y), TextColor);
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone() as Button;
        }
    }
}