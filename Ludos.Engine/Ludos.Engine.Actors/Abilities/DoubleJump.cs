namespace Ludos.Engine.Actors
{
    internal class DoubleJump : IAbility
    {
        public DoubleJump()
        {
            AbilityEnabled = true;
        }

        public bool AbilityEnabled { get; set; }
        public bool DoubleJumpAvailable { get; set; } = true;
        public bool DoubleJumpUsed { get; set; } = false;
        public bool AbilityTemporarilyDisabled { get; set; }

        public void JumpUsedOrCanceled()
        {
            DoubleJumpAvailable = false;
            DoubleJumpUsed = true;
        }

        public void ResetAbility()
        {
            DoubleJumpAvailable = true;
            DoubleJumpUsed = false;
        }
    }
}