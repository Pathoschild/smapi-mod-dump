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
    using StardewModdingAPI.Utilities;
    using StardewValley;
    using StardewValley.TerrainFeatures;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class ForageFantasy : Mod, IAssetEditor
    {
        public ForageFantasyConfig Config { get; set; }

        internal bool TappersDreamAndMushroomTreesGrowInWinter { get; set; }

        public static int DetermineForageQuality(Farmer farmer, bool allowBotanist = true)
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

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ForageFantasyConfig>();

            ForageFantasyConfig.VerifyConfigValues(Config, this);

            Helper.Events.GameLoop.GameLaunched += delegate { ForageFantasyConfig.SetUpModConfigMenu(Config, this); };

            Helper.Events.GameLoop.GameLaunched += delegate { DeluxeGrabberCompatibility.Setup(this); };

            Helper.Events.GameLoop.DayStarted += delegate
            {
                TapperAndMushroomQualityLogic.IncreaseTreeAges(this);
                GrapeLogic.SetDropToNewGrapes(this);
                CheckForTappersDream();
            };

            Helper.Events.GameLoop.DayEnding += delegate { GrapeLogic.ResetGrapes(this); };

            Helper.Events.GameLoop.SaveLoaded += delegate { FernAndBurgerLogic.ChangeBundle(this); };

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;

            Patcher.PatchAll(this);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return FernAndBurgerLogic.CanEdit<T>(asset, Config);
        }

        public void Edit<T>(IAssetData asset)
        {
            FernAndBurgerLogic.Edit<T>(asset, Config);
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
                if (Game1.currentCursorTile == terrainfeature.Value.currentTileLocation)
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

        private void CheckForTappersDream()
        {
            try
            {
                if (Helper.ModRegistry.IsLoaded("Goldenrevolver.ATappersDream"))
                {
                    var data = Helper.ModRegistry.Get("Goldenrevolver.ATappersDream");

                    var path = data.GetType().GetProperty("DirectoryPath");

                    if (path?.GetValue(data) != null)
                    {
                        var list = ReadConfigFile("config.json", path.GetValue(data) as string, new[] { "MushroomTreesGrowInWinter" }, data.Manifest.Name, true);

                        TappersDreamAndMushroomTreesGrowInWinter = list["MushroomTreesGrowInWinter"]?.ToLower().Contains("true") == true;
                    }
                    else
                    {
                        TappersDreamAndMushroomTreesGrowInWinter = false;
                    }
                }
            }
            catch (Exception)
            {
                TappersDreamAndMushroomTreesGrowInWinter = false;
            }
        }

        private Dictionary<string, string> ReadConfigFile(string path, string modFolderPath, string[] options, string modName, bool isNonString)
        {
            string fullPath = Path.Combine(modFolderPath, PathUtilities.NormalizePath(path));

            var result = new Dictionary<string, string>();

            try
            {
                string fullText = File.ReadAllText(fullPath).ToLower();
                var split = fullText.Split('\"');
                int offset = isNonString ? 1 : 2;

                for (int i = 0; i < split.Length; i++)
                {
                    foreach (var option in options)
                    {
                        if (option.ToLower() == split[i].Trim() && i + offset < split.Length)
                        {
                            string optionText = split[i + offset].Trim();

                            result.Add(option, optionText);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog($"There was an exception while {ModManifest.Name} was reading the config for {modName}:", e);
            }

            return result;
        }
    }
}