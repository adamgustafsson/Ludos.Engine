using System;
using System.Collections.Generic;
using Ludos.Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ludos.Engine.Particles
{
    public class SplitterSystem
    {
        private const int PARTICLES_PER_ARRAY = 150;

        private float m_visability = 1f;
        private List<ExplosionParticle> m_particleArray;
        private ExplosionParticle m_particle;
        private Texture2D m_particleTexture;
        private static Random m_rand = new Random();

        //Konstruktor; skapar ny partikelarray via ArrayKlass
        public SplitterSystem(Vector2 a_explosionPosition, float a_scale)
        {
            m_particleArray = new List<ExplosionParticle>();
            int i = 0;

            while (i < PARTICLES_PER_ARRAY)
            {
                Vector2 m_randomDirection = new Vector2((float)m_rand.NextDouble() - 0.5f, (float)m_rand.NextDouble() - 0.5f);
                m_randomDirection.Normalize();
                m_randomDirection = m_randomDirection * ((float)m_rand.NextDouble());// * 0.005f);

                this.m_particle = new ExplosionParticle(m_randomDirection, a_explosionPosition, a_scale);
                m_particleArray.Add(m_particle);
                i++;
            }

        }

        //Uppdatering/Ritfunktion för Explosionssplitter (Lab2.4)
        public void UpdateAndDrawExplosionSplitter(float a_elapsedTime, SpriteBatch a_spriteBatch, Graphics.Camera2D a_camera)
        {
            foreach (ExplosionParticle particle in m_particleArray)
            {
                particle.ParticlePositionX += particle.DirectionX;
                particle.ParticlePositionY += particle.DirectionY;

                float particleRadius = particle.GetWidth() / 2;

                Vector2 particleCoords = a_camera.VisualizeCordinates(new Vector2(particle.ParticlePositionX - particleRadius, particle.ParticlePositionY - particleRadius));

                // int particleSize = a_camera.VisualizeObject(particleRadius);
                int particleSize = particleRadius.ToInt32();

                int vx = (int)particleCoords.X;
                int vy = (int)particleCoords.Y;
                int vw = particleSize;
                int vh = particleSize;

                Rectangle particleRectangel = new Rectangle(vx, vy, vw, vh);

                float a = m_visability -= a_elapsedTime / 500;
                Color particleColor = new Color(a, a, a, a);

                a_spriteBatch.Draw(m_particleTexture, particleRectangel, particleColor);
            }

        }

        //Uppdatering/Ritfunktion för splitter (Lab2.1)
        public void UpdateAndDrawDefaultSplitter(float a_elapsedTime, Microsoft.Xna.Framework.Graphics.SpriteBatch a_spriteBatch, Graphics.Camera2D a_camera)
        {
            foreach (ExplosionParticle particle in m_particleArray)
            {
                particle.ParticlePositionX += particle.DirectionX;
                particle.ParticlePositionY += particle.DirectionY;
                particle.ParticlePositionY += particle.Gravitation += 0.0005f;

                float particleRadius = particle.GetWidth() / 2;

                Vector2 particleCoords = a_camera.VisualizeCordinates(new Vector2(particle.ParticlePositionX - particleRadius, particle.ParticlePositionY - particleRadius));

                //int particleSize = a_camera.VisualizeObject(particleRadius);
                int particleSize = particleRadius.ToInt32();

                int vx = (int)particleCoords.X;
                int vy = (int)particleCoords.Y;
                int vw = particleSize;
                int vh = particleSize;

                Rectangle particleRectangel = new Rectangle(vx, vy, vw, vh);

                //Ingen fade
                float a = 1f;
                Color particleColor = new Color(a, a, a, a);

                a_spriteBatch.Draw(m_particleTexture, particleRectangel, particleColor);
            }

        }

        //Laddar textur
        public void LoadContent(GraphicsDevice graphics)
        {
            //m_particleTexture = a_content.Load<Texture2D>(a_textureSrc);
            m_particleTexture = Utilities.Utilities.CreateTexture2D(graphics, new Point(2, 2), Color.White, 1);
        }
    }
}
