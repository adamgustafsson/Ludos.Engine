namespace Ludos.Engine.Model
{
    public interface IAbility
    {
        bool AbilityEnabled { get; set; }

        void ResetAbility();
    }
}
