/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewMods
**
*************************************************/

using AchtuurCore.Events;
using AchtuurCore.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using Microsoft.Xna.Framework;
using StardewValley;
using HoverLabels.Framework;
using HoverLabels.Labels;
using AchtuurCore.Utility;
using System.Xml.Linq;
using SObject = StardewValley.Object;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HoverLabels.Labels.Buildings;
using HoverLabels.Labels.Objects;

namespace HoverLabels
{
    public class ModEntry : Mod
    {
        /// <summary>
        /// Number of days in an ingame season
        /// </summary>
        internal const int DaysInSeason = 28;

        internal static readonly List<string> Seasons = new()
        {
            "spring",
            "summer",
            "fall",
            "winter"
        };

        internal static readonly List<string> SeasonsDisplayName = new()
        {
            I18n.SeasonSpring(),
            I18n.SeasonSummer(),
            I18n.SeasonFall(),
            I18n.SeasonWinter(),
        };

        internal static ModEntry Instance;
        internal ModConfig Config;

        internal IHoverLabelApi HoverLabelApi;
        internal LabelManager LabelManager { get; set; }
        internal LabelOverlay Overlay { get; set; }

        /// <summary>
        /// Number of ticks the cursor is hovering over <see cref="cursorHoverTile"/>
        /// </summary>
        private int cursorHoverCount;

        internal static bool IsShowDetailButtonPressed()
        {
            return Instance.Helper.Input.IsDown(Instance.Config.ShowDetailsButton);
        }

        internal static string GetShowDetailButtonName()
        {
            return SplitCapitalCase(Instance.Config.ShowDetailsButton.ToString());
        }

        internal static bool IsAlternativeSortButtonPressed()
        {
            return Instance.Helper.Input.IsDown(Instance.Config.AlternativeSortButton);
        }

        internal static string GetAlternativeSortButtonName()
        {
            return SplitCapitalCase(Instance.Config.AlternativeSortButton.ToString());
        }

        /// <summary>
        /// Returns size limit of labels, depending on if <see cref="ModConfig.ShowDetailsButton"/> is pressed
        /// </summary>
        /// <returns></returns>
        internal static int GetLabelSizeLimit()
        {
            return IsShowDetailButtonPressed()
                ? int.MaxValue
                : Instance.Config.LabelListMaxSize;

        }
        
        /// <summary>
        /// returns true if player is currently on the farm (outdoors)
        /// </summary>
        /// <returns></returns>
        internal static bool IsPlayerOnFarm()
        {
            return Game1.currentLocation.IsFarm && Game1.currentLocation.IsOutdoors;
        }

        internal static SObject GetObjectWithId(int id)
        {
            return new SObject(id, 1, false, -1, 0);
        }

        internal static string GetDateAfterDays(int days)
        {
            int new_day = days + Game1.dayOfMonth;
            if (new_day > DaysInSeason)
            {
                int season_diff = new_day / DaysInSeason;
                int current_season = Seasons.IndexOf(Game1.currentSeason);
                string season_name = SeasonsDisplayName[(current_season + season_diff) % 4];

                // Final result should be new_day % 28, where 28*k % 28 = 1.
                // (thank you Shockah)
                int day = (new_day - 1) % DaysInSeason + 1;
                return $"{season_name} {day}";
            }

            string season = SeasonsDisplayName[Seasons.IndexOf(Game1.currentSeason)];
            return $"{season} {new_day}";
        }

        public override void Entry(IModHelper helper)
        {

            I18n.Init(helper.Translation);
            ModEntry.Instance = this;

            // HarmonyPatcher.ApplyPatches(this,
            
            // );

            this.Config = this.Helper.ReadConfig<ModConfig>();

            this.cursorHoverCount = 0;
            this.LabelManager = new LabelManager();
            this.Overlay = new LabelOverlay();

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            Debug.DebugOnlyExecute(() =>
            {
                helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            });
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.RightControl)
            {
                this.RegisterLabels();
                AchtuurCore.Logger.DebugLog(ModEntry.Instance.Monitor, $"Registered labels");
            }
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.Config.createMenu();
        }

        public override object GetApi(IModInfo mod)
        {
            return new HoverLabelApi(mod.Manifest);
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            this.Overlay.DrawOverlay(e.SpriteBatch);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            UpdateCursorHover();
        }

        private void UpdateCursorHover()
        {
            // Reset cursor tile position if it shouldn't be enabled
            Vector2 cursorTile = Game1.currentCursorTile;
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
            {
                ResetCursorHoverState();
                return;
            }

            // Create label when tick count is larger than delay and no label exists yet
            cursorHoverCount++;
            if (cursorHoverCount >= Config.LabelPopupDelayTicks)
            {
                LabelManager.TrySetLabel(cursorTile);
                if (LabelManager.HasLabel())
                {
                    LabelManager.CurrentLabel.UpdateCursorTile(cursorTile);
                    Overlay.Enable();
                }
                // If label creation failed, nothing is being hovered over -> reset state
                // This is done here so there will be no downtime when
                // hovering over adjacent tiles that should generate labels
                else
                {
                    this.ResetCursorHoverState();
                }
            }
        }

        private void ResetCursorHoverState()
        {
            this.cursorHoverCount = 0;
            this.LabelManager.ClearLabel();
            Overlay.Disable();
        }

        private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
        {
            this.HoverLabelApi = this.Helper.ModRegistry.GetApi<IHoverLabelApi>("Achtuur.HoverLabels");
            this.RegisterLabels();
            this.Config.createMenu();
        }

        private void RegisterLabels()
        {
            // misc labels
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Crops", new CropLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Trees", new TreeLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Fruit Trees", new FruittreeLabel());

            // object labels
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Objects", new ObjectLabel(int.MinValue));
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Machines", new MachineLabel(-1));
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Chests", new ChestLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Junimo Huts", new JunimoHutLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Garden Pots", new IndoorPotLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Nodes", new NodeLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Scarecrows", new ScarecrowLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Sprinklers", new SprinklerLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Casks", new Casklabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Fish Tanks", new FishTankLabel());

            // building labels
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Buildings", new BuildingLabel(int.MinValue));
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Animal Building", new AnimalBuildingLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Greenhouse", new GreenhouseLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Fish Pond", new FishPondLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Silo", new SiloLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Slime Hutch", new SlimeHutchLabel());
            this.HoverLabelApi.RegisterLabel(this.ModManifest, "Mill", new MillLabel());

            Debug.DebugOnlyExecute(() =>
            {
                this.HoverLabelApi.RegisterLabel(this.ModManifest, "example", new ExampleLabel());
            });
        }

        private static string SplitCapitalCase(string s)
        {
            Regex r = new Regex(@"(?<=[a-z])([A-Z])(?=[a-z])");
            return r.Replace(s, @" $1");
        }
    }
}
