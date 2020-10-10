/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Drachenkaetzchen/AdvancedKeyBindings
**
*************************************************/

using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace AdvancedKeyBindings.KeyHandlers
{
    public class ChestKeyHandler: IKeyHandler
    {
        public ChestKeyHandler(SButton[] addToStackButtons)
        {
            AddToStackButtons = addToStackButtons;
        }

        public SButton[] AddToStackButtons { get; }
        
        public bool ReceiveButtonPress(SButton input)
        {
            if (AddToStackButtons.Contains(input) && Game1.activeClickableMenu is ItemGrabMenu menu)
            {
                menu.FillOutStacks();
                Game1.playSound("Ship");
                return true;
            }
            return false;
        }
    }
}