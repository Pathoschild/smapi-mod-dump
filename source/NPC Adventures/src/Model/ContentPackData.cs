/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace NpcAdventure.Model
{
    internal class ContentPackData
    {
        public const string FORMAT_VERSION = "1.2";
        public const string MIN_FORMAT_VERSION = "1.1";
        public string Format { get; set; }
        public DataChanges[] Changes { get; set; }

        internal class DataChanges
        {
            public string Action { get; set; }
            public string Target { get; set; }
            public string FromFile { get; set; }
            public string Locale { get; set; }
            public string LogName { get; set; }
        }
    }
}