namespace Ludos.Engine.Actors
{
    public interface IAbility
    {
        bool AbilityEnabled { get; set; }

        void ResetAbility();
    }
}
