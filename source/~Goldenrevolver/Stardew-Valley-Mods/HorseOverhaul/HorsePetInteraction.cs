/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Menus;
using System.Linq;

namespace HorseOverhaul
{
    internal class HorsePetInteraction
    {
        public static bool CheckHorseInteraction(HorseOverhaul mod, Farmer who, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            if (who.currentLocation == null)
            {
                return false;
            }

            foreach (Horse horse in who.currentLocation.characters.OfType<Horse>())
            {
                if (horse == null || horse.IsTractor())
                {
                    continue;
                }

                // check if the interaction was a mouse click on a horse or a button press near a horse
                if (!horse.WithinRangeOfPlayer(mod, who)
                    || !horse.MouseOrPlayerIsInRange(mod, who, mouseX, mouseY, ignoreMousePosition))
                {
                    continue;
                }

                var horseW = mod.Horses.Where(h => h?.Horse?.HorseId == horse.HorseId).FirstOrDefault();

                if (horseW == null)
                {
                    continue;
                }

                if (who.CurrentItem != null && mod.Config.Feeding)
                {
                    Item currentItem = who.CurrentItem;

                    if (currentItem.QualifiedItemId == "(O)Carrot")
                    {
                        // don't combine the if statements, so we can fall into the saddle bag case

                        // prevent feeding, use for speed boost instead
                        if (!horse.ateCarrotToday)
                        {
                            return horse.checkAction(who, who.currentLocation);
                        }
                    }
                    else if (mod.Config.Feeding)
                    {
                        if (Feeding.CheckHorseFeeding(mod, who, horseW, who.CurrentItem))
                        {
                            return true;
                        }
                    }
                }

                if (Context.IsWorldReady && Context.CanPlayerMove && Context.IsPlayerFree)
                {
                    if (mod.Config.SaddleBag && SaddleBagAccess.HasAccessToSaddleBag(horse) && horseW.SaddleBag != null)
                    {
                        OpenSaddleBagWithDelay(horseW);

                        return true;
                    }
                }
            }

            return false;
        }

        // based on Chest.ShowMenu, but only allows interacting with content after one frame delay to prevent immediately transfering one item (mostly when using controller)
        // also uses the context of type Stable instead of type Chest
        public static void OpenSaddleBagWithDelay(HorseWrapper horseW)
        {
            //Game1.activeClickableMenu = new ItemGrabMenu(
            Game1.activeClickableMenu = new HighlightDelayedItemGrabMenu(1,
                horseW.SaddleBag.GetItemsForPlayer(), reverseGrab: false, showReceivingMenu: true, InventoryMenu.highlightAllItems, horseW.SaddleBag.grabItemFromInventory, null, horseW.SaddleBag.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, horseW.SaddleBag.fridge.Value ? null : horseW.SaddleBag, -1, horseW.Stable);
        }

        public static bool CheckPetInteraction(HorseOverhaul mod, Farmer who, int mouseX, int mouseY, bool ignoreMousePosition)
        {
            if (!mod.Config.PetFeeding)
            {
                return false;
            }

            if (who.currentLocation == null)
            {
                return false;
            }

            foreach (NPC npc in who.currentLocation.characters)
            {
                if (npc is not Pet pet)
                {
                    continue;
                }

                // check if the interaction was a mouse click on a pet or a button press near a pet
                if (!pet.WithinRangeOfPlayer(mod, who)
                    || !pet.MouseOrPlayerIsInRange(mod, who, mouseX, mouseY, ignoreMousePosition))
                {
                    continue;
                }

                return Feeding.CheckPetFeeding(mod, who, pet, who.CurrentItem);
            }

            return false;
        }
    }
}