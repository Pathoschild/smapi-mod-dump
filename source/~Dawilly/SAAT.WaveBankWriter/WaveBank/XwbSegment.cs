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
    /// Data structure that represents a single segment of a WaveBank (XWB).
    /// </summary>
    internal struct XwbSegment {
        /// <summary>The length of the section.</summary>
        public int Length;

        /// <summary>The address offset detailing the start of the section.</summary>
        public int Offset;
    }
}
