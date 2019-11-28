using System.Linq;
using JoysOfEfficiency.Core;
using JoysOfEfficiency.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using SVObject = StardewValley.Object;
using static StardewValley.Game1;

namespace JoysOfEfficiency.Automation
{
    internal class MachineOperator
    {
        public static void DepositIngredientsToMachines()
        {
            Farmer player = Game1.player;
            if (player.CurrentItem == null || !(Game1.player.CurrentItem is SVObject item))
            {
                return;
            }
            foreach (SVObject obj in Util.GetObjectsWithin<SVObject>(InstanceHolder.Config.MachineRadius).Where(IsObjectMachine))
            {
                Vector2 loc = Util.GetLocationOf(currentLocation, obj);
                if (obj.heldObject.Value != null)
                    continue;

                bool flag = false;
                bool accepted = obj.Name == "Furnace" ? CanFurnaceAcceptItem(item, player) : Utility.isThereAnObjectHereWhichAcceptsThisItem(currentLocation, item, (int)loc.X * tileSize, (int)loc.Y * tileSize);
                if (obj is Cask)
                {
                    if (ModEntry.IsCoGOn || ModEntry.IsCaOn)
                    {
                        if (obj.performObjectDropInAction(item, true, player))
                        {
                            obj.heldObject.Value = null;
                            flag = true;
                        }
                    }
                    else if (currentLocation is Cellar && accepted)
                    {
                        flag = true;
                    }
                }
                else if (accepted)
                {
                    flag = true;
                }
                if (!flag)
                    continue;

                obj.performObjectDropInAction(item, false, player);
                if (!(obj.Name == "Furnace" || obj.Name == "Charcoal Kiln") || item.Stack == 0)
                {
                    player.reduceActiveItemByOne();
                }

                return;
            }
        }

        public static void PullMachineResult()
        {
            Farmer player = Game1.player;
            foreach (SVObject obj in Util.GetObjectsWithin<SVObject>(InstanceHolder.Config.MachineRadius).Where(IsObjectMachine))
            {
                if (!obj.readyForHarvest.Value || obj.heldObject.Value == null)
                    continue;

                Item item = obj.heldObject.Value;
                if (player.couldInventoryAcceptThisItem(item))
                    obj.checkForAction(player);
            }
        }

        private static bool CanFurnaceAcceptItem(Item item, Farmer player)
        {
            if (player.getTallyOfObject(382, false) <= 0)
                return false;
            if (item.Stack < 5 && item.ParentSheetIndex != 80 && item.ParentSheetIndex != 82 && item.ParentSheetIndex != 330)
                return false;
            switch (item.ParentSheetIndex)
            {
                case 378:
                case 380:
                case 384:
                case 386:
                case 80:
                case 82:
                    break;
                default:
                    return false;
            }
            return true;
        }




        private static bool IsObjectMachine(SVObject obj)
        {
            if (obj is CrabPot)
                return true;

            if (!obj.bigCraftable.Value)
                return false;

            switch (obj.Name)
            {
                case "Incubator":
                case "Slime Incubator":
                case "Keg":
                case "Preserves Jar":
                case "Cheese Press":
                case "Mayonnaise Machine":
                case "Loom":
                case "Oil Maker":
                case "Seed Maker":
                case "Crystalarium":
                case "Recycling Machine":
                case "Furnace":
                case "Charcoal Kiln":
                case "Slime Egg-Press":
                case "Cask":
                case "Bee House":
                case "Mushroom Box":
                case "Statue Of Endless Fortune":
                case "Statue Of Perfection":
                case "Tapper":
                    return true;
                default: return false;
            }
        }
    }
}
