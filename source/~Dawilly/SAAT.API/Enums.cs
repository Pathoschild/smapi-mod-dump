/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/SAAT
**
*************************************************/

namespace SAAT.API {
    /// <summary>
    /// Enumeration that represents the audio category a track belongs to.
    /// 
    /// Setting a track to a category allows the game settings to control the volume
    /// output based on the category's setting.
    /// </summary>
    public enum Category : int
    {
        /// <summary>Do not use.</summary>
        Global = 0,

        /// <summary>Do not use.</summary>
        Default = 1,

        /// <summary>A background music type.</summary>
        /// <remarks>All audio tracks, once heard by the player, are listed in the jukebox.</remarks>
        Music = 2,

        /// <summary>A general sound effect type. (I.e. sword swinging, bomb explosion, etc).</summary>
        Sound = 3,

        /// <summary>A ambient sound effect type. (I.e. Birds, wind, etc).</summary>
        Ambient = 4,

        /// <summary>A sound effect that resembles a particular foot-step.</summary>
        Footsteps = 5
    }

    /// <summary>
    /// Enumeration that represents the audio file type.
    /// </summary>
    internal enum AudioFileType
    {
        /// <summary>A PCM .wav file type.</summary>
        Wav,

        /// <summary>A Vorbis Ogg file type.</summary>
        Ogg
    }

    /// <summary>
    /// Enumeration that details the error during an operation (method call).
    /// </summary>
    public enum AudioOperationError
    {
        /// <summary>Generic fail case.</summary>
        Failed,

        /// <summary>The audio asset file / data was not found.</summary>
        AssetNotFound,

        /// <summary>An entry already exists with the provided parameter data.</summary>
        Exists,

        /// <summary>No errors occurred. Success.</summary>
        None
    }
}
