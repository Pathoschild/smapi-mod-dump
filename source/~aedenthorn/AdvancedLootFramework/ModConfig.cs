/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

namespace AdvancedLootFramework
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int[] ForbiddenWeapons { get; set; } = { 32, 34 };
        public int[] ForbiddenBigCraftables { get; set; } = { 22, 23, 101 };
        public int[] ForbiddenObjects { get; set; } = {  };
    }
}
