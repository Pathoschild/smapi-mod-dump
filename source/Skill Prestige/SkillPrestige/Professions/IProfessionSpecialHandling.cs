namespace SkillPrestige.Professions
{
    /// <summary>
    /// Represents special handling for professions where Stardew Valley applies the profession's effects in a custom manner.
    /// </summary>
    public interface IProfessionSpecialHandling
    {
        void ApplyEffect();

        void RemoveEffect();
    }
}
