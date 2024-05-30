/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/AnythingAnywhere
**
*************************************************/

using Common.Helpers;
using Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xTile.Dimensions;

namespace AnythingAnywhere.Framework.Patches.Menus
{
    internal sealed class AnimalQueryMenuPatch : PatchHelper
    {
        private static GameLocation? TargetLocation;
        internal AnimalQueryMenuPatch() : base(typeof(AnimalQueryMenu)) { }
        internal void Apply()
        {
            Patch(PatchType.Prefix, nameof(AnimalQueryMenu.receiveLeftClick), nameof(ReceiveLeftClickPrefix), [typeof(int), typeof(int), typeof(bool)]);
            Patch(PatchType.Postfix, nameof(AnimalQueryMenu.performHoverAction), nameof(PerformHoverActionPostfix), [typeof(int), typeof(int)]);
            Patch(PatchType.Postfix, nameof(AnimalQueryMenu.prepareForAnimalPlacement), nameof(PrepareForAnimalPlacementPostfix));
            Patch(PatchType.Postfix, nameof(AnimalQueryMenu.receiveKeyPress), nameof(ReceiveKeyPressPostfix), [typeof(Keys)]);
        }

        private static bool ReceiveLeftClickPrefix(AnimalQueryMenu __instance, int x, int y, bool playSound = true)
        {
            if (!ModEntry.Config.EnableAnimalRelocate)
                return true;

            if (Game1.globalFade)
                return true;

            // TODO: FIX THIS AND ADD REFLECTION HELPER TO COMMON
            bool movingAnimal = (bool)typeof(AnimalQueryMenu).GetField("movingAnimal", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;
            FarmAnimal animal = (FarmAnimal)typeof(AnimalQueryMenu).GetField("animal", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;

            if (movingAnimal)
            {
                if (__instance.okButton?.containsPoint(x, y) == true)
                {
                    Game1.globalFadeToBlack(__instance.prepareForReturnFromPlacement);
                    Game1.playSound("smallSelect");
                }
                Vector2 clickTile = new((float)(Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (float)(Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
                Building? selection = TargetLocation?.getBuildingAt(clickTile);
                if (selection == null)
                {
                    return false;
                }
                if (animal.CanLiveIn(selection))
                {
                    AnimalHouse selectedHome = (AnimalHouse)selection.GetIndoors();
                    if (selectedHome.isFull())
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_BuildingFull"));
                        return false;
                    }
                    if (selection.Equals(animal.home))
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_AlreadyHome"));
                        return false;
                    }
                    AnimalHouse oldHome = (AnimalHouse)animal.home.GetIndoors();
                    if (TargetLocation != null && (oldHome.animals.Remove(animal.myID.Value) || TargetLocation.animals.Remove(animal.myID.Value)))
                    {
                        oldHome.animalsThatLiveHere.Remove(animal.myID.Value);
                        selectedHome.adoptAnimal(animal);
                    }
                    animal.makeSound();
                    Game1.globalFadeToBlack(__instance.finishedPlacingAnimal);
                }
                else
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:AnimalQuery_Moving_CantLiveThere", animal.shortDisplayType()));
                }
                return false;
            }

            bool confirmingSell = (bool)typeof(AnimalQueryMenu).GetField("confirmingSell", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;

            if (confirmingSell)
                return true;

            if (!__instance.moveHomeButton.containsPoint(x, y)) return true;
            List<KeyValuePair<string, string>> validLocations = [];
            Game1.playSound("smallSelect");

            // ReSharper disable once LoopCanBeConvertedToQuery -- so I can actually read the damn thing
            foreach (GameLocation location in Game1.locations)
            {
                if (location.buildings.Any(p => p.GetIndoors() is AnimalHouse) && (!Game1.IsClient || location.CanBeRemotedlyViewed()))
                {
                    validLocations.Add(new KeyValuePair<string, string>(location.NameOrUniqueName, location.DisplayName));
                }
            }
            if (validLocations.Count == 0)
            {
                Farm farm = Game1.getFarm();

                validLocations.Add(new KeyValuePair<string, string>(farm.NameOrUniqueName, farm.DisplayName));
            }
            Game1.currentLocation.ShowPagedResponses(I18n.Message("ChooseAnimalLocation"), validLocations, value =>
            {
                GameLocation locationFromName = Game1.getLocationFromName(value);

                if (locationFromName != null)
                {
                    TargetLocation = locationFromName;
                    if (Game1.activeClickableMenu is DialogueBox dialogueBox)
                    {
                        dialogueBox.closeDialogue();
                    }
                    Game1.activeClickableMenu = __instance;
                    Game1.globalFadeToBlack(__instance.prepareForAnimalPlacement);
                }
                else
                {
                    ModEntry.ModMonitor.Log("Can't find location '" + value + "' for animal relocate menu.", LogLevel.Error);
                }
            }, auto_select_single_choice: true);
            return false;
        }

        // Update current location to TargetLocation
        private static void PrepareForAnimalPlacementPostfix(AnimalQueryMenu __instance)
        {
            if (!ModEntry.Config.EnableAnimalRelocate) return;
            typeof(AnimalQueryMenu).GetField("movingAnimal", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(__instance, true);
            Game1.currentLocation.cleanupBeforePlayerExit();
            Game1.currentLocation = TargetLocation;
            Game1.player.viewingLocation.Value = Game1.currentLocation?.NameOrUniqueName;
            Game1.globalFadeToClear();
            __instance.okButton.bounds.X = Game1.uiViewport.Width - 128;
            __instance.okButton.bounds.Y = Game1.uiViewport.Height - 128;
            Game1.displayHUD = false;
            Game1.viewportFreeze = true;
            Game1.viewport.Location = new Location(3136, 320);
            Game1.panScreen(0, 0);
            Game1.currentLocation?.resetForPlayerEntry();
            Game1.displayFarmer = false;
        }

        // Show correct hover colors
        private static void PerformHoverActionPostfix(AnimalQueryMenu __instance, int x, int y)
        {
            if (!ModEntry.Config.EnableAnimalRelocate)
                return;

            bool movingAnimal = (bool)typeof(AnimalQueryMenu).GetField("movingAnimal", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;
            FarmAnimal animal = (FarmAnimal)typeof(AnimalQueryMenu).GetField("animal", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;

            if (TargetLocation != null && (!movingAnimal || TargetLocation.Equals(Game1.getFarm()))) return;

            Vector2 clickTile = new((float)(Game1.viewport.X + Game1.getOldMouseX(ui_scale: false)) / 64, (float)(Game1.viewport.Y + Game1.getOldMouseY(ui_scale: false)) / 64);
            if (TargetLocation == null) return;
            foreach (Building building in TargetLocation.buildings)
            {
                building.color = Color.White;
            }

            Building selection = TargetLocation.getBuildingAt(clickTile);

            if (selection == null) return;

            if (animal.CanLiveIn(selection) && !((AnimalHouse)selection.GetIndoors()).isFull() &&
                !selection.Equals(animal.home))
            {
                selection.color = Color.LightGreen * 0.8f;
            }
            else
            {
                selection.color = Color.Red * 0.8f;
            }
        }

        // Enable WASD on move animal menu
        private static void ReceiveKeyPressPostfix(AnimalQueryMenu __instance, Keys key)
        {
            if (!ModEntry.Config.EnableAnimalRelocate) return;

            bool movingAnimal = (bool)typeof(AnimalQueryMenu).GetField("movingAnimal", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(__instance)!;
            if (!movingAnimal) return;

            if (Game1.options.doesInputListContain(Game1.options.moveDownButton, key))
            {
                Game1.panScreen(0, 4);
            }
            else if (Game1.options.doesInputListContain(Game1.options.moveRightButton, key))
            {
                Game1.panScreen(4, 0);
            }
            else if (Game1.options.doesInputListContain(Game1.options.moveUpButton, key))
            {
                Game1.panScreen(0, -4);
            }
            else if (Game1.options.doesInputListContain(Game1.options.moveLeftButton, key))
            {
                Game1.panScreen(-4, 0);
            }
        }
    }
}