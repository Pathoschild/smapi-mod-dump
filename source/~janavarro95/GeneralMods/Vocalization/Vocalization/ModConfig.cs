using System.Collections.Generic;
using Vocalization.Framework;

namespace Vocalization
{
    /// <summary>The configuration file for the mod.</summary>
    public class ModConfig
    {

        /// <summary>Handles all of the translation information and parsing.</summary>
        public TranslationInfo translationInfo = new TranslationInfo();

        /// <summary>Keeps track of the voice modes for determining how much audio is played.</summary>
        public List<string> modes = new List<string> { "Simple", "Full", "HeartEvents", "SimpleAndHeartEvents" };

        /// <summary>The current mode for the mod.</summary>
        public string currentMode = "Full";

        /// <summary>The volume at which the sound for voices is played at.</summary>
        public decimal voiceVolume = 1;

        /// <summary>Validates</summary>
        public void verifyValidMode()
        {
            if (!this.modes.Contains(this.currentMode))
            {
                Vocalization.ModMonitor.Log("Invalid configuration: " + this.currentMode + ". Changing to Full voiced mode.");
                this.currentMode = "Full";
            }
        }
    }
}
