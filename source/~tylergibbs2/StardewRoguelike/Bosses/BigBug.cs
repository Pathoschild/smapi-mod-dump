/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewRoguelike.Extensions;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace StardewRoguelike.Bosses
{
    public class BigBug : Bat, IBossMonster
    {
        public string DisplayName => "Odys the Hivemother";

        public string MapPath => "boss-bug";

        public string TextureName => "Characters\\Monsters\\Bug_dangerous";

        public Vector2 SpawnLocation => new(17, 23);

        public List<string> MusicTracks
        => new() { "ceaseless_and_incessant" };

        public bool InitializeWithHealthbar
        => true;

        public float Difficulty { get; set; }

        private enum State
        {
            Normal,
            MovingToEgg,
            LayingEgg
        }

        public enum MinionState
        {
            Normal,
            Fast,
            Debuffing,
            Defensive,
            Aggressive,
            Suicidal
        }

        private readonly NetEnum<State> CurrentState = new(State.Normal);

        private MinionState PreviousMinionState;

        private readonly NetEnum<MinionState> CurrentMinionState = new(MinionState.Normal);

        private readonly Dictionary<Vector2, int> Eggs = new();

        private readonly NetEvent1Field<Vector2, NetVector2> SpawnEggEvent = new();

        private readonly NetEvent1Field<Vector2, NetVector2> despawnEggEvent = new();

        private readonly NetEvent0 OnAttackedEvent = new();

        private Vector2 TargetEggSpot;

        private int TicksToLayEgg = 300;

        private int StateTickCount = 0;

        private int TicksToChangeMinions = 800;

        private int TicksToOffscreenBugs = 180;

        private readonly List<Vector2> EggSpawnLocations = new()
        {
            new(14, 10),
            new(25, 11),
            new(33, 14),
            new(10, 16),
            new(16, 21),
            new(30, 27),
            new(10, 26),
            new(13, 34),
            new(23, 25)
        };

        public BigBug() { }

        public BigBug(float difficulty) : base(Vector2.Zero)
        {
            setTileLocation(SpawnLocation);
            Difficulty = difficulty;

            Sprite.LoadTexture(TextureName);
            Sprite.SpriteHeight = 16;
            Sprite.UpdateSourceRect();
            Scale = 3f;

            moveTowardPlayerThreshold.Value = 20;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(SpawnEggEvent, despawnEggEvent, OnAttackedEvent);
            SpawnEggEvent.onEvent += SpawnEgg;
            despawnEggEvent.onEvent += DespawnEgg;
            OnAttackedEvent.onEvent += OnAttacked;
        }

        public override void reloadSprite()
        {
            Sprite = new(TextureName);
            Sprite.LoadTexture(TextureName);
            Sprite.SpriteHeight = 16;
            Sprite.UpdateSourceRect();
            HideShadow = true;
        }

        public void OnAttacked()
        {
            if (Context.IsMainPlayer && CurrentState.Value == State.LayingEgg)
            {
                CurrentState.Value = State.Normal;
                TicksToLayEgg = 300;
                StateTickCount = 0;
            }
        }

        public override void MovePosition(GameTime time, xTile.Dimensions.Rectangle viewport, GameLocation currentLocation)
        {
            if (CurrentState == State.Normal)
                base.MovePosition(time, viewport, currentLocation);
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            CheckAllEggs();

            if (TicksToOffscreenBugs > 0)
            {
                TicksToOffscreenBugs--;

                if (TicksToOffscreenBugs == 0)
                {
                    int toSpawn = Game1.random.Next(1, 3);
                    SpawnOffScreenBugs(toSpawn);

                    TicksToOffscreenBugs = Game1.random.Next(6 * 60, 10 * 60);
                    if (Roguelike.HardMode)
                        TicksToOffscreenBugs -= Game1.random.Next(60, 3 * 60);
                }
            }

            if (TicksToChangeMinions > 0)
            {
                TicksToChangeMinions--;

                if (TicksToChangeMinions == 0)
                {
                    if (CurrentMinionState.Value != MinionState.Normal)
                    {
                        PreviousMinionState = CurrentMinionState;
                        CurrentMinionState.Value = MinionState.Normal;
                        TicksToChangeMinions = 300;
                    }
                    else
                    {
                        var validStates = new List<MinionState>((IEnumerable<MinionState>)Enum.GetValues(typeof(MinionState)));
                        validStates.Remove(PreviousMinionState);

                        CurrentMinionState.Value = validStates[Game1.random.Next(validStates.Count)];

                        TicksToChangeMinions = Roguelike.HardMode ? 480 : 600;
                    }

                    SetAllMinionStates(CurrentMinionState);
                }
            }

            if (CurrentState == State.Normal)
            {
                base.behaviorAtGameTick(time);

                if (TicksToLayEgg > 0)
                {
                    TicksToLayEgg--;

                    if (TicksToLayEgg == 0)
                    {
                        do
                        {
                            TargetEggSpot = EggSpawnLocations[Game1.random.Next(EggSpawnLocations.Count)];
                        } while (Eggs.ContainsKey(TargetEggSpot));

                        TicksToLayEgg = 120;
                        CurrentState.Value = State.MovingToEgg;
                        StateTickCount = 0;
                    }
                }
            }
            else if (CurrentState == State.MovingToEgg)
            {
                Vector2 moveVector = (new Vector2(TargetEggSpot.X + 1, TargetEggSpot.Y + 1) * 64f) - GetBoundingBox().Center.ToVector2();

                if (moveVector.LengthSquared() <= 90)
                {
                    CurrentState.Value = State.LayingEgg;
                    StateTickCount = 0;
                    return;
                }

                moveVector.Normalize();
                moveVector *= Roguelike.HardMode ? 11f : 8f;

                Position += moveVector;
                rotation = RoguelikeUtility.VectorToRadians(moveVector) + RoguelikeUtility.DegreesToRadians(90);
            }
            else if (CurrentState == State.LayingEgg)
            {
                if (StateTickCount > 120)
                {
                    Eggs[TargetEggSpot] = 500;
                    SpawnEggEvent.Fire(TargetEggSpot);

                    CurrentState.Value = State.Normal;
                    StateTickCount = 0;
                }
            }

            StateTickCount++;
        }

        public void CheckAllEggs()
        {
            foreach (Vector2 tileLocation in Eggs.Keys)
            {
                if (Eggs[tileLocation] == 0)
                    continue;

                Eggs[tileLocation]--;
                if (Eggs[tileLocation] == 0)
                {
                    despawnEggEvent.Fire(tileLocation);
                    SpawnBugs(tileLocation);
                }
            }
        }

        public void SetAllMinionStates(MinionState newState)
        {
            if (newState != MinionState.Normal)
                currentLocation.playSound("shadowpeep");

            foreach (Character character in currentLocation.characters)
            {
                if (character is BigBugMinion minion)
                    minion.ChangeState(newState);
            }
        }

        public void SpawnOffScreenBugs(int amount)
        {
            while (amount > 0)
            {
                Vector2 spawnLocation = Vector2.Zero;
                switch (Game1.random.Next(4))
                {
                    case 0:
                        spawnLocation.X = Game1.random.Next(currentLocation.map.Layers[0].LayerWidth);
                        break;
                    case 3:
                        spawnLocation.Y = Game1.random.Next(currentLocation.map.Layers[0].LayerHeight);
                        break;
                    case 1:
                        spawnLocation.X = currentLocation.map.Layers[0].LayerWidth - 1;
                        spawnLocation.Y = Game1.random.Next(currentLocation.map.Layers[0].LayerHeight);
                        break;
                    case 2:
                        spawnLocation.Y = currentLocation.map.Layers[0].LayerHeight - 1;
                        spawnLocation.X = Game1.random.Next(currentLocation.map.Layers[0].LayerWidth);
                        break;
                }
                if (Utility.isOnScreen(spawnLocation * 64f, 64))
                    spawnLocation.X -= Game1.viewport.Width / 64;

                BigBugMinion minion = new(spawnLocation, Difficulty, CurrentMinionState);
                minion.moveTowardPlayerThreshold.Value = 100;
                minion.focusedOnFarmers = true;
                currentLocation.characters.Add(minion);

                amount--;
            }
        }

        public void SpawnBugs(Vector2 tileLocation)
        {
            int bugsToSpawn = Game1.random.Next(2, 4);
            if (Curse.AnyFarmerHasCurse(CurseType.MoreEnemiesLessHealth))
                bugsToSpawn++;

            tileLocation *= 64;

            while (bugsToSpawn > 0)
            {
                tileLocation = new(tileLocation.X + 64 + Game1.random.Next(-32, 33), tileLocation.Y + 64 + Game1.random.Next(-32, 33));
                BigBugMinion minion = new(tileLocation, Difficulty, CurrentMinionState);
                currentLocation.characters.Add(minion);

                bugsToSpawn--;
            }
        }

        public void SpawnEgg(Vector2 tileLocation)
        {
            MineShaft mine = (MineShaft)currentLocation;
            mine.SetTile((int)tileLocation.X, (int)tileLocation.Y, "Front", "bugLandTiles", 92, 1000, new int[] { 92, 86, 92, 102 });
            mine.SetTile((int)tileLocation.X + 1, (int)tileLocation.Y, "Front", "bugLandTiles", 93, 1000, new int[] { 93, 87, 93, 103 });
            mine.SetTile((int)tileLocation.X, (int)tileLocation.Y + 1, "Front", "bugLandTiles", 100, 1000, new int[] { 100, 94, 100, 110 });
            mine.SetTile((int)tileLocation.X + 1, (int)tileLocation.Y + 1, "Front", "bugLandTiles", 101, 1000, new int[] { 101, 95, 101, 111 });

            Game1.playSound("fishSlap");
        }

        public void DespawnEgg(Vector2 tileLocation)
        {
            MineShaft mine = (MineShaft)currentLocation;
            xTile.Dimensions.Location tile = new((int)tileLocation.X, (int)tileLocation.Y);
            mine.removeTile(tile, "Front");
            mine.removeTile(new(tile.X + 1, tile.Y), "Front");
            mine.removeTile(new(tile.X, tile.Y + 1), "Front");
            mine.removeTile(new(tile.X + 1, tile.Y + 1), "Front");

            Game1.playSound("slimeHit");

            if (Context.IsMainPlayer)
                Eggs.Remove(tileLocation);
        }

        protected override void updateAnimation(GameTime time)
        {
            if (CurrentState == State.LayingEgg)
            {
                Sprite.currentFrame = 4;
                Halt();
                return;
            }

            base.updateAnimation(time);
        }

        public override void update(GameTime time, GameLocation location)
        {
            this.KeepInMap();
            base.update(time, location);
            SpawnEggEvent.Poll();
            despawnEggEvent.Poll();
            OnAttackedEvent.Poll();
        }

        public override Rectangle GetBoundingBox()
        {
            int boxWidth = (int)(Sprite.SpriteWidth * 4 * 3 / 4 * Scale);
            int boxHeight = (int)(26 * Scale);
            return new((int)Position.X - (boxWidth / 4), (int)Position.Y + boxHeight / 3, boxWidth, boxHeight);
        }

        public override void shedChunks(int number, float scale)
        {
            Game1.createRadialDebris(currentLocation, Sprite.textureName.Value, new Rectangle(0, Sprite.SpriteHeight * 4, Sprite.SpriteWidth, Sprite.SpriteHeight), Sprite.SpriteWidth / 2, GetBoundingBox().Center.X, GetBoundingBox().Center.Y, number, (int)getTileLocation().Y, Color.White, 4f);
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            OnAttackedEvent.Fire();

            if (CurrentState.Value != State.Normal)
                damage /= 2;

            int result = base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
            if (Health <= 0)
                BossManager.Death(currentLocation, who, DisplayName, SpawnLocation);

            return result;
        }
    }
}
