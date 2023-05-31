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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.World.Objects.Crafting;
using Omegasis.Revitalize.Framework.Constants.CraftingIds;
using Omegasis.Revitalize.Framework.World.Objects.Interfaces;
using Omegasis.Revitalize.Framework.World.Objects.Machines.EnergyGeneration;
using Omegasis.Revitalize.Framework.World.Objects.Machines;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.StardustCore.UIUtilities;
using Omegasis.StardustCore.Animations;
using Omegasis.Revitalize.Framework.Managers;
using Omegasis.Revitalize.Framework.World.Objects.Farming;
using Omegasis.Revitalize.Framework.World.Objects.Items.Farming;
using Omegasis.Revitalize.Framework.World.Objects.Machines.Furnaces;
using Omegasis.Revitalize.Framework.World.Objects.Resources;
using Omegasis.Revitalize.Framework.Constants.ItemCategoryInformation;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Data;
using System.IO;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles.Json.Crafting;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.Content.JsonContent.Objects;
using Omegasis.Revitalize.Framework.World.Objects.Machines.ResourceGeneration;
using Omegasis.Revitalize.Framework.World.Objects.Machines.Misc;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.Constants.Ids.Items;
using Omegasis.Revitalize.Framework.World.Objects.Misc;
using static Omegasis.Revitalize.Framework.Constants.Enums;
using Omegasis.Revitalize.Framework.Illuminate;
using Omegasis.Revitalize.Framework.World.Objects.Storage;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.Content.JsonContent.Crafting;

namespace Omegasis.Revitalize.Framework.World.Objects
{
    /// <summary>
    /// Deals with handling all objects for the mod.
    /// </summary>
    public class ObjectManager
    {

        public const string StardewValleyObjectIdPrefix = "(O)";
        public const string StardewValleyBigCraftablePrefix = "(BC)";

        /// <summary>
        /// All of the object managers id'd by a mod's or content pack's unique id.
        /// </summary>
        public static Dictionary<string, ObjectManager> ObjectPools;


        /// <summary>
        /// The name of this object manager.
        /// </summary>
        public string name;

        public ResourceManager resources;

        /// <summary>
        /// The list of registered items for this object manager.
        /// </summary>
        public Dictionary<string, Item> itemsById;

        /// <summary>
        /// Display strings for all loaded items.
        /// </summary>
        public Dictionary<string, IdToDisplayStrings> displayStrings = new Dictionary<string, IdToDisplayStrings>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectManager()
        {
            this.initialize();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="manifest"></param>
        public ObjectManager(IManifest manifest)
        {
            this.name = manifest.UniqueID;
            this.initialize();
        }

        /// <summary>
        /// Initialize all objects used to manage this class.
        /// </summary>
        private void initialize()
        {

            this.resources = new ResourceManager();
            this.itemsById = new Dictionary<string, Item>();

            //Load in furniture again!
        }

        /// <summary>
        /// Loads in the items for the object and resource managers.
        /// </summary>
        public void loadItemsFromDisk()
        {
            this.registerStardewValleyItems();

            this.resources.loadInItems(); //Should take priority over other modded content.

            this.loadInItems();
            this.loadInCraftingTables();
            this.loadInMachines();
            this.loadInMiscObjects();
            this.loadInAestheticsObjects();
            this.addInStorageObjects();

            this.loadInResourcePlants();

            //Should load blueprints last due to the fact that they can draw references to objects.
            this.loadInBlueprints();
        }

        protected virtual void loadInMiscObjects()
        {
            this.addItem(new StatueOfStatistics(new BasicItemInformation("", MiscObjectIds.StatueOfStatistics, "", CategoryNames.Misc, CategoryColors.Misc, -300, -300, 0, false, 100, false, false, TextureManagers.Objects_Misc.createAnimationManager("StatueOfStatistics", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null)));
            this.addItem(new StatueOfPerfecionTracking(new BasicItemInformation("", MiscObjectIds.StatueOfPerfectionTracking, "", CategoryNames.Misc, CategoryColors.Misc, -300, -300, 0, false, 100, false, false, TextureManagers.Objects_Misc.createAnimationManager("StatueOfPerfectionTracking", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null)));

        }

        /// <summary>
        /// Used to register items in the object managaer so that they can be used for the ObjectManager and for referencing things such as crafting recipes.
        /// </summary>
        protected virtual void registerStardewValleyItems()
        {
            this.addItem("StardewValley.Tools.Pickaxe", new StardewValley.Tools.Pickaxe());
            this.addItem("StardewValley.Tools.Axe", new StardewValley.Tools.Axe());
            this.addItem("StardewValley.Tools.WateringCan", new StardewValley.Tools.WateringCan());
            this.addItem("StardewValley.Tools.Hoe", new StardewValley.Tools.Hoe());
            this.addItem("StardewValley.Tools.CopperPickaxe", new StardewValley.Tools.Pickaxe() { UpgradeLevel = Tool.copper });
            this.addItem("StardewValley.Tools.CopperAxe", new StardewValley.Tools.Axe() { UpgradeLevel = Tool.copper });
            this.addItem("StardewValley.Tools.CopperWateringCan", new StardewValley.Tools.WateringCan() { UpgradeLevel = Tool.copper });
            this.addItem("StardewValley.Tools.CopperHoe", new StardewValley.Tools.Hoe() { UpgradeLevel = Tool.copper });
            this.addItem("StardewValley.Tools.SteelPickaxe", new StardewValley.Tools.Pickaxe() { UpgradeLevel = Tool.steel });
            this.addItem("StardewValley.Tools.SteelAxe", new StardewValley.Tools.Axe() { UpgradeLevel = Tool.steel });
            this.addItem("StardewValley.Tools.SteelWateringCan", new StardewValley.Tools.WateringCan() { UpgradeLevel = Tool.steel });
            this.addItem("StardewValley.Tools.SteelHoe", new StardewValley.Tools.Hoe() { UpgradeLevel = Tool.steel });
            this.addItem("StardewValley.Tools.GoldPickaxe", new StardewValley.Tools.Pickaxe() { UpgradeLevel = Tool.gold });
            this.addItem("StardewValley.Tools.GoldAxe", new StardewValley.Tools.Axe() { UpgradeLevel = Tool.gold });
            this.addItem("StardewValley.Tools.GoldWateringCan", new StardewValley.Tools.WateringCan() { UpgradeLevel = Tool.gold });
            this.addItem("StardewValley.Tools.GoldHoe", new StardewValley.Tools.Hoe() { UpgradeLevel = Tool.gold });
            this.addItem("StardewValley.Tools.IridiumPickaxe", new StardewValley.Tools.Pickaxe() { UpgradeLevel = Tool.iridium });
            this.addItem("StardewValley.Tools.IridiumAxe", new StardewValley.Tools.Axe() { UpgradeLevel = Tool.iridium });
            this.addItem("StardewValley.Tools.IridiumWateringCan", new StardewValley.Tools.WateringCan() { UpgradeLevel = Tool.iridium });
            this.addItem("StardewValley.Tools.IridiumHoe", new StardewValley.Tools.Hoe() { UpgradeLevel = Tool.iridium });

            //Start migrating conent to new 1.6 format.
            foreach (SDVObject obj in Enum.GetValues<Enums.SDVObject>())
            {
                this.addItem(this.createVanillaObjectId(obj), new StardewValley.Object((int)obj, 1));
            }

            //Start migrating conent to new 1.6 format.
            foreach (SDVBigCraftable obj in Enum.GetValues<Enums.SDVBigCraftable>())
            {
                this.addItem(this.createVanillaBigCraftableId(obj), new StardewValley.Object(Vector2.Zero, (int)obj));
            }
        }

        private void loadInAestheticsObjects()
        {
        }

        private void loadInResourcePlants()
        {
            ResourceBush coalBush = new ResourceBush(new BasicItemInformation("", ResourceObjectIds.CoalBush, "", CategoryNames.Resource, CategoryColors.Misc, -300, -300, 0, false, 5000, false, false, TextureManagers.Objects_Resources_ResourcePlants.createAnimationManager("CoalBush", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), this.getObject(Enums.SDVObject.Coal), 1);
            this.addItem(ResourceObjectIds.CoalBush, coalBush);

            ResourceBush copperOreBush = new ResourceBush(new BasicItemInformation("", ResourceObjectIds.CopperOreBush, "", CategoryNames.Resource, CategoryColors.Misc, -300, -300, 0, false, 2000, false, false, TextureManagers.Objects_Resources_ResourcePlants.createAnimationManager("CopperOreBush", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), this.getObject(Enums.SDVObject.CopperOre), 1);
            this.addItem(ResourceObjectIds.CopperOreBush, copperOreBush);

            ResourceBush ironOreBush = new ResourceBush(new BasicItemInformation("", ResourceObjectIds.IronOreBush, "", CategoryNames.Resource, CategoryColors.Misc, -300, -300, 0, false, 5000, false, false, TextureManagers.Objects_Resources_ResourcePlants.createAnimationManager("IronOreBush", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), this.getObject(Enums.SDVObject.IronOre), 2);
            this.addItem(ResourceObjectIds.IronOreBush, ironOreBush);

            ResourceBush goldOreBush = new ResourceBush(new BasicItemInformation("", ResourceObjectIds.GoldOreBush, "", CategoryNames.Resource, CategoryColors.Misc, -300, -300, 0, false, 10000, false, false, TextureManagers.Objects_Resources_ResourcePlants.createAnimationManager("GoldOreBush", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), this.getObject(Enums.SDVObject.GoldOre), 2);
            this.addItem(ResourceObjectIds.GoldOreBush, goldOreBush);

            ResourceBush iridiumResourceBush = new ResourceBush(new BasicItemInformation("", ResourceObjectIds.IridiumOreBush, "", CategoryNames.Resource, CategoryColors.Misc, -300, -300, 0, false, 25000, false, false, TextureManagers.Objects_Resources_ResourcePlants.createAnimationManager("IridiumOreBush", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), this.getObject(Enums.SDVObject.IridiumOre), 3);
            this.addItem(ResourceObjectIds.IridiumOreBush, iridiumResourceBush);

            ResourceBush radioactiveOreBush = new ResourceBush(new BasicItemInformation("", ResourceObjectIds.RadioactiveOreBush, "", CategoryNames.Resource, CategoryColors.Misc, -300, -300, 0, false, 50000, false, false, TextureManagers.Objects_Resources_ResourcePlants.createAnimationManager("RadioactiveOreBush", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), this.getObject(Enums.SDVObject.RadioactiveOre), 3);
            this.addItem(ResourceObjectIds.RadioactiveOreBush, radioactiveOreBush);
        }

        /// <summary>
        /// Loads in the item for the object manager.
        /// </summary>
        private void loadInItems()
        {

            AutoPlanterGardenPotAttachment autoPlanterGardenPotAttachment = new AutoPlanterGardenPotAttachment(new BasicItemInformation("", FarmingItems.AutoPlanterGardenPotAttachment, "", CategoryNames.Farming, CategoryColors.Farming, -300, -300, 0, false, 5000, false, false, TextureManagers.Items_Farming.createAnimationManager("AutoPlanterGardenPotAttachment", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null));
            this.addItem(FarmingItems.AutoPlanterGardenPotAttachment, autoPlanterGardenPotAttachment);

            AutoHarvesterGardenPotAttachment autoHarvesterGardenPotAttachment = new AutoHarvesterGardenPotAttachment(new BasicItemInformation("", FarmingItems.AutoHarvesterGardenPotAttachment, "", CategoryNames.Farming, CategoryColors.Farming, -300, -300, 0, false, 5000, false, false, TextureManagers.Items_Farming.createAnimationManager("AutoHarvesterGardenPotAttachment", new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null));
            this.addItem(FarmingItems.AutoHarvesterGardenPotAttachment, autoHarvesterGardenPotAttachment);

            this.addItem(MiscItemIds.RadioactiveFuel, new CustomItem(new BasicItemInformation("", MiscItemIds.RadioactiveFuel, "", CategoryNames.Misc, CategoryColors.Misc, -300, -300, 0, false, 5000, false, false, TextureManagers.Items_Misc.createAnimationManager("RadioactiveFuel", new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null)));

            //Placeholder item for refilling the silos on the player's farm.
            this.addItem(FarmingItems.RefillSilosFakeItem, new CustomItem(new BasicItemInformation("", FarmingItems.RefillSilosFakeItem, "", CategoryNames.Farming, CategoryColors.Farming, -300, -300, StardewValley.Object.fragility_Removable, false, 100, false, false, TextureManagers.Items_Farming.createAnimationManager("HayRefill", new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null)));

            this.addItem(MiscItemIds.MovieTheaterTicketSubscription, new CustomItem(new BasicItemInformation("", MiscItemIds.MovieTheaterTicketSubscription, "", CategoryNames.Misc, CategoryColors.Misc, -300, -300, StardewValley.Object.fragility_Removable, false, 100, false, false, TextureManagers.Items_Misc.createAnimationManager("MovieTicketSubscription", new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null)));
        }

        private void loadInBlueprints()
        {

            //Make sure that all blueprints registered here have a id reference in Blueprints.cs for easier access via code.
            foreach (KeyValuePair<string, JsonCraftingBlueprint> jsonBlueprintPair in JsonUtilities.LoadJsonFilesFromDirectoriesWithPaths<JsonCraftingBlueprint>(ObjectsDataPaths.CraftingBlueprintsPath))
            {
                /*
                foreach (CraftingBookIdToRecipeId recipeId in jsonBlueprintPair.Value.recipesToUnlockV2)
                {
                    jsonBlueprintPair.Value.recipesToUnlock.Add(recipeId);
                }
                */
                try
                {
                    this.addItem(jsonBlueprintPair.Value.id, jsonBlueprintPair.Value.toBlueprint());
                }
                catch (Exception e)
                {
                    throw new InvalidJsonBlueprintException(string.Format("There was an error loading the json crafting blueprint from disk with path {0}. Please see the following error message for more details. \n {1}", jsonBlueprintPair.Key, e.ToString()));
                }

                JsonUtilities.WriteJsonFile(jsonBlueprintPair.Value, jsonBlueprintPair.Key);

            }

        }

        private void loadInCraftingTables()
        {
            CraftingTable WorkStationObject = new CraftingTable(new BasicItemInformation("", CraftingStations.WorkBench_Id, "", CategoryNames.Crafting, Color.Brown, -300, -300, 0, false, 500, true, true, TextureManagers.Objects_Crafting.createAnimationManager("Workbench", new Animation(0, 0, 32, 32)), Color.White, false, new Vector2(2, 2), Vector2.Zero, null, null), CraftingRecipeBooks.WorkbenchCraftingRecipies);
            this.addItem(CraftingStations.WorkBench_Id, WorkStationObject);
        }

        private void loadInMachines()
        {

            this.addItem(new AdvancedSolarPanel(new BasicItemInformation("", MachineIds.AdvancedSolarPanel, "", CategoryNames.Machine, Color.SteelBlue, -300, -300, 0, false, 1000, true, true, TextureManagers.Objects_Machines.createAnimationManager("AdvancedSolarPanel", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 2), Vector2.Zero, null, null), AdvancedSolarPanel.SolarPanelTier.Advanced));
            this.addItem(new AdvancedSolarPanel(new BasicItemInformation("", MachineIds.SuperiorSolarPanel, "", CategoryNames.Machine, Color.SteelBlue, -300, -300, 0, false, 1000, true, true, TextureManagers.Objects_Machines.createAnimationManager("SuperiorSolarPanel", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 2), Vector2.Zero, null, null), AdvancedSolarPanel.SolarPanelTier.Superior));


            Windmill windMillV1_0_0 = new Windmill(new BasicItemInformation("", MachineIds.Windmill, "", CategoryNames.Machine, Color.SteelBlue, -300, -300, 0, false, 500, true, true, TextureManagers.Objects_Machines.createAnimationManager("Windmill", new SerializableDictionary<string, Animation>() {

                {Machine.DEFAULT_ANINMATION_KEY,new Animation( new AnimationFrame(0,0,16,32)) },
                {Machine.WORKING_ANIMATION_KEY,new Animation(new List<AnimationFrame>(){
                    new AnimationFrame(0,0,16,32,20),
                    new AnimationFrame(16,0,16,32,20) },true)
                }
            }, Machine.DEFAULT_ANINMATION_KEY, Machine.WORKING_ANIMATION_KEY), Color.White, false, new Vector2(1, 2), Vector2.Zero, null, null, false, null), Vector2.Zero);

            this.addItem(MachineIds.Windmill, windMillV1_0_0);

            this.addItem(FarmingObjectIds.HayMaker, new HayMaker(new BasicItemInformation("", FarmingObjectIds.HayMaker, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 2000, true, true, TextureManagers.Objects_Farming.createAnimationManager("HayMaker", new SerializableDictionary<string, Animation>()
            {
                {Machine.DEFAULT_ANINMATION_KEY,new Animation( new AnimationFrame(0,0,16,32)) },
                    {HayMaker.HayAnimation,new Animation(new List<AnimationFrame>(){
                       new AnimationFrame(16,0,16,32,20)}
                    ,true)},
                    {HayMaker.WheatAnimation,new Animation(new List<AnimationFrame>(){
                       new AnimationFrame(80,0,16,32,20)}
                    ,true)},
                    {HayMaker.CornAnimation,new Animation(new List<AnimationFrame>(){
                       new AnimationFrame(32,0,16,32,20)}
                    ,true)
                    },
                    {HayMaker.AmaranthAnimation,new Animation(new List<AnimationFrame>(){
                       new AnimationFrame(48,0,16,32,20)}
                    ,true)
                    },
                    {HayMaker.FiberAnimation,new Animation(new List<AnimationFrame>(){
                       new AnimationFrame(64,0,16,32,20)}
                    ,true)
                    }
            }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, /* Bounding box is the number of pixels taken up */ new Vector2(1, 1),/*Shift by whitespace*/ new Vector2(0, -1), new InventoryManager(), new Illuminate.LightManager())));

            this.addItem(FarmingObjectIds.HayMaker_FeedShop, new HayMaker(new BasicItemInformation("", FarmingObjectIds.HayMaker_FeedShop, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 2000, true, true, TextureManagers.Objects_Farming.createAnimationManager("HayMaker", new Animation(16, 0, 16, 32)), Color.White, false, /* Bounding box is the number of pixels taken up */ new Vector2(1, 1),/*Shift by whitespace*/ new Vector2(0, -1), new InventoryManager(), new Illuminate.LightManager()), true));



            this.addItem(FarmingObjectIds.IrrigatedGardenPot, new IrrigatedGardenPot(new BasicItemInformation("", FarmingObjectIds.IrrigatedGardenPot, "", CategoryNames.Farming, CategoryColors.Farming, -300, -300, 0, false, 5000, true, true, TextureManagers.Objects_Farming.createAnimationManager("IrrigatedGardenPot", new SerializableDictionary<string, Animation>()
            {
                {IrrigatedGardenPot.DEFAULT_ANIMATION_KEY,new Animation( new AnimationFrame(0,0,16,32)) },
                {IrrigatedGardenPot.DRIPPING_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,0,16,32,13, 6)},

                {IrrigatedGardenPot.DEFAULT_WITH_ENRICHER_AND_PLANTER_ATTACHMENT_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,32,16,32,1, 6)},
                {IrrigatedGardenPot.DRIPPING_WITH_ENRICHER_AND_PLANTER_ATTACHMENT_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,32,16,32,13, 6)},

                {IrrigatedGardenPot.DEFAULT_WITH_PLANTER_ATTACHMENT_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,64,16,32,1, 6)},
                {IrrigatedGardenPot.DRIPPING_WITH_PLANTER_ATTACHMENT_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,64,16,32,13, 6)},

                {IrrigatedGardenPot.DEFAULT_WITH_ENRICHER_ATTACHMENT_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,96,16,32,1, 6)},
                {IrrigatedGardenPot.DRIPPING_WITH_ENRICHER_ATTACHMENT_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,96,16,32,13, 6)},


                {IrrigatedGardenPot.DEFAULT_WITH_AUTO_HARVESTER_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,128,16,32,1, 6)},
                {IrrigatedGardenPot.DRIPPING_WITH_AUTO_HARVESTER_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,128,16,32,13, 6)},

                {IrrigatedGardenPot.DEFAULT_WITH_ALL_ATTACHMENTS_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,160,16,32,1, 6)},
                {IrrigatedGardenPot.DRIPPING_WITH_ALL_ATTACHMENTS_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,160,16,32,13, 6)},

                {IrrigatedGardenPot.DEFAULT_WITH_AUTO_HARVESTER_PLANTER_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,192,16,32,1, 6)},
                {IrrigatedGardenPot.DRIPPING_WITH_AUTO_HARVESTER_PLANTER_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,192,16,32,13, 6)},

                {IrrigatedGardenPot.DEFAULT_WITH_AUTO_HARVESTER_ENRICHER_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,224,16,32,1, 6)},
                {IrrigatedGardenPot.DRIPPING_WITH_AUTO_HARVESTER_ENRICHER_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,224,16,32,13, 6)},



            }, IrrigatedGardenPot.DEFAULT_ANIMATION_KEY, IrrigatedGardenPot.DRIPPING_ANIMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), new InventoryManager(), new Illuminate.LightManager())));
            this.addItem(FarmingObjectIds.AdvancedFarmingSystem, new AdvancedFarmingSystem(new BasicItemInformation("", FarmingObjectIds.AdvancedFarmingSystem, "", CategoryNames.Farming, CategoryColors.Farming, -300, -300, 0, false, 10000, true, true, TextureManagers.Objects_Farming.createAnimationManager("AdvancedFarmingSystem", new Animation(0, 0, 16, 32)), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null)));


            this.addInFurnaces();
            this.addInGeodeCrushers();
            this.addInMiningDrills();
            this.addInCharcoalKilns();

            this.addItem(MachineIds.BurnerGenerator, new BatteryGenerator(new BasicItemInformation("", MachineIds.BurnerGenerator, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("BurnerGenerator", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), Color.DarkCyan.Invert()));

            this.addItem(MachineIds.AdvancedGenerator, new BatteryGenerator(new BasicItemInformation("", MachineIds.AdvancedGenerator, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("AdvancedGenerator", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), Color.DarkCyan.Invert()));

            this.addItem(MachineIds.NuclearGenerator, new BatteryGenerator(new BasicItemInformation("", MachineIds.NuclearGenerator, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("NuclearGenerator", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), Color.GreenYellow));


            this.addItem(MachineIds.CrystalRefiner, new CrystalRefiner(new BasicItemInformation("", MachineIds.CrystalRefiner, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("CrystalRefiner", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null)));

            this.addItem(FarmingObjectIds.DarkwoodCask, new AdvancedCask(new BasicItemInformation("", FarmingObjectIds.DarkwoodCask, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Farming.createAnimationManager("DarkwoodCask", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), .75, 1));

            this.addItem(FarmingObjectIds.MidnightCask, new AdvancedCask(new BasicItemInformation("", FarmingObjectIds.MidnightCask, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Farming.createAnimationManager("MidnightCask", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), .5, .75));

            this.addItem(FarmingObjectIds.AbyssCask, new AdvancedCask(new BasicItemInformation("", FarmingObjectIds.AbyssCask, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Farming.createAnimationManager("AbyssCask", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), .25, .5));


            this.addItem(new AdvancedPreservesJar(new BasicItemInformation("", FarmingObjectIds.HardwoodPreservesJar, "", CategoryNames.Farming, CategoryColors.Farming, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Farming.createAnimationManager("HardwoodPreservesJar", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), .75));

            this.addItem(new AdvancedPreservesJar(new BasicItemInformation("", FarmingObjectIds.AncientPreservesJar, "", CategoryNames.Farming, CategoryColors.Farming, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Farming.createAnimationManager("AncientPreservesJar", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), .5));


            this.addItem(new AdvancedKeg(new BasicItemInformation("", FarmingObjectIds.HardwoodKeg, "", CategoryNames.Farming, CategoryColors.Farming, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Farming.createAnimationManager("HardwoodKeg", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), .75));

            this.addItem(new AdvancedKeg(new BasicItemInformation("", FarmingObjectIds.IndustrialKeg, "", CategoryNames.Farming, CategoryColors.Farming, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Farming.createAnimationManager("IndustrialKeg", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), .5));


            int frameSize = 16;

            this.addItem(new AutomaticTreeFarm(new BasicItemInformation("", FarmingObjectIds.AutomaticTreeFarm, "", CategoryNames.Farming, CategoryColors.Farming, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Farming.createAnimationManager("AutomaticTreeFarm", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,frameSize*2,frameSize,frameSize*2)) },
                    {AutomaticTreeFarm.OAK_0_ANIMATION_KEY,  new Animation(0,frameSize*4,16,32) },
                    {AutomaticTreeFarm.OAK_1_ANIMATION_KEY,  new Animation(frameSize,frameSize*4,16,32) },
                    {AutomaticTreeFarm.OAK_2_ANIMATION_KEY,  new Animation(frameSize*2,frameSize*4,16,32) },
                    {AutomaticTreeFarm.OAK_3_ANIMATION_KEY,  new Animation(frameSize*3,frameSize*4,16,32) },

                    {AutomaticTreeFarm.MAPLE_0_ANIMATION_KEY,  new Animation(0,frameSize*6,16,32) },
                    {AutomaticTreeFarm.MAPLE_1_ANIMATION_KEY,  new Animation(frameSize,frameSize*6,16,32) },
                    {AutomaticTreeFarm.MAPLE_2_ANIMATION_KEY,  new Animation(frameSize*2,frameSize*6,16,32) },
                    {AutomaticTreeFarm.MAPLE_3_ANIMATION_KEY,  new Animation(frameSize*3,frameSize*6,16,32) },

                    {AutomaticTreeFarm.PINE_0_ANIMATION_KEY,  new Animation(0,frameSize*8,16,32) },
                    {AutomaticTreeFarm.PINE_1_ANIMATION_KEY,  new Animation(frameSize,frameSize*8,16,32) },
                    {AutomaticTreeFarm.PINE_2_ANIMATION_KEY,  new Animation(frameSize*2,frameSize*8,16,32) },
                    {AutomaticTreeFarm.PINE_3_ANIMATION_KEY,  new Animation(frameSize*3,frameSize*8,16,32) },

                    {AutomaticTreeFarm.MAHOGANY_0_ANIMATION_KEY,  new Animation(0,frameSize*8,16,32) },
                    {AutomaticTreeFarm.MAHOGANY_1_ANIMATION_KEY,  new Animation(frameSize,frameSize*8,16,32) },
                    {AutomaticTreeFarm.MAHOGANY_2_ANIMATION_KEY,  new Animation(frameSize*2,frameSize*8,16,32) },
                    {AutomaticTreeFarm.MAHOGANY_3_ANIMATION_KEY,  new Animation(frameSize*3,frameSize*8,16,32) },

                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null)));


        }


        protected virtual void addInFurnaces()
        {
            this.addItem(MachineIds.ElectricFurnace, new ElectricFurnace(new BasicItemInformation("", MachineIds.ElectricFurnace, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 2500, true, true, TextureManagers.Objects_Machines.createAnimationManager("ElectricFurnace",
    new Dictionary<string, Animation>()
    {
                    {ElectricFurnace.ELECTRIC_IDLE_ANIMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {ElectricFurnace.ELECTRIC_WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }

    }, ElectricFurnace.ELECTRIC_IDLE_ANIMATION_KEY, ElectricFurnace.ELECTRIC_IDLE_ANIMATION_KEY

    ), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Electric));
            this.addItem(MachineIds.NuclearFurnace, new ElectricFurnace(new BasicItemInformation("", MachineIds.ElectricFurnace, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 10000, true, true, TextureManagers.Objects_Machines.createAnimationManager("ElectricFurnace",
                new Dictionary<string, Animation>()
                {
                    {ElectricFurnace.NUCLEAR_IDLE_ANIMATION_KEY,  new Animation(new Rectangle(0,32,16,32)) },
                    {ElectricFurnace.NUCLEAR_WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,32,16,32)) }

                }, ElectricFurnace.NUCLEAR_IDLE_ANIMATION_KEY, ElectricFurnace.NUCLEAR_IDLE_ANIMATION_KEY
                ), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Nuclear));
            this.addItem(MachineIds.MagicalFurnace, new ElectricFurnace(new BasicItemInformation("", MachineIds.ElectricFurnace, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 50000, true, true, TextureManagers.Objects_Machines.createAnimationManager("ElectricFurnace",
                new Dictionary<string, Animation>()
                {
                    {ElectricFurnace.MAGICAL_IDLE_ANIMATION_KEY,  new Animation(new Rectangle(0,64,16,32)) },
                    {ElectricFurnace.MAGICAL_WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,64,16,32)) }

                }, ElectricFurnace.MAGICAL_IDLE_ANIMATION_KEY, ElectricFurnace.MAGICAL_IDLE_ANIMATION_KEY

                ), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Magical));
        }

        protected virtual void addInGeodeCrushers()
        {
            this.addItem(MachineIds.AdvancedBurnerGeodeCrusher, new AdvancedGeodeCrusher(new BasicItemInformation("", MachineIds.AdvancedBurnerGeodeCrusher, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("CoalAdvancedGeodeCrusher", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Coal));
            this.addItem(MachineIds.ElectricAdvancedGeodeCrusher, new AdvancedGeodeCrusher(new BasicItemInformation("", MachineIds.ElectricAdvancedGeodeCrusher, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("ElectricAdvancedGeodeCrusher", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Electric));
            this.addItem(MachineIds.NuclearAdvancedGeodeCrusher, new AdvancedGeodeCrusher(new BasicItemInformation("", MachineIds.NuclearAdvancedGeodeCrusher, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("NuclearAdvancedGeodeCrusher", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Nuclear));
            this.addItem(MachineIds.MagicalAdvancedGeodeCrusher, new AdvancedGeodeCrusher(new BasicItemInformation("", MachineIds.MagicalAdvancedGeodeCrusher, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("MagicalAdvancedGeodeCrusher", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Magical));


            this.addItem(CraftingStations.Anvil_Id, new AdvancedGeodeCrusher(new BasicItemInformation("", CraftingStations.Anvil_Id, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Crafting.createAnimationManager("Anvil", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,32,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(0,0,32,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(2, 2), Vector2.Zero, null, null), PoweredMachine.PoweredMachineTier.Manual));
        }

        protected virtual void addInMiningDrills()
        {
            MiningDrill electricMiningDrill = new MiningDrill(new BasicItemInformation("", MachineIds.ElectricMiningDrill, "", CategoryNames.Machine, Color.SteelBlue, -300, -300, 0, false, 4000, true, true, TextureManagers.Objects_Machines.createAnimationManager("ElectricMiningDrill", new SerializableDictionary<string, Animation>() {
                {Machine.DEFAULT_ANINMATION_KEY,new Animation(new AnimationFrame(0,0,16,32))  },
                { Machine.WORKING_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,0,16,32,10, 6).appendAnimation(Animation.CreateAnimationFromReverseTextureSequence(0,0,16,32,10, 6))}
            }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Electric);
            this.addItem(MachineIds.ElectricMiningDrill, electricMiningDrill);
            MiningDrill coalMiningDrill = new MiningDrill(new BasicItemInformation("", MachineIds.CoalMiningDrill, "", CategoryNames.Machine, Color.SteelBlue, -300, -300, 0, false, 4000, true, true, TextureManagers.Objects_Machines.createAnimationManager("CoalMiningDrill", new SerializableDictionary<string, Animation>() {
                {Machine.DEFAULT_ANINMATION_KEY,new Animation(new AnimationFrame(0,0,16,32))  },
                { Machine.WORKING_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,0,16,32,10, 6).appendAnimation(Animation.CreateAnimationFromReverseTextureSequence(0,0,16,32,10, 6))}
            }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Coal);

            this.addItem(MachineIds.CoalMiningDrill, coalMiningDrill);
            MiningDrill nuclearMiningDrill = new MiningDrill(new BasicItemInformation("", MachineIds.NuclearMiningDrill, "", CategoryNames.Machine, Color.SteelBlue, -300, -300, 0, false, 4000, true, true, TextureManagers.Objects_Machines.createAnimationManager("NuclearMiningDrill", new SerializableDictionary<string, Animation>() {
                {Machine.DEFAULT_ANINMATION_KEY,new Animation(new AnimationFrame(0,0,16,32))  },
                { Machine.WORKING_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,0,16,32,10, 6).appendAnimation(Animation.CreateAnimationFromReverseTextureSequence(0,0,16,32,10, 6))}
            }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Nuclear);
            this.addItem(MachineIds.NuclearMiningDrill, nuclearMiningDrill);
            MiningDrill magicalMiningDrill = new MiningDrill(new BasicItemInformation("", MachineIds.MagicalMiningDrill, "", CategoryNames.Machine, Color.SteelBlue, -300, -300, 0, false, 4000, true, true, TextureManagers.Objects_Machines.createAnimationManager("MagicalMiningDrill", new SerializableDictionary<string, Animation>() {
                {Machine.DEFAULT_ANINMATION_KEY,new Animation(new AnimationFrame(0,0,16,32))  },
                { Machine.WORKING_ANIMATION_KEY,Animation.CreateAnimationFromTextureSequence(0,0,16,32,10, 6).appendAnimation(Animation.CreateAnimationFromReverseTextureSequence(0,0,16,32,10, 6))}
            }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), PoweredMachine.PoweredMachineTier.Magical);
            this.addItem(MachineIds.MagicalMiningDrill, magicalMiningDrill);
        }

        protected virtual void addInCharcoalKilns()
        {
            this.addItem(new AdvancedCharcoalKiln(new BasicItemInformation("", MachineIds.AdvancedCharcoalKiln, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("AdvancedCharcoalKiln", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null)));
            this.addItem(new AdvancedCharcoalKiln(new BasicItemInformation("", MachineIds.DeluxeCharcoalKiln, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("DeluxCharcoalKiln", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null)));
            this.addItem(new AdvancedCharcoalKiln(new BasicItemInformation("", MachineIds.SuperiorCharcoalKiln, "", CategoryNames.Machine, CategoryColors.Machines, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Machines.createAnimationManager("SuperiorCharcoalKiln", new Dictionary<string, Animation>()
                {
                    {Machine.DEFAULT_ANINMATION_KEY,  new Animation(new Rectangle(0,0,16,32)) },
                    {Machine.WORKING_ANIMATION_KEY,  new Animation(new Rectangle(16,0,16,32)) }
                }, Machine.DEFAULT_ANINMATION_KEY, Machine.DEFAULT_ANINMATION_KEY), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null)));
        }

        protected virtual void addInStorageObjects()
        {

            //New chest types
            int chestCapacity = 36;
            this.addItem(new ItemVault(new BasicItemInformation("", StorageIds.ItemVault, "", CategoryNames.Storage, CategoryColors.Misc, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Storage.createAnimationManager("ItemVault", new Dictionary<string, Animation>()
                {
                    {"Default",  new Animation(new Rectangle(0,0,16,32)) },
                }, "Default", "Default"), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), chestCapacity * 2));


            this.addItem(new ItemVault(new BasicItemInformation("", StorageIds.BigItemVault, "", CategoryNames.Storage, CategoryColors.Misc, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Storage.createAnimationManager("BigItemVault", new Dictionary<string, Animation>()
                {
                    {"Default",  new Animation(new Rectangle(0,0,16,32)) },
                }, "Default", "Default"), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), chestCapacity * 3));

            this.addItem(new ItemVault(new BasicItemInformation("", StorageIds.LargeItemVault, "", CategoryNames.Storage, CategoryColors.Misc, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Storage.createAnimationManager("LargeItemVault", new Dictionary<string, Animation>()
                {
                    {"Default",  new Animation(new Rectangle(0,0,16,32)) },
                }, "Default", "Default"), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), chestCapacity * 4));

            this.addItem(new ItemVault(new BasicItemInformation("", StorageIds.HugeItemVault, "", CategoryNames.Storage, CategoryColors.Misc, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Storage.createAnimationManager("HugeItemVault", new Dictionary<string, Animation>()
                {
                    {"Default",  new Animation(new Rectangle(0,0,16,32)) },
                }, "Default", "Default"), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null), chestCapacity * 5));

            this.addItem(new DimensionalStorageChest(new BasicItemInformation("", StorageIds.DimensionalStorageChest, "", CategoryNames.Storage, CategoryColors.Misc, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Storage.createAnimationManager("DimensionalStorageChest", new Dictionary<string, Animation>()
                {
                    {"Default",  new Animation(new Rectangle(0,0,16,32)) },
                }, "Default", "Default"), Color.White, false, new Vector2(1, 1), new Vector2(0, -1), null, null)));


            //Storage Bags
            this.addItem(new StorageBag(new BasicItemInformation("", StorageIds.SmallItemBag, "", CategoryNames.Storage, CategoryColors.Misc, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Storage.createAnimationManager("SmallPouch", new Dictionary<string, Animation>()
                {
                    {"Default",  new Animation(new Rectangle(0,0,16,16)) },
                }, "Default", "Default"), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null), 5));

            this.addItem(new StorageBag(new BasicItemInformation("", StorageIds.BigItemBag, "", CategoryNames.Storage, CategoryColors.Misc, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Storage.createAnimationManager("BigPouch", new Dictionary<string, Animation>()
                {
                    {"Default",  new Animation(new Rectangle(0,0,32,32),.5f) },
                }, "Default", "Default"), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null), 5));

            this.addItem(new DimensionalStorageBag(new BasicItemInformation("", StorageIds.DimensionalStorageBag, "", CategoryNames.Storage, CategoryColors.Misc, -300, -300, 0, false, 0, true, true, TextureManagers.Objects_Storage.createAnimationManager("DimensionalStorageBag", new Dictionary<string, Animation>()
                {
                    {"Default",  new Animation(new Rectangle(0,0,32,32),.5f) },
                }, "Default", "Default"), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null)));

        }

        /// <summary>
        /// Gets a random object from the dictionary passed in.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public Item getRandomObject(Dictionary<string, CustomObject> dictionary)
        {
            if (dictionary.Count == 0) return null;
            List<CustomObject> objs = new List<CustomObject>();
            foreach (KeyValuePair<string, CustomObject> pair in dictionary)
                objs.Add(pair.Value);
            int rand = Game1.random.Next(0, objs.Count);
            return objs[rand].getOne();
        }

        /// <summary>
        /// Adds in an item to be tracked by the mod's object manager.
        ///
        /// TODO: When SDV 1.6 is released, might need to migrate this to the new ItemRegistry.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="I"></param>
        public void addItem(string key, Item I)
        {
            if (this.itemsById.ContainsKey(key))
                throw new Exception(string.Format("Item with the same key has already been added into the mod! The offending key is {0}", key));
            else
                this.itemsById.Add(key, I);
        }

        public void addItem<T>(T item) where T : StardewValley.Object, IBasicItemInfoProvider
        {
            if (this.itemsById.ContainsKey(item.basicItemInformation.id.Value))
                throw new Exception(string.Format("Item with the same key has already been added into the mod! The offending key is {0}", item.basicItemInformation.id.Value));
            else
                this.itemsById.Add(item.basicItemInformation.id.Value, item);
        }

        /// <summary>
        /// Gets an item from the list of modded items.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Stack"></param>
        /// <returns></returns>
        public virtual Item getItem(string Key, int Stack = 1, int Quality = 0)
        {
            return this.getItem<Item>(Key, Stack, Quality);
        }

        /// <summary>
        /// Gets an item from the list of modded items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="Stack"></param>
        /// <returns></returns>
        public virtual T getItem<T>(string Key, int Stack = 1, int Quality = 0) where T : Item
        {

            if (this.itemsById.ContainsKey(Key))
            {
                Item I = this.itemsById[Key].getOne();
                I.Stack = Stack;
                if (I is StardewValley.Object)
                {
                    (I as StardewValley.Object).Quality = Quality;
                }
                return (T)I;
            }
            else
                throw new InvalidObjectManagerItemException(string.Format("Error: Trying to request an item with id {0} but there is none registered with the object manager!", Key));

        }


        public virtual T getObject<T>(string Key, int Stack = 1, int Quality = 0) where T : StardewValley.Object
        {

            return this.getItem<T>(Key, Stack, Quality);

        }

        /// <summary>
        /// Gets the SDV 1.6+ item id for the item. 
        /// </summary>
        /// <param name="sdvObject"></param>
        /// <returns></returns>
        public virtual string createVanillaObjectId(Enums.SDVObject sdvObject)
        {
            return this.createVanillaObjectId((int)sdvObject);
        }

        /// <summary>
        /// Gets the SDV 1.6+ item id for the item. 
        /// </summary>
        /// <param name="sdvObject"></param>
        /// <returns></returns>
        public virtual string createVanillaBigCraftableId(Enums.SDVBigCraftable sdvObject)
        {
            return this.createVanillaBigCraftableId(((int)sdvObject));
        }

        public virtual string createVanillaObjectId(int ParentSheetIndex)
        {
            return this.createVanillaItemId(StardewValleyObjectIdPrefix, ParentSheetIndex);
        }
        public virtual string createVanillaBigCraftableId(int ParentSheetIndex)
        {
            return this.createVanillaItemId(StardewValleyBigCraftablePrefix, ParentSheetIndex);
        }

        /// <summary>
        /// Creates a vanilla item id given an item prefix and an item id.
        /// </summary>
        /// <param name="ObjectPrefix"></param>
        /// <param name="ObjectId"></param>
        /// <returns></returns>
        public virtual string createVanillaItemId(string ObjectPrefix, int ObjectId)
        {
            return this.createVanillaItemId(ObjectPrefix, ObjectId.ToString());
        }

        /// <summary>
        /// Creates a vanilla item id given an item prefix and an item id.
        /// </summary>
        /// <param name="ObjectPrefix"></param>
        /// <param name="ObjectId"></param>
        /// <returns></returns>
        public virtual string createVanillaItemId(string ObjectPrefix, string ObjectId)
        {
            return string.Format("{0}{1}", ObjectPrefix, ObjectId);
        }

        /// <summary>
        /// Converts an object id from the 1.6 format to the 1.5 format.
        /// </summary>
        /// <param name="ObjectId"></param>
        /// <returns>-1 if failure. Otherwise returns an int representation.</returns>
        public virtual int convertSDVStringObjectIdToIntObjectId(string ObjectId)
        {

            int position = ObjectId.IndexOf(")");
            if (position == -1) return -1;
            string converted = ObjectId.Remove(0, ObjectId.IndexOf(")") + 1);
            return Convert.ToInt32(converted);
        }

        /// <summary>
        /// Gets a StardewValley vanilla item with the given id.
        /// </summary>
        /// <param name="sdvObjectId"></param>
        /// <param name="Stack"></param>
        /// <returns></returns>
        public virtual Item getItem(Enums.SDVObject sdvObjectId, int Stack = 1)
        {
            return this.getItem(this.createVanillaObjectId(sdvObjectId), Stack);
        }

        public virtual StardewValley.Object getObject(Enums.SDVObject sdvId, int Stack = 1)
        {
            return (StardewValley.Object)this.getItem(sdvId, Stack);
        }

        /// <summary>
        /// Gets a Stardew Valley vanilla big craftable object with the given id.
        /// </summary>
        /// <param name="sdvBigCraftableId"></param>
        /// <param name="Stack"></param>
        /// <returns></returns>
        public virtual Item getItem(Enums.SDVBigCraftable sdvBigCraftableId, int Stack = 1)
        {
            return this.getItem(this.createVanillaBigCraftableId(sdvBigCraftableId), Stack);
        }

        /// <summary>
        /// Adds a new object manager to the master pool of managers.
        /// </summary>
        /// <param name="Manifest"></param>
        public static void addObjectManager(IManifest Manifest)
        {
            if (ObjectPools == null) ObjectPools = new Dictionary<string, ObjectManager>();
            ObjectPools.Add(Manifest.UniqueID, new ObjectManager(Manifest));
        }


        /// <summary>
        /// Cleans up all stored information.
        /// </summary>
        public void returnToTitleCleanUp()
        {

        }

    }
}
