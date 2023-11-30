/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cl4r3/Halloween-Mod-Jam-2023
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using static TricksAndTreats.Globals;

namespace TricksAndTreats
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Globals.Initialize(this, helper);

            Tricks.Initialize(this);
            Treats.Initialize(this);
            Costumes.Initialize(this);

            //helper.Events.Display.RenderingWorld += OnRenderingWorld;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Specialized.LoadStageChanged += OnLoadStageChanged;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.Content.AssetReady += OnAssetReady;
            helper.Events.GameLoop.DayStarted += DayStart;
            helper.Events.GameLoop.DayEnding += DayEnd;
            helper.Events.GameLoop.TimeChanged += OnTimeChange;
        }

        /*
        private static void OnRenderingWorld(object sender, RenderingWorldEventArgs e)
        {
            Game1.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            Game1.graphics.ApplyChanges();
            Game1.graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.Stencil, Color.Transparent, 0, 0);
        }
        */

        private void OnGameLaunched(object sender, EventArgs e)
        {
            ClothingInfo = Helper.Data.ReadJsonFile<Dictionary<string, int>>(PathUtilities.NormalizePath("assets/clothing.json"));
            FoodInfo = Helper.Data.ReadJsonFile<Dictionary<string, int>>(PathUtilities.NormalizePath("assets/food.json"));
            HatInfo = Helper.Data.ReadJsonFile<Dictionary<string, int>>(PathUtilities.NormalizePath("assets/hats.json"));

            JA = Helper.ModRegistry.GetApi<IJsonAssetsApi>("spacechase0.JsonAssets");
            if (JA == null)
            {
                Log.Error("Json Assets API not found. Please check that JSON Assets is correctly installed.");
                return;
            }
            JA.LoadAssets(Path.Combine(Helper.DirectoryPath, JAPath), Helper.Translation);

            Config = Helper.ReadConfig<ModConfig>();
            GMCM = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            Globals.ConfigMenu.RegisterGMCM();

            CP = Helper.ModRegistry.GetApi<IContentPatcherApi>("Pathoschild.ContentPatcher");
            if (CP == null)
            {
                Log.Error("Content Patcher API not found. Please check that Content Patcher is correctly installed.");
                return;
            }
            Globals.ConfigMenu.RegisterTokens();

            HarmonyPatches.Patch(id: ModManifest.UniqueID);
        }

        private void OnLoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == LoadStage.CreatedInitialLocations || e.NewStage == LoadStage.SaveAddedLocations)
                Game1.locations.Add(new GameLocation(Helper.ModContent.GetInternalAssetName("assets/Maze.tmx").BaseName, "Custom_TaT_Maze"));
        }

        [EventPriority(EventPriority.High)]
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            NPCData = Game1.content.Load<Dictionary<string, Celebrant>>(AssetPath + NPCsExt);
            CostumeData = Game1.content.Load<Dictionary<string, Costume>>(AssetPath + CostumesExt);
            TreatData = Game1.content.Load<Dictionary<string, Treat>>(AssetPath + TreatsExt);

            Utils.ValidateNPCData();
            Utils.ValidateCostumeData();
            Utils.ValidateTreatData();
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.Name.IsEquivalentTo(AssetPath + NPCsExt))
                e.LoadFrom(() => new Dictionary<string, Celebrant>(), AssetLoadPriority.Exclusive);
            else if (e.Name.IsEquivalentTo(AssetPath + CostumesExt))
                e.LoadFrom(() => new Dictionary<string, Costume>(), AssetLoadPriority.Exclusive);
            else if (e.Name.IsEquivalentTo(AssetPath + TreatsExt))
                e.LoadFrom(() => new Dictionary<string, Treat>(), AssetLoadPriority.Exclusive);

        }

        private void OnAssetReady(object sender, AssetReadyEventArgs e)
        {
            if (e.Name.IsEquivalentTo(AssetPath + NPCsExt))
                NPCData = Game1.content.Load<Dictionary<string, Celebrant>>(AssetPath + NPCsExt);
            else if (e.Name.IsEquivalentTo(AssetPath + CostumesExt))
                CostumeData = Game1.content.Load<Dictionary<string, Costume>>(AssetPath + CostumesExt);
            else if (e.Name.IsEquivalentTo(AssetPath + TreatsExt))
                TreatData = Game1.content.Load<Dictionary<string, Treat>>(AssetPath + TreatsExt);
        }

        [EventPriority(EventPriority.Low - 100)]
        private static void DayStart(object sender, DayStartedEventArgs e)
        {
            Tricks.CheckHouseTrick();

            Farmer farmer = Game1.player;
            if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 27)
            {
                // Reset modData stuff
                foreach (string key in farmer.modData.Keys.Where(x => { return (x == StolenKey || x == ChestKey || x == CostumeKey); }).ToArray())
                    Log.Trace($"TaT: Removed modData key {key}: " + farmer.modData.Remove(key));
                // Remove CTs
                foreach (string key in farmer.activeDialogueEvents.Keys.Where(x => { return (x.Contains(CostumeCT) || x == HouseCT || x == TreatCT); }).ToArray())
                    Log.Trace($"TaT: Removed CT key {key}: " + farmer.activeDialogueEvents.Remove(key));
                // Remove mail flags
                foreach (string mail in farmer.mailReceived.Where(x => { return (x.Contains(TreatCT) || x.Contains(HouseCT) || x.Contains(CostumeCT) || x == HouseFlag); }).ToArray())
                    Log.Trace($"TaT: Removed mail flag {mail}: " + farmer.mailReceived.Remove(mail));

                // In case player is already wearing costume
                Costumes.CheckForCostume();
            }
            if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 28)
            {
                // Undo paint
                if (farmer.modData.ContainsKey(PaintKey))
                {
                    farmer.changeSkinColor(int.Parse(farmer.modData[PaintKey]), true);
                    farmer.modData.Remove(PaintKey);
                }

                // Add House CT if necessary
                if (farmer.mailReceived.Contains(HouseFlag))
                    farmer.activeDialogueEvents.Add(HouseCT, 1);
            }
        }

        private static void DayEnd(object sender, DayEndingEventArgs e)
        {
            if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 27)
            {
                // reset nickname if changed
                Game1.player.Name = Game1.player.displayName;

                if (Config.ScoreCalcMethod != "none")
                {
                    int score = int.Parse(Game1.player.modData[ScoreKey]);
                    int min = Config.ScoreCalcMethod == "minmult" ? (int)Math.Round(NPCData.Keys.Count * Config.CustomMinMult) : Config.CustomMinVal;
                    Log.Trace($"TaT: Total treat score for {Game1.player.Name} is {score}, min score needed to avoid house prank is {min}.");
                    if (score < min)
                        Game1.player.mailReceived.Add(HouseFlag);
                    Game1.player.modData.Remove(ScoreKey);
                }
                else
                    Log.Trace($"TaT: House pranks disabled; skipping score calculation.");
            }
            else if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 28)
            {
                if (Game1.player.mailReceived.Contains(HouseFlag))
                    Game1.player.mailReceived.Remove(HouseFlag);
            }
        }

        private void OnTimeChange(object sender, TimeChangedEventArgs e)
        {
            if (Game1.currentSeason == "fall" && Game1.dayOfMonth == 27)
            {
                if (Game1.timeOfDay < 2100)
                    Game1.whereIsTodaysFest = null;
                else if (Game1.timeOfDay >= 2100)
                {
                    Game1.whereIsTodaysFest = "Town";
                    if (Game1.timeOfDay < 2400)
                    {
                        Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("info.festival_prep"));
                        Game1.warpFarmer("BusStop", 34, 23, 3);
                    }
                }
            }
        }
    }
}
