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
    /// Data structure that represents the fifle data of a WaveBank (XWB).
    /// </summary>
    internal struct XwbData {
        /// <summary>Entry alignment, in bytes.</summary>
        public int Alignment;

        /// <summary>User friendly bank name.</summary>
        public string BankName;

        /// <summary>Timestamp for when the wave bank was built.</summary>
        public int BuildTime;

        /// <summary>Format data for compact wave bank.</summary>
        public int CompactFormat;

        /// <summary>The number of entries (audio files) in the wave bank.</summary>
        public int EntryCount;

        /// <summary>Size of each entry meta-data element, in bytes.</summary>
        public int EntryMetaDataElementSize;

        /// <summary>Size of each entry name element, in bytes.</summary>
        public int EntryNameElementSize;

        /// <summary>Wave bank flags.</summary>
        public int Flags;
    }
}
