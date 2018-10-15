using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewMods.ToolUpgradeDeliveryService.Framework.Menus
{
    /// <summary>
    /// This class extends the in-game LetterViewerMenu page so you can pass the item to be added to the mail.
    /// It also adds fucntionaility to remove any instance of the tool included in the palyer's inventory 
    /// when the player adds a tool included in Clint's e-mail (Support for mod [Rented Tools])
    /// </summary>
    internal class LetterViewerMenuForToolUpgrade : LetterViewerMenu
    {
        public LetterViewerMenuForToolUpgrade(string text, Item item) : base(text)
        {
            ModEntry.CommonServices.ReflectionHelper.GetField<bool>(this, "isMail").SetValue(true);

            if (item == null)
            {
                return;
            }

            // Add item to email
            this.itemsToGrab.Add(new ClickableComponent(
                    new Rectangle(this.xPositionOnScreen + this.width / 2 - 48, this.yPositionOnScreen + this.height - 32 - 96, 96, 96),
                    item)
            {
                myID = 104,
                leftNeighborID = 101,
                rightNeighborID = 102
            });

            this.backButton.rightNeighborID = 104;
            this.forwardButton.leftNeighborID = 104;

            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {          
            foreach (ClickableComponent clickableComponent in this.itemsToGrab)
            {
                if (clickableComponent.containsPoint(x, y) && clickableComponent.item != null)
                {
                    string itemToDelete = null;
                    if (clickableComponent.item is Axe || clickableComponent.item is Pickaxe 
                        || clickableComponent.item is Hoe || clickableComponent.item is Shears || clickableComponent.item is WateringCan)
                    {
                        itemToDelete = (clickableComponent.item as Tool).BaseName;
                    }

                    if (itemToDelete != null)
                    {
                        var removableItems = Game1.player.Items.Where(item => (item is Tool) && (item as Tool).BaseName.Equals(itemToDelete));

                        foreach (var item in removableItems)
                        {
                            Game1.player.removeItemFromInventory(item);
                        }
                    }

                    Game1.playSound("coin");
                    Game1.player.addItemByMenuIfNecessary(clickableComponent.item);
                    clickableComponent.item = (Item)null;

                    Game1.player.toolBeingUpgraded.Value = null;

                    return;
                }
            }

            base.receiveLeftClick(x, y, playSound);
        }
    }
}
