namespace Ludos.Engine.Particles
{
    using Ludos.Engine.Graphics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ExplosionParticle : IParticle
    {
        private static readonly float LifetimeMax = 3f;

        private Vector2 _position;
        private Vector2 _direction;
        private float _timeLived = 0;
        private Texture2D _texture;
        private bool _isActive = true;
        private Color _color;
        private bool _useRandomColors = true;
        private bool _useFade = true;

        public ExplosionParticle(Texture2D texture, Vector2 position, float scale)
        {
            _texture = texture;
            _position = position;
            Size *= scale;

            var random = new System.Random();
            var randomDirection = new Vector2((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f);
            randomDirection.Normalize();
            randomDirection *= (float)random.NextDouble();

            _direction = randomDirection;
            _color = _useRandomColors ? new Color(random.Next(256), random.Next(256), random.Next(256)) : Color.White;
        }

        public float Size { get; set; } = 1f;

        public void Update(float elapsedTime)
        {
            _position += _direction;
            _timeLived += elapsedTime;

            if (_timeLived > LifetimeMax)
            {
                _isActive = false;
            }
        }

        public void Draw(float elapsedTime, SpriteBatch spriteBatch, Camera2D camera)
        {
            var particleRadius = Size / 2;
            var particleCoords = camera.VisualizeCordinates(new Vector2(_position.X - particleRadius, _position.Y - particleRadius));
            var origin = new Vector2(_texture.Width / 2, _texture.Height / 2);

            if (_useFade)
            {
                _color *= 0.99f;
            }

            spriteBatch.Draw(_texture, particleCoords, null, _color, 0f, origin, Size, SpriteEffects.None, 0f);
        }

        public bool IsActive()
        {
            return _isActive;
        }

        public void Respawn()
        {
            return;
        }
    }
}
