/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Zamiell/stardew-valley-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using xTile.Dimensions;

namespace EatDrinkFromInventory
{
    public class ModEntry : Mod
    {
        // Variables
        bool usedHotkeyToEat = false;
        bool isEating = false;
        int facingDirectionBeforeEating = 0;
        bool shouldMountHorse = false;
        GameLocation shouldMountHorseLocation = Game1.currentLocation;

        private ModConfig config = new();

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
            {
                return;
            }

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => config = new ModConfig(),
                save: () => this.Helper.WriteConfig(config)
            );

            configMenu.AddKeybindList(
                this.ModManifest,
                () => config.Hotkey,
                (KeybindList val) => config.Hotkey = val,
                () => "Consume Hotkey",
                () => "The hotkey to consume the food/drink that the cursor is over."
            );
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            CheckEating();
            CheckHorseAppeared();
        }

        private void CheckEating()
        {
            var oldIsEating = isEating;
            var newIsEating = Game1.player.isEating;
            isEating = newIsEating;

            if (oldIsEating && !newIsEating)
            {
                Game1.player.FacingDirection = facingDirectionBeforeEating;

                if (usedHotkeyToEat)
                {
                    usedHotkeyToEat = false;
                    EmulatePause();
                }
            }
        }

        private void CheckHorseAppeared()
        {
            if (!shouldMountHorse)
            {
                return;
            }

            var oldLocation = shouldMountHorseLocation;
            var newLocation = Game1.currentLocation;
            shouldMountHorseLocation = newLocation;

            if (oldLocation != newLocation)
            {
                shouldMountHorse = false;
                return;
            }

            Horse? potentialHorse = GetHorse(newLocation);

            if (
                Game1.player.canMove
                && potentialHorse is Horse horse
                && horse.Tile.X == Game1.player.Tile.X
                && horse.Tile.Y == Game1.player.Tile.Y
            )
            {
                shouldMountHorse = false;
                horse.checkAction(Game1.player, Game1.currentLocation);
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (config.Hotkey.IsDown())
            {
                ConsumeItemThatCursorIsOver();
            }
        }

        private void ConsumeItemThatCursorIsOver()
        {
            if (Game1.activeClickableMenu is not GameMenu gameMenu) {
                return;
            }

            if (gameMenu.pages.Count == 0)
            {
                return;
            }

            var firstPage = gameMenu.pages[0];
            if (firstPage is not InventoryPage inventoryPage)
            {
                return;
            }

            if (inventoryPage.hoveredItem is StardewValley.Object obj)
            {
                PotentialUseObject(obj);
            } else if (inventoryPage.hoveredItem is Tool tool)
            {
                PotentiallyUseTool(tool);
            }
        }

        private void PotentialUseObject(StardewValley.Object obj)
        {
            if (obj.Edibility > 0)
            {
                EatObject(obj);
            }
            else if (obj.Name == "Staircase" && Game1.currentLocation is MineShaft mineShaft)
            {
                DecrementStack(obj);

                // From: MineShaft.cs
                Game1.enterMine(mineShaft.mineLevel + 1);
                Game1.playSound("stairsdown");
                Game1.activeClickableMenu = null;
            }
            else if (obj.Name == "Warp Totem: Farm")
            {
                DecrementStack(obj);

                // From: Object::totemWarpForReal
                if (!Game1.getFarm().TryGetMapPropertyAs("WarpTotemEntry", out Point warp_location, false))
                {
                    switch (Game1.whichFarm)
                    {
                        case 6:
                            warp_location = new Point(82, 29);
                            break;
                        case 5:
                            warp_location = new Point(48, 39);
                            break;
                        default:
                            warp_location = new Point(48, 7);
                            break;
                    }
                }
                Game1.warpFarmer("Farm", warp_location.X, warp_location.Y, false);
            }
            else if (obj.Name == "Warp Totem: Mountains")
            {
                DecrementStack(obj);

                // From: Object::totemWarpForReal
                Game1.warpFarmer("Mountain", 31, 20, false);
            }
            else if (obj.Name == "Warp Totem: Beach")
            {
                DecrementStack(obj);

                // From: Object::totemWarpForReal
                Game1.warpFarmer("Beach", 20, 4, false);
            }
            else if (obj.Name == "Warp Totem: Desert")
            {
                DecrementStack(obj);

                // From: Object::totemWarpForReal
                Game1.warpFarmer("Desert", 35, 43, false);
            }
            else if (obj.Name == "Warp Totem: Island")
            {
                DecrementStack(obj);

                // From: Object::totemWarpForReal
                Game1.warpFarmer("IslandSouth", 11, 11, false);
            }
            else if (obj.Name == "Horse Flute")
            {
                // We can invoke the vanilla method by using: obj.performUseAction(Game1.currentLocation)
                // However, we don't want to wait for the song to play, so we reimplement the logic.
                bool success = PerformUseActionHorseFlute(Game1.currentLocation);
                if (success)
                {
                    Game1.activeClickableMenu = null;
                    shouldMountHorse = true;
                    shouldMountHorseLocation = Game1.currentLocation;
                }
            }
        }

        // Edited from vanilla:
        // - Removed increasing the stack.
        // - Removed the animation by invoking the event directly.
        private bool PerformUseActionHorseFlute(GameLocation location)
        {
            bool normalGameplay = (
                !Game1.eventUp
                && !Game1.isFestival()
                && !Game1.fadeToBlack
                && !Game1.player.swimming.Value
                && !Game1.player.bathingClothes.Value
                && !Game1.player.onBridge.Value
            );
            if (!normalGameplay)
            {
                return false;
            }

            string warpError = Utility.GetHorseWarpErrorMessage(Utility.GetHorseWarpRestrictionsForFarmer(Game1.player));
            if (warpError != null)
            {
                Game1.showRedMessage(warpError);
                return false;
            }

            Horse? horse = GetHorse(location);
            if (
                horse == null
                || Math.Abs(Game1.player.TilePoint.X - horse.TilePoint.X) > 1
                || Math.Abs(Game1.player.TilePoint.Y - horse.TilePoint.Y) > 1
            )
            {
                Game1.player.team.requestHorseWarpEvent.Fire(Game1.player.UniqueMultiplayerID);
            }
            return true;
        }

        private Horse? GetHorse(GameLocation location)
        {
            foreach (NPC character in location.characters)
            {
                if (character is Horse curHorse && curHorse.getOwner() == Game1.player)
                {
                    return curHorse;
                }
            }

            return null;
        }

        private void PotentiallyUseTool(Tool tool)
        {
            if (tool.Name == "Return Scepter")
            {
                // Ensure that the horse is dismounted.
                Game1.player.mount?.dismount();

                // The "wandWarpForReal" method is private. Invoking it via reflection causes a run-time error, so we instead copy paste the function here.
                // From: Wand::wandWarpForReal
                FarmHouse home = Utility.getHomeOfFarmer(Game1.player);
                if (home != null)
                {
                    Point position = home.getFrontDoorSpot();
                    Game1.warpFarmer("Farm", position.X, position.Y, false);
                    Game1.fadeToBlackAlpha = 0.99f;
                    Game1.screenGlow = false;
                    Game1.player.temporarilyInvincible = false;
                    Game1.player.temporaryInvincibilityTimer = 0;
                    Game1.displayFarmer = true;
                    Game1.player.CanMove = true;
                }
            }
        }

        private void EatObject(StardewValley.Object obj)
        {
            usedHotkeyToEat = true;
            facingDirectionBeforeEating = Game1.player.FacingDirection;

            DecrementStack(obj);
            Game1.player.eatObject(obj);
            Game1.activeClickableMenu = null;
        }

        private void DecrementStack(StardewValley.Object obj)
        {
            if (obj.Stack > 1)
            {
                obj.Stack--;
            }
            else
            {
                // Cannot use "Items.Remove" since it causes other items to slide around.
                Game1.player.Items.RemoveButKeepEmptySlot(obj);
            }
        }

        private void EmulatePause()
        {
            if (Game1.activeClickableMenu is null)
            {
                Game1.activeClickableMenu = new GameMenu();
            }
        }

        private void Log(string msg)
        {
            this.Monitor.Log(msg, LogLevel.Debug);
        }
    }
}
