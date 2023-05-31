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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Data;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.Utilities.Extensions;
using Omegasis.Revitalize.Framework.Utilities.JsonContentLoading;
using Omegasis.Revitalize.Framework.Utilities.Ranges;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Items;
using StardewValley;
using StardewValley.Locations;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.ResourceGeneration
{
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Machines.ResourceGeneration.MiningDrill")]
    public class MiningDrill : PoweredMachine
    {
        public const string MineEntranceLootTableSuffix = "_Mine";
        public const string MineUndergroundLootTableSuffix = "_UndergroundMines";
        public const string MineUnderground_Floors_0_39Suffix = "_Mines0-39";
        public const string MineUnderground_Floors_50Plus = "_Mines50+";
        public const string MineUnderground_Floors_40_80Suffix = "_Mines40-80";
        public const string MineUnderground_Floors_80Plus = "_Mines80+";
        public const string ReachedBottomOfHardMinesSuffix = "_ReachedBottomOfHardMines";
        public const string SkullCaveLootTableSuffix = "_SkullCave";
        public const string VolcanoDungeonLootTableSuffix = "_VolcanoDungeon";
        public const string CalderaLootTableSuffix = "_Caldera";

        public MiningDrill()
        {

        }

        public MiningDrill(BasicItemInformation Info, PoweredMachineTier machineTier) : this(Info, Vector2.Zero, machineTier)
        {

        }

        public MiningDrill(BasicItemInformation Info, Vector2 TilePosition, PoweredMachineTier machineTier) : base(Info, TilePosition, machineTier)
        {
        }

        /// <summary>
        /// Attempts to run the mining drill again after the player get's the item fronm it and the queue is freed.
        /// </summary>
        /// <returns></returns>
        public override bool tryToAddHeldItemsToFarmersInventory()
        {
            bool added=base.tryToAddHeldItemsToFarmersInventory();
            if (added && this.heldObject.Value==null)
            {
                this.tryToRunMiningDrill();
            }
            return added;
        }

        public virtual void tryToRunMiningDrill()
        {
            if (this.hasFuel())
            {
                this.processInput(null,null,false);
                this.updateAnimation();
            }
        }

        public override CraftingResult processInput(IList<Item> dropInItem, Farmer who, bool ShowRedMessage = true)
        {
            //Since we don't use a recipe book here, we need to return true so that the logic properly updates.
            //return new CraftingResult(true);


            if (string.IsNullOrEmpty(this.getCraftingRecipeBookId()) || this.isWorking() || this.finishedProduction())
            {
                return new CraftingResult(false);
            }

            List<KeyValuePair<IList<Item>, ProcessingRecipe>> validRecipes = this.getListOfValidRecipes(dropInItem, who, ShowRedMessage);

            if (validRecipes.Count > 0)
            {
                int randElement = Game1.random.Next(validRecipes.Count);
                return this.onSuccessfulRecipeFound(validRecipes.ElementAt(randElement).Key, validRecipes.ElementAt(randElement).Value, who);
            }

            return new CraftingResult(false);
        }

        /// <summary>
        /// Try to increase the fuel charges for the mining drill only if the active object the player is holding is a valid fuel item.
        /// </summary>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool tryToIncreaseFuelCharges(Farmer who, bool ShowRedMessage = true)
        {
            if (this.getListOfValidRecipes(null, who, ShowRedMessage).Count == 0)
            {
                if (ShowRedMessage) Game1.showRedMessage(JsonContentPackUtilities.LoadErrorString(this.getErrorStringFile(), "CantRunHere", this.DisplayName));
                return false; //Don't consume fuel when no valid recipes can be set.
            }

            bool hasFuel = this.useFuelItemToIncreaseCharges(who, true, ShowRedMessage);
            return hasFuel;
        }

        public override void playDropInSound()
        {
            SoundUtilities.PlaySound(Enums.StardewSound.Ship);
        }

        public override int getElectricFuelChargeIncreaseAmount()
        {
            return 3;
        }

        public override int getNuclearFuelChargeIncreaseAmount()
        {
            return this.getElectricFuelChargeIncreaseAmount() * 10;
        }

        public override List<KeyValuePair<IList<Item>, ProcessingRecipe>> getListOfValidRecipes(IList<Item> inputItems, Farmer who, bool ShowRedMessage = true)
        {
            GameLocation objectLocation = this.getCurrentLocation();
            List<KeyValuePair<IList<Item>, ProcessingRecipe>> validRecipes = new();
            if (objectLocation == null) return validRecipes;


            if (GameLocationUtilities.IsLocationTheEntranceToTheMines(objectLocation))
            {
                foreach (ProcessingRecipe recipe in RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId() + MineEntranceLootTableSuffix))
                {
                    validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(new List<Item>(), recipe));
                }
            }

            //Load different recipes depending on the different conditions.
            if (GameLocationUtilities.IsLocationInTheMines(objectLocation))
            {


                int floorLevel = GameLocationUtilities.CurrentMineLevel(objectLocation);


                foreach (ProcessingRecipe recipe in RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId() + MineUndergroundLootTableSuffix))
                {
                    validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(new List<Item>(), recipe));
                }

                if (floorLevel <= 39)
                {
                    foreach (ProcessingRecipe recipe in RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId() + MineUnderground_Floors_0_39Suffix))
                    {
                        validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(new List<Item>(), recipe));
                    }

                }
                if (floorLevel >= 50)
                {
                    foreach (ProcessingRecipe recipe in RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId() + MineUnderground_Floors_50Plus))
                    {
                        validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(new List<Item>(), recipe));
                    }
                }

                if (floorLevel >= 40 && floorLevel <= 80)
                {
                    foreach (ProcessingRecipe recipe in RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId() + MineUnderground_Floors_40_80Suffix))
                    {
                        validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(new List<Item>(), recipe));
                    }
                }
                if (floorLevel >= 80)
                {
                    foreach (ProcessingRecipe recipe in RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId() + MineUnderground_Floors_80Plus))
                    {
                        validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(new List<Item>(), recipe));
                    }
                }

                if (Game1.player.hasOrWillReceiveMail("reachedBottomOfHardMines"))
                {
                    foreach (ProcessingRecipe recipe in RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId() + ReachedBottomOfHardMinesSuffix))
                    {
                        validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(new List<Item>(), recipe));
                    }
                }
            }
            else if (GameLocationUtilities.IsLocationInSkullCaves(objectLocation))
            {
                //SkullCave
                foreach (ProcessingRecipe recipe in RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId() + "_" + objectLocation.Name))
                {
                    validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(new List<Item>(), recipe));
                }

                if (Game1.player.hasOrWillReceiveMail("reachedBottomOfHardMines"))
                {
                    foreach (ProcessingRecipe recipe in RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId() + ReachedBottomOfHardMinesSuffix))
                    {
                        validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(new List<Item>(), recipe));
                    }
                }
            }


            else
            {

                //Add processing recipes sepcific to game locations.
                //Need Mine (Mines entrance), SkullCave, Caldera, and VolcanoDungeon Loot pools with this here.
                foreach (ProcessingRecipe recipe in RevitalizeModCore.ModContentManager.objectProcessingRecipesManager.getProcessingRecipesForObject(this.getCraftingRecipeBookId() + "_" + objectLocation.Name))
                {
                    validRecipes.Add(new KeyValuePair<IList<Item>, ProcessingRecipe>(new List<Item>(), recipe));
                }
            }


            return validRecipes;
        }

        public override Item getOne()
        {
            return new MiningDrill(this.basicItemInformation.Copy(), this.machineTier.Value);
        }

        #region
        public static void GenerateOutputJsonFiles()
        {

            GenerateBurnerMiningDrillOutputJsonFiles("BurnerMiningDrill", 50, .25f, .1f, 10, 1, 10);
            GenerateElectricMiningDrillOutputJsonFiles("ElectricMiningDrill", 75, .50f, .25f, 25, 5, 20);
            GenerateNuclearMiningDrillOutputJsonFiles("NuclearMiningDrill", 100, .75f, .50f, 50, 15, 35);
            GenerateMagicalMiningDrillOutputJsonFiles("MagicalMiningDrill", 100, 1f, .75f, 80, 25, 50);
            GenerateGalaxyMiningDrillOutputJsonFiles("GalaxyMiningDrill", 100, 1.25f, 1f, 100, 50, 75); //If for some reason I ever add in Galaxy Mining Drills, 
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetMiningDrillOutputs_Mines()
        {
            string baseId = "Omegasis.Revitalize.Machines.MiningDrillOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> geodeOutputs = new()
            {
                GenerateLootTableEntry(baseId, Enums.SDVObject.Geode,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Stone,GenerateStackSizeOutcomesForNormalResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Clay,GenerateStackSizeOutcomesForNormalResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.CopperOre,GenerateStackSizeOutcomesForNormalResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Coal,GenerateStackSizeOutcomesForNormalResources()),
            };

            return geodeOutputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetMiningDrillOutputs_InsideMines()
        {
            string baseId = "Omegasis.Revitalize.Machines.MiningDrillOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> geodeOutputs = new()
            {
                GenerateLootTableEntry(baseId, Enums.SDVObject.Quartz,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Stone,GenerateStackSizeOutcomesForNormalResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Coal,GenerateStackSizeOutcomesForNormalResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.OmniGeode,1),

                GenerateLootTableEntry(baseId, Enums.SDVObject.Amethyst,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Aquamarine,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Emerald,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Jade,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Ruby,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Topaz,1),
            };

            return geodeOutputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetMiningDrillOutputs_Mines_Floors_0_39()
        {
            string baseId = "Omegasis.Revitalize.Machines.MiningDrillOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> outputs = new()
            {
                GenerateLootTableEntry(baseId, Enums.SDVObject.Geode,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.EarthCrystal,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.CopperOre,GenerateStackSizeOutcomesForNormalResources()),
            };

            return outputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetMiningDrillOutputs_Mines_Floors_50Plus()
        {
            string baseId = "Omegasis.Revitalize.Machines.MiningDrillOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> outputs = new()
            {
                GenerateLootTableEntry(baseId, Enums.SDVObject.Diamond,1),
            };

            return outputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetMiningDrillOutputs_Mines_Floors_40_80()
        {
            string baseId = "Omegasis.Revitalize.Machines.MiningDrillOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> outputs = new()
            {
                GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenTear,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.IronOre,GenerateStackSizeOutcomesForNormalResources()),
            };

            return outputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetMiningDrillOutputs_Mines_Floors_80Plus()
        {
            string baseId = "Omegasis.Revitalize.Machines.MiningDrillOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> outputs = new()
            {
                GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.FireQuartz,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.GoldOre,GenerateStackSizeOutcomesForNormalResources()),
            };

            return outputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetMiningDrillOutputs_Mines_BottomOfHardMines()
        {
            string baseId = "Omegasis.Revitalize.Machines.MiningDrillOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> outputs = new()
            {
                GenerateLootTableEntry(baseId, Enums.SDVObject.RadioactiveOre,GenerateStackSizeOutcomesForUltraRareResources()),
            };

            return outputs;
        }
        public static List<KeyValuePair<string, ProcessingRecipe>> GetMiningDrillOutputs_SkullCaves()
        {
            string baseId = "Omegasis.Revitalize.Machines.MiningDrillOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> outputs = new()
            {
                GenerateLootTableEntry(baseId, Enums.SDVObject.Stone,GenerateStackSizeOutcomesForNormalResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Coal,GenerateStackSizeOutcomesForNormalResources()),

                GenerateLootTableEntry(baseId, Enums.SDVObject.Amethyst,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Aquamarine,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Emerald,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Jade,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Ruby,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Topaz,1),
                GenerateLootTableEntry(baseId, Enums.SDVObject.Diamond,1),

                GenerateLootTableEntry(baseId, Enums.SDVObject.CopperOre,GenerateStackSizeOutcomesForNormalResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.IronOre,GenerateStackSizeOutcomesForNormalResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.GoldOre,GenerateStackSizeOutcomesForNormalResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.IridiumOre,GenerateStackSizeOutcomesForRareResources()),

                GenerateLootTableEntry(baseId, Enums.SDVObject.OmniGeode,GenerateStackSizeOutcomesForUltraRareResources()),
            };

            outputs.AddRange(GetMiningDrillOutputs_Mines_BottomOfHardMines());

            return outputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetMiningDrillOutputs_VolcanoDungeon()
        {
            string baseId = "Omegasis.Revitalize.Machines.MiningDrillOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> outputs = new()
            {
                GenerateLootTableEntry(baseId, Enums.SDVObject.CinderShard,GenerateStackSizeOutcomesForRareResources()),

                GenerateLootTableEntry(baseId, Enums.SDVObject.CopperOre,GenerateStackSizeOutcomesForNormalResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.IronOre,GenerateStackSizeOutcomesForRareResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.GoldOre,GenerateStackSizeOutcomesForRareResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.IridiumOre,GenerateStackSizeOutcomesForRareResources()),

                GenerateLootTableEntry(baseId, Enums.SDVObject.OmniGeode,GenerateStackSizeOutcomesForUltraRareResources()),
                GenerateLootTableEntry(baseId, Enums.SDVObject.CinderShard,GenerateStackSizeOutcomesForRareResources()),
            };

            outputs.AddRange(GetMiningDrillOutputs_Mines_BottomOfHardMines());

            return outputs;
        }

        private static void GenerateBurnerMiningDrillOutputJsonFiles(string MachineFolderName, int chanceToObtainDoubleArtifacts, float buildingResourceMultiplier, float oreResourceMultiplier, int chanceToObtainDoubleMinerals, int chanceToObtainDoubleGems, int chanceToObtainDoubleGeodes)
        {
            string baseId = MachineIds.CoalMiningDrill;


            GenerateOutputJsonFiles(baseId + MineEntranceLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_Mines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUndergroundLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_InsideMines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_0_39Suffix, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_0_39(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_50Plus, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_50Plus(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_40_80Suffix, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_40_80(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_80Plus, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_80Plus(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + ReachedBottomOfHardMinesSuffix, MachineFolderName, GetMiningDrillOutputs_Mines_BottomOfHardMines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + SkullCaveLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_SkullCaves(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + VolcanoDungeonLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_VolcanoDungeon(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + CalderaLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_VolcanoDungeon(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
        }

        private static void GenerateElectricMiningDrillOutputJsonFiles(string MachineFolderName, int chanceToObtainDoubleArtifacts, float buildingResourceMultiplier, float oreResourceMultiplier, int chanceToObtainDoubleMinerals, int chanceToObtainDoubleGems, int chanceToObtainDoubleGeodes)
        {
            string baseId = MachineIds.ElectricMiningDrill;

            GenerateOutputJsonFiles(baseId + MineEntranceLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_Mines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUndergroundLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_InsideMines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_0_39Suffix, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_0_39(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_50Plus, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_50Plus(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_40_80Suffix, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_40_80(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_80Plus, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_80Plus(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + ReachedBottomOfHardMinesSuffix, MachineFolderName, GetMiningDrillOutputs_Mines_BottomOfHardMines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + SkullCaveLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_SkullCaves(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + VolcanoDungeonLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_VolcanoDungeon(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + CalderaLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_VolcanoDungeon(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
        }

        private static void GenerateNuclearMiningDrillOutputJsonFiles(string MachineFolderName, int chanceToObtainDoubleArtifacts, float buildingResourceMultiplier, float oreResourceMultiplier, int chanceToObtainDoubleMinerals, int chanceToObtainDoubleGems, int chanceToObtainDoubleGeodes)
        {
            string baseId = MachineIds.NuclearMiningDrill;

            GenerateOutputJsonFiles(baseId + MineEntranceLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_Mines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUndergroundLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_InsideMines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_0_39Suffix, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_0_39(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_50Plus, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_50Plus(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_40_80Suffix, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_40_80(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_80Plus, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_80Plus(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + ReachedBottomOfHardMinesSuffix, MachineFolderName, GetMiningDrillOutputs_Mines_BottomOfHardMines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + SkullCaveLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_SkullCaves(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + VolcanoDungeonLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_VolcanoDungeon(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + CalderaLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_VolcanoDungeon(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
        }

        private static void GenerateMagicalMiningDrillOutputJsonFiles(string MachineFolderName, int chanceToObtainDoubleArtifacts, float buildingResourceMultiplier, float oreResourceMultiplier, int chanceToObtainDoubleMinerals, int chanceToObtainDoubleGems, int chanceToObtainDoubleGeodes)
        {
            string baseId = MachineIds.MagicalAdvancedGeodeCrusher;

            GenerateOutputJsonFiles(baseId + MineEntranceLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_Mines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUndergroundLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_InsideMines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_0_39Suffix, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_0_39(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_50Plus, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_50Plus(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_40_80Suffix, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_40_80(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_80Plus, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_80Plus(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + ReachedBottomOfHardMinesSuffix, MachineFolderName, GetMiningDrillOutputs_Mines_BottomOfHardMines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + SkullCaveLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_SkullCaves(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + VolcanoDungeonLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_VolcanoDungeon(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + CalderaLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_VolcanoDungeon(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
        }

        private static void GenerateGalaxyMiningDrillOutputJsonFiles(string MachineFolderName, int chanceToObtainDoubleArtifacts, float buildingResourceMultiplier, float oreResourceMultiplier, int chanceToObtainDoubleMinerals, int chanceToObtainDoubleGems, int chanceToObtainDoubleGeodes)
        {
            string baseId = MachineIds.GalaxyAdvancedGeodeCrusher;

            GenerateOutputJsonFiles(baseId + MineEntranceLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_Mines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUndergroundLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_InsideMines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_0_39Suffix, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_0_39(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_50Plus, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_50Plus(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_40_80Suffix, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_40_80(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + MineUnderground_Floors_80Plus, MachineFolderName, GetMiningDrillOutputs_Mines_Floors_80Plus(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + ReachedBottomOfHardMinesSuffix, MachineFolderName, GetMiningDrillOutputs_Mines_BottomOfHardMines(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + SkullCaveLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_SkullCaves(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + VolcanoDungeonLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_VolcanoDungeon(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
            GenerateOutputJsonFiles(baseId + CalderaLootTableSuffix, MachineFolderName, GetMiningDrillOutputs_VolcanoDungeon(), chanceToObtainDoubleArtifacts, buildingResourceMultiplier, oreResourceMultiplier, chanceToObtainDoubleMinerals, chanceToObtainDoubleGems, chanceToObtainDoubleGeodes);
        }

        private static void GenerateOutputJsonFiles(string LootTableId, string MachineFolderName, List<KeyValuePair<string, ProcessingRecipe>> outputs, int chanceToObtainDoubleArtifacts, float buildingResourceMultiplier, float oreResourceMultiplier, int chanceToObtainDoubleMinerals, int chanceToObtainDoubleGems, int chanceToObtainDoubleGeodes)
        {
            foreach (var v in outputs)
            {
                foreach (var output in v.Value.outputs)
                {
                    //Artifacts
                    if ((output.item.getItem() as StardewValley.Object).Type.Equals("Arch"))
                    {
                        //50% chance to be able to get double artifacts, which is fair considering how useless they quickly become.
                        output.stackSizeDeterminer.First().validRangeForChance = new DoubleRange(0, 100 - chanceToObtainDoubleArtifacts);
                        output.stackSizeDeterminer.Add(new IntOutcomeChanceDeterminer(new DoubleRange(100 - chanceToObtainDoubleArtifacts, 100), new IntRange(output.stackSizeDeterminer.First().outcomeValue.Max + 1, output.stackSizeDeterminer.First().outcomeValue.Max + 1)));
                    }

                    //Stone, clay
                    if ((output.item.getItem() as StardewValley.Object).Category == StardewValley.Object.buildingResources)
                    {
                        foreach (var stackSizeOutputDeterminer in output.stackSizeDeterminer)
                        {
                            int bonus = Math.Max(1, (int)(stackSizeOutputDeterminer.outcomeValue.Max * buildingResourceMultiplier)); //Give up to 25% more resources
                            stackSizeOutputDeterminer.outcomeValue.Max += bonus;
                        }
                    }

                    //Ores
                    if ((output.item.getItem() as StardewValley.Object).Category == StardewValley.Object.metalResources)
                    {
                        foreach (var stackSizeOutputDeterminer in output.stackSizeDeterminer)
                        {
                            int bonus = Math.Max(1, (int)(stackSizeOutputDeterminer.outcomeValue.Max * oreResourceMultiplier)); //Give up to 50% more ore
                            stackSizeOutputDeterminer.outcomeValue.Max += bonus;
                        }
                    }

                    //Gems/Minerals
                    if ((output.item.getItem() as StardewValley.Object).Category == StardewValley.Object.GemCategory)
                    {
                        //50% chance to be able to get double artifacts, which is fair considering how useless they quickly become.
                        output.stackSizeDeterminer.First().validRangeForChance = new DoubleRange(0, 100 - chanceToObtainDoubleGems);
                        output.stackSizeDeterminer.Add(new IntOutcomeChanceDeterminer(new DoubleRange(100 - chanceToObtainDoubleGems, 100), new IntRange(output.stackSizeDeterminer.First().outcomeValue.Max + 1, output.stackSizeDeterminer.First().outcomeValue.Max + 1)));
                    }

                    //Gems/Minerals
                    if ((output.item.getItem() as StardewValley.Object).Category == StardewValley.Object.mineralsCategory)
                    {
                        //50% chance to be able to get double artifacts, which is fair considering how useless they quickly become.
                        output.stackSizeDeterminer.First().validRangeForChance = new DoubleRange(0, 100 - chanceToObtainDoubleMinerals);
                        output.stackSizeDeterminer.Add(new IntOutcomeChanceDeterminer(new DoubleRange(100 - chanceToObtainDoubleMinerals, 100), new IntRange(output.stackSizeDeterminer.First().outcomeValue.Max + 1, output.stackSizeDeterminer.First().outcomeValue.Max + 1)));
                    }

                    if (Utility.IsGeode((output.item.getItem() as StardewValley.Object)))
                    {
                        //50% chance to be able to get double artifacts, which is fair considering how useless they quickly become.
                        output.stackSizeDeterminer.First().validRangeForChance = new DoubleRange(0, 100 - chanceToObtainDoubleGeodes);
                        output.stackSizeDeterminer.Add(new IntOutcomeChanceDeterminer(new DoubleRange(100 - chanceToObtainDoubleGeodes, 100), new IntRange(output.stackSizeDeterminer.First().outcomeValue.Max + 1, output.stackSizeDeterminer.First().outcomeValue.Max + 1)));
                    }
                }

                JsonUtilities.WriteJsonFile(new Dictionary<string, List<ProcessingRecipe>>() { { LootTableId, new List<ProcessingRecipe>() { v.Value } } }, ObjectsDataPaths.ProcessingRecipesPath, "Production", "MiningDrills", MachineFolderName, LootTableId.Split("_").Last(), v.Key + ".json");
            }
        }

        public static KeyValuePair<string, ProcessingRecipe> GenerateLootTableEntry(string baseId, Enums.SDVObject outputObject, int outputStackSize)
        {
            return GenerateLootTableEntry(baseId, outputObject, new IntRange(outputStackSize, outputStackSize));
        }

        public static KeyValuePair<string, ProcessingRecipe> GenerateLootTableEntry(string baseId, Enums.SDVObject outputObject, IntRange outputStackSizeRange)
        {
            return GenerateLootTableEntry(baseId, outputObject, new List<IntOutcomeChanceDeterminer>() { new IntOutcomeChanceDeterminer(new DoubleRange(0, 100), outputStackSizeRange) });
        }

        public static KeyValuePair<string, ProcessingRecipe> GenerateLootTableEntry(string baseId, Enums.SDVObject outputObject, List<IntOutcomeChanceDeterminer> outputStackSizeRange)
        {
            string outputName = Enum.GetName(outputObject);
            string id = baseId + outputName;
            string relativePath = outputName;

            return new KeyValuePair<string, ProcessingRecipe>(relativePath, new ProcessingRecipe(id, new GameTimeStamp(0, 0, 1, 0, 0), new ItemReference(""), new LootTableEntry(new ItemReference(outputObject), outputStackSizeRange)));
        }

        public static KeyValuePair<string, ProcessingRecipe> GenerateLootTableEntry(string Id, string Path, ItemReference outputObject, List<IntOutcomeChanceDeterminer> outputStackSizeRange)
        {

            return new KeyValuePair<string, ProcessingRecipe>(Path, new ProcessingRecipe(Id, new GameTimeStamp(0, 0, 1, 0, 0), new ItemReference(""), new LootTableEntry(outputObject, outputStackSizeRange)));
        }

        public static List<IntOutcomeChanceDeterminer> GenerateStackSizeOutcomesForNormalResources()
        {
            List<IntOutcomeChanceDeterminer> outcomes = new List<IntOutcomeChanceDeterminer>
            {
                new IntOutcomeChanceDeterminer(new DoubleRange(0, 30), 1),
                new IntOutcomeChanceDeterminer(new DoubleRange(30, 60), 3),
                new IntOutcomeChanceDeterminer(new DoubleRange(60, 90), 5),
                new IntOutcomeChanceDeterminer(new DoubleRange(90, 99), 10),
                new IntOutcomeChanceDeterminer(new DoubleRange(99, 100), 20)
            };
            return outcomes;
        }

        public static List<IntOutcomeChanceDeterminer> GenerateStackSizeOutcomesForRareResources()
        {
            List<IntOutcomeChanceDeterminer> outcomes = new List<IntOutcomeChanceDeterminer>
            {
                new IntOutcomeChanceDeterminer(new DoubleRange(0, 30), 1),
                new IntOutcomeChanceDeterminer(new DoubleRange(30, 60), 2),
                new IntOutcomeChanceDeterminer(new DoubleRange(60, 90), 3),
                new IntOutcomeChanceDeterminer(new DoubleRange(90, 99), 6),
                new IntOutcomeChanceDeterminer(new DoubleRange(99, 100), 11)
            };
            return outcomes;
        }

        public static List<IntOutcomeChanceDeterminer> GenerateStackSizeOutcomesForUltraRareResources()
        {
            List<IntOutcomeChanceDeterminer> outcomes = new List<IntOutcomeChanceDeterminer>
            {
                new IntOutcomeChanceDeterminer(new DoubleRange(0, 30), 1),
                new IntOutcomeChanceDeterminer(new DoubleRange(30, 60), 2),
                new IntOutcomeChanceDeterminer(new DoubleRange(60, 90), 3),
                new IntOutcomeChanceDeterminer(new DoubleRange(90, 99), 5),
                new IntOutcomeChanceDeterminer(new DoubleRange(99, 100), 8)
            };
            return outcomes;
        }
        #endregion
    }
}
