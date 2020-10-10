/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/hcoona/StardewValleyMods
**
*************************************************/

using System.Runtime.Serialization;

namespace MSBuildExtensions
{
    [DataContract]
    public class Manifest
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string Author { get; set; }

        [DataMember(Order = 3)]
        public ModVersion Version { get; set; }

        [DataMember(Order = 4)]
        public string Description { get; set; }

        [DataMember(Order = 5)]
        public string UniqueID { get; set; }

        [DataMember(Order = 6)]
        public string EntryDll { get; set; }
    }

    [DataContract]
    public class ModVersion
    {
        [DataMember(Order = 1)]
        public int MajorVersion { get; set; }

        [DataMember(Order = 2)]
        public int MinorVersion { get; set; }

        [DataMember(Order = 3)]
        public int PatchVersion { get; set; }

        [DataMember(Order = 4)]
        public int? Build { get; set; }
    }
}
