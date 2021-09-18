namespace Ludos.Engine.Particles
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class FireParticle : IParticle
    {
        private static readonly float DelayMax = 1f;
        private static readonly float LifetimeMax = 1f; 
        private static Random _random;

        private float _delay;
        private Vector2 _position;
        private float _randomYspeed;
        private float _timeLived = 0;
        private Vector2 _speed = new Vector2(0, 0);
        private Vector2 _gravity = new Vector2(0, -0.15f);
        private bool _isRespawning = false;
        private float _respawnSize;
        private Vector2 _respawnPosition;
        private Texture2D _texture;

        public FireParticle(Texture2D texture, Vector2 startPosition, float scale, int seed, bool doRespawn)
        {
            _random = new Random(seed);

            _position = startPosition + new Vector2(_random.Next(-2, 2), 0);
            GetSize *= scale;
            _delay = (float)_random.NextDouble() * DelayMax;
            _randomYspeed = ((float)_random.NextDouble() - 0.4f) * 50;
            _gravity.X = ((float)_random.NextDouble() - 0.5f) * 0.1f;
            _speed.Y = _randomYspeed < 0 ? _randomYspeed : -_randomYspeed;

            _respawnSize = GetSize;
            _respawnPosition = _position;
            _isRespawning = doRespawn;

            _texture = texture;
        }

        public float GetSize { get; set; } = 3f;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float Size
        {
            get { return GetSize; }
            set { GetSize = value; }
        }

        public void Update(float a_elapsedTime)
        {
            if (_delay > 0)
            {
                _delay -= a_elapsedTime;
                return;
            }

            if (_isRespawning)
            {
                if (_timeLived > LifetimeMax)
                {
                    Respawn();
                }
            }

            _timeLived += a_elapsedTime;
            _speed = _speed + (_gravity * a_elapsedTime);
            _position = _position + (_speed * a_elapsedTime);

            float lifePercent = _timeLived / LifetimeMax;
            GetSize *= 1f - (lifePercent / 8);
        }

        public void Draw(float elapsedTime, SpriteBatch spriteBatch, Graphics.Camera2D camera)
        {
            if (IsActive())
            {
                Vector2 smokePosition = camera.VisualizeCordinates(Position);
                int particleSize = (int)GetSize;

                int vx = (int)smokePosition.X;
                int vy = (int)smokePosition.Y;
                int vw = particleSize;
                int vh = particleSize;

                Rectangle particleRectangle = new Rectangle(vx, vy, vw, vh);

                //float a = GetVisibility() / 2;
                //Color fadecolor = new Color(a, a, a, a);
                var particleColor = Color.Yellow;
                float rotation = 0;
                float scale = GetSize;
                Vector2 origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
                spriteBatch.Draw(_texture, particleRectangle, null, particleColor, rotation, origin, SpriteEffects.None, 0.0f);
            }
        }

        public void Respawn()
        {
            _timeLived = 0;
            GetSize = _respawnSize;
            _position = _respawnPosition;
            _speed = new Vector2(0, _randomYspeed < 0 ? _randomYspeed : -_randomYspeed);
        }

        public float GetVisibility()
        {
            return 0.3f / _timeLived;
        }

        public bool IsActive()
        {
            return _timeLived > 0;
        }

        public void GetVisability()
        {
            throw new NotImplementedException();
        }
    }
}






















    //    //Constants
    //    public static Vector2 DEFAULT_POSITION = new Vector2(Model.Level.LEVEL_WIDTH / 2, Model.Level.LEVEL_HEIGHT / 1.1f);
    //    public static float MAX_LIFETIMES = 1f;
    //    public static float MAX_DELAY = 5f;
       
    //    //State
    //    private float m_delay;
    //    private float m_lifeLived;
    //    private float m_size = (Model.Level.LEVEL_WIDTH + Model.Level.LEVEL_HEIGHT) / 20;
        
    //    private Vector2 m_position;
    //    private Vector2 m_particleSpeed = new Vector2(0, 0f);    
    //    private Vector2 m_gravity = new Vector2(0, -1f);    



    //    public SmokeParticle(Vector2 a_randomXgravitation, int a_randomSeed)
    //    {
    //        m_position = DEFAULT_POSITION;
    //        m_lifeLived = 0;
    //        //m_gravity.X = a_randomXgravitation.X;

    //        Random r = new Random(a_randomSeed);
    //        m_delay = (float)r.NextDouble() * MAX_DELAY;
    //    }

    //    private void Respawn(int a_randomSeed)
    //    {       
    //        m_lifeLived = 0;
    //        m_position = DEFAULT_POSITION;
    //        m_particleSpeed = new Vector2(0.0f, -1f);

    //    }

    //    public bool IsAlive()
    //    {
    //        return m_lifeLived > 0;
    //    }

    //    internal void Update(float a_elapsedTime, int a_randomSeed)
    //    {
    //        //if (m_delay > 0)
    //        //{
    //        //    m_delay -= a_elapsedTime;
    //        //    return;
    //        //}
            
    //        //v1 = v0 + a *t
    //        m_particleSpeed = m_particleSpeed + m_gravity * a_elapsedTime;

    //        //s1 = s0 + var * t
    //        m_position = m_position + m_particleSpeed * a_elapsedTime;

    //        m_lifeLived += a_elapsedTime;


    //        //Ökar storleken på röken
    //        float lifePercent = m_lifeLived / MAX_LIFETIMES;
    //        m_size = 0.1f + lifePercent * 10f;

    //        //Respawnar om lifeLived överstiger 1
    //        if (m_lifeLived > MAX_LIFETIMES)
    //        {
    //            if (m_delay > 0)
    //            {
    //                m_delay -= a_elapsedTime;
    //                return;
    //            }
    //            Respawn(a_randomSeed);
    //        }




    //    }

        
    //    public float GetVisibility()
    //    {
    //        return m_lifeLived * MAX_LIFETIMES;
    //    }

    //    public float GetPositionX
    //    {
    //        get { return m_position.X; }
    //        set { m_position.X = value; }
    //    }

    //    public float GetPositionY
    //    {
    //        get { return m_position.Y; }
    //        set { m_position.Y = value; }
    //    }
        


    //    public float GetSize
    //    {
    //        get { return m_size; }
    //        set { m_size = value; }
    //    }

    //}

//Random r = new Random(a_randomSeed);
//m_delay = (float)r.NextDouble() * MAX_DELAY;