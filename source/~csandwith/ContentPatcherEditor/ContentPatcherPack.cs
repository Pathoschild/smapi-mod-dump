/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;

namespace ContentPatcherEditor
{
    public class ContentPatcherPack
    {
        public MyManifest manifest;
        public ContentPatcherContent content;
        public string directory;
        public List<KeyValuePair<string, ConfigVar>> config;
    }
}