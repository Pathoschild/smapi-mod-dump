namespace Omegasis.TimeFreeze.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>Whether time should be unfrozen while the player is swimming.</summary>
        public bool PassTimeWhileSwimming { get; set; } = true;

        /// <summary>Whether time should be unfrozen while the player is swimming in the vanilla bathhouse.</summary>
        public bool PassTimeWhileSwimmingInBathhouse { get; set; } = true;
    }
}
