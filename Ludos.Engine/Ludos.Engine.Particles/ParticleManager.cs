namespace Ludos.Engine.Particles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class ParticleManager
    {
        private GraphicsDevice _graphics;
        private Graphics.Camera2D _camera;
        private Texture2D _pixelParticle;
        private List<ParticleSystemDefinition> _particleSystemDefinitions;
        private List<Dictionary<Vector2, List<IParticle>>> _particleSystems;

        public ParticleManager(GraphicsDevice graphics, Graphics.Camera2D camera, List<ParticleSystemDefinition> particleSystemDefinitions)
        {
            _graphics = graphics;
            _camera = camera;
            _particleSystemDefinitions = particleSystemDefinitions;
            _particleSystems = new List<Dictionary<Vector2, List<IParticle>>>();
            LoadContent();

            foreach (var systemDef in _particleSystemDefinitions.Where(x => x.Type != ParticleSystemType.RenderOnTrigger))
            {
                var particleDict = new Dictionary<Vector2, List<IParticle>>();

                foreach (var position in systemDef.Positions)
                {
                    var particles = new List<IParticle>();

                    for (int i = 0; i < systemDef.Amount; i++)
                    {
                        if (systemDef.ParticleType == typeof(FireParticle))
                        {
                            particles.Add(new FireParticle(_pixelParticle, position, systemDef.Scale, i, doRespawn: systemDef.DoRepeat));
                        }
                        else if (systemDef.ParticleType == typeof(ExplosionParticle))
                        {
                            particles.Add(new ExplosionParticle(_pixelParticle, position, systemDef.Scale));
                        }
                    }

                    particleDict.Add(position, particles);
                 }

                _particleSystems.Add(particleDict);
            }
        }

        public void Update(float elapsedTime)
        {
            foreach (var particleSystem in _particleSystems)
            {
                foreach (var positionAndParticles in particleSystem.Where(x => _camera.IsOnScreen(x.Key)))
                {
                    foreach (IParticle particle in positionAndParticles.Value.Where(x => x.IsActive()))
                    {
                        particle.Update(elapsedTime);
                    }
                }
            }
        }

        public void Draw(float elapsedTime, SpriteBatch spriteBatch)
        {
            foreach (var particleSystem in _particleSystems)
            {
                foreach (var positionAndParticles in particleSystem.Where(x => _camera.IsOnScreen(x.Key)))
                {
                    foreach (IParticle particle in positionAndParticles.Value.Where(x => x.IsActive()))
                    {
                        particle.Draw(elapsedTime, spriteBatch, _camera);
                    }
                }
            }
        }

        public void InitiateParticleSystemAt(Vector2 position, Type particleSystemType)
        {
            var def = _particleSystemDefinitions.Where(x => x.Type == ParticleSystemType.RenderOnTrigger && x.ParticleType == particleSystemType).FirstOrDefault();

            var particles = new List<IParticle>();

            var particleDict = new Dictionary<Vector2, List<IParticle>>();

            for (int i = 0; i < def.Amount; i++)
            {
                if (def.ParticleType == typeof(FireParticle))
                {
                    particles.Add(new FireParticle(_pixelParticle, position, def.Scale, i, doRespawn: def.DoRepeat));
                }
                else if (def.ParticleType == typeof(ExplosionParticle))
                {
                    particles.Add(new ExplosionParticle(_pixelParticle, position, def.Scale));
                }
            }

            particleDict.Add(position, particles);

            _particleSystems.Add(particleDict);
        }

        public void LoadContent()
        {
            _pixelParticle = Utilities.Utilities.CreateTexture2D(_graphics, new Point(1, 1), Color.White, 1);
        }
    }
}
