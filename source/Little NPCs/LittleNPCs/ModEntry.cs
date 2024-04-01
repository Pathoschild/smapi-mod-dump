/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/LittleNPCs
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.GameData.Characters;

using LittleNPCs.Framework;


namespace LittleNPCs {
    public class ModEntry : Mod {
        public static IModHelper helper_;

        public static IMonitor monitor_;

        public static ModConfig config_;

        private int? relativeSeconds_;

        // We have to keep track of LittleNPCs vor various reasons.
        public static Dictionary<LittleNPC, Child> TrackedLittleNPCs { get; } = new Dictionary<LittleNPC, Child>();

        public override void Entry(IModHelper helper) {
            ModEntry.helper_ = helper;
            ModEntry.monitor_ = this.Monitor;

            // Check for LittleNPC content packs. This is quite heavy but the only way I know so far:
            // We have to check for ContentPatcher packs that depend on LittleNPCs.
            var contentPatcherPacks = from entry in helper.ModRegistry.GetAll()
                                      where entry.IsContentPack && entry.Manifest.ContentPackFor.UniqueID == "Pathoschild.ContentPatcher"
                                      select entry.Manifest;

            var littleNPCPacks = from pack in contentPatcherPacks
                                 from dependency in pack.Dependencies
                                 where dependency.UniqueID == "Candidus42.LittleNPCs"
                                 select pack.UniqueID;

            if (!littleNPCPacks.Any()) {
                this.Monitor.Log("Could not find a content pack for LittleNPCs. Your LittleNPCs will look like mere toddlers and don't do much.", LogLevel.Error);
            }

            foreach (var pack in littleNPCPacks) {
                this.Monitor.Log($"Found content pack for LittleNPCs: {pack}", LogLevel.Info);
            }

            // Read config.
            config_ = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Content.AssetRequested += OnAssetRequested;
            helper.Events.GameLoop.OneSecondUpdateTicking += OnOneSecondUpdateTicking;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;

            HarmonyPatcher.Create(this);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            ContentPatcherTokens.Register(this);
            
            // GenericModConfigMenu support.
            var configMenu = this.Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) {
                return;
            }
            
            configMenu.Register(this.ModManifest,
                                () => config_ = new ModConfig(),
                                () => this.Helper.WriteConfig(config_));
            
            configMenu.AddNumberOption(this.ModManifest,
                                       () => config_.AgeWhenKidsAreModified,
                                       (val) => config_.AgeWhenKidsAreModified = val,
                                       () => "Age when kids are modified",
                                       min: 1);
            
            configMenu.AddBoolOption(this.ModManifest,
                                     () => config_.DoChildrenWander,
                                     (val) => config_.DoChildrenWander = val,
                                     () => "Do children wander");
            
            configMenu.AddBoolOption(this.ModManifest,
                                     () => config_.DoChildrenHaveCurfew,
                                     (val) => config_.DoChildrenHaveCurfew = val,
                                     () => "Do children have curfew");
            
            configMenu.AddNumberOption(this.ModManifest,
                                       () => config_.CurfewTime,
                                       (val) => config_.CurfewTime = val,
                                       () => "Curfew time",
                                       min: 1200,
                                       max: 2400,
                                       interval: 100);
            
            configMenu.AddBoolOption(this.ModManifest,
                                     () => config_.DoChildrenVisitVolcanoIsland,
                                     (val) => config_.DoChildrenVisitVolcanoIsland = val,
                                     () => "Do children visit Volcano Island");
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            // ATTENTION: OnDayStarted() is too early for child conversion, not all assets are loaded yet.
            // We have to use OnOneSecondUpdateTicking() at 60 ticks after OnDayStarted() instead.
            // The only thing we can do here is putting all children about to convert into bed.
            var farmHouse = Utility.getHomeOfFarmer(Game1.player);
            var convertibleChildren = farmHouse.getChildren().Where(c => c.daysOld.Value >= config_.AgeWhenKidsAreModified);
            if (convertibleChildren.Count() > 2) {
                this.Monitor.Log("There are more than two children, only first and second child will be converted.", LogLevel.Info);
            }

            // Put first and second child about to convert into bed.
            foreach (var child in convertibleChildren) {
                if (IsValidLittleNPCIndex(child.GetChildIndex())) {
                    child.setTilePosition(farmHouse.GetChildBedSpot(child.GetChildIndex()));
                    // Set the original child invisible during the day.
                    child.IsInvisible = true;
                    child.HideShadow = true;
                }
            }

            // Set the counter for OnOneSecondUpdateTicking().
            relativeSeconds_ = 0;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e) {
            ProvideFallbackAssets(e, 0);
            ProvideFallbackAssets(e, 1);
        }

        private void OnOneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs e) {
            // Run only once per day at 60 ticks after OnDayStarted().
            if (!relativeSeconds_.HasValue || ++relativeSeconds_ != 1) {
                return;
            }

            if (TrackedLittleNPCs.Any()) {
                foreach (var npc in TrackedLittleNPCs) {
                    this.Monitor.Log($"{nameof(TrackedLittleNPCs)} still contains {npc.Key.Name}.", LogLevel.Warn);
                }
                this.Monitor.Log($"{nameof(TrackedLittleNPCs)} is not empty, clearing it.", LogLevel.Error);
                TrackedLittleNPCs.Clear();
            }

            var farmHouse = Utility.getHomeOfFarmer(Game1.player);

            var convertibleChildren = farmHouse.getChildren().Where(c => c.daysOld.Value >= config_.AgeWhenKidsAreModified);

            var npcs = farmHouse.characters;
            
            var childrenToConvert = new List<Child>();
            foreach (var child in convertibleChildren) {
                // Convert only the first two children.
                if (IsValidLittleNPCIndex(child.GetChildIndex())) {
                    childrenToConvert.Add(child);
                }
                else {
                    this.Monitor.Log($"Skipping child {child.Name}.", LogLevel.Info);
                }
            }
            foreach (var child in childrenToConvert) {
                var littleNPC = LittleNPC.FromChild(child, farmHouse, this.Monitor);
                    // Replace Child by LittleNPC object.
                    npcs.Add(littleNPC);

                    // Copy friendship data.
                    if (Game1.player.friendshipData.TryGetValue(child.Name, out var friendship)) {
                        Game1.player.friendshipData[littleNPC.Name] = friendship;
                        // Removing friendship data removes the child from the social page which is exactly whet we want.
                        Game1.player.friendshipData.Remove(child.Name);
                    }

                    // Add to tracking list.
                    TrackedLittleNPCs[littleNPC] = child;

                    this.Monitor.Log($"Added LittleNPC {littleNPC.Name}, deactivated child {child.Name}.", LogLevel.Info);
            }

            if (config_.DoChildrenVisitVolcanoIsland) {
                // Add random island schedule.
                AddRandomIslandSchedule(TrackedLittleNPCs.Keys.ToList());
            }
        }

        private void OnSaving(object sender, SavingEventArgs e) {
            var npcDispositions = Game1.content.Load<Dictionary<string, CharacterData>>("Data/Characters");

            // Only convert items in our tracking list.
            foreach (var item in TrackedLittleNPCs) {
                var littleNPC = item.Key;
                var child = item.Value;

                this.Monitor.Log($"ConvertLittleNPCsToChildren: {littleNPC.Name}", LogLevel.Info);
                    
                // Put hat on (part of the save game).
                if (littleNPC.WrappedChildHat is not null) {
                    child.hat.Value = littleNPC.WrappedChildHat;
                }

                // Copy friendship data.
                if (Game1.player.friendshipData.TryGetValue(littleNPC.Name, out var friendship)) {
                    Game1.player.friendshipData[child.Name] = friendship;
                }

                // Set child visible before saving.
                child.IsInvisible = false;

                // Remove NPCDispositions to prevent auto-load on next day.
                npcDispositions.Remove(littleNPC.Name);

                // Remove from game.
                bool success = false;
                Utility.ForEachLocation(location => {
                    for (int i = 0; i < location.characters.Count; ++i) {
                        if (location.characters[i].Name == littleNPC.Name) {
                            this.Monitor.Log($"Removed LittleNPC {littleNPC.Name} in {littleNPC.currentLocation.Name}, reactivated child {child.Name}.", LogLevel.Info);
                            location.characters.RemoveAt(i);
                            success = true;
                        
                            break;
                        }
                    }
                
                    return true;
                });

                if (!success) {
                    this.Monitor.Log($"Failed to remove LittleNPC {littleNPC.Name} from tracking list.", LogLevel.Error);
                }
            }

            // Clear tracking list.
            TrackedLittleNPCs.Clear();

            relativeSeconds_ = null;
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e) {
            // Forward the call.
            OnSaving(sender, null);
        }

        /// <summary>
        /// Add island schedule randomly.
        /// </summary>
        /// <param name="littleNPCs"></param>
        private void AddRandomIslandSchedule(List<LittleNPC> littleNPCs) {
            // ATTENTION: CustomNPCExclusions patches the very same methods we'd have to patch,
            // IslandSouth.CanVisitIslandToday() and IslandSouth.SetupIslandSchedules() in a conflicting way.
            // To avoid that we just copied the important parts from IslandSouth.SetupIslandSchedules().
            if (Utility.isFestivalDay(Game1.Date.DayOfMonth, Game1.Date.Season)
             || (Game1.Date.Season == Season.Winter && Game1.Date.DayOfMonth >= 15 && Game1.Date.DayOfMonth <= 17)) {
                return;
            }
            IslandSouth islandSouth = Game1.getLocationFromName("IslandSouth") as IslandSouth;
            if (islandSouth is null || !islandSouth.resortRestored.Value || Game1.IsRainingHere(islandSouth) || !islandSouth.resortOpenToday.Value) {
                return;
            }
            
            var islandActivityAssignments = new List<IslandSouth.IslandActivityAssigments>();
            var last_activity_assignments = new Dictionary<Character, string>();
            var random = new Random((int) ((float) Game1.uniqueIDForThisGame * 1.21f) + (int) ((float) Game1.stats.DaysPlayed * 2.5f));

            var npcs = littleNPCs.Cast<NPC>().ToList();
            islandActivityAssignments.Add(new IslandSouth.IslandActivityAssigments(1200, npcs, random, last_activity_assignments));
            islandActivityAssignments.Add(new IslandSouth.IslandActivityAssigments(1400, npcs, random, last_activity_assignments));
            islandActivityAssignments.Add(new IslandSouth.IslandActivityAssigments(1600, npcs, random, last_activity_assignments));
            last_activity_assignments = null;

            foreach (NPC npc in npcs) {
                if (random.NextDouble() < 0.4) {
                    StringBuilder sb = new StringBuilder();
                    bool hasIslandAttire = IslandSouth.HasIslandAttire(npc);

                    if (hasIslandAttire) {
                        Point dressingRoomPoint = IslandSouth.GetDressingRoomPoint(npc);
                        sb.Append($"/a1150 IslandSouth {dressingRoomPoint.X} {dressingRoomPoint.Y} change_beach");
                        
                        foreach (IslandSouth.IslandActivityAssigments activity in islandActivityAssignments) {
                            string text = activity.GetScheduleStringForCharacter(npc);
                            if (!string.IsNullOrEmpty(text)) {
                                sb.Append(text);
                            }
                        }
                       
                        Point dressingRoomPoint2 = IslandSouth.GetDressingRoomPoint(npc);
                        sb.Append($"/a1730 IslandSouth {dressingRoomPoint2.X} {dressingRoomPoint2.Y} change_normal");
                        
                    }
                    else {
                        bool endActivity = false;
                        foreach (IslandSouth.IslandActivityAssigments activity in islandActivityAssignments) {
                            string text = activity.GetScheduleStringForCharacter(npc);
                            if (!string.IsNullOrEmpty(text)) {
                                if (!endActivity) {
                                    text = $"/a{text.Substring(1)}";
                                    endActivity = true;
                                }
                                sb.Append(text);
                            }
                        }
                    }

                    sb.Append("/1800 bed");

                    sb.Remove(0, 1);
                    if (npc.TryLoadSchedule("island", sb.ToString())) {
                        npc.islandScheduleName.Value = "island";
                        Game1.netWorldState.Value.IslandVisitors.Add(npc.Name);
                    }

                    this.Monitor.Log($"{npc.Name} will visit Volcano Island today.", StardewModdingAPI.LogLevel.Info);
                }
            }
        }

        /// <summary>
        /// Provides fallback assets from game content.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="index"></param>
        private void ProvideFallbackAssets(AssetRequestedEventArgs e, int index) {
            var littleNPC = new LittleNPCInfo(index, this.Monitor);

            // We also use the sprite texture as portrait but should be good enough as a fallback.
            string spriteTextureName = string.Concat("Characters/Toddler",
                                                     (littleNPC.Gender == Gender.Male) ? "" : "_girl");

            // Fallback dialogue.
            string message = Game1.IsMultiplayer
                           ? string.Concat("Hi @! Don't worry that I'm still a toddler, ",
                                           "multiplayer support is not easy to implement.",
                                           "#$e#",
                                           "This is work in progress.")
                           : string.Concat("Hi dad! Please install a content pack for me.",
                                           "^Hi mom! Please install a content pack for me.",
                                           "#$e#",
                                           "Look for StardewValley Mod 15152 on nexusmods.com for details.");

            var dialogue = new Dictionary<string, string>() {
                { "Mon", message },
                { "Tue", message },
                { "Wed", message },
                { "Thu", message },
                { "Fri", message },
                { "Sat", message },
                { "Sun", message }
            };

            string prefix = index == 0 ? "FirstLittleNPC" : "SecondLittleNPC";

            if (e.NameWithoutLocale.StartsWith($"Characters/{prefix}")) {
                // Fallback assets are loaded with low prioriy.
                e.LoadFrom(() => Game1.content.Load<Texture2D>(spriteTextureName), AssetLoadPriority.Low);
            }

            if (e.NameWithoutLocale.StartsWith($"Portraits/{prefix}")) {
                // This uses part of the sprite texture as portrait but should be good enough as a fallback.
                e.LoadFrom(() => Game1.content.Load<Texture2D>(spriteTextureName), AssetLoadPriority.Low);
            }

            if (e.NameWithoutLocale.StartsWith($"Characters/Dialogue/{prefix}")) {
                e.LoadFrom(() => dialogue, AssetLoadPriority.Low);
            }
        }

        /// <summary>
        /// Gets a LittleNPC by child index.
        /// </summary>
        /// <param name="childIndex"></param>
        /// <returns></returns>
        internal static LittleNPC GetLittleNPC(int childIndex) {
            // The list of LittleNPCs is not sorted by child index, thus we need a query.
            return TrackedLittleNPCs.Keys.FirstOrDefault(c => c.ChildIndex == childIndex);
        }

        /// <summary>
        /// Checks if child index is a valid LittleNPC index.
        /// </summary>
        /// <param name="childIndex"></param>
        /// <returns></returns>
        internal static bool IsValidLittleNPCIndex(int childIndex) {
            // Only the first two children can be converted.
            return (childIndex == 0 || childIndex == 1);
        }
    }
}
