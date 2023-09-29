/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jangofett4/FishingTreasureSpawner
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FishingTreasureSpawner
{
    /// <summary>
    /// This mod spawns a fishing treasure with the press of a hotkey.
    /// This mod also doesn't give a damn about where you are in the map, be it inside or outside.
    /// Mod only triggers the event for getting a fishing treasure using players fishing rod, if they don't have one also gives a fishing rod.
    /// </summary>
    internal sealed class ModEntry : Mod
    {
        private int lastClearWaterDistance = 0;
        private FieldInfo clearWaterDistanceField;

        private ModConfig Config { get; set; } = new ModConfig();

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                ModManifest, 
                () => Config = new ModConfig(), 
                () => Helper.WriteConfig(Config)
            );

            configMenu.AddSectionTitle(ModManifest, () => "Treasure Spawner Keybinds");

            configMenu.AddKeybind(
                ModManifest,
                () => Config.FZ0Keymap,
                value => Config.FZ0Keymap = value,
                () => "Fishing Zone 0",
                () => "Hotkey to trigger Fishing Zone 0 treasure chests"
            );
            configMenu.AddKeybind(
                ModManifest,
                () => Config.FZ1Keymap,
                value => Config.FZ1Keymap = value,
                () => "Fishing Zone 1",
                () => "Hotkey to trigger Fishing Zone 1 treasure chests"
            );
            configMenu.AddKeybind(
                ModManifest,
                () => Config.FZ2Keymap,
                value => Config.FZ2Keymap = value,
                () => "Fishing Zone 2",
                () => "Hotkey to trigger Fishing Zone 2 treasure chests"
            );
            configMenu.AddKeybind(
                ModManifest,
                () => Config.FZ3Keymap,
                value => Config.FZ3Keymap = value,
                () => "Fishing Zone 3",
                () => "Hotkey to trigger Fishing Zone 3 treasure chests"
            );
            configMenu.AddKeybind(
                ModManifest,
                () => Config.FZ4Keymap,
                value => Config.FZ4Keymap = value,
                () => "Fishing Zone 4",
                () => "Hotkey to trigger Fishing Zone 4 treasure chests"
            );
            configMenu.AddKeybind(
                ModManifest,
                () => Config.FZ5Keymap,
                value => Config.FZ5Keymap = value,
                () => "Fishing Zone 5",
                () => "Hotkey to trigger Fishing Zone 5 treasure chests"
            );
            configMenu.AddKeybind(
                ModManifest,
                () => Config.FZ6Keymap,
                value => Config.FZ6Keymap = value,
                () => "Fishing Zone 6",
                () => "Hotkey to trigger Fishing Zone 6 treasure chests"
            );
            configMenu.AddKeybind(
                ModManifest,
                () => Config.FZ7Keymap,
                value => Config.FZ7Keymap = value,
                () => "Fishing Zone 7",
                () => "Hotkey to trigger Fishing Zone 7 treasure chests"
            );
            configMenu.AddKeybind(
                ModManifest,
                () => Config.FZ8Keymap,
                value => Config.FZ8Keymap = value,
                () => "Fishing Zone 8",
                () => "Hotkey to trigger Fishing Zone 8 treasure chests"
            );
            configMenu.AddKeybind(
                ModManifest,
                () => Config.FZ9Keymap,
                value => Config.FZ9Keymap = value,
                () => "Fishing Zone 9",
                () => "Hotkey to trigger Fishing Zone 9 treasure chests"
            );

            configMenu.AddSectionTitle(ModManifest, () => "[BETA] Brute Forcing Settings");

            configMenu.AddBoolOption(
                ModManifest,
                () => Config.BruteForceEnabled,
                value => Config.BruteForceEnabled = value,
                () => "Enable Brute Force",
                () => "Enable or disable brute forcing feature. Warning: This feature is beta and improper item names might cause temporary lockups."
            );

            configMenu.AddKeybind(
                ModManifest,
                () => Config.BruteForceKey,
                value => Config.BruteForceKey = value,
                () => "Brute Force Key",
                () => "Hotkey to trigger treasure brute forcing"
            );

            configMenu.AddNumberOption(
                ModManifest,
                () => Config.BruteForceFishingZone,
                value => { Config.BruteForceFishingZone = value; Config.BruteForceAllow = true; },
                () => "Brute Force Fishing Zone",
                () => "Fishing zone to brute force in",
                min: 0,
                max: 20
            );

            configMenu.AddNumberOption(
                ModManifest,
                () => Config.BruteForceTries,
                value => Config.BruteForceTries = value,
                () => "Brute Force Tries",
                () => "How many times will the brute force run, useful for collecting data. Warning: Big numbers might cause lockups and crash the game.",
                min: 0,
                max: 10000,
                interval: 10
            );

            configMenu.AddTextOption(
                ModManifest,
                () => Config.BruteForceItem,
                value => { Config.BruteForceItem = value.ToLower(); Config.BruteForceAllow = true; },
                () => "Brute Force Item",
                () => "Item name to search for, lowercase."
            );

            configMenu.AddTextOption(
                ModManifest,
                () => Config.BruteForceReportFile,
                value => Config.BruteForceReportFile = value.ToLower(),
                () => "Brute Force Report File",
                () => "File to write after brute forcing is fully complete. This file should be saved next to executable."
            );
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
                return;

            /*
             * Hotkeys are configurable but defaulted to be Numpad 0 to 9
             * 0 being fishing zone 0, and 9 being fishing zone 9
             * According to wiki only fishing zones 0 to 5 are used for treasures, I added other zones just for fun
             */

            if (e.Button == Config.FZ0Keymap)
            {
                CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod);
                SetClearWaterDistance(playerFishingRod, 0);
                playerFishingRod.openTreasureMenuEndFunction(0);
            }
            else if (e.Button == Config.FZ1Keymap)
            {
                CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod);
                SetClearWaterDistance(playerFishingRod, 1);
                playerFishingRod.openTreasureMenuEndFunction(0);
            }
            else if (e.Button == Config.FZ2Keymap)
            {
                CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod);
                SetClearWaterDistance(playerFishingRod, 2);
                playerFishingRod.openTreasureMenuEndFunction(0);
            }
            else if (e.Button == Config.FZ3Keymap)
            {
                CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod);
                SetClearWaterDistance(playerFishingRod, 3);
                playerFishingRod.openTreasureMenuEndFunction(0);
            }
            else if (e.Button == Config.FZ4Keymap)
            {
                CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod);
                SetClearWaterDistance(playerFishingRod, 4);
                playerFishingRod.openTreasureMenuEndFunction(0);
            }
            else if (e.Button == Config.FZ5Keymap)
            {
                CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod);
                SetClearWaterDistance(playerFishingRod, 5);
                playerFishingRod.openTreasureMenuEndFunction(0);
            }
            else if (e.Button == Config.FZ6Keymap)
            {
                CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod);
                SetClearWaterDistance(playerFishingRod, 6);
                playerFishingRod.openTreasureMenuEndFunction(0);
            }
            else if (e.Button == Config.FZ7Keymap)
            {
                CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod);
                SetClearWaterDistance(playerFishingRod, 7);
                playerFishingRod.openTreasureMenuEndFunction(0);
            }
            else if (e.Button == Config.FZ8Keymap)
            {
                CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod);
                SetClearWaterDistance(playerFishingRod, 8);
                playerFishingRod.openTreasureMenuEndFunction(0);
            }
            else if (e.Button == Config.FZ9Keymap)
            {
                CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod);
                SetClearWaterDistance(playerFishingRod, 9);
                playerFishingRod.openTreasureMenuEndFunction(0);
            }
            else if (Config.BruteForceEnabled && e.Button == Config.BruteForceKey)
            {
                if (!Config.BruteForceAllow) // Something was wrong with previous configuration?
                {
                    Game1.chatBox.addInfoMessage($"Cannot brute force with current configuration, change the configuration and try again.");
                    Monitor.Log($"Cannot brute force with current configuration, change the configuration and try again.", LogLevel.Warn);
                }
                else
                {
                    Game1.chatBox.addInfoMessage($"Brute forcing until treasure chest contains '*{Config.BruteForceItem}*' ({Config.BruteForceTries} loops)");
                    Monitor.Log($"Brute forcing until treasure chest contains '*{Config.BruteForceItem}*' ({Config.BruteForceTries} loops)", LogLevel.Debug);
                    int tries = 0;
                    long totalTries = 0;
                    int min = int.MaxValue;
                    int max = int.MinValue;
                    string foundItemName = string.Empty;
                    double lastAvg = 0;

                    int lockupPreventionCount = 0;      // How many times did the system prevent lockups
                    bool shouldPreventLockup = true;    // Should the system try and prevent lockups
                    bool lockupKill = false;            // Should the loop stop because there is an improper config

                    var report = new StringBuilder();
                    report.AppendLine("Tries, Average, Item");

                    if (!CheckAndGetPlayerFishingRod(out FishingRod playerFishingRod))
                    {
                        Game1.chatBox.addInfoMessage("Player didn't have a fishing rod, gave a Iridium Rod. Please trigger the brute forcing operation again using the hotkey.");
                        return;
                    }

                    SetClearWaterDistance(playerFishingRod, Config.BruteForceFishingZone);

                    for (int t = 0; t < Config.BruteForceTries && !lockupKill; t++)
                    {
                        for (; ; )
                        {
                            playerFishingRod.openTreasureMenuEndFunction(0);

                            var menu = (ItemGrabMenu)Game1.activeClickableMenu; // TODO: Very bad to cast it EVERY LOOP, VERY inefficient
                            var foundItem = menu.ItemsToGrabMenu.actualInventory.FirstOrDefault(x => x.DisplayName.ToLower().Contains(Config.BruteForceItem));
                            tries++;
                            if (foundItem != null)
                            {
                                foundItemName = foundItem.DisplayName;
                                lockupPreventionCount = 0; // We found the item, no need to prevent lockups now as we know it exists with current configuration
                                shouldPreventLockup = false;
                                break;
                            }

                            // We been searching this item for 100k chests? If so there might be something wrong
                            if (shouldPreventLockup && tries >= 100000)
                            {
                                lockupPreventionCount++;
                                Monitor.Log($"Brute force prevented possible lockup ({lockupPreventionCount}. offense)", LogLevel.Warn);
                                // If this is the third possible lockup, kill the loop so we dont crash the game
                                if (lockupPreventionCount >= 3)
                                {
                                    Game1.chatBox.addErrorMessage($"Brute forcer prevented a possible lockup, check the configuration and be sure item exists and is in specified brute force fishing zone");
                                    Monitor.Log($"Brute forcer prevented a possible lockup, check the configuration and be sure item exists and is in specified brute force fishing zone", LogLevel.Alert);
                                    lockupKill = true;              // Stop the loop
                                    Config.BruteForceAllow = false; // Prevent this configuration from being brute forced until its changed
                                    Helper.WriteConfig(Config);
                                    break;
                                }

                                break;
                            }
                        }

                        if (!lockupKill)
                        {
                            totalTries += tries;
                            lastAvg = totalTries / (double)t;
                            report.AppendLine($"{tries}, {lastAvg}, { foundItemName }");
                            if (tries < min)
                                min = tries;
                            if (tries > max)
                                max = tries;
                            tries = 0;
                            // Monitor.Log($"Brute force: found { Config.BruteForceItem } in { tries } tries", LogLevel.Debug);
                        }
                    }

                    if (!Config.BruteForceAllow)
                        File.WriteAllText(Config.BruteForceReportFile, "Brute forcing aborted abruptly due to lockup prevention, check the configuration and be sure item name and fishing zone is correct.");
                    else
                    {
                        Game1.chatBox.addInfoMessage($"Brute forcing finished [Min: {min}, Max: {max}, Avg: {lastAvg}], report written to {Config.BruteForceReportFile}");
                        Monitor.Log($"Brute forcing finished [Min: {min}, Max: {max}, Avg: {lastAvg}], report written to {Config.BruteForceReportFile}", LogLevel.Info);
                        File.WriteAllText(Config.BruteForceReportFile, report.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Player needs to have a fishing rod in order for them to get a fishing treasure chest.
        /// This functions checks if player has a fishing rod, if they don't gives them one and returns the fishing rod object reference.
        /// </summary>
        /// <returns></returns>
        public bool CheckAndGetPlayerFishingRod(out FishingRod rod)
        {
            var playerFishingRod = Game1.player.Items.FirstOrDefault(x => x is FishingRod);
            if (playerFishingRod == null)
            {
                Monitor.Log("Player doesn't have a fishing rod, giving one.", LogLevel.Debug);
                playerFishingRod = new FishingRod(3);
                Game1.player.addItemToInventory(playerFishingRod);
                rod = ((FishingRod)playerFishingRod);
                return false;
            }

            rod = ((FishingRod)playerFishingRod);
            return true;
        }

        /// <summary>
        /// Some treasures are only available if bobber is 'x' units away from the nearest land (land includes bridges, dirt, anything player can stand on)
        /// For example iridium ores are only available in fishing zone 5, that is bobber was 5 tiles away from land when treasure was caught
        /// This function changes fishing rod fishing zone to a specific value without having to cast the line.
        /// Because this value is 'private' in game code, we use reflection to set it forcibly.
        /// </summary>
        /// <param name="rod">Fishing rod to set the value </param>
        /// <param name="distance">Fishing zone to set</param>
        public void SetClearWaterDistance(FishingRod rod, int distance)
        {
            if (clearWaterDistanceField == null) // Reflection is pricey in terms of performance. We want to minimize 'GetField' calls, so we get it and put it somewhere for later use.
                clearWaterDistanceField = typeof(FishingRod).GetField("clearWaterDistance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (lastClearWaterDistance == distance) // If current clear water distance is same as previous one, don't set it to avoid unnecessary reflection
                return;
            clearWaterDistanceField.SetValue(rod, distance);
            lastClearWaterDistance = distance;
        }
    }
}
