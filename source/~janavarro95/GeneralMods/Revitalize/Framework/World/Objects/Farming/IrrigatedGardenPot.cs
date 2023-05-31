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
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.Ids.Items;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.Objects.Items.Farming;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.StardustCore.Animations;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace Omegasis.Revitalize.Framework.World.Objects.Farming
{
    /// <summary>
    /// A variation of the <see cref="StardewValley.Objects.IndoorPot"/> which keeps watered every day. Credits goes to the game for most of this code, modifications were made to accomidate it being always being irrigated and having a custom animation. 
    /// </summary>
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Farming.IrrigatedGardenPot")]
    public class IrrigatedGardenPot : CustomObject, ICustomModObject
    {

        public const string DEFAULT_ANIMATION_KEY = "Default";
        public const string DRIPPING_ANIMATION_KEY = "Dripping";

        public const string DRIPPING_WITH_ENRICHER_AND_PLANTER_ATTACHMENT_ANIMATION_KEY = "Dripping_With_Enricher_And_Planter_Attachments";
        public const string DEFAULT_WITH_ENRICHER_AND_PLANTER_ATTACHMENT_ANIMATION_KEY = "Default_With_Enricher_And_Planter_Attachments";

        public const string DRIPPING_WITH_PLANTER_ATTACHMENT_ANIMATION_KEY = "Dripping_With_Planter_Attachment";
        public const string DEFAULT_WITH_PLANTER_ATTACHMENT_ANIMATION_KEY = "Default_With_Planter_Attachment";

        public const string DRIPPING_WITH_ENRICHER_ATTACHMENT_ANIMATION_KEY = "Dripping_With_Enricher_Attachment";
        public const string DEFAULT_WITH_ENRICHER_ATTACHMENT_ANIMATION_KEY = "Default_With_Enricher_Attachment";


        public const string DEFAULT_WITH_AUTO_HARVESTER_ANIMATION_KEY = "Default_With_Auto_Harvester_Attachment";
        public const string DRIPPING_WITH_AUTO_HARVESTER_ANIMATION_KEY = "Dripping_With_Auto_Harvester_Attachment";
        public const string DEFAULT_WITH_AUTO_HARVESTER_ENRICHER_ANIMATION_KEY = "Default_With_Auto_Harvester_Enricher_Attachments";
        public const string DRIPPING_WITH_AUTO_HARVESTER_ENRICHER_ANIMATION_KEY = "Dripping_With_Auto_Harvester_Enricher_Attachments";
        public const string DEFAULT_WITH_AUTO_HARVESTER_PLANTER_ANIMATION_KEY = "Default_With_Auto_Harvester_Planter_Attachments";
        public const string DRIPPING_WITH_AUTO_HARVESTER_PLANTER_ANIMATION_KEY = "Dripping_With_Auto_Harvester_Planter_Attachments";

        public const string DEFAULT_WITH_ALL_ATTACHMENTS_ANIMATION_KEY = "Default_With_All_Attachments";
        public const string DRIPPING_WITH_ALL_ATTACHMENTS_ANIMATION_KEY = "Dripping_With_All_Attachments";


        [XmlElement("hoeDirt")]
        public readonly NetRef<HoeDirt> hoeDirt = new NetRef<HoeDirt>();
        [XmlIgnore]
        public Crop Crop
        {
            get
            {
                return this.hoeDirt.Value.crop;
            }
            set
            {
                this.hoeDirt.Value.crop = value;
            }
        }

        [XmlIgnore]
        public HoeDirt HoeDirt
        {
            get
            {
                return this.hoeDirt.Value;
            }
            set
            {
                this.hoeDirt.Value = value;
            }
        }

        /// <summary>
        /// The fertilizer applied to this garden pot, referenced by the parentSheetIndex in the game.
        /// </summary>
        public int Fertilizer
        {
            get
            {
                return this.HoeDirt.fertilizer.Value;
            }
            set
            {
                this.HoeDirt.fertilizer.Value = value;
            }
        }

        [XmlElement("bush")]
        public readonly NetRef<Bush> bush = new NetRef<Bush>();

        [XmlIgnore]
        private readonly NetBool bushLoadDirty = new NetBool(value: true);

        public readonly NetBool hasPlanterAttachment = new NetBool(false);
        public readonly NetBool hasEnricherAttachment = new NetBool(false);
        public readonly NetBool hasAutoHarvestAttachment = new NetBool(false);


        public IrrigatedGardenPot()
        {
        }

        public IrrigatedGardenPot(BasicItemInformation Info) : this(Info, Vector2.Zero)
        {

        }

        public IrrigatedGardenPot(BasicItemInformation Info, Vector2 TilePosition) : base(Info, TilePosition)
        {
            this.basicItemInformation = Info;

            this.hoeDirt.Value = new HoeDirt();
            this.makeSoilWet();
            base.showNextIndex.Value = (int)this.hoeDirt.Value.state.Value == 1;
        }

        protected override void initializeNetFieldsPostConstructor()
        {
            base.initializeNetFieldsPostConstructor();
            this.NetFields.AddFields(this.bush, this.hoeDirt, this.bushLoadDirty, this.hasPlanterAttachment,this.hasEnricherAttachment,this.hasAutoHarvestAttachment);
        }

        public override bool performItemDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            if (who != null && dropInItem != null && this.bush.Value == null && this.hoeDirt.Value.canPlantThisSeedHere(dropInItem.ParentSheetIndex, (int)base.TileLocation.X, (int)base.TileLocation.Y, dropInItem.Category == -19))
            {
                if ((int)dropInItem.ParentSheetIndex == 805)
                {
                    if (!probe)
                    {
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                    }
                    return false;
                }
                if ((int)dropInItem.ParentSheetIndex == 499)
                {
                    if (!probe)
                    {
                        Game1.playSound("cancel");
                        Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Objects:AncientFruitPot"));
                    }
                    return false;
                }
                if (!probe)
                {
                    if (!this.hoeDirt.Value.plant(dropInItem.ParentSheetIndex, (int)base.TileLocation.X, (int)base.TileLocation.Y, who, dropInItem.Category == -19, who.currentLocation))
                    {
                        return false;
                    }
                }
                else
                {
                    base.heldObject.Value = new StardewValley.Object();
                }
                return true;
            }
            if (who != null && dropInItem != null && this.hoeDirt.Value.crop == null && this.bush.Value == null && dropInItem is StardewValley.Object && !(dropInItem as StardewValley.Object).bigCraftable.Value && (int)dropInItem.ParentSheetIndex == 251)
            {
                if (probe)
                {
                    base.heldObject.Value = new StardewValley.Object();
                }
                else
                {
                    this.bush.Value = new Bush(base.TileLocation, 3, who.currentLocation);
                    if (!who.currentLocation.IsOutdoors)
                    {
                        this.bush.Value.greenhouseBush.Value = true;
                        this.bush.Value.loadSprite();
                        Game1.playSound("coin");
                    }
                }
                return true;
            }

            //Probe is always checking regardless of actioning.
            if (!probe && dropInItem != null)
            {

                if (dropInItem.ParentSheetIndex == (int)Enums.SDVObject.Enricher && this.hasEnricherAttachment.Value==false)
                {
                    this.hasEnricherAttachment.Value = true;
                    this.updateAnimation(true);
                    return true;
                }

                if (dropInItem is AutoPlanterGardenPotAttachment && this.hasPlanterAttachment.Value==false)
                {
                    this.hasPlanterAttachment.Value = true;
                    this.updateAnimation(true);
                    return true;
                }

                if (dropInItem is AutoHarvesterGardenPotAttachment && this.hasAutoHarvestAttachment.Value==false)
                {
                    this.hasAutoHarvestAttachment.Value = true;
                    this.updateAnimation(true);
                    return true;
                }

            }


            return false;
        }

        public virtual bool isCropReadyForHarvest()
        {
            return this.HoeDirt.readyForHarvest();
        }

        /// <summary>
        /// Plants a StardewValley vanilla crop.
        /// </summary>
        /// <param name="index">The parent sheet index for the seeds.</param>
        /// <param name="tileX">The tile x position to plant at</param>
        /// <param name="tileY">The tile y position to plant at.</param>
        /// <param name="farmerForSpeedIncreases">Who planted the seeds.</param>
        /// <param name="isFertilizer">Is this actually fertilizer?</param>
        /// <param name="location">The location to plant at.</param>
        /// <returns></returns>
        public virtual bool plant(int index, int tileX, int tileY, Farmer farmerForSpeedIncreases, Farmer farmerActuallyPlantingThisCrop ,bool isFertilizer, GameLocation location)
        {
            if (isFertilizer)
            {
                if (this.Crop != null && (int)this.Crop.currentPhase.Value != 0 && (index == (int)Enums.SDVObject.BasicFertilizer || index == (int)Enums.SDVObject.DeluxeFertilizer))
                {
                    return false;
                }
                if ((int)this.Fertilizer != 0)
                {
                    return false;
                }
                this.Fertilizer = index;
                this.applySpeedIncreases(farmerForSpeedIncreases);
                if (farmerActuallyPlantingThisCrop != null)
                {
                    location.playSound("dirtyHit");
                }
                return true;
            }
            Crop c = new Crop(index, tileX, tileY);
            if (c.seasonsToGrowIn.Count == 0)
            {
                return false;
            }
            this.Crop = c;
            if ((bool)c.raisedSeeds.Value && farmerActuallyPlantingThisCrop!=null)
            {
                location.playSound("stoneStep");
            }
            if (farmerActuallyPlantingThisCrop != null)
            {
                location.playSound("dirtyHit");
            }
            this.applySpeedIncreases(farmerForSpeedIncreases);
            return true;
        }

        public virtual bool plant(Crop crop, Farmer farmerToApplySpeedIncreasesToCrop, Farmer farmerActuallyHarvestingThisCrop, GameLocation location)
        {
            if (crop.seasonsToGrowIn.Count == 0)
            {
                return false;
            }
            this.Crop = crop;
            if (farmerActuallyHarvestingThisCrop != null)
            {
                location.playSound("dirtyHit");
            }
            this.applySpeedIncreases(farmerToApplySpeedIncreasesToCrop);
            return true;
        }

        protected void applySpeedIncreases(Farmer who)
        {
            if (this.Crop == null)
            {
                return;
            }
            if (!((int)this.Fertilizer == (int)Enums.SDVObject.SpeedGrow || (int)this.Fertilizer == (int)Enums.SDVObject.DeluxeSpeedGrow || (int)this.Fertilizer ==(int)Enums.SDVObject.HyperSpeedGro || who.professions.Contains(5)))
            {
                return;
            }
            this.Crop.ResetPhaseDays();
            int totalDaysOfCropGrowth = 0;
            for (int j = 0; j < this.Crop.phaseDays.Count - 1; j++)
            {
                totalDaysOfCropGrowth += this.Crop.phaseDays[j];
            }
            float speedIncrease = 0f;
            if ((int)this.Fertilizer == (int)Enums.SDVObject.SpeedGrow)
            {
                speedIncrease += 0.1f;
            }
            else if ((int)this.Fertilizer == (int)Enums.SDVObject.DeluxeSpeedGrow)
            {
                speedIncrease += 0.25f;
            }
            else if ((int)this.Fertilizer == (int)Enums.SDVObject.HyperSpeedGro)
            {
                speedIncrease += 0.33f;
            }
            if (who.professions.Contains(5))
            {
                speedIncrease += 0.1f;
            }
            int daysToRemove = (int)Math.Ceiling((float)totalDaysOfCropGrowth * speedIncrease);
            int tries = 0;
            while (daysToRemove > 0 && tries < 3)
            {
                for (int i = 0; i < this.Crop.phaseDays.Count; i++)
                {
                    if ((i > 0 || this.Crop.phaseDays[i] > 1) && this.Crop.phaseDays[i] != 99999)
                    {
                        this.Crop.phaseDays[i]--;
                        daysToRemove--;
                    }
                    if (daysToRemove <= 0)
                    {
                        break;
                    }
                }
                tries++;
            }
        }


        public virtual List<StardewValley.Item> harvest()
        {
            if ((bool)this.Crop.dead.Value)
            {
                return null;
            }
            if ((bool)this.Crop.forageCrop.Value)
            {
                StardewValley.Object o = null;
                Random r2 = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + (int)this.TileLocation.X * 1000 + (int)this.TileLocation.Y * 2000);
                switch ((int)this.Crop.whichForageCrop.Value)
                {
                    case 1:
                        o = new StardewValley.Object(399, 1);
                        break;
                    case 2:
                        this.HoeDirt.shake((float)Math.PI / 48f, (float)Math.PI / 40f, (float)((int)this.TileLocation.X * 64) < Game1.player.Position.X);
                        return null;
                }
                if (this.getOwner().professions.Contains(16))
                {
                    o.Quality = 4;
                }
                else if (r2.NextDouble() < (double)((float)this.getOwner().ForagingLevel / 30f))
                {
                    o.Quality = 2;
                }
                else if (r2.NextDouble() < (double)((float)this.getOwner().ForagingLevel / 15f))
                {
                    o.Quality = 1;
                }
                //Game1.stats.ItemsForaged += (uint)o.Stack;
                return new List<Item>{ o};
            }
            else if ((int)this.Crop.currentPhase.Value>= this.Crop.phaseDays.Count - 1 && (!this.Crop.fullyGrown.Value || (int)this.Crop.dayOfCurrentPhase.Value <= 0))
            {
                int numToHarvest = 1;
                int cropQuality = 0;
                int fertilizerQualityLevel = 0;
                if ((int)this.Crop.indexOfHarvest.Value == 0)
                {
                    return null;
                }
                Random r = new Random((int)this.TileLocation.X * 7 + (int)this.TileLocation.Y * 11 + (int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame);
                switch ((int)this.Fertilizer)
                {
                    case 368:
                        fertilizerQualityLevel = 1;
                        break;
                    case 369:
                        fertilizerQualityLevel = 2;
                        break;
                    case 919:
                        fertilizerQualityLevel = 3;
                        break;
                }
                double chanceForGoldQuality = 0.2 * ((double)this.getOwner().FarmingLevel / 10.0) + 0.2 * (double)fertilizerQualityLevel * (((double)Game1.player.FarmingLevel + 2.0) / 12.0) + 0.01;
                double chanceForSilverQuality = Math.Min(0.75, chanceForGoldQuality * 2.0);
                if (fertilizerQualityLevel >= 3 && r.NextDouble() < chanceForGoldQuality / 2.0)
                {
                    cropQuality = 4;
                }
                else if (r.NextDouble() < chanceForGoldQuality)
                {
                    cropQuality = 2;
                }
                else if (r.NextDouble() < chanceForSilverQuality || fertilizerQualityLevel >= 3)
                {
                    cropQuality = 1;
                }
                if ((int)this.Crop.minHarvest.Value > 1 || (int)this.Crop.maxHarvest.Value > 1)
                {
                    int max_harvest_increase = 0;
                    if (this.Crop.maxHarvestIncreasePerFarmingLevel.Value > 0)
                    {
                            max_harvest_increase = this.getOwner().FarmingLevel / (int)this.Crop.maxHarvestIncreasePerFarmingLevel.Value;
                        
                    }
                    numToHarvest = r.Next(this.Crop.minHarvest.Value, Math.Max((int)this.Crop.minHarvest.Value + 1, (int)this.Crop.maxHarvest.Value + 1 + max_harvest_increase));
                }
                if ((double)this.Crop.chanceForExtraCrops.Value > 0.0)
                {
                    while (r.NextDouble() < Math.Min(0.9, this.Crop.chanceForExtraCrops.Value))
                    {
                        numToHarvest++;
                    }
                }
                if ((int)this.Crop.indexOfHarvest.Value == 771 || (int)this.Crop.indexOfHarvest.Value == 889)
                {
                    cropQuality = 0;
                }
                StardewValley.Object harvestedItem = (this.Crop.programColored.Value ? new ColoredObject(this.Crop.indexOfHarvest.Value, 1, this.Crop.tintColor.Value)
                {
                    Quality = cropQuality
                } : new StardewValley.Object(this.Crop.indexOfHarvest.Value, 1, isRecipe: false, -1, cropQuality));

                harvestedItem.Stack = numToHarvest;

                List<Item> harvestedObjects = new List<Item>();
                harvestedObjects.Add(harvestedItem);

                if ((int)this.Crop.indexOfHarvest.Value == 421)
                {
                    this.Crop.indexOfHarvest.Value = 431;
                    numToHarvest = r.Next(1, 4);
                }
                harvestedItem = (this.Crop.programColored.Value ? new ColoredObject(this.Crop.indexOfHarvest.Value, 1, this.Crop.tintColor.Value) : new StardewValley.Object(this.Crop.indexOfHarvest.Value, 1));


                if ((int)this.Crop.indexOfHarvest.Value == 262 && r.NextDouble() < 0.4)
                {
                    StardewValley.Object hay_item = new StardewValley.Object(178, 1);
                    harvestedObjects.Add(hay_item.getOne());
                }
                else if ((int)this.Crop.indexOfHarvest.Value == 771)
                {
                    if (r.NextDouble() < 0.1)
                    {
                        StardewValley.Object mixedSeeds_item = new StardewValley.Object(770, 1);
                        harvestedObjects.Add(mixedSeeds_item.getOne());
                    }
                }
                this.Crop.fullyGrown.Value = true;
                if (this.Crop.dayOfCurrentPhase.Value == (int)this.Crop.regrowAfterHarvest.Value)
                {
                    this.Crop.updateDrawMath(this.TileLocation);
                }
                this.Crop.dayOfCurrentPhase.Value = this.Crop.regrowAfterHarvest.Value;

                if (this.Crop.regrowAfterHarvest.Value == -1)
                {
                    this.Crop = null;
                }


                return harvestedObjects;


                }
            
            return new List<Item>();
        }

        public override bool pickupFromGameWorld(Vector2 tileLocation, GameLocation environment, Farmer who)
        {
            bool canPickupGardenPot = base.pickupFromGameWorld(tileLocation, environment, who);
            if (canPickupGardenPot == false) return false;

            this.removeAttachments(environment, who);




            return canPickupGardenPot;

        }
        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            this.removeAttachments(environment, Game1.player);
            base.performRemoveAction(tileLocation, environment);
        }

        protected virtual void removeAttachments(GameLocation environment,Farmer farmer)
        {
            if (this.hasAutoHarvestAttachment.Value)
            {
                Item autoHarvester = RevitalizeModCore.ModContentManager.objectManager.getItem(FarmingItems.AutoHarvesterGardenPotAttachment);
                if (farmer!=null && farmer.isInventoryFull())
                {
                    WorldUtility.CreateItemDebrisAtTileLocation(environment, autoHarvester, this.TileLocation, this.TileLocation);
                    this.hasAutoHarvestAttachment.Value = false;
                    this.updateAnimation(true);
                }
                else
                {
                    farmer.addItemToInventoryBool(autoHarvester);
                }
                this.hasAutoHarvestAttachment.Value = false;
                this.updateAnimation(true);
            }
            if (this.hasPlanterAttachment.Value)
            {
                Item autoPlanter = RevitalizeModCore.ModContentManager.objectManager.getItem(FarmingItems.AutoPlanterGardenPotAttachment);
                if (farmer!=null && farmer.isInventoryFull())
                {
                    WorldUtility.CreateItemDebrisAtTileLocation(environment, autoPlanter, this.TileLocation, this.TileLocation);
                }
                else
                {
                    farmer.addItemToInventoryBool(autoPlanter);
                }
                this.hasPlanterAttachment.Value = false;
                this.updateAnimation(true);
            }
            if (this.hasEnricherAttachment.Value)
            {
                Item enricher = RevitalizeModCore.ModContentManager.objectManager.getItem(Enums.SDVObject.Enricher);
                if (farmer!=null && farmer.isInventoryFull())
                {
                    WorldUtility.CreateItemDebrisAtTileLocation(environment, enricher, this.TileLocation, this.TileLocation);
                }
                else
                {
                    farmer.addItemToInventoryBool(enricher);
                }
                this.hasEnricherAttachment.Value = false;
                this.updateAnimation(true);
            }
        }


        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t != null)
            {

                this.hoeDirt.Value.performToolAction(t, -1, base.TileLocation, location);
                if (this.bush.Value != null)
                {
                    if (this.bush.Value.performToolAction(t, -1, base.TileLocation, location))
                    {
                        this.bush.Value = null;
                    }
                    return false;
                }

            }
            if ((int)this.hoeDirt.Value.state.Value == 1)
            {
                base.showNextIndex.Value = true;
            }
            this.removeAttachments(location, Game1.player);

            return base.performToolAction(t, location);
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            return base.canBePlacedHere(l, tile);
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            bool placed = base.placementAction(location, x, y, who);

            if (placed)
            {
                StardewValley.Object obj = location.getObjectAtTile(x, y);
                if (obj is IrrigatedGardenPot)
                {
                    (obj as IrrigatedGardenPot).Crop = this.Crop;
                }
            }
            return placed;
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (who != null)
            {
                if (justCheckingForActivity)
                {
                    string season = Game1.GetSeasonForLocation(who.currentLocation);
                    if (this.bush.Value != null && (int)this.bush.Value.overrideSeason.Value != -1)
                    {
                        season = Utility.getSeasonNameFromNumber(this.bush.Value.overrideSeason.Value);
                    }
                    if (!this.hoeDirt.Value.readyForHarvest() && base.heldObject.Value == null)
                    {
                        if (this.bush.Value != null)
                        {
                            return this.bush.Value.inBloom(season, Game1.dayOfMonth);
                        }
                        return false;
                    }
                    return true;
                }
                if (who.isMoving())
                {
                    Game1.haltAfterCheck = false;
                }
                if (base.heldObject.Value != null)
                {
                    bool num = who.addItemToInventoryBool(base.heldObject.Value);
                    if (num)
                    {
                        base.heldObject.Value = null;
                        base.readyForHarvest.Value = false;
                        Game1.playSound("coin");
                    }
                    return num;
                }
                bool b = this.hoeDirt.Value.performUseAction(base.TileLocation, who.currentLocation);
                if (b)
                {
                    return b;
                }
                if (this.hoeDirt.Value.crop != null && (int)this.hoeDirt.Value.crop.currentPhase.Value > 0 && this.hoeDirt.Value.getMaxShake() == 0f)
                {
                    this.hoeDirt.Value.shake((float)Math.PI / 32f, (float)Math.PI / 50f, Game1.random.NextDouble() < 0.5);
                    DelayedAction.playSoundAfterDelay("leafrustle", Game1.random.Next(100));
                }
                if (this.bush.Value != null)
                {
                    this.bush.Value.performUseAction(base.TileLocation, who.currentLocation);
                }
            }
            return false;
        }

        protected virtual void updateAnimation(bool ShowDrippingAnimation)
        {

            if (this.hasEnricherAttachment.Value == true && this.hasPlanterAttachment.Value == true && this.hasAutoHarvestAttachment.Value)
            {
                if (ShowDrippingAnimation)
                {
                    this.AnimationManager.playAnimation(DRIPPING_WITH_ALL_ATTACHMENTS_ANIMATION_KEY, true);
                }
                else
                {
                    this.AnimationManager.playAnimation(DEFAULT_WITH_ALL_ATTACHMENTS_ANIMATION_KEY, true);
                }
                return;
            }

            if (this.hasEnricherAttachment.Value==false && this.hasPlanterAttachment.Value==false && this.hasAutoHarvestAttachment.Value)
            {
                if (ShowDrippingAnimation)
                {
                    this.AnimationManager.playAnimation(DRIPPING_WITH_AUTO_HARVESTER_ANIMATION_KEY, true);
                }
                else
                {
                    this.AnimationManager.playAnimation(DEFAULT_WITH_AUTO_HARVESTER_ANIMATION_KEY, true);
                }
                return;
            }

            if (this.hasEnricherAttachment.Value == false && this.hasPlanterAttachment.Value == true && this.hasAutoHarvestAttachment.Value)
            {
                if (ShowDrippingAnimation)
                {
                    this.AnimationManager.playAnimation(DRIPPING_WITH_AUTO_HARVESTER_PLANTER_ANIMATION_KEY, true);
                }
                else
                {
                    this.AnimationManager.playAnimation(DEFAULT_WITH_AUTO_HARVESTER_PLANTER_ANIMATION_KEY, true);
                }
                return;
            }

            if (this.hasEnricherAttachment.Value == true && this.hasPlanterAttachment.Value == false && this.hasAutoHarvestAttachment.Value)
            {
                if (ShowDrippingAnimation)
                {
                    this.AnimationManager.playAnimation(DRIPPING_WITH_AUTO_HARVESTER_ENRICHER_ANIMATION_KEY, true);
                }
                else
                {
                    this.AnimationManager.playAnimation(DEFAULT_WITH_AUTO_HARVESTER_ENRICHER_ANIMATION_KEY, true);
                }
                return;
            }

            if (this.hasEnricherAttachment.Value && this.hasPlanterAttachment.Value && this.hasAutoHarvestAttachment.Value==false)
            {
                if (ShowDrippingAnimation)
                {
                    this.AnimationManager.playAnimation(DRIPPING_WITH_ENRICHER_AND_PLANTER_ATTACHMENT_ANIMATION_KEY, true);
                }
                else
                {
                    this.AnimationManager.playAnimation(DEFAULT_WITH_ENRICHER_AND_PLANTER_ATTACHMENT_ANIMATION_KEY, true);
                }
                return;
            }
            if (this.hasEnricherAttachment.Value && !this.hasPlanterAttachment.Value && this.hasAutoHarvestAttachment.Value == false)
            {
                if (ShowDrippingAnimation)
                {
                    this.AnimationManager.playAnimation(DRIPPING_WITH_ENRICHER_ATTACHMENT_ANIMATION_KEY, true);
                }
                else
                {
                    this.AnimationManager.playAnimation(DEFAULT_WITH_ENRICHER_ATTACHMENT_ANIMATION_KEY, true);
                }
                return;
            }
            if (!this.hasEnricherAttachment.Value && this.hasPlanterAttachment.Value && this.hasAutoHarvestAttachment.Value == false)
            {
                if (ShowDrippingAnimation)
                {
                    this.AnimationManager.playAnimation(DRIPPING_WITH_PLANTER_ATTACHMENT_ANIMATION_KEY, true);
                }
                else
                {
                    this.AnimationManager.playAnimation(DEFAULT_WITH_PLANTER_ATTACHMENT_ANIMATION_KEY, true);
                }
                return;
            }

            //Default everything is false.
            if (ShowDrippingAnimation)
            {
                this.AnimationManager.playAnimation(DRIPPING_ANIMATION_KEY, true);
            }
            else
            {
                this.AnimationManager.playAnimation(DEFAULT_ANIMATION_KEY, true);
            }
            return;

        }

        public override void actionOnPlayerEntry()
        {
            //base.actionOnPlayerEntry();
            this.updateAnimation(true);
            if (this.hoeDirt.Value != null)
            {
                this.hoeDirt.Value.performPlayerEntryAction(base.TileLocation);
            }
        }

        public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
        {
            //base.updateWhenCurrentLocation(time, environment);
            this.hoeDirt.Value.tickUpdate(time, base.TileLocation, environment);
            this.bush.Value?.tickUpdate(time, environment);
            if ((bool)this.bushLoadDirty.Value)
            {
                this.bush.Value?.loadSprite();
                this.bushLoadDirty.Value = false;
            }
        }


        public virtual void makeSoilWet()
        {
            this.hoeDirt.Value.state.Value = 1;
        }

        public override void doActualDayUpdateLogic(GameLocation location)
        {
            // base.DayUpdate(location)

            this.hoeDirt.Value.dayUpdate(location, base.TileLocation);
            this.makeSoilWet();
            base.showNextIndex.Value = (int)this.hoeDirt.Value.state.Value == 1;
            if (base.heldObject.Value != null)
            {
                base.readyForHarvest.Value = true;
            }
            if (this.bush.Value != null)
            {
                this.bush.Value.dayUpdate(location);
            }
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

        /// <summary>What happens when the object is drawn at a tile location.</summary>
        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
        {
            this.DrawICustomModObject(spriteBatch, alpha);

            if ((int)this.hoeDirt.Value.fertilizer.Value != 0)
            {
                Rectangle fertilizer_rect = this.hoeDirt.Value.GetFertilizerSourceRect(this.hoeDirt.Value.fertilizer.Value);
                fertilizer_rect.Width = 13;
                fertilizer_rect.Height = 13;
                float depth = (base.TileLocation.Y - this.basicItemInformation.drawOffset.Y+ 0.65f) * 64f / 10000f + (float)x * 1E-05f;
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(base.TileLocation.X * 64f + 4f, base.TileLocation.Y * 64f - 12f)), fertilizer_rect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, depth+ .00001f);
            }
            if (this.hoeDirt.Value.crop != null)
            {
                //  this.hoeDirt.Value.crop.drawWithOffset(spriteBatch, base.tileLocation, ((int)this.hoeDirt.Value.state == 1 && (int)this.hoeDirt.Value.crop.currentPhase == 0 && !this.hoeDirt.Value.crop.raisedSeeds) ? (new Color(180, 100, 200) * 1f) : Color.White, this.hoeDirt.Value.getShakeRotation(), new Vector2(32f, 8f));

                this.drawCropWithOffset(spriteBatch, this.TileLocation, ((int)this.hoeDirt.Value.state.Value == 1 && (int)this.hoeDirt.Value.crop.currentPhase.Value == 0 && !this.hoeDirt.Value.crop.raisedSeeds.Value) ? (new Color(180, 100, 200) * 1f) : Color.White, this.hoeDirt.Value.getShakeRotation(), new Vector2(32f, 8f), (this.TileLocation.Y - this.basicItemInformation.drawOffset.Y));
            }
            if (base.heldObject.Value != null)
            {
                base.heldObject.Value.draw(spriteBatch, x * 64, y * 64 - 48 + 64, (base.TileLocation.Y + 0.66f) * 64f / 10000f + (float)x * 1E-05f, 1f);
            }
            if (this.bush.Value != null)
            {
                this.bush.Value.draw(spriteBatch, new Vector2(x, y + 64), -24f);
            }
        }

        public virtual void drawCropWithOffset(SpriteBatch b, Vector2 tileLocation, Color toTint, float rotation, Vector2 offset, float YTileDepthOffset)
        {

            if ((bool)this.Crop.forageCrop.Value)
            {
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), this.sourceRect.Value, Color.White, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (tileLocation.Y + 0.66f) * 64f / 10000f + tileLocation.X * 1E-05f);
                return;
            }
            Rectangle coloredSourceRect = new Rectangle(((!this.Crop.fullyGrown.Value) ? ((int)this.Crop.currentPhase.Value + 1 + 1) : (((int)this.Crop.dayOfCurrentPhase.Value <= 0) ? 6 : 7)) * 16 + (((int)this.Crop.rowInSpriteSheet.Value % 2 != 0) ? 128 : 0), (int)this.Crop.rowInSpriteSheet.Value / 2 * 16 * 2, 16, 32); ;

            float originalDepth = (YTileDepthOffset - .5f + 0.66f) * 64f / 10000f + tileLocation.X * 1E-05f;

            float depth = originalDepth;//Math.Max(originalDepth, modDepth);

            b.Draw(Game1.cropSpriteSheet, Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), this.Crop.getSourceRect((int)tileLocation.X * 7 + (int)tileLocation.Y * 11), toTint, rotation, new Vector2(8f, 24f), 4f, this.Crop.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, depth + .00001f);
            if (!this.Crop.tintColor.Equals(Color.White) && (int)this.Crop.currentPhase.Value == this.Crop.phaseDays.Count - 1 && !this.Crop.dead.Value)
            {
                b.Draw(Game1.cropSpriteSheet, Game1.GlobalToLocal(Game1.viewport, offset + new Vector2(tileLocation.X * 64f, tileLocation.Y * 64f)), coloredSourceRect, this.Crop.tintColor.Value, rotation, new Vector2(8f, 24f), 4f, this.Crop.flip.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, depth + .00002f);
            }
        }

        public override Item getOne()
        {
            IrrigatedGardenPot pot = new IrrigatedGardenPot(this.basicItemInformation.Copy(), Vector2.Zero);
            pot.Crop = this.Crop;
            return pot;
        }


        public override bool canStackWith(ISalable other)
        {
            if (other is IrrigatedGardenPot)
            {

                IrrigatedGardenPot otherPot = (IrrigatedGardenPot)other;
                if (this.Crop != null && otherPot.Crop != null)
                {

                    if (this.Crop.GetType() == typeof(Crop) && otherPot.Crop.GetType() == typeof(Crop))
                    {

                        if (this.Crop.netSeedIndex.Value == otherPot.Crop.netSeedIndex.Value)
                        {
                            if (this.Crop.dayOfCurrentPhase.Value == otherPot.Crop.dayOfCurrentPhase.Value)
                            {
                                if (this.Crop.currentPhase.Value == otherPot.Crop.currentPhase.Value)
                                {
                                    if (this.hoeDirt.Value.fertilizer.Value == otherPot.hoeDirt.Value.fertilizer.Value)
                                    {
                                        if (this.Crop.dead == otherPot.Crop.dead)
                                        {

                                            return true;
                                        }
                                        else
                                        {
                                            return false;
                                        }

                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }


            return base.canStackWith(other);
        }
    }
}
