namespace Ludos.Engine.Graphics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ScrollingTexture
    {
        private readonly List<Texture> _texturePair;
        private Camera2D _camera;
        private bool _constantSpeed;
        private Vector2 _scrollingSpeed;
        private Vector2 _speed;

        public ScrollingTexture(Texture2D texture, Camera2D camera, Vector2 scrollingSpeed, bool constantSpeed = false, float offsetY = 0)
        {
            _camera = camera;

            var textures = new List<Texture2D>() { texture, texture };
            _texturePair = new List<Texture>();

            for (int i = 0; i < textures.Count; i++)
            {
                var t = textures[i];

                _texturePair.Add(new Texture(t)
                {
                    Position = new Vector2((i * t.Width) - Math.Min(i, i + 1), 270 - t.Height + offsetY),
                });
            }

            _scrollingSpeed = scrollingSpeed;
            _constantSpeed = constantSpeed;
        }

        public void Update(GameTime gameTime)
        {
            ApplySpeed(gameTime);
            CheckPosition();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var sprite in _texturePair)
            {
                sprite.Draw(gameTime, spriteBatch);
            }
        }

        private void ApplySpeed(GameTime gameTime)
        {
            _speed.X = _scrollingSpeed.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _speed.Y = _scrollingSpeed.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!_constantSpeed)
            {
                _speed.X *= _camera.Velocity.X;
                _speed.Y *= _camera.Velocity.Y;
            }

            foreach (var sprite in _texturePair)
            {
                sprite.Position -= new Vector2(_speed.X, _speed.Y);
            }
        }

        private void CheckPosition()
        {
            for (int i = 0; i < _texturePair.Count; i++)
            {
                var sprite = _texturePair[i];

                var otherTextureIndex = i - 1;

                if (otherTextureIndex < 0)
                {
                    otherTextureIndex = _texturePair.Count - 1;
                }

                if (sprite.Rectangle.Right <= 0)
                {
                    sprite.Position = new Vector2(_texturePair[otherTextureIndex].Rectangle.Right - (_speed.X * 2), sprite.Position.Y);
                }
                else if (sprite.Rectangle.Left >= sprite.Rectangle.Width)
                {
                    sprite.Position = new Vector2((_texturePair[otherTextureIndex].Rectangle.Left - sprite.Rectangle.Width) - (_speed.X * 2), sprite.Position.Y);
                }
            }
        }
    }
}