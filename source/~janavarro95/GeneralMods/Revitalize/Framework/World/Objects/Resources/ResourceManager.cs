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
using StardewValley;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Items;
using Omegasis.Revitalize.Framework.World.Objects;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.StardustCore.UIUtilities;
using Omegasis.StardustCore.Animations;
using Omegasis.Revitalize.Framework.Managers;
using Omegasis.Revitalize.Framework.Constants.ItemCategoryInformation;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.World.Objects.Items.Resources;
using Omegasis.Revitalize.Framework.Constants.Ids.Resources.EarthenResources;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.Utilities.Ranges;

namespace Omegasis.Revitalize.Framework.World.Objects.Resources
{
    public class ResourceManager
    {

        private string oreResourceDataPath = Path.Combine("Data", "Objects", "Resources", "Ore");

        /// <summary>
        /// A static reference to the resource manager for quicker access.
        /// </summary>
        public static ResourceManager self;

        /// <summary>
        /// A list of all of the ores held by the resource manager.
        /// </summary>
        //public Dictionary<string, OreVein> oreVeins;
        public Dictionary<string, OreResourceInformation> oreResourceInformationTable;

        public OreResourceSpawner oreResourceSpawner;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourceManager()
        {
            self = this;
            this.oreResourceInformationTable = new Dictionary<string, OreResourceInformation>();
            this.oreResourceSpawner = new OreResourceSpawner();
        }


        //Loads in the items for the resource manager.
        public void loadInItems()
        {
            this.loadInOreItems();
            this.loadInResourceItems();
            this.loadOreVeins();
        }

        /// <summary>
        /// Loads in all of the ore veins for the game.
        /// </summary>
        protected void loadOreVeins()
        {
            foreach (var v in this.createOreVeins())
                RevitalizeModCore.ModContentManager.objectManager.addItem(v.basicItemInformation.id, v);
        }


        /// <summary>
        /// Serializes an example ore to eb
        /// </summary>
        protected List<OreVein> createOreVeins()
        {
            //Tin
            List<OreVein> oreVeins = new List<OreVein>();
            OreVein tinOre_0_0 = new OreVein(new BasicItemInformation("Tin Ore Vein", ResourceObjectIds.TinOreVein, "A ore vein that is full of tin.", "Omegasis.Revitalize.Ore", Color.Black, -300, -300, 0, false, 350, true, true, TextureManagers.Resources_Ore.createAnimationManager("Tin", new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null),
                new OreResourceInformation(new ItemReference(Ores.TinOre), true, true, true, false, new List<IntRange>()
            {
                new IntRange(1,20)
            }, new List<IntRange>(),

            1, 3, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, 0.80d, 0.20d, 0.25d, 1d, 1d, 1, 1, 1, 1), new List<ResourceInformation>(), 4);

            /*
            //Aluminum
            OreVein bauxiteOre_0_0 = new OreVein(new BasicItemInformation("Bauxite Ore Vein", ResourceObjectIds.BauxiteOreVein, "A ore vein that is full of bauxite ore.", "Omegasis.Revitalize.Ore", Color.Black, -300, -300, 0, false, 350, true, true, TextureManagers.Resources_Ore.createAnimationManager("Bauxite", new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null),
                new OreResourceInformation(new ObjectManagerItemReference(Ores.BauxiteOre), true, true, true, false, new List<IntRange>()
            {
                new IntRange(20,50)
            }, new List<IntRange>(), 1, 3, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, .70d, 0.16d, 0.20d, 1d, 1d, 0, 0, 0, 0), new List<ResourceInformation>(), 5);
            */

            //Silver
            OreVein silverOre_0_0 = new OreVein(new BasicItemInformation("Silver Ore Vein", ResourceObjectIds.SilverOreVein, "A ore vein that is full of silver ore.", "Omegasis.Revitalize.Ore", Color.Black, -300, -300, 0, false, 350, true, true, TextureManagers.Resources_Ore.createAnimationManager("Silver", new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null),
                new OreResourceInformation(new ItemReference(Ores.SilverOre), true, true, true, false, new List<IntRange>()
            {
                new IntRange(60,100)
            }, new List<IntRange>(), 1, 3, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, .50d, 0.10d, 0.14d, 1d, 1d, 0, 0, 0, 0), new List<ResourceInformation>(), 6);

            //Lead
            OreVein leadOre_0_0 = new OreVein(new BasicItemInformation("Lead Ore Vein", ResourceObjectIds.LeadOreVein, "A ore vein that is full of lead ore.", "Omegasis.Revitalize.Ore", Color.Black, -300, -300, 0, false, 350, true, true, TextureManagers.Resources_Ore.createAnimationManager("Lead", new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null),
                new OreResourceInformation(new ItemReference(Ores.LeadOre), true, true, true, false, new List<IntRange>()
            {
                new IntRange(60,70),
                new IntRange(90,120)
            }, new List<IntRange>(), 1, 3, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, .60d, 0.13d, 0.17d, 1d, 1d, 0, 0, 0, 0), new List<ResourceInformation>(), 7);

            /*
            //Titanium
            OreVein titaniumOre_0_0 = new OreVein(new BasicItemInformation("Titanium Ore Vein", ResourceObjectIds.TitaniumOreVein, "A ore vein that is full of lead ore.", "Omegasis.Revitalize.Ore", Color.Black, -300, -300, 0, false, 350, true, true, TextureManagers.Resources_Ore.createAnimationManager("Titanium", new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null),
                new OreResourceInformation(new ObjectManagerItemReference(Ores.TitaniumOre), true, true, true, false, new List<IntRange>()
            {
                new IntRange(60,70),
                new IntRange(90,120)
            }, new List<IntRange>(), 1, 3, 1, 10, new IntRange(1, 3), new IntRange(1, 3), new IntRange(0, 0), new List<IntRange>()
            {
                new IntRange(0,0)
            }, new List<IntRange>()
            {
                new IntRange(0,9999)
            }, .40d, 0.05d, 0.10d, 1d, 1d, 0, 0, 0, 0), new List<ResourceInformation>(), 8);
            */

            //Prismatic nugget ore
            OreVein prismaticOre_0_0 = new OreVein(new BasicItemInformation("Prismatic Ore Vein", ResourceObjectIds.PrismaticOreVein, "A ore vein that is full of prismatic ore.", "Omegasis.Revitalize.Ore", Color.Black, -300, -300, 0, false, 350, true, true, TextureManagers.Resources_Ore.createAnimationManager("Prismatic", new Animation(0, 0, 16, 16)), Color.White, false, new Vector2(1, 1), Vector2.Zero, null, null),
                new OreResourceInformation(new ItemReference(Gems.PrismaticNugget), true, true, true, false, new List<IntRange>()
            {
                new IntRange(110,120)
            }, new List<IntRange>(), 1, 3, 1, 1, new IntRange(1, 1), new IntRange(1, 1), new IntRange(1, 5), new List<IntRange>()
            {
                new IntRange(1,9999)
            }, new List<IntRange>()
            {
            }, .05d, 0.01d, 0.01d, 0.10, 1d, 1, 1, 1, 1), new List<ResourceInformation>(), 10);

            //Early game machines
            oreVeins.Add(tinOre_0_0);
            //oreVeins.Add(bauxiteOre_0_0);

            //Mid game+ electrical machines + weapons to fight undead monsters?
            oreVeins.Add(silverOre_0_0);
            //Nuclear tier machines
            oreVeins.Add(leadOre_0_0);
            //oreVeins.Add(titaniumOre_0_0);

            //For fun.
            oreVeins.Add(prismaticOre_0_0);
            return oreVeins;
        }

        /// <summary>
        /// Loads in all of the ore items into the game.
        /// </summary>
        private void loadInOreItems()
        {
            Ore tinOre = new Ore(new BasicItemInformation("Tin Ore", Ores.TinOre, "Tin ore that can be smelted into tin ingots for further use.", CategoryNames.Ore, Color.Silver, -300, -300, 0, false, 7, false, false, TextureManagers.createOreResourceAnimationManager("TinOre"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ores.TinOre, tinOre);

            Ore bauxiteOre = new Ore(new BasicItemInformation("Bauxite Ore", Ores.BauxiteOre, "Bauxite ore that can be smelted into aluminum ingots for further use.", CategoryNames.Ore, Color.Silver, -300, -300, 0, false, 11, false, false, TextureManagers.createOreResourceAnimationManager("BauxiteOre"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ores.BauxiteOre, bauxiteOre);

            Ore leadOre = new Ore(new BasicItemInformation("Lead Ore", Ores.LeadOre, "Lead ore that can be smelted into lead ingots for further use.", CategoryNames.Ore, Color.Silver, -300, -300, 0, false, 15, false, false, TextureManagers.createOreResourceAnimationManager("LeadOre"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ores.LeadOre, leadOre);

            Ore silverOre = new Ore(new BasicItemInformation("Silver Ore", Ores.SilverOre, "Silver ore that can be smelted into silver ingots for further use.", CategoryNames.Ore, Color.Silver, -300, -300, 0, false, 20, false, false, TextureManagers.createOreResourceAnimationManager("SilverOre"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ores.SilverOre, silverOre);

            Ore titaniumOre = new Ore(new BasicItemInformation("Titanium Ore", Ores.TitaniumOre, "Titanium ore that can be smelted into titanium ingots for further use.", CategoryNames.Ore, Color.Silver, -300, -300, 0, false, 35, false, false, TextureManagers.createOreResourceAnimationManager("TitaniumOre"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ores.TitaniumOre, titaniumOre);

            Ore prismaticOre = new Ore(new BasicItemInformation("Prismatic Nugget", Gems.PrismaticNugget, "Rare prismatic ore that can be smelted into a prismatic shard when seven are gathered.", CategoryNames.Ore, Color.Silver, -300, -300, 0, false, 200, false, false, TextureManagers.createOreResourceAnimationManager("PrismaticNugget"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Gems.PrismaticNugget, prismaticOre);

            CustomObject tinIngot = new CustomObject(new BasicItemInformation("Tin Ingot", Ingots.TinIngot, "A tin ingot that can be used for crafting purposes.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 75, false, false, TextureManagers.createOreResourceAnimationManager("TinIngot"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ingots.TinIngot, tinIngot);

            CustomObject aluminumIngot = new CustomObject(new BasicItemInformation("Aluminum Ingot", Ingots.AluminumIngot, "An aluminum ingot that can be used for crafting purposes.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 120, false, false, TextureManagers.createOreResourceAnimationManager("AluminumIngot"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ingots.AluminumIngot, aluminumIngot);

            CustomObject leadIngot = new CustomObject(new BasicItemInformation("Lead Ingot", Ingots.LeadIngot, "A lead ingot that can be used for crafting purposes.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 165, false, false, TextureManagers.createOreResourceAnimationManager("LeadIngot"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ingots.LeadIngot, leadIngot);

            CustomObject silverIngot = new CustomObject(new BasicItemInformation("Silver Ingot", Ingots.SilverIngot, "A silver ingot that can be used for crafting purposes.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 220, false, false, TextureManagers.createOreResourceAnimationManager("SilverIngot"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ingots.SilverIngot, silverIngot);

            CustomObject titaniumIngot = new CustomObject(new BasicItemInformation("Titanium Ingot", Ingots.TitaniumIngot, "A titanium ingot that can be used for crafting purposes.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 325, false, false, TextureManagers.createOreResourceAnimationManager("TitaniumIngot"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ingots.TitaniumIngot, titaniumIngot);

            CustomObject brassIngot = new CustomObject(new BasicItemInformation("Brass Ingot", Ingots.BrassIngot, "A brass alloy ingot made from copper and aluminum. It can be used for crafting purposes.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 195, false, false, TextureManagers.createOreResourceAnimationManager("BrassIngot"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ingots.BrassIngot, brassIngot);

            CustomObject bronzeIngot = new CustomObject(new BasicItemInformation("Bronze Ingot", Ingots.BronzeIngot, "A bronze alloy ingot made from copper and tin. It can be used for crafting purposes.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 150, false, false, TextureManagers.createOreResourceAnimationManager("BronzeIngot"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ingots.BronzeIngot, bronzeIngot);

            CustomObject electrumIngot = new CustomObject(new BasicItemInformation("Electrum Ingot", Ingots.ElectrumIngot, "A electrum alloy ingot made from gold and silver. It can be used for crafting purposes for things that use electricity.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 500, false, false, TextureManagers.createOreResourceAnimationManager("ElectrumIngot"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ingots.ElectrumIngot, electrumIngot);

            CustomObject steelIngot = new CustomObject(new BasicItemInformation("Steel Ingot", Ingots.SteelIngot, "A steel ingot that was made by processing iron again with more coal. It can be used for crafting purposes especially for making new machines.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 180, false, false, TextureManagers.createOreResourceAnimationManager("SteelIngot"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            RevitalizeModCore.ModContentManager.objectManager.addItem(Ingots.SteelIngot, steelIngot);

            CustomObject bauxiteSand = new CustomObject(new BasicItemInformation("Bauxite Sand", OreSands.BauxiteSand, "Bauxite ore which has been crushed into sand. Smelt it to get aluminum ingots.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 11, false, false, TextureManagers.createOreResourceAnimationManager("BauxiteSand"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            CustomObject copperSand = new CustomObject(new BasicItemInformation("Copper Sand", OreSands.CopperSand, "Copper ore which has been crushed into sand. Smelt it to get copper bars.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 5, false, false, TextureManagers.createOreResourceAnimationManager("CopperSand"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            CustomObject goldSand = new CustomObject(new BasicItemInformation("Gold Sand", OreSands.GoldSand, "Gold ore which has been crushed into sand. Smelt it to get gold bars.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 25, false, false, TextureManagers.createOreResourceAnimationManager("GoldSand"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            CustomObject ironSand = new CustomObject(new BasicItemInformation("Iron Sand", OreSands.IronSand, "Iron ore which has been crushed into sand. Smelt it to get iron bars.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 10, false, false, TextureManagers.createOreResourceAnimationManager("IronSand"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            CustomObject iridiumSand = new CustomObject(new BasicItemInformation("Iridium Sand", OreSands.IridiumSand, "Iridium ore which has been crushed into sand. Smelt it to get iridium bars.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 100, false, false, TextureManagers.createOreResourceAnimationManager("IridiumSand"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            CustomObject leadSand = new CustomObject(new BasicItemInformation("Lead Sand", OreSands.LeadSand, "Lead ore which has been crushed into sand. Smelt it to get lead ingots.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 15, false, false, TextureManagers.createOreResourceAnimationManager("LeadSand"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            CustomObject silverSand = new CustomObject(new BasicItemInformation("Silver Sand", OreSands.SilverSand, "Silver ore which has been crushed into sand. Smelt it to get silver ingots.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 20, false, false, TextureManagers.createOreResourceAnimationManager("SilverSand"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            CustomObject tinSand = new CustomObject(new BasicItemInformation("Tin Sand", OreSands.TinSand, "Tin ore which has been crushed into sand. Smelt it to get tin ingots.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 7, false, false, TextureManagers.createOreResourceAnimationManager("TinSand"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);
            CustomObject titaniumSand = new CustomObject(new BasicItemInformation("Titanium Sand", OreSands.TitaniumSand, "Titanium ore which has been crushed into sand. Smelt it to get titanium bars.", CategoryNames.Resource, Color.Silver, -300, -300, 0, false, 35, false, false, TextureManagers.createOreResourceAnimationManager("TitaniumSand"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null), 1);

            RevitalizeModCore.ModContentManager.objectManager.addItem(OreSands.BauxiteSand, bauxiteSand);
            RevitalizeModCore.ModContentManager.objectManager.addItem(OreSands.CopperSand, copperSand);
            RevitalizeModCore.ModContentManager.objectManager.addItem(OreSands.GoldSand, goldSand);
            RevitalizeModCore.ModContentManager.objectManager.addItem(OreSands.IronSand, ironSand);
            RevitalizeModCore.ModContentManager.objectManager.addItem(OreSands.IridiumSand, iridiumSand);
            RevitalizeModCore.ModContentManager.objectManager.addItem(OreSands.LeadSand, leadSand);
            RevitalizeModCore.ModContentManager.objectManager.addItem(OreSands.SilverSand, silverSand);
            RevitalizeModCore.ModContentManager.objectManager.addItem(OreSands.TinSand, tinSand);
            RevitalizeModCore.ModContentManager.objectManager.addItem(OreSands.TitaniumSand, titaniumSand);
        }



        private void loadInResourceItems()
        {
            CustomObject sand = new CustomObject(new BasicItemInformation("Sand", MiscEarthenResources.Sand, "Sand which is made from tiny rocks and can be used for smelting. Also unfun to have inside of swimwear.", CategoryNames.Resource, Color.Brown, -300, -300, 0, false, 2, false, false, TextureManagers.createMiscResourceAnimationManager("Sand"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null));
            RevitalizeModCore.ModContentManager.objectManager.addItem(MiscEarthenResources.Sand, sand);

            CustomObject glass_normal = new CustomObject(new BasicItemInformation("Glass", MiscEarthenResources.Glass, "Glass smelted from sand. Used in decorations and glass objects.", CategoryNames.Resource, Color.Brown, -300, -300, 0, false, 20, false, false, TextureManagers.createMiscResourceAnimationManager("Glass"), Color.White, true, new Vector2(1, 1), Vector2.Zero, null, null));
            RevitalizeModCore.ModContentManager.objectManager.addItem(MiscEarthenResources.Glass, glass_normal);
        }




        //~~~~~~~~~~~~~~~~~~~~~~~//
        //  World Ore Spawn Code //
        //~~~~~~~~~~~~~~~~~~~~~~~//

        #region




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
            /*
            this.oreResourceSpawner.spawnOreInMine();
            if (GameLocationUtilities.IsPlayerInMine() == false && GameLocationUtilities.IsPlayerInSkullCave() == false && GameLocationUtilities.IsPlayerInTheEnteranceToTheMines() == false)
                this.oreResourceSpawner.visitedFloors.Clear();

            */
        }

        /// <summary>
        /// Triggers at the start of every new day to populate the world full of ores.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="NewDay"></param>
        public void DailyResourceSpawn(object o, EventArgs NewDay)
        {
            this.oreResourceSpawner.mountainFarmDayUpdate();
            this.oreResourceSpawner.quarryDayUpdate();
        }
        #endregion

    }
}
