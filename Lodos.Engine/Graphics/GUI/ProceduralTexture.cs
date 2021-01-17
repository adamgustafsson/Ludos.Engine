using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Ludos.Engine.Graphics
{
    public class ProceduralTexture : GUIComponent, ICloneable
    {
        private readonly GraphicsDevice _graphicsDevice;
        public Color[] TextureColors { get; set; }
        public Color BorderColor { get; set; }
        public int BorderWidth { get; set; } = 0;
        public float Transparancy { get; set; } = 1;

        public ProceduralTexture(GraphicsDevice graphicsDevice, Rectangle area)
        {
            _graphicsDevice = graphicsDevice;
            _size = new System.Drawing.Size(area.Width, area.Height);

            Position = new Vector2(area.X, area.Y);
        }
        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var size = Rectangle.Size;

            if (BorderWidth > 0)
            {
                var borderTexture = Utilities.Utilities.CreateTexture2D(_graphicsDevice, Rectangle.Size, BorderColor, Transparancy);
                spriteBatch.Draw(borderTexture, Position, Color.White);
                size = Rectangle.Size - new Point(BorderWidth * 2, BorderWidth * 2);
            }

            var textures = new List<Texture2D>();
            
            var newHeight = size.Y / TextureColors.Length;

            foreach (var c in TextureColors)
                textures.Add(Utilities.Utilities.CreateTexture2D(_graphicsDevice, new Point(size.X, newHeight), c, Transparancy));

            var texturePosition = Position + new Vector2(BorderWidth, BorderWidth);

            foreach (var t in textures)
            {
                spriteBatch.Draw(t, texturePosition, Color.White);
                texturePosition += new Vector2(0, (float)newHeight);
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone() as ProceduralButton;
        }
    }
}
