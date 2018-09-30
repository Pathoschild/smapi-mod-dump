using Microsoft.Xna.Framework.Input;

namespace SprintAndDashRedux
{
    public class SprintDashConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The key to hold for sprinting.</summary>
        public Keys SprintKey { get; set; } = Keys.Space;

        /// <summary>The key to activate combat dash.</summary>
        public Keys DashKey { get; set; } = Keys.Q;

        /// <summary>The stamina cost per tick for sprinting.</summary>
        public float StamCost { get; set; } = 2;

        /// <summary>Length of time combat dash lasts.</summary>
        public int DashDuration { get; set; } = 4;

        /// <summary>Number of seconds of sprinting before player is winded, or 0 to disable windedness.</summary>
        public int WindedStep { get; set; }

        /// <summary>Whether to operate the button as a toggle.</summary>
        public bool ToggleMode { get; set; }
    }
}
