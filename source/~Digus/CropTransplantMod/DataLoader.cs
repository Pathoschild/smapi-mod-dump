/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CropTransplantMod.integrations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace CropTransplantMod
{
    public class DataLoader
    {
        public static IModHelper Helper;
        public static ITranslationHelper I18N;
        public static ModConfig ModConfig;

        public DataLoader(IModHelper helper, IManifest manifest)
        {
            Helper = helper;
            I18N = helper.Translation;
            ModConfig = helper.ReadConfig<ModConfig>();

            LoadMail();

            CreateConfigMenu(manifest);
        }

        internal static void LoadMail()
        {
            IMailFrameworkModApi mailFrameworkModApi = Helper.ModRegistry.GetApi<IMailFrameworkModApi>("DIGUS.MailFrameworkMod");

            mailFrameworkModApi?.RegisterLetter(
                new ApiLetter
                {
                    Id = "CropTransplantLetter"
                    , Text = "CropTransplant.Letter"
                    , Title = "CropTransplant.Letter.Title"
                    , I18N = I18N
                }, (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CropTransplantPotLetter") && Game1.player.craftingRecipes.ContainsKey("Garden Pot") && !ModConfig.GetGardenPotEarlier
                , (l) => Game1.player.mailReceived.Add(l.Id)
            );

            mailFrameworkModApi?.RegisterLetter
            (
                new ApiLetter
                {
                    Id ="CropTransplantPotLetter"
                    , Text = "CropTransplantPot.Letter"
                    , Items = new List<Item>() { ItemRegistry.Create("(BC)62") }
                    , Title = "CropTransplant.Letter.Title"
                    , I18N = I18N
                }
                , (l) => !Game1.player.mailReceived.Contains(l.Id) && !Game1.player.mailReceived.Contains("CropTransplantLetter") && GetNpcFriendship("Evelyn") >= 2 * 250 && ModConfig.GetGardenPotEarlier
                , (l) => Game1.player.mailReceived.Add(l.Id)
            );
        }

        private static int GetNpcFriendship(string name)
        {
            if (Game1.player.friendshipData.ContainsKey(name))
            {
                return Game1.player.friendshipData[name].Points;
            }
            else
            {
                return 0;
            }
        }

        private void CreateConfigMenu(IManifest manifest)
        {
            GenericModConfigMenuApi api = Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (api != null)
            {
                api.Register( manifest, () => DataLoader.ModConfig = new ModConfig(), () => Helper.WriteConfig(DataLoader.ModConfig));

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.GetGardenPotEarlier, (bool val) => DataLoader.ModConfig.GetGardenPotEarlier = val, () => "Get Garden Pot Earlier", () => "Evelyn will send you a Garden Pot once you reach 2 hearts level of friendship with her. You will not learn the recipe though, for that you should get the greenhouse as normal.");
                
                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableSoilTileUnderTrees, (bool val) => DataLoader.ModConfig.EnableSoilTileUnderTrees = val, () => "Soil Under Trees", () => "A soil tile will be shown under trees when planted on stone floor, wood and other 'not plantable' soil. Some tiles aren't correct labeled by the game, so the soil tile may appears on places you would think it's not needed.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableUnlimitedRangeToTransplant, (bool val) => DataLoader.ModConfig.EnableUnlimitedRangeToTransplant = val, () => "Unlimited Range", () => "Let you grab a plant into the held garden pot and place it again on the floor from/to any suitable tile on the screen. No need to get near it.");

                api.AddSectionTitle(manifest, () => "Crop Properties:", () => "Properties for crops transplant.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnablePlacementOfCropsOutsideOutOfTheFarm, (bool val) => DataLoader.ModConfig.EnablePlacementOfCropsOutsideOutOfTheFarm = val, () => "Place Outside The Farm", () => "You'll be able to place Garden Pot holding a crop on outdoor areas out of the farm. You can also put crops on hoed tiles out of the farm.");

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.CropTransplantEnergyCost, (float val) => DataLoader.ModConfig.CropTransplantEnergyCost = val, () => "Transplant Energy Cost", () => "The cost of energy for lifting a crop from the ground. The energy cost decrease as you level up the farming skill. Level 10 will cost 50% less energy than the base cost.");

                api.AddSectionTitle(manifest, () => "Tree Properties:", () => "Properties for regular tree transplant.");

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.TreeTransplantMaxStage, (int val) => DataLoader.ModConfig.TreeTransplantMaxStage = val, () => "Max Stage", () => "The max stage a tree can be lifted. 0 to disable the lifting of any tree. 5 will enable all stages.",  0, 5);

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnablePlacementOfTreesOnAnyTileType, (bool val) => DataLoader.ModConfig.EnablePlacementOfTreesOnAnyTileType = val, () => "Place On Any Tile", () => "You'll be able to place trees on any unoccupied tile type.");

                string energyCostDescription = "The cost of energy for lifting a {0} stage {1}({2}) from the ground. Check the wiki for the stage images. The energy cost decreases as you level up the {3} skill. Level 10 will cost 50% less energy than the base cost.";

                string treeName = "tree";
                string treeSkill = "foraging";
                api.AddNumberOption(manifest, () => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[0], (float val) => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[0] = val, () => "Stage 1 Energy Cost", () => string.Format(energyCostDescription, treeName, "1", "seed", treeSkill));

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[1], (float val) => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[1] = val, () => "Stage 2 Energy Cost", () => string.Format(energyCostDescription, treeName, "2", "sprout", treeSkill));

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[2], (float val) => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[2] = val, () => "Stage 3 Energy Cost", () => string.Format(energyCostDescription, treeName, "3", "sapling", treeSkill));

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[3], (float val) => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[3] = val, () => "Stage 4 Energy Cost", () => string.Format(energyCostDescription, treeName, "4", "bush", treeSkill));

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[4], (float val) => DataLoader.ModConfig.TreeTransplantEnergyCostPerStage[4] = val, () => "Stage 5 Energy Cost", () => string.Format(energyCostDescription, treeName, "5", "tree", treeSkill));

                api.AddSectionTitle(manifest, () => "Fruit Tree Properties:", () => "Properties for fruit tree transplant.");

                api.AddNumberOption(manifest,  () => DataLoader.ModConfig.TreeTransplantMaxStage, (int val) => DataLoader.ModConfig.FruitTreeTransplantMaxStage = val, () => "Max Stage", () => "The max stage a fruit tree can be lifted. 0 to disable the lifting of any fruit tree. 5 will enable all stages.", min: 0, max: 5);

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnablePlacementOfFruitTreesOutOfTheFarm, (bool val) => DataLoader.ModConfig.EnablePlacementOfFruitTreesOutOfTheFarm = val, () => "Place Outside The Farm", () => "﻿You'll be able to place fruit trees out of the farm.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnablePlacementOfFruitTreesOnAnyTileType, (bool val) => DataLoader.ModConfig.EnablePlacementOfFruitTreesOnAnyTileType = val, () => "Place On Any Tile", () => "You'll be able to place fruit trees on any unoccupied tile type.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnablePlacementOfFruitTreesNextToAnotherTree, (bool val) => DataLoader.ModConfig.EnablePlacementOfFruitTreesNextToAnotherTree = val, () => "Place Next To Another Tree", () => "You'll be able to place fruit trees next to other trees. They will still not mature if to close to other stuff.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnablePlacementOfFruitTreesBlockedGrowth, (bool val) => DataLoader.ModConfig.EnablePlacementOfFruitTreesBlockedGrowth = val, () => "Place Blocked Growth", () => "You'll be able to place immature fruit trees where they will not mature.");

                string fruitTreeName = "fruit tree";
                string fruitTreeSkill = "foraging";

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[0], (float val) => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[0] = val, () => "Stage 1 Energy Cost", () => string.Format(energyCostDescription, fruitTreeName, "1", "sapling", fruitTreeSkill));

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[1], (float val) => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[1] = val, () => "Stage 2 Energy Cost", () => string.Format(energyCostDescription, fruitTreeName, "2", "small bush", fruitTreeSkill));

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[2], (float val) => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[2] = val, () => "Stage 3 Energy Cost", () => string.Format(energyCostDescription, fruitTreeName, "3", "large bush", fruitTreeSkill));

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[3], (float val) => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[3] = val, () => "Stage 4 Energy Cost", () => string.Format(energyCostDescription, fruitTreeName, "4", "small tree", fruitTreeSkill));

                api.AddNumberOption(manifest, () => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[4], (float val) => DataLoader.ModConfig.FruitTreeTransplantEnergyCostPerStage[4] = val, () => "Stage 5 Energy Cost", () => string.Format(energyCostDescription, fruitTreeName, "5", "tree", fruitTreeSkill) );

                api.AddSectionTitle(manifest, () => "Tea Bush Properties:", () => "Properties for tea bush transplant.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableToPlantTeaBushesOutOfTheFarm, (bool val) => DataLoader.ModConfig.EnableToPlantTeaBushesOutOfTheFarm = val, () => "Place Outside The Farm", () => "You'll be able to place tea bush out of the farm.");

                api.AddBoolOption(manifest, () => DataLoader.ModConfig.EnableToPlantTeaBushesOnAnyTileType, (bool val) => DataLoader.ModConfig.EnableToPlantTeaBushesOnAnyTileType = val, () => "Place On Any Tile", () => "You'll be able to place tea bush on any unoccupied tile type.");
            }
        }
    }
}
