/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/SAAT
**
*************************************************/

using StardewValley;

namespace SAAT.API {
    /// <summary>
    /// Data container class that details an audio track.
    /// </summary>
    internal class Track {
        /// <summary>Gets or sets the size of the audio data buffer, in bytes.</summary>
        public uint BufferSize { get; set; }

        /// <summary>Gets or sets the SoundBank category the track belongs to.</summary>
        public Category Category { get; set; }

        /// <summary>Gets or sets the identification of the track.</summary>
        public string Id { get; set; }

        /// <summary>Gets or sets the cue instance of the audio track.</summary>
        public ICue Instance { get; set; }

        /// <summary>Gets or sets the relative file path of the audio file.</summary>
        public string Filepath { get; set; }

        /// <summary>Gets or sets the unique identification of the owner (mod).</summary>
        public string Owner { get; set; }

        /// <summary>Gets or sets a value indicating if the audio track should loop.</summary>
        public bool Loop { get; set; }
    }
}
