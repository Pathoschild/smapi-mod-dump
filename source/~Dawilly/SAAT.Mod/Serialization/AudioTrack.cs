/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/SAAT
**
*************************************************/

using SAAT.API;

namespace SAAT.Mod.Serialization
{
    /// <summary>
    /// Data container class that details a content pack entry for audio.
    /// </summary>
    public class AudioTrack
    {
        /// <summary>Gets or sets the identification of the track.</summary>
        public string Id { get; set; }

        /// <summary>Gets or sets the relative file path of the audio file.</summary>
        public string Filepath { get; set; }

        /// <summary>Gets or sets the SoundBank category the track belongs to.</summary>
        public Category Category { get; set; }

        /// <summary>Gets the parameters detailing the audio track's settings.</summary>
        public AudioTrackSettings Settings { get; set; } = new AudioTrackSettings();

        /// <summary>
        /// Creates a new instance of the <see cref="AudioTrack"/> class.
        /// </summary>
        public AudioTrack() { }

        /// <summary>
        /// Creates a new instance of the <see cref="AudioTrack"/> class.
        /// </summary>
        /// <param name="id">Identification of the track.</param>
        /// <param name="filePath">Relative file path to the audio file.</param>
        /// <param name="category">SoundBank category the track belongs to.</param>
        public AudioTrack(string id, string filePath, Category category)
        {
            this.Id = id;
            this.Filepath = filePath;
            this.Category = category;
        }
    }
}
