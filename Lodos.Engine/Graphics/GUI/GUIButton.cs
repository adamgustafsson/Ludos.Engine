using Ludos.Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SD = System.Drawing;

namespace Ludos.Engine.Graphics
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
        public Rectangle Rectangle
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, _size.Width, _size.Height); }
        }

        public override void Update(GameTime gameTime)
        {
            _isHovering = _inputMangager.IsHovering(Rectangle);

            if (_inputMangager.LeftClicked(Rectangle))
                Click?.Invoke(this, new EventArgs());
        }
    }
}
