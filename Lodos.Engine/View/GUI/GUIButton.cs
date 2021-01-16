using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SD = System.Drawing;

namespace Ludos.Engine.View
{
    public abstract class GUIButton : GUIComponent
    {
        protected SpriteFont _font;
        protected InputManager _inputMangager;
        protected bool _isHovering;
        protected SD.Size _size;

        public event EventHandler Click;
        public bool Clicked { get; private set; }
        public Color TextColor { get; set; }
        public Vector2 Position { get; set; }
        public string Text { get; set; }
        public bool UseFontShading { get; set; }
        public int Scale { get; set; }
        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, _size.Width, _size.Height); }
        }

        public override void Update(GameTime gameTime)
        {
            _isHovering = _inputMangager.IsHovering(Rectangle, scale: Scale);

            if (_inputMangager.LeftClicked(Rectangle, scale: Scale))
                Click?.Invoke(this, new EventArgs());
        }
    }
}
