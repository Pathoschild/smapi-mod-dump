namespace Vocalization.Framework
{
    /// <summary>Deals with determining what mode should play at the current moment.</summary>
    public class VoiceAudioOptions
    {
        /// <summary>The audio clip that plays when the current voice mode is "Simple".</summary>
        public string simple;

        /// <summary>The audio clip that plays when the current voice mode is "Full".</summary>
        public string full;

        /// <summary>The audio clip that plays when the current voice mode is "HeartEvents".</summary>
        public string heartEvents;

        /// <summary>The audio clip that plays when the current voice mode is "SimpleAndHeartEvents".</summary>
        public string simpleAndHeartEvents;

        public VoiceAudioOptions()
        {
            this.simple = "";
            this.full = "";
            this.heartEvents = "";
            this.simpleAndHeartEvents = "";
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
            switch (Vocalization.config.currentMode)
            {
                case "Simple":
                    return this.simple;

                case "Full":
                    return this.full;

                case "HeartEvents":
                    return this.heartEvents;

                case "SimpleAndHeartEvents":
                    return this.simpleAndHeartEvents;

                default:
                    return ""; //The current mode must not have been valid for some reason???
            }
        }
    }
}
