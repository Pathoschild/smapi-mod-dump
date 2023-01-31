/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI;

namespace GameboyArcade
{
    class Content
    {
        public string Name;
        public string ID;
        public string FilePath;
        public string DGAID;
        public bool EnableEvents = false;
        public string SaveStyle = "LOCAL";
        public string LinkStyle = "NONE";
        public string SoundStyle = "NONE";

        [JsonIgnore]
        public IContentPack ContentPack;

        [JsonIgnore]
        public string UniqueID;
    }
}
