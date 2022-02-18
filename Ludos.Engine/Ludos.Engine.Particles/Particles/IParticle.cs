namespace Ludos.Engine.Particles
{
    using Microsoft.Xna.Framework.Graphics;

    internal interface IParticle
    {
        void Update(float elapsedTime);

        void Draw(float elapsedTime, SpriteBatch spriteBatch, Graphics.Camera2D camera);

        bool IsActive();

        void Respawn();
    }
}
