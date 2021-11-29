/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sakorona/SDVMods
**
*************************************************/

using System.Collections.Generic;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using HarmonyLib;
using Microsoft.Xna.Framework;
using TwilightShards.Stardew.Common;
using System.Linq;

namespace HappyFishJump
{
    public class FishConfig
    {
        public float JumpChance = .30f;
        public bool SplashSound = true;
        public int NumberOfJumpingFish = 18;
        public bool LegendariesJumpAfterCatch = true;
        public float LegendaryJumpChance = .18f;
    }

    public class HappyFishJump : Mod
    {
        internal static FishConfig ModConfig;
        private List<JumpFish> _jumpingFish;
        private Dictionary<Vector2, int> _validFishLocations;

        public override void Entry(IModHelper helper)
        {
            _jumpingFish = new List<JumpFish>();
            _validFishLocations = new Dictionary<Vector2, int>();

            var harmony = new Harmony("koihimenakamura.happyfishjump");
            harmony.PatchAll();
            Monitor.Log("Patching JumpingFish.Splash with a transpiler", LogLevel.Trace);

            ModConfig = Helper.ReadConfig<FishConfig>();

            Helper.Events.Player.Warped += Player_Warped;
            Helper.Events.Display.RenderedWorld += Display_RenderedWorld;
            Helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            Helper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged;
        }

        private void Display_RenderedWorld(object sender, StardewModdingAPI.Events.RenderedWorldEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            foreach (var t in this._jumpingFish)
            {
                t.Draw(Game1.spriteBatch);
            }
        }

        private void Player_Warped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (Game1.currentLocation != null)
            {
                _validFishLocations.Clear();
                PopulateValidFishLocations();
            }
        }
        
        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            for (int index = 0; index < this._jumpingFish.Count; ++index)
            {
                JumpFish jumpingFish = this._jumpingFish[index];
                var elapsedGameTime = Game1.currentGameTime.ElapsedGameTime;
                double totalSeconds = elapsedGameTime.TotalSeconds;
                if (jumpingFish.Update((float)totalSeconds))
                {
                   this._jumpingFish.RemoveAt(index);
                    --index;
                }
            }
        }

        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            var GMCMapi = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (GMCMapi != null)
            {
                GMCMapi.RegisterModConfig(ModManifest, () => ModConfig = new FishConfig(), () => Helper.WriteConfig(ModConfig));
                GMCMapi.RegisterSimpleOption(ModManifest,"Splash Sound","Disable to stop the splash sound on jump", () => ModConfig.SplashSound, (bool val) => ModConfig.SplashSound = val);
                GMCMapi.RegisterSimpleOption(ModManifest, "Legendaries Jump After Catch", "If enabled, legendaries will jump after you catch them.", () => ModConfig.LegendariesJumpAfterCatch, (bool val) => ModConfig.LegendariesJumpAfterCatch = val);
                GMCMapi.RegisterClampedOption(ModManifest,"Jumping Fish","The number of jumping fish. Minimum 2. Note that large numbers may lag a computer.", () => ModConfig.NumberOfJumpingFish, (int val) => ModConfig.NumberOfJumpingFish = val,2,1000);
                GMCMapi.RegisterSimpleOption(ModManifest,"Jump Chance","Controls the jump chance per pond every 10 minutes.", () => ModConfig.JumpChance,
                    (float val) => ModConfig.JumpChance = val);
            }
        }

#pragma warning disable IDE0060 // Remove unused parameter - it's not unused, for one thing. :V
        public static void PlaySound(string cueName)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (ModConfig.SplashSound)
            {
                Game1.playSound("dropItemInWater");
            }
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            if (Game1.currentLocation != null && Game1.currentLocation is Farm f)
            {
                foreach (var v in f.buildings)
                {
                    if (v is FishPond fp)
                    {
                        if (Game1.random.NextDouble() <= ModConfig.JumpChance)
                        {
                            NetEvent0 animateHappyFishEvent = this.Helper.Reflection
                                .GetField<NetEvent0>(fp, "animateHappyFishEvent").GetValue();
                            animateHappyFishEvent.Fire();
                        }
                    }
                }
            }

            if (Game1.currentLocation != null && Game1.currentLocation.IsOutdoors && _validFishLocations.Count >= ModConfig.NumberOfJumpingFish)
            {
                //get fish
                Dictionary<int, int> fishLocation = SDVUtilities.GetFishListing(Game1.currentLocation);

                if (fishLocation.Keys.Count <= 0)
                    return;

                //legendaries!
                List<int> legendaryFishAdded = new List<int>();
                if (ModConfig.LegendariesJumpAfterCatch) {
                    switch (Game1.currentLocation.Name)
                    {
                        case "ExteriorMuseum":
                            foreach(var v in Game1.objectInformation)
                            {
                                    if (v.Value.Split('/')[0] == "Pufferchick" && Game1.player.fishCaught.ContainsKey(v.Key))
                                    {
                                        legendaryFishAdded.Add(v.Key);
                                        fishLocation.Add(v.Key, -1);
                                    }
                            }
                            break;
                        case "Mountain":
                            if (Game1.currentSeason == "spring" && Game1.player.fishCaught.ContainsKey(163))
                            {
                                legendaryFishAdded.Add(163);
                                fishLocation.Add(163, -1);
                            }
                            break;
                        case "Beach":
                            if (Game1.currentSeason == "summer" && Game1.player.fishCaught.ContainsKey(159))
                            {
                                legendaryFishAdded.Add(159);
                                fishLocation.Add(159, -1);
                            }
                            break;
                        case "Forest":
                            if (Game1.currentSeason == "winter" && Game1.player.fishCaught.ContainsKey(775))
                            {
                                legendaryFishAdded.Add(775);
                                fishLocation.Add(775, 0);
                            }
                            break;
                        case "Town":
                            if (Game1.currentSeason == "fall" && Game1.player.fishCaught.ContainsKey(160))
                                fishLocation.Add(160, -1);
                            break;
                        case "Sewer":
                            if (Game1.player.fishCaught.ContainsKey(682))
                                fishLocation.Add(682, - 1);
                            break;
                    }
                }

                int[] fishIDs = fishLocation.Keys.ToArray();
                Vector2[] validLocs = _validFishLocations.Keys.ToArray();

                for (int i = 0; i < ModConfig.NumberOfJumpingFish; i++)
                {
                    if (Game1.random.NextDouble() > ModConfig.JumpChance)
                        continue;

                    int rndFish = Game1.random.Next(0, fishIDs.Count() - 1);

                    if (legendaryFishAdded.Contains(rndFish) && Game1.random.NextDouble() < ModConfig.LegendaryJumpChance)
                    {
                        int loopCheck = 0;
                        do
                        {
                            rndFish = Game1.random.Next(0, fishIDs.Count() - 1);
                            i++;
                        } while (legendaryFishAdded.Contains(rndFish) && loopCheck < 12000);
                    }

                    int rndLoc = Game1.random.Next(0, validLocs.Count() - 1);
                    if (fishLocation[fishIDs[rndFish]] == -1 ||
                        fishLocation[fishIDs[rndFish]] == _validFishLocations[validLocs[rndLoc]] && rndFish != 0)
                    {
                        StardewValley.Object fish = new StardewValley.Object(fishIDs[rndFish],1, false, -1, 0);
                        var startPosition = validLocs[rndLoc];
                        var endPosition = new Vector2(startPosition.X + 1.5f, startPosition.Y + 1.5f);
                        if (ValidFishForJumping(fish.ParentSheetIndex))
                            this._jumpingFish.Add(new JumpFish(fish, startPosition * Game1.tileSize,
                                endPosition * Game1.tileSize));
                    }
                }
            }
        }

        private void PopulateValidFishLocations()
        {
            //get starting position
            if (Game1.currentLocation.waterTiles is null)
                return;

            for (int j = 0; j < Game1.currentLocation.map.Layers[0].LayerWidth; j++)
            {
                for (int k = 0; k < Game1.currentLocation.map.Layers[0].LayerHeight; k++)
                {
                    if (Game1.currentLocation.waterTiles[j, k])
                    {
                        if ((j + 2) >= Game1.currentLocation.map.Layers[0].LayerWidth ||
                            (k + 2) >= Game1.currentLocation.map.Layers[0].LayerHeight)
                            continue;

                        if (Game1.currentLocation.waterTiles[j + 2, k + 2])
                        {
                            Vector2 pos = new Vector2(j, k);
                            _validFishLocations.Add(pos, Game1.currentLocation.getFishingLocation(pos));
                        }
                    }
                }
            }
        }
        
        private bool ValidFishForJumping(int index)
        {
            switch (index)
            {
                case 372:
                case 718:
                case 719:
                case 721:
                case 152:
                case 153:
                case 157:
                case 723:
                    return false;
                default:
                    return true;
            }
        }
    }
}