/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;

namespace ContentPackCreator
{
    public class ContentPatcherData
    {
        public string Format;
        public List<ChangeData> Changes = new List<ChangeData>();
        public Dictionary<string, ConfigData> ConfigSchema = new Dictionary<string, ConfigData>();

    }
}