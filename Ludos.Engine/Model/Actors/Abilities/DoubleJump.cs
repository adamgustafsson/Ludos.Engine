namespace Ludos.Engine.Model
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

        public void ResetAbility()
        {
            DoubleJumpAvailable = true;
            DoubleJumpUsed = false;
        }
    }
}