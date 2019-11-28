using StardewModdingAPI;
using System.Collections.Generic;

namespace NpcAdventure.Model
{
    internal class ContentPackData
    {
        public const string FORMAT_VERSION = "1.0";
        public const string MIN_FORMAT_VERSION = "1.0";
        public string Format { get; set; }
        public DataChanges[] Changes { get; set; }

        internal class DataChanges
        {
            public string Action { get; set; }
            public string Target { get; set; }
            public string FromFile { get; set; }
        }
    }
}