/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Monsters;
using System;
using System.Xml.Serialization;
using xTile.Layers;

namespace FarmTypeManager.Monsters
{
    /// <summary>A subclass of Stardew's Duggy class, adjusted for use by this mod.</summary>
    public class DuggyFTM : Duggy, ICustomDamage
    {
        [XmlElement("FTM_customDamage")]
        public readonly NetInt customDamage = new NetInt(8); //default set to mimic hardcoded values

        /// <summary>A customizable value for DamageToFarmer, used to preserve it during temporary damage changes.</summary>
        [XmlIgnore]
        public int CustomDamage
        {
            get
            {
                return customDamage.Value;
            }

            set
            {
                customDamage.Value = value;
            }
        }

        /// <summary>Creates an instance of Stardew's Duggy class, but with adjustments made for this mod.</summary>
        public DuggyFTM()
            : base()
        {

        }

        /// <summary>Creates an instance of Stardew's Duggy class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        public DuggyFTM(Vector2 position)
            : base(position)
        {

        }

        /// <summary>Creates an instance of Stardew's Duggy class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="magmaDuggy">True if this should be a Magma Duggy. NOTE: Currently redundant; this constructor always produces a Magma Duggy.</param>
        public DuggyFTM(Vector2 position, bool magmaDuggy)
            : base(position, magmaDuggy)
        {

        }

        /// <summary>This method adds the CustomDamage setting to to the monster's list of net fields for multiplayer functionality.</summary>
        protected override void initNetFields()
        {
            base.initNetFields();
            this.NetFields.AddField(customDamage);
        }

        //This override fixes the following Duggy behavioral bugs:
        // * error that prevented multiplayer farmhands from loading the game while these monsters exist (null location/map/layer data)
        public override void update(GameTime time, GameLocation location)
        {
            if (this.invincibleCountdown > 0)
            {
                this.glowingColor = Color.Cyan;
                this.invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
                if (this.invincibleCountdown <= 0)
                    this.stopGlowing();
            }
            if (!location.farmers.Any())
                return;
            this.behaviorAtGameTick(time);

            Layer backLayer = location?.map?.RequireLayer("Back"); //if this monster's location exists and is loaded, get the back layer
            if (backLayer != null) //if the layer exists
            {
                //perform the original removal check
                if ((double)this.Position.X < 0.0 || (double)this.Position.X > (double)(backLayer.LayerWidth * 64) || ((double)this.Position.Y < 0.0 || (double)this.Position.Y > (double)(backLayer.LayerHeight * 64)))
                    location.characters.Remove(this);
            }

            this.updateGlow();
        }

        //This override fixes the following Duggy behavioral bugs:
        // * permanently editing tiles' TileIndex (attempting to display the "empty hole" sprite)
        // * failing to un-burrow in most game locations
        // * using hard-coded damage values, making it non-customizable
        public override void behaviorAtGameTick(GameTime time)
        {
            Monster_behaviorAtGameTick(time); //call this manual implementation rather than the "base" method, due to the way nested subclasses work
            this.isEmoting = false;
            this.Sprite.loop = false;
            Rectangle boundingBox = this.GetBoundingBox();
            if (this.Sprite.currentFrame < 4)
            {
                boundingBox.Inflate(128, 128);
                if (!this.IsInvisible || boundingBox.Contains(Player.StandingPixel))
                {
                    if (this.IsInvisible)
                    {
                        if (currentLocation?.map != null) //if the player has access to the current location's map (a necessary check for farmhands in some locations)
                        {
                            //only check for the NPCBarrier flag, ignoring the base Duggy's other movement restrictions
                            if (currentLocation.map.RequireLayer("Back").Tiles[Player.TilePoint.X, Player.TilePoint.Y].Properties.ContainsKey("NPCBarrier"))
                                return;
                        }
                        this.Position = new Vector2(this.Player.Position.X, this.Player.Position.Y + (float)this.Player.Sprite.SpriteHeight - (float)this.Sprite.SpriteHeight);
                        this.currentLocation.localSound(nameof(Duggy));
                        this.Position = this.Player.Tile * 64f;
                    }
                    this.IsInvisible = false;
                    this.Sprite.interval = 100f;
                    this.Sprite.AnimateDown(time);
                }
            }
            if (this.Sprite.currentFrame >= 4 && this.Sprite.currentFrame < 8)
            {
                boundingBox.Inflate(-128, -128);
                this.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 8, false, this);
                this.Sprite.AnimateRight(time);
                this.Sprite.interval = 220f;
                this.DamageToFarmer = CustomDamage; //use customizable damage instead of hardcoded values
            }
            if (this.Sprite.currentFrame >= 8)
                this.Sprite.AnimateUp(time);
            if (this.Sprite.currentFrame < 10)
                return;
            this.IsInvisible = true;
            this.Sprite.currentFrame = 0;
            this.DamageToFarmer = 0;
            //skip the base Duggy's tile alterations
        }

        /// <summary>Except where commented, this is a copy of "Monster.behaviorAtGameTick", used to implement this monster's "base.behaviorAtGameTick" call.</summary>
        private void Monster_behaviorAtGameTick(GameTime time)
        {
            if (timeBeforeAIMovementAgain > 0f)
            {
                timeBeforeAIMovementAgain -= time.ElapsedGameTime.Milliseconds;
            }
            if (Player?.isRafting != true || !withinPlayerThreshold(4)) //check for null on Player due to reported errors (not necessarily FTM-specific)
            {
                return;
            }
            IsWalkingTowardPlayer = false;
            Point monsterPixel = StandingPixel;
            Point playerPixel = Player.StandingPixel;
            if (Math.Abs(playerPixel.Y - monsterPixel.Y) > 192)
            {
                if (playerPixel.X - monsterPixel.X > 0)
                {
                    SetMovingLeft(b: true);
                }
                else
                {
                    SetMovingRight(b: true);
                }
            }
            else if (playerPixel.Y - monsterPixel.Y > 0)
            {
                SetMovingUp(b: true);
            }
            else
            {
                SetMovingDown(b: true);
            }
            MovePosition(time, Game1.viewport, currentLocation);
        }
    }
}
