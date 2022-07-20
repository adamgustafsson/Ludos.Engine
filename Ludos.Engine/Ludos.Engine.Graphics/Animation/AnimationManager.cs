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
        private readonly Dictionary<Actor.State, Animation> _actorAnimations;
        private readonly List<Animation> _animations;

        public AnimationManager(Camera2D camera, Dictionary<Actor.State, Animation> actorAnimations)
        {
            _camera = camera;
            _actorAnimations = actorAnimations;
        }

        public AnimationManager(Camera2D camera, Dictionary<Actor.State, Animation> actorAnimations, List<Animation> animations)
        {
            _camera = camera;
            _actorAnimations = actorAnimations;
            _animations = animations;
        }

        public void Update(GameTime gameTime)
        {
            var visibleActorAnimations = _actorAnimations.Where(x => _camera.CameraBounds.IntersectsWith(x.Value.Actor.Bounds) && x.Value.FrameCount > 1 && x.Key == x.Value.Actor.CurrentState).Select(x => x.Value);
            var inactiveActorAnimations = _actorAnimations.Where(x => !_camera.CameraBounds.IntersectsWith(x.Value.Actor.Bounds) && x.Value.FrameCount > 1).Select(x => x.Value);

            var animationsToUpdate = visibleActorAnimations.ToList();
            var animationsToReset = inactiveActorAnimations.ToList();

            if (_animations?.Any() == true)
            {
                animationsToUpdate.AddRange(_animations.Where(x => _camera.CameraBounds.IntersectsWith(x.Bounds) && x.FrameCount > 1));
                animationsToReset.AddRange(_animations.Where(x => !_camera.CameraBounds.IntersectsWith(x.Bounds) && x.FrameCount > 1));
            }

            foreach (var animation in animationsToUpdate)
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

            foreach (var animation in animationsToReset)
            {
                animation.Reset();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var visibleAnimations = _actorAnimations.Where(x => _camera.CameraBounds.IntersectsWith(x.Value.Actor.Bounds) && x.Key == x.Value.Actor.CurrentState).Select(x => x.Value).ToList();

            if (_animations?.Any() == true)
            {
                visibleAnimations.AddRange(_animations.Where(x => _camera.CameraBounds.IntersectsWith(x.Bounds)));
            }

            foreach (var animation in visibleAnimations)
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
                    animation.Actor?.CurrentDirection == Actor.Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    layerDepth: 0);
            }
        }
    }
}
