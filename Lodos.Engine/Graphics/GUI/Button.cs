using Ludos.Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Ludos.Engine.Graphics
{
    public class Button : GUIButton, ICloneable
    {
        private readonly Texture2D[] _textures; 
        public Button(Texture2D texture, SpriteFont font, InputManager inputManager)
        : this (new Texture2D[] {texture}, font, inputManager)  
        {
        }

        public Button(Texture2D[] textures, SpriteFont font, InputManager inputManager)
        {
            _textures = textures;
            _font = font;
            _inputMangager = inputManager;
            _size = new System.Drawing.Size(_textures[0].Width, _textures[0].Height);
            TextColor = Color.White;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var color = Color.White;

            if (_isHovering)
                color = Color.Gray;

            foreach (Texture2D texture in _textures)
                spriteBatch.Draw(texture, Rectangle, color);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                if (UseFontShading && !_isHovering)
                    spriteBatch.DrawString(_font, Text, new Vector2(x, y + 1), Color.Black);

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), TextColor);
            }
        }
        public object Clone()
        {
            return this.MemberwiseClone() as Button;
        }
    }
}