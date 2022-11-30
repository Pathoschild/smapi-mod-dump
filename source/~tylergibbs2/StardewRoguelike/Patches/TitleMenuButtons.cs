/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Reflection;
using static StardewValley.Menus.LoadGameMenu;

namespace StardewRoguelike.Patches
{
    class TitleMenuButtons : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(TitleMenu), "setUpIcons");

        public static void Postfix(TitleMenu __instance)
        {
            __instance.buttons.Clear();
            int buttonWidth = 222;
            int buttonMarginRight = 24;
            int mainButtonSetWidth = buttonWidth + (buttonWidth + buttonMarginRight) * 2;
            int curx = (__instance.width / 2) - (mainButtonSetWidth / 2);
            __instance.buttons.Add(new ClickableTextureComponent("Load", new Rectangle(curx, __instance.height - 174 - 24, 222, 174), null, "", __instance.titleButtonsTexture, new Rectangle(74, 187, 74, 58), 3f)
            {
                myID = 81116,
                leftNeighborID = 81115,
                rightNeighborID = -7777,
                upNeighborID = 81111
            });
            curx += buttonWidth + buttonMarginRight;
            __instance.buttons.Add(new ClickableTextureComponent("Co-op", new Rectangle(curx, __instance.height - 174 - 24, 222, 174), null, "", __instance.titleButtonsTexture, new Rectangle(148, 187, 74, 58), 3f)
            {
                myID = 81119,
                leftNeighborID = 81116,
                rightNeighborID = 81117
            });
            curx += buttonWidth + buttonMarginRight;
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
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(TitleMenu), "update");

        public static bool Prefix(TitleMenu __instance)
        {
            int buttonsToShow = (int)__instance.GetType().GetField("buttonsToShow", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(__instance)!;
            if (buttonsToShow == 3)
                __instance.GetType().GetField("buttonsToShow", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(__instance, 4);

            return true;
        }
    }

    class CoopMenuNoNewFarm : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(CoopMenu), "connectionFinished");

        public static void Postfix(CoopMenu __instance)
        {

            List<MenuSlot> slots = (List<MenuSlot>)__instance.GetType().GetField("hostSlots", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(__instance)!;
            slots.RemoveAll(s => s.GetType().Name == "HostNewFarmSlot");
        }
    }
}
