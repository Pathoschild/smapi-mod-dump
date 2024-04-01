/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace OpenFolder
{
    public class ModConfig
    {
        public bool ModEnabled { get; set; } = true;
        public bool AddGameFolderButton { get; set; } = false;
        public bool AddModsFolderButton { get; set; } = true;
        public bool AddGMCMModsFolderButton { get; set; } = true;
    }
}
