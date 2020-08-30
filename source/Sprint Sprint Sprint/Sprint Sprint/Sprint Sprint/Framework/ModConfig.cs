using Sprint_Sprint.Framework.Config;
using StardewModdingAPI;

namespace Sprint_Sprint.Framework
{
    class ModConfig
    {
        /// <summary> The sprinting keybind </summary>
        public SButton SprintKey { get; set; } = SButton.LeftShift;
        /// <summary> Check if user has to hold down <see cref="SprintKey"/> in order to sprint </summary>
        public bool HoldToSprint { get; set; } = true;
        /// <summary> The sprinting speed </summary>
        public int SprintSpeed { get; set; } = 8;
        /// <summary> The default horse speed, since sprinting while on mount is disabled. </summary>
        public int HorseSpeed { get; set; } = 5;
        /// <summary> Modify stamina draining settings </summary>
        public StaminaDrainConfig StaminaDrain { get; set; } = new StaminaDrainConfig();
        /// <summary> Disable sprinting when player is too tired </summary>
        public NoSprintIfTooTiredConfig NoSprintIfTooTired { get; set; } = new NoSprintIfTooTiredConfig();
    }
}
