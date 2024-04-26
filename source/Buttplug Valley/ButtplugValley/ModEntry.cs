/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DryIcedTea/Buttplug-Valley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace ButtplugValley
{
    internal sealed class ModEntry : Mod
    {
        private BPManager buttplugManager;
        private ModConfig Config;
        private FishingMinigame fishingMinigame;
        private bool isVibrating = false;
        private int previousHealth;
        private int previousCoins;
        private int _levelUps;
        private bool wasRidingHorse = false;

        private const int CoffeeBeansID = 433;
        private const int WoolID = 440;
        
        //Arcade Machines
        private int previousMinekartHealth;
        private int previousAbigailHealth;
        private int previousPowerupCount;
        private bool isGameOverAbigail = false;

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            buttplugManager = new BPManager();
            fishingMinigame = new FishingMinigame(helper, Monitor, buttplugManager);
            new FishingRod(helper, Monitor, buttplugManager, Config);
            Task.Run(async () =>
            {
                await buttplugManager.ConnectButtplug(Monitor, Config.IntifaceIP);
                await buttplugManager.ScanForDevices();

            });

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.World.TerrainFeatureListChanged += OnTerrainFeatureListChanged;
            helper.Events.World.ObjectListChanged += OnObjectListChanged;
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.World.NpcListChanged += OnNpcListChanged;
            helper.Events.Player.LevelChanged += OnLevelChanged;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            

            // var harmony = new Harmony(this.ModManifest.UniqueID);
            //
            //
            // harmony.Patch(
            //     original: AccessTools.Method(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.PerformKiss)),
            //     postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Kissing_Postfix))
            // );
        }
        
        private static void Kissing_Postfix()
        {
            //This code is suposed to be ran every time the player kisses another player, but the vibrations do not work.
            //Not going to mess around with this anymore, but if anyone wants to make it work then that would be amazing
            //Kissing_Postfix needs to be static which sucks
            try
            {
                BPManager buttplugManager = new BPManager();
                buttplugManager.VibrateDevicePulse(100,4000);
            }
            catch (Exception ex)
            {
                //Monitor.Log($"Failed in {nameof(Kissing_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox && Config.VibrateOnDialogue)
            {
                Monitor.Log("Dialogue Box Triggered", LogLevel.Trace);
                buttplugManager.VibrateDevicePulse(Config.DialogueLevel, 550);
            }
        }

        private void OnLevelChanged(object sender, LevelChangedEventArgs e)
        {
            if (e.IsLocalPlayer)
            {
                if (e.NewLevel > e.OldLevel)
                {
                    _levelUps++;
                }
            }
        }

        private void OnNpcListChanged(object sender, NpcListChangedEventArgs e)
        {
            // Get the defeated monsters from the removed NPCs
            var defeatedMonsters = e.Removed.Where(npc => npc.IsMonster).ToList();
            var defeatedEnemyCount = 0;
            
            if (defeatedMonsters.Any() && Config.VibrateOnEnemyKilled)
            {
                // Increment the defeated enemy count
                defeatedEnemyCount += defeatedMonsters.Count;
                Monitor.Log($"Defeated {defeatedEnemyCount} enemies", LogLevel.Trace);
                

                // Vibrate the device
                buttplugManager.VibrateDevicePulse(Config.EnemyKilledLevel, 400*defeatedEnemyCount);
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );
            
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "ButtplugValley.VibrationEvents",
                text: () => "Vibration Events"
            );
            
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "ButtplugValley.VibrationLevels",
                text: () => "Vibration Levels"
            );
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "ButtplugValley.Keybinds",
                text: () => "Keybinds"
            );
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "ButtplugValley.EditIP",
                text: () => "Edit IP"
            ); 
            
            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "ButtplugValley.VibrationEvents",
                pageTitle: () => "Vibration Events"
            );
            
            configMenu.AddSectionTitle(mod:this.ModManifest, text: () => "Vibration Events");

            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Crop and Milk Pickup",
                tooltip: () => "Should the device vibrate on collecting crops and milk?",
                getValue: () => this.Config.VibrateOnCropAndMilkCollected,
                setValue: value => this.Config.VibrateOnCropAndMilkCollected = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Fish Pickup",
                tooltip: () => "Should the device vibrate on collecting fish?",
                getValue: () => this.Config.VibrateOnFishCollected,
                setValue: value => this.Config.VibrateOnFishCollected = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Flower Pickup",
                tooltip: () => "Should the device vibrate on collecting flowers?",
                getValue: () => this.Config.VibrateOnFlowersCollected,
                setValue: value => this.Config.VibrateOnFlowersCollected = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Foraging Pickup",
                tooltip: () => "Should the device vibrate on collecting foraging?",
                getValue: () => this.Config.VibrateOnForagingCollected,
                setValue: value => this.Config.VibrateOnForagingCollected = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Stone Broken",
                tooltip: () => "Should the device vibrate on breaking stone and ores?",
                getValue: () => this.Config.VibrateOnStoneBroken,
                setValue: value => this.Config.VibrateOnStoneBroken = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Tree Broken",
                tooltip: () => "Should the device vibrate on fully chopping down a tree?",
                getValue: () => this.Config.VibrateOnTreeBroken,
                setValue: value => this.Config.VibrateOnTreeBroken = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Damage Taken",
                tooltip: () => "Should the device vibrate on taking damage? Scales with health",
                getValue: () => this.Config.VibrateOnDamageTaken,
                setValue: value => this.Config.VibrateOnDamageTaken = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Enemy Killed",
                tooltip: () => "Should the device vibrate on killing an enemy? Scales with enemies killed at once.",
                getValue: () => this.Config.VibrateOnEnemyKilled,
                setValue: value => this.Config.VibrateOnEnemyKilled = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Day Start",
                tooltip: () => "Should the device vibrate when the day starts?",
                getValue: () => this.Config.VibrateOnDayStart,
                setValue: value => this.Config.VibrateOnDayStart = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Day Ending",
                tooltip: () => "Should the device vibrate when the day ends?",
                getValue: () => this.Config.VibrateOnDayEnd,
                setValue: value => this.Config.VibrateOnDayEnd = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Fishing Minigame",
                tooltip: () => "Should the device vibrate in the fishing minigame? Scales with the capture bar",
                getValue: () => this.Config.VibrateOnFishingMinigame,
                setValue: value => this.Config.VibrateOnFishingMinigame = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Fishing Rod",
                tooltip: () => "Should the device vibrate when using fishing rod",
                getValue: () => this.Config.VibrateOnFishingRodUsage,
                setValue: value => this.Config.VibrateOnFishingRodUsage = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Arcade Minigames",
                tooltip: () => "Should the device vibrate on certain events in the arcade minigames?",
                getValue: () => this.Config.VibrateOnArcade,
                setValue: value => this.Config.VibrateOnArcade = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Dialogue Boxes",
                tooltip: () => "Should the device vibrate on opening a dialogue box?",
                getValue: () => this.Config.VibrateOnDialogue,
                setValue: value => this.Config.VibrateOnDialogue = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Horse Riding",
                tooltip: () => "Should the device vibrate while riding a horse?",
                getValue: () => this.Config.VibrateOnHorse,
                setValue: value => this.Config.VibrateOnHorse = value
            );
            
            // configMenu.AddBoolOption(
            //     mod: this.ModManifest,
            //     name: () => "STONE PICK UP (Test version only)",
            //     tooltip: () => "Should the device vibrate on picking up stone and ore?",
            //     getValue: () => this.Config.StonePickedUpDebug,
            //     setValue: value => this.Config.StonePickedUpDebug = value
            // );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => "Keep Alive Pulse",
                tooltip: () => "Vibrate every 30s to keep connection alive?",
                getValue: () => this.Config.KeepAlive,
                setValue: value => this.Config.KeepAlive = value
            );
            /*
             * 
             * VIBRATION LEVELS
             * 
             */
            
            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "ButtplugValley.VibrationLevels",
                pageTitle: () => "Vibration Levels"
            );
            
            configMenu.AddSectionTitle(mod:this.ModManifest, text: () => "Vibration Levels (0-100)");
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Basic Crops and Milk",
                tooltip: () => "How Strong should the vibration be for normal milk and crops?",
                getValue: () => this.Config.CropAndMilkBasic,
                setValue: value => this.Config.CropAndMilkBasic = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Basic Fish Pickup",
                tooltip: () => "How Strong should the vibration be for picking up normal fish?",
                getValue: () => this.Config.FishCollectedBasic,
                setValue: value => this.Config.FishCollectedBasic = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Basic Flower Pickup",
                tooltip: () => "How Strong should the vibration be for picking up normal flowers?",
                getValue: () => this.Config.FlowerBasic,
                setValue: value => this.Config.FlowerBasic = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Basic Foraging Pickup",
                tooltip: () => "How Strong should the vibration be for picking up normal foraging?",
                getValue: () => this.Config.ForagingBasic,
                setValue: value => this.Config.ForagingBasic = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Silver Fish, Crops and Milk",
                tooltip: () => "How Strong should the vibration be for ALL silver fish, crops and milk?",
                getValue: () => this.Config.SilverLevel,
                setValue: value => this.Config.SilverLevel = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Gold Fish, Crops and Milk",
                tooltip: () => "How Strong should the vibration be for ALL Gold fish, crops and milk?",
                getValue: () => this.Config.GoldLevel,
                setValue: value => this.Config.GoldLevel = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Iridium Fish, Crops and Milk",
                tooltip: () => "How Strong should the vibration be for ALL Iridium fish, crops and milk?",
                getValue: () => this.Config.IridiumLevel,
                setValue: value => this.Config.IridiumLevel = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Stone Broken",
                tooltip: () => "How Strong should the vibration be?",
                getValue: () => this.Config.StoneBrokenLevel,
                setValue: value => this.Config.StoneBrokenLevel = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Tree Broken",
                tooltip: () => "How Strong should the vibration be for breaking a tree?",
                getValue: () => this.Config.TreeBrokenLevel,
                setValue: value => this.Config.TreeBrokenLevel = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Damage Taken Max",
                tooltip: () => "How Strong should the MAX vibration be when taking damage?",
                getValue: () => this.Config.DamageTakenMax,
                setValue: value => this.Config.DamageTakenMax = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Enemy Killed",
                tooltip: () => "How Strong should the vibration be when killing an enemy?",
                getValue: () => this.Config.EnemyKilledLevel,
                setValue: value => this.Config.EnemyKilledLevel = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Day Start",
                tooltip: () => "How Strong should the vibration be when the day starts?",
                getValue: () => this.Config.DayStartLevel,
                setValue: value => this.Config.DayStartLevel = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Day End",
                tooltip: () => "How Strong should the MAX vibration be when the day ends? Min 50",
                getValue: () => this.Config.DayEndMax,
                setValue: value => this.Config.DayEndMax = value,
                min: 50,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Fishing Minigame",
                tooltip: () => "How Strong should the MAX vibration be in the fishing minigame?",
                getValue: () => this.Config.MaxFishingVibration,
                setValue: value => this.Config.MaxFishingVibration = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Arcade Minigames",
                tooltip: () => "How Strong should the vibration be in the arcade minigames?",
                getValue: () => this.Config.ArcadeLevel,
                setValue: value => this.Config.ArcadeLevel = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Dialogue Box",
                tooltip: () => "How Strong should the vibration be when opening a dialogue box?",
                getValue: () => this.Config.DialogueLevel,
                setValue: value => this.Config.DialogueLevel = value,
                min: 0,
                max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Horse Riding",
                tooltip: () => "How Strong should the vibration be when riding a horse?",
                getValue: () => this.Config.HorseLevel,
                setValue: value => this.Config.HorseLevel = value,
                min: 0,
                max: 100
            );
            
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Keep Alive Interval",
                tooltip: () => "How frequently should the Keep alive signal be sent (in seconds)",
                getValue: () => this.Config.KeepAliveInterval,
                setValue: value => this.Config.KeepAliveInterval = value,
                min: 5,
                max: 300,
                interval: 5
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => "Keep Alive Intensity",
                tooltip: () => "How strong should the keep alive vibration be?",
                getValue: () => this.Config.KeepAliveLevel,
                setValue: value => this.Config.KeepAliveLevel = value,
                min: 0,
                max: 100
            );
            /*
             * 
             * Keybinds
             * 
             */
            
            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "ButtplugValley.Keybinds",
                pageTitle: () => "Keybinds"
            );
            
            configMenu.AddSectionTitle(mod:this.ModManifest, text: () => "Keybinds");
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Stop Vibrations",
                tooltip: () => "Stops all ongoing vibrations",
                getValue: () => this.Config.StopVibrations,
                setValue: value => this.Config.StopVibrations = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Disconnect",
                tooltip: () => "Disconnects the game from intiface",
                getValue: () => this.Config.DisconnectButtplug,
                setValue: value => this.Config.DisconnectButtplug = value
            );
            configMenu.AddKeybind(
                mod: this.ModManifest,
                name: () => "Reconnect",
                tooltip: () => "Reconnects the game to intiface",
                getValue: () => this.Config.ReconnectButtplug,
                setValue: value => this.Config.ReconnectButtplug = value
            );
            
            /*
             * 
             * Intiface Connection
             * 
             */
            
            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "ButtplugValley.EditIP",
                pageTitle: () => "Edit IP"
            );
            configMenu.AddSectionTitle(mod:this.ModManifest, text: () => "Edit IP");
            configMenu.AddParagraph(mod:this.ModManifest, text: () => "Press the Reconnect keybind after saving to reconnect. Ignore this if you don't know what this is.");
            configMenu.AddTextOption(
                mod: this.ModManifest,
                name: () => "Intiface IP",
                tooltip: () => "The address used to connect to intiface. Leave default if you don't know what this is",
                getValue: () => this.Config.IntifaceIP,
                setValue: value => this.Config.IntifaceIP = value
            );
        }

        private void OnObjectListChanged(object sender, ObjectListChangedEventArgs e)
        {
            // Get the destroyed stones
            var destroyedStones = e.Removed.Where(pair =>
            {
                if (pair.Value is StardewValley.Object obj)
                {
                    return obj.Name == "Stone" || obj.Name.Contains("Ore");
                }
                return false;
            }).ToList();
            var destroyedStoneCount = 0;
            
            if (destroyedStones.Any())
            {
                // Increment the destroyed stone count
                destroyedStoneCount += destroyedStones.Count;
                //print the count
                this.Monitor.Log($"DESTROYED {destroyedStoneCount} STONES", LogLevel.Trace);
                double durationmath = 3920 / (1 + (10 * Math.Exp(-0.16 * destroyedStoneCount)));
                int duration = Convert.ToInt32(durationmath);

                // Vibrate the device
                this.Monitor.Log($"VIBRATING FOR {duration} milliseconds", LogLevel.Trace);
                buttplugManager.VibrateDevicePulse(Config.StoneBrokenLevel, duration);
            }
            GameLocation location = Game1.currentLocation;
            
            bool hasBrokenBranches = e.Removed.Any(pair =>
            {
                
                if (pair.Value is StardewValley.Object obj)
                {
                    return obj.Name == "Twig";
                }
                return false;
            });

            if (hasBrokenBranches)
            {
                // Vibrate the device when a broken branch is found
                this.Monitor.Log("Branch Broken", LogLevel.Trace);
                buttplugManager.VibrateDevicePulse(Config.TreeBrokenLevel);
            }
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            Task.Run(async () =>
            {
                if (!Config.VibrateOnDayEnd) return;
                
                var level = Config.DayEndMax;
                this.Monitor.Log($"{Game1.player.Name} VIBRATING AT {50} then 100.", LogLevel.Trace);
                await buttplugManager.VibrateDevice(level-50);
                await Task.Delay(800 + (500*_levelUps));
                await buttplugManager.VibrateDevice(level-20);
                await Task.Delay(400 + (250*_levelUps));
                await buttplugManager.VibrateDevice(level);
                await Task.Delay(200 + (125*_levelUps));
                await buttplugManager.VibrateDevice(0);
            });
            
        }

        private void OnInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // Check if any items were added from the inventory
            foreach (Item item in e.Added)
            {
                if (item is StardewValley.Object obj)
                {
                    this.Monitor.Log($"Added Item: {obj.Name}, Category: {obj.getCategoryName()}, Category Id: {obj.Category}", LogLevel.Trace);
                    if (obj.Category == StardewValley.Object.FishCategory)
                    {
                        if (!Config.VibrateOnFishCollected) return;
                        this.Monitor.Log("Fish", LogLevel.Trace);
                        VibrateBasedOnQuality(obj, Config.FishCollectedBasic);
                        break; // Exit the loop after the first harvested crop is found
                    }
                    if (obj.Category == StardewValley.Object.VegetableCategory ||
                        obj.Category == StardewValley.Object.FruitsCategory || 
                        obj.Category == StardewValley.Object.MilkCategory || 
                        obj.Category == StardewValley.Object.EggCategory || 
                        obj.ParentSheetIndex is CoffeeBeansID or WoolID)
                    {
                        
                        if (!Config.VibrateOnCropAndMilkCollected) return;
                        this.Monitor.Log("Crop or Milk Added", LogLevel.Trace);
                        VibrateBasedOnQuality(obj, Config.CropAndMilkBasic);
                        break; // Exit the loop after the first harvested crop is found
                    }
                    if (obj.Category == StardewValley.Object.flowersCategory)
                    {
                        if (!Config.VibrateOnFlowersCollected) return;
                        this.Monitor.Log("Flower Added", LogLevel.Trace);
                        VibrateBasedOnQuality(obj, Config.FlowerBasic);
                        break; // Exit the loop after the first harvested crop is found
                    }
                    if (obj.Category == StardewValley.Object.GreensCategory ||
                        obj.Category == StardewValley.Object.sellAtFishShopCategory ||
                        obj.parentSheetIndex == 771) //771 is fiber i think
                    {
                        if (!Config.VibrateOnForagingCollected) return;
                        this.Monitor.Log("Foraging Added", LogLevel.Trace);
                        VibrateBasedOnQuality(obj, Config.ForagingBasic);
                        break; // Exit the loop after the first harvested crop is found
                    }
                    if (obj.Category == StardewValley.Object.metalResources || obj.parentSheetIndex == 390) //390 is stone
                    {
                        if (Config.StonePickedUpDebug)
                        {
                            //THIS IS TEMPORARY CODE FOR TESTING PURPOSES ==============================================================================================================
                            double durationmath = 3920 / (1 + (10 * Math.Exp(-0.16 * 1)));
                            buttplugManager.VibrateDevicePulse(Config.StoneBrokenLevel, Convert.ToInt32(durationmath));
                            break;
                        }
                    }
                }
            }
            foreach (ItemStackSizeChange change in e.QuantityChanged)
            {
                // Check if the changed item is a fish
                if (change.Item is StardewValley.Object obj)
                {
                    //this.Monitor.Log($"Changed Item: {obj.Name}, Category: {obj.getCategoryName()}, Category Id: {obj.Category}", LogLevel.Debug);
                    if (obj.Category == StardewValley.Object.FishCategory)
                    {
                        if (!Config.VibrateOnFishCollected) return;
                        VibrateBasedOnQuality(obj, Config.FishCollectedBasic);
                        break; // Exit the loop after the first harvested crop is found
                    }

                    if (obj.Category == StardewValley.Object.metalResources || obj.parentSheetIndex == 390) //390 is stone
                    {
                        if (Config.StonePickedUpDebug)
                        {
                            //THIS IS TEMPORARY CODE FOR TESTING PURPOSES ===================================================================================================================================
                            double durationmath = 3920 / (1 + (10 * Math.Exp(-0.16 * 1)));
                            buttplugManager.VibrateDevicePulse(Config.StoneBrokenLevel, Convert.ToInt32(durationmath));
                            break;
                        }
                    }
                    
                    if (obj.Category == StardewValley.Object.VegetableCategory ||
                        obj.Category == StardewValley.Object.FruitsCategory ||
                        obj.Category == StardewValley.Object.MilkCategory ||
                        obj.Category == StardewValley.Object.EggCategory ||
                        obj.ParentSheetIndex is CoffeeBeansID or WoolID)
                    {
                        if (!Config.VibrateOnCropAndMilkCollected) return;
                        this.Monitor.Log("Crop or Milk Changed", LogLevel.Trace);
                        VibrateBasedOnQuality(obj, Config.CropAndMilkBasic);
                        break; // Exit the loop after the first harvested crop is found
                    }
                    if (obj.Category == StardewValley.Object.flowersCategory)
                    {
                        if (!Config.VibrateOnFlowersCollected) return;
                        this.Monitor.Log("Flower Changed", LogLevel.Trace);
                        VibrateBasedOnQuality(obj, Config.FlowerBasic);
                        break; // Exit the loop after the first harvested crop is found
                    }
                    if (obj.Category == StardewValley.Object.GreensCategory ||
                        obj.Category == StardewValley.Object.sellAtFishShopCategory)
                    {
                        if (!Config.VibrateOnForagingCollected) return;
                        this.Monitor.Log("Foraging Changed", LogLevel.Trace);
                        VibrateBasedOnQuality(obj, Config.ForagingBasic);
                        break; // Exit the loop after the first harvested crop is found
                    }
                }
            }
        }

        private void VibrateBasedOnQuality(StardewValley.Object obj, int basicLevel)
        {
            switch (obj.Quality)
            {
                case StardewValley.Object.medQuality:
                    _ = buttplugManager.VibrateDevicePulse(Config.SilverLevel);
                    break;
                case StardewValley.Object.highQuality:
                    _ = buttplugManager.VibrateDevicePulse(Config.GoldLevel, 650);
                    break;
                case StardewValley.Object.bestQuality:
                    _ = buttplugManager.VibrateDevicePulse(Config.IridiumLevel, 1200);
                    break;
                default:
                    _ = buttplugManager.VibrateDevicePulse(basicLevel); // Adjust the power level as desired
                    break;
            }
        }

        private void OnTerrainFeatureListChanged(object sender, TerrainFeatureListChangedEventArgs e)
        {
            if (e.IsCurrentLocation)
            {
                foreach (var feature in e.Removed)
                {
                    if (feature.Value is Tree tree && Config.VibrateOnTreeBroken)
                    {
                        // Tree is fully chopped
                            Task.Run(async () =>
                            {
                                this.Monitor.Log($"{Game1.player.Name} VIBRATING AT {80}.", LogLevel.Trace);
                                await buttplugManager.VibrateDevice(Config.TreeBrokenLevel);
                                await Task.Delay(420);
                                await buttplugManager.VibrateDevice(0);
                            });
                    }
                    if (feature.Value is ResourceClump resourceClump && Config.VibrateOnStoneBroken)
                    {
                        // Large rock or stub i think
                        Task.Run(async () =>
                        {
                            this.Monitor.Log($"{Game1.player.Name} VIBRATING AT {80}. for 1.2 seconds", LogLevel.Trace);
                            await buttplugManager.VibrateDevicePulse(80, 1200);
                        });
                    }
                }
            }
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {

            // print button presses to the console window
            //this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);

            // Check if the button pressed is the desired button
            /*if (e.Button == SButton.A)
            {
                // Trigger the device vibration
                Task.Run(async () =>
                {
                    await buttplugManager.VibrateDevice(50);
                    await Task.Delay(200);
                    await buttplugManager.VibrateDevice(0);
                });
            }
            if (e.Button == SButton.Space)
            {
                // Toggle the vibration state
                isVibrating = !isVibrating;
                buttplugManager.VibrateDevice(isVibrating ? 100 : 0);
            }*/
            if (e.Button == Config.StopVibrations)
            {
                // Stop Vibrations
                Task.Run(async () => await buttplugManager.StopDevices());
            }
            if (e.Button == Config.DisconnectButtplug)
            {
                Task.Run(async () =>
                {
                    //await buttplugManager.StopDevices();
                    await buttplugManager.DisconnectButtplug();
                });

            }
            if (e.Button == Config.ReconnectButtplug)
            {
                // Reconnect
                Task.Run(async () =>
                {
                    await buttplugManager.DisconnectButtplug();
                    await buttplugManager.ConnectButtplug(Monitor, Config.IntifaceIP);
                    await buttplugManager.ScanForDevices();
                });
            }
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            _levelUps = 0;
            if (!Config.VibrateOnDayStart) return;
            //fishingMinigame.previousCaptureLevel = 0f;
            Task.Run(async () =>
            {
                await buttplugManager.VibrateDevice(Config.DayStartLevel);
                await Task.Delay(150);
                await buttplugManager.VibrateDevice(0);

                await Task.Delay(350);

                await buttplugManager.VibrateDevice(Config.DayStartLevel);
                await Task.Delay(150);
                await buttplugManager.VibrateDevice(0);
            });
        }
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            ArcadeMinigames(sender, e);
            fishingMinigame.isActive = Config.VibrateOnFishingMinigame;
            fishingMinigame.maxVibration = Config.MaxFishingVibration;
            // Check if the player's health has decreased since the last tick
            if (Game1.player.health < previousHealth)
            {
                if (!Config.VibrateOnDamageTaken) return;
                float intensity = (Config.DamageTakenMax * (1f - (float)Game1.player.health / (float)Game1.player.maxHealth));
                intensity = Math.Min(intensity, 100f);
                Task.Run(async () =>
                {
                    this.Monitor.Log($"{Game1.player.Name} VIBRATING AT {intensity}.", LogLevel.Trace);
                    await buttplugManager.VibrateDevice(intensity);
                    await Task.Delay(380);
                    await buttplugManager.VibrateDevice(0);
                });
            }
            // Update the previous health value for the next tick
            previousHealth = Game1.player.health;
            
            //Check if the player is riding a horse
            if (e.IsMultipleOf(20)) HorseRidingCheck();


            // Vibrate the plug for keepalive
            if (e.IsMultipleOf((uint)Config.KeepAliveInterval*60))
            {
                if (!Config.KeepAlive) return;
                int duration = 250;
                buttplugManager.VibrateDevicePulse(Config.KeepAliveLevel, duration);
            }
        }

        private void HorseRidingCheck()
        {
            if (!Context.IsWorldReady) return;
            
            // Check if the player is riding a horse
            bool isRidingHorse = Game1.player.isRidingHorse();

            if (isRidingHorse && Config.VibrateOnHorse)
            {
                //Deliberately not including a check for if you werent riding a horse before in case some other vibration interrupts the horseriding
                wasRidingHorse = true;
                buttplugManager.VibrateDevice(Config.HorseLevel);
            }
            else
            {
                if (wasRidingHorse)
                {
                    // Player just left the horse, vibrate once at 0
                    buttplugManager.VibrateDevice(0);
                }
                // Set the toggle to false since the player is not riding the horse
                wasRidingHorse = false;
            }
        }
 
        private void ArcadeMinigames(object sender, UpdateTickedEventArgs e)
        {
            //junimo kart lives trigger
            if (!Context.IsWorldReady || Game1.currentMinigame == null) return;

            if (Game1.currentMinigame is MineCart game && e.IsMultipleOf(5))
            {
                IReflectedField<int> minekartLives = Helper.Reflection.GetField<int>(game, "livesLeft");
                if (minekartLives.GetValue() < previousMinekartHealth)
                {
                    buttplugManager.VibrateDevicePulse(Config.ArcadeLevel);
                    this.Monitor.Log($"{Game1.player.Name} Life lost. Vibrating at {Config.ArcadeLevel}.", LogLevel.Trace);
                }
                previousMinekartHealth = minekartLives.GetValue();
                

                IReflectedField<int> livesLeft = Helper.Reflection.GetField<int>(game, "livesLeft");
            }
            //ABIGAIL GAME TRIGGER
            if (Game1.currentMinigame is AbigailGame abigailGame && e.IsMultipleOf(5))
            {
                IReflectedField<int> abigailLives = Helper.Reflection.GetField<int>(abigailGame, "lives");
                if (abigailLives.GetValue() != previousAbigailHealth)
                {
                    buttplugManager.VibrateDevicePulse(Config.ArcadeLevel, 600);
                    this.Monitor.Log($"{Game1.player.Name} Life lost. Vibrating at {Config.ArcadeLevel}.", LogLevel.Trace);
                }
                previousAbigailHealth = abigailLives.GetValue();
                
                IReflectedField<int> coinsField = Helper.Reflection.GetField<int>(abigailGame, "coins");
                int currentCoins = coinsField.GetValue();
                if (currentCoins > previousCoins)
                {
                    // Coin collected, trigger vibration
                    buttplugManager.VibrateDevicePulse(Config.ArcadeLevel, 600);
                    Monitor.Log($"{Game1.player.Name} Coin collected. Vibrating at {Config.ArcadeLevel}.", LogLevel.Trace);
                }
                previousCoins = currentCoins;
                
            }
        }
        public void Unload()
        {
            buttplugManager.DisconnectButtplug();
        }

    }
}
