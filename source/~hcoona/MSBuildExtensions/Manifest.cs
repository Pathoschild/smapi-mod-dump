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
