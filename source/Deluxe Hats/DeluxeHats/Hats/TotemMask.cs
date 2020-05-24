using System.Linq;
using StardewModdingAPI;
using StardewValley;

namespace DeluxeHats.Hats
{
    public static class TotemMask
    {
        public const string Name = "Totem Mask";
        public const string Description = "Chance not to consume totem on use.";

        private const float saveTotemChance = 0.40f;
        public static void Activate()
        {
            HatService.OnButtonPressed = (e) =>
            {
                if (e.IsDown(SButton.MouseRight) || e.IsDown(SButton.ControllerX))
                {
                    if (Game1.activeClickableMenu != null
                    || !Game1.displayFarmer
                    || Game1.menuUp
                    || Game1.dialogueUp
                    || !Game1.player.canMove
                    || Game1.player.isRidingHorse()
                    || Game1.isFestival()
                    || Game1.isWarping
                    || Game1.eventUp
                    || Game1.fadeIn
                    || Game1.player.isInBed
                    || Game1.nameSelectUp
                    || Game1.isActionAtCurrentCursorTile
                    || Game1.fadeToBlack)
                    {
                        return;
                    }
                    if (new string[] { "Warp Totem: Farm", "Warp Totem: Beach", "Warp Totem: Desert", "Rain Totem" }.Contains(Game1.player.CurrentItem.Name))
                    {
                        HatService.Helper.Input.Suppress(SButton.MouseRight);
                        if (Game1.player.ActiveObject.performUseAction(Game1.currentLocation))
                        {
                            if (!(Game1.random.NextDouble() < saveTotemChance))
                            {
                                Game1.player.reduceActiveItemByOne();
                            }
                        }
                    }
                }
            };
        }

        public static void Disable()
        {
            HatService.OnButtonPressed = null;
        }
    }
}
