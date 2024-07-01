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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace ButtplugValley
{
    internal sealed class ModEntry : Mod
    {
        private static BPManager buttplugManager;
        private static ModConfig SConfig;
        private ModConfig Config;
        private FishingMinigame fishingMinigame;
        private ConfigMenu configMenu;
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
        
        public static IMonitor StaticMonitor { get; private set; }
        public static BPManager StaticButtplugManager { get; private set; }
        
        public static ModConfig StaticConfig { get; private set; }

        private static IMonitor ModMonitor;

        private readonly static string[] darkClubSounds = {"badend", "fellatio01", "fellatio02", "fellatio03", "fellatio04", "fellatio05", "fuck01", "fuck02", "fuck03"};
        private readonly static string[] darkClubSoundsOther = {"moan01", "moan02", "moan03", "moan04", "moan05", "moan06", "moan07", "moan08", "moan09", "moan10", "moan11", "moan12", "moan13", "moan14", "moan15", "pant01", "pant02", "pant03", "pant04", "pant05"};


        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            SConfig = this.Config;
            buttplugManager = new BPManager();
            fishingMinigame = new FishingMinigame(helper, Monitor, buttplugManager);
            configMenu = new ConfigMenu(helper, Monitor, buttplugManager, Config, this.ModManifest);
            new FishingRod(helper, Monitor, buttplugManager, Config);
            Task.Run(async () =>
            {
                await buttplugManager.ConnectButtplug(Monitor, Config.IntifaceIP);
                await buttplugManager.ScanForDevices();

            });

            StaticMonitor = Monitor;
            StaticButtplugManager = buttplugManager;
            StaticConfig = Config;
            
            ModMonitor = this.Monitor;

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
            

            var harmony = new Harmony(this.ModManifest.UniqueID);
            
            
            // Tree hit harmony patch
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), nameof(Tree.performToolAction)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(TreeHit_Postfix))
            );

            // Tree fell harmony patch
            harmony.Patch(
                original: AccessTools.Method(typeof(Tree), "performTreeFall"),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(TreeFall_Postfix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(WateringCan), nameof(WateringCan.DoFunction)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(WateringCan_Postfix))
            );
            
            harmony.Patch(
                original: AccessTools.Method(typeof(Hoe), nameof(Hoe.DoFunction)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(Hoe_Postfix))
            );
            //Harmony kiss detect
            MethodInfo originalCheckKiss = AccessTools.Method(typeof(Farmer), nameof(Farmer.PerformKiss));           
            harmony.Patch(original: originalCheckKiss, new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Kissing_Postfix)));


            //SV has multiple ways of playing sound, so I need to capture all the sounds
            MethodInfo prefixCheckLocal2 = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.localSound));
            MethodInfo prefixCheckLocal = AccessTools.Method(typeof(GameLocation), nameof(GameLocation.playSound));
            harmony.Patch(prefixCheckLocal, new HarmonyMethod(typeof(ModEntry), nameof(PlaySoundPrefix)));
            harmony.Patch(prefixCheckLocal2, new HarmonyMethod(typeof(ModEntry), nameof(PlaySoundPrefix)));
            var methodList = typeof(Game1).GetMethods();
            foreach (var method in methodList)
            {
                if (method.Name == "playSound" && method.GetParameters().Length > 0)
                {                  
                    harmony.Patch(method, new HarmonyMethod(typeof(ModEntry), nameof(PlaySoundPrefix)));
                }
            }
        }
        
        public static void TreeHit_Postfix(Tree __instance)
        {
            StaticMonitor.Log("Tree hit", LogLevel.Info);
            StaticButtplugManager.VibrateDevicePulse(StaticConfig.TreeChopLevel, 300);
            // Use StaticButtplugManager as needed
        }

        // This method will be called after a tree falls
        public static void TreeFall_Postfix(Tree __instance)
        {
            StaticMonitor.Log("Tree fell", LogLevel.Info);
            StaticButtplugManager.VibrateDevicePulse(StaticConfig.TreeFellLevel, 2000);
        }
        
        public static void WateringCan_Postfix(WateringCan __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            int intensity = 25 * (power+1);

            
            StaticMonitor.Log($"Watering can used with power {power}, intensity {intensity}", LogLevel.Info);
            
            StaticButtplugManager.VibrateDevicePulse(intensity, 300);
        }

        public static void Hoe_Postfix(Hoe __instance, GameLocation location, int x, int y, int power, Farmer who)
        {
            int intensity = 25 * (power + 1);

            StaticMonitor.Log($"Hoe used with power {power}, intensity {intensity}", LogLevel.Info);

            StaticButtplugManager.VibrateDevicePulse(intensity, 300);
        }

        static async void OnSoundPlayed(string cueName)
        {                                  
            if (SConfig.VibrateOnSexScene) {
                // Almost all sex scenes in mods use these sounds
                if ((cueName == "slimeHit" || cueName == "fishSlap" || cueName == "gulp") && Game1.eventUp)
                {                 
                    buttplugManager.VibrateDevicePulse(SConfig.SexSceneLevel, 150);
                    return;
                }

                //Spesificlly for cumming sex scene
                if (cueName == "swordswipe" && Game1.eventUp)
                {
                    buttplugManager.VibrateDevicePulse(SConfig.SexSceneLevel, 600);
                    return;
                }
            }
            // VibrateOnRainsInteractionMod sex sound
            if (SConfig.VibrateOnRainsInteractionMod && cueName == "ButtHit")
            {                
                buttplugManager.VibrateDevicePulse(SConfig.RainsInteractionModLevel, 100);
                return;
            }
            if (SConfig.VibrateOnHorse)
            {
                bool isRidingHorse = Game1.player.isRidingHorse();
                if (isRidingHorse)
                {
                    // All step sounds while riding will be procesed as movement
                    // Don't add this to the queue, so if it's a little laggy, it will create a different pattern
                    if (cueName.Contains("Step"))
                    {                       
                        buttplugManager.VibrateDevicePulse(SConfig.HorseLevel, 100);
                        return;
                    }
                }
            }
            if (SConfig.VibrateOnDarkClubMoans)
            {
                // Sounds of machines, slaves, etc.
                foreach (string soundName in darkClubSoundsOther)
                {
                    if (soundName.Contains(cueName))
                    {                        
                        buttplugManager.VibrateDevicePulse(SConfig.DarkClubMoanLevel, 150);
                    }
                }
            }

            if(SConfig.VibrateOnDarkClubSex)
            {
                foreach (string soundName in darkClubSounds)
                {
                    if (soundName.Contains(cueName))
                    {
                        // There are multiple intensities during a sex scene separated by a number in the sound name
                        ICue testC = Game1.soundBank.GetCue(cueName);
                        testC.Play();
                        string numString = Regex.Match(cueName, @"\d+\.*\d*").Value;
                        int num = 0;
                        if (numString != "")
                        {
                            num = int.Parse(numString);
                        }
                        double power = SConfig.MaxDarkClubSexLevel;
                        if (num == 1)
                        {
                            power = Math.Round(power * 0.3);
                        }
                        if (num == 2)
                        {
                            power = Math.Round(power * 0.6);
                        }

                        // Sex sounds in this mod have usually have multiple seconds, so I need a loop with a delay
                        // Might be interesting to put a random deley in there
                        while (testC.IsPlaying)
                        {
                            if (!Game1.eventUp)
                            {
                                testC.Stop(AudioStopOptions.Immediate);
                                return;
                            }                            
                            buttplugManager.VibrateDevicePulse((float)power, 100);
                            await Task.Delay(300);
                        }

                    }
                }
            }
        }
        


        // Just to merge and ignore other arguments besides the sound name
        public static void PlaySoundPrefix(object __0)
        {            
            if (__0 is string soundName)
            {
                OnSoundPlayed(soundName);
            }           
        }

        private static async void Kissing_Postfix()
        {
            try
            {   if (SConfig.VibrateOnKiss)
                {                   
                    await buttplugManager.VibrateDevicePulse(100, 1000);
                }

            }
            catch (Exception ex)
            {
                ModMonitor.Log($"Failed in {nameof(Kissing_Postfix)}:\n{ex}", LogLevel.Error);
            }
        }

        private async void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox && Config.VibrateOnDialogue)
            {
                Monitor.Log("Dialogue Box Triggered", LogLevel.Trace);
                await buttplugManager.VibrateDevicePulse(Config.DialogueLevel, 550);
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
            buttplugManager.config = this.Config;
            this.configMenu.LoadConfigMenu();
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
                    
                    //if feature is grass
                    if (feature.Value is Grass grass && Config.VibrateOnGrass)
                    {
                        Monitor.Log($"Removed {feature.Value.GetType().Name}", LogLevel.Trace);
                        Task.Run(async () =>
                        {
                            this.Monitor.Log($"{Game1.player.Name} VIBRATING AT {Config.GrassLevel}.", LogLevel.Trace);
                            await buttplugManager.VibrateDevicePulse(Config.GrassLevel, 300);
                        });
                    }
                    
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


            // Vibrate the plug for keepalive
            if (e.IsMultipleOf((uint)Config.KeepAliveInterval*60))
            {
                if (!Config.KeepAlive) return;
                int duration = 250;
                buttplugManager.VibrateDevicePulse(Config.KeepAliveLevel, duration);
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
