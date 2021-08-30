using StardewModdingAPI;

namespace SprintAndDashRedux
{
    public class SprintDashConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The key to hold for sprinting.</summary>
        public SButton SprintKey { get; set; } = SButton.Space;

        /// <summary>The key to activate combat dash.</summary>
        public SButton DashKey { get; set; } = SButton.Q;

        /// <summary>The stamina cost per tick for sprinting.</summary>
        public float StamCost { get; set; } = 2;

        /// <summary>Length of time combat dash lasts.</summary>
        public int DashDuration { get; set; } = 4;

        /// <summary>Number of seconds of sprinting before player is winded, or 0 to disable windedness.</summary>
        public int WindedStep { get; set; } = 5;

        /// <summary>Minimum player stamina for sprint to activate.</summary>
        public float QuitSprintingAt { get; set; } = 30f;

        /// <summary>Whether to operate the button as a toggle.</summary>
        public bool ToggleMode { get; set; } = true;

        /// <summary>How often per second to check sprint logic.</summary>
        public double TimeInterval { get; set; } = 0.1;

        /// <summary>An extra control of log output, silences all logging.</summary>
        public bool VerboseMode { get; set; } = false;
    }
}
