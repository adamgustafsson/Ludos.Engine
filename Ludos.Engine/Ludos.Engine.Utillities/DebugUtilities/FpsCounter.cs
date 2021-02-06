namespace Ludos.Engine.Utilities
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class FpsCounter
    {
        private double _msgFrequency = 1.0f;
        private string _msg = string.Empty;

        private double _frames = 0;
        private double _updates = 0;
        private double _elapsed = 0;
        private double _last = 0;
        private double _now = 0;

        /// <summary>
        /// The msgFrequency here is the reporting time to update the message.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            _now = gameTime.TotalGameTime.TotalSeconds;
            _elapsed = (double)(_now - _last);
            if (_elapsed > _msgFrequency)
            {
                _msg = " Fps: " + (_frames / _elapsed).ToString() + "\n Elapsed time: " + _elapsed.ToString() + "\n Updates: " + _updates.ToString() + "\n Frames: " + _frames.ToString();
                _elapsed = 0;
                _frames = 0;
                _updates = 0;
                _last = _now;
            }

            _updates++;
        }

        public void DrawFps(SpriteBatch spriteBatch, SpriteFont font, Vector2 fpsDisplayPosition, Color fpsTextColor)
        {
            spriteBatch.DrawString(font, _msg, fpsDisplayPosition, fpsTextColor);
            _frames++;
        }
    }
}