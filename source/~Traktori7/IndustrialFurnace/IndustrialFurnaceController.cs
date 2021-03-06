/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Menus;
using StardewValley.Buildings;


namespace IndustrialFurnace
{
    /// <summary>
    /// Furnace data handling class.
    /// </summary>
    public class IndustrialFurnaceController
    {
        private ModEntry mod;

        public readonly int ID;
        public bool CurrentlyOn;

        public Chest input = new Chest();
        public Chest output = new Chest();

        public Building furnace;
        public LightSource lightSource;


        public IndustrialFurnaceController(int tag, bool currentlyOn, ModEntry mod)
        {
            this.mod = mod;
            this.ID = tag;
            this.CurrentlyOn = currentlyOn;
        }


        public void ChangeCurrentlyOn(bool value)
        {
            CurrentlyOn = value;
        }


        public void AddItemsToSmelt(int objectId, int amount)
        {
            StardewValley.Object item = new StardewValley.Object(objectId, amount);
            input.addItem(item);
        }


        public void AddItemsToSmeltedChest(int objectId, int amount)
        {
            //Keep creating stacks of max size if needed to avoid going over it.
            while (amount > 0)
			{
                StardewValley.Object item = new StardewValley.Object(objectId, amount);

				if (item.Stack > item.maximumStackSize())
				{
                    item.Stack = item.maximumStackSize();
                    amount -= item.maximumStackSize();
				}
                else
				{
                    amount = 0;
				}

				if (item != null && Utility.canItemBeAddedToThisInventoryList(item, output.items, 36))
                {
                    output.addItem(item);
                }
            }
        }


        public void GrabItemFromChest(Item item, Farmer who)
        {
            if (!who.couldInventoryAcceptThisItem(item))
                return;

            TakeFromOutput(item, who);


            Game1.activeClickableMenu = (IClickableMenu)new ItemGrabMenu(
                output.items,
                false,
                true,
                new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),
                null,
                (string)null,
                (itemParam, farmer) => GrabItemFromChest(itemParam, farmer),
                false,
                true,
                true,
                true,
                false,
                0,
                null,
                -1,
                null);
        }


        public void TakeFromOutput(Item item, Farmer who = null)
        {
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                // Handle moving the items to the player's inventory since I have no idea how the android version handles its menus
                // Will most likely break at some point
                if (who != null && who.addItemToInventoryBool(item))
                {
                    output.items.Remove(item);
                    output.clearNulls();
                }
            }
            else
            {
                output.items.Remove(item);
                output.clearNulls();
            }

            mod.SendUpdateMessage();
        }
    }
}
