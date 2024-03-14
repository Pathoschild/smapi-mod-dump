/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace ForageFantasy
{
    using StardewModdingAPI;
    using StardewModdingAPI.Events;
    using StardewValley;
    using StardewValley.GameData.WildTrees;
    using StardewValley.TerrainFeatures;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ForageFantasy : Mod
    {
        public ForageFantasyConfig Config { get; set; }

        public static IManifest Manifest { get; set; }

        internal static string FineGrapeAssetPath { get; private set; }

        internal static bool MushroomTreeTapperWorksInWinter { get; set; }

        // maybe TODO seasonal mushroom tappers, magma cap on ginger island for mushroom tree and box

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ForageFantasyConfig>();
            Manifest = this.ModManifest;

            ForageFantasyConfig.VerifyConfigValues(Config, this);

            FineGrapeAssetPath = Helper.ModContent.GetInternalAssetName("assets/fineGrape.png").BaseName;

            helper.Events.GameLoop.GameLaunched += delegate
            {
                ForageFantasyConfig.SetUpModConfigMenu(Config, this);
                DeluxeGrabberCompatibility.Setup(this);
            };

            helper.Events.GameLoop.DayStarted += delegate
            {
                TapperAndMushroomQualityLogic.IncreaseTreeAges(this);
                GrapeLogic.SetDropToNewGrapes(this);
            };

            helper.Events.GameLoop.DayEnding += delegate { GrapeLogic.ResetGrapes(this); };

            helper.Events.GameLoop.SaveLoaded += delegate { FernAndBurgerLogic.UpdateExistingBundle(this); };

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;

            helper.Events.Content.AssetRequested += OnAssetRequested;

            Patcher.PatchAll(this);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            FernAndBurgerLogic.Apply(e, this.Config, this.Helper.Translation);

            TapperAssetChanges.Apply(e, this.Config, this.Helper.Translation);

            GrapeLogic.Apply(e, this.Config, this.Helper.Translation);

            if (e.NameWithoutLocale.IsEquivalentTo("Data/WildTrees"))
            {
                e.Edit((asset) =>
                {
                    IDictionary<string, WildTreeData> data = asset.AsDictionary<string, WildTreeData>().Data;

                    if (data.TryGetValue(Tree.mushroomTree, out var mushroomTreeData))
                    {
                        var defaultTap = mushroomTreeData.TapItems.Where((s) => s.Id == "Default").FirstOrDefault();

                        if (defaultTap?.Condition != null && defaultTap.Condition.Contains("!LOCATION_SEASON Target Winter"))
                        {
                            MushroomTreeTapperWorksInWinter = false;
                        }
                        else
                        {
                            MushroomTreeTapperWorksInWinter = !mushroomTreeData.IsStumpDuringWinter;
                        }
                    }
                }, AssetEditPriority.Late + (int)AssetEditPriority.Late);
            }
        }

        // do not combine these overloads with a default parameter. this one is used in a transpiler patch
        public static int DetermineForageQuality(Farmer farmer)
        {
            return DetermineForageQuality(farmer, true);
        }

        public static int DetermineForageQuality(Farmer farmer, bool allowBotanist)
        {
            if (allowBotanist && farmer.professions.Contains(Farmer.botanist))
            {
                return 4;
            }
            else
            {
                if (Game1.random.NextDouble() < farmer.ForagingLevel / 30f)
                {
                    return 2;
                }
                else if (Game1.random.NextDouble() < farmer.ForagingLevel / 15f)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Small helper method to log to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        /// <summary>
        /// Small helper method to log an error to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        /// <param name="e">an optional error message to log additionally</param>
        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree)
            {
                return;
            }

            // this is done in buttonsChanged instead of buttonPressed as recommended
            // in the documentation: https://stardewcommunitywiki.com/Modding:Modder_Guide/APIs/Input#KeybindList
            if (Config.TreeMenuKey.JustPressed())
            {
                OpenTreeMenu(Game1.currentLocation);
            }
        }

        private void OpenTreeMenu(GameLocation currentLocation)
        {
            foreach (var terrainfeature in currentLocation.terrainFeatures.Pairs)
            {
                if (Game1.currentCursorTile == terrainfeature.Value.Tile)
                {
                    if (terrainfeature.Value is Tree tree)
                    {
                        if (tree.growthStage.Value >= 5)
                        {
                            Game1.activeClickableMenu = new TreeMenu(this, tree);
                            return;
                        }
                    }

                    if (terrainfeature.Value is FruitTree fruittree)
                    {
                        // fruit tree ages are negative
                        if (fruittree.daysUntilMature.Value <= 0)
                        {
                            Game1.activeClickableMenu = new TreeMenu(this, fruittree);
                            return;
                        }
                    }
                }
            }
        }
    }
}