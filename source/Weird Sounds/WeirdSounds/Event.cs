/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alcmoe/SVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Tools;

namespace WeirdSounds
{
    internal partial class Mod
    {
        private static string CueName(string key)
        {
            return WeirdSoundsLibrary.GetCueName(key);
        }
        
        private static void TimeChangeEvent(object? sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            switch (e.NewTime) {
                case 2400:
                    Game1.playSound(CueName("midnight2400"));
                    return;
                case 2500:
                    Game1.playSound(CueName("midnight2500"));
                    return;
                case 2600:
                    Game1.playSound(CueName("goodNight"));
                    return;
            }
        }
        
        private static readonly string[] JewelList = ["797", "62", "72", "60", "82", "84", "70", "74", "64", "68", "66"];
            
        private static void MenuChangedEvent(object? sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            switch (e.NewMenu) {
                // case GameMenu or ItemGrabMenu or JunimoNoteMenu when e.OldMenu?.GetType() != e.NewMenu.GetType():
                    // Game1.playSound(CueName("inventory"));
                    // return;
                case BobberBar { treasure: true } bBar:
                    DelayedAction.playSoundAfterDelay(CueName("treasureBox"), (int) bBar.treasureAppearTimer);
                    return;
                case ItemGrabMenu { context: FishingRod } igm: {
                    var price = 0;
                    var jewel = false;
                    foreach (var tr in igm.ItemsToGrabMenu.actualInventory) {
                        price += tr.sellToStorePrice() * tr.Stack;
                        if (JewelList.Any(jewelId => tr.ItemId == jewelId)) {
                            jewel = true;
                        }
                    }
                    if (price >= 700) {
                        Game1.playSound(CueName("treasure"));
                    } else if (jewel){
                        Game1.playSound(CueName("jewel"));
                    }
                    break;
                }
            }
        }
            
        private static void DayStartedEvent(object? sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            Mutex.DailyClearCache();
            if (Game1.dayTimeMoneyBox.moneyDial.previousTargetValue - Game1.dayTimeMoneyBox.moneyDial.currentValue >= 1000000) {
                DelayedAction.playSoundAfterDelay(CueName("million"), 1500);
            }
        }
        
        private static void OneSecondUpdateTickingEvent(object? sender, StardewModdingAPI.Events.OneSecondUpdateTickingEventArgs e)
        {
            if (!Context.IsWorldReady) {
                return;
            }
            const int timeout = 300 * 1000;
            if (Game1.player.timerSinceLastMovement % timeout >= timeout - 1 * 1000) {
                Game1.playSound(CueName("timeout"));
            }
            if (Game1.player.currentLocation is MineShaft { locationContextId: "Desert" } ms) {
                foreach (var npc in ms.characters) {
                    if (npc is not Serpent s || !s.withinPlayerThreshold() || Mutex.SerpentBarkDictionary.ContainsKey(s.GetHashCode()) || !Game1.viewport.Contains(new xTile.Dimensions.Location((int)s.Position.X, (int)s.Position.Y))) {
                        continue;
                    }
                    Game1.playSound(CueName("IAmComing"));
                    Mutex.SerpentBarkDictionary.Add(s.GetHashCode(), true);
                }
            }
            if (Game1.player.currentLocation is not (Farm or FarmHouse)) {
                return;
            }
            foreach (var npc in Game1.player.currentLocation.characters.Where(p => p is Pet pet && pet.petType.Value =="Cat")) {
                if (npc is not Pet pet) {
                    continue;
                }
                if (!Mutex.CatFlopDictionary.ContainsKey(pet.GetHashCode())) {
                    Mutex.CatFlopDictionary.Add(pet.GetHashCode(), true);
                }
                if (pet.CurrentBehavior == "Flop") {
                    if (!Mutex.CatFlopDictionary[pet.GetHashCode()]) {
                        continue;
                    }
                    if (Vector2.Distance(pet.Position, Game1.player.Position) < 16 * 6) {
                        Game1.playSound(CueName("sleep"));
                    }
                    Mutex.CatFlopDictionary[pet.GetHashCode()] = false;
                } else {
                    Mutex.CatFlopDictionary[pet.GetHashCode()] = true;
                }
            }
        }       
            
        private static void WarpedEvent(object? sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (e.NewLocation is AdventureGuild && e.Player == Game1.player) {
                Game1.playSound(CueName("sell"));
            }
        }

        private static void UpdateTickedEvent(object? sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) {
                return;
            }
            if (Game1.player.ActiveItem is MeleeWeapon weapon) {
                if (Mutex.WeaponAnimate.Any(i => i == Game1.player.FarmerSprite.currentSingleAnimation) && Game1.player.FarmerSprite.currentAnimationIndex < 1) {
                    if (!Mutex.WeaponMutex) {
                        return;
                    }
                    switch (weapon.type.Value) {
                        case MeleeWeapon.dagger when weapon.isOnSpecial:
                            if (MeleeWeapon.daggerHitsLeft == 3) {
                                Game1.playSound(CueName("daggerSpecial"));
                            }
                            break;
                        case MeleeWeapon.defenseSword when weapon.isOnSpecial:
                            Game1.playSound(CueName("defense"));
                            var toolLocation = Game1.player.GetToolLocation();
                            var zero1 = Vector2.Zero;
                            var zero2 = Vector2.Zero;
                            var areaOfEffect = weapon.getAreaOfEffect((int)toolLocation.X, (int)toolLocation.Y, Game1.player.FacingDirection, ref zero1, ref zero2, Game1.player.GetBoundingBox(), 1);
                            areaOfEffect.Inflate(50, 50);
                            if (Game1.player.currentLocation.getAllFarmAnimals().Any(animal => animal.GetBoundingBox().Intersects(areaOfEffect))) {
                                Game1.playSound(CueName("defenseHa"));
                            }
                            break;
                        case MeleeWeapon.club when weapon.isOnSpecial:
                            Game1.playSound(CueName("clubSmash"));
                            return;
                        case MeleeWeapon.club:
                        case MeleeWeapon.dagger:
                        case MeleeWeapon.defenseSword:
                            Game1.playSound(CueName("tool"));
                            break;
                    }
                    Mutex.WeaponMutex = false;
                } else {
                    Mutex.WeaponMutex = true;
                }
            } else {
                if (Game1.player.UsingTool) {
                    if (!Mutex.ToolMutex) {
                        return;
                    }
                    Mutex.ToolMutex = false;
                    if (Game1.player.ActiveItem is FishingRod or WateringCan) {
                        return;
                    }
                    Game1.playSound(CueName("tool"));
                } else {
                    Mutex.ToolMutex = true;
                }
            }
            if (Game1.player.currentLocation is not Farm && Game1.player.currentLocation is not AnimalHouse) {
                return;
            }
            foreach (var animal in Game1.player.currentLocation.getAllFarmAnimals().Where(animal => animal.type.Value.EndsWith("Chicken") && animal.wasPet.Value && !Mutex.CluckMutex.ContainsKey(animal.GetHashCode()))) {
                Game1.playSound(CueName("cluck"), WeirdSoundsLibrary.Random.Next(-200, 300));
                Mutex.CluckMutex.Add(animal.GetHashCode(), true);
            }
        }

        private void ButtonPressedEvent(object? sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) {
                return;
            }
            if (e.Button != SButton.F5) {
                return;
            }
            Mutex.DisableMod = Mutex.DisableMod ? !EnableMod() : DisableMod();
        }        
    }
}