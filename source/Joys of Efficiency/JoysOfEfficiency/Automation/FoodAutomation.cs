/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using System.Linq;
using JoysOfEfficiency.Core;
using StardewValley;
using StardewValley.Tools;
using SVObject = StardewValley.Object;

namespace JoysOfEfficiency.Automation
{
    internal class FoodAutomation
    {
        private static Config Config => InstanceHolder.Config;

        public static void TryToEatIfNeeded(Farmer player)
        {
            if (player.isEating || Game1.activeClickableMenu != null)
            {
                return;
            }
            if (player.CurrentTool != null && player.CurrentTool is FishingRod rod)
            {
                if (rod.inUse() && !player.UsingTool)
                {
                    return;
                }
            }

            if (!(player.Stamina <= player.MaxStamina * Config.StaminaToEatRatio) &&
                !(player.health <= player.maxHealth * Config.HealthToEatRatio))
            {
                return;
            }

            SVObject itemToEat = null;
            foreach (SVObject item in player.Items.OfType<SVObject>())
            {
                if (item.Edibility <= 0)
                    continue;

                //It's a edible item
                if (itemToEat == null || itemToEat.Edibility / itemToEat.salePrice() < item.Edibility / item.salePrice())
                {
                    //Found good edibility per price or just first food
                    itemToEat = item;
                }
            }

            if (itemToEat == null)
            {
                return;
            }

            player.eatObject(itemToEat);
            itemToEat.Stack--;
            if (itemToEat.Stack == 0)
            {
                player.removeItemFromInventory(itemToEat);
            }
        }
    }
}
