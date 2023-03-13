namespace Ludos.Engine.Graphics
{
    using Ludos.Engine.Actors;
    using Ludos.Engine.Core;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RectangleF = System.Drawing.RectangleF;

    public class Animation
    {
        private float _frameSpeed;
        private Vector2 _staticPosition;

        public Animation(Texture2D texture, Actor actor, Point startFrame, Point sheetFrameCount, int frameCount, float scale = 1)
            : this(texture, actor as GameObject, startFrame, sheetFrameCount, frameCount, scale)
        {
            Actor = actor;
        }

        public Animation(Texture2D texture, GameObject gameObject, Point startFrame, Point sheetFrameCount, int frameCount, float scale = 1)
            : this(texture, gameObject.Position, startFrame, sheetFrameCount, frameCount, scale)
        {
            GameObject = gameObject;

            if (GameObject.Bounds.Width != (FrameWidth * Scale))
            {
                PositionOffset = new Vector2((GameObject.Bounds.Width - (FrameWidth * Scale)) / 2, PositionOffset.Y);
            }

            if (GameObject.Bounds.Height != (FrameHeight * Scale))
            {
                PositionOffset = new Vector2(PositionOffset.X, GameObject.Bounds.Height - (FrameHeight * Scale));
            }
        }

        public Animation(Texture2D texture, Vector2 position, Point startFrame, Point sheetFrameCount, int frameCount, float scale = 1)
        {
            Texture = texture;
            StartFrame = startFrame;
            FrameCount = frameCount;
            FrameWidth = Texture.Width / sheetFrameCount.X;
            FrameHeight = Texture.Height / sheetFrameCount.Y;
            IsLooping = true;
            Scale = scale;
            CurrentXFrame = StartFrame.X;

            _staticPosition = position;
            _frameSpeed = 0.2f;
        }

        public Actor Actor { get; }
        public GameObject GameObject { get; }
        public RectangleF Bounds
        {
            get { return GameObject == null ? new RectangleF(Position.X, Position.Y, FrameWidth, FrameHeight) : GameObject.Bounds; }
        }

        public float Timer { get; set; }
        public float Scale { get; set; } = 1;
        public int CurrentXFrame { get; set; }
        public int FrameCount { get; set; }
        public int FrameHeight { get; set; }
        public Point StartFrame { get; set; }
        public float FrameSpeed { get => UseVelocityBasedFrameSpeed && GameObject != null ? (_frameSpeed / System.Math.Abs(GameObject.Velocity.X)) : _frameSpeed; set => _frameSpeed = value; }
        public bool UseVelocityBasedFrameSpeed { get; set; }
        public int FrameWidth { get; set; }
        public bool IsLooping { get; set; }
        public bool IsAnimating { get; set; }
        public Texture2D Texture { get; private set; }
        public Vector2 Position { get => GameObject != null ? GameObject.Position + PositionOffset : _staticPosition + PositionOffset; set => Position = value; }
        public Vector2 PositionOffset { get; set; }

        public void Reset()
        {
            IsAnimating = false;
            Timer = 0;
            CurrentXFrame = StartFrame.X;
        }
    }
}
