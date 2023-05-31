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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Illuminate;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.StardustCore.Animations;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.World.Debris;
using StardewValley.Menus;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using StardewValley.TerrainFeatures;
using StardewValley.Locations;

namespace Omegasis.Revitalize.Framework.World.Objects
{

    /*
     * 
     * NOTES: Calling this.performRemoveAction(this.tileLocation, location); is NOT the same as calling GameLocation.objects.Remove(). Perform remove action cleans up the item before being removed, but has no actual deletion/removal logic from the game world itself.
     * 
     * 
     * Current issue: When right clicking the object is picked up. This is currently not intended.
     * 
     */


    /// <summary>
    /// A base class that is to be extended by other implementations of objects.
    ///
    /// Clicking to remove and click place are bound to the samething. Need to find a way to change that.
    /// Bounding boxes work, but not for clicking to remove. Why is that?
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.CustomObject")]
    public class CustomObject : StardewValley.Objects.Furniture, ICommonObjectInterface, ILightManagerProvider, ICustomModObject
    {
        public bool isCurrentLocationAStructure;

        public readonly NetRef<BasicItemInformation> netBasicItemInformation = new NetRef<BasicItemInformation>();

        [XmlElement("basicItemInfo")]
        public BasicItemInformation basicItemInformation
        {
            get
            {
                return this.netBasicItemInformation.Value;
            }
            set
            {
                this.netBasicItemInformation.Value = value;
            }
        }

        [XmlIgnore]
        public string Id
        {
            get
            {
                return this.basicItemInformation.id.Value;
            }
        }

        [XmlIgnore]
        public AnimationManager AnimationManager
        {
            get
            {
                if (this.basicItemInformation == null) return null;
                if (this.basicItemInformation.animationManager == null) return null;
                return this.basicItemInformation.animationManager;
            }
        }

        [XmlIgnore]
        public Texture2D CurrentTextureToDisplay
        {

            get
            {
                if (this.AnimationManager == null) return null;
                return this.AnimationManager.getTexture();
            }
        }

        /// <summary>
        /// The internal name used to reference this item in code.
        /// </summary>
        public override string Name
        {
            get
            {
                if (this.basicItemInformation == null) return null;
                return this.basicItemInformation.name.Value;
            }
            set
            {
                if (this.basicItemInformation != null)
                {
                    this.basicItemInformation.name.Value = value;
                }
            }


        }

        /// <summary>
        /// The name displayed to the player for the object.
        /// </summary>
        public override string DisplayName
        {
            get
            {
                if (this.basicItemInformation == null) return null;

                //Potentially get an overriden display name for certain objects depending on if I ever implement the renaming feature.
                string displayName = "";
                if (string.IsNullOrEmpty(this.basicItemInformation.name.Value))
                {
                   displayName = JsonContentPackUtilities.LoadItemDisplayName(this.Id, false);
                }
                if (!string.IsNullOrEmpty(displayName))
                {
                    this.basicItemInformation.name.Value = displayName;
                    return displayName;
                }
                if(string.IsNullOrEmpty(this.basicItemInformation.name.Value) && string.IsNullOrEmpty(displayName))
                {
                    throw new JsonContentLoadingException(string.Format("The given item id {0} does not have a registered value for display strings! A file can be created under the ModAssets/Strings/Objects/DisplayStrings directory with the given info.",this.Id));
                }

                return this.basicItemInformation.name.Value;
            }
            set
            {
                if (this.basicItemInformation != null)
                {
                    this.basicItemInformation.name.Value = value;
                }
            }
        }

        public LightManager LightManager { get => this.basicItemInformation.lightManager; set => this.basicItemInformation.lightManager=value; }

        /// <summary>
        /// Since objects are counted as both furniture and objects, 2 day update ticks happen for them, so we need to add a buffer for when the day updates trigger.
        /// </summary>
        public NetInt dayUpdateCounter = new NetInt();

        public CustomObject()
        {
            this.basicItemInformation = new BasicItemInformation();

            this.furniture_type.Value = Furniture.other;
            this.bigCraftable.Value = true;
            this.Type = "interactive";
            this.name = this.Name;


            IReflectedField<int> placementRestriction = RevitalizeModCore.ModHelper.Reflection.GetField<int>(this, "_placementRestriction");
            placementRestriction.SetValue(2);
            this.Fragility = 0;

            this.initializeNetFieldsPostConstructor();
        }

        public CustomObject(BasicItemInformation BasicItemInfo)
        {
            this.basicItemInformation = BasicItemInfo;
            this.bigCraftable.Value = true;
            this.furniture_type.Value = Furniture.other;
            this.Type = "interactive";
            this.name = this.Name;

            IReflectedField<int> placementRestriction = RevitalizeModCore.ModHelper.Reflection.GetField<int>(this, "_placementRestriction");
            placementRestriction.SetValue(2);
            this.Fragility = 0;

            this.initializeNetFieldsPostConstructor();
        }

        public CustomObject(BasicItemInformation BasicItemInfo, int StackSize = 1)
        {
            this.basicItemInformation = BasicItemInfo;
            this.TileLocation = Vector2.Zero;
            this.Stack = StackSize;
            this.bigCraftable.Value = true;
            this.furniture_type.Value = Furniture.other;
            this.Type = "interactive";
            this.name = this.Name;
            this.Fragility = 0;

            IReflectedField<int> placementRestriction = RevitalizeModCore.ModHelper.Reflection.GetField<int>(this, "_placementRestriction");
            placementRestriction.SetValue(2);

            this.initializeNetFieldsPostConstructor();
        }

        public CustomObject(BasicItemInformation BasicItemInfo, Vector2 TileLocation)
        {
            this.basicItemInformation = BasicItemInfo;
            this.TileLocation = TileLocation;
            this.bigCraftable.Value = true;
            this.furniture_type.Value = Furniture.other;
            this.Type = "interactive";
            this.name = this.Name;
            this.Fragility = 0;

            IReflectedField<int> placementRestriction = RevitalizeModCore.ModHelper.Reflection.GetField<int>(this, "_placementRestriction");
            placementRestriction.SetValue(2);

            this.initializeNetFieldsPostConstructor();
        }
        public CustomObject(BasicItemInformation BasicItemInfo, Vector2 TileLocation, int StackSize = 1)
        {
            this.basicItemInformation = BasicItemInfo;
            this.TileLocation = TileLocation;
            this.Stack = StackSize;
            this.bigCraftable.Value = true;
            this.furniture_type.Value = Furniture.other;
            this.Type = "interactive";
            this.name = this.Name;
            this.Fragility = 0;

            IReflectedField<int> placementRestriction = RevitalizeModCore.ModHelper.Reflection.GetField<int>(this, "_placementRestriction");
            placementRestriction.SetValue(2);

            this.initializeNetFieldsPostConstructor();
        }

        /// <summary>
        /// Initializes NetFields to send information for multiplayer after all of the constructor initialization for this object has taken place.
        /// </summary>
        protected virtual void initializeNetFieldsPostConstructor()
        {
            if (this.basicItemInformation != null)
            {
                this.NetFields.AddFields(this.netBasicItemInformation);
            }

        }

        /// <summary>
        /// What happens when the item is placed into the game world.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool performDropDownAction(Farmer who)
        {
            Game1.showRedMessage("Drop object into game world?");
            return false;
        }


        public override void actionOnPlayerEntry()
        {
            base.actionOnPlayerEntry();
        }

        public override void actionWhenBeingHeld(Farmer who)
        {
            base.actionWhenBeingHeld(who);
        }

        public override bool actionWhenPurchased()
        {
            return base.actionWhenPurchased();
        }

        public override void actionWhenStopBeingHeld(Farmer who)
        {
            base.actionWhenStopBeingHeld(who);
        }

        public override void ApplySprinkler(GameLocation location, Vector2 tile)
        {
            base.ApplySprinkler(location, tile);
        }

        public override bool canBeDropped()
        {
            return true;
        }

        public override bool canBeGivenAsGift()
        {
            return base.canBeGivenAsGift();
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            //Replace with SDV 1.6 code when that releases. I'll know because this method calling signiture WILL break.

            if (!this.isGroundFurniture())
            {
                tile.Y = this.GetModifiedWallTilePosition(l, (int)tile.X, (int)tile.Y);
            }
            for (int x = 0; x < base.boundingBox.Width / 64; x++)
            {
                for (int y = 0; y < base.boundingBox.Height / 64; y++)
                {
                    Vector2 nonTile = tile * 64f + new Vector2(x, y) * 64f;
                    nonTile.X += 32f;
                    nonTile.Y += 32f;
                    foreach (Furniture f in l.furniture)
                    {
                        if ((int)f.furniture_type == 11 && f.getBoundingBox(f.tileLocation).Contains((int)nonTile.X, (int)nonTile.Y) && f.heldObject.Value == null && this.getTilesWide() == 1 && this.getTilesHigh() == 1)
                        {
                            return true;
                        }
                        if (((int)f.furniture_type != 12 || (int)this.furniture_type == 12) && f.getBoundingBox(f.tileLocation).Contains((int)nonTile.X, (int)nonTile.Y) && !f.AllowPlacementOnThisTile((int)tile.X + x, (int)tile.Y + y))
                        {
                            return false;
                        }
                    }
                    Vector2 currentTile = tile + new Vector2(x, y);
                    if (l.Objects.ContainsKey(currentTile))
                    {
                        return false;
                    }
                    if (l.getLargeTerrainFeatureAt((int)currentTile.X, (int)currentTile.Y) != null)
                    {
                        return false;
                    }
                    if (l.terrainFeatures.ContainsKey(currentTile) && l.terrainFeatures[currentTile] is Tree)
                    {
                        return false;
                    }
                    if (l.isTerrainFeatureAt((int)currentTile.X, (int)currentTile.Y))
                    {
                        return false;
                    }
                }
            }
            Rectangle bounding_box = new Rectangle(base.boundingBox.Value.X, base.boundingBox.Value.Y, base.boundingBox.Value.Width, base.boundingBox.Value.Height);
            bounding_box.X = (int)tile.X * 64;
            bounding_box.Y = (int)tile.Y * 64;
            if (!this.isPassable())
            {
                foreach (Farmer farmer in l.farmers)
                {
                    if (farmer.GetBoundingBox().Intersects(bounding_box))
                    {
                        return false;
                    }
                }
                foreach (NPC character in l.characters)
                {
                    if (character.GetBoundingBox().Intersects(bounding_box))
                    {
                        return false;
                    }
                }
            }
            if (this.GetAdditionalFurniturePlacementStatus(l, (int)tile.X * 64, (int)tile.Y * 64) != 0)
            {
                return false;
            }


            if (l.getObjectAtTile((int)tile.X, (int)tile.Y) != null)
            {
                return false;
            }
            return true;
        }

        public override int GetAdditionalFurniturePlacementStatus(GameLocation location, int x, int y, Farmer who = null)
        {
            //Migrtae this code once SDV 1.6 is released.

            Point point = new Point(x / 64, y / 64);
            this.tileLocation.Value = new Vector2(point.X, point.Y);
            bool flag = false;
            if ((int)this.furniture_type == 6 || (int)this.furniture_type == 17 || (int)this.furniture_type == 13 || (int)this.parentSheetIndex == 1293)
            {
                int num = (((int)this.parentSheetIndex == 1293) ? 3 : 0);
                bool flag2 = false;
                if (location is DecoratableLocation)
                {
                    DecoratableLocation decoratableLocation = location as DecoratableLocation;
                    if (((int)this.furniture_type == 6 || (int)this.furniture_type == 17 || (int)this.furniture_type == 13 || num != 0) && decoratableLocation.isTileOnWall(point.X, point.Y - num) && decoratableLocation.GetWallTopY(point.X, point.Y - num) + num == point.Y)
                    {
                        flag2 = true;
                    }
                    else if (!this.isGroundFurniture() && decoratableLocation.isTileOnWall(point.X, point.Y - 1) && decoratableLocation.GetWallTopY(point.X, point.Y) + 1 == point.Y)
                    {
                        flag2 = true;
                    }
                }

                if (!flag2)
                {
                    return 1;
                }

                flag = true;
            }

            int num2 = this.getTilesHigh();
            if ((int)this.furniture_type == 6 && num2 > 2)
            {
                num2 = 2;
            }

            for (int i = point.X; i < point.X + this.getTilesWide(); i++)
            {
                for (int j = point.Y; j < point.Y + num2; j++)
                {
                    if (location.doesTileHaveProperty(i, j, "NoFurniture", "Back") != null)
                    {
                        return 2;
                    }

                    if (!flag && location is DecoratableLocation && (location as DecoratableLocation).isTileOnWall(i, j))
                    {
                        if (!(this is BedFurniture) || j != point.Y)
                        {
                            return 3;
                        }

                        continue;
                    }

                    int tileIndexAt = location.getTileIndexAt(i, j, "Buildings");
                    if (tileIndexAt != -1 && (!(location is IslandFarmHouse) || tileIndexAt < 192 || tileIndexAt > 194 || !(location.getTileSheetIDAt(i, j, "Buildings") == "untitled tile sheet")))
                    {
                        return -1;
                    }

                    if (location is BuildableGameLocation && (location as BuildableGameLocation).isTileOccupiedForPlacement(new Vector2(i, j), this))
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }

        public override bool canBePlacedInWater()
        {
            return base.canBePlacedInWater();
        }

        public override bool canBeShipped()
        {
            return base.canBeShipped();
        }

        public override int attachmentSlots()
        {
            return base.attachmentSlots();
        }

        public override bool canBeTrashed()
        {
            return base.canBeTrashed();
        }

        public override bool CanBuyItem(Farmer who)
        {
            return base.CanBuyItem(who);
        }

        
        /// <summary>
        /// Checks to see if the object is being interacted with. Seems to only happen when right clicked.
        /// </summary>
        /// <param name="who"></param>
        /// <param name="justCheckingForActivity"></param>
        /// <returns>True if something meaningful has occured. False otherwise.</returns>
        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (justCheckingForActivity)
            {
                //basically on item hover.
                return true;
            }
            return base.checkForAction(who, justCheckingForActivity);

            //True should be retruned when something meaningful has happened.
            //False should be returned when things like error messages have occurd.
        }
        

        public override string checkForSpecialItemHoldUpMeessage()
        {
            return base.checkForSpecialItemHoldUpMeessage();
        }

        public override bool clicked(Farmer who)
        {
            if (Game1.didPlayerJustLeftClick())
            {
                return this.attemptToPickupFromGameWorld(this.TileLocation, who.currentLocation, who);
            }
            return true;

        }

        public override void DayUpdate(GameLocation location)
        {
            base.DayUpdate(location);
            this.dayUpdateCounter.Value++;
            if (this.shouldDoDayUpdate())
            {
                this.doActualDayUpdateLogic(location);
                this.resetDayUpdateCounter();
            }
        }

        public virtual void doActualDayUpdateLogic(GameLocation location)
        {

        }

        /// <summary>
        /// Since <see cref="Furniture"/> and <see cref="StardewValley.Object"/> each do a seperate day update tick, we need to actually not do a day update for one of them.
        /// </summary>
        /// <returns></returns>
        public virtual bool shouldDoDayUpdate()
        {
            return this.dayUpdateCounter.Value >= 2;
        }

        /// <summary>
        /// Resets the day update counter on this Object.
        /// </summary>
        public virtual void resetDayUpdateCounter()
        {
            this.dayUpdateCounter.Value = 0;
        }

        public override void farmerAdjacentAction(GameLocation location)
        {
            base.farmerAdjacentAction(location);
        }

        public override int GetBaseRadiusForSprinkler()
        {
            return base.GetBaseRadiusForSprinkler();
        }

        /// <summary>
        /// Category color.
        /// </summary>
        /// <returns></returns>
        public override Color getCategoryColor()
        {
            return this.basicItemInformation.categoryColor;
        }

        /// <summary>
        /// Category name
        /// </summary>
        /// <returns></returns>
        public override string getCategoryName()
        {
            return this.basicItemInformation.categoryName;
        }

        /// <summary>
        /// Hover box text
        /// </summary>
        /// <param name="hoveredItem"></param>
        /// <returns></returns>
        public override string getHoverBoxText(Item hoveredItem)
        {
            return base.getHoverBoxText(hoveredItem);
        }

        /// <summary>
        /// Gets the decription to be displayed when hovering over an item.
        /// </summary>
        /// <returns></returns>
        public override string getDescription()
        {
            string description = JsonContentPackUtilities.LoadItemDescription(this.basicItemInformation.id.Value,false);
            if (string.IsNullOrEmpty(description))
            {
                description = this.basicItemInformation.description.Value; //Get default set description.
            }
            return Game1.parseText(description, Game1.smallFont, this.getDescriptionWidth());
        }

        public override StardewValley.Object GetDeconstructorOutput(Item item)
        {
            return base.GetDeconstructorOutput(item);
        }

        public override Item getOne()
        {
            return new CustomObject(this.basicItemInformation.Copy());
        }

        public override void _GetOneFrom(Item source)
        {
            base._GetOneFrom(source);
        }

        public override int healthRecoveredOnConsumption()
        {
            return this.basicItemInformation.healthRestoredOnEating;
        }

        public override void hoverAction()
        {
            base.hoverAction();
        }

        public override void initializeLightSource(Vector2 tileLocation, bool mineShaft = false)
        {
            this.LightManager.regenerateRealLightsFromFakeLights(this.getCurrentLocation());
        }

        public virtual bool addLight(Vector2 IdKey, LightManager.LightIdentifier LightShape, Color DesiredColor, float Radius)
        {
            return this.LightManager.addLightToTileLocation(IdKey, this.getCurrentLocation(), LightShape, DesiredColor, this.TileLocation, Radius);
        }

        public virtual bool removeLight(Vector2 IdKey)
        {
            return this.LightManager.removeLight(IdKey, this.getCurrentLocation());
        }

        public override bool isActionable(Farmer who)
        {
            return true;
        }

        public override bool isAnimalProduct()
        {
            return base.isAnimalProduct();
        }

        public override bool isForage(GameLocation location)
        {
            return base.isForage(location);
        }

        public override bool isPassable()
        {
            if (this.basicItemInformation.ignoreBoundingBox) return true;
            return false;
        }

        public override Rectangle getBoundingBox(Vector2 tileLocation)
        {

            Microsoft.Xna.Framework.Rectangle boundingBox = this.boundingBox.Value;
            Microsoft.Xna.Framework.Rectangle newBounds = boundingBox;
            newBounds.X = (int)tileLocation.X * Game1.tileSize;
            newBounds.Y = (int)tileLocation.Y * Game1.tileSize;

            newBounds.Width = Game1.tileSize * (int)this.basicItemInformation.boundingBoxTileDimensions.X;
            newBounds.Height = Game1.tileSize * (int)this.basicItemInformation.boundingBoxTileDimensions.Y;

            return newBounds;
        }

        public override List<Vector2> GetSprinklerTiles()
        {
            return base.GetSprinklerTiles();
        }

        public override bool isPlaceable()
        {
            return true;
        }


        public override bool IsSprinkler()
        {
            return base.IsSprinkler();
        }

        public override int maximumStackSize()
        {
            return 999;
        }

        /// <summary>
        /// When so many minutes pass,update this object.
        /// </summary>
        /// <param name="minutes"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            this.MinutesUntilReady -= minutes/2;
            if (this.MinutesUntilReady < 0) this.MinutesUntilReady = 0;
            return false;
        }

        public override bool onExplosion(Farmer who, GameLocation location)
        {
            return base.onExplosion(who, location);
        }

        public override void onReadyForHarvest(GameLocation environment)
        {
            base.onReadyForHarvest(environment);
        }

        /// <summary>
        /// Wraps Stardew valleys object dropin code. For actual mod implementation see <see cref="performItemDropInAction(Item, bool, Farmer)"/>
        /// </summary>
        /// <param name="dropInItem"></param>
        /// <param name="probe"></param>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            bool success=this.performItemDropInAction(dropInItem, probe, who);
            if (dropInItem == null) return false;
            if(success && (dropInItem is StardewValley.Object) && (dropInItem as StardewValley.Object).Edibility!= StardewValley.Object.inedible)
            {
                dropInItem.Stack++;
                return true;
            }

            return false;
        }

        public virtual bool performItemDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            if (probe == true) return false; //Just checking for action.
            if (who != null && who.ActiveObject == null) return false;
            if (dropInItem == null) return false;
            if (this.heldObject.Value != null) return false;
            return true;
        }

        /// <summary>
        /// Performs cleanup that should happen related to an object's removal and removes it properly from the game world.
        /// </summary>
        /// <param name="tileLocation"></param>
        /// <param name="environment"></param>
        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {

            if (environment == null) return;

            if (this.LightManager != null) this.LightManager.removeLights(this.getCurrentLocation());

            this.removeLights(environment);
            if ((int)this.furniture_type == 14 || (int)this.furniture_type == 16)
            {
                base.isOn.Value = false;
                this.setFireplace(environment, playSound: false);
            }
            this.RemoveLightGlow(environment);

            this.basicItemInformation.locationName.Value = "";
            this.boundingBox.Value = this.getBoundingBox(Vector2.Zero);

            this.sittingFarmers.Clear();

            this.removeFromGameWorld(tileLocation, environment);

            this.TileLocation = Vector2.Zero;
        }


        /// <summary>
        /// Attempts to pickup the object from the game world, but will show a message if the player's inventory is full.
        /// </summary>
        /// <param name="TileLocation"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual bool attemptToPickupFromGameWorld(Vector2 TileLocation, GameLocation location, Farmer who)
        {
            if (!this.canBeRemoved(who)) return false;

            if ( who == Game1.player && who.isInventoryFull() && who.couldInventoryAcceptThisItem(this)==false)
            {
                Game1.showRedMessage("Inventory full.");
                return false;
            }
            return this.pickupFromGameWorld(TileLocation, location, who);
        }


        /// <summary>
        /// Removes this from the game world, performs cleanup, and puts the object into the player's inventory. For similar logic, <see cref="CustomObject.AttemptRemoval(Action{Furniture})"/> for furniture removal logic as this is specific to the <see cref="StardewValley.Object"/> removal logic.
        /// </summary>
        /// <param name="tileLocation"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public virtual bool pickupFromGameWorld(Vector2 tileLocation, GameLocation environment, Farmer who)
        {



            this.performRemoveAction(tileLocation, environment);

            bool pickedUp=Game1.player.addItemToInventoryBool(this);
            //bool pickedUp=PlayerUtilities.AddItemToInventory(who, this);

            if (pickedUp && who!=null)
            {
                SoundUtilities.PlaySound(environment, Enums.StardewSound.coin);
            }

            return true;
        }

        /// <summary>
        /// Removes a game object from a <see cref="GameLocation"/>'s furniture and object lists.
        /// </summary>
        public virtual void removeFromGameWorld(Vector2 TileLocation, GameLocation environment)
        {

            if (environment != null)
            {

                if (this.LightManager != null)
                {
                    this.LightManager.removeLights(environment);
                }

                environment.objects.Remove(TileLocation);
                this.boundingBox.Value = new Rectangle(0, 0, 0, 0);

                environment.furniture.Remove(this);
                WorldUtility.RemoveFurnitureAtTileLocation(environment, TileLocation);
            }
        }

        /// <summary>
        /// A hack method to quickly readd an object to a game world if it wasn't actually needed to be fully removed.
        /// </summary>
        /// <param name="TileLocation"></param>
        /// <param name="environment"></param>
        public virtual void reAddToGameWorld(Vector2 TileLocation, GameLocation environment)
        {
            if (environment != null)
            {
                environment.objects.Add(TileLocation, this);
                environment.furniture.Add(this);
            }
        }

        public override void resetOnPlayerEntry(GameLocation environment, bool dropDown = false)
        {


            this.AnimationManager.resetCurrentAnimation();
            base.resetOnPlayerEntry(environment, dropDown);
        }

        /// <summary>
        /// When a tool is used on this item.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t == null)
            {
                RevitalizeModCore.log("Null tool used! Probably just the player's hands then.");
                this.shakeTimer = 200; //Milliseconds.
                this.basicItemInformation.shakeTimer.Value = 200;
                return false;

            }
            else
            {
                RevitalizeModCore.log("Player used tool: " + t.DisplayName);

                if (t is Pickaxe)
                {
                    RevitalizeModCore.log("Player used pickaxe!: ");
                    this.createItemDebris(location, this.TileLocation, Game1.player.getTileLocation());
                    this.performRemoveAction(this.TileLocation, location);
                    this.shakeTimer = 200;
                    this.basicItemInformation.shakeTimer.Value = 200;
                    SoundUtilities.PlaySound(Enums.StardewSound.hammer);
                    return false;
                }


            }
            return false;
            //False is returned if we signify no meaningul tool interactions?
            //True is returned when something significant happens

        }

        public virtual void createItemDebris(GameLocation location, Vector2 OriginTile, Vector2 DestinationTile)
        {
            //location.debris.Add(new CustomObjectDebris(this, OriginTile, DestinationTile));
            WorldUtility.CreateItemDebrisAtTileLocation(location,this ,OriginTile, DestinationTile);
        }

        public override void drawTooltip(SpriteBatch spriteBatch, ref int x, ref int y, SpriteFont font, float alpha, StringBuilder overrideText)
        {
            base.drawTooltip(spriteBatch, ref x, ref y, font, alpha, overrideText);


            /*
            overrideText = new StringBuilder();
            overrideText.Append(this.getActualDescription());
            Vector2 small_text_size = Vector2.Zero;
            int width = Math.Max( 0, Math.Max((int)font.MeasureString(this.getActualDescription()).X, ((int)Game1.dialogueFont.MeasureString(this.DisplayName).X))) + 32;
            int height = Math.Max(20 * 3, (int)font.MeasureString(this.getActualDescription()).Y + 32 +  (int)((this.DisplayName != null) ? (Game1.dialogueFont.MeasureString(this.DisplayName).Y + 16f) : 0f));

            IClickableMenu.drawTextureBox(spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width +  0, height, Color.White * alpha);
            if (this.DisplayName != null)
            { string boldTitleText = this.DisplayName;
                SpriteBatch b = spriteBatch;

                Vector2 bold_text_size = Game1.dialogueFont.MeasureString(this.DisplayName);
                IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width + 0, (int)Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (int)(font.MeasureString("asd").Y) - 4, Color.White * alpha, 1f, false);
                b.Draw(Game1.menuTexture, new Rectangle(x + 12, y + (int)Game1.dialogueFont.MeasureString(boldTitleText).Y + 32 + (int)( font.MeasureString("asd").Y) - 4, width - 4 * (6), 4), new Rectangle(44, 300, 4, 4), Color.White);
                b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor);
                b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y + 16 + 4) + new Vector2(0f, 2f), Game1.textShadowColor);
                b.DrawString(Game1.dialogueFont, boldTitleText, new Vector2(x + 16, y + 16 + 4), Game1.textColor);
                y += (int)Game1.dialogueFont.MeasureString(boldTitleText).Y;
            }


            if (overrideText != null && overrideText.Length != 0 && (overrideText.Length != 1 || overrideText[0] != ' '))
            {
                spriteBatch.DrawString(font, overrideText, new Vector2(x + 16, y + 16 + 4) + new Vector2(2f, 2f), Game1.textShadowColor * alpha);
                spriteBatch.DrawString(font, overrideText, new Vector2(x + 16, y + 16 + 4) + new Vector2(0f, 2f), Game1.textShadowColor * alpha);
                spriteBatch.DrawString(font, overrideText, new Vector2(x + 16, y + 16 + 4) + new Vector2(2f, 0f), Game1.textShadowColor * alpha);
                spriteBatch.DrawString(font, overrideText, new Vector2(x + 16, y + 16 + 4), Game1.textColor * 0.9f * alpha);
                y += (int)font.MeasureString(overrideText).Y + 4;
            }
            */
        }

        

        /// <summary>
        /// When this item is used. (Left clicked)
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override bool performUseAction(GameLocation location)
        {
            return base.performUseAction(location);
        }

        /// <summary>
        /// Places this object at a given TILE location for the game.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="TileX"></param>
        /// <param name="TileY"></param>
        /// <param name="who"></param>
        /// <returns></returns>
        public virtual bool placementActionAtTile(GameLocation location, int TileX, int TileY, Farmer who = null)
        {
            return this.placementAction(location, TileX * Game1.tileSize, TileY * Game1.tileSize, who);
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {

            if (!this.isGroundFurniture())
            {
                y = this.GetModifiedWallTilePosition(location, x / 64, y / 64) * 64;
            }
            if (this.GetAdditionalFurniturePlacementStatus(location, x, y, who) != 0)
            {
                return false;
            }

            CustomObject obj = (CustomObject)this.getOne();

            obj.boundingBox.Value = this.getBoundingBox(new Vector2((float)x / (float)64, (float)y / (float)64));

            Vector2 placementTile = new Vector2(x / 64, y / 64);
            obj.health = 10;
            if (who != null)
            {
                obj.owner.Value = who.UniqueMultiplayerID;
            }
            else
            {
                obj.owner.Value = Game1.player.UniqueMultiplayerID;
            }
            obj.TileLocation = placementTile;

            //HERE????


            location.furniture.Add(obj);
            location.objects.Add(placementTile, obj);

            //location.setObject(placementTile, obj);
            //location.fur


            if (who != null)
            {
                SoundUtilities.PlaySound(location, Enums.StardewSound.woodyStep);
            }

            string locationName = location.NameOrUniqueName;
            if (string.IsNullOrEmpty(location.NameOrUniqueName))
            {
                locationName = location.Name;
            }

            obj.basicItemInformation.locationName.Value = locationName;
            obj.updateDrawPosition();

            if (who == Game1.player)
            {
                this.Stack--;
                if (this.Stack == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }




            return true;
            //Base code throws and error so I have to do it this way.
        }


        /// <summary>
        /// Gets a reference to the actual owner for this object.
        /// </summary>
        /// <returns></returns>
        public virtual Farmer getOwner()
        {
            if (this.owner.Value == Game1.player.UniqueMultiplayerID)
            {
                return Game1.player;
            }

            foreach(Farmer farmer in Game1.getAllFarmers())
            {
                if (farmer.uniqueMultiplayerID.Value.Equals(this.owner.Value))
                {
                    return farmer;
                }
            }
            return null;
        }

        public override int salePrice()
        {
            return this.basicItemInformation.price;
        }

        public override int sellToStorePrice(long specificPlayerID = -1)
        {
            return this.basicItemInformation.price;
            return base.sellToStorePrice(specificPlayerID); //logic for when it's regarding the player's professions and such.
        }

        public override void reloadSprite()
        {
            base.reloadSprite();
        }

        public override int staminaRecoveredOnConsumption()
        {
            return this.basicItemInformation.staminaRestoredOnEating;
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            if (this.shakeTimer > 0)
            {
                this.basicItemInformation.shakeTimer.Value -= time.ElapsedGameTime.Milliseconds;
                this.shakeTimer -= time.ElapsedGameTime.Milliseconds;
                if (this.shakeTimer <= 0)
                {
                    this.health = 10;
                }
            }
            if (this.basicItemInformation.shakeTimer.Value > 0)
            {
                
            }

        }


        /// <summary>What happens when the player shift-right clicks this object.</summary>
        public virtual bool shiftRightClicked(Farmer who)
        {
            return false;
        }


        public virtual void setGameLocation(string LocationName, bool IsStructure)
        {
            this.basicItemInformation.locationName.Value = LocationName;
            this.isCurrentLocationAStructure = IsStructure;
        }

        public virtual GameLocation getCurrentLocation()
        {
            if (string.IsNullOrEmpty(this.basicItemInformation.locationName.Value))
            {
                return null;
            }
            else
            {
                return Game1.getLocationFromName(this.basicItemInformation.locationName.Value, this.isCurrentLocationAStructure);
            }
        }

        public virtual BasicItemInformation getItemInformation()
        {
            return this.basicItemInformation;
        }

        public virtual void setItemInformation(BasicItemInformation Info)
        {
            this.basicItemInformation = Info;
        }


        public override bool canBeRemoved(Farmer who = null)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DyeColor"></param>
        public virtual void dyeColor(NamedColor DyeColor)
        {
            this.basicItemInformation.dyedColor.setFields(DyeColor);
        }

        public virtual void eraseDye()
        {
            this.basicItemInformation.dyedColor.clearFields();
        }

        public override bool canStackWith(ISalable other)
        {
            if (other is CustomObject == false) return false;
            CustomObject o = (CustomObject)other;

            if (this.basicItemInformation.id.Value.Equals(o.basicItemInformation.id.Value) == false) return false;


            if (this.maximumStackSize() >= this.Stack + other.Stack)
            {
                return true;
            }

            //if (this.basicItemInformation.id.Equals(o.basicItemInformation.id) == true) return true;

            return false;
        }

        public override int addToStack(Item otherStack)
        {
            int maxStack = this.maximumStackSize();
            if (maxStack != 1)
            {
                this.stack.Value += otherStack.Stack;
                if ((int)this.stack > maxStack)
                {
                    int result = (int)this.stack - maxStack;
                    this.stack.Value = maxStack;
                    return result;
                }
                return 0;
            }
            return otherStack.Stack;
        }

        public override void AttemptRemoval(Action<Furniture> removal_action)
        {
            this.attemptToPickupFromGameWorld(this.TileLocation, this.getCurrentLocation(), Game1.player);


            Game1.player.currentLocation.localSound("coin");

            //base.AttemptRemoval(removal_action);
        }



        public override int getTilesHigh()
        {
            return (int)this.basicItemInformation.boundingBoxTileDimensions.Y;
        }

        public override int getTilesWide()
        {
            return (int)this.basicItemInformation.boundingBoxTileDimensions.X;
        }

        /// <summary>
        /// Gets the bounding box position and dimensions for this object IN TILES.
        /// </summary>
        /// <returns></returns>
        public virtual Rectangle getBoundingBoxTiles()
        {
            return new Rectangle(this.boundingBox.Value.X / Game1.tileSize, this.boundingBox.Value.Y / Game1.tileSize, this.getTilesWide(), this.getTilesHigh());
        }

        public override void dropItem(GameLocation location, Vector2 origin, Vector2 destination)
        {
            WorldUtilities.WorldUtility.CreateItemDebrisAtTileLocation(location,this ,origin / Game1.tileSize, destination / Game1.tileSize);
        }

        /// <summary>
        /// Drops the held object for this item to the ground.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        public virtual void dropHeldObject(GameLocation location, Vector2 origin, Vector2 destination)
        {
            this.dropHeldObjectAtTileLocation(location, origin / Game1.tileSize, destination / Game1.tileSize);
        }

        public virtual void dropHeldObjectAtTileLocation(GameLocation location, Vector2 origin, Vector2 destination)
        {
            if (this.heldObject.Value != null)
            {
                WorldUtilities.WorldUtility.CreateItemDebrisAtTileLocation(location, this.heldObject.Value, origin, destination);
                this.heldObject.Value = null;
            }
        }



        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //                            Rendering code                   //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

        public override void drawAsProp(SpriteBatch b)
        {
            base.drawAsProp(b);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            this.DrawICustomModObjectInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
        }

        public override void drawAttachments(SpriteBatch b, int x, int y)
        {
            base.drawAttachments(b, x, y);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            this.DrawICustomModObjectWhenHeld(spriteBatch, objectPosition, f);
        }

        public virtual void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f, float Transparency, float Scale)
        {
            this.DrawICustomModObjectWhenHeld(spriteBatch, objectPosition, f, Transparency, Scale);
        }

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            this.DrawICustomModObject(spriteBatch, alpha);
        }

        public virtual void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f, bool DrawHeldObject=true)
        {
            this.DrawICustomModObject(spriteBatch, alpha,DrawHeldObject);
        }

        public virtual void clearHeldObject()
        {
            this.heldObject.Value = null;
            this.readyForHarvest.Value = false;
        }

        public override void drawPlacementBounds(SpriteBatch spriteBatch, GameLocation location)
        {
            if (!this.isPlaceable())
            {
                return;
            }
            int X = (int)Game1.GetPlacementGrabTile().X * 64;
            int Y = (int)Game1.GetPlacementGrabTile().Y * 64;
            Game1.isCheckingNonMousePlacement = !Game1.IsPerformingMousePlacement();
            if (Game1.isCheckingNonMousePlacement)
            {
                Vector2 nearbyValidPlacementPosition = Utility.GetNearbyValidPlacementPosition(Game1.player, location, this, X, Y);
                X = (int)nearbyValidPlacementPosition.X;
                Y = (int)nearbyValidPlacementPosition.Y;
            }
            /*
            if (Utility.isThereAnObjectHereWhichAcceptsThisItem(location, this, X, Y))
            {
                return;
            }
            */
            bool canPlaceHere = Utility.playerCanPlaceItemHere(location, this, X, Y, Game1.player) || (Utility.isThereAnObjectHereWhichAcceptsThisItem(location, this, X, Y) && Utility.withinRadiusOfPlayer(X, Y, 1, Game1.player));
            Game1.isCheckingNonMousePlacement = false;
            int width = this.getTilesWide();
            int height = this.getTilesHigh();
            for (int x_offset = 0; x_offset < width; x_offset++)
            {
                for (int y_offset = 0; y_offset < height; y_offset++)
                {
                    spriteBatch.Draw(Game1.mouseCursors, new Vector2((X / 64 + x_offset) * 64 - Game1.viewport.X, (Y / 64 + y_offset) * 64 - Game1.viewport.Y), new Microsoft.Xna.Framework.Rectangle(canPlaceHere ? 194 : 210, 388, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.01f);
                }
            }
            this.DrawICustomModObject(spriteBatch, X/64+(int)this.basicItemInformation.drawOffset.X, Y/64+(int)this.basicItemInformation.drawOffset.Y ,0.5f);
        }

        /// <summary>Draw the game object at a non-tile spot. Aka like debris.</summary>
        public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1f)
        {

            if (this.AnimationManager == null)
            {
                spriteBatch.Draw(this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xNonTile), yNonTile)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, layerDepth));
            }

            else
            {
                //Log.AsyncC("Animation Manager is working!");
                this.AnimationManager.draw(spriteBatch, this.CurrentTextureToDisplay, Game1.GlobalToLocal(Game1.viewport, new Vector2((float)(xNonTile), yNonTile)), new Rectangle?(this.AnimationManager.getCurrentAnimationFrameRectangle()), this.basicItemInformation.DrawColor * alpha, 0f, Vector2.Zero, (float)Game1.pixelZoom, this.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, layerDepth));
            }
        }






    }
}
