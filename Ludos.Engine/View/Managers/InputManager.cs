using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ludos.Engine.View
{
    public class InputManager
    {
        private KeyboardState _kbs;
        private KeyboardState _prevKbs;
        private MouseState _mouseState;
        private MouseState _prevMoseState;

        public void SetKeyboardState()
        {
            _prevKbs = _kbs;
            _kbs = Keyboard.GetState();
        }
        public void SetMouseState()
        {
            _prevMoseState = _mouseState;
            _mouseState = Mouse.GetState();
        }

        public void Update()
        {
            SetKeyboardState();
            SetMouseState();
        }

        public bool DidGetTargetedByLeftClick(Rectangle a_target)
        {
            //Om spelare har tryckt på vänster musknapp och släppt.
            if (_prevMoseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
            {
                //Om musens Position/"Triangel" är innanför en motståndare så sätts target.
                if (a_target.Intersects(new Rectangle(_mouseState.X, _mouseState.Y, 1, 1)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
