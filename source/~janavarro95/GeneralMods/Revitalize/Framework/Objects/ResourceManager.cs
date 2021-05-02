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
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Revitalize.Framework.Factories.Objects.Resources;
using Revitalize.Framework.Managers;
using Revitalize.Framework.Objects.InformationFiles;
using Revitalize.Framework.Objects.Items.Resources;
using Revitalize.Framework.Objects.Resources.OreVeins;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardustCore.Animations;
using StardustCore.UIUtilities;

namespace Revitalize.Framework.Objects
{
    public class ResourceManager
    {

        private string oreResourceDataPath= Path.Combine("Data", "Objects", "Resources","Ore");

        /// <summary>
        /// A static reference to the resource manager for quicker access.
        /// </summary>
        public static ResourceManager self;

        /// <summary>
        /// A list of all of the ores held by the resource manager.
        /// </summary>
        public Dictionary<string, OreVeinObj> oreVeins;
        public Dictionary<string, OreResourceInformation> oreResourceInformationTable;
        public Dictionary<string, Ore> ores;
        public Dictionary<string, CustomObject> resources;

        public Dictionary<string, ResourceInformation> miningDrillResources;
        public Dictionary<string, Fluid> fluids;

        /// <summary>
        /// A dictionary containing the names of all objects that can be burned with their burn times for a value.
        /// </summary>
        public Dictionary<string, int> burnableObjects; 

        /// <summary>
        /// A list of all visited floors on the current visit to the mines.
        /// </summary>
        public List<int> visitedFloors;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourceManager()
        {
            self = this;
            this.oreVeins = new Dictionary<string, OreVeinObj>();
            this.oreResourceInformationTable = new Dictionary<string, OreResourceInformation>();
            this.ores = new Dictionary<string, Ore>();
            this.visitedFloors = new List<int>();
            this.resources = new Dictionary<string, CustomObject>();
            this.miningDrillResources = new Dictionary<string, ResourceInformation>();
            this.fluids = new Dictionary<string, Fluid>();
            this.burnableObjects = new Dictionary<string, int>();
        }


        //Loads in the items for the resource manager.
        public void loadInItems()
        {
            this.loadInOreItems();
            this.loadInResourceItems();
            this.serializeOreVeins();
            this.loadOreVeins();
            this.loadInMiningDrillLootTable();
            this.loadInFluidDictionary();
            this.loadInBurnableObjects();
        }

        private void loadInFluidDictionary()
        {
            this.fluids.Add("Water", new Fluid("Water", Color.Blue));
            this.fluids.Add("Steam", new Fluid("Steam", Color.White));
        }

        private void loadInBurnableObjects()
        {
            this.burnableObjects.Add("Coal", TimeUtilities.GetMinutesFromTime(0, 1, 0));
            this.burnableObjects.Add("Wood", TimeUtilities.GetMinutesFromTime(0, 0, 10));
        }
        private void loadInMiningDrillLootTable()
        {
            this.miningDrillResources.Add("Bauxite", new ResourceInformation(this.getResource("Bauxite"), ModCore.Configs.miningDrillConfig.amountOfBauxiteToMine.min, ModCore.Configs.miningDrillConfig.amountOfBauxiteToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.bauxiteMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Tin", new ResourceInformation(this.getResource("Tin"), ModCore.Configs.miningDrillConfig.amountOfTinToMine.min, ModCore.Configs.miningDrillConfig.amountOfTinToMine.max, 1, 1,1,ModCore.Configs.miningDrillConfig.tinMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Lead", new ResourceInformation(this.getResource("Lead"), ModCore.Configs.miningDrillConfig.amountOfLeadToMine.min, ModCore.Configs.miningDrillConfig.amountOfLeadToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.leadMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Silver", new ResourceInformation(this.getResource("Silver"), ModCore.Configs.miningDrillConfig.amountOfSilverToMine.min, ModCore.Configs.miningDrillConfig.amountOfSilverToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.silverMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Titanium", new ResourceInformation(this.getResource("Titanium"), ModCore.Configs.miningDrillConfig.amountOfTitaniumToMine.min, ModCore.Configs.miningDrillConfig.amountOfTitaniumToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.titaniumMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Prismatic", new ResourceInformation(this.getResource("PrismaticNugget"), ModCore.Configs.miningDrillConfig.amountOfPrismaticNuggetsToMine.min, ModCore.Configs.miningDrillConfig.amountOfPrismaticNuggetsToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.prismaticNuggetMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Copper", new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.CopperOre, 1), ModCore.Configs.miningDrillConfig.amountOfCopperToMine.min, ModCore.Configs.miningDrillConfig.amountOfCopperToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.copperMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Iron", new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.IronOre, 1), ModCore.Configs.miningDrillConfig.amountOfIronToMine.min, ModCore.Configs.miningDrillConfig.amountOfIronToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.ironMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Gold", new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.GoldOre, 1), ModCore.Configs.miningDrillConfig.amountOfGoldToMine.min, ModCore.Configs.miningDrillConfig.amountOfGoldToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.goldMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Iridium", new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.IridiumOre,1), ModCore.Configs.miningDrillConfig.amountOfIridiumToMine.min, ModCore.Configs.miningDrillConfig.amountOfIridiumToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.iridiumMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Stone", new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.Stone, 1), ModCore.Configs.miningDrillConfig.amountOfStoneToMine.min, ModCore.Configs.miningDrillConfig.amountOfStoneToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.stoneMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Clay", new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.Clay, 1), ModCore.Configs.miningDrillConfig.amountOfClayToMine.min, ModCore.Configs.miningDrillConfig.amountOfClayToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.clayMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Sand", new ResourceInformation(this.getResource("Sand"), ModCore.Configs.miningDrillConfig.amountOfSandToMine.min, ModCore.Configs.miningDrillConfig.amountOfSandToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.sandMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("Geode", new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.Geode,1), ModCore.Configs.miningDrillConfig.amountOfGeodesToMine.min, ModCore.Configs.miningDrillConfig.amountOfGeodesToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.geodeMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("FrozenGeode", new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.FrozenGeode, 1), ModCore.Configs.miningDrillConfig.amountOfFrozenGeodesToMine.min, ModCore.Configs.miningDrillConfig.amountOfFrozenGeodesToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.frozenGeodeMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("MagmaGeode", new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.MagmaGeode, 1), ModCore.Configs.miningDrillConfig.amountOfMagmaGeodesToMine.min, ModCore.Configs.miningDrillConfig.amountOfMagmaGeodesToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.magmaGeodeMineChance, 0, 0, 0, 0));
            this.miningDrillResources.Add("OmniGeode", new ResourceInformation(new StardewValley.Object((int)Enums.SDVObject.OmniGeode, 1), ModCore.Configs.miningDrillConfig.amountOfOmniGeodesToMine.min, ModCore.Configs.miningDrillConfig.amountOfOmniGeodesToMine.max, 1, 1, 1,ModCore.Configs.miningDrillConfig.omniGeodeMineChance, 0, 0, 0, 0));
        }

        /// <summary>
        /// Loads in all of the ore veins for the game.
        /// </summary>
        private void loadOreVeins()
        {
            //The pancake ore.

            /*
            OreVeinObj testOre = new OreVeinObj(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Test", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Test"), typeof(OreVeinTile), Color.White), new BasicItemInformation("Test Ore Vein", "Omegasis.Revitalize.Resources.Ore.Test", "A ore vein that is used for testing purposes.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, Vector2.Zero, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Test"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Test"), new Animation(0, 0, 16, 16)), Color.White, false, null, null));
            testOre.addComponent(new Vector2(0, 0), new OreVeinTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Test", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Test"), typeof(OreVeinTile), Color.White), new BasicItemInformation("Test Ore Vein", "Omegasis.Revitalize.Resources.Ore.Test", "A ore vein that is used for testing purposes.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, Vector2.Zero, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Test"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Test"), new Animation(0, 0, 16, 16)), Color.White, false, null, null),
                new InformationFiles.OreResourceInformation(new StardewValley.Object(211, 1), true, true, true, false, new List<IntRange>()
            {
                new IntRange(1,9)
            }, new List<IntRange>(), (i => i == 1), (i => i % 10 == 0), 1, 5, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, (i => i == -1), (i => i == -1), 1d, 1.0d, 0.10d, 1d, 1d, 0, 0, 0, 0), new List<ResourceInformaton>(),4));

            this.ores.Add("Omegasis.Revitalize.Resources.Ore.Test", testOre);
            */

            if (!Directory.Exists(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", this.oreResourceDataPath))) Directory.CreateDirectory(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", this.oreResourceDataPath));
            List<string> directories = Directory.GetDirectories(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", this.oreResourceDataPath)).ToList();
            directories.Add(Path.Combine(ModCore.ModHelper.DirectoryPath, "Content", this.oreResourceDataPath));
            foreach (string directory in directories)
            {
                string[] files = Directory.GetFiles(directory);

                Dictionary<string, OreVeinObj> objs = new Dictionary<string, OreVeinObj>();

                //Deserialize container.
                foreach (string file in files)
                {
                    if ((Path.GetFileName(file)).Contains("_") == true) continue;
                    else
                    {
                        OreFactoryInfo factoryInfo = ModCore.Serializer.DeserializeContentFile<OreFactoryInfo>(file);
                        objs.Add(Path.GetFileNameWithoutExtension(file), new OreVeinObj(factoryInfo.PyTkData, factoryInfo.info));
                    }
                }
                //Deseralize components
                foreach (string file in files)
                {
                    if ((Path.GetFileName(file)).Contains("_") == false) continue;
                    else
                    {

                        string[] splits = Path.GetFileNameWithoutExtension(file).Split('_');
                        string name = splits[0];
                        Vector2 offset = new Vector2(Convert.ToInt32(splits[1]), Convert.ToInt32(splits[2]));
                        OreFactoryInfo info = ModCore.Serializer.DeserializeContentFile<OreFactoryInfo>(file);

                        OreVeinTile orePiece = new OreVeinTile(info.PyTkData, info.info,info.OreSpawnInfo,info.ExtraDrops,info.Health);
                        objs[name].addComponent(offset, orePiece);
                    }
                }
                foreach (var v in objs)
                {
                    this.oreVeins.Add(v.Value.info.id, v.Value);
                    //ModCore.ObjectManager.lamps.Add(v.Value.info.id, v.Value);
                }
            }

        }

        /// <summary>
        /// Serializes an example ore to eb
        /// </summary>
        private void serializeOreVeins() {
            OreVeinObj tinOre = new OreVeinObj(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Tin", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Tin"), typeof(OreVeinObj), Color.White), new BasicItemInformation("Tin Ore Vein", "Omegasis.Revitalize.Resources.Ore.Tin", "A ore vein that is full of tin.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Tin"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Tin"), new Animation(0, 0, 16, 16)), Color.White, false, null, null));
            OreVeinTile tinOre_0_0= new OreVeinTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Tin", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Tin"), typeof(OreVeinTile), Color.White), new BasicItemInformation("Tin Ore Vein", "Omegasis.Revitalize.Resources.Ore.Tin", "A ore vein that is full of tin.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Tin"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Tin"), new Animation(0, 0, 16, 16)), Color.White, false, null, null),
                new InformationFiles.OreResourceInformation(this.getOre("Tin"), true, true, true, false, new List<IntRange>()
            {
                new IntRange(1,20)
            }, new List<IntRange>(), (i => i == 1), (i => i % 10 == 0), 1, 3, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, null,null, 0.80d, 0.20d, 0.25d, 1d, 1d, 1, 1, 1, 1), new List<ResourceInformation>(), 4);

            OreFactoryInfo tinOre_0_0_file = new OreFactoryInfo(tinOre_0_0);
            OreFactoryInfo tinOre_file = new OreFactoryInfo(tinOre);

            ModCore.Serializer.SerializeContentFile("TinOre_0_0", tinOre_0_0_file,Path.Combine(this.oreResourceDataPath,"TinOre"));
            ModCore.Serializer.SerializeContentFile("TinOre", tinOre_file, Path.Combine(this.oreResourceDataPath, "TinOre"));

            
            OreVeinObj bauxiteOre = new OreVeinObj(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Bauxite", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Bauxite"), typeof(OreVeinObj), Color.White), new BasicItemInformation("Bauxite Ore Vein", "Omegasis.Revitalize.Resources.Ore.Bauxite", "A ore vein that is full of bauxite ore.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Bauxite"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Bauxite"), new Animation(0, 0, 16, 16)), Color.White, false, null, null));
            OreVeinTile bauxiteOre_0_0 = new OreVeinTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Bauxite", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Bauxite"), typeof(OreVeinTile), Color.White), new BasicItemInformation("Bauxite Ore Vein", "Omegasis.Revitalize.Resources.Ore.Bauxite", "A ore vein that is full of bauxite ore.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Bauxite"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Bauxite"), new Animation(0, 0, 16, 16)), Color.White, false, null, null),
                new InformationFiles.OreResourceInformation(this.getOre("Bauxite"), true, true, true, false, new List<IntRange>()
            {
                new IntRange(20,50)
            }, new List<IntRange>(), null, (i => i % 10 == 0), 1, 3, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, null, null, .70d, 0.16d, 0.20d, 1d, 1d, 0, 0, 0, 0), new List<ResourceInformation>(), 5);

            OreFactoryInfo bauxiteOre_0_0_file = new OreFactoryInfo(bauxiteOre_0_0);
            OreFactoryInfo bauxiteOre_file = new OreFactoryInfo(bauxiteOre);

            ModCore.Serializer.SerializeContentFile("BauxiteOre_0_0", bauxiteOre_0_0_file, Path.Combine(this.oreResourceDataPath, "BauxiteOre"));
            ModCore.Serializer.SerializeContentFile("BauxiteOre", bauxiteOre_file, Path.Combine(this.oreResourceDataPath, "BauxiteOre"));

            
            OreVeinObj silverOre = new OreVeinObj(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Silver", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Silver"), typeof(OreVeinObj), Color.White), new BasicItemInformation("Silver Ore Vein", "Omegasis.Revitalize.Resources.Ore.Silver", "A ore vein that is full of silver ore.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Silver"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Silver"), new Animation(0, 0, 16, 16)), Color.White, false, null, null));
            OreVeinTile silverOre_0_0 = new OreVeinTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Silver", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Silver"), typeof(OreVeinTile), Color.White), new BasicItemInformation("Silver Ore Vein", "Omegasis.Revitalize.Resources.Ore.Silver", "A ore vein that is full of silver ore.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Silver"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Silver"), new Animation(0, 0, 16, 16)), Color.White, false, null, null),
                new InformationFiles.OreResourceInformation(this.getOre("Silver"), true, true, true, false, new List<IntRange>()
            {
                new IntRange(60,100)
            }, new List<IntRange>(), null, (i => i % 10 == 0), 1, 3, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, null, null, .50d, 0.10d, 0.14d, 1d, 1d, 0, 0, 0, 0), new List<ResourceInformation>(), 6);

            OreFactoryInfo silverOre_0_0_file = new OreFactoryInfo(silverOre_0_0);
            OreFactoryInfo silverOre_file = new OreFactoryInfo(silverOre);
            
            ModCore.Serializer.SerializeContentFile("SilverOre_0_0", silverOre_0_0_file, Path.Combine(this.oreResourceDataPath, "SilverOre"));
            ModCore.Serializer.SerializeContentFile("SilverOre", silverOre_file, Path.Combine(this.oreResourceDataPath, "SilverOre"));
            
            OreVeinObj leadOre = new OreVeinObj(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Lead", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Lead"), typeof(OreVeinObj), Color.White), new BasicItemInformation("Lead Ore Vein", "Omegasis.Revitalize.Resources.Ore.Lead", "A ore vein that is full of lead ore.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Lead"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Lead"), new Animation(0, 0, 16, 16)), Color.White, false, null, null));
            OreVeinTile leadOre_0_0 = new OreVeinTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Lead", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Lead"), typeof(OreVeinTile), Color.White), new BasicItemInformation("Lead Ore Vein", "Omegasis.Revitalize.Resources.Ore.Lead", "A ore vein that is full of lead ore.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Lead"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Lead"), new Animation(0, 0, 16, 16)), Color.White, false, null, null),
                new InformationFiles.OreResourceInformation(this.getOre("Lead"), true, true, true, false, new List<IntRange>()
            {
                new IntRange(60,70),
                new IntRange(90,120)
            }, new List<IntRange>(), null, (i => i % 10 == 0), 1, 3, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, null, null, .60d, 0.13d, 0.17d, 1d, 1d, 0, 0, 0, 0), new List<ResourceInformation>(), 7);

            OreFactoryInfo leadOre_0_0_file = new OreFactoryInfo(leadOre_0_0);
            OreFactoryInfo leadOre_file = new OreFactoryInfo(leadOre);

            ModCore.Serializer.SerializeContentFile("LeadOre_0_0", leadOre_0_0_file, Path.Combine(this.oreResourceDataPath, "LeadOre"));
            ModCore.Serializer.SerializeContentFile("LeadOre", leadOre_file, Path.Combine(this.oreResourceDataPath, "LeadOre"));

            
            OreVeinObj titaniumOre = new OreVeinObj(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Titanium", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Titanium"), typeof(OreVeinObj), Color.White), new BasicItemInformation("Titanium Ore Vein", "Omegasis.Revitalize.Resources.Ore.Titanium", "A ore vein that is full of lead ore.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Titanium"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Titanium"), new Animation(0, 0, 16, 16)), Color.White, false, null, null));
            OreVeinTile titaniumOre_0_0 = new OreVeinTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Titanium", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Titanium"), typeof(OreVeinTile), Color.White), new BasicItemInformation("Titanium Ore Vein", "Omegasis.Revitalize.Resources.Ore.Titanium", "A ore vein that is full of lead ore.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Titanium"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Titanium"), new Animation(0, 0, 16, 16)), Color.White, false, null, null),
                new InformationFiles.OreResourceInformation(this.getOre("Titanium"), true, true, true, false, new List<IntRange>()
            {
                new IntRange(60,70),
                new IntRange(90,120)
            }, new List<IntRange>(), null, (i => i % 10 == 0), 1, 3, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, null, null, .40d, 0.05d, 0.10d, 1d, 1d, 0, 0, 0, 0), new List<ResourceInformation>(), 8);

            OreFactoryInfo titaniumOre_0_0_file = new OreFactoryInfo(titaniumOre_0_0);
            OreFactoryInfo titaniumOre_file = new OreFactoryInfo(titaniumOre);

            ModCore.Serializer.SerializeContentFile("TitaniumOre_0_0", titaniumOre_0_0_file, Path.Combine(this.oreResourceDataPath, "TitaniumOre"));
            ModCore.Serializer.SerializeContentFile("TitaniumOre", titaniumOre_file, Path.Combine(this.oreResourceDataPath, "TitaniumOre"));


            OreVeinObj prismaticOre = new OreVeinObj(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Prismatic", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Prismatic"), typeof(OreVeinObj), Color.White), new BasicItemInformation("Prismatic Ore Vein", "Omegasis.Revitalize.Resources.Ore.Prismatic", "A ore vein that contains rare prismatic nuggets!", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Prismatic"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Prismatic"), new Animation(0, 0, 16, 16)), Color.White, false, null, null));
            OreVeinTile prismaticOre_0_0 = new OreVeinTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Resources.Ore.Prismatic", TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Prismatic"), typeof(OreVeinTile), Color.White), new BasicItemInformation("Prismatic Ore Vein", "Omegasis.Revitalize.Resources.Ore.Prismatic", "A ore vein that is full of prismatic ore.", "Revitalize.Ore", Color.Black, -300, 0, false, 350, true, true, TextureManager.GetTexture(ModCore.Manifest, "Resources.Ore", "Prismatic"), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Resources.Ore", "Prismatic"), new Animation(0, 0, 16, 16)), Color.White, false, null, null),
                new InformationFiles.OreResourceInformation(this.getOre("PrismaticNugget"), true, true, true, false, new List<IntRange>()
            {
                new IntRange(110,120)
            }, new List<IntRange>(), null, (i => i % 10 == 0), 1, 3, 1, 1, new IntRange(1, 1), new IntRange(1, 1), new IntRange(1, 5), new List<IntRange>()
            {
                new IntRange(1,9999)
            }, new List<IntRange>()
            {
            }, null, null, .05d, 0.01d, 0.01d, 0.10, 1d, 1, 1, 1,1), new List<ResourceInformation>(), 10);

            OreFactoryInfo prismaticOre_0_0_file = new OreFactoryInfo(prismaticOre_0_0);
            OreFactoryInfo prismaticOre_file = new OreFactoryInfo(prismaticOre);

            ModCore.Serializer.SerializeContentFile("PrismaticOre_0_0", prismaticOre_0_0_file, Path.Combine(this.oreResourceDataPath, "PrismaticOre"));
            ModCore.Serializer.SerializeContentFile("PrismaticOre", prismaticOre_file, Path.Combine(this.oreResourceDataPath, "PrismaticOre"));


        }

        /// <summary>
        /// Loads in all of the ore items into the game.
        /// </summary>
        private void loadInOreItems()
        {
            Ore tinOre = new Ore(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.TinOre", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TinOre"), typeof(Ore), Color.White, true), new BasicItemInformation("Tin Ore", "Omegasis.Revitalize.Items.Resources.Ore.TinOre", "Tin ore that can be smelted into tin ingots for further use.", "Ore", Color.Silver, -300, 0, false, 7,false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TinOre"), new AnimationManager(), Color.White, true, null, null), 1);
            this.ores.Add("Tin", tinOre);

            Ore bauxiteOre = new Ore(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.Bauxite", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "BauxiteOre"), typeof(Ore), Color.White, true), new BasicItemInformation("Bauxite Ore", "Omegasis.Revitalize.Items.Resources.Ore.BauxiteOre", "Bauxite ore that can be smelted into aluminum ingots for further use.", "Ore", Color.Silver, -300, 0, false, 11, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "BauxiteOre"), new AnimationManager(), Color.White, true, null, null), 1);
            this.ores.Add("Bauxite", bauxiteOre);
            this.ores.Add("Aluminum", bauxiteOre);

            Ore leadOre = new Ore(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.LeadOre", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "LeadOre"), typeof(Ore), Color.White, true), new BasicItemInformation("Lead Ore", "Omegasis.Revitalize.Items.Resources.Ore.LeadOre", "Lead ore that can be smelted into lead ingots for further use.", "Ore", Color.Silver, -300, 0, false, 15, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "LeadOre"), new AnimationManager(), Color.White, true, null, null), 1);
            this.ores.Add("Lead", leadOre);

            Ore silverOre = new Ore(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.SilverOre", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "SilverOre"), typeof(Ore), Color.White, true), new BasicItemInformation("Silver Ore", "Omegasis.Revitalize.Items.Resources.Ore.SilverOre", "Silver ore that can be smelted into silver ingots for further use.", "Ore", Color.Silver, -300, 0, false, 20, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "SilverOre"), new AnimationManager(), Color.White, true, null, null), 1);
            this.ores.Add("Silver", silverOre);

            Ore titaniumOre = new Ore(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.TitaniumOre", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TitaniumOre"), typeof(Ore), Color.White, true), new BasicItemInformation("Titanium Ore", "Omegasis.Revitalize.Items.Resources.Ore.TitaniumOre", "Titanium ore that can be smelted into titanium ingots for further use.", "Ore", Color.Silver, -300, 0, false, 35, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TitaniumOre"), new AnimationManager(), Color.White, true, null, null), 1);
            this.ores.Add("Titanium", titaniumOre);

            Ore prismaticOre = new Ore(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.PrismaticNugget", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "PrismaticNugget"), typeof(Ore), Color.White, true), new BasicItemInformation("Prismatic Nugget", "Omegasis.Revitalize.Items.Resources.Ore.PrismaticNugget", "Rare prismatic ore that can be smelted into a prismatic shard when seven are gathered.", "Ore", Color.Silver, -300, 0, false, 200, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "PrismaticNugget"), new AnimationManager(), Color.White, true, null, null), 1);
            this.ores.Add("PrismaticNugget", prismaticOre);

            CustomObject tinIngot = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.TinIngot", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TinIngot"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Tin Ingot", "Omegasis.Revitalize.Items.Resources.Ore.TinIngot", "A tin ingot that can be used for crafting purposes.", "Metal", Color.Silver, -300, 0, false,75, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TinIngot"), new AnimationManager(), Color.White, true, null, null), 1);
            ModCore.ObjectManager.AddItem("TinIngot", tinIngot);

            CustomObject aluminumIngot = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.AluminumIngot", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "AluminumIngot"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Aluminum Ingot", "Omegasis.Revitalize.Items.Resources.Ore.AluminumIngot", "An aluminum ingot that can be used for crafting purposes.", "Ore", Color.Silver, -300, 0, false, 120, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "AluminumIngot"), new AnimationManager(), Color.White, true, null, null), 1);
            ModCore.ObjectManager.AddItem("AluminumIngot", aluminumIngot);

            CustomObject leadIngot = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.LeadIngot", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "LeadIngot"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Lead Ingot", "Omegasis.Revitalize.Items.Resources.Ore.LeadIngot", "A lead ingot that can be used for crafting purposes.", "Ore", Color.Silver, -300, 0, false, 165, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "LeadIngot"), new AnimationManager(), Color.White, true, null, null), 1);
            ModCore.ObjectManager.AddItem("LeadIngot", leadIngot);

            CustomObject silverIngot = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.SilverIngot", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "SilverIngot"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Silver Ingot", "Omegasis.Revitalize.Items.Resources.Ore.SilverIngot", "A silver ingot that can be used for crafting purposes.", "Ore", Color.Silver, -300, 0, false, 220, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "SilverIngot"), new AnimationManager(), Color.White, true, null, null), 1);
            ModCore.ObjectManager.AddItem("SilverIngot", silverIngot);

            CustomObject titaniumIngot = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.TitaniumIngot", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TitaniumIngot"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Titanium Ingot", "Omegasis.Revitalize.Items.Resources.Ore.TitaniumIngot", "A titanium ingot that can be used for crafting purposes.", "Ore", Color.Silver, -300, 0, false, 325, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TitaniumIngot"), new AnimationManager(), Color.White, true, null, null), 1);
            ModCore.ObjectManager.AddItem("TitaniumIngot", titaniumIngot);

            CustomObject brassIngot = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.BrassIngot", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "BrassIngot"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Brass Ingot", "Omegasis.Revitalize.Items.Resources.Ore.BrassIngot", "A brass alloy ingot made from copper and aluminum. It can be used for crafting purposes.", "Ore", Color.Silver, -300, 0, false, 195, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "BrassIngot"), new AnimationManager(), Color.White, true, null, null), 1);
            ModCore.ObjectManager.AddItem("BrassIngot", brassIngot);

            CustomObject bronzeIngot = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.BronzeIngot", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "BronzeIngot"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Bronze Ingot", "Omegasis.Revitalize.Items.Resources.Ore.BronzeIngot", "A bronze alloy ingot made from copper and tin. It can be used for crafting purposes.", "Ore", Color.Silver, -300, 0, false, 150, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "BronzeIngot"), new AnimationManager(), Color.White, true, null, null), 1);
            ModCore.ObjectManager.AddItem("BronzeIngot", bronzeIngot);

            CustomObject electrumIngot = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.ElectrumIngot", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "ElectrumIngot"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Electrum Ingot", "Omegasis.Revitalize.Items.Resources.Ore.ElectrumIngot", "A electrum alloy ingot made from gold and silver. It can be used for crafting purposes for things that use electricity.", "Ore", Color.Silver, -300, 0, false, 500, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "ElectrumIngot"), new AnimationManager(), Color.White, true, null, null), 1);
            ModCore.ObjectManager.AddItem("ElectrumIngot", electrumIngot);

            CustomObject steelIngot = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.SteelIngot", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "SteelIngot"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Steel Ingot", "Omegasis.Revitalize.Items.Resources.Ore.SteelIngot", "A steel ingot that was made by processing iron again with more coal. It can be used for crafting purposes especially for making new machines.", "Ore", Color.Silver, -300, 0, false, 180, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "SteelIngot"), new AnimationManager(), Color.White, true, null, null), 1);
            ModCore.ObjectManager.AddItem("SteelIngot", steelIngot);

            CustomObject bauxiteSand = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.BauxiteSand", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "BauxiteSand"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Bauxite Sand", "Omegasis.Revitalize.Items.Resources.Ore.BauxiteSand", "Bauxite ore which has been crushed into sand. Smelt it to get aluminum ingots.", "Ore", Color.Silver, -300, 0, false, 11, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "BauxiteSand"), new AnimationManager(), Color.White, true, null, null), 1);
            CustomObject copperSand = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.CopperSand", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "CopperSand"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Copper Sand", "Omegasis.Revitalize.Items.Resources.Ore.CopperSand", "Copper ore which has been crushed into sand. Smelt it to get copper bars.", "Ore", Color.Silver, -300, 0, false, 5, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "CopperSand"), new AnimationManager(), Color.White, true, null, null), 1);
            CustomObject goldSand = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.GoldSand", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "GoldSand"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Gold Sand", "Omegasis.Revitalize.Items.Resources.Ore.GoldSand", "Gold ore which has been crushed into sand. Smelt it to get gold bars.", "Ore", Color.Silver, -300, 0, false, 25, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "GoldSand"), new AnimationManager(), Color.White, true, null, null), 1);
            CustomObject ironSand = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.IronSand", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "IronSand"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Iron Sand", "Omegasis.Revitalize.Items.Resources.Ore.IronSand", "Iron ore which has been crushed into sand. Smelt it to get iron bars.", "Ore", Color.Silver, -300, 0, false, 10, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "IronSand"), new AnimationManager(), Color.White, true, null, null), 1);
            CustomObject iridiumSand = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.IridiumSand", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "IridiumSand"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Iridium Sand", "Omegasis.Revitalize.Items.Resources.Ore.IridiumSand", "Iridium ore which has been crushed into sand. Smelt it to get iridium bars.", "Ore", Color.Silver, -300, 0, false, 100, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "IridiumSand"), new AnimationManager(), Color.White, true, null, null), 1);
            CustomObject leadSand = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.LeadSand", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "LeadSand"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Lead Sand", "Omegasis.Revitalize.Items.Resources.Ore.LeadSand", "Lead ore which has been crushed into sand. Smelt it to get lead ingots.", "Ore", Color.Silver, -300, 0, false, 15, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "LeadSand"), new AnimationManager(), Color.White, true, null, null), 1);
            CustomObject silverSand = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.SilverSand", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "SilverSand"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Silver Sand", "Omegasis.Revitalize.Items.Resources.Ore.SilverSand", "Silver ore which has been crushed into sand. Smelt it to get silver ingots.", "Ore", Color.Silver, -300, 0, false, 20, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "SilverSand"), new AnimationManager(), Color.White, true, null, null), 1);
            CustomObject tinSand = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.TinSand", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TinSand"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Tin Sand", "Omegasis.Revitalize.Items.Resources.Ore.TinSand", "Tin ore which has been crushed into sand. Smelt it to get tin ingots.", "Ore", Color.Silver, -300, 0, false, 7, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TinSand"), new AnimationManager(), Color.White, true, null, null), 1);
            CustomObject titaniumSand = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Ore.TitaniumSand", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TitaniumSand"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Copper Sand", "Omegasis.Revitalize.Items.Resources.Ore.TitaniumSand", "Titanium ore which has been crushed into sand. Smelt it to get titanium bars.", "Ore", Color.Silver, -300, 0, false, 35, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Ore", "TitaniumSand"), new AnimationManager(), Color.White, true, null, null), 1);

            this.resources.Add("BauxiteSand", bauxiteSand);
            this.resources.Add("CopperSand", copperSand);
            this.resources.Add("GoldSand", goldSand);
            this.resources.Add("IronSand", ironSand);
            this.resources.Add("IridiumSand", iridiumSand);
            this.resources.Add("LeadSand", leadSand);
            this.resources.Add("SilverSand", silverSand);
            this.resources.Add("TinSand", tinSand);
            this.resources.Add("TitaniumSand", titaniumSand);
        }

        private void loadInResourceItems()
        {
            CustomObject sand=new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Misc.Sand", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Misc", "Sand"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Sand", "Omegasis.Revitalize.Items.Resources.Misc.Sand", "Sand which is made from tiny rocks and can be used for smelting. Also unfun to have inside of swimwear.", "Resource", Color.Brown, -300, 0, false, 2, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Misc", "Sand"), new AnimationManager(), Color.White, true, null, null));
            this.resources.Add("Sand", sand);

            CustomObject glass_normal = new CustomObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Items.Resources.Misc.Glass", TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Misc", "Glass"), typeof(CustomObject), Color.White, true), new BasicItemInformation("Glass", "Omegasis.Revitalize.Items.Resources.Misc.Glass", "Glass smelted from sand. Used in decorations and glass objects.", "Resource", Color.Brown, -300, 0, false, 20, false, false, TextureManager.GetTexture(ModCore.Manifest, "Items.Resources.Misc", "Glass"), new AnimationManager(), Color.White, true, null, null));
            this.resources.Add("Glass", glass_normal);
        }

        /// <summary>
        /// Gets an ore from the list of stored ores in this mod.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Stack"></param>
        /// <returns></returns>
        public Ore getOre(string name,int Stack=1)
        {
            if (this.ores.ContainsKey(name))
            {
                Ore o = (Ore)this.ores[name].getOne();
                o.Stack = Stack;
                return o;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a fluid from the fluid dictionary.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Fluid getFluid(string name)
        {
            if (this.fluids.ContainsKey(name))
            {
                return this.fluids[name].Copy();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get a resource from the resource maanger.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Stack"></param>
        /// <returns></returns>
        public CustomObject getResource(string name, int Stack=1)
        {
            Ore o = this.getOre(name, Stack);
            if (o != null) return o;

            if (this.resources.ContainsKey(name))
            {
                CustomObject obj = (CustomObject)this.resources[name].getOne();
                obj.Stack = Stack;
                return obj;
            }
            else
            {
                return null;
            }
        }

        public OreResourceInformation getOreResourceInfo(string id)
        {
            if (this.oreVeins.ContainsKey(id))
            {
                return (OreResourceInformation)this.oreVeins[id].resourceInfo;
            }
            else
            {
                return null;
            }
        }

        public List<ResourceInformation> getExtraDropInformationFromOres(string id)
        {
            if (this.oreVeins.ContainsKey(id))
            {
                return (this.oreVeins[id].objects[Vector2.Zero] as OreVeinTile).extraDrops;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Checks to see if a resource can be spawned here.
        /// </summary>
        /// <param name="OBJ"></param>
        /// <param name="Location"></param>
        /// <param name="TilePosition"></param>
        /// <returns></returns>
        public bool canResourceBeSpawnedHere(MultiTiledObject OBJ, GameLocation Location, Vector2 TilePosition)
        {
            return OBJ.canBePlacedHere(Location, TilePosition) && Location.isTileLocationTotallyClearAndPlaceable(TilePosition);
        }


        //~~~~~~~~~~~~~~~~~~~~~~~//
        //  World Ore Spawn Code //
        //~~~~~~~~~~~~~~~~~~~~~~~//

        #region


        /// <summary>
        /// Spawns an ore vein at the given location if possible.
        /// </summary>
        /// <param name="name"></param>
        public bool spawnOreVein(string name, GameLocation Location, Vector2 TilePosition)
        {
            if (this.oreVeins.ContainsKey(name))
            {
                OreVeinObj spawn;
                this.oreVeins.TryGetValue(name, out spawn);
                if (spawn != null)
                {
                    spawn = (OreVeinObj)spawn.getOne();
                    bool spawnable = this.canResourceBeSpawnedHere(spawn, Location, TilePosition);
                    if (spawnable)
                    {
                        //ModCore.log("Location is: " + Location.Name);
                        spawn.placementAction(Location, (int)TilePosition.X * Game1.tileSize, (int)TilePosition.Y * Game1.tileSize, Game1.player);
                    }
                    else
                    {
                        ModCore.log("Can't spawn ore: " + name + "at tile location: " + TilePosition);
                    }
                    return spawnable;
                }
                ModCore.log("Key doesn't exist. Weird.");
                return false;
            }
            else
            {
                throw new Exception("The ore dictionary doesn't contain they key for resource: " + name);
            }
        }
        /// <summary>
        /// Spawns an orevein at the tile position at the same location as the player.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="TilePosition"></param>
        /// <returns></returns>
        public bool spawnOreVein(string name, Vector2 TilePosition)
        {
            return this.spawnOreVein(name, Game1.player.currentLocation, TilePosition);
        }

        /// <summary>
        /// Spawns ore in the mine depending on a lot of given variables such as floor level and spawn chance.
        /// </summary>
        public void spawnOreInMine()
        {
            int floorLevel = LocationUtilities.CurrentMineLevel();
            if (this.hasVisitedFloor(floorLevel))
            {
                //Already has spawned ores for this visit.
                return;
            }
            else
            {
                this.visitedFloors.Add(floorLevel);
            }
            List<OreVeinObj> spawnableOreVeins = new List<OreVeinObj>();
            //Get a list of all of the ores that can spawn on this mine level.
            foreach (KeyValuePair<string, OreVeinObj> pair in this.oreVeins)
            {
                if (pair.Value.resourceInfo.canSpawnAtLocation() && (pair.Value.resourceInfo as OreResourceInformation).canSpawnOnCurrentMineLevel())
                {
                    spawnableOreVeins.Add(pair.Value);
                }
            }

            foreach (OreVeinObj ore in spawnableOreVeins)
            {
                if (ore.resourceInfo.shouldSpawn())
                {
                    int amount = ore.resourceInfo.getNumberOfNodesToSpawn();
                    List<Vector2> openTiles = LocationUtilities.GetOpenObjectTiles(Game1.player.currentLocation, (OreVeinObj)ore.getOne());
                    amount = Math.Min(amount, openTiles.Count); //Only spawn for as many open tiles or the amount of nodes to spawn.
                    for (int i = 0; i < amount; i++)
                    {
                        int position = Game1.random.Next(openTiles.Count);
                        bool didSpawn = this.spawnOreVein(ore.info.id, openTiles[position]);
                        if (didSpawn == false)
                        {
                            i--; //If the tile didn't spawn due to some odd reason ensure that the amount is spawned.
                            openTiles.Remove(openTiles[position]);
                        }
                        else
                        {
                            openTiles.Remove(openTiles[position]); //Remove that tile from the list of open tiles.
                        }
                    }
                }
                else
                {
                    //Ore doesn't meet spawn chance.
                }
                //ModCore.log("Spawned :" + amount + " pancake test ores!");
            }

        }

        /// <summary>
        /// Checks to see if the player has visited the given floor.
        /// </summary>
        /// <param name="Floor"></param>
        /// <returns></returns>
        public bool hasVisitedFloor(int Floor)
        {
            return this.visitedFloors.Contains(Floor);
        }

        /// <summary>
        /// Source: SDV. 
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <returns></returns>
        private bool isTileOpenForQuarryStone(int tileX, int tileY)
        {
            GameLocation loc = Game1.getLocationFromName("Mountain");
            if (loc.doesTileHaveProperty(tileX, tileY, "Diggable", "Back") != null)
                return loc.isTileLocationTotallyClearAndPlaceable(new Vector2((float)tileX, (float)tileY));
            return false;
        }

        /// <summary>
        /// Update the quarry every day with new ores to spawn.
        /// </summary>
        private void quarryDayUpdate()
        {
            List<OreVeinObj> spawnableOreVeins = new List<OreVeinObj>();
            //Get a list of all of the ores that can spawn on this mine level.
            foreach (KeyValuePair<string, OreVeinObj> pair in this.oreVeins)
            {
                if ((pair.Value.resourceInfo as OreResourceInformation).spawnsInQuarry)
                {
                    spawnableOreVeins.Add(pair.Value);
                    //ModCore.log("Found an ore that spawns in the quarry");
                }
            }
            foreach (OreVeinObj ore in spawnableOreVeins)
            {
                if ((ore.resourceInfo as OreResourceInformation).shouldSpawnInQuarry())
                {
                    int amount = (ore.resourceInfo as OreResourceInformation).getNumberOfNodesToSpawnQuarry();
                    List<Vector2> openTiles = this.getOpenQuarryTiles(ore);
                    amount = Math.Min(amount, openTiles.Count); //Only spawn for as many open tiles or the amount of nodes to spawn.
                    for (int i = 0; i < amount; i++)
                    {
                        int position = Game1.random.Next(openTiles.Count);
                        bool didSpawn = this.spawnOreVein(ore.info.id, Game1.getLocationFromName("Mountain"), openTiles[position]);
                        if (didSpawn == false)
                        {
                            i--; //If the tile didn't spawn due to some odd reason ensure that the amount is spawned.
                            openTiles.Remove(openTiles[position]);
                            //amount = Math.Min(amount, openTiles.Count); //Only spawn for as many open tiles or the amount of nodes to spawn.
                        }
                        else
                        {
                            //ModCore.log("Spawned ore in the quarry!");
                            openTiles.Remove(openTiles[position]); //Remove that tile from the list of open tiles.
                        }
                    }
                }
                else
                {
                    //Ore doesn't meet spawn chance.
                }
            }

        }

        /// <summary>
        /// Spawns ore in the mountain farm every day.
        /// </summary>
        public void mountainFarmDayUpdate()
        {
            if (LocationUtilities.Farm_IsFarmHiltopFarm() == false)
            {
                //ModCore.log("Farm is not hiltop farm!");
                return;
            }
            GameLocation farm = Game1.getFarm();

            List<OreVeinObj> spawnableOreVeins = new List<OreVeinObj>();
            //Get a list of all of the ores that can spawn on this mine level.
            foreach (KeyValuePair<string, OreVeinObj> pair in this.oreVeins)
            {
                if ((pair.Value.resourceInfo as OreResourceInformation).spawnsOnFarm)
                {
                    spawnableOreVeins.Add(pair.Value);
                    //ModCore.log("Found an ore that spawns on the farm!");
                }
            }
            foreach (OreVeinObj ore in spawnableOreVeins)
            {
                if ((ore.resourceInfo as OreResourceInformation).shouldSpawnOnFarm())
                {
                    int amount = (ore.resourceInfo as OreResourceInformation).getNumberOfNodesToSpawnFarm();
                    List<Vector2> openTiles = this.getFarmQuarryOpenTiles(ore);
                    if (openTiles.Count == 0)
                    {
                        //ModCore.log("No open farm tiles!");
                    }
                    amount = Math.Min(amount, openTiles.Count); //Only spawn for as many open tiles or the amount of nodes to spawn.
                    for (int i = 0; i < amount; i++)
                    {
                        int position = Game1.random.Next(openTiles.Count);
                        bool didSpawn = this.spawnOreVein(ore.info.id, farm, openTiles[position]);
                        if (didSpawn == false)
                        {
                            i--; //If the tile didn't spawn due to some odd reason ensure that the amount is spawned.
                            openTiles.Remove(openTiles[position]);
                            //amount = Math.Min(amount, openTiles.Count); //Only spawn for as many open tiles or the amount of nodes to spawn.
                            //ModCore.log("Did not spawn ore in the farm quarry!");
                        }
                        else
                        {
                            //ModCore.log("Spawned ore in the farm quarry!");
                            openTiles.Remove(openTiles[position]); //Remove that tile from the list of open tiles.
                        }
                    }
                }
                else
                {
                    //Ore doesn't meet spawn chance.
                }
            }

        }

        /// <summary>
        /// Gets a list of all of the open quarry tiles.
        /// </summary>
        /// <returns></returns>
        private List<Vector2> getOpenQuarryTiles(MultiTiledObject obj)
        {
            List<Vector2> tiles = new List<Vector2>();
            Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(106, 13, 21, 21);
            for (int i = r.X; i <= r.X + r.Width; i++)
            {
                for (int j = r.Y; j <= r.Y + r.Height; j++)
                {
                    if (this.isTileOpenForQuarryStone(i, j) && this.canResourceBeSpawnedHere(obj, Game1.getLocationFromName("Mountain"), new Vector2(i, j)))
                    {
                        tiles.Add(new Vector2(i, j));
                    }
                }
            }
            if (tiles.Count == 0)
            {
                //ModCore.log("Quarry is full! Can't spawn more resources!");
            }
            return tiles;
        }

        /// <summary>
        /// Gets all of the open tiles in the farm quarry.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private List<Vector2> getFarmQuarryOpenTiles(MultiTiledObject obj)
        {
            List<Vector2> tiles = new List<Vector2>();
            Microsoft.Xna.Framework.Rectangle r = new Microsoft.Xna.Framework.Rectangle(5, 37, 22, 8);
            GameLocation farm = Game1.getFarm();
            for (int i = r.X; i <= r.X + r.Width; i++)
            {
                for (int j = r.Y; j <= r.Y + r.Height; j++)
                {
                    Vector2 pos = new Vector2(i, j);
                    if (farm.doesTileHavePropertyNoNull((int)pos.X, (int)pos.Y, "Type", "Back").Equals("Dirt") && this.canResourceBeSpawnedHere(obj, farm, new Vector2(i, j)))
                    {
                        tiles.Add(pos);
                    }
                }
            }
            if (tiles.Count == 0)
            {
                //ModCore.log("Quarry is full! Can't spawn more resources!");
            }
            return tiles;
        }

        #endregion


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        //          SMAPI Events       //
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

        #region
        /// <summary>
        /// What happens when the player warps maps.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="playerWarped"></param>
        public void OnPlayerLocationChanged(object o, EventArgs playerWarped)
        {
            this.spawnOreInMine();
            if (LocationUtilities.IsPlayerInMine() == false && LocationUtilities.IsPlayerInSkullCave() == false && LocationUtilities.IsPlayerInMineEnterance() == false)
            {
                this.visitedFloors.Clear();
            }
        }

        /// <summary>
        /// Triggers at the start of every new day to populate the world full of ores.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="NewDay"></param>
        public void DailyResourceSpawn(object o, EventArgs NewDay)
        {
            this.mountainFarmDayUpdate();
            this.quarryDayUpdate();
        }
        #endregion

    }
}
