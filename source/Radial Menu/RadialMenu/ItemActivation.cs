/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using RadialMenu.Config;
using StardewValley;
using StardewValley.Locations;

namespace RadialMenu
{
    internal enum ItemActivationResult { Consumed, Selected, Custom, Ignored, Delayed }

    internal static class FuzzyActivation
    {
        public static ItemActivationResult ConsumeOrSelect(
            int itemIndex,
            DelayedActions? delayedActions = null,
            ItemAction preferredAction = ItemAction.Use)
        {
            if (delayedActions == DelayedActions.All)
            {
                return ItemActivationResult.Delayed;
            }
            var item = itemIndex >= 0 && itemIndex < Game1.player.Items.Count
                ? Game1.player.Items[itemIndex]
                : null;
            if (item is null || (preferredAction == ItemAction.Use && TryConsume(item)))
            {
                return ItemActivationResult.Consumed;
            }
            if (delayedActions == DelayedActions.ToolSwitch)
            {
                return ItemActivationResult.Delayed;
            }
            Game1.player.CurrentToolIndex = itemIndex;
            if (Game1.player.CurrentTool is not null)
            {
                Game1.playSound("toolSwap");
            }
            return ItemActivationResult.Selected;
        }

        private static bool TryConsume(Item item)
        {
            if (item.Name == "Staircase" && Game1.currentLocation is MineShaft mineShaft)
            {
                ReduceStack(item);
                Game1.enterMine(mineShaft.mineLevel + 1);
                Game1.playSound("stairsdown");
                return true;
            }
            if (item is not StardewValley.Object obj)
            {
                return false;
            }
            if (obj.Edibility > 0)
            {
                ReduceStack(obj);
                Game1.player.eatObject(obj);
                return true;
            }
            if (obj.performUseAction(Game1.currentLocation))
            {
                ReduceStack(obj);
                return true;
            }
            return false;
        }

        private static void ReduceStack(Item item)
        {
            if (item.Stack > 1)
            {
                item.Stack--;
            }
            else
            {
                Game1.player.Items.RemoveButKeepEmptySlot(item);
            }
        }
    }
}
