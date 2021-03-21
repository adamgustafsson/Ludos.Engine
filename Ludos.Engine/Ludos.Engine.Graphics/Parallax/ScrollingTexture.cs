namespace Ludos.Engine.Graphics
{
    using System;
    using System.Collections.Generic;
    using Ludos.Engine.Actors;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ScrollingTexture
    {
        private Texture2D _texture;
        private Actor _player;
        private List<Texture> _texturePair;
        private Camera2D _camera;
        private bool _constantSpeed;
        private float _scrollingSpeed;
        private float _speed;
        private float _speedY;
        public Vector2 Velocity = Vector2.Zero;
        private bool allowYParallax;

        public ScrollingTexture(Texture2D texture, Actor actor, Camera2D camera, float scrollingSpeed, bool constantSpeed = false)
        {
            _player = actor;
            _camera = camera;

            var textures = new List<Texture2D>() { texture, texture };
            _texturePair = new List<Texture>();

            for (int i = 0; i < textures.Count; i++)
            {
                var t = textures[i];

                _texturePair.Add(new Texture(t)
                {
                    Position = new Vector2((i * t.Width) - Math.Min(i, i + 1), 270 - t.Height),
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

        }

        private void CheckPosition()
        {

        }
    }
}