/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardustCore.Animations;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardustCore.UIUtilities;
using Newtonsoft.Json;
using Revitalize.Framework;
using Revitalize.Framework.Managers;
using Revitalize.Framework.Illuminate;

namespace Revitalize.Framework.World.Objects.InformationFiles
{
    public class BasicItemInformation
    {
        public string name;

        public string id;

        public string description;

        public string categoryName;

        public Color categoryColor;

        public int price;

        public int healthRestoredOnEating;
        public int staminaRestoredOnEating;

        public int fragility;

        public bool canBeSetIndoors;

        public bool canBeSetOutdoors;

        public bool isLamp;

        public string locationName;

        public AnimationManager animationManager;

        public Vector2 drawPosition;

        private Color _drawColor;
        public Color DrawColor
        {
            get
            {
                if (this.dyedColor != null)
                {
                    if (this.dyedColor.color != null)
                    {
                        if (this.dyedColor.color.A != 0)
                        {
                            return this.colorManager.getBlendedColor(this._drawColor, this.dyedColor.color);
                            //return new Color( (this._drawColor.R + this._dyedColor.color.R)/2, (this._drawColor.G + this._dyedColor.color.G)/2, (this._drawColor.B + this._dyedColor.color.B)/2, 255);
                            //return new Color(this._drawColor.R * this._dyedColor.color.R, this._drawColor.G * this._dyedColor.color.G, this._drawColor.B * this._dyedColor.color.B, 255);
                        }
                    }
                }
                return this._drawColor;
            }
            set
            {
                this._drawColor = value;
            }
        }


        public bool ignoreBoundingBox;

        public InventoryManager inventory;

        public LightManager lightManager;

        public Enums.Direction facingDirection;

        public int shakeTimer;

        public bool alwaysDrawAbovePlayer;

        public NamedColor dyedColor;

        public ColorManager colorManager;

        /// <summary>
        /// The dimensions for the game's bounding box in the number of TILES. So a Vector2(1,1) would have 1 tile width and 1 tile height.
        /// </summary>
        public Vector2 boundingBoxTileDimensions;

        [JsonIgnore]
        public bool requiresUpdate;
        public BasicItemInformation()
        {
            this.name = "";
            this.description = "";
            this.categoryName = "";
            this.categoryColor = new Color(0, 0, 0);
            this.price = 0;
            this.staminaRestoredOnEating = -300;
            this.healthRestoredOnEating = -300;
            this.canBeSetIndoors = false;
            this.canBeSetOutdoors = false;

            this.animationManager = new AnimationManager();
            this.drawPosition = Vector2.Zero;
            this.DrawColor = Color.White;
            this.inventory = new InventoryManager();
            this.lightManager = new LightManager();

            this.facingDirection = Enums.Direction.Down;
            this.id = "";
            this.shakeTimer = 0;
            this.alwaysDrawAbovePlayer = false;
            this.colorManager = new ColorManager(Enums.DyeBlendMode.Blend, 0.5f);

            this.ignoreBoundingBox = false;
            this.boundingBoxTileDimensions = new Vector2(1, 1);
        }

        public BasicItemInformation(string name, string id, string description, string categoryName, Color categoryColor,int staminaRestoredOnEating,int healthRestoredOnEating ,int fragility, bool isLamp, int price, bool canBeSetOutdoors, bool canBeSetIndoors, Texture2D texture, AnimationManager animationManager, Color drawColor, bool ignoreBoundingBox, Vector2 BoundingBoxTileDimensions ,InventoryManager Inventory, LightManager Lights,bool AlwaysDrawAbovePlayer=false,NamedColor DyedColor=null, ColorManager ColorManager=null)
        {
            this.name = name;
            this.id = id;
            this.description = description;
            this.categoryName = categoryName;
            this.categoryColor = categoryColor;
            this.price = price;
            this.staminaRestoredOnEating = staminaRestoredOnEating;
            this.healthRestoredOnEating = healthRestoredOnEating;

            this.canBeSetOutdoors = canBeSetOutdoors;
            this.canBeSetIndoors = canBeSetIndoors;
            this.fragility = fragility;
            this.isLamp = isLamp;

            this.animationManager = animationManager;
            if (this.animationManager.IsNull)
            {
                this.animationManager = new AnimationManager(new Texture2DExtended(), new Animation(new Rectangle(0, 0, 16, 16)), false);
                this.animationManager.getExtendedTexture().texture = texture;
            }

            this.drawPosition = Vector2.Zero;
            this.DrawColor = drawColor;
            this.ignoreBoundingBox = ignoreBoundingBox;
            this.boundingBoxTileDimensions = BoundingBoxTileDimensions;
            this.inventory = Inventory ?? new InventoryManager();
            this.lightManager = Lights ?? new LightManager();
            this.facingDirection = Enums.Direction.Down;
            this.shakeTimer = 0;

            this.alwaysDrawAbovePlayer = AlwaysDrawAbovePlayer;

            this.dyedColor = DyedColor ?? new NamedColor("", new Color(0, 0, 0, 0));
            this.colorManager = ColorManager ?? new ColorManager(Enums.DyeBlendMode.Blend, 0.5f);
        }

        /// <summary>
        /// Gets an x offset for shaking an object. Source code used from game.
        /// </summary>
        /// <returns></returns>
        public int shakeTimerOffset()
        {
            return (this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0);
        }

        /// <summary>
        /// Returns a copy of this object.
        /// </summary>
        /// <returns></returns>
        public BasicItemInformation Copy()
        {
            return new BasicItemInformation(this.name, this.id,this.description, this.categoryName, this.categoryColor, this.staminaRestoredOnEating,this.healthRestoredOnEating ,this.fragility, this.isLamp, this.price, this.canBeSetOutdoors, this.canBeSetIndoors, this.animationManager.getTexture(), this.animationManager.Copy(), this.DrawColor, this.ignoreBoundingBox,this.boundingBoxTileDimensions ,this.inventory.Copy(), this.lightManager.Copy(),this.alwaysDrawAbovePlayer,this.dyedColor,this.colorManager);
        }


        /// <summary>
        /// Gets the name attached to the dyed color.
        /// </summary>
        /// <returns></returns>
        public string getDyedColorName()
        {
            if (this.dyedColor == null)
            {
                return "";
            }
            if (this.dyedColor.color.A == 0)
            {
                return "";
            }
            else
            {
                return this.dyedColor.name;
            }
        }


        
    }
}
