/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/SAAT
**
*************************************************/

using System;
using System.IO;

namespace SAAT.API {
    /// <summary>
    /// Collection of helper methods to aid in various operations.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Parse a filepath for its audio file type.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>Enumeration value detailing the file type.</returns>
        public static AudioFileType ParseAudioExtension(string path)
        {
            string ext = Path.GetExtension(path).ToLower();

            switch (ext) {
                case ".wav":
                    return AudioFileType.Wav;

                case ".ogg":
                    return AudioFileType.Ogg;

                default:
                    throw new ArgumentException("Audio File type not supported");
            }
        }

        /// <summary>
        /// Convert a <see cref="Track"/>'s buffer size from bytes to kilobytes.
        /// </summary>
        /// <param name="track">The track instance.</param>
        /// <returns>The track's buffer size in kilobytes.</returns>
        public static double BufferSizeInKilo(Track track)
        {
            return Math.Round((double)track.BufferSize / 1024, 2);
        }

        /// <summary>
        /// Convert a <see cref="Track"/>'s buffer size from bytes to megabytes.
        /// </summary>
        /// <param name="track">The track instance.</param>
        /// <returns>The track's buffer size in megabytes.</returns>
        public static double BufferSizeInMega(Track track)
        {
            return Math.Round((double)track.BufferSize / 1048576, 2);
        }

        /// <summary>
        /// Convert a provided buffer size in bytes to kilobytes
        /// </summary>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <returns>The buffer size in kilobytes.</returns>
        public static double BufferSizeInKilo(uint bufferSize)
        {
            return Math.Round((double)bufferSize / 1024, 2);
        }

        /// <summary>
        /// Convert a provided buffer size in bytes to megabytes
        /// </summary>
        /// <param name="bufferSize">The buffer size in bytes.</param>
        /// <returns>The buffer size in megabytes.</returns>
        public static double BufferSizeInMega(uint bufferSize)
        {
            return Math.Round((double)bufferSize / 1048576, 2);
        }
    }
}
