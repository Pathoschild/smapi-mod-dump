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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using StardewModdingAPI.Utilities;

using StardewValley;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Objects;


namespace LittleNPCs.Framework {
    public class LittleNPC : NPC {
        private IMonitor monitor_;

        private static Random random_ = new Random(Game1.Date.TotalDays + (int) Game1.uniqueIDForThisGame / 2 + (int) Game1.MasterPlayer.UniqueMultiplayerID * 2);

        // Check that NPCParseMasterSchedulePatch executed.
        internal bool ParseMasterSchedulePatchExecuted { get; set; }

        /// <summary>
        /// Wrapped child object. Required to replace the corresponding LittleNPC object on save.
        /// </summary>
        /// <value></value>
        public Child WrappedChild { get; private set; }
        
        /// <summary>
        /// Wrapped child's hat, if any. Must be removed during the day.
        /// </summary>
        /// <value></value>
        public Hat WrappedChildHat { get; private set; }

        /// <summary>
        /// Cached child index. The method <code>Child.GetChildIndex()</code>
        /// becomes useless after removing any child object so we must cache this value.
        /// </summary>
        /// <value></value>
        public int ChildIndex { get; private set; }

        protected LittleNPC(IMonitor monitor, Child child, AnimatedSprite sprite, Vector2 position, string defaultMap, int facingDir, string name, string displayName, Dictionary<int, int[]> schedule, Texture2D portrait, bool eventActor)
        : base(sprite, position, defaultMap, facingDir, name, schedule, portrait, eventActor, null) {
            monitor_ = monitor;
            WrappedChild = child;
            // Take hat off because it stays visible even when making a child invisible.
            if (WrappedChild.hat.Value is not null) {
                WrappedChildHat = WrappedChild.hat.Value;
                WrappedChild.hat.Value = null;
            }

            ChildIndex = child.GetChildIndex();

            // Set birthday.
            var birthday = GetBirthday();
            Birthday_Day = birthday.Day;
            Birthday_Season = birthday.Season;

            // Set gender.
            Gender = child.Gender;

            // Set displayName.
            this.displayName = displayName;

            // Ensure that the original child stays invisible.
            if (!WrappedChild.IsInvisible) {
                monitor_.Log($"Made child {WrappedChild.Name} invisible.", LogLevel.Info);
                WrappedChild.IsInvisible = true;
            }
        }

        public static LittleNPC FromChild(Child child, FarmHouse farmHouse, IMonitor monitor) {
            Vector2 bedSpot = Utility.PointToVector2(farmHouse.GetChildBedSpot(child.GetChildIndex())) * 64f;
            // (0, 0) means there's noe bed available and the child will stuck in the wall. We must avoid that.
            if (bedSpot == Vector2.Zero) {
                bedSpot = Utility.PointToVector2(farmHouse.getRandomOpenPointInHouse(random_, 1)) * 64f;
                monitor.Log($"No bed spot for {child.Name} found, setting it to random point {Utility.Vector2ToPoint(bedSpot / 64f)}", LogLevel.Warn);
            }

            string prefix = child.GetChildIndex() == 0 ? "FirstLittleNPC" : "SecondLittleNPC";

            var npcDispositions = Game1.content.Load<Dictionary<string, string>>("Data/NPCDispositions");

            var sprite = new AnimatedSprite($"Characters/{prefix}{child.Name}", 0, 16, 32);
            var portrait = Game1.content.Load<Texture2D>($"Portraits/{prefix}{child.Name}");
            var npc = new LittleNPC(monitor,
                                    child,
                                    sprite,
                                    bedSpot,
                                    child.DefaultMap,
                                    child.FacingDirection,
                                    $"{prefix}{child.Name}",
                                    child.Name,
                                    null,
                                    portrait,
                                    false);

            monitor.Log(string.Join(' ',
                                    $"Created LittleNPC {npc.Name}:",
                                    $"index {npc.ChildIndex},",
                                    $"bed spot {Utility.Vector2ToPoint(bedSpot / 64f)},",
                                    $"birthday {new SDate(npc.Birthday_Day, npc.Birthday_Season)}",
                                    $"({npc.WrappedChild.daysOld.Value} days ago)"), LogLevel.Info);

            // Generate and set NPCDispositions.
            // ATTENTION: Don't use CP to set Data/NPCDispositions, you will get into big trouble then.
            // If we add something to 'Data/NPCDispositions' the game attempts to create that NPC.
            // We must control NPC creation, however, so we generate and set dispositions here.
            // Fortunately all important data is provided by the save file.
            // Note that the content pack must not provide NPCDispositions.
            // Example: 
            // child/neutral/outgoing/neutral/male/non-datable/null/Town/summer 23//Farmhouse 23 5/Eric
            // child/neutral/outgoing/neutral/female/non-datable/null/Town/summer 24//Farmhouse 27 5/Sandra
            npcDispositions[npc.Name] = string.Join('/',
                                                    "child",
                                                    "neutral",
                                                    "outgoing",
                                                    "neutral",
                                                    $"{(npc.Gender == 0 ? "male": "female")}",
                                                    "non-datable",
                                                    "null",
                                                    "Town",
                                                    $"{npc.Birthday_Season} {npc.Birthday_Day}",
                                                    "",
                                                    $"{npc.DefaultMap} {(int) bedSpot.X / 64f} {(int) bedSpot.Y / 64f}",
                                                    $"{npc.displayName}");

            monitor.Log($"Created dispositions for {npc.Name}: {npcDispositions[npc.Name]}", LogLevel.Info);

            // ATTENTION: NPC.reloadData() parses dispositions and resets DefaultMap and DefaultPosition for non-married NPCs.
            // This is not a problem since we generated dispositions with matching default values beforehand.
            // We must not call this method in the constructor since it is virtual.
            npc.reloadData();

            // Reload schedule.
            npc.Schedule = npc.getSchedule(Game1.dayOfMonth);

            // Check if NPCParseMasterSchedulePatch ran.
            if (npc.ParseMasterSchedulePatchExecuted) {
                monitor.Log($"NPCParseMasterSchedulePatch executed for {npc.Name}.", LogLevel.Info);
            }
            else {
                monitor.Log($"NPCParseMasterSchedulePatch didn't execute for {npc.Name}. Schedule won't work.", LogLevel.Warn);

                // NPC's default location might have been messed up on error.
                npc.reloadDefaultLocation();

                monitor.Log($"Reset default location of {npc.Name} to {npc.DefaultMap}, {Utility.Vector2ToPoint(npc.DefaultPosition / 64f)}.", LogLevel.Warn);
            }

            return npc;
        }

        /// <inheritdoc/>
        public override void performTenMinuteUpdate(int timeOfDay, GameLocation l) {
            FarmHouse farmHouse = Utility.getHomeOfFarmer(Game1.player);
            if (farmHouse.characters.Contains(this)) {
                ModConfig config = ModEntry.config_;
                // Send children to bed when inside home.
                if (config.DoChildrenHaveCurfew && Game1.timeOfDay == config.CurfewTime) {
                    IsWalkingInSquare = false;
                    Halt();
                    temporaryController = null;

                    // Child is at home, direct path to bed (DefaultPosition).
                    Point bedPoint = new Point((int) DefaultPosition.X / 64, (int) DefaultPosition.Y / 64);
                    controller = new PathFindController(this, farmHouse, bedPoint, 2);

                    if (controller.pathToEndPoint is null || !farmHouse.isTileOnMap(controller.pathToEndPoint.Last().X, controller.pathToEndPoint.Last().Y)) {
                        controller = null;
                    }
                }
                // Make children wander if they have nothing better to do.
                // ATTENTION: We have to skip that for scheduled times, otherwise schedule and random wandering overlap in a weird way:
                // The NPCs get warped out of farm house before they reach their random destination points in the house
                // and thus are doomed to walk around in the BusStop location endlessly without a chance to reach their destination!
                else if (controller is null
                         && config.DoChildrenWander
                         && (Schedule is null || !Schedule.ContainsKey(Game1.timeOfDay))
                         && Game1.timeOfDay % 100 == 0
                         && Game1.timeOfDay < config.CurfewTime) {
                    if (!currentLocation.Equals(Utility.getHomeOfFarmer(Game1.player))) {
                        return;
                    }

                    IsWalkingInSquare = false;
                    Halt();

                    // If I'm going to prevent them from wandering into doorways, I need to do it here.
                    controller = new PathFindController(this, farmHouse, farmHouse.getRandomOpenPointInHouse(Game1.random, 0, 30), 2);
                    if (controller.pathToEndPoint is null || !farmHouse.isTileOnMap(controller.pathToEndPoint.Last().X, controller.pathToEndPoint.Last().Y)) {
                        controller = null;
                    }
                }
            }

            // Call base method.
            base.performTenMinuteUpdate(timeOfDay, l);
        }

        public SDate GetBirthday() {
            return GetBirthday(WrappedChild);
        }

        public static SDate GetBirthday(Child child) {
            SDate birthday;

            try {
                // Subtract age of child in days from current date.
                birthday = SDate.Now().AddDays(-child.daysOld.Value);
            }
            catch (ArithmeticException) {
                // Fallback.
                birthday = new SDate(1, "spring");
            }

            return birthday;
        }
    }
}
