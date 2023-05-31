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
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Data;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.Crafting.JsonContent;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.Utilities.Extensions;
using Omegasis.Revitalize.Framework.Utilities.Ranges;
using Omegasis.Revitalize.Framework.World.Objects.InformationFiles;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Items;
using StardewValley;
using StardewValley.Locations;

namespace Omegasis.Revitalize.Framework.World.Objects.Machines.Misc
{
    [XmlType("Mods_Omegasis.Revitalize.Framework.World.Objects.Machines.Misc.AdvancedGeodeCrusher")]
    public class AdvancedGeodeCrusher : PoweredMachine
    {

        public AdvancedGeodeCrusher()
        {

        }

        public AdvancedGeodeCrusher(BasicItemInformation Info, PoweredMachineTier machineTier) : this(Info, Vector2.Zero, machineTier)
        {

        }

        public AdvancedGeodeCrusher(BasicItemInformation Info, Vector2 TilePosition, PoweredMachineTier machineTier) : base(Info, TilePosition, machineTier)
        {
        }

        public override CraftingResult processInput(IList<Item> inputItems, Farmer who, bool ShowRedMessage = true)
        {
            if (string.IsNullOrEmpty(this.getCraftingRecipeBookId()) || this.isWorking() || this.finishedProduction())
            {
                return new CraftingResult(false);
            }

            List<KeyValuePair<IList<Item>, ProcessingRecipe>> validRecipes = this.getListOfValidRecipes(inputItems, who, ShowRedMessage);

            if (validRecipes.Count > 0)
            {
                int randElement = Game1.random.Next(validRecipes.Count);
                return this.onSuccessfulRecipeFound(validRecipes.ElementAt(randElement).Key, validRecipes.ElementAt(randElement).Value, who);
            }

            return new CraftingResult(false);
        }

        public override CraftingResult onSuccessfulRecipeFound(IList<Item> dropInItem, ProcessingRecipe craftingRecipe, Farmer who = null)
        {
            CraftingResult result = base.onSuccessfulRecipeFound(dropInItem, craftingRecipe, who);
            if (result.successful)
            {
                if (who != null && this.MachineTier!=PoweredMachineTier.Manual)
                {
                    Utility.addSmokePuff(who.currentLocation, this.TileLocation * 64f + new Vector2(4f, -48f), 200);
                    Utility.addSmokePuff(who.currentLocation, this.TileLocation * 64f + new Vector2(-16f, -56f), 300);
                    Utility.addSmokePuff(who.currentLocation, this.TileLocation * 64f + new Vector2(16f, -52f), 400);
                    Utility.addSmokePuff(who.currentLocation, this.TileLocation * 64f + new Vector2(32f, -56f), 200);
                    Utility.addSmokePuff(who.currentLocation, this.TileLocation * 64f + new Vector2(40f, -44f), 500);
                }
            }
            return result;
        }

        public override void playDropInSound()
        {
            SoundUtilities.PlaySound(Enums.StardewSound.drumkit4);
            SoundUtilities.PlaySound(Enums.StardewSound.stoneCrack);
            if (this.MachineTier != PoweredMachineTier.Manual)
            {
                SoundUtilities.PlaySoundWithDelay(Enums.StardewSound.steam, 200);
            }
        }


        public override Item getOne()
        {
            return new AdvancedGeodeCrusher(this.basicItemInformation.Copy(), this.machineTier.Value);
        }


        public static void GenerateOutputJsonFiles()
        {

            List<KeyValuePair<string, ProcessingRecipe>> defaultOutputs = GetNormalGeodeOutputs();
            defaultOutputs.AddRange(GetFrozenGeodeOutputs());
            defaultOutputs.AddRange(GetMagmaGeodeOutputs());
            defaultOutputs.AddRange(GetOmniGeodeOutputs());

            defaultOutputs.Clear();

            defaultOutputs.AddRange(GetNormalGeodeOutputs());
            defaultOutputs.AddRange(GetFrozenGeodeOutputs());
            defaultOutputs.AddRange(GetMagmaGeodeOutputs());
            defaultOutputs.AddRange(GetOmniGeodeOutputs());

            GenerateGeodeCrusherOutputsJsonFiles(MachineIds.AdvancedBurnerGeodeCrusher, "AdvancedBurnerGeodeCrusher", defaultOutputs, 50, .25f, .1f, 10, 1);

            defaultOutputs.Clear();

            defaultOutputs.AddRange(GetNormalGeodeOutputs());
            defaultOutputs.AddRange(GetFrozenGeodeOutputs());
            defaultOutputs.AddRange(GetMagmaGeodeOutputs());
            defaultOutputs.AddRange(GetOmniGeodeOutputs());

            GenerateGeodeCrusherOutputsJsonFiles(MachineIds.ElectricAdvancedGeodeCrusher, "ElectricGeodeCrusher", defaultOutputs.DeepClone(), 75, .50f, .25f, 25, 5);

            defaultOutputs.Clear();

            defaultOutputs.AddRange(GetNormalGeodeOutputs());
            defaultOutputs.AddRange(GetFrozenGeodeOutputs());
            defaultOutputs.AddRange(GetMagmaGeodeOutputs());
            defaultOutputs.AddRange(GetOmniGeodeOutputs());

            GenerateGeodeCrusherOutputsJsonFiles(MachineIds.NuclearAdvancedGeodeCrusher, "NuclearAdvancedGeodeCrusher", defaultOutputs.DeepClone(), 100, .75f, .50f, 50, 15);

            defaultOutputs.Clear();

            defaultOutputs.AddRange(GetNormalGeodeOutputs());
            defaultOutputs.AddRange(GetFrozenGeodeOutputs());
            defaultOutputs.AddRange(GetMagmaGeodeOutputs());
            defaultOutputs.AddRange(GetOmniGeodeOutputs());
            defaultOutputs.AddRange(GetArtifactTroveOutputs());
            defaultOutputs.AddRange(GetGoldenCoconutOutputs());

            GenerateGeodeCrusherOutputsJsonFiles(MachineIds.MagicalAdvancedGeodeCrusher, "MagicalAdvancedGeodeCrusher", defaultOutputs.DeepClone(), 100, 1f, .75f, 80, 25);

            defaultOutputs.Clear();

            defaultOutputs.AddRange(GetNormalGeodeOutputs());
            defaultOutputs.AddRange(GetFrozenGeodeOutputs());
            defaultOutputs.AddRange(GetMagmaGeodeOutputs());
            defaultOutputs.AddRange(GetOmniGeodeOutputs());
            defaultOutputs.AddRange(GetArtifactTroveOutputs());
            defaultOutputs.AddRange(GetGoldenCoconutOutputs());

            GenerateGeodeCrusherOutputsJsonFiles(MachineIds.GalaxyAdvancedGeodeCrusher, "GalaxyAdvancedGeodeCrusher", defaultOutputs.DeepClone(), 100, 1.25f, 1f, 100, 50); //If for some reason I ever add in Galaxy Geode Crushers,


            defaultOutputs.Clear();

            defaultOutputs.AddRange(GetNormalGeodeOutputs());
            defaultOutputs.AddRange(GetFrozenGeodeOutputs());
            defaultOutputs.AddRange(GetMagmaGeodeOutputs());
            defaultOutputs.AddRange(GetOmniGeodeOutputs());
            defaultOutputs.AddRange(GetArtifactTroveOutputs());
            defaultOutputs.AddRange(GetGoldenCoconutOutputs());

            GenerateGeodeCrusherOutputsJsonFiles(MachineIds.Anvil, "Anvil", defaultOutputs.DeepClone(), 0, 0, 0, 0, 0); //If for some reason I ever add in Galaxy Geode Crushers, 
        }

        

        private static List<KeyValuePair<string, ProcessingRecipe>> GenerateGeodeCrusherOutputsJsonFiles(string MachineId, string MachineFolderName, List<KeyValuePair<string, ProcessingRecipe>> outputs, int chanceToObtainDoubleArtifacts, float buildingResourceMultiplier, float oreResourceMultiplier, int chanceToObtainDoubleMinerals, int chanceToObtainDoubleGems)
        {

            foreach (var v in outputs)
            {
                foreach (var output in v.Value.outputs)
                {
                    //Artifacts
                    if ((output.item.getItem() as StardewValley.Object).Type.Equals("Arch") && chanceToObtainDoubleArtifacts>0)
                    {
                        //50% chance to be able to get double artifacts, which is fair considering how useless they quickly become.
                        output.stackSizeDeterminer.First().validRangeForChance = new DoubleRange(0, 100 - chanceToObtainDoubleArtifacts);
                        output.stackSizeDeterminer.Add(new IntOutcomeChanceDeterminer(new DoubleRange(100 - chanceToObtainDoubleArtifacts, 100), new IntRange(output.stackSizeDeterminer.First().outcomeValue.Max + 1, output.stackSizeDeterminer.First().outcomeValue.Max + 1)));
                    }

                    //Stone, clay
                    if ((output.item.getItem() as StardewValley.Object).Category == StardewValley.Object.buildingResources && buildingResourceMultiplier>0)
                    {
                        foreach (var stackSizeOutputDeterminer in output.stackSizeDeterminer)
                        {
                            int bonus = Math.Max(1, (int)(stackSizeOutputDeterminer.outcomeValue.Max * buildingResourceMultiplier)); //Give up to 25% more resources
                            stackSizeOutputDeterminer.outcomeValue.Max += bonus;
                        }
                    }

                    //Ores
                    if ((output.item.getItem() as StardewValley.Object).Category == StardewValley.Object.metalResources && oreResourceMultiplier >0)
                    {
                        foreach (var stackSizeOutputDeterminer in output.stackSizeDeterminer)
                        {
                            int bonus = Math.Max(1, (int)(stackSizeOutputDeterminer.outcomeValue.Max * oreResourceMultiplier)); //Give up to 50% more ore

                            stackSizeOutputDeterminer.outcomeValue.Max += bonus;
                        }
                    }

                    //Gems/Minerals
                    if ((output.item.getItem() as StardewValley.Object).Category == StardewValley.Object.GemCategory && chanceToObtainDoubleGems>0)
                    {
                        //50% chance to be able to get double artifacts, which is fair considering how useless they quickly become.
                        output.stackSizeDeterminer.First().validRangeForChance = new DoubleRange(0, 100 - chanceToObtainDoubleGems);
                        output.stackSizeDeterminer.Add(new IntOutcomeChanceDeterminer(new DoubleRange(100 - chanceToObtainDoubleGems, 100), new IntRange(output.stackSizeDeterminer.First().outcomeValue.Max + 1, output.stackSizeDeterminer.First().outcomeValue.Max + 1)));
                    }

                    //Gems/Minerals
                    if ((output.item.getItem() as StardewValley.Object).Category == StardewValley.Object.mineralsCategory && chanceToObtainDoubleMinerals >0)
                    {
                        //50% chance to be able to get double artifacts, which is fair considering how useless they quickly become.
                        output.stackSizeDeterminer.First().validRangeForChance = new DoubleRange(0, 100 - chanceToObtainDoubleMinerals);
                        output.stackSizeDeterminer.Add(new IntOutcomeChanceDeterminer(new DoubleRange(100 - chanceToObtainDoubleMinerals, 100), new IntRange(output.stackSizeDeterminer.First().outcomeValue.Max + 1, output.stackSizeDeterminer.First().outcomeValue.Max + 1)));
                    }
                }

                JsonUtilities.WriteJsonFile(new Dictionary<string, List<ProcessingRecipe>>() { { MachineId, new List<ProcessingRecipe>() { v.Value } } }, ObjectsDataPaths.ProcessingRecipesPath, "Production", "GeodeCrushers", MachineFolderName, v.Key + ".json");
            }
            return outputs;
        }


        public static List<KeyValuePair<string, ProcessingRecipe>> GetNormalGeodeOutputs()
        {
            string baseId = "Omegasis.Revitalize.Machines.GeodeCrusherOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> geodeOutputs = new()
            {
                //Geodes

                //Artifacts
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.DwarvishHelm, 1),

                //Forged minerals
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.EarthCrystal, 1),

                //Geode minerals
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Alamite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Calcite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Celestine, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Granite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Jagoite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Jamborite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Limestone, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Malachite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Mudstone, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Nekoite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Orpiment, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.PetrifiedSlime, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Sandstone, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Slate, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.ThunderEgg, 1),

                //Resources
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Stone, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Clay, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.Coal, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.CopperOre, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.Geode, Enums.SDVObject.IronOre, GenerateStackSizeOutcomesForNormalResources())
            };

            return geodeOutputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetFrozenGeodeOutputs()
        {
            string baseId = "Omegasis.Revitalize.Machines.GeodeCrusherOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> geodeOutputs = new() { 

            //Artifacts

                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.AncientDrum, 1),
            //Forged minerals

                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.FrozenTear, 1),

            //Geode minerals

                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Aerinite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Esperite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.FairyStone, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Fluorapatite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Geminite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.GhostCrystal, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Hematite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Kyanite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Lunarite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Marble, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.OceanStone, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Opal, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Pyrite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Soapstone, 1),

            //Resources
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Stone, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Clay, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.Coal, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.CopperOre, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.IronOre, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.FrozenGeode, Enums.SDVObject.GoldOre, GenerateStackSizeOutcomesForNormalResources())
            };

            return geodeOutputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetMagmaGeodeOutputs()
        {
            string baseId = "Omegasis.Revitalize.Machines.GeodeCrusherOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> geodeOutputs = new() { 

            //Artifacts

                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.DwarfGadget, 1),
            //Forged minerals

                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.FireQuartz, 1),

            //Geode minerals

                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Baryte, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Basalt, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Bixite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Dolomite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.FireOpal, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Helvite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Jasper, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.LemonStone, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Neptunite, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Obsidian, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.StarShards, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Tigerseye, 1),

            //Resources
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Stone, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Clay, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.Coal, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.CopperOre, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.IronOre, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.GoldOre, GenerateStackSizeOutcomesForNormalResources()),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.MagmaGeode, Enums.SDVObject.IridiumOre, GenerateStackSizeOutcomesForRareResources())
            };

            return geodeOutputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetOmniGeodeOutputs()
        {
            string baseId = "Omegasis.Revitalize.Machines.GeodeCrusherOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> geodeOutputs = new() {

               ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.OmniGeode, Enums.SDVObject.PrismaticShard, 1),
            };


            foreach (KeyValuePair<string, ProcessingRecipe> pair in GetNormalGeodeOutputs())
            {
                string outputObject = pair.Key.Split("\\").Skip(1).First();
                string relativePath = Path.Combine("OmniGeode", outputObject);
                string id = baseId + "OmniGeode" + "." + outputObject;

                geodeOutputs.Add(ProcessingRecipe.GenerateLootTableEntry(id, relativePath, Enums.SDVObject.OmniGeode, pair.Value.outputs.First().item, pair.Value.outputs.First().stackSizeDeterminer));
            }

            foreach (KeyValuePair<string, ProcessingRecipe> pair in GetFrozenGeodeOutputs())
            {
                string outputObject = pair.Key.Split("\\").Skip(1).First();
                string relativePath = Path.Combine("OmniGeode", outputObject);
                string id = baseId + "OmniGeode" + "." + outputObject;

                geodeOutputs.Add(ProcessingRecipe.GenerateLootTableEntry(id, relativePath, Enums.SDVObject.OmniGeode, pair.Value.outputs.First().item, pair.Value.outputs.First().stackSizeDeterminer));
            }

            foreach (KeyValuePair<string, ProcessingRecipe> pair in GetMagmaGeodeOutputs())
            {
                string outputObject = pair.Key.Split("\\").Skip(1).First();
                string relativePath = Path.Combine("OmniGeode", outputObject);
                string id = baseId + "OmniGeode" + "." + outputObject;

                geodeOutputs.Add(ProcessingRecipe.GenerateLootTableEntry(id, relativePath, Enums.SDVObject.OmniGeode, pair.Value.outputs.First().item, pair.Value.outputs.First().stackSizeDeterminer));
            }

            return geodeOutputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetArtifactTroveOutputs()
        {
            string baseId = "Omegasis.Revitalize.Machines.GeodeCrusherOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> geodeOutputs = new()
            {
                //Geodes

                //Artifacts
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.Anchor, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.AncientDoll, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.AncientDrum, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.AncientSeed, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.AncientSword, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.Arrowhead, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.BoneFlute, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.ChewingStick, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.ChickenStatue, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.ChippedAmphora, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.DriedStarfish, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.DwarfGadget, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.DwarvishHelm, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.ElvishJewelry, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.GlassShards, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.GoldenMask, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.GoldenPumpkin, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.GoldenRelic, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.OrnamentalFan, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.Pearl, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.PrehistoricHandaxe, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.PrehistoricTool, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.RareDisc, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.RustyCog, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.RustySpoon, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.RustySpur, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.ArtifactTrove, Enums.SDVObject.TreasureChest, 1),
            };

            return geodeOutputs;
        }

        public static List<KeyValuePair<string, ProcessingRecipe>> GetGoldenCoconutOutputs()
        {
            string baseId = "Omegasis.Revitalize.Machines.GeodeCrusherOutputs.";
            List<KeyValuePair<string, ProcessingRecipe>> geodeOutputs = new()
            {
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.GoldenCoconut, Enums.SDVObject.BananaSapling, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.GoldenCoconut, Enums.SDVObject.MangoSapling, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.GoldenCoconut, Enums.SDVObject.PineappleSeeds, 5),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.GoldenCoconut, Enums.SDVObject.TaroTuber, 5),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.GoldenCoconut, Enums.SDVObject.MahoganySeed, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.GoldenCoconut, Enums.SDVObject.FossilizedSkull, 1),
                ProcessingRecipe.GenerateLootTableEntry(baseId, Enums.SDVObject.GoldenCoconut, Enums.SDVObject.IridiumOre, 5),
            };

            return geodeOutputs;
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
    }
}
