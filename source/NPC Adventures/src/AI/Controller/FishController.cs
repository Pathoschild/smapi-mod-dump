/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using StardewModdingAPI.Events;
using StardewModdingAPI;
using PurrplingCore.Movement;
using SObject = StardewValley.Object;
using IDrawable = PurrplingCore.Internal.IDrawable;

namespace NpcAdventure.AI.Controller
{
    /// <summary>
    /// Fish controler for fisherman companions
    /// </summary>
    class FishController : IController, IDrawable
    {
        public const int FISHING_DISTANCE = 4;

        private readonly PathFinder pathFinder;
        private readonly NpcMovementController joystick;
        private readonly AI_StateMachine ai;
        private readonly NPC fisher;
        private readonly Farmer farmer;
        private readonly Vector2 negativeOne = new Vector2(-1, -1);
        private readonly List<FarmerSprite.AnimationFrame> fishingLeftAnim;
        private readonly List<FarmerSprite.AnimationFrame> fishingRightAnim;
        private readonly Stack<SObject> fishCaught;
        private bool fishingFacingRight;
        private int fishCaughtTimer;
        private int lastCaughtFishIdx;
        private Vector2 fishingSpot;

        public bool IsIdle { get; private set; }
        public bool IsFishing { get; private set; }
        public int Invicibility { get; private set; }

        private enum TileReachability
        {
            Unreachable,
            Walkable,
            Unwalkable,
            Water
        }

        public FishController(AI_StateMachine ai, IModEvents events)
        {
            this.ai = ai;
            this.fisher = ai.npc;
            this.farmer = ai.farmer;
            this.fishingSpot = this.negativeOne;
            this.pathFinder = new PathFinder(this.fisher.currentLocation, this.fisher, this.farmer);
            this.joystick = new NpcMovementController(this.fisher, this.pathFinder);
            this.fishCaught = new Stack<SObject>();

            if (!ai.Csm.HasSkill("fisherman"))
                return;

            ai.LocationChanged += this.Ai_LocationChanged;
            events.GameLoop.TimeChanged += this.OnTimeChanged;
            this.joystick.EndOfRouteReached += this.ArrivedFishingSpot;

            string key = $"{ai.npc.Name.ToLower()}_sideFish";
            var animationDescriptions = this.ai.ContentLoader.LoadData<string, string>("Data/AnimationDescriptions");

            if (!animationDescriptions.ContainsKey(key))
            {
                ai.Monitor.Log($"No fishing animation for `{ai.npc.Name}`: Key `{key}` doesn't exist.", LogLevel.Error);
                return;
            }

            string[] animation = animationDescriptions[key].Split('/');

            if (animation.Length < 3)
            {
                ai.Monitor.Log($"Wrong animation description format for key `{key}`", LogLevel.Error);
                return;
            }

            int[] frames = Utility.parseStringToIntArray(animation[1]);

            this.fishingLeftAnim = frames.Select(f => new FarmerSprite.AnimationFrame(f, 4000, false, true, null, false)).ToList();
            this.fishingRightAnim = frames.Select(f => new FarmerSprite.AnimationFrame(f, 4000)).ToList();
        }

        private float GetSkillMultiplier()
        {
            var multiplier = this.farmer.professions.Contains(6) ? 0.5f : 0.25f;

            if (this.farmer.professions.Contains(8))
                multiplier += 0.25f;

            return multiplier;
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (!this.IsFishing)
                return;

            if (Game1.random.Next(3) == 1)
            {
                SObject fish = this.fisher.currentLocation.getFish(200, 1, this.farmer.FishingLevel / 2, this.farmer, 4.5d, Vector2.Zero);
                if (fish == null || fish.ParentSheetIndex <= 0)
                    fish = new SObject(Game1.random.Next(167, 173), 1, false, -1, 0);
                if (fish.Category == -20 || fish.ParentSheetIndex == 152 || fish.ParentSheetIndex == 153 ||
                    fish.ParentSheetIndex == 157 || fish.ParentSheetIndex == 797 || fish.ParentSheetIndex == 79)
                {
                    fish = new SObject(Game1.random.Next(167, 173), 1, false, -1, 0);
                }
                if (fish.Category != -20 && fish.ParentSheetIndex != 152 && fish.ParentSheetIndex != 153 &&
                    fish.ParentSheetIndex != 157 && fish.ParentSheetIndex != 797 && fish.ParentSheetIndex != 79)
                {
                    int skill = this.farmer.fishingLevel.Value;
                    int addedSkill = this.farmer.addedFishingLevel.Value;
                    int quality = 0;
                    float skillPower = (skill * 0.01f + addedSkill * 0.001f + (float)this.farmer.DailyLuck) * this.GetSkillMultiplier();

                    // There are chance to miss a fish. Depends on farmer's fishing skill and luck
                    if (Game1.random.NextDouble() < 0.2f - Math.Min(skill * 0.01 - this.farmer.DailyLuck * 2, 0.18f) - skillPower / 4)
                    {
                        var bbox = this.fisher.GetBoundingBox();
                        this.Invicibility += 1000;
                        this.fisher.currentLocation.debris.Add(new Debris("Miss", 1, new Vector2((float)bbox.Center.X, (float)bbox.Center.Y), Color.Yellow, 1f, 0f));
                        return;
                    }

                    if (skill >= 8 && Game1.random.NextDouble() < .01f + skillPower)
                        quality = 4;
                    else if (skill >= 6 && Game1.random.NextDouble() < 0.1f + skillPower)
                        quality = 2;
                    else if (skill >= 2 && Game1.random.NextDouble() < 0.2f + skillPower)
                        quality = 1;

                    fish.Quality = quality;

                    this.fishCaught.Push(fish);
                    this.Invicibility += 2000 - Math.Min(this.fishCaught.Count + 100, 2000);
                }

                this.lastCaughtFishIdx = fish.ParentSheetIndex;
                this.fishCaughtTimer = 3000;
                this.Invicibility += 1000;
            }
        }

        private void ArrivedFishingSpot(object sender, NpcMovementController.EndOfRouteReachedEventArgs e)
        {
            this.StartFishing();
        }

        private void StartFishing()
        {
            if (this.IsFishing)
                return;

            this.IsFishing = true;
            this.Invicibility = 30000;

            if (this.fishingFacingRight)
            {
                this.fisher.Sprite.setCurrentAnimation(this.fishingRightAnim);
            }
            else
            {
                this.fisher.drawOffset.Value = new Vector2(-64f, 0);
                this.fisher.Sprite.setCurrentAnimation(this.fishingLeftAnim);
            }

            this.fisher.extendSourceRect(16, 0, false);
            this.fisher.currentLocation.playSound("slosh");
        }

        private void Ai_LocationChanged(object sender, EventArgsLocationChanged e)
        {
            if (this.IsFishing)
                this.IsIdle = true;

            this.joystick.Reset();
            this.CheckFishingHere();
        }

        public void Activate()
        {
            this.IsIdle = false;
            this.fishCaughtTimer = 0;
            this.CheckFishingHere();
        }

        public bool HasAnyFish()
        {
            return this.fishCaught.Count > 0;
        }

        public void CheckFishingHere()
        {
            this.fishingSpot = this.GetFishStandingPoint();

            if (this.fishingSpot == this.negativeOne)
            {
                this.IsIdle = true;
                return;
            }

            if (!this.IsFishing && this.fisher.getTileLocation() == this.fishingSpot)
            {
                this.fisher.Halt();
                this.StartFishing();
                return;
            }

            this.joystick.AcquireTarget(this.fishingSpot);
        }

        /// <summary>
        /// Give caught fishes to farmer's repository.
        /// </summary>
        /// <param name="farmer">The farmer</param>
        /// <returns>Sucessfully placed in repository?</returns>
        public bool GiveFishesTo(Farmer farmer)
        {
            bool somethingAdded = false;
            while (this.HasAnyFish())
            {
                if (!farmer.addItemToInventoryBool(this.fishCaught.Peek()))
                    break;

                somethingAdded = true;
                this.fishCaught.Pop();
            }

            if (!somethingAdded)
                this.ai.Monitor.Log("Can't add shared fishes to inventory, it's probably full!");

            return somethingAdded;
        }

        /// <summary>
        /// Get standing point for fishing (ground tile near water)
        /// </summary>
        /// <returns>Tile position</returns>
        private Vector2 GetFishStandingPoint()
        {
            Vector2 tile = this.negativeOne;
            bool anyWater = false;

            if (this.pathFinder.GameLocation.waterTiles == null)
                return tile;

            TileReachability[,] tileCache = new TileReachability[(FISHING_DISTANCE * 2) + 1, (FISHING_DISTANCE * 2) + 1];
            tileCache[FISHING_DISTANCE, FISHING_DISTANCE] = TileReachability.Walkable;
            Vector2 loc = this.fisher.getTileLocation();
            Vector3 translate = new Vector3(loc.X, loc.Y, 0) - new Vector3(FISHING_DISTANCE, FISHING_DISTANCE, 0);
            Queue<Vector3> tileQueue = new Queue<Vector3>();
            tileQueue.Enqueue(new Vector3(loc.X, loc.Y, 0));

            while (tileQueue.Count != 0)
            {
                Vector3 t = tileQueue.Dequeue();
                Vector3[] neighbors = this.pathFinder.GetDirectWalkableNeighbors(t);
                foreach (Vector3 neighbor in neighbors)
                {
                    Vector3 pos = neighbor - translate;
                    if (pos.X >= 0 && pos.X <= FISHING_DISTANCE * 2 &&
                        pos.Y >= 0 && pos.Y <= FISHING_DISTANCE * 2 &&
                        tileCache[(int)pos.X, (int)pos.Y] == TileReachability.Unreachable)
                    {
                        if (neighbor.Z == 1)
                        {
                            tileCache[(int)pos.X, (int)pos.Y] = TileReachability.Walkable;
                            tileQueue.Enqueue(neighbor);
                        }
                        else
                        {
                            try
                            {
                                if (this.pathFinder.GameLocation.waterTiles[(int)neighbor.X, (int)neighbor.Y])
                                {
                                    tileCache[(int)pos.X, (int)pos.Y] = TileReachability.Water;
                                    anyWater = true;
                                }
                                else
                                {
                                    tileCache[(int)pos.X, (int)pos.Y] = TileReachability.Unwalkable;
                                }
                            } catch (IndexOutOfRangeException)
                            {
                                tileCache[(int)pos.X, (int)pos.Y] = TileReachability.Unreachable;
                            }
                        }
                    }
                }
            }
            tile = this.negativeOne;

            if (!anyWater)
                return this.negativeOne;

            List<Vector3> fishingTiles = new List<Vector3>(FISHING_DISTANCE);
            int xDim = FISHING_DISTANCE * 2;
            for (int y = 0; y < (FISHING_DISTANCE * 2) + 1; y++)
            {
                for (int x = 0; x < (FISHING_DISTANCE * 2) + 1; x++)
                {
                    if (tileCache[x, y] == TileReachability.Water)
                    {
                        if (x > 0 && tileCache[x - 1, y] == TileReachability.Walkable)
                            fishingTiles.Add(new Vector3(x - 1, y, 1));
                        if (x < xDim && tileCache[x + 1, y] == TileReachability.Walkable)
                            fishingTiles.Add(new Vector3(x + 1, y, -1));
                    }
                }
            }

            if (fishingTiles.Count > 0)
            {
                Vector3 t = fishingTiles[Game1.random.Next(fishingTiles.Count)] + translate;
                tile = new Vector2((int)t.X, (int)t.Y);
                this.fishingFacingRight = t.Z > 0;
            }

            return tile;
        }

        public void Deactivate()
        {
            if (this.IsFishing)
            {
                this.fisher.reloadSprite();
                this.fisher.Sprite.SpriteWidth = 16;
                this.fisher.Sprite.SpriteHeight = 32;
                this.fisher.drawOffset.Value = new Vector2(0, 0);
                this.fisher.Sprite.UpdateSourceRect();
                this.IsFishing = false;
            }

            this.fishCaughtTimer = 0;
            this.Invicibility = 10000;
            this.joystick.Reset();
        }

        public void SideUpdate(UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree)
                return;

            if (this.IsFarmerFishingOnInactive())
                this.Invicibility = 0;

            if (this.Invicibility > 0)
                this.Invicibility -= (int)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
        }

        /// <summary>
        /// Check if player do fishing when fisherman don't.
        /// </summary>
        /// <returns></returns>
        public bool IsFarmerFishingOnInactive()
        {
            return this.ai.CurrentState != AI_StateMachine.State.FISH 
                && !this.IsFishing 
                && this.farmer.UsingTool 
                && this.farmer.CurrentTool is FishingRod;
        }

        public void Update(UpdateTickedEventArgs e)
        {
            if (this.IsFishing)
            {
                this.fisher.Sprite.UpdateSourceRect();
                this.fisher.extendSourceRect(16, 0, false);
            }

            if (this.IsIdle || !Context.IsPlayerFree)
                return;

            bool breakFishingImmediatelly = this.IsFishing && this.Invicibility <= 0 && Game1.random.NextDouble() < 0.01f;
            bool shorter = this.IsFishing && this.farmer.isMoving();
            if (!this.ai.IsFarmerNear(shorter ? 8f : 11f) || breakFishingImmediatelly)
            {
                this.IsIdle = true;
                return;
            }

            if (this.fishCaughtTimer > 0)
                this.fishCaughtTimer -= (int)Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;

            if (this.IsFishing && e.IsOneSecond && this.farmer.UsingTool && this.farmer.CurrentTool is FishingRod)
                this.Invicibility += 100;

            this.joystick.Update(e);
        }

        public bool CanFish()
        {
            return this.Invicibility <= 0 && this.GetFishStandingPoint() != this.negativeOne;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.fishCaughtTimer <= 0)
                return;

            float num1 = (float)(2.0 * Math.Round(Math.Sin(DateTime.UtcNow.TimeOfDay.TotalMilliseconds / 250.0), 2)) - 40f;
            Point tileLocationPoint = this.fisher.getTileLocationPoint();
            Vector2 offset = this.fisher.drawOffset.Value;

            if (this.fishingFacingRight)
                offset.X -= this.fisher.Sprite.SpriteWidth * 4;

            float num2 = (tileLocationPoint.Y + 1) * 64 / 10000f;
            spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocationPoint.X * 64 + 64 + offset.X, tileLocationPoint.Y * 64 - 96 - 36 + num1 + offset.Y)), new Rectangle?(new Rectangle(141, 465, 20, 24)), Color.White * 0.75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, num2 + 1E-06f);
            spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocationPoint.X * 64 + 64 + 40 + offset.X, tileLocationPoint.Y * 64 - 64 - 16 - 10 + num1 + offset.Y)), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, this.lastCaughtFishIdx, 16, 16)), Color.White, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, num2 + 1E-05f);
        }
    }
}
