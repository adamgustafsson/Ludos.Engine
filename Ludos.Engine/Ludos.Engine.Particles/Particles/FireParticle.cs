namespace Ludos.Engine.Particles
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class FireParticle : IParticle
    {
        private static readonly float DelayMax = 0.5f;
        private static readonly float LifetimeMax = 0.7f;
        private static Random _random;

        private readonly Texture2D _texture;
        private readonly bool _isRespawning = false;
        private readonly float _respawnSize;
        private readonly bool _pixelFire = false;

        private float _delay;
        private float _randomYspeed;
        private float _timeLived = 0;
        private Vector2 _position;
        private Vector2 _speed = new Vector2(0, 0);
        private Vector2 _gravity = new Vector2(0, -0.15f);
        private Vector2 _respawnPosition;

        public FireParticle(Texture2D texture, Vector2 startPosition, float scale, int seed, bool doRespawn)
        {
            _random = new Random(seed);

            if (_pixelFire)
            {
                Size *= scale;
                _position = startPosition + new Vector2(_random.Next(-2, 2), 0);
                _delay = (float)_random.NextDouble() * DelayMax;
                _randomYspeed = ((float)_random.NextDouble() - 0.4f) * 50;
                _gravity.X = ((float)_random.NextDouble() - 0.5f) * 0.1f;
                _speed.Y = _randomYspeed < 0 ? _randomYspeed : -_randomYspeed;
            }
            else
            {
                Size *= scale;
                _position = startPosition + new Vector2(_random.Next(-3, 1), 0);
                _delay = (float)_random.NextDouble() * DelayMax / 4;
                _randomYspeed = _random.Next(0, 25);
                _gravity.X = 10;
            }

            _respawnSize = Size;
            _respawnPosition = _position;
            _isRespawning = doRespawn;
            _texture = texture;
        }

        public float Size { get; set; } = 3f;

        public void Update(float a_elapsedTime)
        {
            if (_delay > 0)
            {
                _delay -= a_elapsedTime;
                return;
            }

            if (_isRespawning)
            {
                if (_timeLived > LifetimeMax || Size < 1f)
                {
                    Respawn();
                }
            }

            _timeLived += a_elapsedTime;
            _speed += _gravity * a_elapsedTime;
            _position += _speed * a_elapsedTime;

            float lifePercent = _timeLived / LifetimeMax;
            Size *= 1f - (lifePercent / 8);
        }

        public void Draw(float elapsedTime, SpriteBatch spriteBatch, Graphics.Camera2D camera)
        {
            if (_timeLived > 0)
            {
                var smokePosition = camera.VisualizeCordinates(_position);
                var particleSize = (int)Size;
                var particleRectangle = new Rectangle((int)smokePosition.X, (int)smokePosition.Y, particleSize, particleSize);

                var particleColor = Color.Yellow;
                var rotation = 0f;
                var scale = Size;
                var origin = new Vector2(_texture.Width / 2, _texture.Height / 2);

                if (_pixelFire)
                {
                    spriteBatch.Draw(_texture, particleRectangle, null, particleColor, rotation, origin, SpriteEffects.None, 0.0f);
                }
                else
                {
                    spriteBatch.Draw(_texture, smokePosition, null, particleColor, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }
        }

        public void Respawn()
        {
            Size = _respawnSize;
            _timeLived = 0;
            _position = _respawnPosition;
            _speed = new Vector2(0, _randomYspeed < 0 ? _randomYspeed : -_randomYspeed);
            _delay = (float)_random.NextDouble() * DelayMax;
        }

        public bool IsActive()
        {
            return _timeLived >= 0;
        }
    }
}