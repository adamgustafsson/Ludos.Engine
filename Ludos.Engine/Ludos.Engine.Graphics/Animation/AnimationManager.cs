namespace Ludos.Engine.Graphics
{
    using System.Collections.Generic;
    using System.Linq;
    using Ludos.Engine.Actors;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class AnimationManager
    {
        private readonly Camera2D _camera;
        private readonly Dictionary<Actor.State, Animation> _animations;

        public AnimationManager(Camera2D camera, Dictionary<Actor.State, Animation> animations)
        {
            _camera = camera;
            _animations = animations;
        }

        public void Update(GameTime gameTime)
        {
            var visibleAnimations = _animations.Where(x => _camera.CameraBounds.IntersectsWith(x.Value.Actor.Bounds) && x.Value.FrameCount > 1);
            var inactiveAnimations = _animations.Where(x => !_camera.CameraBounds.IntersectsWith(x.Value.Actor.Bounds) && x.Value.FrameCount > 1).Select(x => x.Value);

            foreach (var animation in visibleAnimations.Where(x => x.Key == x.Value.Actor.CurrentState).Select(x => x.Value))
            {
                animation.IsAnimating = true;
                animation.Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (animation.Timer > animation.FrameSpeed)
                {
                    animation.Timer = 0f;

                    animation.CurrentXFrame++;

                    if ((animation.StartFrame.X + animation.CurrentXFrame) >= (animation.StartFrame.X + animation.FrameCount))
                    {
                        animation.CurrentXFrame = 0;
                    }
                }
            }

            foreach (var animation in inactiveAnimations)
            {
                animation.Reset();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var visibleAnimations = _animations.Where(x => _camera.CameraBounds.IntersectsWith(x.Value.Actor.Bounds));

            foreach (var animation in visibleAnimations.Where(x => x.Key == x.Value.Actor.CurrentState).Select(x => x.Value))
            {
                spriteBatch.Draw(
                    animation.Texture,
                    _camera.VisualizeCordinates(animation.Position),
                    new Rectangle(
                        (animation.StartFrame.X + animation.CurrentXFrame) * animation.FrameWidth,
                        animation.StartFrame.Y * animation.FrameHeight,
                        animation.FrameWidth,
                        animation.FrameHeight),
                    Color.White,
                    rotation: 0,
                    origin: Vector2.Zero,
                    scale: animation.Scale,
                    animation.Actor.CurrentDirection == Actor.Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    layerDepth: 0);
            }
        }
    }
}
