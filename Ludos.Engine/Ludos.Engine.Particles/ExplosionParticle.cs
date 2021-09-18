using Microsoft.Xna.Framework;

namespace Ludos.Engine.Particles
{
    public class ExplosionParticle
    {

        public Vector2 m_particlePosition;

        //private float startPositionX = ParticlePositionX.X;

        private Vector2 m_direction;
        private float m_speed = 0.2f;
        private float m_gravitation = -0.01f;


        private float m_size = 2f;//0.005f;


        public ExplosionParticle(Vector2 a_direction, Vector2 a_position, float a_scale)
        {
            m_particlePosition = a_position;
            m_size = m_size * a_scale;
            m_direction.X = a_direction.X;
            m_direction.Y = a_direction.Y;
        }

        public float ParticlePositionX
        {
            get { return m_particlePosition.X; }
            set { m_particlePosition.X = value; }
        }

        public float ParticlePositionY
        {
            get { return m_particlePosition.Y; }
            set { m_particlePosition.Y = value; }
        }

        public float DirectionX
        {
            get { return m_direction.X; }
            set { m_direction.X = value; }
        }

        public float DirectionY
        {
            get { return m_direction.Y; }
            set { m_direction.Y = value; }
        }

        public float Gravitation
        {
            get { return m_gravitation; }
            set { m_gravitation = value; }
        }

        public float GetWidth()
        {
            return m_size;
        }

    }
}
