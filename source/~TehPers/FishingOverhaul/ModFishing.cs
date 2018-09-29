using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TehPers.Stardew.FishingOverhaul.Configs;
using TehPers.Stardew.Framework;
using SObject = StardewValley.Object;

namespace TehPers.Stardew.FishingOverhaul {

    /// <inheritdoc />
    /// <summary>The mod entry point.</summary>
    public class ModFishing : Mod {
        public const bool DEBUG = false;
        public static ModFishing INSTANCE;

        public ConfigMain Config;
        public ConfigTreasure TreasureConfig;
        public ConfigFish FishConfig;
        public ConfigStrings Strings;

        private readonly Dictionary<SObject, float> _lastDurability = new Dictionary<SObject, float>();

        public ModFishing() {
            ModFishing.INSTANCE = this;
        }

        /// <inheritdoc />
        /// <summary>Initialize the mod.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory, such as read/writing a config file or custom JSON files.</param>
        public override void Entry(IModHelper helper) {
            // Load configs
            this.Config = helper.ReadConfig<ConfigMain>();
            this.TreasureConfig = helper.ReadJsonFile<ConfigTreasure>("treasure.json") ?? new ConfigTreasure();
            this.FishConfig = helper.ReadJsonFile<ConfigFish>("fish.json") ?? new ConfigFish();

            // Make sure the extra configs are generated
            helper.WriteJsonFile("treasure.json", this.TreasureConfig);
            helper.WriteJsonFile("fish.json", this.FishConfig);

            this.Config.AdditionalLootChance = (float) Math.Min(this.Config.AdditionalLootChance, 0.99);
            helper.WriteConfig(this.Config);

            this.OnLanguageChange(LocalizedContentManager.CurrentLanguageCode);

            // Stop here if the mod is disabled
            if (!this.Config.ModEnabled) return;

            // Events
            GameEvents.UpdateTick += this.UpdateTick;
            ControlEvents.KeyPressed += this.KeyPressed;
            LocalizedContentManager.OnLanguageChange += this.OnLanguageChange;
        }

        #region Events
        private void UpdateTick(object sender, EventArgs e) {
            // Auto-populate the fish config file if it's empty
            if (this.FishConfig.PossibleFish == null) {
                this.FishConfig.PopulateData();
                this.Helper.WriteJsonFile("fish.json", this.FishConfig);
            }

            this.TryChangeFishingTreasure();

            if (Game1.player.CurrentTool is FishingRod rod) {
                SObject bobber = rod.attachments[1];
                if (bobber != null) {
                    if (this._lastDurability.ContainsKey(bobber)) {
                        float last = this._lastDurability[bobber];
                        bobber.scale.Y = last + (bobber.scale.Y - last) * this.Config.TackleDestroyRate;
                        if (bobber.scale.Y <= 0) {
                            this._lastDurability.Remove(bobber);
                            rod.attachments[1] = null;
                            try {
                                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:FishingRod.cs.14086"));
                            } catch (Exception) {
                                Game1.showGlobalMessage("Your tackle broke!");
                                this.Monitor.Log("Could not load string for broken tackle", LogLevel.Warn);
                            }
                        } else
                            this._lastDurability[bobber] = bobber.scale.Y;
                    } else
                        this._lastDurability[bobber] = bobber.scale.Y;
                }
            }
        }

        private void KeyPressed(object sender, EventArgsKeyPressed e) {
            if (!Enum.TryParse(this.Config.GetFishInWaterKey, out Keys getFishKey) || e.KeyPressed != getFishKey)
                return;
            if (Game1.currentLocation == null)
                return;

            int[] possibleFish;
            if (Game1.currentLocation is MineShaft m)
                possibleFish = FishHelper.GetPossibleFish(5, m.mineLevel).Select(f => f.Key).ToArray();
            else
                possibleFish = FishHelper.GetPossibleFish(5).Select(f => f.Key).ToArray();

            Dictionary<int, string> fish = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            string[] fishByName = (
                from id in possibleFish
                let data = fish[id].Split('/')
                select data.Length > 13 ? data[13] : data[0]
            ).ToArray();

            Game1.showGlobalMessage(fishByName.Length == 0 ? this.Strings.NoPossibleFish : string.Format(this.Strings.PossibleFish, string.Join<string>(", ", fishByName)));
        }

        private void OnLanguageChange(LocalizedContentManager.LanguageCode code) {
            //Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Translations"));
            this.Strings = this.Helper.ReadJsonFile<ConfigStrings>("Translations/" + Helpers.GetLanguageCode() + ".json") ?? new ConfigStrings();
            this.Helper.WriteJsonFile("Translations/" + Helpers.GetLanguageCode() + ".json", this.Strings);
        }
        #endregion

        private void TryChangeFishingTreasure() {
            if (!(Game1.player.CurrentTool is FishingRod rod))
                return;

            // Look through all animated sprites in the main game
            if (this.Config.OverrideFishing) {
                foreach (TemporaryAnimatedSprite anim in Game1.screenOverlayTempSprites) {
                    if (anim.endFunction != rod.startMinigameEndFunction)
                        continue;

                    this.Monitor.Log("Overriding bobber bar", LogLevel.Trace);
                    anim.endFunction = (i => FishingRodOverrides.StartMinigameEndFunction(rod, i));
                }
            }

            // Look through all animated sprites in the fishing rod
            if (!this.Config.OverrideTreasureLoot)
                return;

            //HashSet<TemporaryAnimatedSprite> toRemove = new HashSet<TemporaryAnimatedSprite>();
            foreach (TemporaryAnimatedSprite anim in rod.animations) {
                if (anim.endFunction == rod.openTreasureMenuEndFunction) {
                    this.Monitor.Log("Overriding treasure animation end function", LogLevel.Trace);
                    anim.endFunction = (i => FishingRodOverrides.OpenTreasureMenuEndFunction(rod, i));
                } else if (false && anim.endFunction == rod.playerCaughtFishEndFunction) {
#pragma warning disable
                    /*double fishChance = config.FishBaseChance + Game1.player.FishingLevel * config.FishLevelEffect + Game1.dailyLuck * config.FishDailyLuckEffect + Game1.player.LuckLevel * config.FishLuckLevelEffect + FishHelper.getStreak(Game1.player) * config.FishStreakEffect;
                        if (FishHelper.isTrash(anim.extraInfoForEndBehavior) && Game1.random.NextDouble() < config.FishBaseChance) {
                            // Remove the catching animation
                            anim.endFunction = (extra) => { };
                            toRemove.Add(anim);
                            Game1.player.completelyStopAnimatingOrDoingAction();

                            // Undo all the stuff pullFishFromWater does
                            Game1.player.gainExperience(1, -1);

                            // Add the *HIT* animation and whatnot
                            rod.hit = true;
                            List<TemporaryAnimatedSprite> overlayTempSprites = Game1.screenOverlayTempSprites;
                            TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite(Game1.mouseCursors, new Rectangle(612, 1913, 74, 30), 1500f, 1, 0, Game1.GlobalToLocal(Game1.viewport, Helper.Reflection.GetPrivateValue<Vector2>(rod, "bobber") + new Vector2(-140f, (float) (-Game1.tileSize * 5 / 2))), false, false, 1f, 0.005f, Color.White, 4f, 0.075f, 0.0f, 0.0f, true);
                            temporaryAnimatedSprite.scaleChangeChange = -0.005f;
                            Vector2 vector2 = new Vector2(0.0f, -0.1f);
                            temporaryAnimatedSprite.motion = vector2;
                            TemporaryAnimatedSprite.endBehavior endBehavior = new TemporaryAnimatedSprite.endBehavior(rod.startMinigameEndFunction);
                            temporaryAnimatedSprite.endFunction = endBehavior;
                            int parentSheetIndex = FishHelper.getRandomFish(Helper.Reflection.GetPrivateValue<int>(rod, "clearWaterDistance")); // This doesn't matter, it gets overridden anyway
                            temporaryAnimatedSprite.extraInfoForEndBehavior = parentSheetIndex;
                            overlayTempSprites.Add(temporaryAnimatedSprite);
                            Game1.playSound("FishHit");
                        }*/
#pragma warning restore
                }
            }

            //rod.animations.RemoveAll(anim => toRemove.Contains(anim));
        }

        #region Fish Data Generator
        public void GenerateWeightedFishData(string path) {
            IEnumerable<FishInfo> fishList = from fishInfo in this.Config.PossibleFish
                                             let loc = fishInfo.Key
                                             from entry in fishInfo.Value
                                             let fish = entry.Key
                                             let data = entry.Value
                                             let seasons = data.Season
                                             let chance = data.Chance
                                             select new FishInfo {
                                                 Seasons = seasons,
                                                 Location = loc,
                                                 Fish = fish,
                                                 Chance = chance
                                             };

            Dictionary<string, Dictionary<string, Dictionary<int, double>>> result = new Dictionary<string, Dictionary<string, Dictionary<int, double>>>();

            // Spring
            Season s = Season.SPRING;
            string str = "spring";
            result[str] = new Dictionary<string, Dictionary<int, double>>();
            IEnumerable<FishInfo> seasonalFish = fishList.Where(info => (info.Seasons & s) > 0);
            foreach (string loc in seasonalFish.Select(info => info.Location).ToHashSet()) {
                IEnumerable<FishInfo> locFish = seasonalFish.Where(fish => fish.Location == loc);
                result[str][loc] = locFish.ToDictionary(fish => fish.Fish, fish => fish.Chance);
            }

            // Summer
            s = Season.SUMMER;
            str = "summer";
            result[str] = new Dictionary<string, Dictionary<int, double>>();
            seasonalFish = fishList.Where(info => (info.Seasons & s) > 0);
            foreach (string loc in seasonalFish.Select(info => info.Location).ToHashSet()) {
                IEnumerable<FishInfo> locFish = seasonalFish.Where(fish => fish.Location == loc);
                result[str][loc] = locFish.ToDictionary(fish => fish.Fish, fish => fish.Chance);
            }

            // Fall
            s = Season.FALL;
            str = "fall";
            result[str] = new Dictionary<string, Dictionary<int, double>>();
            seasonalFish = fishList.Where(info => (info.Seasons & s) > 0);
            foreach (string loc in seasonalFish.Select(info => info.Location).ToHashSet()) {
                IEnumerable<FishInfo> locFish = seasonalFish.Where(fish => fish.Location == loc);
                result[str][loc] = locFish.ToDictionary(fish => fish.Fish, fish => fish.Chance);
            }

            // Winter
            s = Season.WINTER;
            str = "winter";
            result[str] = new Dictionary<string, Dictionary<int, double>>();
            seasonalFish = fishList.Where(info => (info.Seasons & s) > 0);
            foreach (string loc in seasonalFish.Select(info => info.Location).ToHashSet()) {
                IEnumerable<FishInfo> locFish = seasonalFish.Where(fish => fish.Location == loc);
                result[str][loc] = locFish.ToDictionary(fish => fish.Fish, fish => fish.Chance);
            }

            this.Helper.WriteJsonFile("path", result);
        }

        private class FishInfo {
            public Season Seasons { get; set; }
            public string Location { get; set; }
            public double Chance { get; set; }
            public int Fish { get; set; }
        }
        #endregion
    }
}