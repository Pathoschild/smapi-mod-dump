/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewArchipelago.Archipelago;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewArchipelago.GameModifications
{
    public class AdvancedOptionsManager
    {
        private static ModEntry _modEntry;
        private static IMonitor _console;
        private static IModHelper _modHelper;
        private Harmony _harmony;
        private static ArchipelagoClient _archipelago;

        public AdvancedOptionsManager(ModEntry modEntry, IMonitor console, IModHelper modHelper, Harmony harmony, ArchipelagoClient archipelago)
        {
            _modEntry = modEntry;
            _console = console;
            _modHelper = modHelper;
            _harmony = harmony;
            _archipelago = archipelago;
        }

        public void InjectArchipelagoAdvancedOptions()
        {
            InjectAdvancedOptionsRemoval();
            InjectNewGameForcedSettings();
            InjectArchipelagoConnectionFields();
        }

        private void InjectAdvancedOptionsRemoval()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(CharacterCustomization), "setUpPositions"),
                postfix: new HarmonyMethod(typeof(AdvancedOptionsManager), nameof(SetUpPositions_RemoveAdvancedOptionsButton_Postfix))
            );
        }

        private void InjectNewGameForcedSettings()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.loadForNewGame)),
                prefix: new HarmonyMethod(typeof(AdvancedOptionsManager), nameof(LoadForNewGame_ForceSettings_Prefix))
            );
        }

        private void InjectArchipelagoConnectionFields()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.update)),
                postfix: new HarmonyMethod(typeof(AdvancedOptionsManager), nameof(TitleMenuUpdate_ReplaceCharacterMenu_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.canLeaveMenu)),
                prefix: new HarmonyMethod(typeof(AdvancedOptionsManager), nameof(CanLeaveMenu_ConsiderNewFields_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(CharacterCustomization), "optionButtonClick"),
                prefix: new HarmonyMethod(typeof(AdvancedOptionsManager), nameof(OptionButtonClick_OkConnectToAp_Prefix))
            );
        }

        public static void SetUpPositions_RemoveAdvancedOptionsButton_Postfix(CharacterCustomization __instance)
        {
            try
            {
                if (__instance?.advancedOptionsButton == null)
                {
                    return;
                }

                __instance.advancedOptionsButton.visible = false;
            }
            catch (Exception ex)
            {
                _modEntry.Monitor.Log($"Failed in {nameof(SetUpPositions_RemoveAdvancedOptionsButton_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static bool LoadForNewGame_ForceSettings_Prefix(bool loadedGame = false)
        {
            try
            {
                if (!_archipelago.IsConnected)
                {
                    return true; // run original logic
                }

                ForceGameSeedToArchipelagoProvidedSeed();
                ForceFarmTypeToArchipelagoProvidedFarm();
                Game1.bundleType = Game1.BundleType.Default;
                Game1.game1.SetNewGameOption<bool>("YearOneCompletable", false);

                return true; // run original logic
            }
            catch (Exception ex)
            {
                _modEntry.Monitor.Log($"Failed in {nameof(LoadForNewGame_ForceSettings_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void ForceGameSeedToArchipelagoProvidedSeed()
        {
            var trimmedSeed = _archipelago.SlotData.Seed.Trim();
            
            int result = int.Parse(trimmedSeed.Substring(0, Math.Min(9, trimmedSeed.Length)));
            Game1.startingGameSeed = (ulong)result;
        }

        private static void ForceFarmTypeToArchipelagoProvidedFarm()
        {
            var farmTypes = new[]
            {
                "Standard Farm", "Riverland Farm", "Forest Farm", "Hill-top Farm", "Wilderness Farm", "Four Corners Farm", "Beach Farm",
            };

            // To remove once Beta 5 is done
            for (var i = 0; i < farmTypes.Length; i++)
            {
                if (_archipelago.HasReceivedItem(farmTypes[i]))
                {
                    Game1.whichFarm = i;
                    Game1.spawnMonstersAtNight = i == 4;
                    return;
                }
            }

            var farmType = _archipelago.SlotData.FarmType;
            Game1.whichFarm = (int)_archipelago.SlotData.FarmType;
            Game1.spawnMonstersAtNight = farmType == FarmType.Wilderness;
        }

        public static void TitleMenuUpdate_ReplaceCharacterMenu_Postfix(TitleMenu __instance, GameTime time)
        {
            try
            {
                if (!(TitleMenu.subMenu is CharacterCustomization characterMenu) || TitleMenu.subMenu is CharacterCustomizationArchipelago || characterMenu.source != CharacterCustomization.Source.NewGame)
                {
                    return;
                }

                var apCharacterMenu = new CharacterCustomizationArchipelago(characterMenu, _modHelper);
                TitleMenu.subMenu = apCharacterMenu;
            }
            catch (Exception ex)
            {
                _modEntry.Monitor.Log($"Failed in {nameof(TitleMenuUpdate_ReplaceCharacterMenu_Postfix)}:\n{ex}", LogLevel.Error);
                return;
            }
        }

        public static bool CanLeaveMenu_ConsiderNewFields_Prefix(CharacterCustomization __instance, ref bool __result)
        {
            try
            {
                if (!(__instance is CharacterCustomizationArchipelago apInstance))
                {
                    return true; // run original logic
                }

                __result = Game1.player.Name.Length > 0 &&
                           Game1.player.farmName.Length > 0 &&
                           Game1.player.favoriteThing.Length > 0 &&
                           apInstance.IpAddressTextBox.Text.Length > 0 &&
                           apInstance.SlotNameTextBox.Text.Length > 0 &&
                           apInstance.IpIsFormattedCorrectly();

                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                _modEntry.Monitor.Log($"Failed in {nameof(CanLeaveMenu_ConsiderNewFields_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        public static bool OptionButtonClick_OkConnectToAp_Prefix(CharacterCustomization __instance, string name)
        {
            try
            {
                if (!(__instance is CharacterCustomizationArchipelago apInstance) || name != "OK" || !__instance.canLeaveMenu())
                {
                    return true; // run original logic
                }

                if (!apInstance.TryParseIpAddress(out var ip, out var port))
                {
                    return false;
                }

                var connected = _modEntry.ArchipelagoConnect(ip, port, apInstance.SlotNameTextBox.Text, apInstance.PasswordTextBox.Text, out var errorMessage);

                if (!connected)
                {
                    var currentMenu = TitleMenu.subMenu;
                    TitleMenu.subMenu = new InformationDialog(errorMessage, (_) => OnClickOkBehavior(currentMenu));
                }

                return connected; // run original logic only if connected successfully
            }
            catch (Exception ex)
            {
                _modEntry.Monitor.Log($"Failed in {nameof(OptionButtonClick_OkConnectToAp_Prefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic
            }
        }

        private static void OnClickOkBehavior(IClickableMenu previousMenu)
        {
            TitleMenu.subMenu = previousMenu;
        }
    }
}
