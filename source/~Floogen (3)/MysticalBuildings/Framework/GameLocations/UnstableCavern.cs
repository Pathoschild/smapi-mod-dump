/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/MysticalBuildings
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Dimensions;
using xTile.Tiles;

namespace CaveOfMemories.Framework.GameLocations
{
    internal class UnstableCavern : GameLocation
    {
        private const int MAP_VARIATIONS = 3;

        internal Vector2 tileBeneathLadder;

        private int _timeRemaining;
        private int _shakeCooldown;
        private bool _hasBeenWarned;
        private bool _isLeaving;
        private bool _isCollapsing;
        private Point _exitTile;
        private GameLocation _exitLocation;

        public UnstableCavern(GameLocation exitLocation, Point exitTile) : base(GenerateMapName(), "UnstableCavern")
        {
            this.IsOutdoors = false;
            this.findLadder();
            this.populateLevel();
            this.LightLevel = 0.1f;

            _exitTile = exitTile;
            _exitLocation = exitLocation;
            _timeRemaining = ((MysticalBuildings.GenerateRandom().Next(1, 6) * 5) + 35) * 1000;
        }

        private static string GenerateMapName()
        {
            int index = MysticalBuildings.GenerateRandom().Next(1, MAP_VARIATIONS + 1);
            return "Maps\\" + $"unstable_mine_{index}";
        }

        public override bool isTileOccupiedForPlacement(Vector2 tileLocation, StardewValley.Object toPlace = null)
        {
            // Preventing player from placing items here
            return true;
        }

        public override void UpdateWhenCurrentLocation(GameTime time)
        {
            base.UpdateWhenCurrentLocation(time);

            if (_isLeaving is false && Game1.activeClickableMenu is null)
            {
                if (_shakeCooldown > 0)
                {
                    _shakeCooldown -= time.ElapsedGameTime.Milliseconds;
                }
                if (MysticalBuildings.shakeTimer > 0)
                {
                    MysticalBuildings.shakeTimer -= time.ElapsedGameTime.Milliseconds;
                }

                if (MysticalBuildings.shakeTimer <= 0 && MysticalBuildings.GenerateRandom().NextDouble() < 0.2)
                {
                    if (_shakeCooldown <= 0)
                    {
                        MysticalBuildings.shakeTimer = (MysticalBuildings.GenerateRandom().Next(1, 3) * 1000) + (MysticalBuildings.GenerateRandom().Next(1, 5) * 250);
                        _shakeCooldown = MysticalBuildings.GenerateRandom().Next(5, 15) * 1000;
                        this.playSound("thunder_small");
                    }
                }

                _timeRemaining -= time.ElapsedGameTime.Milliseconds;
                MysticalBuildings.cavernTimer = _timeRemaining / 1000;

                if (_timeRemaining < 0)
                {
                    _isLeaving = true;
                    Game1.addHUDMessage(new HUDMessage(MysticalBuildings.i18n.Get("Mine.Message.Leaving"), null));
                    HandleCavernExit(Game1.player, "Leave");
                }
                else if (_timeRemaining < 15000 && _hasBeenWarned is false)
                {
                    _hasBeenWarned = true;
                    Game1.addHUDMessage(new HUDMessage(MysticalBuildings.i18n.Get("Mine.Message.Warning"), null));
                }
                else if (_timeRemaining < 3500 && _isCollapsing is false)
                {
                    // Shake constantly during last 3.5 seconds
                    MysticalBuildings.shakeTimer = 3500;
                    this.playSound("thunder_small");
                    _isCollapsing = true;
                }
            }
        }

        public override bool checkAction(Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            Tile tile = base.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            if (tile != null && who.IsLocalPlayer)
            {
                switch (tile.TileIndex)
                {
                    case 115:
                        {
                            Response[] responses = new Response[2]
                            {
                                new Response("Leave", Game1.content.LoadString("Strings\\Locations:Mines_LeaveMine")).SetHotKey(Keys.Y),
                                new Response("Do", Game1.content.LoadString("Strings\\Locations:Mines_DoNothing")).SetHotKey(Keys.Escape)
                            };


                            who.currentLocation.createQuestionDialogue(MysticalBuildings.i18n.Get("Mine.Question.Leave"), responses, new GameLocation.afterQuestionBehavior((who, whichAnswer) => HandleCavernExit(who, whichAnswer)));
                            return true;
                        }
                }
            }
            return base.checkAction(tileLocation, viewport, who);
        }

        private void HandleCavernExit(Farmer who, string whichAnswer)
        {
            if (whichAnswer == "Leave")
            {
                MysticalBuildings.shakeTimer = 0;
                MysticalBuildings.cavernTimer = 0;
                Game1.warpFarmer(_exitLocation.NameOrUniqueName, _exitTile.X, _exitTile.Y, 2);
            }
        }

        private void findLadder()
        {
            int found = 0;
            int currentTileIndex = -1;
            base.lightGlows.Clear();
            for (int y = 0; y < base.map.GetLayer("Buildings").LayerHeight; y++)
            {
                for (int x = 0; x < base.map.GetLayer("Buildings").LayerWidth; x++)
                {
                    if (base.map.GetLayer("Buildings").Tiles[x, y] != null)
                    {
                        currentTileIndex = base.map.GetLayer("Buildings").Tiles[x, y].TileIndex;
                        switch (currentTileIndex)
                        {
                            case 115:
                                this.tileBeneathLadder = new Vector2(x, y + 1);
                                base.sharedLights[x + y * 999] = new LightSource(4, new Vector2(x, y - 2) * 64f + new Vector2(32f, 0f), 0.25f, new Color(0, 20, 50), x + y * 999, LightSource.LightContext.None, 0L);
                                base.sharedLights[x + y * 998] = new LightSource(4, new Vector2(x, y - 1) * 64f + new Vector2(32f, 0f), 0.5f, new Color(0, 20, 50), x + y * 998, LightSource.LightContext.None, 0L);
                                base.sharedLights[x + y * 997] = new LightSource(4, new Vector2(x, y) * 64f + new Vector2(32f, 0f), 0.75f, new Color(0, 20, 50), x + y * 997, LightSource.LightContext.None, 0L);
                                base.sharedLights[x + y * 1000] = new LightSource(4, new Vector2(x, y + 1) * 64f + new Vector2(32f, 0f), 1f, new Color(0, 20, 50), x + y * 1000, LightSource.LightContext.None, 0L);
                                found++;
                                break;
                        }
                    }
                }
            }
        }

        private void populateLevel()
        {
            base.objects.Clear();
            base.terrainFeatures.Clear();
            base.resourceClumps.Clear();
            base.debris.Clear();
            base.characters.Clear();

            var mineRandom = MysticalBuildings.GenerateRandom();
            int stonesLeftOnThisLevel = 0;
            double stoneChance = (double)mineRandom.Next(10, 30) / 100.0;
            double gemStoneChance = 0.003;

            for (int j = 0; j < base.map.GetLayer("Back").LayerWidth; j++)
            {
                for (int l = 0; l < base.map.GetLayer("Back").LayerHeight; l++)
                {
                    if (this.isTileClearForMineObjects(j, l))
                    {
                        if (mineRandom.NextDouble() <= stoneChance)
                        {
                            Vector2 objectPos4 = new Vector2(j, l);
                            if (base.Objects.ContainsKey(objectPos4))
                            {
                                continue;
                            }
                            else
                            {
                                Object stone = this.chooseStoneType(0.001, 5E-05, gemStoneChance, objectPos4);
                                if (stone != null)
                                {
                                    base.Objects.Add(objectPos4, stone);
                                    stonesLeftOnThisLevel++;
                                }
                            }
                        }
                        else if (mineRandom.NextDouble() <= 0.005)
                        {
                            if (!this.isTileClearForMineObjects(j + 1, l) || !this.isTileClearForMineObjects(j, l + 1) || !this.isTileClearForMineObjects(j + 1, l + 1))
                            {
                                continue;
                            }
                            Vector2 objectPos2 = new Vector2(j, l);
                            int whichClump = ((mineRandom.NextDouble() < 0.5) ? 752 : 754);
                            base.resourceClumps.Add(new ResourceClump(whichClump, 2, 2, objectPos2));
                        }
                    }
                }
            }

            this.tryToAddOreClumps();

            if (this.getObjectAtTile((int)tileBeneathLadder.X, (int)tileBeneathLadder.Y) is not null)
            {
                base.objects.Remove(tileBeneathLadder);
            }
        }

        private void tryToAddOreClumps()
        {
            var mineRandom = MysticalBuildings.GenerateRandom();
            Vector2 endPoint = base.getRandomTile();
            for (int tries = 0; tries < 125 || mineRandom.NextDouble() < 0.85 + Game1.player.team.AverageDailyLuck(Game1.currentLocation); tries++)
            {
                if (this.tileBeneathLadder.Equals(endPoint))
                {
                    continue;
                }
                if (this.isTileLocationTotallyClearAndPlaceable(endPoint) && this.isTileOnClearAndSolidGround(endPoint) && this.doesTileHaveProperty((int)endPoint.X, (int)endPoint.Y, "Diggable", "Back") == null)
                {
                    StardewValley.Object ore = this.getAppropriateOre(endPoint);
                    if ((int)ore.parentSheetIndex == 670)
                    {
                        ore.ParentSheetIndex = 668;
                    }
                    Utility.recursiveObjectPlacement(ore, (int)endPoint.X, (int)endPoint.Y, 0.949999988079071, 0.30000001192092896, this, "Dirt", ((int)ore.parentSheetIndex == 668) ? 1 : 0, 0.05000000074505806, ((int)ore.parentSheetIndex != 668) ? 1 : 2);
                }
                endPoint = base.getRandomTile();
            }
        }

        private StardewValley.Object getAppropriateOre(Vector2 tile)
        {
            var mineRandom = new System.Random((int)((long)Game1.uniqueIDForThisGame + (long)tile.X * 777L + (long)tile.Y * 7L + Game1.stats.DaysPlayed + Game1.player.DailyLuck * 500));
            StardewValley.Object ore = new StardewValley.Object(tile, 751, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false);
            ore.minutesUntilReady.Value = 3;

            var chance = mineRandom.NextDouble();
            if (mineRandom.NextDouble() < 0.02)
            {
                ore = new StardewValley.Object(tile, 765, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false);
                ore.minutesUntilReady.Value = 16;
            }
            else if (mineRandom.NextDouble() < 0.25)
            {
                ore = new StardewValley.Object(tile, (mineRandom.NextDouble() < 0.5) ? 668 : 670, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false);
                ore.minutesUntilReady.Value = 2;
            }
            else if (chance < 0.3)
            {
                ore = new StardewValley.Object(tile, 764, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false);
                ore.minutesUntilReady.Value = 8;
            }
            else if (chance < 0.6)
            {
                ore = new StardewValley.Object(tile, 764, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false);
                ore.minutesUntilReady.Value = 8;
            }
            else if (chance < 0.8)
            {
                ore = new StardewValley.Object(tile, 290, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false);
                ore.minutesUntilReady.Value = 4;
            }

            return ore;
        }

        private bool isTileClearForMineObjects(Vector2 v)
        {
            if (this.tileBeneathLadder.Equals(v))
            {
                return false;
            }
            if (!this.isTileLocationTotallyClearAndPlaceable(v))
            {
                return false;
            }
            string s = this.doesTileHaveProperty((int)v.X, (int)v.Y, "Type", "Back");
            if (s == null || !s.Equals("Stone"))
            {
                return false;
            }
            if (!this.isTileOnClearAndSolidGround(v))
            {
                return false;
            }
            if (base.objects.ContainsKey(v))
            {
                return false;
            }
            return true;
        }

        private bool isTileClearForMineObjects(int x, int y)
        {
            return this.isTileClearForMineObjects(new Vector2(x, y));
        }

        private bool isTileOnClearAndSolidGround(int x, int y)
        {
            if (base.map.GetLayer("Back").Tiles[x, y] == null)
            {
                return false;
            }
            if (base.map.GetLayer("Front").Tiles[x, y] != null)
            {
                return false;
            }
            if (base.getTileIndexAt(x, y, "Back") == 77)
            {
                return false;
            }
            return true;
        }

        private bool isTileOnClearAndSolidGround(Vector2 v)
        {
            if (base.map.GetLayer("Back").Tiles[(int)v.X, (int)v.Y] == null)
            {
                return false;
            }
            if (base.map.GetLayer("Front").Tiles[(int)v.X, (int)v.Y] != null || base.map.GetLayer("Buildings").Tiles[(int)v.X, (int)v.Y] != null)
            {
                return false;
            }
            if (base.getTileIndexAt((int)v.X, (int)v.Y, "Back") == 77)
            {
                return false;
            }
            return true;
        }


        private StardewValley.Object chooseStoneType(double chanceForPurpleStone, double chanceForMysticStone, double gemStoneChance, Vector2 tile)
        {
            var mineRandom = MysticalBuildings.GenerateRandom();
            Color stoneColor = Color.White;
            int whichStone = mineRandom.Next(31, 42);
            int stoneHealth = 1;

            double averageDailyLuck = Game1.player.team.AverageDailyLuck(Game1.currentLocation);
            double averageMiningLevel = Game1.player.team.AverageSkillLevel(3, Game1.currentLocation);
            double chanceModifier = averageDailyLuck + averageMiningLevel * 0.005;
            if (gemStoneChance != 0.0 && mineRandom.NextDouble() < gemStoneChance + gemStoneChance * chanceModifier)
            {
                return new StardewValley.Object(tile, mineRandom.Next(59, 70), "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false)
                {
                    MinutesUntilReady = 5
                };
            }
            if (mineRandom.NextDouble() < chanceForPurpleStone / 2.0 + chanceForPurpleStone * averageMiningLevel * 0.008 + chanceForPurpleStone * (averageDailyLuck / 2.0))
            {
                whichStone = 44;
            }
            if (mineRandom.NextDouble() < chanceForMysticStone + chanceForMysticStone * averageMiningLevel * 0.008 + chanceForMysticStone * (averageDailyLuck / 2.0))
            {
                whichStone = 46;
            }
            whichStone += whichStone % 2;
            if (mineRandom.NextDouble() < 0.1)
            {
                if (!stoneColor.Equals(Color.White))
                {
                    return new ColoredObject((mineRandom.NextDouble() < 0.5) ? 668 : 670, 1, stoneColor)
                    {
                        MinutesUntilReady = 2,
                        CanBeSetDown = true,
                        name = "Stone",
                        TileLocation = tile,
                        ColorSameIndexAsParentSheetIndex = true,
                        Flipped = (mineRandom.NextDouble() < 0.5)
                    };
                }
                return new StardewValley.Object(tile, (mineRandom.NextDouble() < 0.5) ? 668 : 670, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false)
                {
                    MinutesUntilReady = 2,
                    Flipped = (mineRandom.NextDouble() < 0.5)
                };
            }
            if (!stoneColor.Equals(Color.White))
            {
                return new ColoredObject(whichStone, 1, stoneColor)
                {
                    MinutesUntilReady = stoneHealth,
                    CanBeSetDown = true,
                    name = "Stone",
                    TileLocation = tile,
                    ColorSameIndexAsParentSheetIndex = true,
                    Flipped = (mineRandom.NextDouble() < 0.5)
                };
            }
            return new StardewValley.Object(tile, whichStone, "Stone", canBeSetDown: true, canBeGrabbed: false, isHoedirt: false, isSpawnedObject: false)
            {
                MinutesUntilReady = stoneHealth
            };
        }
    }
}
