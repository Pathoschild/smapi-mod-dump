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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAAT.WaveBankWriter {
    internal class XwbFile {
        private const int LatestVersion = 46;
        private const int LatestSegmentCount = 5;

        public readonly string Magic = "WBND";

        public XwbHeader Header { get; }

        public XwbData Data { get; }

        public XwbFile() {
            this.Header = new XwbHeader {
                Version = XwbFile.LatestVersion,
                Segments = new XwbSegment[XwbFile.LatestSegmentCount]
            };


        }
    }
}
