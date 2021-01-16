using Ludos.Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ludos.Engine.Graphics
{
    public class ProceduralButton : GUIButton, ICloneable
    {
        private readonly GraphicsDevice _graphicsDevice;
        public Color ButtonColor { get; set; }
        public Color BorderColor { get; set; }
        public int BorderWidth { get; set; } = 0;
        public float Transparancy { get; set; } = 1;

        public ProceduralButton(GraphicsDevice graphicsDevice, SpriteFont font, InputManager inputManager, Rectangle area)
        {
            _graphicsDevice = graphicsDevice;
            _font = font;
            _inputMangager = inputManager;
            _size = new System.Drawing.Size(area.Width, area.Height);

            Position = new Vector2(area.X, area.Y);
            TextColor = Color.White;
            ButtonColor = Color.DarkBlue;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Texture2D outerLineTexture;

            var color = Color.White;

            if (_isHovering)
                color = Color.Gray;

            if (BorderWidth > 0)
            {
                outerLineTexture = CreateTexture2D(_graphicsDevice, Rectangle.Size, BorderColor, Transparancy);
                var innerTexture = CreateTexture2D(_graphicsDevice, Rectangle.Size - new Point(BorderWidth * 2,BorderWidth * 2), ButtonColor, Transparancy);

                spriteBatch.Draw(outerLineTexture, Position, color);
                spriteBatch.Draw(innerTexture, (Position + new Vector2(BorderWidth, BorderWidth)), color);
            }
            else
            {
                outerLineTexture = CreateTexture2D(_graphicsDevice, Rectangle.Size, ButtonColor, Transparancy);
                spriteBatch.Draw(outerLineTexture, Position, color);
            }

            var textSize = _font.MeasureString(Text);
            var posX = (Rectangle.X + (outerLineTexture.Width / 2)) - (textSize.X / 2);
            var posY = (Rectangle.Y + (outerLineTexture.Height / 2)) - (textSize.Y / 2);

            if (UseFontShading && !_isHovering)
            {
                spriteBatch.DrawString(_font, Text, new Vector2(posX, posY + 1), Color.Black);
            }

            spriteBatch.DrawString(_font, Text, new Vector2(posX, posY), TextColor);
        }

        public object Clone()
        {
            return this.MemberwiseClone() as ProceduralButton;
        }

        private static Texture2D CreateTexture2D(GraphicsDevice graphicsDevice, Point size, Color color, float transparency)
        {
            Texture2D r = new Texture2D(graphicsDevice, size.X, size.Y);
            Color[] data = new Color[size.X * size.Y];
            for (int i = 0; i < data.Length; ++i) data[i] = color * transparency;
            r.SetData(data);
            return r;
        }
    }
}
