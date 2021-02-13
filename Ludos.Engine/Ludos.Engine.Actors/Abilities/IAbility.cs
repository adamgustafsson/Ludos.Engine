namespace Ludos.Engine.Actors
{
    public interface IAbility
    {
        bool AbilityEnabled { get; set; }

        bool AbilityTemporarilyDisabled { get; set; }

        void ResetAbility();
    }
}
