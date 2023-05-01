/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using SolidFoundations.Framework.External.SpaceCore;
using SolidFoundations.Framework.Models.Backport;
using SolidFoundations.Framework.Utilities;
using SolidFoundations.Framework.Utilities.Backport;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static SolidFoundations.Framework.Models.ContentPack.InputFilter;
using static StardewValley.Object;
using Object = StardewValley.Object;

namespace SolidFoundations.Framework.Models.ContentPack
{
    public class GenericBuilding : Building
    {
        [XmlIgnore]
        public ExtendedBuildingModel Model { get; set; }
        public string Id { get { return base.modData.ContainsKey(ModDataKeys.GENERIC_BUILDING_ID) ? base.modData[ModDataKeys.GENERIC_BUILDING_ID] : String.Empty; } set { base.modData[ModDataKeys.GENERIC_BUILDING_ID] = value; } }
        public string LocationName { get; set; }

        // Start of backported properties
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building.buildingLocation
        [XmlIgnore]
        public NetLocationRef buildingLocation = new NetLocationRef();
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building.buildingChests
        public NetList<Chest, NetRef<Chest>> buildingChests = new NetList<Chest, NetRef<Chest>>();
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building.animalDoorOpenAmount
        public readonly NetFloat animalDoorOpenAmount = new NetFloat();
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building._buildingMetadata
        [XmlIgnore]
        protected Dictionary<string, string> _buildingMetadata = new Dictionary<string, string>();
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building._lastHouseUpgradeLevel
        protected int _lastHouseUpgradeLevel = -1;
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building._chimneyPosition
        protected Vector2 _chimneyPosition = Vector2.Zero;
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building._hasChimney
        protected bool? _hasChimney;
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building.chimneyTimer
        protected int chimneyTimer = 500;
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building.skinID
        public NetString skinID = new NetString();
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building.hasLoaded
        public bool hasLoaded;
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building.upgradeName
        public readonly NetString upgradeName = new NetString();

        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building.nonInstancedIndoors
        public readonly NetLocationRef nonInstancedIndoors = new NetLocationRef();
        // TODO: When updated to SDV v1.6, this property should be deleted in favor of using the native StardewValley.Buildings.Building.IndoorOrInstancedIndoor
        public GameLocation IndoorOrInstancedIndoor
        {
            get
            {
                if (this.indoors.Value != null)
                {
                    return this.indoors.Value;
                }
                return this.nonInstancedIndoors.Value;
            }
        }

        public GenericBuilding() : base()
        {

        }

        public GenericBuilding(ExtendedBuildingModel model) : base(new BluePrint(model.ID), Vector2.Zero)
        {
            RefreshModel(model);

            base.indoors.Value = this.getIndoors(this.Model.IndoorMap);
            this.updateInteriorWarps();
        }

        public GenericBuilding(ExtendedBuildingModel model, BluePrint bluePrint) : base(bluePrint, Vector2.Zero)
        {
            RefreshModel(model);

            base.texture = new Lazy<Texture2D>(bluePrint.texture);

            base.indoors.Value = this.getIndoors(this.Model.IndoorMap);
            this.updateInteriorWarps();
        }

        public GenericBuilding(ExtendedBuildingModel model, BluePrint bluePrint, Vector2 tileLocation) : base(bluePrint, tileLocation)
        {
            RefreshModel(model);

            base.texture = new Lazy<Texture2D>(bluePrint.texture);

            base.indoors.Value = this.getIndoors(this.Model.IndoorMap);
            this.updateInteriorWarps();
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            base.NetFields.AddFields(this.buildingLocation.NetFields, this.buildingChests, this.animalDoorOpenAmount, this.skinID, this.upgradeName, this.nonInstancedIndoors.NetFields);
        }

        public void RefreshModel()
        {
            if (Model is not null)
            {
                RefreshModel(Model);
            }
        }

        public void RefreshModel(ExtendedBuildingModel model)
        {
            Model = model;
            Id = model.ID;

            base.tilesWide.Value = model.Size.X;
            base.tilesHigh.Value = model.Size.Y;
            base.fadeWhenPlayerIsBehind.Value = model.FadeWhenBehind;

            if (String.IsNullOrEmpty(this.Model.IndoorMap) is false && base.indoors.Value is not null)
            {
                base.indoors.Value.mapPath.Value = "Maps\\" + this.Model.IndoorMap;
                base.indoors.Value.loadMap(base.indoors.Value.mapPath.Value, true);
                this.updateInteriorWarps(base.indoors.Value);
            }
            this.LoadFromBuildingData(false);

            // Set the building chests
            if (this.Model is not null && this.Model.Chests is not null)
            {
                foreach (ExtendedBuildingChest chestData in this.Model.Chests)
                {
                    if (this.GetBuildingChest(chestData.Name) is Chest chest && chest is not null)
                    {
                        chest.modData[ModDataKeys.CUSTOM_CHEST_CAPACITY] = chestData.Capacity.ToString();
                    }
                }
            }
        }

        public bool ValidateConditions(string condition, string[] modDataFlags = null)
        {
            if (GameStateQuery.CheckConditions(condition))
            {
                if (modDataFlags is not null)
                {
                    foreach (string flag in modDataFlags)
                    {
                        // Clear whitespace
                        var cleanedFlag = flag.Replace(" ", String.Empty);
                        bool flagShouldNotExist = String.IsNullOrEmpty(cleanedFlag) || cleanedFlag[0] != '!' ? false : true;
                        if (flagShouldNotExist)
                        {
                            cleanedFlag = cleanedFlag[1..];
                        }
                        cleanedFlag = String.Concat(ModDataKeys.FLAG_BASE, ".", cleanedFlag.ToLower());

                        //string flagKey = cleanedFlag.Contains(':') ? cleanedFlag.Split(':')[0] : String.Empty;
                        //string flagValue = cleanedFlag.Contains(':') && cleanedFlag.Split(':').Length > 1 ? cleanedFlag.Split(':')[1] : String.Empty;

                        if (this.modData.ContainsKey(cleanedFlag) is false == flagShouldNotExist is false)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public bool ValidateLayer(ExtendedBuildingDrawLayer layer)
        {
            if (ValidateConditions(layer.Condition, layer.ModDataFlags) is false)
            {
                return false;
            }
            if (layer.OnlyDrawIfChestHasContents != null)
            {
                Chest buildingChest = this.GetBuildingChest(layer.OnlyDrawIfChestHasContents);
                if (buildingChest == null || buildingChest.isEmpty())
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsAuxiliaryTile(Vector2 tileLocation)
        {
            if (this.Model is null || this.Model.AuxiliaryHumanDoors.Count == 0)
            {
                return false;
            }

            var tilePoint = new Point((int)tileLocation.X, (int)tileLocation.Y);
            foreach (var door in this.Model.AuxiliaryHumanDoors)
            {
                if (door.X + this.tileX.Value == tilePoint.X && door.Y + this.tileY.Value == tilePoint.Y)
                {
                    return true;
                }
            }

            return false;
        }

        // TODO: Will need preserve the changes made (such as price changing and wine / jellies and other preserves)
        public void ProcessItemConversions(int minutesElapsed, bool isDayStart = false)
        {
            if (this.Model is null || this.Model.ItemConversions is null)
            {
                return;
            }

            int processedConversions = 0;
            foreach (ExtendedBuildingItemConversion itemConversion in this.Model.ItemConversions)
            {
                if (this.Model.MaxConcurrentConversions != -1 && processedConversions >= this.Model.MaxConcurrentConversions)
                {
                    break;
                }

                if (itemConversion.MinutesPerConversion > 0)
                {
                    if (itemConversion.MinutesRemaining is null)
                    {
                        itemConversion.MinutesRemaining = itemConversion.MinutesPerConversion - minutesElapsed;
                        continue;
                    }
                    else if (itemConversion.MinutesRemaining - minutesElapsed > 0 && isDayStart is false && itemConversion.ShouldTrackTime)
                    {
                        itemConversion.MinutesRemaining -= minutesElapsed;
                        continue;
                    }
                }
                else if (isDayStart is false)
                {
                    continue;
                }

                // Reset the MaxDailyConversions
                if (isDayStart || itemConversion.RefreshMaxDailyConversions)
                {
                    if (itemConversion.CachedMaxDailyConversions is null)
                    {
                        itemConversion.CachedMaxDailyConversions = itemConversion.MaxDailyConversions;
                    }
                    else
                    {
                        itemConversion.MaxDailyConversions = (int)itemConversion.CachedMaxDailyConversions;
                    }
                }

                int num = 0;
                int num2 = 0;
                int preserveDroppedInId = -1;
                string DGAFullId = null;
                Chest buildingChest = this.GetBuildingChest(itemConversion.SourceChest);
                Chest buildingChest2 = this.GetBuildingChest(itemConversion.DestinationChest);
                if (buildingChest == null)
                {
                    continue;
                }
                foreach (Item item4 in buildingChest.items)
                {
                    if (item4 == null)
                    {
                        continue;
                    }
                    bool flag = false;
                    foreach (string requiredTag in itemConversion.RequiredTags)
                    {
                        if (!item4.HasContextTag(requiredTag))
                        {
                            flag = true;
                            break;
                        }
                        else if (preserveDroppedInId == -1 && (item4.HasContextTag("category_fruits") || item4.HasContextTag("category_vegetable") || item4.ParentSheetIndex == 812))
                        {
                            // Item is a fruit, vegetable or roe
                            preserveDroppedInId = item4.ParentSheetIndex;
                            if (DGAIntegration.IsDGASObject?.Invoke(item4) == true)
                            {
                                DGAFullId = DGAIntegration.GetDGAFullID?.Invoke(item4);
                            }
                        }
                    }
                    if (flag)
                    {
                        continue;
                    }
                    num2 += item4.Stack;
                    if (num2 >= itemConversion.RequiredCount)
                    {
                        int num3 = num2 / itemConversion.RequiredCount;
                        if (itemConversion.MaxDailyConversions >= 0)
                        {
                            num3 = Math.Min(num3, itemConversion.MaxDailyConversions - num);
                        }
                        num += num3;
                        num2 -= num3 * itemConversion.RequiredCount;
                    }

                    itemConversion.MaxDailyConversions -= num;
                    if (itemConversion.MaxDailyConversions >= 0 && num >= itemConversion.MaxDailyConversions)
                    {
                        break;
                    }
                }

                if (num == 0)
                {
                    itemConversion.ShouldTrackTime = false;
                    continue;
                }
                else
                {
                    // Start tracking the time for the items to convert and reset MinutesRemaining
                    if (itemConversion.MinutesPerConversion > 0 && itemConversion.ShouldTrackTime is false)
                    {
                        itemConversion.ShouldTrackTime = true;
                        itemConversion.MinutesRemaining = itemConversion.MinutesPerConversion - minutesElapsed;
                        continue;
                    }
                }

                // Track the processed conversion and reset ShouldTrackTime, MinutesRemaining
                processedConversions += 1;
                itemConversion.ShouldTrackTime = false;

                int num4 = 0;
                for (int i = 0; i < num; i++)
                {
                    bool flag2 = false;
                    for (int j = 0; j < itemConversion.ProducedItems.Count; j++)
                    {
                        ExtendedAdditionalChopDrops additionalChopDrops = itemConversion.ProducedItems[j];
                        if (ValidateConditions(additionalChopDrops.Condition, additionalChopDrops.ModDataFlags) is false)
                        {
                            continue;
                        }
                        int num5 = new Random((int)((long)Game1.uniqueIDForThisGame + (long)this.tileX.Value * 777L + (long)this.tileY.Value * 7L + Game1.stats.DaysPlayed + j * 500 + Game1.ticks + DateTime.Now.Ticks + (500 * processedConversions))).Next(additionalChopDrops.MinCount, additionalChopDrops.MaxCount + 1);
                        if (num5 != 0 && Toolkit.CreateItemByID(additionalChopDrops.ItemID, num5, additionalChopDrops.Quality) is Item item && item is not null)
                        {
                            if (item is Object obj)
                            {
                                if (!string.IsNullOrEmpty(additionalChopDrops.PreserveType))
                                {
                                    obj.preserve.Value = Enum.Parse<PreserveType>(additionalChopDrops.PreserveType);

                                    if (!string.IsNullOrEmpty(additionalChopDrops.PreserveID))
                                    {
                                        if (additionalChopDrops.PreserveID == "DROP_IN" && preserveDroppedInId != -1)
                                        {
                                            obj.preservedParentSheetIndex.Value = preserveDroppedInId;
                                            if (!string.IsNullOrWhiteSpace(DGAFullId))
                                            {
                                                obj.modData[DGAIntegration.DGAModataKey] = DGAFullId;
                                            }
                                        }
                                        else if (int.TryParse(additionalChopDrops.PreserveID, out int parentId))
                                        {
                                            obj.preservedParentSheetIndex.Value = parentId;
                                        }

                                        if (obj.preservedParentSheetIndex.Value != default(int) && Toolkit.CreateItemByID(obj.preservedParentSheetIndex.Value.ToString(), 1, 0) is Object preserveItem && preserveItem is not null)
                                        {
                                            var preserveType = obj.preserve.Value is PreserveType.AgedRoe ? "Aged Roe" : obj.preserve.Value.ToString();
                                            obj.name = $"{preserveItem.Name} {preserveType}";
                                            obj.Price = preserveItem.Price;
                                        }
                                    }
                                }

                                obj.Price = additionalChopDrops.AddPrice + (int)(obj.Price * additionalChopDrops.MultiplyPrice);
                            }

                            Item item2 = buildingChest2.addItem(item);
                            if (item2 == null || item2.Stack != num5)
                            {
                                flag2 = true;
                            }
                        }
                    }
                    if (flag2)
                    {
                        num4++;
                    }
                }
                if (num4 <= 0)
                {
                    continue;
                }
                int num6 = num4 * itemConversion.RequiredCount;
                for (int k = 0; k < buildingChest.items.Count; k++)
                {
                    Item item3 = buildingChest.items[k];
                    if (item3 == null)
                    {
                        continue;
                    }
                    bool flag3 = false;
                    foreach (string requiredTag2 in itemConversion.RequiredTags)
                    {
                        if (!item3.HasContextTag(requiredTag2))
                        {
                            flag3 = true;
                            break;
                        }
                    }
                    if (!flag3)
                    {
                        int num7 = Math.Min(num6, item3.Stack);
                        buildingChest.items[k] = Toolkit.ConsumeStack(item3, num7);
                        num6 -= num7;
                        if (num6 <= 0)
                        {
                            break;
                        }
                    }
                }
            }
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.OnUseHumanDoor
        public virtual void ToggleAnimalDoor(Farmer who)
        {
            if (!this.animalDoorOpen)
            {
                if (this.Model.AnimalDoorOpenSound != null)
                {
                    who.currentLocation.playSound(this.Model.AnimalDoorOpenSound);
                }
            }
            else if (this.Model.AnimalDoorCloseSound != null)
            {
                who.currentLocation.playSound(this.Model.AnimalDoorCloseSound);
            }
            this.animalDoorOpen.Value = !this.animalDoorOpen;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.OnUseHumanDoor
        public virtual bool OnUseHumanDoor(Farmer who)
        {
            return true;
        }

        // Preserve this override (IsAuxiliaryTile) when updated to SDV v1.6, but call the base doAction method if ExtendedBuildingModel.
        public override bool isActionableTile(int xTile, int yTile, Farmer who)
        {
            if (IsAuxiliaryTile(new Vector2(xTile, yTile)))
            {
                return true;
            }

            return base.isActionableTile(xTile, yTile, who);
        }

        // Preserve this override (specifically the CustomAction / IsAuxiliaryTile) when updated to SDV v1.6, but call the base doAction method if ExtendedBuildingModel.
        public override bool doAction(Vector2 tileLocation, Farmer who)
        {
            if (who.isRidingHorse())
            {
                return false;
            }
            if (who.IsLocalPlayer && tileLocation.X >= (float)(int)this.tileX && tileLocation.X < (float)((int)this.tileX + (int)this.tilesWide) && tileLocation.Y >= (float)(int)this.tileY && tileLocation.Y < (float)((int)this.tileY + (int)this.tilesHigh) && (int)this.daysOfConstructionLeft > 0)
            {
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Buildings:UnderConstruction"));
            }
            else
            {
                if (who.IsLocalPlayer && (IsAuxiliaryTile(tileLocation) || tileLocation.X == (float)(this.humanDoor.X + (int)this.tileX) && tileLocation.Y == (float)(this.humanDoor.Y + (int)this.tileY)) && this.IndoorOrInstancedIndoor != null)
                {
                    if (who.mount != null)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:DismountBeforeEntering"));
                        return false;
                    }
                    if (who.team.demolishLock.IsLocked())
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:CantEnter"));
                        return false;
                    }
                    if (this.OnUseHumanDoor(who))
                    {
                        who.currentLocation.playSoundAt("doorClose", tileLocation);
                        bool isStructure = false;
                        if (this.indoors.Value != null)
                        {
                            isStructure = true;
                        }
                        Game1.warpFarmer(this.IndoorOrInstancedIndoor.NameOrUniqueName, this.IndoorOrInstancedIndoor.warps[0].X, this.IndoorOrInstancedIndoor.warps[0].Y - 1, Game1.player.FacingDirection, isStructure);
                    }
                    return true;
                }
                if (this.Model != null)
                {
                    Microsoft.Xna.Framework.Rectangle rectForAnimalDoor = this.getRectForAnimalDoor();
                    rectForAnimalDoor.Width /= 64;
                    rectForAnimalDoor.Height /= 64;
                    rectForAnimalDoor.X /= 64;
                    rectForAnimalDoor.Y /= 64;
                    if ((int)this.daysOfConstructionLeft <= 0 && rectForAnimalDoor != Microsoft.Xna.Framework.Rectangle.Empty && rectForAnimalDoor.Contains(Utility.Vector2ToPoint(tileLocation)) && Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true))
                    {
                        this.ToggleAnimalDoor(who);
                        return true;
                    }
                    this.GetAdditionalTilePropertyRadius();
                    if (who.IsLocalPlayer && this.IsInTilePropertyRadius(tileLocation) && !this.isTilePassable(tileLocation))
                    {
                        Point actualTile = new Point((int)tileLocation.X - this.tileX.Value, (int)tileLocation.Y - this.tileY.Value);
                        var specialActionAtTile = this.Model.GetSpecialActionAtTile(actualTile.X, actualTile.Y);
                        if (specialActionAtTile is not null)
                        {
                            specialActionAtTile.Trigger(who, this, actualTile);
                            return true;
                        }

                        if (who.ActiveObject is not null && this.Model.LoadChestTiles is not null && this.Model.GetLoadChestActionAtTile(actualTile.X, actualTile.Y) is var loadChestName && String.IsNullOrEmpty(loadChestName) is false)
                        {
                            this.PerformBuildingChestAction(loadChestName, who);
                            return true;
                        }
                        if (who.ActiveObject is null && this.Model.CollectChestTiles is not null && this.Model.GetCollectChestActionAtTile(actualTile.X, actualTile.Y) is var collectChestName && String.IsNullOrEmpty(collectChestName) is false)
                        {
                            this.PerformBuildingChestAction(collectChestName, who);
                            return true;
                        }

                        string actionAtTile = this.Model.GetActionAtTile(actualTile.X, actualTile.Y);
                        if (actionAtTile != null)
                        {
                            actionAtTile = TextParser.ParseText(actionAtTile);
                            if (who.currentLocation.performAction(actionAtTile, who, new xTile.Dimensions.Location((int)tileLocation.X, (int)tileLocation.Y)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return base.doAction(tileLocation, who);
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.doesTileHaveProperty
        public override bool doesTileHaveProperty(int tile_x, int tile_y, string property_name, string layer_name, ref string property_value)
        {
            if (this.Model != null)
            {
                return this.Model.HasPropertyAtTile(tile_x - this.tileX.Value, tile_y - this.tileY.Value, property_name, layer_name, ref property_value);
            }
            return false;
        }


        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.ApplySourceRectOffsets
        public virtual Rectangle ApplySourceRectOffsets(Rectangle source)
        {
            if (this.Model.SeasonOffset != Point.Zero)
            {
                int num = 0;
                if (Game1.IsSpring)
                {
                    num = 0;
                }
                else if (Game1.IsSummer)
                {
                    num = 1;
                }
                else if (Game1.IsFall)
                {
                    num = 2;
                }
                else if (Game1.IsWinter)
                {
                    num = 3;
                }
                source.X += this.Model.SeasonOffset.X * num;
                source.Y += this.Model.SeasonOffset.Y * num;
            }

            return source;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.getSourceRect
        public override Rectangle getSourceRect()
        {
            if (this.Model is not null)
            {
                Rectangle rectangle = this.Model.GetSourceRect();
                if (rectangle != Rectangle.Empty)
                {
                    return this.ApplySourceRectOffsets(rectangle);
                }
            }

            return base.texture.Value.Bounds;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.getRectForAnimalDoor
        public override Rectangle getRectForAnimalDoor()
        {
            if (this.Model != null)
            {
                Rectangle animalDoorRect = this.Model.GetAnimalDoorRect();
                return new Rectangle((animalDoorRect.X + (int)this.tileX) * 64, (animalDoorRect.Y + (int)this.tileY) * 64, animalDoorRect.Width * 64, animalDoorRect.Height * 64);
            }
            return new Rectangle((this.animalDoor.X + (int)this.tileX) * 64, ((int)this.tileY + this.animalDoor.Y) * 64, 64, 64);
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.intersects
        public override bool intersects(Rectangle boundingBox)
        {
            if (this.Model != null)
            {
                Rectangle rectangle = new Rectangle((int)this.tileX.Value * 64, (int)this.tileY.Value * 64, (int)this.tilesWide.Value * 64, (int)this.tilesHigh.Value * 64);
                int additionalTilePropertyRadius = this.GetAdditionalTilePropertyRadius();
                if (additionalTilePropertyRadius > 0)
                {
                    rectangle.Inflate(additionalTilePropertyRadius * 64, additionalTilePropertyRadius * 64);
                }

                var isEntityWithinBuilding = rectangle.Contains(Game1.player.GetBoundingBox());
                if (rectangle.Intersects(boundingBox))
                {
                    for (int i = boundingBox.Left / 64; i <= boundingBox.Right / 64; i++)
                    {
                        for (int j = boundingBox.Top / 64; j <= boundingBox.Bottom / 64; j++)
                        {
                            if (!this.isTilePassable(new Vector2(i, j)))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            return new Rectangle((int)this.tileX.Value * 64, (int)this.tileY.Value * 64, (int)this.tilesWide.Value * 64, (int)this.tilesHigh.Value * 64).Intersects(boundingBox);
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.isTilePassable
        public override bool isTilePassable(Vector2 tile)
        {
            bool flag = this.occupiesTile(tile);
            if (flag && this.isUnderConstruction())
            {
                return false;
            }
            if (this.Model != null && this.IsInTilePropertyRadius(tile))
            {
                return this.Model.IsTilePassable((int)tile.X - this.tileX.Value, (int)tile.Y - this.tileY.Value);
            }
            return !flag;
        }


        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.IsInTilePropertyRadius
        public virtual bool IsInTilePropertyRadius(Vector2 tileLocation)
        {
            int additionalTilePropertyRadius = this.GetAdditionalTilePropertyRadius();
            if (tileLocation.X >= (float)((int)this.tileX.Value - additionalTilePropertyRadius) && tileLocation.X < (float)((int)this.tileX.Value + (int)this.tilesWide.Value + additionalTilePropertyRadius) && tileLocation.Y >= (float)((int)this.tileY.Value - additionalTilePropertyRadius))
            {
                return tileLocation.Y < (float)((int)this.tileY.Value + (int)this.tilesHigh.Value + additionalTilePropertyRadius);
            }
            return false;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.getIndoors
        protected override GameLocation getIndoors(string nameOfIndoorsWithoutUnique)
        {
            GameLocation gameLocation = null;
            if (this.indoors.Value is not null && this.Model.IndoorMapType == this.indoors.Value.GetType().ToString())
            {
                gameLocation = this.indoors.Value;
            }
            else if (this.Model != null && !string.IsNullOrEmpty(this.Model.IndoorMap))
            {
                Type type = typeof(GameLocation);
                try
                {
                    if (this.Model.IndoorMapType != null)
                    {
                        type = Type.GetType($"{this.Model.IndoorMapType},{this.Model.IndoorMapTypeAssembly}");
                    }
                }
                catch (Exception)
                {
                    type = typeof(GameLocation);
                }

                // Verify that the type isn't null, otherwise use the default value
                if (type is null)
                {
                    type = typeof(GameLocation);
                }

                try
                {
                    gameLocation = (GameLocation)Activator.CreateInstance(type, "Maps\\" + this.Model.IndoorMap, this.buildingType.Value);
                }
                catch (Exception)
                {
                    try
                    {
                        gameLocation = (GameLocation)Activator.CreateInstance(type, "Maps\\" + this.Model.IndoorMap);
                    }
                    catch (Exception arg)
                    {
                        SolidFoundations.monitor.Log($"Error trying to instantiate indoors for {this.buildingType}: {arg}", LogLevel.Warn);
                        gameLocation = new GameLocation("Maps\\" + nameOfIndoorsWithoutUnique, buildingType);
                    }
                }
            }

            if (gameLocation != null)
            {
                gameLocation.uniqueName.Value = nameOfIndoorsWithoutUnique + Guid.NewGuid().ToString();
                gameLocation.IsFarm = true;
                gameLocation.isStructure.Value = true;
                this.updateInteriorWarps(gameLocation);
            }

            return gameLocation;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.LoadFromBuildingData
        public virtual void LoadFromBuildingData(bool for_upgrade = false)
        {
            if (this.Model == null)
            {
                return;
            }
            this.tilesWide.Value = this.Model.Size.X;
            this.tilesHigh.Value = this.Model.Size.Y;
            this.humanDoor.X = this.Model.HumanDoor.X;
            this.humanDoor.Y = this.Model.HumanDoor.Y;
            this.animalDoor.Value = this.Model.GetAnimalDoorRect().Location;
            if (this.Model.MaxOccupants >= 0)
            {
                this.maxOccupants.Value = this.Model.MaxOccupants;
            }
            //this.hayCapacity.Value = this.Model.HayCapacity;
            this.magical.Value = this.Model.Builder == "Wizard";
            this.fadeWhenPlayerIsBehind.Value = this.Model.FadeWhenBehind;
            foreach (KeyValuePair<string, string> modDatum in this.Model.ModData)
            {
                this.modData[modDatum.Key] = modDatum.Value;
            }
            if (this.indoors.Value != null)
            {
                this.indoors.Value.InvalidateCachedMultiplayerMap(SolidFoundations.multiplayer.cachedMultiplayerMaps);
            }
            if (!Game1.IsMasterGame)
            {
                return;
            }
            if (this.hasLoaded && this.nonInstancedIndoors.Value == null)
            {
                string indoorMap = this.Model.IndoorMap;
                string text = typeof(GameLocation).ToString();
                if (this.Model.IndoorMapType != null)
                {
                    text = this.Model.IndoorMapType;
                }
                if (indoorMap != null)
                {
                    if (this.indoors.Value == null)
                    {
                        this.indoors.Value = this.getIndoors(this.getBuildingMapFileName(this.Model.Name));
                        this.InitializeIndoor(for_upgrade);
                    }
                    else if (this.indoors.Value.mapPath.Value == indoorMap)
                    {
                        if (for_upgrade)
                        {
                            this.InitializeIndoor(for_upgrade);
                        }
                    }
                    else
                    {
                        if (this.indoors.Value.GetType().ToString() != text)
                        {
                            this.load();
                        }
                        else
                        {
                            this.indoors.Value.mapPath.Value = "Maps\\" + indoorMap;
                            this.indoors.Value.updateMap();
                            base.indoors.Value.updateWarps();
                        }
                        this.updateInteriorWarps(this.indoors.Value);
                        this.InitializeIndoor(for_upgrade);
                    }
                }
            }

            HashSet<string> hashSet = new HashSet<string>();
            if (this.Model.Chests != null)
            {
                foreach (BuildingChest chest2 in this.Model.Chests)
                {
                    hashSet.Add(chest2.Name);
                }
            }
            for (int num = this.buildingChests.Count - 1; num >= 0; num--)
            {
                if (!hashSet.Contains(this.buildingChests[num].Name))
                {
                    this.buildingChests.RemoveAt(num);
                }
            }
            if (this.Model.Chests == null)
            {
                return;
            }
            foreach (BuildingChest chest3 in this.Model.Chests)
            {
                if (this.GetBuildingChest(chest3.Name) == null)
                {
                    Chest chest = new Chest(playerChest: true);
                    chest.Name = chest3.Name;
                    this.buildingChests.Add(chest);
                }
            }
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.GetBuildingChest
        protected override string getBuildingMapFileName(string name)
        {
            if (this.Model != null)
            {
                return this.Model.IndoorMap;
            }
            return name switch
            {
                "Slime Hutch" => "SlimeHutch",
                "Big Coop" => "Coop2",
                "Deluxe Coop" => "Coop3",
                "Big Barn" => "Barn2",
                "Deluxe Barn" => "Barn3",
                "Big Shed" => "Shed2",
                _ => name,
            };
        }


        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.GetBuildingChest
        public Chest GetBuildingChest(string name)
        {
            foreach (Chest buildingChest in this.buildingChests)
            {
                if (buildingChest.Name == name)
                {
                    return buildingChest;
                }
            }
            return null;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.GetBuildingChestData
        public BuildingChest GetBuildingChestData(string name)
        {
            if (this.Model == null)
            {
                return null;
            }

            foreach (BuildingChest chest in this.Model.Chests)
            {
                if (chest.Name == name)
                {
                    return chest;
                }
            }
            return null;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.ShouldDrawShadow
        public bool ShouldDrawShadow()
        {
            if (this.Model != null && !this.Model.DrawShadow)
            {
                return false;
            }
            return true;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.GetItemConversionForItem
        public BuildingItemConversion GetItemConversionForItem(Item item, Chest chest)
        {
            if (this.Model == null)
            {
                return null;
            }
            if (item == null)
            {
                return null;
            }
            if (chest == null)
            {
                return null;
            }
            foreach (BuildingItemConversion itemConversion in this.Model.ItemConversions)
            {
                if (!(itemConversion.SourceChest == chest.Name))
                {
                    continue;
                }
                bool flag = false;
                foreach (string requiredTag in itemConversion.RequiredTags)
                {
                    if (!item.HasContextTag(requiredTag))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return itemConversion;
                }
            }
            return null;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.IsValidObjectForChest
        public bool IsValidObjectForChest(Item item, Chest chest)
        {
            return this.GetItemConversionForItem(item, chest) != null;
        }

        // Preserve this custom implementation
        public bool IsObjectFilteredForChest(Item inputItem, Chest chest, bool performSilentCheck = false)
        {
            if (this.Model == null || this.Model.InputFilters is null)
            {
                return false;
            }
            if (inputItem == null)
            {
                return true;
            }
            if (chest == null)
            {
                return true;
            }

            foreach (InputFilter inputFilter in this.Model.InputFilters)
            {
                if (chest.Name != inputFilter.InputChest)
                {
                    continue;
                }

                bool isFiltered = false;
                foreach (RestrictedItem restriction in inputFilter.RestrictedItems)
                {
                    if (restriction.RequiredTags.Any(t => inputItem.HasContextTag(t) is false))
                    {
                        continue;
                    }

                    if (restriction.RejectWhileProcessing && this.Model.ItemConversions is not null && this.Model.ItemConversions.Any(c => c.ShouldTrackTime))
                    {
                        isFiltered = true;
                        break;
                    }
                    else if (String.IsNullOrEmpty(restriction.Condition) is false && this.ValidateConditions(restriction.Condition, restriction.ModDataFlags))
                    {
                        isFiltered = true;
                        break;
                    }
                    else if (restriction.ModDataFlags is not null && this.ValidateConditions(restriction.Condition, restriction.ModDataFlags))
                    {
                        isFiltered = true;
                        break;
                    }

                    if (restriction.MaxAllowed >= 0)
                    {
                        int currentTagStackCount = 0;
                        foreach (var chestItem in chest.items.Where(i => i is not null))
                        {
                            if (restriction.RequiredTags.Any(t => chestItem.HasContextTag(t) is false))
                            {
                                continue;
                            }

                            currentTagStackCount += chestItem.Stack;
                            if (currentTagStackCount > restriction.MaxAllowed)
                            {
                                isFiltered = true;
                                break;
                            }
                        }
                    }
                }

                if (isFiltered)
                {
                    if (performSilentCheck is false && inputFilter.FilteredItemMessage != null)
                    {
                        Game1.showRedMessage(TextParser.ParseText(this.Model.GetTranslation(inputFilter.FilteredItemMessage)));
                    }

                    return true;
                }
            }

            return false;
        }

        // Preserve this custom implementation
        public int GetMaxAllowedInChest(Item inputItem, Chest chest)
        {
            if (inputItem == null)
            {
                return -1;
            }
            if (chest == null)
            {
                return inputItem.Stack;
            }
            if (this.Model == null || this.Model.InputFilters is null)
            {
                return inputItem.Stack;
            }

            foreach (InputFilter inputFilter in this.Model.InputFilters)
            {
                if (chest.Name != inputFilter.InputChest)
                {
                    continue;
                }

                foreach (RestrictedItem restriction in inputFilter.RestrictedItems)
                {
                    if (restriction.RequiredTags.Any(t => inputItem.HasContextTag(t) is false))
                    {
                        continue;
                    }

                    if (restriction.MaxAllowed >= 0 && inputItem.Stack > restriction.MaxAllowed)
                    {
                        int currentTagStackCount = 0;
                        foreach (var chestItem in chest.items.Where(i => i is not null))
                        {
                            if (restriction.RequiredTags.Any(t => chestItem.HasContextTag(t) is false))
                            {
                                continue;
                            }

                            currentTagStackCount += chestItem.Stack;
                            if (currentTagStackCount > restriction.MaxAllowed)
                            {
                                return restriction.MaxAllowed;
                            }
                        }

                        if (currentTagStackCount + inputItem.Stack > restriction.MaxAllowed)
                        {
                            return restriction.MaxAllowed - currentTagStackCount;
                        }
                    }
                }
            }

            return inputItem.Stack;
        }

        // TODO: When updated to SDV v1.6, this method (StardewValley.Buildings.Building.PerformBuildingChestAction) should be patched to utilize fixes
        // Current fixes:
        // - Only take required amount from player's active stack, instead of all that can fit within chest
        public bool PerformBuildingChestAction(string name, Farmer who)
        {
            Chest chest = this.GetBuildingChest(name);
            if (chest == null)
            {
                return false;
            }

            BuildingChest buildingChestData = this.GetBuildingChestData(name);
            if (buildingChestData == null)
            {
                return false;
            }
            if (buildingChestData.Type == BuildingChest.ChestType.Chest)
            {
                Game1.activeClickableMenu = new ItemGrabMenu(chest.items, reverseGrab: false, showReceivingMenu: true, (Item item) => this.IsValidObjectForChest(item, chest), chest.grabItemFromInventory, null, chest.grabItemFromChest, snapToBottom: false, canBeExitedWithKey: true, playRightClickSound: true, allowRightClick: true, showOrganizeButton: true, 1, null, -1, this);
                (Game1.activeClickableMenu as ItemGrabMenu).inventory.moveItemSound = buildingChestData.Sound;
                return true;
            }
            if (buildingChestData.Type == BuildingChest.ChestType.Load)
            {
                if (who != null && who.ActiveObject != null)
                {
                    bool flag = false;
                    if (this.IsValidObjectForChest(who.ActiveObject, chest))
                    {
                        flag = true;
                    }
                    if (!flag)
                    {
                        if (buildingChestData.InvalidItemMessage != null)
                        {
                            Game1.showRedMessage(TextParser.ParseText(this.Model.GetTranslation(buildingChestData.InvalidItemMessage)));
                        }
                        return false;
                    }
                    if (this.IsObjectFilteredForChest(who.ActiveObject, chest))
                    {
                        return false;
                    }
                    BuildingItemConversion itemConversionForItem = this.GetItemConversionForItem(who.ActiveObject, chest);
                    Utility.consolidateStacks(chest.items);
                    chest.clearNulls();
                    int numberOfItemThatCanBeAddedToThisInventoryList = Toolkit.GetNumberOfItemThatCanBeAddedToThisInventoryList(who.ActiveObject, chest.items, chest.GetActualCapacity());
                    if (who.ActiveObject.Stack > itemConversionForItem.RequiredCount && numberOfItemThatCanBeAddedToThisInventoryList < itemConversionForItem.RequiredCount)
                    {
                        Game1.showRedMessage(TextParser.ParseText(this.Model.GetTranslation(buildingChestData.ChestFullMessage)));
                        return false;
                    }
                    int num = Math.Min(Math.Min(numberOfItemThatCanBeAddedToThisInventoryList, who.ActiveObject.Stack), itemConversionForItem.RequiredCount);
                    if (num == 0)
                    {
                        if (buildingChestData.InvalidCountMessage != null)
                        {
                            Game1.showRedMessage(TextParser.ParseText(this.Model.GetTranslation(buildingChestData.InvalidCountMessage)));
                        }
                        return false;
                    }
                    Item one = who.ActiveObject.getOne();
                    Object @object = (Object)Toolkit.ConsumeStack(who.ActiveObject, num);
                    who.ActiveObject = null;
                    if (@object != null)
                    {
                        who.ActiveObject = @object;
                    }
                    one.Stack = num;
                    if (Utility.addItemToThisInventoryList(one, chest.items, chest.GetActualCapacity()) is not null)
                    {
                        Game1.showRedMessage(TextParser.ParseText(this.Model.GetTranslation(buildingChestData.ChestFullMessage)));
                        return false;
                    }

                    if (buildingChestData.Sound != null)
                    {
                        Game1.playSound(buildingChestData.Sound);
                    }
                }
                return true;
            }
            if (buildingChestData.Type == BuildingChest.ChestType.Collect)
            {
                Utility.CollectSingleItemOrShowChestMenu(chest);
                return true;
            }
            return false;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.load
        public override void load()
        {
            if (!Game1.IsMasterGame)
            {
                return;
            }
            if (!this.hasLoaded)
            {
                this.hasLoaded = true;
                if (this.Model != null)
                {
                    if (this.Model.NonInstancedIndoorLocation == null && this.nonInstancedIndoors.Value != null)
                    {
                        this.nonInstancedIndoors.Value = null;
                        //this.nonInstancedIndoors.Value.parentLocation.Value = null;
                    }
                    else if (this.Model.NonInstancedIndoorLocation != null)
                    {
                        GameLocation locationFromName = Game1.getLocationFromName(this.Model.NonInstancedIndoorLocation);
                        bool flag = false;
                        foreach (BuildableGameLocation location in Game1.locations.Where(b => b is BuildableGameLocation))
                        {
                            foreach (GenericBuilding building in location.buildings.Where(b => b is GenericBuilding))
                            {
                                if (building.Model != null && building.nonInstancedIndoors.Value == locationFromName)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        if (!flag)
                        {
                            this.nonInstancedIndoors.Value = locationFromName;
                        }
                    }
                }
                this.LoadFromBuildingData();
            }
            if (this.nonInstancedIndoors.Value != null)
            {
                //this.UpdateIndoorParent();
            }
            else
            {
                GameLocation gameLocation = this.getIndoors(this.getBuildingMapFileName(this.Model.Name));
                if (gameLocation != null && this.indoors.Value != null)
                {
                    gameLocation.characters.Set(this.indoors.Value.characters);
                    gameLocation.netObjects.MoveFrom(this.indoors.Value.netObjects);
                    gameLocation.terrainFeatures.MoveFrom(this.indoors.Value.terrainFeatures);
                    gameLocation.IsFarm = true;
                    gameLocation.IsOutdoors = false;
                    gameLocation.isStructure.Value = true;
                    gameLocation.miniJukeboxCount.Set(this.indoors.Value.miniJukeboxCount.Value);
                    gameLocation.miniJukeboxTrack.Set(this.indoors.Value.miniJukeboxTrack.Value);
                    gameLocation.uniqueName.Value = this.indoors.Value.uniqueName;
                    if (gameLocation.uniqueName.Value == null)
                    {
                        gameLocation.uniqueName.Value = this.nameOfIndoorsWithoutUnique + ((int)this.tileX * 2000 + (int)this.tileY);
                    }
                    gameLocation.numberOfSpawnedObjectsOnMap = this.indoors.Value.numberOfSpawnedObjectsOnMap;
                    if (this.indoors.Value is AnimalHouse indoorAnimalHouse && gameLocation is AnimalHouse animalHouse)
                    {
                        animalHouse.animals.MoveFrom(indoorAnimalHouse.animals);
                        ((AnimalHouse)gameLocation).animalsThatLiveHere.Set(((AnimalHouse)this.indoors.Value).animalsThatLiveHere);

                        foreach (KeyValuePair<long, FarmAnimal> pair in animalHouse.animals.Pairs)
                        {
                            pair.Value.reload(this);
                        }
                    }
                    if (this.indoors.Value != null)
                    {
                        gameLocation.furniture.Set(this.indoors.Value.furniture);
                        foreach (Furniture item in gameLocation.furniture)
                        {
                            item.updateDrawPosition();
                        }
                    }
                    if (this.indoors.Value is Cabin && gameLocation is Cabin)
                    {
                        Cabin obj = gameLocation as Cabin;
                        obj.fridge.Value = (this.indoors.Value as Cabin).fridge.Value;
                        obj.farmhand.Set((this.indoors.Value as Cabin).farmhand);
                    }
                    if (this.indoors.Value != null)
                    {
                        //base.indoors.Value.mapPath.Value = "Maps\\" + this.Model.IndoorMap;
                        //base.indoors.Value.loadMap(base.indoors.Value.mapPath.Value, true);
                        gameLocation.TransferDataFromSavedLocation(this.indoors.Value);
                    }
                    this.indoors.Value = gameLocation;
                    gameLocation = null;
                }
                this.updateInteriorWarps();
                if (this.indoors.Value != null)
                {
                    for (int num = this.indoors.Value.characters.Count - 1; num >= 0; num--)
                    {
                        SaveGame.initializeCharacter(this.indoors.Value.characters[num], this.indoors.Value);
                    }
                    foreach (TerrainFeature value in this.indoors.Value.terrainFeatures.Values)
                    {
                        value.loadSprite();
                    }
                    foreach (KeyValuePair<Vector2, Object> pair2 in this.indoors.Value.objects.Pairs)
                    {
                        pair2.Value.initializeLightSource(pair2.Key);
                        pair2.Value.reloadSprite();
                    }
                }
                if (this.Model == null && this.IndoorOrInstancedIndoor is AnimalHouse)
                {
                    AnimalHouse animalHouse = this.IndoorOrInstancedIndoor as AnimalHouse;
                    string text = this.buildingType.Value.Split(' ')[0];
                    if (!(text == "Big"))
                    {
                        if (text == "Deluxe")
                        {
                            animalHouse.animalLimit.Value = 12;
                        }
                        else
                        {
                            animalHouse.animalLimit.Value = 4;
                        }
                    }
                    else
                    {
                        animalHouse.animalLimit.Value = 8;
                    }
                }
            }
            this.InitializeIndoor(false);

            BluePrint bluePrint = new BluePrint(this.buildingType.Value);
            if (bluePrint != null)
            {
                this.humanDoor.X = bluePrint.humanDoor.X;
                this.humanDoor.Y = bluePrint.humanDoor.Y;
                this.additionalPlacementTiles.Clear();
                this.additionalPlacementTiles.AddRange(bluePrint.additionalPlacementTiles);
            }
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.InitializeIndoor
        public virtual void InitializeIndoor(bool for_upgrade)
        {
            if (this.Model == null || this.indoors.Value == null)
            {
                return;
            }
            if (this.IndoorOrInstancedIndoor is AnimalHouse && this.Model.MaxOccupants > 0)
            {
                (this.IndoorOrInstancedIndoor as AnimalHouse).animalLimit.Value = this.Model.MaxOccupants;
            }
            if (for_upgrade && this.Model.IndoorItemMoves != null)
            {
                foreach (IndoorItemMove indoorItemMove in this.Model.IndoorItemMoves)
                {
                    for (int i = 0; i < indoorItemMove.Size.X; i++)
                    {
                        for (int j = 0; j < indoorItemMove.Size.Y; j++)
                        {
                            this.IndoorOrInstancedIndoor.moveObject(indoorItemMove.Source.X + i, indoorItemMove.Source.Y + j, indoorItemMove.Destination.X + i, indoorItemMove.Destination.Y + j);
                        }
                    }
                }
            }
            if (this.Model.IndoorItems == null)
            {
                return;
            }
            foreach (IndoorItemAdd indoorItem in this.Model.IndoorItems)
            {
                Point tile = indoorItem.Tile;
                Vector2 vectorizedTile = Utility.PointToVector2(tile);
                if (this.IndoorOrInstancedIndoor.objects.ContainsKey(Utility.PointToVector2(tile)) || this.IndoorOrInstancedIndoor.furniture.Any(f => f.TileLocation.X == vectorizedTile.X && f.TileLocation.Y == vectorizedTile.Y))
                {
                    continue;
                }

                var item = Toolkit.CreateItemByID(indoorItem.ItemID, 1, 0);
                if (item is not null && item is StardewValley.Object obj)
                {
                    if (indoorItem.Indestructible)
                    {
                        obj.Fragility = 2;
                    }

                    if (obj is Furniture)
                    {
                        var furniture = Furniture.GetFurnitureInstance(obj.ParentSheetIndex, vectorizedTile);
                        furniture.Fragility = obj.Fragility;
                        this.IndoorOrInstancedIndoor.furniture.Add(furniture);
                    }
                    else
                    {
                        obj.TileLocation = Utility.PointToVector2(tile);
                        this.IndoorOrInstancedIndoor.objects.Add(vectorizedTile, obj);
                    }
                }
            }
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.InitializeIndoor
        public override void performActionOnConstruction(GameLocation location)
        {
            base.performActionOnConstruction(location);

            if (this.Model != null && this.Model.AddMailOnBuild != null)
            {
                foreach (string item in this.Model.AddMailOnBuild)
                {
                    Game1.addMail(item, noLetter: false, sendToEveryone: true);
                }
            }
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.upgrade
        public override void upgrade()
        {
            base.upgrade();

            if (this.Model != null && this.Model.AddMailOnBuild != null)
            {
                foreach (string item in this.Model.AddMailOnBuild)
                {
                    Game1.addMail(item, noLetter: false, sendToEveryone: true);
                }
            }
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.getUpgradeSignLocation
        public override Vector2 getUpgradeSignLocation()
        {
            if (this.Model != null && this.Model.UpgradeSignTile.X >= 0f)
            {
                return new Vector2((float)(int)this.tileX + this.Model.UpgradeSignTile.X, (float)(int)this.tileY + this.Model.UpgradeSignTile.Y) * 64f + new Vector2(0f, -4f * this.Model.UpgradeSignHeight);
            }
            if (this.IndoorOrInstancedIndoor != null && this.IndoorOrInstancedIndoor is Shed)
            {
                return new Vector2((int)this.tileX + 5, (int)this.tileY + 1) * 64f + new Vector2(-12f, -16f);
            }
            return new Vector2((int)this.tileX * 64 + 32, (int)this.tileY * 64 - 32);
        }


        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.resetTexture
        public override void resetTexture()
        {
            this.texture = new Lazy<Texture2D>(delegate
            {
                if (this.paintedTexture != null)
                {
                    this.paintedTexture.Dispose();
                    this.paintedTexture = null;
                }
                string text = this.textureName();
                Texture2D texture2D;
                try
                {
                    texture2D = Game1.content.Load<Texture2D>(text);
                }
                catch (Exception)
                {
                    return Game1.content.Load<Texture2D>("Buildings\\Shipping Bin");
                }
                this.paintedTexture = BuildingPainter.Apply(texture2D, text + "_PaintMask", this.netBuildingPaintColor.Value);
                if (this.paintedTexture != null)
                {
                    texture2D = this.paintedTexture;
                }
                return texture2D;
            });
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.textureName
        public override string textureName()
        {
            if (this.Model != null)
            {
                if (this.Model.Skins != null && this.Model.Skins.Count > 0 && this.skinID.Value != null)
                {
                    foreach (BuildingSkin skin in this.Model.Skins)
                    {
                        if (skin.ID == this.skinID.Value)
                        {
                            return skin.Texture;
                        }
                    }
                }
                return this.Model.Texture;
            }

            return "Buildings\\" + (string)this.buildingType.Value;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted
        public override bool CanBePainted()
        {
            var paintDataKey = base.buildingType.Value;
            if (String.IsNullOrEmpty(this.skinID.Value) is false)
            {
                paintDataKey = this.skinID.Value;
            }

            Dictionary<string, string> paintData = Game1.content.Load<Dictionary<string, string>>("Data\\PaintData");
            if (paintData.ContainsKey(paintDataKey))
            {
                return true;
            }

            return false;
        }

        // Preserve this override when updated to SDV v1.6, but call the base draw method if ExtendedBuildingModel
        public override void updateInteriorWarps(GameLocation interior = null)
        {
            if (interior is null)
            {
                interior = this.IndoorOrInstancedIndoor;
            }

            if (interior is not null && this.Model is not null)
            {
                var targetLocation = FlexibleLocationFinder.GetBuildableLocationByName("Farm");
                var targetName = this.buildingLocation.Value is null ? targetLocation.NameOrUniqueName : this.buildingLocation.Value.NameOrUniqueName;
                var baseX = Model.HumanDoor.X;
                var baseY = Model.HumanDoor.Y;

                if (this.Model.TunnelDoors.Count > 0)
                {
                    var firstTunnelDoor = this.Model.TunnelDoors.First();
                    baseX = firstTunnelDoor.X;
                    baseY = firstTunnelDoor.Y;
                }

                if (interior.warps is not null && interior.warps.Count == 0)
                {
                    interior.warps.Add(new Warp(0, 0, targetName, baseX + (int)this.tileX.Value, baseY + (int)this.tileY.Value + 1, false));
                }

                foreach (Warp warp in interior.warps)
                {
                    warp.TargetName = targetName;
                    warp.TargetX = baseX + (int)this.tileX.Value;
                    warp.TargetY = baseY + (int)this.tileY.Value + 1;
                }
            }
        }

        // Preserve this override when updated to SDV v1.6, but call the base dayUpdate method if ExtendedBuildingModel
        public override void dayUpdate(int dayOfMonth)
        {
            var targetLocation = this.buildingLocation.Value is BuildableGameLocation buildableGameLocation && buildableGameLocation is not null ? buildableGameLocation : FlexibleLocationFinder.GetBuildableLocationByName("Farm");
            if ((int)this.daysOfConstructionLeft.Value > 0 && !Utility.isFestivalDay(dayOfMonth, Game1.currentSeason))
            {
                this.daysOfConstructionLeft.Value--;
                if ((int)this.daysOfConstructionLeft.Value > 0)
                {
                    return;
                }
                Game1.player.checkForQuestComplete(null, -1, -1, null, this.buildingType, 8);
                if (this.buildingType.Equals("Slime Hutch") && this.indoors.Value != null)
                {
                    this.indoors.Value.objects.Add(new Vector2(1f, 4f), new Object(new Vector2(1f, 4f), 156)
                    {
                        Fragility = 2
                    });
                    if (!Game1.player.mailReceived.Contains("slimeHutchBuilt"))
                    {
                        Game1.player.mailReceived.Add("slimeHutchBuilt");
                    }
                }
            }

            if ((int)this.daysUntilUpgrade.Value > 0 && !Utility.isFestivalDay(dayOfMonth, Game1.currentSeason))
            {
                this.daysUntilUpgrade.Value--;
                if ((int)this.daysUntilUpgrade.Value <= 0)
                {
                    targetLocation.buildings.Remove(this);
                    string upgradeName = this.upgradeName.Value;

                    if (SolidFoundations.buildingManager.GetSpecificBuildingModel(upgradeName) is ExtendedBuildingModel model && model is not null)
                    {
                        Game1.player.checkForQuestComplete(null, -1, -1, null, upgradeName, 8);
                        RefreshModel(SolidFoundations.buildingManager.GetSpecificBuildingModel(upgradeName));

                        BluePrint bluePrint = new BluePrint(upgradeName);
                        if (bluePrint is not null)
                        {
                            this.buildingType.Value = bluePrint.name;
                            if (this.indoors.Value is AnimalHouse)
                            {
                                ((AnimalHouse)(GameLocation)this.indoors).resetPositionsOfAllAnimals();
                                ((AnimalHouse)(GameLocation)this.indoors).loadLights();
                            }
                            this.upgrade();
                            this.resetTexture();
                            this.updateInteriorWarps();
                        }
                    }

                    targetLocation.buildings.Add(this);
                }
            }

            this.ProcessItemConversions(0, true);
        }

        public override void performTenMinuteAction(int timeElapsed)
        {
            base.performTenMinuteAction(timeElapsed);

            // Update building item conversions
            this.ProcessItemConversions(timeElapsed, false);

            // Handle any sub-buildings
            if (this.indoors.Value is BuildableGameLocation buildableGameLocation && buildableGameLocation is not null)
            {
                foreach (var building in buildableGameLocation.buildings)
                {
                    building.performTenMinuteAction(timeElapsed);
                    if (building.indoors.Value != null && !Game1.locations.Contains(building.indoors.Value) && timeElapsed >= 10)
                    {
                        building.indoors.Value.performTenMinuteUpdate(Game1.timeOfDay);
                        if (timeElapsed > 10)
                        {
                            building.indoors.Value.passTimeForObjects(timeElapsed - 10);
                        }
                    }
                }
            }
        }

        // Preserve this override when updated to SDV v1.6, but call the base draw method if ExtendedBuildingModel.
        public override void draw(SpriteBatch b)
        {
            if (this.isMoving)
            {
                return;
            }
            if ((int)this.daysOfConstructionLeft.Value > 0 || (int)this.newConstructionTimer.Value > 0)
            {
                this.drawInConstruction(b);
                return;
            }
            if (this.ShouldDrawShadow())
            {
                this.drawShadow(b);
            }

            float num = ((int)this.tileY.Value + (int)this.tilesHigh.Value) * 64;
            float num2 = num;
            if (this.Model != null)
            {
                num2 -= this.Model.SortTileOffset * 64f;
            }
            num2 /= 10000f;
            Vector2 vector = new Vector2((int)this.tileX.Value * 64, (int)this.tileY.Value * 64 + (int)this.tilesHigh.Value * 64);
            Vector2 vector2 = Vector2.Zero;
            if (this.Model != null)
            {
                vector2 = this.Model.DrawOffset * 4f;
            }
            Vector2 vector3 = new Vector2(0f, this.getSourceRect().Height);

            if (this.Model is not null && this.Model.DrawLayers is not null)
            {
                foreach (ExtendedBuildingDrawLayer drawLayer in this.Model.DrawLayers.Where(d => d.DrawBehindBase is true))
                {
                    if (ValidateLayer(drawLayer) is false)
                    {
                        continue;
                    }

                    Rectangle sourceRect = drawLayer.GetSourceRect((int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds, this);
                    sourceRect = this.ApplySourceRectOffsets(sourceRect);
                    vector2 = Vector2.Zero;
                    if (drawLayer.AnimalDoorOffset != Point.Zero)
                    {
                        vector2 = new Vector2((float)drawLayer.AnimalDoorOffset.X * this.animalDoorOpenAmount.Value, (float)drawLayer.AnimalDoorOffset.Y * this.animalDoorOpenAmount.Value);
                    }
                    Texture2D texture2D = this.texture.Value;
                    if (drawLayer.Texture != null)
                    {
                        texture2D = Game1.content.Load<Texture2D>(drawLayer.Texture);
                    }
                    b.Draw(texture2D, Game1.GlobalToLocal(Game1.viewport, vector + (vector2 - vector3 + drawLayer.DrawPosition) * 4f), sourceRect, this.color.Value * this.alpha.Value, 0f, new Vector2(0f, 0f), 4f, SpriteEffects.None, num2);
                    num2 += 0.00001f;
                }
            }
            if (this.Model is null || this.Model.DrawLayers is null || this.Model.DrawLayers.Count(l => l is not null && l.HideBaseTexture && ValidateLayer(l)) == 0)
            {
                b.Draw(this.texture.Value, Game1.GlobalToLocal(Game1.viewport, vector + vector2), this.getSourceRect(), this.color.Value * this.alpha.Value, 0f, vector3, 4f, SpriteEffects.None, num2);
            }
            if ((bool)this.magical.Value && this.buildingType.Value.Equals("Gold Clock"))
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)this.tileX.Value * 64 + 92, (int)this.tileY.Value * 64 - 40)), Town.hourHandSource, Color.White * this.alpha.Value, (float)(Math.PI * 2.0 * (double)((float)(Game1.timeOfDay % 1200) / 1200f) + (double)((float)Game1.gameTimeInterval / 7000f / 23f)), new Vector2(2.5f, 8f), 3f, SpriteEffects.None, (float)(((int)this.tileY.Value + (int)this.tilesHigh.Value) * 64) / 10000f + 0.0001f);
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)this.tileX.Value * 64 + 92, (int)this.tileY.Value * 64 - 40)), Town.minuteHandSource, Color.White * this.alpha.Value, (float)(Math.PI * 2.0 * (double)((float)(Game1.timeOfDay % 1000 % 100 % 60) / 60f) + (double)((float)Game1.gameTimeInterval / 7000f * 1.02f)), new Vector2(2.5f, 12f), 3f, SpriteEffects.None, (float)(((int)this.tileY.Value + (int)this.tilesHigh.Value) * 64) / 10000f + 0.00011f);
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)this.tileX.Value * 64 + 92, (int)this.tileY.Value * 64 - 40)), Town.clockNub, Color.White * this.alpha.Value, 0f, new Vector2(2f, 2f), 4f, SpriteEffects.None, (float)(((int)this.tileY.Value + (int)this.tilesHigh.Value) * 64) / 10000f + 0.00012f);
            }
            if (this.Model != null)
            {
                foreach (Chest buildingChest2 in this.buildingChests)
                {
                    BuildingChest buildingChestData = this.GetBuildingChestData(buildingChest2.Name);
                    if (buildingChestData.DisplayTile.X != -1f && buildingChestData.DisplayTile.Y != -1f && buildingChest2.items.Count > 0 && buildingChest2.items[0] != null)
                    {
                        num2 = ((float)(int)this.tileY.Value + buildingChestData.DisplayTile.Y + 1f) * 64f;
                        num2 += 1f;
                        float num3 = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2) - buildingChestData.DisplayHeight * 64f;
                        float num4 = ((float)(int)this.tileX.Value + buildingChestData.DisplayTile.X) * 64f;
                        float num5 = ((float)(int)this.tileY.Value + buildingChestData.DisplayTile.Y - 1f) * 64f;
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(num4, num5 + num3)), new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, num2 / 10000f);

                        // TODO: Display item texture here
                        //ParsedItemData itemDataForItemID = Utility.GetItemDataForItemID(buildingChest2.items[0].QualifiedItemID);
                        b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(num4 + 32f + 4f, num5 + 32f + num3)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, buildingChest2.items[0].ParentSheetIndex, 16, 16), Color.White * 0.75f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (num2 + 1f) / 10000f);
                    }
                }
                if (this.Model.DrawLayers != null)
                {
                    foreach (ExtendedBuildingDrawLayer drawLayer in this.Model.DrawLayers.Where(d => d.DrawBehindBase is false))
                    {
                        if (drawLayer.DrawInBackground)
                        {
                            continue;
                        }
                        if (ValidateLayer(drawLayer) is false)
                        {
                            continue;
                        }

                        num2 = num - drawLayer.SortTileOffset * 64f;
                        num2 += 1f;
                        num2 /= 10000f;
                        Rectangle sourceRect = drawLayer.GetSourceRect((int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds, this);
                        sourceRect = this.ApplySourceRectOffsets(sourceRect);
                        vector2 = Vector2.Zero;
                        if (drawLayer.AnimalDoorOffset != Point.Zero)
                        {
                            vector2 = new Vector2((float)drawLayer.AnimalDoorOffset.X * this.animalDoorOpenAmount.Value, (float)drawLayer.AnimalDoorOffset.Y * this.animalDoorOpenAmount.Value);
                        }
                        Texture2D texture2D = this.texture.Value;
                        if (drawLayer.Texture != null)
                        {
                            texture2D = Game1.content.Load<Texture2D>(drawLayer.Texture);
                        }
                        b.Draw(texture2D, Game1.GlobalToLocal(Game1.viewport, vector + (vector2 - vector3 + drawLayer.DrawPosition) * 4f), sourceRect, this.color.Value * this.alpha.Value, 0f, new Vector2(0f, 0f), 4f, SpriteEffects.None, num2);
                    }
                }
            }
            if ((int)this.daysUntilUpgrade.Value <= 0)
            {
                return;
            }
            if (this.Model != null)
            {
                if (this.Model.UpgradeSignTile.X >= 0f)
                {
                    num2 = ((float)(int)this.tileY.Value + this.Model.UpgradeSignTile.Y + 1f) * 64f;
                    num2 += 2f;
                    num2 /= 10000f;
                    b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.getUpgradeSignLocation()), new Rectangle(367, 309, 16, 15), Color.White * this.alpha.Value, 0f, Vector2.Zero, 4f, SpriteEffects.None, num2);
                }
            }
            else if (this.indoors.Value != null && this.indoors.Value is Shed)
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, this.getUpgradeSignLocation()), new Rectangle(367, 309, 16, 15), Color.White * this.alpha.Value, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(((int)this.tileY.Value + (int)this.tilesHigh.Value) * 64) / 10000f + 0.0001f);
            }
        }

        private void AdjustForLargeBuildings(Rectangle sourceRect, ref float adjustedScale, ref Vector2 adjustedOrigin, ref int windowYOffset, ref int windowHeightOffset)
        {
            if (sourceRect.Height + sourceRect.Width > 300)
            {
                adjustedScale = 2f;
                adjustedOrigin = new Vector2(-(this.tilesWide.Value * adjustedScale * 4), -(this.tilesHigh.Value * adjustedScale * 4));
            }
            else if (sourceRect.Height + sourceRect.Width > 250)
            {
                adjustedScale = 3f;
                adjustedOrigin = new Vector2(-(this.tilesWide.Value * adjustedScale), -(this.tilesWide.Value * adjustedScale));
            }

            if (sourceRect.Height * adjustedScale > 514)
            {
                windowHeightOffset = (int)(sourceRect.Height * adjustedScale) - 514;
                windowYOffset = (int)((windowHeightOffset) * adjustedScale);
            }
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.drawInMenu
        public override void drawInMenu(SpriteBatch b, int x, int y)
        {
            if (this.Model != null)
            {
                x += (int)(this.Model.DrawOffset.X * 4f);
                y += (int)(this.Model.DrawOffset.Y * 4f);
            }

            float num = (int)this.tilesHigh.Value * 64;
            float num2 = num;
            if (this.Model != null)
            {
                num2 -= this.Model.SortTileOffset * 64f;
            }
            num2 /= 10000f;
            if (this.ShouldDrawShadow())
            {
                this.drawShadow(b, x, y);
            }

            var adjustedScale = 4f;
            var adjustedOrigin = new Vector2(0f, 0f);
            var windowYOffset = 0;
            var windowHeightOffset = 0;

            var buildingRectangle = this.getSourceRect();
            AdjustForLargeBuildings(buildingRectangle, ref adjustedScale, ref adjustedOrigin, ref windowYOffset, ref windowHeightOffset);

            if (this.Model != null && this.Model.DrawLayers != null)
            {
                foreach (var drawLayer in this.Model.DrawLayers.Where(l => l.DrawBehindBase is true))
                {
                    if (ValidateLayer(drawLayer) is false)
                    {
                        continue;
                    }

                    num2 = num - drawLayer.SortTileOffset * 64f;
                    num2 += 1f;
                    num2 /= 10000f;

                    Microsoft.Xna.Framework.Rectangle sourceRect = drawLayer.GetSourceRect((int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds, this);
                    sourceRect = this.ApplySourceRectOffsets(sourceRect);
                    Texture2D texture2D = this.texture.Value;
                    if (drawLayer.Texture != null)
                    {
                        texture2D = Game1.content.Load<Texture2D>(drawLayer.Texture);
                    }
                    b.Draw(texture2D, new Vector2(x, y) + drawLayer.DrawPosition * adjustedScale, sourceRect, Color.White, 0f, adjustedOrigin, adjustedScale, SpriteEffects.None, num2);
                }
            }

            if (this.Model is null || this.Model.DrawLayers is null || this.Model.DrawLayers.Count(l => l is not null && l.HideBaseTexture && ValidateLayer(l)) == 0)
            {
                b.Draw(this.texture.Value, new Vector2(x, y + windowYOffset), new Rectangle(buildingRectangle.X, buildingRectangle.Y + windowHeightOffset, buildingRectangle.Width, buildingRectangle.Height - windowHeightOffset), this.color, 0f, adjustedOrigin, adjustedScale, SpriteEffects.None, num2);
            }
            if (this.Model != null && this.Model.DrawLayers != null)
            {
                foreach (var drawLayer in this.Model.DrawLayers.Where(l => l.DrawBehindBase is false))
                {
                    if (ValidateLayer(drawLayer) is false)
                    {
                        continue;
                    }

                    num2 = num - drawLayer.SortTileOffset * 64f;
                    num2 += 1f;

                    var actualLayer = num2;
                    if (drawLayer.DrawInBackground)
                    {
                        actualLayer = 0f;
                    }
                    else
                    {
                        num2 /= 10000f;
                        actualLayer = num2;
                    }

                    Microsoft.Xna.Framework.Rectangle sourceRect = drawLayer.GetSourceRect((int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds, this);
                    sourceRect = this.ApplySourceRectOffsets(sourceRect);
                    Texture2D texture2D = this.texture.Value;
                    if (drawLayer.Texture != null)
                    {
                        texture2D = Game1.content.Load<Texture2D>(drawLayer.Texture);
                    }

                    AdjustForLargeBuildings(sourceRect, ref adjustedScale, ref adjustedOrigin, ref windowYOffset, ref windowHeightOffset);
                    b.Draw(texture2D, new Vector2(x, y + windowYOffset) + drawLayer.DrawPosition * adjustedScale, new Rectangle(sourceRect.X, sourceRect.Y + windowHeightOffset, sourceRect.Width, sourceRect.Height - windowHeightOffset), Color.White, 0f, adjustedOrigin, adjustedScale, SpriteEffects.None, actualLayer);
                }
            }
        }


        // Preserve this override when updated to SDV v1.6
        public override void drawShadow(SpriteBatch b, int localX = -1, int localY = -1)
        {
            var adjustedScale = 4f;
            var adjustedOffset = new Vector2(0f, 0f);
            var buildingRectangle = this.getSourceRect();

            if (localX != -1)
            {
                if (buildingRectangle.Height + buildingRectangle.Width > 300)
                {
                    adjustedScale = 2f;
                    adjustedOffset = new Vector2(-(this.tilesWide.Value * adjustedScale * 4), -(this.tilesHigh.Value * adjustedScale * 4));
                }
                else if (buildingRectangle.Height + buildingRectangle.Width > 250)
                {
                    adjustedScale = 3f;
                    adjustedOffset = new Vector2(-(this.tilesWide.Value * adjustedScale), -(this.tilesHigh.Value * adjustedScale));
                }
            }

            Vector2 vector = ((localX == -1) ? Game1.GlobalToLocal(new Vector2((int)this.tileX * 64, ((int)this.tileY + (int)this.tilesHigh) * 64)) : new Vector2(localX, localY + this.getSourceRectForMenu().Height * adjustedScale));
            b.Draw(Game1.mouseCursors, vector, Building.leftShadow, Color.White * ((localX == -1) ? ((float)this.alpha) : 1f), 0f, adjustedOffset, adjustedScale, SpriteEffects.None, 1E-05f);
            for (int i = 1; i < (int)this.tilesWide.Value - 1; i++)
            {
                b.Draw(Game1.mouseCursors, vector + new Vector2(i * 16 * adjustedScale, 0f), Building.middleShadow, Color.White * ((localX == -1) ? ((float)this.alpha) : 1f), 0f, adjustedOffset, adjustedScale, SpriteEffects.None, 1E-05f);
            }
            b.Draw(Game1.mouseCursors, vector + new Vector2(((int)this.tilesWide - 1) * 16 * adjustedScale, 0f), Building.rightShadow, Color.White * ((localX == -1) ? ((float)this.alpha) : 1f), 0f, adjustedOffset, adjustedScale, SpriteEffects.None, 1E-05f);
        }


        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.draw
        public override void drawBackground(SpriteBatch b)
        {
            if (this.isMoving || (int)this.daysOfConstructionLeft > 0 || (int)this.newConstructionTimer > 0 || this.Model == null || this.Model.DrawLayers == null)
            {
                return;
            }

            Vector2 vector = new Vector2((int)this.tileX * 64, (int)this.tileY * 64 + (int)this.tilesHigh * 64);
            Vector2 vector2 = new Vector2(0f, this.getSourceRect().Height);
            foreach (ExtendedBuildingDrawLayer drawLayer in this.Model.DrawLayers.Where(l => ValidateConditions(l.Condition, l.ModDataFlags)))
            {
                if (!drawLayer.DrawInBackground)
                {
                    continue;
                }

                if (drawLayer.OnlyDrawIfChestHasContents != null)
                {
                    Chest buildingChest = this.GetBuildingChest(drawLayer.OnlyDrawIfChestHasContents);
                    if (buildingChest == null || buildingChest.isEmpty())
                    {
                        continue;
                    }
                }
                Rectangle sourceRect = drawLayer.GetSourceRect((int)Game1.currentGameTime.TotalGameTime.TotalMilliseconds, this);
                sourceRect = this.ApplySourceRectOffsets(sourceRect);
                Vector2 vector3 = Vector2.Zero;
                if (drawLayer.AnimalDoorOffset != Point.Zero)
                {
                    vector3 = new Vector2((float)drawLayer.AnimalDoorOffset.X * this.animalDoorOpenAmount.Value, (float)drawLayer.AnimalDoorOffset.Y * this.animalDoorOpenAmount.Value);
                }
                Texture2D texture2D = this.texture.Value;
                if (drawLayer.Texture != null)
                {
                    texture2D = Game1.content.Load<Texture2D>(drawLayer.Texture);
                }
                b.Draw(texture2D, Game1.GlobalToLocal(Game1.viewport, vector + (vector3 - vector2 + drawLayer.DrawPosition) * 4f), sourceRect, this.color.Value * this.alpha, 0f, new Vector2(0f, 0f), 4f, SpriteEffects.None, 0f);
            }
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.Building.GetMetadata
        public string GetMetadata(string key)
        {
            if (this._buildingMetadata == null)
            {
                this._buildingMetadata = new Dictionary<string, string>();
                if (this.Model != null)
                {
                    foreach (KeyValuePair<string, string> metadatum in this.Model.Metadata)
                    {
                        this._buildingMetadata[metadatum.Key] = metadatum.Value;
                    }
                    if (this.Model.Skins != null && this.Model.Skins.Count > 0 && this.skinID.Value != null)
                    {
                        foreach (BuildingSkin skin in this.Model.Skins)
                        {
                            if (!(skin.ID == this.skinID.Value))
                            {
                                continue;
                            }
                            foreach (KeyValuePair<string, string> metadatum2 in skin.Metadata)
                            {
                                this._buildingMetadata[metadatum2.Key] = metadatum2.Value;
                            }
                            break;
                        }
                    }
                }
            }
            if (this._buildingMetadata.TryGetValue(key, out key))
            {
                return key;
            }
            return null;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.GameLocation.hasActiveFireplace
        public bool HasActiveFireplaceBackport()
        {
            if (this.Model is null || this.IndoorOrInstancedIndoor is null)
            {
                return false;
            }

            for (int i = 0; i < this.IndoorOrInstancedIndoor.furniture.Count(); i++)
            {
                if ((int)this.IndoorOrInstancedIndoor.furniture[i].furniture_type.Value == 14 && (bool)this.IndoorOrInstancedIndoor.furniture[i].isOn.Value)
                {
                    return true;
                }
            }
            return false;
        }


        // TODO: When updated to SDV v1.6, this method should be deleted
        private void UpdateBackport(GameTime time)
        {
            this.alpha.Value = Math.Min(1f, this.alpha.Value + 0.05f);
            int num = this.tilesHigh.Get();
            if (this.fadeWhenPlayerIsBehind.Value && Game1.player.GetBoundingBox().Intersects(new Microsoft.Xna.Framework.Rectangle(64 * (int)this.tileX, 64 * ((int)this.tileY + (-(this.getSourceRectForMenu().Height / 16) + num)), (int)this.tilesWide * 64, (this.getSourceRectForMenu().Height / 16 - num) * 64 + 32)))
            {
                this.alpha.Value = Math.Max(0.4f, this.alpha.Value - 0.09f);
            }
            if (this.isUnderConstruction())
            {
                return;
            }
            if (!this._hasChimney.HasValue)
            {
                string metadata = this.GetMetadata("ChimneyPosition");
                if (metadata != null)
                {
                    this._hasChimney = true;
                    string[] array = metadata.Split(' ');
                    this._chimneyPosition.X = int.Parse(array[0]);
                    this._chimneyPosition.Y = int.Parse(array[1]);
                }
                else
                {
                    this._hasChimney = false;
                }
            }
            if (this.IndoorOrInstancedIndoor is FarmHouse)
            {
                int houseUpgradeLevel = (this.IndoorOrInstancedIndoor as FarmHouse).owner.HouseUpgradeLevel;
                if (this._lastHouseUpgradeLevel != houseUpgradeLevel)
                {
                    this._lastHouseUpgradeLevel = houseUpgradeLevel;
                    string text = null;
                    for (int i = 1; i <= this._lastHouseUpgradeLevel; i++)
                    {
                        string metadata2 = this.GetMetadata("ChimneyPosition" + (i + 1));
                        if (metadata2 != null)
                        {
                            text = metadata2;
                        }
                    }
                    if (text != null)
                    {
                        this._hasChimney = true;
                        string[] array2 = text.Split(' ');
                        this._chimneyPosition.X = int.Parse(array2[0]);
                        this._chimneyPosition.Y = int.Parse(array2[1]);
                    }
                }
            }
            if (!this._hasChimney.Value)
            {
                return;
            }
            this.chimneyTimer -= time.ElapsedGameTime.Milliseconds;
            if (this.chimneyTimer > 0)
            {
                return;
            }
            if (this.HasActiveFireplaceBackport())
            {
                GameLocation value = this.buildingLocation.Value;
                Vector2 vector = new Vector2((int)this.tileX * 64, (int)this.tileY * 64 + num * 64 - this.getSourceRect().Height * 4);
                Vector2 vector2 = Vector2.Zero;
                if (this.Model != null)
                {
                    vector2 = this.Model.DrawOffset * 4f;
                }
                TemporaryAnimatedSprite temporaryAnimatedSprite = new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(372, 1956, 10, 10), new Vector2(vector.X + vector2.X, vector.Y + vector2.Y) + this._chimneyPosition * 4f + new Vector2(-8f, -12f), flipped: false, 0.002f, Color.Gray);
                temporaryAnimatedSprite.alpha = 0.75f;
                temporaryAnimatedSprite.motion = new Vector2(0f, -0.5f);
                temporaryAnimatedSprite.acceleration = new Vector2(0.002f, 0f);
                temporaryAnimatedSprite.interval = 99999f;
                temporaryAnimatedSprite.layerDepth = 1f;
                temporaryAnimatedSprite.scale = 2f;
                temporaryAnimatedSprite.scaleChange = 0.02f;
                temporaryAnimatedSprite.rotationChange = (float)Game1.random.Next(-5, 6) * (float)Math.PI / 256f;
                value.temporarySprites.Add(temporaryAnimatedSprite);
            }
            this.chimneyTimer = 500;
        }

        // TODO: When updated to SDV v1.6, this method should be deleted in favor of using the native StardewValley.Buildings.updateWhenFarmNotCurrentLocation
        public override void updateWhenFarmNotCurrentLocation(GameTime time)
        {
            base.updateWhenFarmNotCurrentLocation(time);

            if (!Game1.IsMasterGame || this.Model == null)
            {
                return;
            }
            if (this.animalDoorOpen.Value)
            {
                if (this.animalDoorOpenAmount.Value < 1f)
                {
                    if (this.Model.AnimalDoorOpenDuration > 0f)
                    {
                        this.animalDoorOpenAmount.Value = Utility.MoveTowards(this.animalDoorOpenAmount.Value, 1f, (float)time.ElapsedGameTime.TotalSeconds / this.Model.AnimalDoorOpenDuration);
                    }
                    else
                    {
                        this.animalDoorOpenAmount.Value = 1f;
                    }
                }
            }
            else if (this.animalDoorOpenAmount.Value > 0f)
            {
                if (this.Model.AnimalDoorCloseDuration > 0f)
                {
                    this.animalDoorOpenAmount.Value = Utility.MoveTowards(this.animalDoorOpenAmount.Value, 0f, (float)time.ElapsedGameTime.TotalSeconds / this.Model.AnimalDoorCloseDuration);
                }
                else
                {
                    this.animalDoorOpenAmount.Value = 0f;
                }
            }

            if (this.indoors.Value is not null && this.indoors.Value is AnimalHouse animalHouse)
            {
                animalHouse.updateWhenNotCurrentLocation(this, time);
            }
        }

        public override void Update(GameTime time)
        {
            // TODO: When updated to SDV v1.6, this line should be replaced with base.Update(time)
            UpdateBackport(time);

            // Catch touch actions
            if (this.Model != null && buildingLocation.Value != null)
            {
                Vector2 playerStandingPosition = new Vector2(Game1.player.getStandingX() / 64, Game1.player.getStandingY() / 64);
                if (buildingLocation.Value.lastTouchActionLocation.Equals(Vector2.Zero))
                {
                    Point actualTile = new Point((int)playerStandingPosition.X - this.tileX.Value, (int)playerStandingPosition.Y - this.tileY.Value);
                    if (this.Model.TunnelDoors.Any(d => d.X == actualTile.X && d.Y == actualTile.Y))
                    {
                        buildingLocation.Value.lastTouchActionLocation = new Vector2(Game1.player.getStandingX() / 64, Game1.player.getStandingY() / 64);
                        bool isStructure = false;
                        if (this.indoors.Value != null)
                        {
                            isStructure = true;
                        }
                        Game1.warpFarmer(this.IndoorOrInstancedIndoor.NameOrUniqueName, this.IndoorOrInstancedIndoor.warps[0].X, this.IndoorOrInstancedIndoor.warps[0].Y - 1, Game1.player.FacingDirection, isStructure);
                    }

                    var specialActionAtTile = this.Model.GetSpecialEventAtTile(actualTile.X, actualTile.Y);
                    if (specialActionAtTile is not null)
                    {
                        buildingLocation.Value.lastTouchActionLocation = new Vector2(Game1.player.getStandingX() / 64, Game1.player.getStandingY() / 64);
                        specialActionAtTile.Trigger(Game1.player, this, actualTile);
                    }
                    else
                    {
                        string eventTile = this.Model.GetEventAtTile((int)playerStandingPosition.X - this.tileX.Value, (int)playerStandingPosition.Y - this.tileY.Value);
                        if (eventTile != null)
                        {
                            buildingLocation.Value.lastTouchActionLocation = new Vector2(Game1.player.getStandingX() / 64, Game1.player.getStandingY() / 64);

                            eventTile = TextParser.ParseText(eventTile);
                            eventTile = SolidFoundations.modHelper.Reflection.GetMethod(new Dialogue(eventTile, null), "checkForSpecialCharacters").Invoke<string>(eventTile);
                            if (buildingLocation.Value.performAction(eventTile, Game1.player, new xTile.Dimensions.Location((int)buildingLocation.Value.lastTouchActionLocation.X, (int)buildingLocation.Value.lastTouchActionLocation.Y)) is false)
                            {
                                buildingLocation.Value.performTouchAction(eventTile, playerStandingPosition);
                            }
                        }
                    }
                }
                else if (!buildingLocation.Value.lastTouchActionLocation.Equals(playerStandingPosition))
                {
                    buildingLocation.Value.lastTouchActionLocation = Vector2.Zero;
                }
            }
        }
    }
}
