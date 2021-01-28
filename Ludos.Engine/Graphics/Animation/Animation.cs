using Ludos.Engine.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ludos.Engine.Graphics
{
    public class Animation
    {
        private float _frameSpeed;
        public Actor Actor { get; }
        public float Timer { get; set; }
        public float Scale { get; set; } = 1;
        public int CurrentXFrame { get; set; }
        public Point StartFrame { get; set; }
        public int FrameCount { get; set; }
        public int FrameHeight { get; set; }       
        public float FrameSpeed { get => UseVelocityBasedFrameSpeed ? (_frameSpeed / System.Math.Abs(Actor.Velocity.X)) : _frameSpeed; set => _frameSpeed = value; } 
        public bool UseVelocityBasedFrameSpeed { get; set; }
        public int FrameWidth { get; set; }
        public bool IsLooping { get; set; }
        public bool IsAnimating { get; set; }
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get => Actor.Position + PositionOffset; set => Position = value; }
        public Vector2 PositionOffset { get; set; }

        public Animation(Texture2D texture, Actor actor, Point startFrame, Point sheetFrameCount, int frameCount)
        {
            Actor = actor;
            Texture = texture;
            StartFrame = startFrame;
            FrameCount = frameCount;
            FrameWidth = Texture.Width / sheetFrameCount.X;
            FrameHeight = Texture.Height / sheetFrameCount.Y;
            IsLooping = true;
            _frameSpeed = 0.2f;
        }

        public void Reset()
        {
            IsAnimating = false;
            Timer = 0;
            CurrentXFrame = StartFrame.X;
        }
    }
}
