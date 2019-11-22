using System;
using System.Reflection;
using StardewValley;
using StardewValley.Menus;
using Bookcase.Events;
using Microsoft.Xna.Framework.Graphics;


namespace Bookcase.Patches {

    class TooltipPatch : IGamePatch {

        public Type TargetType => typeof(IClickableMenu);

        public MethodBase TargetMethod => TargetType.GetMethod("drawToolTip");

        public static void Prefix(SpriteBatch b, ref string hoverText, ref string hoverTitle, Item hoveredItem, ref int healAmountToDisplay, ref int currencySymbol, ref int moneyAmountToShowAtBottom) {

            ItemTooltipEvent theEvent = new ItemTooltipEvent(b, hoveredItem, hoverTitle, hoverText, healAmountToDisplay, currencySymbol, moneyAmountToShowAtBottom);
            BookcaseEvents.OnItemTooltip.Post(theEvent);

            hoverTitle = theEvent.Title;
            hoverText = theEvent.Description;
            healAmountToDisplay = theEvent.HealAmount;
            currencySymbol = theEvent.CurrencySymbol;
            moneyAmountToShowAtBottom = theEvent.MoneyToShow;
        }
    }
}
