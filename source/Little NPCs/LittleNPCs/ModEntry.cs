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

        // We have to track child indices since they change when children are removed.
        private static Dictionary<string, int> childIndexMap_ = new Dictionary<string, int>();

        // We have to keep track of LittleNPCs vor various reasons.
        public static List<LittleNPC> LittleNPCsList { get; } = new List<LittleNPC>();

        // The ChildGetChildIndexPatch must be enabled or disabled conditionally.
        internal static bool ChildGetChildIndexPatchEnabled { get; set; }

        public override void Entry(IModHelper helper) {
            ModEntry.helper_ = helper;

            // Read config.
            config_ = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;

            HarmonyPatcher.Create(this);
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            ContentPatcherTokens.Register(this);
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            // ATTENTION: OnDayStarted is too early for child conversion, not all assets are loaded yet.
            // We have to use OnTimeChanged() at 06:10 instead. The only thing we can do here is puttting
            // all children about to convert into bed.
            var farmHouse = Utility.getHomeOfFarmer(Game1.player);
            var convertibleChildren = farmHouse.getChildren().Where(c => c.daysOld.Value >= config_.AgeWhenKidsAreModified);
            convertibleChildren.ToList().ForEach(c => c.setTilePosition(farmHouse.GetChildBedSpot(c.GetChildIndex())));

            // ATTENTION: Getting child indices must be done before removing any child and doesn't depend on age.
            Assert(!childIndexMap_.Any(), $"{nameof(childIndexMap_)} is not empty");
            foreach (var c in farmHouse.getChildren()) {
                childIndexMap_.Add(c.Name, c.GetChildIndex());
                this.Monitor.Log($"Get child index for {c.Name}: {childIndexMap_[c.Name]}");
            }

            // Enabling this patch changes the semantics of Child.GetChildIndex() and must be done after filling the map.
            ChildGetChildIndexPatchEnabled = true;
            this.Monitor.Log("GetChildIndex patch enabled.");
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e) {
            // Run only once per day at 06:10 .
            // ATTENTION: This method runs at 06:00 but not on the first day after loading the save!
            if (e.NewTime != 610) {
                return;
            }

            Assert(!LittleNPCsList.Any(), $"{nameof(LittleNPCsList)} is not empty");

            var farmHouse = Utility.getHomeOfFarmer(Game1.player);

            var convertibleChildren = farmHouse.getChildren().Where(c => c.daysOld.Value >= config_.AgeWhenKidsAreModified);

            var npcs = farmHouse.characters;

            // Plain old for loop because we have to replace list elements.
            for (int i = 0; i < npcs.Count; ++i) {
                if (npcs[i] is Child child && convertibleChildren.Contains(child)) {
                    var littleNPC = LittleNPC.FromChild(child, childIndexMap_[child.Name], farmHouse, this.Monitor);
                    // Replace Child by LittleNPC object.
                    npcs[i] = littleNPC;

                    // Copy friendship data.
                    if (Game1.player.friendshipData.TryGetValue(child.Name, out var friendship)) {
                        Game1.player.friendshipData[littleNPC.Name] = friendship;
                    }

                    // Add to tracking list.
                    LittleNPCsList.Add(littleNPC);

                    this.Monitor.Log($"Replaced child {child.Name} by LittleNPC {littleNPC.Name}.", LogLevel.Info);
                }
            }
        }

        private void OnSaving(object sender, SavingEventArgs e) {
            var npcDispositions = Game1.content.Load<Dictionary<string, string>>("Data/NPCDispositions");

            // Local function, only needed here.
            void ConvertLittleNPCsToChildren(Netcode.NetCollection<NPC> npcs) {
                // Plain old for-loop because we have to replace list elements.
                for (int i = 0; i < npcs.Count; ++i) {
                    if (npcs[i] is LittleNPC littleNPC) {
                        var child = littleNPC.WrappedChild;
                        // ATTENTION: By removing children we prevent them from aging properly so we have call dayUpdate() explicitly.
                        child.dayUpdate(Game1.dayOfMonth);
                        // Replace LittleNPC by Child object.
                        npcs[i] = child;

                        // Copy friendship data.
                        if (Game1.player.friendshipData.TryGetValue(littleNPC.Name, out var friendship)) {
                            Game1.player.friendshipData[child.Name] = friendship;
                        }

                        // Remove NPCDispositions to prevent auto-load on next day.
                        npcDispositions.Remove(littleNPC.Name);

                        // Remove from tracking list.
                        LittleNPCsList.Remove(littleNPC);

                        this.Monitor.Log($"Replaced LittleNPC {littleNPC.Name} in {npcs[i].currentLocation.Name} by child {child.Name}.", LogLevel.Info);
                    }
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

            Assert(!LittleNPCsList.Any(), $"{nameof(LittleNPCsList)} is not empty");

            childIndexMap_.Clear();

            ChildGetChildIndexPatchEnabled = false;
            this.Monitor.Log("GetChildIndex patch disabled.");
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e) {
            // Clear state before returning to title.
            LittleNPCsList.Clear();

            childIndexMap_.Clear();

            ChildGetChildIndexPatchEnabled = false;
            this.Monitor.Log("GetChildIndex patch disabled.");
        }

        /// <summary>
        /// Gets the index of the given child name from the cache.
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        internal static int GetChildIndex(string childName) {
            // Return an invalid index if not found. That might cause an error
            // when happening at the wrong time but gives a hint for debugging at least.
            int retval = childIndexMap_.TryGetValue(childName, out int childIndex) ? childIndex : -1;

            return retval;
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
        /// Custom assert method because <code>Debug.Assert()</code> takes the whole application down. 
        /// </summary>
        private static void Assert(bool condition, string message) {
            if (!condition) {
                throw new InvalidOperationException(message);
            }
        }
    }
}
