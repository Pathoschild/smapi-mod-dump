/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewValley.Menus;

namespace CustomBackpack
{
    public interface ICustomBackpackApi
    {
        public bool SetPlayerSlots(int slots, bool force);
        public bool ChangeScroll(InventoryMenu menu, int delta);
    }
    public class CustomBackpackApi
    {
        public bool SetPlayerSlots(int slots, bool force)
        {
            return ModEntry.SetPlayerSlots(slots, force);
        }
        public bool ChangeScroll(InventoryMenu menu, int delta)
        {
            return ModEntry.ChangeScroll(menu, delta);
        }
    }
}