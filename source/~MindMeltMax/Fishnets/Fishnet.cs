/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using xTile.Tiles;
using Object = StardewValley.Object;

namespace Fishnets
{
    public class FishNet : Object
    {
        private float yBob;
        [XmlElement("directionOffset")]
        public readonly NetVector2 directionOffset = new NetVector2();
        [XmlElement("bait")]
        public readonly NetRef<Object> bait = new NetRef<Object>();

        public class FishNetSerializable
        {
            public long Owner { get; set; } = 0L;

            public int Bait { get; set; } = -1;

            public int ObjectId { get; set; } = -1;

            public int ObjectStack { get; set; } = -1;

            public Vector2 Tile { get; set; }

            public FishNetSerializable() { }

            public FishNetSerializable(FishNet f) 
            {
                Owner = f.owner.Value;
                if (f.bait.Value is not null)
                    Bait = f.bait.Value.ParentSheetIndex;
                if (f.heldObject.Value is not null)
                {
                    ObjectId = f.heldObject.Value.ParentSheetIndex;
                    ObjectStack = f.heldObject.Value.Stack;
                }
                Tile = f.TileLocation;
            }
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(directionOffset, bait);
        }

        public FishNet() { }

        public FishNet(Vector2 tileLocation, int stack = 1) : base(tileLocation, ModEntry.FishNetId, "Fish Net", true, false, false, false)
        {
            Type = "interactive";
            Stack = stack;
        }

        protected void addOverlayTilesIfNecessary( GameLocation location, int x, int y, List<Vector2> tiles)
        {
            if (location != Game1.currentLocation || location.getTileIndexAt(x, y, "Buildings") < 0 || location.doesTileHaveProperty(x, y + 1, "Back", "Water") != null)
                return;
            tiles.Add(new Vector2(x, y));
        }

        protected bool checkLocation(GameLocation location, float x, float y) => location.doesTileHaveProperty((int)x, (int)y, "Water", "Back") == null || location.doesTileHaveProperty((int)x, (int)y, "Passable", "Buildings") != null;

        public List<Vector2> getOverlayTiles(GameLocation location)
        {
            List<Vector2> tiles = new List<Vector2>();
            if ((double)directionOffset.Y < 0.0)
                addOverlayTilesIfNecessary(location, (int)TileLocation.X, (int)TileLocation.Y, tiles);
            addOverlayTilesIfNecessary(location, (int)TileLocation.X, (int)TileLocation.Y + 1, tiles);
            if ((double)directionOffset.X < 0.0)
                addOverlayTilesIfNecessary(location, (int)TileLocation.X - 1, (int)TileLocation.Y + 1, tiles);
            if ((double)directionOffset.X > 0.0)
                addOverlayTilesIfNecessary(location, (int)TileLocation.X + 1, (int)TileLocation.Y + 1, tiles);
            return tiles;
        }

        public void addOverlayTiles(GameLocation location)
        {
            if (location != Game1.currentLocation)
                return;
            foreach (Vector2 overlayTile in getOverlayTiles(location))
            {
                if (!Game1.crabPotOverlayTiles.ContainsKey(overlayTile))
                    Game1.crabPotOverlayTiles[overlayTile] = 0;
                Game1.crabPotOverlayTiles[overlayTile]++;
            }
        }

        public void removeOverlayTiles(GameLocation location)
        {
            if (location != Game1.currentLocation)
                return;
            foreach (Vector2 overlayTile in getOverlayTiles(location))
            {
                if (Game1.crabPotOverlayTiles.ContainsKey(overlayTile))
                {
                    Game1.crabPotOverlayTiles[overlayTile]--;
                    if (Game1.crabPotOverlayTiles[overlayTile] <= 0)
                        Game1.crabPotOverlayTiles.Remove(overlayTile);
                }
            }
        }

        public void updateOffset(GameLocation location)
        {
            Vector2 zero = Vector2.Zero;
            if (checkLocation(location, TileLocation.X - 1f, TileLocation.Y))
                zero += new Vector2(32f, 0f);
            if (checkLocation(location, tileLocation.X + 1f, TileLocation.Y))
                zero += new Vector2(-32f, 0f);
            if (zero.X != 0.0f && checkLocation(location, TileLocation.X + (float)Math.Sign(zero.X), TileLocation.Y + 1f))
                zero += new Vector2(0.0f, -42f);
            if (checkLocation(location, TileLocation.X, TileLocation.Y - 1f))
                zero += new Vector2(0.0f, 32f);
            if (checkLocation(location, TileLocation.X, TileLocation.Y + 1f))
                zero += new Vector2(0.0f, -42f);
            directionOffset.Value = zero;
        }

        public static bool IsValidPlacementLocation(GameLocation location, int x, int y)
        {
            Vector2 tile = new Vector2(x, y);
            bool flag = location.doesTileHaveProperty(x + 1, y, "Water", "Back") != null && location.doesTileHaveProperty(x - 1, y, "Water", "Back") != null || location.doesTileHaveProperty(x, y + 1, "Water", "Back") != null && location.doesTileHaveProperty(x, y - 1, "Water", "Back") != null;
            return !location.Objects.ContainsKey(tile) && !location.Objects.ContainsKey(new Vector2(tile.X + .5f, tile.Y + .5f)) && flag && (location.doesTileHaveProperty(x, y, "Water", "Back") != null && location.doesTileHaveProperty(x, y, "Passable", "Buildings") == null);
        }

        public override bool canBePlacedInWater() => true;

        public override Item getOne()
        {
            Object o = new Object(ParentSheetIndex, 1);
            o._GetOneFrom(this);
            return o;
        }

        public override bool canBeShipped() => false;

        public override bool canBeTrashed() => true;

        public override bool canBeDropped() => true;

        public override bool canBeGivenAsGift() => false;

        public override void actionOnPlayerEntry()
        {
            updateOffset(Game1.currentLocation);
            addOverlayTiles(Game1.currentLocation);
            base.actionOnPlayerEntry();
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            if (who != null)
                owner.Value = who.UniqueMultiplayerID;
            if (!IsValidPlacementLocation(location, (int)Math.Floor(x / 64f), (int)Math.Floor(y / 64f)))
                return false;
            TileLocation = new Vector2((int)Math.Floor(x / 64f), (int)Math.Floor(y / 64f));
            location.Objects.Add(TileLocation, this);
            location.playSound("waterSlosh");
            DelayedAction.playSoundAfterDelay("slosh", 150);
            addOverlayTiles(location);
            updateOffset(location);
            return true;
        }

        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            if (dropInItem is not Object o || o.Category != -21 || (bait.Value != null || who.professions.Contains(11)))
                return false;
            if (!probe)
            {
                bait.Value = o.getOne() as Object;
                who.currentLocation.playSound("Ship");
            }
            return true;
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (heldObject.Value is not null)
            {
                if (justCheckingForActivity)
                    return true;
                Object o = heldObject.Value;
                heldObject.Value = null;
                if (who.IsLocalPlayer && !who.addItemToInventoryBool(o))
                {
                    heldObject.Value = o;
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                    return false;
                }
                Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                if (fishData.ContainsKey(o.ParentSheetIndex))
                    who.caughtFish(o.ParentSheetIndex, -1);
                readyForHarvest.Value = false;
                bait.Value = null;
                who.animateOnce(279 + who.FacingDirection);
                who.currentLocation.playSound("fishingRodBend");
                DelayedAction.playSoundAfterDelay("coin", 500);
                who.gainExperience(1, 5);
                return true;
            }
            if (bait.Value is null)
            {
                if (justCheckingForActivity)
                    return true;
                if (Game1.didPlayerJustClickAtAll(true))
                {
                    if (Game1.player.addItemToInventoryBool(getOne()))
                    {
                        if (who.isMoving())
                            Game1.haltAfterCheck = false;
                        Game1.playSound("coin");
                        who.currentLocation.Objects.Remove(TileLocation);
                        return true;
                    }
                    else
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                }
            }

            return false;
        }

        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            removeOverlayTiles(environment);
            base.performRemoveAction(tileLocation, environment);
        }

        public override void DayUpdate(GameLocation location)
        {
            bool flag1 = Game1.getFarmer(owner) != null && Game1.getFarmer(owner).professions.Contains(11);
            bool flag2 = Game1.getFarmer(owner) != null && Game1.getFarmer(owner).professions.Contains(10);
            if (owner == 0L && Game1.player.professions.Contains(11))
                flag2 = true;
            if (!(bait.Value != null | flag1) || heldObject.Value != null)
                return;
            readyForHarvest.Value = true;
            Random r = new Random((int)Game1.stats.DaysPlayed + (int)(Game1.uniqueIDForThisGame / 2 + TileLocation.X * 1000 + TileLocation.Y));
            Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            List<int> nums = new List<int>();
            double chance = flag2 ? 0.0 : 0.2;
            if (!flag2)
                chance += location.getExtraTrashChanceForCrabPot((int)TileLocation.X, (int)TileLocation.Y);
            if (r.NextDouble() > chance)
            {
                foreach (KeyValuePair<int, string> kvp in fishData)
                {
                    if (kvp.Value.Contains("trap")) continue;
                    if (flag2)
                        nums.Add(kvp.Key);
                    else
                    {
                        if (r.NextDouble() <= .15)
                        {
                            heldObject.Value = Statics.GetRandomFishForLocation(bait.Value.ParentSheetIndex, Game1.player, location.Name);
                            break;
                        }
                    }
                }
            }
            if (heldObject.Value != null) return;
            if (flag2 && nums.Count > 0)
                heldObject.Value = new Object(nums[r.Next(nums.Count)], bait.Value.ParentSheetIndex == 774 && r.NextDouble() <= .15 ? 2 : 1);
            else
                heldObject.Value = new Object(r.Next(168, 173), 1);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            yBob = (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500.0f + (x * 64f)) * 8.0f + 8.0f;
            spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64f, y * 64f + yBob)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ModEntry.FishNetId, 16, 16), Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((y * 64f) + directionOffset.Y + (x % 4)) / 10000.0f);
            if (!readyForHarvest.Value || heldObject.Value is null) return;
            float num = 4.0f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
            spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64f - 8, y * 64f - 112 + num)), new Rectangle(141, 465, 20, 24), Color.White * .75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((y + 1) * 64f / 10000.0f + 9.99999997475243E-07 + TileLocation.X / 10000.0f));
            spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64f + 32, y * 64f - 72 + num)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, heldObject.Value.ParentSheetIndex, 16, 16), Color.White * .75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float)((y + 1) * 64f / 10000.0f + 9.99999974737875E-06 + TileLocation.X / 10000.0f));
        }
    }
}
