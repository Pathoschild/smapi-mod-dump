/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/SAAT
**
*************************************************/

namespace SAAT.Mod.Serialization
{
    /// <summary>
    /// Data container class that details an audio tracks settings.
    /// </summary>
    public class AudioTrackSettings
    {
        /// <summary>Get or set a value indicating if the audio track loops.</summary>
        public bool Loop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the audio track should be added to the jukebox's
        /// playlist on the creation of a new game.
        /// </summary>
        public bool AddToJukebox { get; set; }
    }
}
