﻿namespace Ludos.Engine.Graphics
{
    using System;
    using Ludos.Engine.Input;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class GUIButton : TextureComponent
    {
        public GUIButton(SpriteFont font)
        {
            Font = font;
        }

        public event EventHandler Click;
        public bool Clicked { get; private set; }
        public bool UseFontShading { get; set; }
        public string Text { get; set; }
        public Color TextColor { get; set; }

        protected SpriteFont Font { get; }
        protected bool IsHovering { get; private set; }

        public override void Update(GameTime gameTime)
        {
            IsHovering = InputManager.IsHovering(Rectangle, Core.LudosGame.GraphicsScale);

            if (InputManager.LeftClicked(Rectangle, Core.LudosGame.GraphicsScale))
            {
                Click?.Invoke(this, new EventArgs());
            }
        }
    }
}
