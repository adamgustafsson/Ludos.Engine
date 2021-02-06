namespace Ludos.Engine.Input
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Size = System.Drawing.Size;

    public class InputManager
    {
        private KeyboardState _keyboardState;
        private KeyboardState _prevKeyboardState;

        private GamePadState _gamepadState;
        private GamePadState _prevGamepadState;
        private bool _gamepadIsConnected;

        private MouseState _mouseState;
        private MouseState _prevMoseState;

        private Size _defaultPreferredBackBuffer;
        private Rectangle _clientBounds;

        public InputManager(Size defaultPreferredBackBuffer)
        {
            _defaultPreferredBackBuffer = defaultPreferredBackBuffer;
            _clientBounds = new Rectangle(0, 0, _defaultPreferredBackBuffer.Width, _defaultPreferredBackBuffer.Height);
        }

        public Dictionary<string, Input> UserControls { get; set; }

        public void Update(Rectangle clientBounds)
        {
            _prevMoseState = _mouseState;
            _mouseState = Mouse.GetState();
            _prevKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();
            _prevGamepadState = _gamepadState;
            _gamepadState = GamePad.GetState(PlayerIndex.One);
            _gamepadIsConnected = _gamepadState.IsConnected;
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

        public KeyboardState GetPreviousKeyboardState()
        {
            return _prevKeyboardState;
        }

        public bool IsInputDown(string inputName)
        {
            var input = UserControls[inputName];
            return _keyboardState.IsKeyDown(input.Key) || (_gamepadIsConnected && _gamepadState.IsButtonDown(input.Button));
        }

        public bool IsInputUp(string inputName)
        {
            var input = UserControls[inputName];
            return _keyboardState.IsKeyUp(input.Key) || (_gamepadIsConnected && _gamepadState.IsButtonUp(input.Button));
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
    }
}
