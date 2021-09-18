//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Ludos.Engine.Particles
//{
//    public class SmokeSystem
//    {
//        private const int PARTICLES_PER_ARRAY = 100;
        
//        public List<FireParticle> m_particleArray;
//        public FireParticle m_particle;

//        Texture2D m_smokeTexture;
//        Texture2D m_fireTexture;

//        Random rand = new Random();

//        //Konstruktor - skapar partikelarray via Arrayklass  
//        public SmokeSystem(Vector2 a_position, float a_scale, bool a_isRespawning)
//        {
//            m_particleArray = new List<FireParticle>(PARTICLES_PER_ARRAY);
//            int i = 0;

//            while (i < PARTICLES_PER_ARRAY)
//            {
//                m_particle = new FireParticle(a_position, a_scale, i, a_isRespawning);
//                m_particleArray.Add(m_particle);
//                i++;
//            }
//        }

//        //Laddar textur
//        public void LoadContent(GraphicsDevice graphics)
//        {
//            m_smokeTexture = Utilities.Utilities.CreateTexture2D(graphics, new Point(1, 1), Color.Orange, 1);  //a_content.Load<Texture2D>("particlesmoke");
//            m_fireTexture = Utilities.Utilities.CreateTexture2D(graphics, new Point(1, 1), Color.Yellow, 1);
//        }

//        //Uppdaterar och ritar
//        public void UpdateAndDraw(float a_elapsedTime, SpriteBatch a_spriteBatch, Graphics.Camera2D a_camera)
//        {

//            foreach(FireParticle smokeParticle in m_particleArray)
//            {

//                smokeParticle.Update(a_elapsedTime);


//                if (smokeParticle.IsAlive())
//                {

//                    var isSmoke = rand.Next(1, 100) < 0;

//                    //Visualiserar logiska kordinater för smoken
//                    Vector2 smokePosition = a_camera.VisualizeCordinates(new Vector2(smokeParticle.GetPositionX, smokeParticle.GetPositionY));
//                    int particleSize = (int)smokeParticle.GetSize;

//                    int vx = (int)smokePosition.X;
//                    int vy = (int)smokePosition.Y;
//                    int vw = particleSize;
//                    int vh = particleSize;

//                    Rectangle particleRectangle = new Rectangle(vx, vy, vw, vh);

//                    //Blenda ut färgen
//                    float a = smokeParticle.GetVisibility() / 2;
//                    Color particleColor = new Color(a, a, a, a);


//                    float rotation = 0;//smokeParticle.GetSize * 4;                 
//                    float scale = smokeParticle.GetSize;
//                    Vector2 origin = new Vector2(m_smokeTexture.Width / 2, m_smokeTexture.Height / 2);
//                   // Vector2 origin = Vector2.Zero;

//                    //rita ut partikeln
//                    a_spriteBatch.Draw(isSmoke ? m_smokeTexture : m_fireTexture, particleRectangle, null, particleColor, rotation, origin, SpriteEffects.None, 0.0f);

//                    //a_spriteBatch.Draw(m_smokeTexture, smokePosition, null, particleColor, rotation, origin, scale, SpriteEffects.None, 0.0f); 
//                }

//            }

//        }


//    }
//}
