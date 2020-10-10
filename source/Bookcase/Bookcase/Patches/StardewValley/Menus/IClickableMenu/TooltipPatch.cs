/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

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
