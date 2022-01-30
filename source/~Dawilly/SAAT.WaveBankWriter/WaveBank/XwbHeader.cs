/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/SAAT
**
*************************************************/

namespace SAAT.WaveBankWriter {
    /// <summary>
    /// Data structure that represents the file header of a WaveBank (XWB).
    /// </summary>
    /// <remarks>XACT no longer recieves updates. So Version and the number of Segments should be 46 and 5 respectively.</remarks>
    internal struct XwbHeader {
        /// <summary>The version of the WaveBank.</summary>
        public int Version;

        /// <summary>The segments of the WaveBank.</summary>
        public XwbSegment[] Segments;
    }
}
