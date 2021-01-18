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

        public event EventHandler Click;
        public bool Clicked { get; private set; }
        public Color TextColor { get; set; }
        public string Text { get; set; }
        public bool UseFontShading { get; set; }

        public override void Update(GameTime gameTime)
        {
            _isHovering = _inputMangager.IsHovering(Rectangle, Core.LudosGame.GraphicsScale);

            if (_inputMangager.LeftClicked(Rectangle, Core.LudosGame.GraphicsScale))
                Click?.Invoke(this, new EventArgs());
        }
    }
}
