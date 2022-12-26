namespace Ludos.Engine.Actors
{
    public class DoubleJump : IAbility
    {
        private bool _abilityTemporarilyDisabled;

        public DoubleJump()
        {
            AbilityEnabled = true;
        }

        public bool AbilityEnabled { get; set; }
        public bool DoubleJumpAvailable { get; set; } = true;
        public bool DoubleJumpUsed { get; set; } = false;
        public bool AbilityTemporarilyDisabled
        {
            get
            {
                return _abilityTemporarilyDisabled;
            }

            set
            {
                _abilityTemporarilyDisabled = value;
                JumpUsedOrCanceled();
            }
        }

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