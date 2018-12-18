using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vocalization.Framework;

namespace Vocalization
{
    /// <summary>
    /// The configuration file for the mod.
    /// </summary>
    public class ModConfig
    {

        /// <summary>
        /// Handles all of the translation information and parsing.
        /// </summary>
        public TranslationInfo translationInfo;

        /// <summary>
        /// Keeps track of the voice modes for determining how much audio is played.
        /// </summary>
        public List<string> modes;

        /// <summary>
        /// The curent mode for the mod.
        /// </summary>
        public string currentMode;

        /// <summary>
        /// The volume at which the sound for voices is played at.
        /// </summary>
        public decimal voiceVolume;

        public ModConfig()
        {
            modes = new List<string>();

            modes.Add("Simple");
            modes.Add("Full");
            modes.Add("HeartEvents");
            modes.Add("SimpleAndHeartEvents");
            currentMode = "Full";

            translationInfo = new TranslationInfo();


            this.voiceVolume = (decimal)1.0f;
        }

        /// <summary>
        /// Validates
        /// </summary>
        public void verifyValidMode()
        {
            if (!modes.Contains(currentMode))
            {
                Vocalization.ModMonitor.Log("Invalid configuration: " + currentMode + ". Changing to Full voiced mode.");
                currentMode = "Full";
            }
        }




    }
}
