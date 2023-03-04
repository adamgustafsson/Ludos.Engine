namespace Ludos.Engine.Graphics
{
    using System;
    using Ludos.Engine.Input;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ProceduralButton : GUIButton, ICloneable
    {
        private readonly GraphicsDevice _graphicsDevice;

        public ProceduralButton(GraphicsDevice graphicsDevice, SpriteFont font, Rectangle area)
            : base(font)
        {
            _graphicsDevice = graphicsDevice;
            Rectangle = area;
            TextColor = Color.White;
            ButtonColor = Color.DarkBlue;
        }

        public Color ButtonColor { get; set; }

        public Color BorderColor { get; set; }

        public int BorderWidth { get; set; } = 0;

        public float Transparancy { get; set; } = 1;

        public override Rectangle Rectangle { get; set; }

        public override Vector2 Position
        {
            get => Rectangle.Location.ToVector2();
            set => Rectangle = new Rectangle((int)value.X, (int)value.Y, Rectangle.Width, Rectangle.Height);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D outerLineTexture;

            var color = Color.White;

            if (IsHovering)
            {
                color = Color.Gray;
            }

            if (BorderWidth > 0)
            {
                outerLineTexture = Utilities.Utilities.CreateTexture2D(_graphicsDevice, Rectangle.Size, BorderColor, Transparancy);
                var innerTexture = Utilities.Utilities.CreateTexture2D(_graphicsDevice, Rectangle.Size - new Point(BorderWidth * 2, BorderWidth * 2), ButtonColor, Transparancy);

                spriteBatch.Draw(outerLineTexture, Position, color);
                spriteBatch.Draw(innerTexture, Position + new Vector2(BorderWidth, BorderWidth), color);
            }
            else
            {
                outerLineTexture = Utilities.Utilities.CreateTexture2D(_graphicsDevice, Rectangle.Size, ButtonColor, Transparancy);
                spriteBatch.Draw(outerLineTexture, Position, color);
            }

            var textSize = Font.MeasureString(Text);
            var posX = (Rectangle.X + (outerLineTexture.Width / 2)) - (textSize.X / 2);
            var posY = (Rectangle.Y + (outerLineTexture.Height / 2)) - (textSize.Y / 2);

            if (UseFontShading && !IsHovering)
            {
                spriteBatch.DrawString(Font, Text, new Vector2(posX, posY + 1), Color.Black);
            }

            spriteBatch.DrawString(Font, Text, new Vector2(posX, posY), TextColor);
        }

        public object Clone()
        {
            return this.MemberwiseClone() as ProceduralButton;
        }
    }
}
