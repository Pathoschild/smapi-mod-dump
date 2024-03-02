/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using StardewValley.Menus;
using StardewValley.Objects;
using System.Collections.Generic;
using SVBundle = StardewValley.Menus.Bundle;
using SVItem = StardewValley.Item;
using SVObject = StardewValley.Object;

namespace Randomizer
{
    public class OverriddenJunimoNoteMenu : JunimoNoteMenu
    {
        public OverriddenJunimoNoteMenu(int whichArea, Dictionary<int, bool[]> bundlesComplete)
          : base(whichArea, bundlesComplete)
        {
        }

        /// <summary>
        /// The current bundle being looed at
        /// </summary>
        public SVBundle CurrentPageBundle 
        { 
            get 
            {
                return Globals.ModRef.Helper.Reflection
                    .GetField<SVBundle>(this, "currentPageBundle", true)
                    .GetValue();
            } 
        }

        /// <summary>
        /// Whether we're looing at a specific bundle page or not
        /// </summary>
        public bool SpecificBundlePage
        {
            get
            {
                return Globals.ModRef.Helper.Reflection
                    .GetField<bool>(this, "specificBundlePage", true)
                    .GetValue();
            }
        }

        /// <summary>
        /// The current item on our cursor
        /// </summary>
        public SVItem HeldItem
        {
            get
            {
                return Globals.ModRef.Helper.Reflection
                    .GetField<SVItem>(this, "heldItem", true)
                    .GetValue();
            }
            set
            {
                Globals.ModRef.Helper.Reflection
                    .GetField<SVItem>(this, "heldItem", true)
                    .SetValue(value);
            }
        }

        /// <summary>
        /// This handles whether the UI will highlight or not when its hovered over with an item that can be deposited
        /// The issue is that a Ring object is NOT a StardewValley.Object, so we convert it to one and call the base version
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if the grid should be highlighted, false otherwise</returns>
        public override bool CanBePartiallyOrFullyDonated(SVItem item)
        {
            var itemToCheck = item;
            if (item is Ring)
            {
                itemToCheck = new SVObject(item.ParentSheetIndex, 1);
            }

            return base.CanBePartiallyOrFullyDonated(itemToCheck);
        }

        /// <summary>
        /// Handle the left click event on the menu
        /// Includes placing, dropping, and depositing items
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (!canClick || scrambledText)
            {
                return;
            }

            if (SpecificBundlePage)
            {
                SVItem clickedItem = inventory.getItemAt(x, y);
                if (clickedItem is Ring || HeldItem is Ring)
                {
                    if (!CurrentPageBundle.complete && CurrentPageBundle.completionTimer <= 0)
                    {
                        HeldItem = inventory.leftClick(x, y, HeldItem);
                    }
                        
                    // The ring was placed back into the inventory
                    if (HeldItem == null)
                    {
                        return;
                    }

                    // The entire problem here is that a Ring object is NOT an StardewValley.Object
                    // we can create a mock object here with the same index so it can pass the bundle check
                    // but DO NOT assign it back to HeldItem, or it will not be a Ring anymore!
                    var ringAsSVObject = new SVObject(HeldItem.ParentSheetIndex, 1);

                    // Check each required item in the bundle to see if it can be deposited
                    for (int index = 0; index < ingredientSlots.Count; ++index)
                    {
                        if (ingredientSlots[index].containsPoint(x, y))
                        {
                            if (CurrentPageBundle.canAcceptThisItem(ringAsSVObject, ingredientSlots[index]))
                            {
                                // This returns back a StardewValley.Object, but that is NOT a ring, so do NOT change HeldItem to it
                                if (CurrentPageBundle.tryToDepositThisItem(ringAsSVObject, ingredientSlots[index], "LooseSprites\\JunimoNote") == null)
                                {
                                    // The ring was deposited, so we shouldn't hold it anymore
                                    HeldItem = null;
                                }
                                Globals.ModRef.Helper.Reflection.GetMethod(this, "checkIfBundleIsComplete", true).Invoke();
                            }
                        }
                    }

                    return;
                }
            }

            base.receiveLeftClick(x, y, playSound);
        }
    }
}
