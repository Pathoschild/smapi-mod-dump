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

namespace SAAT.API
{
    /// <summary>
    /// API reference that provides access to the audio manager and its functionality.
    /// </summary>
    public interface IAudioManager
    {
        /// <summary>Gets the implementation of the sound bank used by Stardew Valley.</summary>
        ISoundBank SoundBank { get; }

        /// <summary>
        /// Add a song to the jukebox, regardless if the player has heard it.
        /// 
        /// This will add a song by it's name / Id to the <see cref="Farmer.songsHeard"/> collection. This is 
        /// the source of all tracks that are available on the jukebox.
        /// </summary>
        /// <param name="name">The name / id of the audio track.</param>
        /// <param name="error">Details the error if the results are <c>false</c>.</param>
        /// <returns><c>true</c> if the track was successfully added. <c>false</c> otherwise.</returns>
        /// <remarks>
        /// Normal jukebox operations indicate audio tracks are added once the player hears the track 
        /// for the first time.
        /// 
        /// Possible <see cref="AudioOperationError"/>s:
        /// <see cref="AudioOperationError.AssetNotFound"/> - The Cue could not be found.
        /// <see cref="AudioOperationError.Exists"/> - The audio track is already in the jukebox's playlist.
        /// </remarks>
        bool AddToJukebox(string name, out AudioOperationError error);

        /// <summary>
        /// Loads an audio asset.
        /// </summary>
        /// <param name="filePath">The path to the audio asset.</param>
        /// <param name="owner">The unique mod identification who is loading said audio asset.</param>
        /// <param name="createInfo">Parameter data for creating the <see cref="ICue"/>.</param>
        /// <returns>A newly created <see cref="ICue"/> instance.</returns>
        ICue Load(string filePath, string owner, CreateAudioInfo createInfo);

        /// <summary>
        /// Details the memory allocations of audio, by each individual audio track that is
        /// currently in memory.
        /// </summary>
        void PrintMemoryAllocationInfo();

        /// <summary>
        /// Detail a specific audio track; its memory allocation and settings.
        /// </summary>
        /// <param name="id">The unique identification of the audio track.</param>
        void PrintTrackAllocationAndSettings(string id);
    }
}
