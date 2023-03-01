/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SinZ163/StardewMods
**
*************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Profiler
{

    public class ProfilerContentPack
    {
        public string? Format { get; }

        public ProfilerContentPackEntry?[] Entries { get; }

        public ProfilerContentPack(string? format, ProfilerContentPackEntry?[] entries)
        {
            this.Format = format;
            this.Entries = entries ?? Array.Empty<ProfilerContentPackEntry>();
        }
    }
    public class ProfilerContentPackEntry
    {
        public string? Type { get; set;  }
        public string? TargetType { get; set;  }
        public string? TargetMethod { get; set; }

        public string? ConditionalMod { get; set; }

        public ProfilerContentPackDetailEntry? Details { get; set; }

        [JsonConstructor]
        public ProfilerContentPackEntry()
        {
        }
    }
    public class ProfilerContentPackDetailEntry
    {
        public string? Type { get; set; }
        public string? Name { get; set; }
        public int? Index { get; set; }

        public bool? Supress { get; set; }

        [JsonConstructor]
        public ProfilerContentPackDetailEntry()
        {
        }
    }
}
