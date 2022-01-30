/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/MiceInTheValley
**
*************************************************/


namespace MiceInTheValley.Framework {
    internal class ModConfig {
        /// <summary>The volume of the sound. Valid range from 0.0 to 1.0 .</summary>
        public float Volume { get; set; } = 0.5f;

        /// <summary>The pitch of the sound. Valid range from -1.0 to 1.0 .</summary>
        public float Pitch { get; set; } = 0.3f;
    }
}
