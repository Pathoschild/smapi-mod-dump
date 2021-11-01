/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using StardewValley.Menus;

namespace BattleRoyale.Patches
{
    class DisableHatRemoval : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(InventoryPage), "receiveLeftClick");

        public static bool Prefix(InventoryPage __instance, int x, int y)
        {
            foreach (ClickableComponent c in __instance.equipmentIcons)
            {
                if (!c.containsPoint(x, y))
                {
                    continue;
                }
                if (c.name == "Hat")
                    return false;
            }

            return true;
        }
    }
}
