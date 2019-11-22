using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using Microsoft.Xna.Framework.Graphics;
using xTile.Layers;
using Netcode;

namespace FarmTypeManager.Monsters
{
    /// <summary>A subclass of Stardew's Duggy class, adjusted for use by this mod.</summary>
    class DuggyFTM : Duggy, ICustomDamage
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

        /// <summary>This method adds the CustomDamage setting to to the monster's list of net fields for multiplayer functionality.</summary>
        protected override void initNetFields()
        {
            base.initNetFields();
            this.NetFields.AddField(customDamage);
        }

        //this override fixes the following Duggy behavioral bugs:
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
                if (!this.IsInvisible || boundingBox.Contains(this.Player.getStandingX(), this.Player.getStandingY()))
                {
                    if (this.IsInvisible)
                    {
                        if (currentLocation?.map != null) //if the player has access to the current location's map (a necessary check for farmhands in some locations)
                        {
                            //only check for the NPCBarrier flag, ignoring the base Duggy's other movement restrictions
                            if (this.currentLocation.map.GetLayer("Back").Tiles[(int)this.Player.getTileLocation().X, (int)this.Player.getTileLocation().Y].Properties.ContainsKey("NPCBarrier"))
                                return;
                        }
                        this.Position = new Vector2(this.Player.Position.X, this.Player.Position.Y + (float)this.Player.Sprite.SpriteHeight - (float)this.Sprite.SpriteHeight);
                        this.currentLocation.localSound(nameof(Duggy));
                        this.Position = this.Player.getTileLocation() * 64f;
                    }
                    this.IsInvisible = false;
                    this.Sprite.interval = 100f;
                    this.Sprite.AnimateDown(time, 0, "");
                }
            }
            if (this.Sprite.currentFrame >= 4 && this.Sprite.currentFrame < 8)
            {
                boundingBox.Inflate((int)sbyte.MinValue, (int)sbyte.MinValue);
                this.currentLocation.isCollidingPosition(boundingBox, Game1.viewport, false, 8, false, (Character)this);
                this.Sprite.AnimateRight(time, 0, "");
                this.Sprite.interval = 220f;
                this.DamageToFarmer = CustomDamage; //use customizable damage instead of hardcoded values
            }
            if (this.Sprite.currentFrame >= 8)
                this.Sprite.AnimateUp(time, 0, "");
            if (this.Sprite.currentFrame < 10)
                return;
            this.IsInvisible = true;
            this.Sprite.currentFrame = 0;
            this.DamageToFarmer = 0;
            //skip the base Duggy's tile alterations
        }

        /// <summary>This is a copy of the virtual method "Monster.behaviorAtGameTick", used to implement the base Duggy class's "base.behaviorAtGameTick" call.</summary>
        private void Monster_behaviorAtGameTick(GameTime time)
        {
            if ((double)this.timeBeforeAIMovementAgain > 0.0)
                this.timeBeforeAIMovementAgain -= (float)time.ElapsedGameTime.Milliseconds;
            if (!this.Player.isRafting || !this.withinPlayerThreshold(4))
                return;
            Microsoft.Xna.Framework.Rectangle boundingBox1 = this.Player.GetBoundingBox();
            int y1 = boundingBox1.Center.Y;
            boundingBox1 = this.GetBoundingBox();
            int y2 = boundingBox1.Center.Y;
            if (Math.Abs(y1 - y2) > 192)
            {
                Microsoft.Xna.Framework.Rectangle boundingBox2 = this.Player.GetBoundingBox();
                int x1 = boundingBox2.Center.X;
                boundingBox2 = this.GetBoundingBox();
                int x2 = boundingBox2.Center.X;
                if (x1 - x2 > 0)
                    this.SetMovingLeft(true);
                else
                    this.SetMovingRight(true);
            }
            else
            {
                Microsoft.Xna.Framework.Rectangle boundingBox2 = this.Player.GetBoundingBox();
                int y3 = boundingBox2.Center.Y;
                boundingBox2 = this.GetBoundingBox();
                int y4 = boundingBox2.Center.Y;
                if (y3 - y4 > 0)
                    this.SetMovingUp(true);
                else
                    this.SetMovingDown(true);
            }
            this.MovePosition(time, Game1.viewport, this.currentLocation);
        }

        //this override fixes the following duggy behavioral bugs:
        // * preventing multiplayer farmhands from loading the game while they exist (caused by null location/map/layer data)
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

            Layer backLayer = location?.map?.GetLayer("Back"); //get the "Back" layer if it exists
            if (backLayer != null) //if the layer exists
            {
                //perform the original layer check
                if ((double)this.Position.X < 0.0 || (double)this.Position.X > (double)(backLayer.LayerWidth * 64) || ((double)this.Position.Y < 0.0 || (double)this.Position.Y > (double)(backLayer.LayerHeight * 64)))
                    location.characters.Remove((NPC)this);
            }

            this.updateGlow();
        }
    }
}
