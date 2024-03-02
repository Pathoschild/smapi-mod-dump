/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/nrobinson12/StardewValleyMods
**
*************************************************/

namespace StaticInventory
{
    public sealed class ModData
    {
        public int ToolbarOffset { get; set; } = 0;
        public bool IsShifted { get; set; } = false;

        public ModData() {}

        public ModData(int toolbarOffset, bool isShifted)
        {
            ToolbarOffset = toolbarOffset;
            IsShifted = isShifted;
        }
    }
}
