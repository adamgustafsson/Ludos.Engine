namespace Ludos.Engine.Particles
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    internal interface IParticle
    {
        Vector2 Position { get; set; }

        float Size { get; set; }

        void Update(float elapsedTime);

        void Draw(float elapsedTime, SpriteBatch spriteBatch, Graphics.Camera2D camera);

        bool IsActive();

        void Respawn();

        void GetVisability();
    }
}
