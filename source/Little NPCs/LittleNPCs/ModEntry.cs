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

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;

using LittleNPCs.Framework;


namespace LittleNPCs {
    public class ModEntry : Mod {
        public static IModHelper helper_;

        public static ModConfig config_;

        private int? relativeSeconds_;

        // We have to keep track of LittleNPCs vor various reasons.
        public static List<LittleNPC> LittleNPCsList { get; } = new List<LittleNPC>();

        public override void Entry(IModHelper helper) {
            ModEntry.helper_ = helper;

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
                throw new InvalidOperationException("Could not find a content pack for LittleNPCs");
            }

            foreach (var pack in littleNPCPacks) {
                this.Monitor.Log($"Found content pack for LittleNPCs: {pack}", LogLevel.Info);
            }

            // Read config.
            config_ = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
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

        private void OnOneSecondUpdateTicking(object sender, OneSecondUpdateTickingEventArgs e) {
            // Run only once per day at 60 ticks after OnDayStarted().
            if (!relativeSeconds_.HasValue || ++relativeSeconds_ != 1) {
                return;
            }

            if (LittleNPCsList.Any()) {
                this.Monitor.Log($"{nameof(LittleNPCsList)} is not empty, clearing it.", LogLevel.Error);
                LittleNPCsList.Clear();
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
                    LittleNPCsList.Add(littleNPC);

                    this.Monitor.Log($"Added LittleNPC {littleNPC.Name}, deactivated child {child.Name}.", LogLevel.Info);
            }

            if (config_.DoChildrenVisitVolcanoIsland) {
                // Add random island schedule.
                AddRandomIslandSchedule(LittleNPCsList);
            }
        }

        private void OnSaving(object sender, SavingEventArgs e) {
            var npcDispositions = Game1.content.Load<Dictionary<string, string>>("Data/NPCDispositions");

            // Local function, only needed here.
            void ConvertLittleNPCsToChildren(Netcode.NetCollection<NPC> npcs) {
                var littleNPCsToConvert = npcs.OfType<LittleNPC>().ToList();
                foreach (var littleNPC in littleNPCsToConvert) {
                    var child = littleNPC.WrappedChild;
                    // Put hat on (part of the save game).
                    if (littleNPC.WrappedChildHat is not null) {
                        child.hat.Value = littleNPC.WrappedChildHat;
                    }

                    // Replace LittleNPC by Child object.
                    npcs.Remove(littleNPC);

                    // Copy friendship data.
                    if (Game1.player.friendshipData.TryGetValue(littleNPC.Name, out var friendship)) {
                        Game1.player.friendshipData[child.Name] = friendship;
                    }

                    // Set child visible before saving.
                    child.IsInvisible = false;

                    // Remove NPCDispositions to prevent auto-load on next day.
                    npcDispositions.Remove(littleNPC.Name);

                    // Remove from tracking list.
                    LittleNPCsList.Remove(littleNPC);

                    this.Monitor.Log($"Removed LittleNPC {littleNPC.Name} in {littleNPC.currentLocation.Name}, reactivated child {child.Name}.", LogLevel.Info);
                }
            }

            // ATTENTION: Avoid Utility.getAllCharacters(), replacing elements in the returned list doesn't work.
            // We have to iterate over all locations instead.

            // Check outdoor locations and convert LittleNPCs back if necessary.
            foreach (GameLocation location in Game1.locations) {
                // Plain old for-loop because we have to replace list elements.
                var npcs = location.characters;
                ConvertLittleNPCsToChildren(npcs);
            }

            // Check indoor locations and convert LittleNPCs back if necessary.
            foreach (BuildableGameLocation location in Game1.locations.OfType<BuildableGameLocation>()) {
                foreach (Building building in location.buildings) {
                    if (building.indoors.Value is not null) {
                        var npcs = building.indoors.Value.characters;
                        ConvertLittleNPCsToChildren(npcs);
                    }
                }
            }

            if (LittleNPCsList.Any()) {
                this.Monitor.Log($"{nameof(LittleNPCsList)} is not empty, clearing it.", LogLevel.Error);
                LittleNPCsList.Clear();
            }

            relativeSeconds_ = null;
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e) {
            // Clear state before returning to title.
            LittleNPCsList.Clear();

            relativeSeconds_ = null;
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
             || (Game1.Date.Season == "winter" && Game1.Date.DayOfMonth >= 15 && Game1.Date.DayOfMonth <= 17)) {
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
                    npc.islandScheduleName.Value = "island";
                    npc.Schedule = npc.parseMasterSchedule(sb.ToString());
                    Game1.netWorldState.Value.IslandVisitors[npc.Name] = true;

                    this.Monitor.Log($"{npc.Name} will visit Volcano Island today.", StardewModdingAPI.LogLevel.Info);
                }
            }
        }

        /// <summary>
        /// Gets a LittleNPC by child index.
        /// </summary>
        /// <param name="childIndex"></param>
        /// <returns></returns>
        internal static LittleNPC GetLittleNPC(int childIndex) {
            // The list of LittleNPCs is not sorted by child index, thus we need a query.
            return LittleNPCsList.FirstOrDefault(c => c.ChildIndex == childIndex);
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
