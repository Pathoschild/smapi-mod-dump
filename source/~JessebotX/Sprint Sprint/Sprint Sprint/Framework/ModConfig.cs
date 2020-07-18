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
        public int SprintSpeed { get; set; } = 10;
        /// <summary> Modify stamina draining settings </summary>
        public StaminaDrainConfig StaminaDrain { get; set; } = new StaminaDrainConfig();
        /// <summary> Disable sprinting when player is too tired </summary>
        public NoSprintIfTooTiredConfig NoSprintIfTooTired { get; set; } = new NoSprintIfTooTiredConfig();
    }
}
