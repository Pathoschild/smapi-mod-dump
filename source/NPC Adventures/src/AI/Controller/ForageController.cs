/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using Microsoft.Xna.Framework;
using NpcAdventure.Utils;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Locations;
using StardewModdingAPI.Utilities;
using PurrplingCore.Movement;

namespace NpcAdventure.AI.Controller
{
    internal class ForageController : IController
    {
        private readonly AI_StateMachine ai;
        private readonly PathFinder pathFinder;
        private readonly NpcMovementController joystick;
        private readonly List<TerrainFeature> ignoreList;
        private readonly Random r;
        private TerrainFeature targetObject;
        protected Stack<Item> foragedObjects;
        private readonly Dictionary<string, int[]> forages;

        public NPC Forager => this.ai.npc;
        public Farmer Leader => this.ai.farmer;
        public int ForagingLevel => Math.Max(this.Leader.ForagingLevel
            - (this.Leader.professions.Contains(Farmer.gatherer) ? 0 : 1), 0);

        public ForageController(AI_StateMachine ai, IModEvents events)
        {
            this.ai = ai;
            this.ignoreList = new List<TerrainFeature>();
            this.pathFinder = new PathFinder(this.Forager.currentLocation, this.Forager, this.ai.farmer);
            this.joystick = new NpcMovementController(this.Forager, this.pathFinder);
            this.joystick.EndOfRouteReached += this.Joystick_EndOfRouteReached;
            this.ai.LocationChanged += this.Ai_LocationChanged;
            this.r = new Random((int)Game1.uniqueIDForThisGame + (int)Game1.stats.DaysPlayed);
            this.foragedObjects = new Stack<Item>();
            this.forages = this.LoadForages();

            events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
        }

        private Dictionary<string, int[]> LoadForages()
        {
            try
            {
                return this.ai.Csm.ContentLoader.LoadStrings("Data/Forages").ToDictionary(f => f.Key, f => Utility.parseStringToIntArray(f.Value));
            } catch (Exception e)
            {
                this.ai.Monitor.Log($"Error while parsing forage definitions: {e}", LogLevel.Error);

                return new Dictionary<string, int[]>();
            }
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            int keepMindChanceNum = this.Leader.getFriendshipHeartLevelForNPC(this.Forager.Name) - this.foragedObjects.Count + 1;

            if (Helper.IsSpouseMarriedToFarmer(this.Forager, this.Leader)) {
                keepMindChanceNum = (int)(keepMindChanceNum * 1.5);
            }

            if (this.HasAnyForage() && this.r.Next(Math.Max(keepMindChanceNum, 2)) == 1)
            {
                // Chance 1:<count of frindship hearts> to forager changes their mind 
                // to share some foraged item with you and don't share it
                this.foragedObjects.Pop();
                this.ai.Monitor.Log($"{this.Forager.Name} changed her/his mind and don't share last forage with farmer.");
            }
        }

        private void Joystick_EndOfRouteReached(object sender, NpcMovementController.EndOfRouteReachedEventArgs e)
        {
            if (this.ai.CurrentController != this)
                return;

            if (this.targetObject != null)
            {
                this.targetObject.performUseAction(this.Forager.getTileLocation(), this.Forager.currentLocation);
                this.ignoreList.Add(this.targetObject);
            } else
            {
                this.Forager.currentLocation.localSound("leafrustle");
                this.Forager.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 1085, 58, 58), 60f, 8, 0, this.Forager.GetGrabTile() * 64, false, this.Forager.FacingDirection == 3, 1f, 0.0f, Color.White, 1f, 0.0f, 0.0f, 0.0f, false));
            }

            double potentialChance = 0.02 + 1.0 / (this.foragedObjects.Count + 1) + this.Leader.LuckLevel / 100.0 + this.Leader.DailyLuck;
            double boost = this.Leader.professions.Contains(Farmer.gatherer) ? 2.0 : 1.0;
            double realChance = potentialChance * 0.33 + (this.ForagingLevel + 1) * 0.005 * boost;
            double current = this.r.NextDouble();

            if (this.ai.Monitor.IsVerbose)
            {
                this.ai.Monitor.VerboseLog($"(Pick forage) Companion foraging level: {this.ForagingLevel} " +
                     $"| potential chance: {potentialChance} " +
                     $"| boost: {boost} " +
                     $"| real chance: {realChance} " +
                     $"| current: {current} " +
                     $"| passed: {(current < realChance ? "Yes" : "No")}");
            }

            if (current < realChance || NpcAdventureMod.DebugFlags.Contains("forager.pickAlways"))
            {
                this.PickForageObject(this.targetObject);
            }

            this.targetObject = null;
            this.IsIdle = true;
        }

        public virtual void SideUpdate(UpdateTickedEventArgs e)
        {
        }

        public void PickForageObject(TerrainFeature source)
        {
            int skill = this.ForagingLevel;
            int quality = 0;

            if (skill >= 8 && this.r.NextDouble() < .05f)
                quality = 4;
            else if (skill >= 6 && this.r.NextDouble() < 0.2f)
                quality = 2;
            else if (skill >= 2 && this.r.NextDouble() < 0.55f)
                quality = 1;

            GameLocation location = this.Forager.currentLocation;
            string locationName = location.Name;
            string season = Game1.currentSeason;
            int objectIndex = -1;

            if (source != null && source is Tree tree && tree.growthStage.Value >= Tree.treeStage)
            {
                if (season == "winter" || this.ForagingLevel < 1)
                    return;

                switch (tree.treeType.Value)
                {
                    case Tree.bushyTree:
                        objectIndex = 309;
                        break;
                    case Tree.leafyTree:
                        objectIndex = 310;
                        break;
                    case Tree.pineTree:
                        objectIndex = 311;
                        break;
                }

                if (season == "fall" && tree.treeType.Value == Tree.leafyTree && SDate.Now().Day >= 14)
                    objectIndex = 408;

                if (objectIndex != -1)
                    this.SaveForage(new SObject(objectIndex, 1, false, -1, quality));

                return;
            }

            if (source != null && source is Bush bush)
            {
                if (this.ForagingLevel > 5 && this.r.NextDouble() < 0.005)
                {
                    // There is a chance <1% to get a rare forage item
                    this.SaveForage(new SObject(this.FetchForage("rare"), 1, false, -1, 0));
                    return;
                }

                if (BushIsInBloom(bush))
                {
                    objectIndex = 296;

                    if (season == "fall")
                        objectIndex = 410;
                    if (bush.size.Value == 3)
                        objectIndex = 815;

                    this.SaveForage(new SObject(objectIndex, 1, false, -1, quality));
                    return;
                }
            }

            if (location is Farm)
            {
                var farmForages = new List<int>();

                farmForages.AddRange(this.GetForageSheet(season, "Farm"));
                farmForages.AddRange(this.GetForageSheet(season, $"farmType_{Farm.getMapNameFromTypeInt(Game1.whichFarm)}"));

                objectIndex = this.FetchForage(farmForages.Distinct().ToArray());
            }
            else if (location is MineShaft)
            {
                objectIndex = this.FetchForage("cave");
            }
            else
            {
                objectIndex = this.FetchGeneralForage(season, locationName);
            }

            if (objectIndex != -1)
                this.SaveForage(new SObject(objectIndex, 1, false, -1, quality));
        }

        private int FetchForage(string sheetName)
        {
            return this.FetchForage(this.forages[sheetName]);
        }

        private int FetchForage(int[] forages)
        {
            return forages[this.r.Next(forages.Length)];
        }

        private int FetchGeneralForage(string season, string locationName)
        {
            var lookup = new string[] { $"{locationName}_{season}", locationName, season };

            foreach (var sheetName in lookup)
            {
                if (this.forages.ContainsKey(sheetName))
                {
                    return this.FetchForage(sheetName);
                }
            }
            
            return -1;
        }

        private int[] GetForageSheet(string season, string locationName)
        {
            var lookup = new string[] { $"{locationName}_{season}", locationName, season };

            foreach (var sheetName in lookup)
            {
                if (this.forages.TryGetValue(sheetName, out int[] forages))
                    return forages;
            }

            return new int[] { };
        }

        private static bool BushIsInBloom(Bush bush)
        {
            SDate date = SDate.Now();

            return !bush.townBush.Value && bush.inBloom(date.Season, date.Day);
        }

        private void SaveForage(SObject foragedObject)
        {
            if (foragedObject == null)
                return;

            // See docs: https://stardewvalleywiki.com/Modding:Object_data#Categories
            
            double shareChance = 0.42 + this.Leader.getFriendshipHeartLevelForNPC(this.Forager.Name) * 0.032;
            double current = this.r.NextDouble();

            this.Forager.doEmote(Game1.random.NextDouble() < .1f ? 20 : 16);

            if (!this.forages.TryGetValue("qualityCategories", out int[] qualityCategories) || !qualityCategories.Contains(foragedObject.Category))
            {
                // Ignore quality for objects from category which is out of bounds the quality categories.
                foragedObject.Quality = 0;
            }

            if (this.ai.Monitor.IsVerbose)
            {
                this.ai.Monitor.VerboseLog($"(Forage share) chance: {shareChance} | current: {current} | passed: {(current < shareChance ? "Yes" : "No")}");
            }

            if (current < shareChance || NpcAdventureMod.DebugFlags.Contains("forager.shareAlways"))
            {
                this.foragedObjects.Push(foragedObject);
                this.ai.Monitor.Log($"{this.Forager.Name} wants to share a foraged item with farmer (items in stack: {this.foragedObjects.Count})");
            }
        }

        internal bool CanForage()
        {
            GameLocation location = this.Forager.currentLocation;

            return (location.IsOutdoors || location is MineShaft mines && mines.mineLevel % 10 != 0) && !this.IsLeaderTooFar();
        }

        internal bool GiveForageTo(Farmer player)
        {
            if (player.addItemToInventoryBool(this.foragedObjects.Peek()))
            {
                this.foragedObjects.Pop();
                return true;
            }

            this.ai.Monitor.Log("Can't add shared forages to inventory, it's probably full!");
            return false;
        }

        internal bool HasAnyForage()
        {
            return this.foragedObjects.Count > 0;
        }

        private void Ai_LocationChanged(object sender, EventArgsLocationChanged e)
        {
            this.targetObject = null;
            this.ignoreList.Clear();
            this.joystick.Reset();
        }

        public bool IsIdle { get; private set; }

        public void Activate()
        {
            this.IsIdle = false;
        }

        public void Deactivate()
        {
            this.targetObject = null;
            this.joystick.Reset();
        }

        protected virtual bool IsLeaderTooFar()
        {
            return Helper.Distance(this.Leader.getTileLocationPoint(), this.Forager.getTileLocationPoint()) > 12f;
        }

        /// <summary>
        /// Pick tile for walk to and forage
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xTilesAround"></param>
        /// <returns></returns>
        protected virtual Vector2 PickTile(Vector2 source, int xTilesAround, int yTilesAround)
        {
            Vector2 thisTile = source;

            int dir = Game1.random.Next(0, 4);
            Vector2 nextTile;
            switch (dir)
            {
                case 0:
                    nextTile = new Vector2(thisTile.X, thisTile.Y - yTilesAround); break;
                case 1:
                    nextTile = new Vector2(thisTile.X + xTilesAround, thisTile.Y); break;
                case 2:
                    nextTile = new Vector2(thisTile.X, thisTile.Y + yTilesAround); break;
                case 3:
                    nextTile = new Vector2(thisTile.X - xTilesAround, thisTile.Y); break;
                default:
                    nextTile = thisTile; break;
            }

            if (this.pathFinder.IsWalkableTile(nextTile))
                thisTile = nextTile;
            return thisTile;
        }

        protected virtual Vector2 PickTile()
        {
            int tilesAround = Game1.random.Next(2, 4);

            return this.PickTile(this.Forager.getTileLocation(), tilesAround, tilesAround);
        }

        private Vector2 GetTerrainFeatureTilePosition(TerrainFeature feature)
        {
            if (feature is Bush bush)
            {
                return bush.tilePosition.Value;
            }

            return feature.currentTileLocation;
        }

        /// <summary>
        /// Acquire a place with a terrain feature (like bush or tree) for foraging
        /// </summary>
        private void AcquireTerrainFeature()
        {
            var bushes = this.Forager.currentLocation.largeTerrainFeatures
                .Where((feature) => feature is Bush && !this.ignoreList.Contains(feature));

            var trees = this.Forager.currentLocation.terrainFeatures.Values
                .Where((feature) => feature is Tree t && t.growthStage.Value > Tree.sproutStage && !this.ignoreList.Contains(feature))
                .Union(bushes.Cast<TerrainFeature>())
                .ToList();

            if (trees.Count <= 0)
            {
                this.joystick.AcquireTarget(this.PickTile());
                return;
            }

            trees.Sort((f1, f2) =>
            {
                Vector2 vfo = this.Forager.getTileLocation(); // vfo as Vector of Forager
                Vector2 vtf1 = this.GetTerrainFeatureTilePosition(f1); // vtf as Vector of Terrain Feature (like a Bush or Tree)
                Vector2 vtf2 = this.GetTerrainFeatureTilePosition(f2);
                float d1 = Utility.distance(vfo.X, vtf1.X, vtf1.Y, vfo.Y);
                float d2 = Utility.distance(vfo.X, vtf2.X, vtf2.Y, vfo.Y);

                if (d1 == d2)
                {
                    return 0;
                }

                return d1.CompareTo(d2);
            });

            var tree = trees.First();
            Vector2 vTree = this.GetTerrainFeatureTilePosition(tree);

            if (Vector2.Distance(vTree, this.Forager.getTileLocation()) > 8)
            {
                // Nearest bush/tree is too far, fallback to pick a tile around
                this.joystick.AcquireTarget(this.PickTile());
                return;
            }

            if (this.joystick.AcquireTarget(this.PickTile(vTree, (tree is Bush bush ? bush.size.Value + 1 : 1), 1)))
            {
                this.targetObject = tree;
            } else
            {
                this.joystick.AcquireTarget(this.PickTile());
            }
        }

        public void Update(UpdateTickedEventArgs e)
        {
            if (this.IsIdle || (!Context.IsPlayerFree && !Context.IsMultiplayer))
            {
                return;
            }

            if (!this.CanForage())
            {
                this.IsIdle = true;
                return;
            }

            if (e.IsMultipleOf(30) && this.IsLeaderTooFar())
            {
                this.YellAndAbort();
                return;
            }

            if (!this.joystick.IsFollowing)
            {
                if (this.r.NextDouble() > .5f)
                {
                    this.AcquireTerrainFeature();
                } else 
                {
                    this.joystick.AcquireTarget(this.PickTile());
                }
            }

            this.joystick.Speed = 4f;
            this.joystick.Update(e);
        }

        private void YellAndAbort()
        {
            this.IsIdle = true;
            this.Leader.changeFriendship(-5, this.Forager);
            Game1.drawDialogue(this.Forager, this.ai.Csm.Dialogues.GetFriendSpecificDialogueText(this.Leader, "farmerRunAway"));

            if (this.HasAnyForage() && this.r.Next(3) == 1)
            {
                // Chance 1:3 forager changes their mind 
                // to share some foraged item with you and don't share it
                this.foragedObjects.Pop();
                this.ai.Monitor.Log($"{this.Forager.Name} changed her/his mind and don't share last forage with farmer.");
            }
        }
    }
}
