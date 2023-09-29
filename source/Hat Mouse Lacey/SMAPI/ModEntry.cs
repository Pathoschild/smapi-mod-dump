/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using ContentPatcher;
using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Characters;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace ichortower_HatMouseLacey
{

    public sealed class ModConfig
    {
        /*
         * AlwaysAdopt forces Lacey to adopt children no matter what the
         * farmer's sex is. The babies will be unmodified humans.
         * If false, Lacey can get pregnant with a male farmer. Any children
         * she has this way will be mice, like their mother (if the farmer is
         * female, children will be adopted humans).
         *
         * This does not affect any other spouses' children.
         *
         * default: true
         */
        public bool AlwaysAdopt { get; set; } = true;

        /*
         * DTF enables some suggestive dialogue lines (they are not explicit;
         * picture Haley's Winter Star dialogue, or sharing Emily's sleeping
         * bag).
         *
         * It will also slightly modify Lacey's 10-heart and 14-heart events.
         *
         * default: true
         */
        public bool DTF { get; set; } = true;
    }

    internal sealed class ModEntry : Mod
    {
        public static ModConfig Config = null!;

        /*
         * Controls whether to enable some CP edits for compatibility with the
         * Stardew Valley Reimagined 3 mod.
         * Automatically detected at save load time.
         */
        public static bool CompatSVR3Forest = false;

        public static IMonitor MONITOR;
        public static IModHelper HELPER;

        /*
         * Lacey's internal name. Please ensure that this matches her internal
         * name in the NPCDispositions file.
         */
        public static string LCInternalName = "HatMouseLacey";

        /*
         * Entry point.
         * Sets up the code patches and necessary event handlers.
         * OnSaveLoaded is intended to hotfix problems that can occur on saves
         * created before installing this mod.
         */
        public override void Entry(IModHelper helper)
        {
            ModEntry.MONITOR = this.Monitor;
            ModEntry.HELPER = helper;
            ModEntry.Config = helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            helper.Events.Specialized.LoadStageChanged += this.OnLoadStageChanged;
            helper.Events.Content.AssetRequested += LCCompat.OnAssetRequested;
            helper.ConsoleCommands.Add("lacey_map_repair", "\nReloads Forest map objects in the vicinity of Lacey's cabin,\nto fix the bushes in saves from before installation.\nYou shouldn't need to run this, but it's safe to do so.", this.LaceyMapRepair);
            helper.ConsoleCommands.Add("mousify_child", "\nSets or unsets mouse child status on one of your children.\nUse this if your config settings weren't right and you got the wrong children,\nor just to morph your kids for fun.\n\nUsage: mousify_child <name> <variant>\n    where <variant> is -1 (human), 0 (grey), or 1 (brown).", this.MousifyChild);
            helper.ConsoleCommands.Add("hat_string", "\nprints hat string to console", this.GetHatString);

            /*
             * Apply Harmony patches by getting all the methods in Patcher
             * and going feral with reflection on them.
             * Was this a good idea, since the annotations feature exists? No.
             * But did it save me a lot of work? Also no.
             */
            var harmony = new Harmony(this.ModManifest.UniqueID);
            MethodInfo[] funcs = typeof(Patcher).GetMethods(
                    BindingFlags.Static | BindingFlags.Public);
            try {
                Assembly sdv = typeof(StardewValley.Game1).Assembly;
                foreach (var func in funcs) {
                    string[] split = func.Name.Split("__");
                    if (split.Length < 3) {
                        MONITOR.Log($"bad Patcher function name '{func.Name}'", LogLevel.Warn);
                        continue;
                    }
                    string fqn = "StardewValley." + split[0].Replace("_", ".");
                    Type t = sdv.GetType(fqn);
                    if (t is null) {
                        MONITOR.Log($"type not found: '{fqn}'", LogLevel.Warn);
                        continue;
                    }
                    List<Type> args = new List<Type>();
                    foreach (var p in func.GetParameters()) {
                        if (p.Name == "__instance" || p.Name == "__result") {
                            continue;
                        }
                        args.Add(p.ParameterType);
                    }
                    /* there are some null arguments here because Type.GetMethod
                     * tries to match an int overload instead of the BindingFlags
                     * one if we use three arguments */
                    MethodInfo m = t.GetMethod(split[1],
                            BindingFlags.Static | BindingFlags.Instance |
                            BindingFlags.Public | BindingFlags.NonPublic,
                            null,
                            args.ToArray(),
                            null);
                    if (m is null) {
                        MONITOR.Log($"within type '{fqn}': method not found: " +
                                $"'{split[1]}({string.Join(", ", args)})'",
                                LogLevel.Warn);
                        continue;
                    }
                    var hm = new HarmonyMethod(typeof(Patcher), func.Name);
                    if (split[2] == "Prefix") {
                        harmony.Patch(original: m, prefix: hm);
                    }
                    else if (split[2] == "Postfix") {
                        harmony.Patch(original: m, postfix: hm);
                    }
                    else {
                        MONITOR.Log($"Not applying unimplemented patch type '{split[2]}'",
                                LogLevel.Warn);
                        continue;
                    }
                    MONITOR.Log($"Patched ({split[2]}) {t.FullName}.{m.Name}", LogLevel.Trace);
                }
            }
            catch (Exception e) {
                MONITOR.Log($"Caught exception while applying Harmony patches:\n{e}",
                        LogLevel.Warn);
            }
        }

        /*
         * Reset terrain features (grass, trees, bushes) around Lacey's cabin
         * by reloading them from the (patched) map data.
         * This is to make sure the save file reflects the final map, even on
         * older saves.
         */
        private void LaceyMapRepair(string command, string[] args)
        {
            this.Monitor.Log($"Reloading terrain features near Lacey's house", LogLevel.Trace);
            /* This is the rectangle to reset. It should include every tile
             * that we hit with terrain-feature map patches. */
            var rect = new Microsoft.Xna.Framework.Rectangle(25, 89, 15, 11);
            GameLocation forest = Game1.getLocationFromName("Forest");
            if (forest is null || forest.map is null) {
                return;
            }
            Layer paths = forest.map.GetLayer("Paths");
            if (paths is null) {
                return;
            }
            // forest.largeTerrainFeatures is the bushes
            var largeToRemove = new List<LargeTerrainFeature>();
            foreach (var feature in forest.largeTerrainFeatures) {
                Vector2 pos = feature.tilePosition.Value;
                if (pos.X >= rect.X && pos.X <= rect.X+rect.Width &&
                        pos.Y >= rect.Y && pos.Y <= rect.Y+rect.Height) {
                    largeToRemove.Add(feature);
                }
            }
            foreach (var doomed in largeToRemove) {
                forest.largeTerrainFeatures.Remove(doomed);
            }
            for (int x = rect.X; x < rect.X+rect.Width; ++x) {
                for (int y = rect.Y; y < rect.Y+rect.Height; ++y) {
                    Tile t = paths.Tiles[x, y];
                    if (t is null) {
                        continue;
                    }
                    if (t.TileIndex >= 24 && t.TileIndex <= 26) {
                        forest.largeTerrainFeatures.Add(
                                new StardewValley.TerrainFeatures.Bush(
                                new Vector2(x,y), 26 - t.TileIndex, forest));
                    }
                }
            }
            // forest.terrainFeatures includes grass and trees
            var smallToRemove = new List<Vector2>();
            foreach (var feature in forest.terrainFeatures.Pairs) {
                Vector2 pos = feature.Key;
                if ((feature.Value is Grass || feature.Value is Tree) &&
                        pos.X >= rect.X && pos.X <= rect.X+rect.Width &&
                        pos.Y >= rect.Y && pos.Y <= rect.Y+rect.Height) {
                    smallToRemove.Add(pos);
                }
            }
            foreach (var doomed in smallToRemove) {
                forest.terrainFeatures.Remove(doomed);
            }
            for (int x = rect.X; x < rect.X+rect.Width; ++x) {
                for (int y = rect.Y; y < rect.Y+rect.Height; ++y) {
                    Tile t = paths.Tiles[x, y];
                    if (t is null) {
                        continue;
                    }
                    if (t.TileIndex >= 9 && t.TileIndex <= 11) {
                        int treeType = t.TileIndex - 8 +
                                (Game1.currentSeason.Equals("winter") && t.TileIndex < 11 ? 3 : 0);
                        forest.terrainFeatures.Add(new Vector2(x,y),
                                new Tree(treeType, 5));
                    }
                    else if (t.TileIndex == 12) {
                        forest.terrainFeatures.Add(new Vector2(x,y),
                                new Tree(6, 5));
                    }
                    else if (t.TileIndex == 31 || t.TileIndex == 32) {
                        forest.terrainFeatures.Add(new Vector2(x,y),
                                new Tree(40 - t.TileIndex, 5));
                    }
                    else if (t.TileIndex == 22) {
                        forest.terrainFeatures.Add(new Vector2(x,y),
                                new Grass(1, 3));
                    }
                }
            }
        }

        private void MousifyChild(string command, string[] args)
        {
            if (args.Length < 2) {
                this.Monitor.Log($"Usage: mousify_child <name> <variant>", LogLevel.Warn);
                return;
            }
            if (Game1.player == null) {
                return;
            }
            Child child = null;
            try {
                foreach (var ch in Game1.player.getChildren()) {
                    if (ch.Name.Equals(args[0])) {
                        child = ch;
                        break;
                    }
                }
            }
            catch {}
            if (child == null) {
                this.Monitor.Log($"Could not find your child named '{args[0]}'.", LogLevel.Warn);
                return;
            }
            string variant = args[1];
            if (variant != "-1" && variant != "0" && variant != "1") {
                this.Monitor.Log($"Unrecognized variant '{variant}'. Using 0 instead.", LogLevel.Warn);
                variant = "0";
            }
            child.modData[$"{LCInternalName}/ChildVariant"] = variant;
            child.reloadSprite();
        }

        private void GetHatString(string command, string[] args)
        {
            this.Monitor.Log($"'{LCHatString.GetCurrentHatString(Game1.player)}'", LogLevel.Warn);
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            /*
             * Sometimes, Lacey's schedule can fail to load when this mod is
             * newly installed. This will rebuild it when that happens.
             * In practice, on new installs the map repair function will run,
             * and if that's necessary we rebuild Lacey's schedule immediately,
             * meaning this won't do anything.
             */
            NPC Lacey = Game1.getCharacterFromName(LCInternalName);
            if (Lacey.Schedule is null) {
                this.Monitor.Log($"Regenerating Lacey's schedule", LogLevel.Trace);
                Lacey.Schedule = Lacey.getSchedule(Game1.dayOfMonth);
                Lacey.checkSchedule(Game1.timeOfDay);
            }

            /*
             * When loading a save, this will attempt to convert the saved hat
             * commentary list and cruelty score from releases <= 1.0.4, where
             * they used the save data (main farmer only, barfs for farmhands).
             * They will be converted to use modData, which is safe for MP.
             */
            if (Game1.IsMasterGame) {
                LCHatsShown hs = HELPER.Data.ReadSaveData<LCHatsShown>("HatsShown");
                if (hs != null) {
                    foreach (int id in hs.ids) {
                        var obj = new StardewValley.Objects.Hat(id);
                        LCModData.AddShownHat($"SV|{obj.Name}");
                    }
                    HELPER.Data.WriteSaveData<LCHatsShown>("HatsShown", null);
                }
                LCCrueltyScore cs = HELPER.Data.ReadSaveData<LCCrueltyScore>("CrueltyScore");
                if (cs != null) {
                    LCModData.CrueltyScore = cs.val;
                    HELPER.Data.WriteSaveData<LCCrueltyScore>("CrueltyScore", null);
                }
            }
        }

        /*
         * Register Content Patcher tokens (for config mirroring).
         * Register GMCM entries.
         * Load the custom .ogg music tracks into the soundBank.
         */
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var cpapi = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>(
                    "Pathoschild.ContentPatcher");
            cpapi.RegisterToken(this.ModManifest, "AlwaysAdopt", () => {
                return new[] {$"{Config.AlwaysAdopt}"};
            });
            cpapi.RegisterToken(this.ModManifest, "DTF", () => {
                return new[] {$"{Config.DTF}"};
            });
            cpapi.RegisterToken(this.ModManifest, "SVRThreeForest", () => {
                return new[] {$"{ModEntry.CompatSVR3Forest}"};
            });
            this.Monitor.Log($"Registered Content Patcher tokens for config options",
                    LogLevel.Trace);

            var cmapi = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(
                    "spacechase0.GenericModConfigMenu");
            if (cmapi != null) {
                cmapi.Register(
                    mod: this.ModManifest,
                    reset: () => ModEntry.Config = new ModConfig(),
                    save: () => this.Helper.WriteConfig(ModEntry.Config)
                );
                cmapi.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "AlwaysAdopt",
                    tooltip: () => this.Helper.Translation.Get("gmcm.alwaysadopt.tooltip"),
                    getValue: () => ModEntry.Config.AlwaysAdopt,
                    setValue: value => ModEntry.Config.AlwaysAdopt = value
                );
                cmapi.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "DTF",
                    tooltip: () => this.Helper.Translation.Get("gmcm.dtf.tooltip"),
                    getValue: () => ModEntry.Config.DTF,
                    setValue: value => ModEntry.Config.DTF = value
                );
                this.Monitor.Log($"Registered Generic Mod Config Menu entries",
                        LogLevel.Trace);
            }

            Dictionary<string, string> songs = new Dictionary<string, string>(){
                    {"HML_Confession", "Confession.ogg"},
                    {"HML_Lonely", "Lonely.ogg"},
                    {"HML_Upbeat", "Upbeat.ogg"},
            };
            Thread t = new Thread((ThreadStart)delegate {
                var l = new LCMusicLoader();
                foreach (var song in songs) {
                    var path = Path.Combine(this.Helper.DirectoryPath, "assets", song.Value);
                    l.LoadOggSong(song.Key, path);
                }
            });
            t.Start();
        }

        /*
         * Clear out savefile-specific cached data.
         * Stop the event background ticker, just in case it's running.
         */
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            LCModData.ClearCache();
            LCEventCommands.stopTicker();
        }

        /*
         * Early in the save load (before maps are loaded, in particular),
         * check config values from other mods and set Content Patcher tokens
         * to reflect them, in order to prevent users from having to manually
         * keep configs in sync (annoying and error-prone).
         *
         * Used for:
         *   Stardew Valley Reimagined 3 (forest map edit is a config setting)
         *
         *
         * Later in the save load, check whether we need to run the map repair
         * function, and run it if we do. In this case, we also immediately
         * rebuild Lacey's schedule, so her pathing will be correct right away
         * on the modified map.
         */
        private void OnLoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == LoadStage.CreatedBasicInfo ||
                    e.NewStage == LoadStage.SaveLoadedBasicInfo) {
                try {
                    var modInfo = HELPER.ModRegistry.Get("DaisyNiko.SVR3");
                    var modPath = (string)modInfo.GetType().GetProperty("DirectoryPath")
                        .GetValue(modInfo);
                    var jConfig = JObject.Parse(File.ReadAllText(Path.Combine(modPath, "config.json")));
                    var forest = jConfig.GetValue("Forest").Value<string>();
                    ModEntry.CompatSVR3Forest = (forest == "on");
                }
                catch (Exception ex) {
                    ModEntry.CompatSVR3Forest = false;
                    MONITOR.Log($"Caught {ex.GetType().Name} when mirroring SVR3\n{ex}",
                            LogLevel.Trace);
                }
            }
            if (e.NewStage == LoadStage.Preloaded) {
                /* check for specific terrain features that should be gone */
                GameLocation forest = Game1.getLocationFromName("Forest");
                if (forest != null) {
                    bool doClean = false;
                    if (forest.terrainFeatures.ContainsKey(new Vector2(29, 97))) {
                        doClean = true;
                    }
                    if (!doClean) {
                        foreach (var feature in forest.largeTerrainFeatures) {
                            Vector2 pos = feature.tilePosition.Value;
                            if (pos.X == 29 && pos.Y == 96) {
                                doClean = true;
                                break;
                            }
                        }
                    }
                    if (doClean) {
                        LaceyMapRepair("", null);
                        NPC Lacey = Game1.getCharacterFromName(LCInternalName);
                        Lacey.Schedule = Lacey.getSchedule(Game1.dayOfMonth);
                        Lacey.checkSchedule(Game1.timeOfDay);
                    }
                }
            }
        }

        /* Unused since CP handles this
        private void NewMapHandler(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == LoadStage.SaveAddedLocations ||
                    e.NewStage == LoadStage.CreatedInitialLocations) {
                Game1.locations.Add(new GameLocation("Maps\\MouseHouse", "MouseHouse"));
            }
        }
        */

    }
}
