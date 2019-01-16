namespace Omegasis.TimeFreeze.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class ModConfig
    {
        /// <summary>Whether time should be unfrozen while the player is swimming.</summary>
        public bool PassTimeWhileSwimming { get; set; } = true;

        /// <summary>Whether time should be unfrozen while the player is swimming in the vanilla bathhouse.</summary>
        public bool PassTimeWhileSwimmingInBathhouse { get; set; } = true;

        /// <summary>Whether time passes normally inside the mine.</summary>
        public bool PassTimeWhileInsideMine { get; set; } = true;

        /// <summary>Whether time passes normally inside the skull cavern.</summary>
        public bool PassTimeWhileInsideSkullCave { get; set; } = true;

        /// <summary>Checks if just one player meets the conditions to freeze time, and then freeze time.</summary>
        public bool freezeIfEvenOnePlayerMeetsTimeFreezeConditions { get; set; } = false;

        /// <summary>Checks if the majority of players can freeze time and then freeze time.</summary>
        public bool freezeIfMajorityPlayersMeetsTimeFreezeConditions { get; set; } = true;

        /// <summary>Checks if all players can freeze time and if so, do so.</summary>
        public bool freezeIfAllPlayersMeetTimeFreezeConditions { get; set; } = false;
    }
}
