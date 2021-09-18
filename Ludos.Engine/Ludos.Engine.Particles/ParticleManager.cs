namespace Ludos.Engine.Particles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class ParticleManager
    {
        private GraphicsDevice _graphics;
        private Graphics.Camera2D _camera;
        private List<ExplosionParticle> _explosionParticles;
        private Texture2D _pixelParticle;
        private int _particleCountExplosion = 150;
        private int _particleCountFire = 100;
        private Dictionary<Vector2, List<FireParticle>> _fireParticles;

        public ParticleManager(GraphicsDevice graphics, Graphics.Camera2D camera, Dictionary<string, List<Vector2>> firePositions, float scale)
        {
            _graphics = graphics;
            _camera = camera;
            _fireParticles = new Dictionary<Vector2, List<FireParticle>>();
            LoadContent();

            foreach (var particleSystem in firePositions)
            {
                if (particleSystem.Key == typeof(FireParticle).Name)
                {
                    foreach (var position in particleSystem.Value)
                    {
                        var fireParticles = new List<FireParticle>();

                        for (int i = 0; i < _particleCountFire; i++)
                        {
                            fireParticles.Add(new FireParticle(_pixelParticle, position, 1.3f, i, doRespawn: true));
                        }

                        _fireParticles.Add(position, fireParticles);
                    }
                }
            }
        }

        public void Update(float a_elapsedTime)
        {
            foreach (var particleList in _fireParticles.Where(x => _camera.IsOnScreen(x.Key)))
            {
                foreach (FireParticle smokeParticle in particleList.Value)
                {
                    smokeParticle.Update(a_elapsedTime);
                }
            }
        }

        public void Draw(float elapsedTime, SpriteBatch spriteBatch)
        {
            foreach (var particleList in _fireParticles.Where(x => _camera.IsOnScreen(x.Key)))
            {
                foreach (FireParticle smokeParticle in particleList.Value)
                {
                    smokeParticle.Draw(elapsedTime, spriteBatch, _camera);
                }
            }
        }

        public void LoadContent()
        {
            _pixelParticle = Utilities.Utilities.CreateTexture2D(_graphics, new Point(1, 1), Color.White, 1);
        }

    }
}
