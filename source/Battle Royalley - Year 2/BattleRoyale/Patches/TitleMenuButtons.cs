/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Collections.Generic;
using static StardewValley.Menus.LoadGameMenu;

namespace BattleRoyale.Patches
{
    class TitleMenuButtons : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(TitleMenu), "setUpIcons");

        public static void Postfix(TitleMenu __instance)
        {
            ModEntry.BRGame.Helper.Reflection.GetField<int>(__instance, "buttonsToShow");

            __instance.buttons.Clear();
            int buttonWidth = 74;
            int mainButtonSetWidth = buttonWidth * 4 * 3;
            mainButtonSetWidth += 72;
            int curx = __instance.width / 2 - mainButtonSetWidth / 2;
            curx += (buttonWidth + 8) * 3;
            __instance.buttons.Add(new ClickableTextureComponent("Co-op", new Rectangle(curx, __instance.height - 174 - 24, 222, 174), null, "", __instance.titleButtonsTexture, new Rectangle(148, 187, 74, 58), 3f)
            {
                myID = 81119,
                leftNeighborID = 81116,
                rightNeighborID = 81117
            });
            curx += (buttonWidth + 8) * 3;
            __instance.buttons.Add(new ClickableTextureComponent("Exit", new Rectangle(curx, __instance.height - 174 - 24, 222, 174), null, "", __instance.titleButtonsTexture, new Rectangle(222, 187, 74, 58), 3f)
            {
                myID = 81117,
                leftNeighborID = 81119,
                rightNeighborID = 81118,
                upNeighborID = 81111
            });
        }
    }

    class TitleMenuButtonSounds : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(TitleMenu), "update");

        public static bool Prefix(TitleMenu __instance)
        {
            int buttonsToShow = ModEntry.BRGame.Helper.Reflection.GetField<int>(__instance, "buttonsToShow").GetValue();
            if (buttonsToShow == 2)
                ModEntry.BRGame.Helper.Reflection.GetField<int>(__instance, "buttonsToShow").SetValue(4);

            return true;
        }
    }

    class CoopMenuNoNewFarm : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(CoopMenu), "connectionFinished");

        public static void Postfix(CoopMenu __instance)
        {

            List<MenuSlot> slots = ModEntry.BRGame.Helper.Reflection.GetField<List<MenuSlot>>(__instance, "hostSlots").GetValue();
            slots.RemoveAll(s => s.GetType().Name == "HostNewFarmSlot");
        }
    }
}
