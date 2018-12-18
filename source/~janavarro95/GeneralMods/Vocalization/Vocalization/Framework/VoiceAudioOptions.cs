using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vocalization.Framework
{
    /// <summary>
    /// Deals with determining what mode should play at the current moment.
    /// </summary>
    public class VoiceAudioOptions
    {
        /// <summary>
        /// The audio clip that plays when the current voice mode is "Simple".
        /// </summary>
        public string simple;

        /// <summary>
        /// The audio clip that plays when the current voice mode is "Full".
        /// </summary>
        public string full;

        /// <summary>
        /// The audio clip that plays when the current voice mode is "HeartEvents".
        /// </summary>
        public string heartEvents;

        /// <summary>
        /// The audio clip that plays when the current voice mode is "SimpleAndHeartEvents".
        /// </summary>
        public string simpleAndHeartEvents;

        public VoiceAudioOptions()
        {
            simple = "";
            full = "";
            heartEvents = "";
            simpleAndHeartEvents = "";
        }

        public VoiceAudioOptions(string simple, string full, string heartEvent, string simpleAndHeartEvent)
        {
            this.simple = simple;
            this.full = full;
            this.heartEvents = heartEvent;
            this.simpleAndHeartEvents = simpleAndHeartEvent;
        }

        public string getAudioClip()
        {
            if (Vocalization.config.currentMode == "Simple") return simple;
            if (Vocalization.config.currentMode == "Full") return full;
            if (Vocalization.config.currentMode == "HeartEvents") return heartEvents;
            if (Vocalization.config.currentMode == "SimpleAndHeartEvents") return simpleAndHeartEvents;
            return ""; //The current mode must not have been valid for some reason???
        }
    }
}
