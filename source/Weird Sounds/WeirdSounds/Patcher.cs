/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/alcmoe/SVMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Audio;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace WeirdSounds
{
    internal class Patcher
    {
        private static Harmony? _harmony;
        internal static void PatchAll(IMod mod)
        {
            _harmony ??= new Harmony(mod.ModManifest.UniqueID);
            _harmony.Patch(
                original: AccessTools.Method(typeof(SoundsHelper), nameof(SoundsHelper.PlayLocal)),
                prefix: new HarmonyMethod(typeof(Patcher), nameof(PlayLocalPrefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.PlaceInMachine)),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(PlaceInMachinePostfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(ClickableComponent), nameof(ClickableComponent.containsPoint), [typeof(int), typeof(int)]),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(ContainsPointPostfix))
            );        
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                postfix: new HarmonyMethod(typeof(Patcher), nameof(AnswerDialogueActionPostfix))
            );
        }

        internal static void UnpatchAll()
        {
            _harmony?.UnpatchAll();
        }

        private static string CueName(string key)
        {
            return WeirdSoundsLibrary.GetCueName(key);
        }
            
        private static bool PlayLocalPrefix(ISoundsHelper __instance, ref string cueName, GameLocation location, Vector2? position, int? pitch, SoundContext context)
        {
            switch (cueName) {
                case "clubSmash":
                    if (Game1.player.ActiveItem is MeleeWeapon club && club.type.Value == MeleeWeapon.club && club.isOnSpecial) {
                        return false;
                    }
                    break;
                case "cancel":
                    if (Game1.activeClickableMenu is PurchaseAnimalsMenu animalsMenu) {
                        var buildingAt = animalsMenu.TargetLocation.getBuildingAt(new Vector2((int) ((Utility.ModifyCoordinateFromUIScale(Game1.getMouseX()) + (double) Game1.viewport.X) / 64.0), (int) ((Utility.ModifyCoordinateFromUIScale(Game1.getMouseY()) + (double) Game1.viewport.Y) / 64.0)));
                        if (animalsMenu.animalBeingPurchased.CanLiveIn(buildingAt)) {
                            __instance.PlayLocal(CueName("noMoreAnimals"), location, position, pitch, context, out _); // building is full
                        } else {
                            __instance.PlayLocal(CueName("cancel"), location, position, pitch, context, out _); //wrong building for animal
                        }
                    } else {
                        __instance.PlayLocal(CueName("cancel"), location, position, pitch, context, out _);
                    }
                    break;
                case "trashcan":
                    if (Game1.activeClickableMenu is null) {
                        __instance.PlayLocal(CueName("trashcan"), location, position, pitch,context, out _);
                    }
                    break;
                case "death":
                    if (Game1.player.health <= 0) {
                        __instance.PlayLocal(CueName("death"), location, position, pitch, context, out _);
                    }
                    break;
                case "breathout":
                    if (Game1.player.health == 10 && Game1.player.Sprite.currentFrame == 5) {
                        Mutex.DeathMutex = true;
                    }
                    break;
                case "bigDeSelect":
                    if (Game1.activeClickableMenu is LevelUpMenu { isProfessionChooser: false }) { //selected profession
                        __instance.PlayLocal(CueName("ok"), location, position, pitch, context, out _);
                    }   
                    break;
                case "openChest":
                    if (Game1.didPlayerJustRightClick() && Game1.currentLocation.objects.TryGetValue(Game1.player.GetGrabTile(), out var obj)) {
                        if (obj is Chest chest && !chest.playerChest.Value) {
                            __instance.PlayLocal(CueName("trashcan"), location, position, pitch, context, out _);
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }
            
        private static void PlaceInMachinePostfix(SObject __instance, ref bool __result, bool probe)
        {
            if (__result && !probe) {
                __instance.Location.playSound(CueName("machine"), __instance.TileLocation);
            }
        }
            
        private static void ContainsPointPostfix(ClickableComponent __instance, ref bool __result)
        {
            if (!__result || __instance is not ClickableTextureComponent ok || !Game1.didPlayerJustLeftClick()) {
                return;
            }
            if (!ok.texture.Name.StartsWith("LooseSprites/Cursors")) {
                return;
            }
            if (ok.sourceRect is not { Left: 128, Top: 256, Height: 64, Width: 64 }){
                return;
            }
            if (Mutex.DeathMutex) {
                Game1.playSound(CueName("back2fight"));
                Mutex.DeathMutex = false;
            } else {
                Game1.playSound(CueName("ok"));
            }
        } 
            
        private static void AnswerDialogueActionPostfix(string questionAndAnswer)
        {
            switch (questionAndAnswer) {
                case "SquidFestBooth_Rewards":
                case "Museum_Collect":
                    Game1.playSound(CueName("reward"));
                    break;
                case "TroutDerbyBooth_Rewards":
                    if (Game1.delayedActions.Any(action => action.stringData == "getNewSpecialItem")) {
                        Game1.playSound(CueName("reward"));
                    }
                    break;
                case "SleepTent_Yes":
                case "Sleep_Yes":
                    Game1.playSound(CueName("goodNight"));
                    break;
            }
        }
    }
}