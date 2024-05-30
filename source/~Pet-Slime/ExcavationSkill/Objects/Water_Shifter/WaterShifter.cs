/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using ArchaeologySkill;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonShared;
using Netcode;
using SpaceCore;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using xTile.Dimensions;
using Object = StardewValley.Object;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ArchaeologySkill.Objects.Water_Shifter
{
    [XmlType("Mods_moonslime.Archaeology.water_shifter")]
    public class WaterShifter : CrabPot
    {
        public const int LidFlapTimerInterval = 60;

        [XmlIgnore]
        private float YBob;

        [XmlElement("Water_shifter_TITS")]
        public readonly NetInt Water_shifter_TITS = new NetInt(0);

        [XmlIgnore]
        public int TileIndexToShow
        {
            get => Water_shifter_TITS.Value;
            set => Water_shifter_TITS.Value = value;
        }

        [XmlIgnore]
        private bool LidFlapping;

        [XmlIgnore]
        private bool LidClosing;

        [XmlIgnore]
        private float LidFlapTimer;

        [XmlIgnore]
        private float ShakeTimer;

        [XmlIgnore]
        private Vector2 Shake;

        public WaterShifter() : base() { }

        public WaterShifter(Vector2 TileLocation, int stack = 1) : this()
        {
            TileLocation = Vector2.Zero;
            ParentSheetIndex = 0;
            base.itemId.Value = "moonslime.Archaeology.water_shifter";
            name = ModEntry.Instance.I18N.Get("moonslime.Archaeology.water_shifter.name");
            CanBeSetDown = true;
            CanBeGrabbed = false;
            IsSpawnedObject = false;
            this.Type = "interactive";
            this.TileIndexToShow = 0;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddField(Water_shifter_TITS, "Water_shifter_TITS");
        }

        protected void AddOverlayTilesIfNecessary(GameLocation location, int x, int y, List<Vector2> tiles)
        {
            if (location != Game1.currentLocation || location.getTileIndexAt(x, y, "Buildings") < 0 || location.doesTileHaveProperty(x, y + 1, "Back", "Water") != null)
                return;
            tiles.Add(new(x, y));
        }


        public override string DisplayName
        {
            get
            {
                displayName = ModEntry.Instance.I18N.Get("moonslime.Archaeology.water_shifter.name");
                if (orderData.Value == "QI_COOKING")
                {
                    displayName = Game1.content.LoadString("Strings\\StringsFromCSFiles:Fresh_Prefix", displayName);
                }

                if (isRecipe.Value)
                {
                    string text = displayName;
                    if (CraftingRecipe.craftingRecipes.TryGetValue(displayName, out string value))
                    {
                        string text2 = ArgUtility.SplitBySpaceAndGet(ArgUtility.Get(value.Split('/'), 2), 1);
                        if (text2 != null)
                        {
                            text = text + " x" + text2;
                        }
                    }

                    return text + Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.12657");
                }

                return displayName;
            }
        }

        public override string getDescription()
        {
            return ModEntry.Instance.I18N.Get("moonslime.Archaeology.water_shifter.description");
        }

        public new string Name = ModEntry.Instance.I18N.Get("moonslime.Archaeology.water_shifter.name");

        public List<Vector2> GetOverlayTiles(GameLocation location)
        {
            List<Vector2> tiles = new();
            if (directionOffset.Y < 0f)
                AddOverlayTilesIfNecessary(location, (int)TileLocation.X, (int)TileLocation.Y, tiles);
            AddOverlayTilesIfNecessary(location, (int)TileLocation.X, (int)TileLocation.Y + 1, tiles);
            if (directionOffset.X < 0f)
                AddOverlayTilesIfNecessary(location, (int)TileLocation.X - 1, (int)TileLocation.Y + 1, tiles);
            if (directionOffset.X > 0f)
                AddOverlayTilesIfNecessary(location, (int)TileLocation.X + 1, (int)TileLocation.Y + 1, tiles);
            return tiles;
        }

        public void AddOverlayTiles(GameLocation location)
        {
            if (location != Game1.currentLocation)
                return;
            foreach (Vector2 overlayTile in GetOverlayTiles(location))
            {
                if (!Game1.crabPotOverlayTiles.ContainsKey(overlayTile))
                    Game1.crabPotOverlayTiles[overlayTile] = 0;
                Game1.crabPotOverlayTiles[overlayTile]++;
            }
        }

        public void RemoveOverlayTiles(GameLocation location)
        {
            if (location != Game1.currentLocation)
                return;
            foreach (Vector2 overlayTile in GetOverlayTiles(location))
            {
                if (Game1.crabPotOverlayTiles.ContainsKey(overlayTile))
                {
                    Game1.crabPotOverlayTiles[overlayTile]--;
                    if (Game1.crabPotOverlayTiles[overlayTile] <= 0)
                        Game1.crabPotOverlayTiles.Remove(overlayTile);
                }
            }
        }

        public void UpdateOffset()
        {
            Vector2 zero = Vector2.Zero;
            if (checkLocation(tileLocation.X - 1f, tileLocation.Y))
            {
                zero += new Vector2(32f, 0f);
            }

            if (checkLocation(tileLocation.X + 1f, tileLocation.Y))
            {
                zero += new Vector2(-32f, 0f);
            }

            if (zero.X != 0f && checkLocation(tileLocation.X + (float)Math.Sign(zero.X), tileLocation.Y + 1f))
            {
                zero += new Vector2(0f, -42f);
            }

            if (checkLocation(tileLocation.X, tileLocation.Y - 1f))
            {
                zero += new Vector2(0f, 32f);
            }

            if (checkLocation(tileLocation.X, tileLocation.Y + 1f))
            {
                zero += new Vector2(0f, -42f);
            }

            directionOffset.Value = zero;
        }

        public static bool IsValidPlacementLocation(GameLocation location, int x, int y)
        {
            Vector2 tile = new(x, y);
            bool flag = location.isWaterTile(x + 1, y) && location.isWaterTile(x - 1, y) || location.isWaterTile(x, y + 1) && location.isWaterTile(x, y - 1);
            return location is not Caldera && !location.Objects.ContainsKey(tile) && flag && (location.isWaterTile(x, y) && location.doesTileHaveProperty(x, y, "Passable", "Buildings") == null);
        }

        

        protected override Item GetOneNew()
        {
            return new Object("moonslime.Archaeology.water_shifter", 1);
        }

        public override bool isPlaceable() => true;

        public override bool canBeTrashed() => true;

        public override bool canBeDropped() => true;

        public override bool canBeGivenAsGift() => false;

        public override bool canBeShipped() => false;



        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who, bool returnFalseIfItemConsumed = false)
        {
            GameLocation location =  Location;
            if (location == null)
            {
                return false;
            }

            if (!(dropInItem is Object @object))
            {
                return false;
            }


            if (@object.name == "Fiber" &&  bait.Value == null)
            {
                if (!probe)
                {
                    if (who != null)
                    {
                         owner.Value = who.UniqueMultiplayerID;
                    }

                     bait.Value = @object.getOne() as Object;
                    location.playSound("Ship");
                     LidFlapping = true;
                     LidFlapTimer = 60f;
                }

                return true;
            }

            return false;
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            GameLocation location = Location;
            if (TileIndexToShow == 4)
            {

                if (justCheckingForActivity)
                {
                    return true;
                }

                if ( heldObject.Value == null)
                {
                    bait.Value = null;
                    readyForHarvest.Value = false;
                    TileIndexToShow = 0;
                    return true;
                }

                Object value =  heldObject.Value;
                heldObject.Value = null;
                if (who.IsLocalPlayer && !who.addItemToInventoryBool(value))
                {
                    heldObject.Value = value;
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                    return false;
                }

                readyForHarvest.Value = false;
                TileIndexToShow = 0;
                LidFlapping = true;
                LidFlapTimer = 60f;
                bait.Value = null;
                who.animateOnce(279 + who.FacingDirection);
                who.currentLocation.playSound("fishingRodBend");
                DelayedAction.playSoundAfterDelay("coin", 500);
                Utilities.AddEXP(Game1.getFarmer(who.UniqueMultiplayerID), ModEntry.Config.ExperienceFromWaterShifter);
                Shake = Vector2.Zero;
                ShakeTimer = 0f;
                return true;
            }

            if ( bait.Value == null)
            {
                if (justCheckingForActivity)
                {
                    return true;
                }

                if (Game1.didPlayerJustClickAtAll(ignoreNonMouseHeldInput: true))
                {
                    if (who.addItemToInventoryBool(GetOneNew()))
                    {
                        if (who.isMoving())
                        {
                            Game1.haltAfterCheck = false;
                        }

                        Game1.playSound("coin");
                        location.objects.Remove(tileLocation.Value);
                        return true;
                    }

                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                }
            }

            return false;
        }

        public override void dropItem(GameLocation location, Vector2 origin, Vector2 destination)
        {
            if (fragility == 2)
                return;
            string itemID = "moonslime.Archaeology.water_shifter";
            location.debris.Add(new(new Object(itemID, 1), origin, destination));
        }

        public override void DayUpdate()
        {
            var player = Game1.getFarmer(owner.Value);
            //Player Can get artifacts from the shift if they have the Trowler Profession
            bool flag = player != null && player.HasCustomProfession(Archaeology_Skill.Archaeology10b1);

            //If there is no fiber in the shifter, return and don't do anything.
            //If there is already an item in the shifter, return an don't do anything
            if (!(bait.Value != null) || heldObject.Value != null)
            {
                return;
            }

            TileIndexToShow = 4;
            readyForHarvest.Value = true;
            Random random = Game1.random;

            //Generate the list of loot
            List<string> list =
            [
                //Populate the loot list
                .. ModEntry.WaterSifterLootTable,
            ];

            if (heldObject.Value == null)
            {
                //If flag is true, add in the artifact loot table to the list
                if (flag)
                {
                    //Check each object in the object data list
                    foreach (var SVobject in Game1.objectData)
                    {
                        //Make sure the object is not an error item and then check to see if said item has the Arch type...
                        if (ItemRegistry.GetData(SVobject.Key) != null && SVobject.Value.Type == "Arch")
                        {
                            //... then add it to the list
                            list.Add(SVobject.Key);
                        }
                    }
                }



                //Shuffle the list so it's in a random order!
                list.Shuffle(random);

                //Choose a random item from the list. If the list somehow ended up empty (it shouldn't but just in case), give coal to the player.
                string item = list.RandomChoose(random, "382");
                if (item != null && Game1.objectData.TryGetValue(item, out ObjectData data) && data != null && data.Type != null)
                {
                    int randomQuality = data.Type == "Arch" ? random.Choose(1) : random.Choose(1, 2, 3, 4, 5);
                    heldObject.Value = new Object(item, randomQuality);
                }
            }
        }

        public override void updateWhenCurrentLocation(GameTime time)
        {
            if ( LidFlapping)
            {
                 LidFlapTimer -= time.ElapsedGameTime.Milliseconds;
                if ( LidFlapTimer <= 0f)
                {
                    TileIndexToShow += ((! LidClosing) ? 1 : (-1));
                    if (TileIndexToShow >= 3 && ! LidClosing)
                    {
                         LidClosing = true;
                        TileIndexToShow--;

                    }
                    else if (TileIndexToShow <= 1 &&  LidClosing)
                    {
                         LidClosing = false;
                        TileIndexToShow++;
                         LidFlapping = false;
                        if ( bait.Value != null)
                        {
                            TileIndexToShow = 3;
                        } else
                        {
                            TileIndexToShow = 0;
                        }
                    }

                     LidFlapTimer = 60f;
                }
            }

            if (readyForHarvest.Value &&  heldObject.Value != null)
            {
                 ShakeTimer -= time.ElapsedGameTime.Milliseconds;
                if ( ShakeTimer < 0f)
                {
                     ShakeTimer = Game1.random.Next(2800, 3200);
                }
            }

            if ( ShakeTimer > 2000f)
            {
                 Shake.X = Game1.random.Next(-1, 2);
            }
            else
            {
                 Shake.X = 0f;
            }
        }



        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            GameLocation location = Location;
            if (location == null)
            {
                return;
            }

            if (heldObject.Value != null)
            {
                TileIndexToShow = 4;
            }
            else if (TileIndexToShow == 0)
            {
                TileIndexToShow = 0;
            }

            YBob = (float)(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500.0 + (double)(x * 64)) * 8.0 + 8.0);
            if (YBob <= 0.001f)
            {
                location.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64, 64), 150f, 8, 0, directionOffset.Value + new Vector2(x * 64 + 4, y * 64 + 32), flicker: false, Game1.random.NextBool(), 0.001f, 0.01f, Color.White, 0.75f, 0.003f, 0f, 0f));
            }

            spriteBatch.Draw(ModEntry.Assets.Water_shifter, Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64, y * 64 + (int)YBob)) + Shake, Game1.getSourceRectForStandardTileSheet(ModEntry.Assets.Water_shifter, TileIndexToShow, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ((float)(y * 64) + directionOffset.Y + (float)(x % 4)) / 10000f);
            if (location.waterTiles != null && x < location.waterTiles.waterTiles.GetLength(0) && y < location.waterTiles.waterTiles.GetLength(1) && location.waterTiles.waterTiles[x, y].isWater)
            {
                if (location.waterTiles.waterTiles[x, y].isVisible)
                {
                    spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64 + 4, y * 64 + 48)) + Shake, new Rectangle(location.waterAnimationIndex * 64, 2112 + (((x + y) % 2 != 0) ? ((!location.waterTileFlip) ? 128 : 0) : (location.waterTileFlip ? 128 : 0)), 56, 16 + (int)YBob), location.waterColor.Value, 0f, Vector2.Zero, 1f, SpriteEffects.None, ((float)(y * 64) + directionOffset.Y + (float)(x % 4)) / 9999f);
                }
                else
                {
                    Color a = new Color(135, 135, 135, 215);
                    a = Utility.MultiplyColor(a, location.waterColor.Value);
                    spriteBatch.Draw(Game1.staminaRect, Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64 + 4, y * 64 + 48)) + Shake, null, a, 0f, Vector2.Zero, new Vector2(56f, 16 + (int)YBob), SpriteEffects.None, ((float)(y * 64) + directionOffset.Y + (float)(x % 4)) / 9999f);
                }
            }

            if (readyForHarvest.Value && heldObject.Value != null)
            {
                float num = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64 - 8, (float)(y * 64 - 96 - 16) + num)), new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((y + 1) * 64) / 10000f + 1E-06f + TileLocation.X / 10000f);
                ParsedItemData dataOrErrorItem = ItemRegistry.GetDataOrErrorItem(heldObject.Value.QualifiedItemId);
                spriteBatch.Draw(dataOrErrorItem.GetTexture(), Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64 + 32, (float)(y * 64 - 64 - 8) + num)), dataOrErrorItem.GetSourceRect(), Color.White * 0.75f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float)((y + 1) * 64) / 10000f + 1E-05f + TileLocation.X / 10000f);
            }
        }
    }
}
