namespace Ludos.Engine.Input
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Size = System.Drawing.Size;

    public static class InputManager
    {
        private static KeyboardState _keyboardState;
        private static KeyboardState _prevKeyboardState;

        private static GamePadState _gamepadState;
        private static GamePadState _prevGamepadState;
        private static bool _gamepadIsConnected;

        private static MouseState _mouseState;
        private static MouseState _prevMoseState;

        private static Size _defaultPreferredBackBuffer;
        private static Rectangle _clientBounds;

        public static Dictionary<string, Input> UserControls { get; set; }

        public static void Init(Size defaultPreferredBackBuffer)
        {
            _defaultPreferredBackBuffer = defaultPreferredBackBuffer;
            _clientBounds = new Rectangle(0, 0, _defaultPreferredBackBuffer.Width, _defaultPreferredBackBuffer.Height);
        }

        public static void Update(Rectangle clientBounds)
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

        public static bool IsHovering(Rectangle target, int scale = 1)
        {
            var mousePosition = GetMousePosition();
            return target.Intersects(new Rectangle(mousePosition.X / scale, mousePosition.Y / scale, 1, 1));
        }

        public static bool LeftClicked(Rectangle target, int scale = 1)
        {
            return IsHovering(target, scale) && _prevMoseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released;
        }

        public static KeyboardState GetPreviousKeyboardState()
        {
            return _prevKeyboardState;
        }

        public static bool IsInputDown(string inputName)
        {
            var input = UserControls[inputName];
            return _keyboardState.IsKeyDown(input.Key) || (_gamepadIsConnected && _gamepadState.IsButtonDown(input.Button));
        }

        public static bool IsInputUp(string inputName)
        {
            var input = UserControls[inputName];
            return _keyboardState.IsKeyUp(input.Key) || (_gamepadIsConnected && _gamepadState.IsButtonUp(input.Button));
        }

        private static Point GetMousePosition()
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
