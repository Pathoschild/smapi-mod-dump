/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/kqrse/StardewValleyMods-aedenthorn
**
*************************************************/

using System.Collections.Generic;

namespace ChestFullnessTextures
{
    internal class TempJson
    {
        public string Format = "1.28.4";
        public List<Temp2Json> Changes = new();
    }

    public class Temp2Json
    {
        public string Action;
        public string Target;
        public string FromFile;
        public Dictionary<string, ChestTextureDataShell> Entries;
    }
}