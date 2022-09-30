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
using StardewValley;
using Newtonsoft.Json;
using System.Xml.Serialization;
using Netcode;
using System.Collections.Generic;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Illuminate;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.StardustCore.Networking;
using Omegasis.StardustCore.Animations;
using Omegasis.StardustCore.UIUtilities;
using Omegasis.Revitalize.Framework.Constants.ItemCategoryInformation;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles.Json;

namespace Omegasis.Revitalize.Framework.World.Objects.InformationFiles
{
    [XmlType("Mods_Revitalize.Framework.World.Objects.InformationFiles.BasicItemInformation")]
    public class BasicItemInformation : NetObject
    {

        public readonly NetString name = new NetString();

        public readonly NetString id = new NetString();

        public readonly NetString description = new NetString();

        public readonly NetString categoryName = new NetString();

        public readonly NetColor categoryColor = new NetColor();

        public readonly NetInt price = new NetInt();

        public readonly NetInt healthRestoredOnEating = new NetInt();
        public readonly NetInt staminaRestoredOnEating = new NetInt();

        public readonly NetInt fragility = new NetInt();

        public readonly NetBool canBeSetIndoors = new NetBool();

        public readonly NetBool canBeSetOutdoors = new NetBool();

        public readonly NetBool isLamp = new NetBool();

        public readonly NetString locationName = new NetString();

        public readonly NetRef<AnimationManager> netAnimationManager = new NetRef<AnimationManager>();

        public AnimationManager animationManager
        {
            get
            {
                return this.netAnimationManager.Value;
            }
            set
            {
                this.netAnimationManager.Value = value;
            }
        }

        public readonly NetVector2 drawPosition = new NetVector2();

        public readonly NetColor _drawColorBase = new NetColor();

        [XmlIgnore]
        public Color DrawColor
        {
            get
            {
                if (this.dyedColor != null)

                    if (this.dyedColor.color.A != 0)
                        return this.dyedColor.getBlendedColor(this._drawColorBase);
                return this._drawColorBase;
            }
            set
            {
                this._drawColorBase.Value = value;
            }
        }

        public readonly NetBool ignoreBoundingBox = new NetBool();

        public NetRef<InventoryManager> netInventory = new();

        public InventoryManager inventory
        {
            get
            {
                return this.netInventory.Value;
            }
            set
            {
                this.netInventory.Value = value;
            }
        }

        public NetRef<LightManager> netLightManager = new();

        public LightManager lightManager
        {
            get
            {
                return this.netLightManager.Value;
            }
            set
            {
                this.netLightManager.Value = value;
            }
        }

        public readonly NetEnum<Enums.Direction> facingDirection = new NetEnum<Enums.Direction>();

        public readonly NetInt shakeTimer = new NetInt();

        public readonly NetBool alwaysDrawAbovePlayer = new NetBool();

        public readonly NetRef<NamedColor> netDyedColor = new();

        public NamedColor dyedColor
        {
            get
            {
                return this.netDyedColor.Value;
            }
            set
            {
                this.netDyedColor.Value = value;
            }
        }

        /// <summary>
        /// The dimensions for the game's bounding box in the number of TILES. So a Vector2(1,1) would have 1 tile width and 1 tile height. There are also no "fractional" vounding boxes as the game counts whole tiles, so work (aka math) is needed to be done to ensure that the player can properly go infront of and behind objects.
        /// </summary>
        public readonly NetVector2 boundingBoxTileDimensions = new NetVector2();

        /// <summary>
        /// The drawing offset of the object IN TILES.
        /// </summary>
        public readonly NetVector2 drawOffset = new NetVector2();

        [JsonIgnore]
        public bool requiresUpdate;
        public BasicItemInformation()
        {
            this.name.Value = "";
            this.description.Value = "";
            this.categoryName.Value = "";
            this.categoryColor.Value = new Color(0, 0, 0);
            this.price.Value = 0;
            this.staminaRestoredOnEating.Value = -300;
            this.healthRestoredOnEating.Value = -300;
            this.canBeSetIndoors.Value = false;
            this.canBeSetOutdoors.Value = false;

            this.animationManager = new AnimationManager();
            this.drawPosition.Value = Vector2.Zero;
            this.DrawColor = Color.White;
            this.inventory = new InventoryManager();
            this.lightManager = new LightManager();

            this.facingDirection.Value = Enums.Direction.Down;
            this.id.Value = "";
            this.shakeTimer.Value = 0;
            this.alwaysDrawAbovePlayer.Value = false;

            this.ignoreBoundingBox.Value = false;
            this.boundingBoxTileDimensions.Value = new Vector2(1, 1);
            this.drawOffset.Value = new Vector2(0, 0);
            this.dyedColor = new NamedColor();

            this.initializeNetFields();
        }

        public BasicItemInformation(string PathToBasicItemInformationFile, InventoryManager Inventory = null, LightManager Lights = null, NamedColor DyedColor = null):this(JsonUtilities.ReadJsonFile<JsonBasicItemInformation>(PathToBasicItemInformationFile), Inventory, Lights,DyedColor)
        {

        }

        public BasicItemInformation(JsonBasicItemInformation jsonItemInformation, InventoryManager Inventory = null, LightManager Lights = null, NamedColor DyedColor = null)
        {
            this.name.Value = jsonItemInformation.name;
            this.id.Value = jsonItemInformation.id;
            this.description.Value = jsonItemInformation.description;

            ItemCategory itemCategory = ItemCategories.GetItemCategory(jsonItemInformation.categoryId);
            this.categoryName.Value = itemCategory.name;
            this.categoryColor.Value = itemCategory.color;

            this.price.Value = jsonItemInformation.sellingPrice;

            this.staminaRestoredOnEating.Value = jsonItemInformation.staminaRestoredOnEating;
            this.healthRestoredOnEating.Value = jsonItemInformation.healthRestoredOnEating;

            this.canBeSetIndoors.Value = jsonItemInformation.canBeSetIndoors;
            this.canBeSetOutdoors.Value = jsonItemInformation.canBeSetOutdoors;
            this.fragility.Value = jsonItemInformation.fraglility;
            this.isLamp.Value = false;

            this.animationManager = jsonItemInformation.animationManager.toAnimationManager();

            this.drawPosition.Value = Vector2.Zero;
            this.DrawColor = jsonItemInformation.drawColor;
            this.drawOffset.Value = jsonItemInformation.drawTileOffset;

            this.ignoreBoundingBox.Value = jsonItemInformation.ignoreBoundingBox;
            this.boundingBoxTileDimensions.Value = jsonItemInformation.boundingBoxTileDimensions;



            this.inventory = Inventory ?? new InventoryManager();
            this.lightManager = Lights ?? new LightManager();

            this.facingDirection.Value = Enums.Direction.Down;
            this.shakeTimer.Value = 0;

            this.alwaysDrawAbovePlayer.Value = false;

            if (DyedColor != null)
                this.dyedColor = DyedColor.getCopy();
            else
                this.dyedColor = new NamedColor("", new Color(0, 0, 0, 0), Enums.DyeBlendMode.Blend, 0.5f);

            this.initializeNetFields();
        }
       
        public BasicItemInformation(string Name, string Id, string Description, string CategoryName, Color CategoryColor, int Fragility, bool IsLamp, int Price, AnimationManager animationManager, bool IgnoreBoundingBox, Vector2 BoundingBoxTileDimensions, Vector2 BoundingBoxTileOffset ,InventoryManager Inventory = null, LightManager Lights = null, bool AlwaysDrawAbovePlayer = false, NamedColor DyedColor = null) : this(Name, Id, Description, CategoryName, CategoryColor, -300, -300, Fragility, IsLamp, Price, true, true, animationManager, Color.White, IgnoreBoundingBox, BoundingBoxTileDimensions, BoundingBoxTileOffset,Inventory, Lights, AlwaysDrawAbovePlayer, DyedColor)
        {

        }


        public BasicItemInformation(string name, string id, string description, string categoryName, Color categoryColor, int staminaRestoredOnEating, int healthRestoredOnEating, int fragility, bool isLamp, int price, bool canBeSetOutdoors, bool canBeSetIndoors, AnimationManager animationManager, Color drawColor, bool ignoreBoundingBox, Vector2 BoundingBoxTileDimensions, Vector2 DrawTileOffset ,InventoryManager Inventory, LightManager Lights, bool AlwaysDrawAbovePlayer = false, NamedColor DyedColor = null)
        {
            this.name.Value = name;
            this.id.Value = id;
            this.description.Value = description;
            this.categoryName.Value = categoryName;
            this.categoryColor.Value = categoryColor;
            this.price.Value = price;
            this.staminaRestoredOnEating.Value = staminaRestoredOnEating;
            this.healthRestoredOnEating.Value = healthRestoredOnEating;

            this.canBeSetOutdoors.Value = canBeSetOutdoors;
            this.canBeSetIndoors.Value = canBeSetIndoors;
            this.fragility.Value = fragility;
            this.isLamp.Value = isLamp;

            this.animationManager = animationManager;

            this.drawPosition.Value = Vector2.Zero;
            this.DrawColor = drawColor;
            this.ignoreBoundingBox.Value = ignoreBoundingBox;
            this.boundingBoxTileDimensions.Value = BoundingBoxTileDimensions;
            this.drawOffset.Value = DrawTileOffset;
            this.inventory = Inventory ?? new InventoryManager();
            this.lightManager = Lights ?? new LightManager();
            this.facingDirection.Value = Enums.Direction.Down;
            this.shakeTimer.Value = 0;

            this.alwaysDrawAbovePlayer.Value = AlwaysDrawAbovePlayer;

            if (DyedColor != null)
                this.dyedColor = DyedColor.getCopy();
            else
                this.dyedColor = new NamedColor("", new Color(0, 0, 0, 0), Enums.DyeBlendMode.Blend, 0.5f);


            this.initializeNetFields();
        }

        /// <summary>
        /// Gets an x offset for shaking an object. Source code used from game.
        /// </summary>
        /// <returns></returns>
        public int shakeTimerOffset()
        {
            return this.shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0;
        }

        /// <summary>
        /// Returns a copy of this object.
        /// </summary>
        /// <returns></returns>
        public BasicItemInformation Copy()
        {
            return new BasicItemInformation(
                this.name,
                this.id,
                this.description,
                this.categoryName,
                this.categoryColor,
                this.staminaRestoredOnEating,
                this.healthRestoredOnEating,
                this.fragility,
                this.isLamp,
                this.price,
                this.canBeSetOutdoors,
                this.canBeSetIndoors,
                this.animationManager.Copy(),
                this.DrawColor,
                this.ignoreBoundingBox,
                this.boundingBoxTileDimensions,
                this.drawOffset ,
                this.inventory.Copy(),
                this.lightManager.Copy(),
                this.alwaysDrawAbovePlayer,
                this.dyedColor.getCopy());
        }


        /// <summary>
        /// Gets the name attached to the dyed color.
        /// </summary>
        /// <returns></returns>
        public string getDyedColorName()
        {
            if (this.dyedColor == null)
                return "";
            if (this.dyedColor.color.A == 0)
                return "";
            else
                return this.dyedColor.name;
        }

        /// <summary>
        /// Gets the netfields that should be synced across server/clients.
        /// </summary>
        /// <returns></returns>

        protected override void initializeNetFields()
        {
            this.NetFields.AddFields(this.name,

                this.id,
                this.description,
                this.categoryName,
                this.categoryColor,
                this.price,
                this.healthRestoredOnEating,
                this.staminaRestoredOnEating,
                this.fragility,
                this.canBeSetIndoors,
                this.canBeSetOutdoors,
                this.isLamp,
                this.locationName,
                this.drawPosition,
                this._drawColorBase,
                this.ignoreBoundingBox,
                this.facingDirection,
                this.shakeTimer,
                this.alwaysDrawAbovePlayer,
                this.boundingBoxTileDimensions,
                this.drawOffset
                );

            this.NetFields.AddField(this.netAnimationManager);
            this.NetFields.AddFields(this.netInventory);
            this.NetFields.AddField(this.netLightManager);
            this.NetFields.AddFields(this.netDyedColor);
        }

    }
}
