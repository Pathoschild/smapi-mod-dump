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