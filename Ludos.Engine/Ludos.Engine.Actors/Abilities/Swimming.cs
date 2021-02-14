namespace Ludos.Engine.Actors
{
    using System.Collections.Generic;
    using System.Linq;
    using FuncWorks.XNA.XTiled;
    using Ludos.Engine.Utilities;
    using Microsoft.Xna.Framework;

    internal class Swimming : IAbility
    {
        public Swimming(float defaultActorGravity, Vector2 defaultActorSpeed, DivingBehavior divingBehavior = DivingBehavior.DiveOnButtonPress)
        {
            DefaultGravity = defaultActorGravity;
            DefaultSpeed = defaultActorSpeed;
            CurrentDivingBehavior = divingBehavior;
            AbilityEnabled = true;
        }

        public enum DivingBehavior
        {
            AlwaysDive = 0,
            DiveOnButtonPress = 1,
        }

        public bool AbilityEnabled { get; set; }
        public bool AbilityTemporarilyDisabled { get; set; }
        public bool IsInWater { get; set; }
        public bool IsSubmerged { get; set; }
        public bool IsDiving { get; set; }
        public DivingBehavior CurrentDivingBehavior { get; set; }
        public float DefaultGravity { get; set; }
        public Vector2 DefaultSpeed { get; set; }

        public void Update(
            Actor actor,
            ref Vector2 actorVelocity,
            float actorLastYPosition,
            bool jumpInitiated,
            IEnumerable<MapObject> water,
            float elapsedTime,
            bool diveButtonIsPressedDown = false)
        {
            var isCollidingWithWater = water.Any();

            if (!IsInWater && isCollidingWithWater)
            {
                IsInWater = true;
                actor.Speed = new Vector2(actor.Speed.X * 0.85f, actor.Speed.Y);

                actor.TemporarilyDisabledAbility<WallJump>();
            }
            else if ((jumpInitiated && isCollidingWithWater && !IsSubmerged) || (IsInWater && !isCollidingWithWater))
            {
                IsInWater = false;
                actor.Speed = DefaultSpeed;
                actor.GetAbility<DoubleJump>()?.ResetAbility();

                actor.EnableTemporarilyDisabledAbility<WallJump>();
            }

            if (IsInWater)
            {
                var waterObjectBounds = water.First().Bounds;
                IsSubmerged = actor.Bounds.Top > waterObjectBounds.Top;

                if (IsDiving)
                {
                    actor.Gravity = 100;

                    if (actorVelocity.Y > 100)
                    {
                        actorVelocity.Y = 100;
                    }
                }
                else if (actor.Bounds.Center().Y > waterObjectBounds.Top)
                {
                    actorVelocity.Y = 0;
                    actor.Gravity -= elapsedTime * (IsSubmerged ? 2000 : 1000);
                }
                else if (actorLastYPosition > actor.Position.Y && (actor.Bounds.Center().Y < waterObjectBounds.Top))
                {
                    if (actor.Gravity < 500)
                    {
                        actor.Gravity = 500;
                    }
                    else
                    {
                        actor.Gravity += elapsedTime * 1000;
                    }
                }

                IsDiving = CurrentDivingBehavior == DivingBehavior.DiveOnButtonPress ? diveButtonIsPressedDown : true;

                if (IsSubmerged && actor.OnGround && !IsDiving)
                {
                    actor.Position = new Vector2(actor.Position.X, actor.Position.Y - 0.85f);
                }
            }
            else
            {
                IsSubmerged = false;
                actor.Gravity = DefaultGravity;
            }

            if (IsSubmerged && actor.AbilityIsActive<DoubleJump>())
            {
                actor.TemporarilyDisabledAbility<DoubleJump>();
            }
            else if (!IsSubmerged && (actor.GetAbility<DoubleJump>()?.AbilityTemporarilyDisabled == true))
            {
                actor.EnableTemporarilyDisabledAbility<DoubleJump>();
            }
        }

        public void ResetAbility()
        {
            IsInWater = false;
            IsSubmerged = false;
        }
    }
}