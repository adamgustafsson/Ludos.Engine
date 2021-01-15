using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using SD = System.Drawing;

namespace Ludos.Engine.View
{
    public class InputManager
    {
        private KeyboardState _kbs;
        private KeyboardState _prevKbs;
        
        private MouseState _mouseState;
        private MouseState _prevMoseState;
        
        private SD.Size _defaultPreferredBackBuffer;
        private Rectangle _clientBounds;

        public InputManager(SD.Size defaultPreferredBackBuffer)
        {
            _defaultPreferredBackBuffer = defaultPreferredBackBuffer;
        }

        private Point GetMousePosition()
        {
            var screenIsRezised = _clientBounds.Width != _defaultPreferredBackBuffer.Width;

            if (screenIsRezised)
            {
                float rx = (float)_defaultPreferredBackBuffer.Width / (float)_clientBounds.Width;
                float ry = (float)_defaultPreferredBackBuffer.Height / (float)_clientBounds.Height;
                return new Point(Convert.ToInt32(_mouseState.X * rx), Convert.ToInt32(_mouseState.Y * ry));
            }

            return new Point(_mouseState.X, _mouseState.Y);
        }

        public void Update(Rectangle clientBounds)
        {
            _prevMoseState = _mouseState;
            _mouseState = Mouse.GetState();
            _prevKbs = _kbs;
            _kbs = Keyboard.GetState();

            _clientBounds = clientBounds;
        }

        public bool IsHovering(Rectangle target, int scale = 1)
        {
            var mousePosition = GetMousePosition();
            return target.Intersects(new Rectangle(mousePosition.X / scale, mousePosition.Y / scale, 1, 1));
        }

        public bool LeftClicked(Rectangle target, int scale = 1)
        {
            return IsHovering(target, scale) && _prevMoseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released;
        }
    }
}
