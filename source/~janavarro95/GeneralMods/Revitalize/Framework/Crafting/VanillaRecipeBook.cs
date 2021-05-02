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
using Revitalize.Framework.Utilities;
using StardewValley;

namespace Revitalize.Framework.Crafting
{
    public class VanillaRecipeBook
    {
        public static VanillaRecipeBook VanillaRecipes;

        public Dictionary<string,Dictionary<string,VanillaRecipe>> recipesByObjectName;
        /// <summary>
        /// All of the recipes bassed off of pytk id. the first key is the name of the SDV object, the second key is the player's held object pktk id if it exists. 
        /// </summary>
        public Dictionary<string, Dictionary<string, VanillaRecipe>> recipesByObjectPyTKID;
        public Dictionary<string, Dictionary<Item, VanillaRecipe>> recipesByObject;

        public VanillaRecipeBook()
        {
            this.recipesByObjectName = new Dictionary<string, Dictionary<string, VanillaRecipe>>();
            this.recipesByObjectPyTKID = new Dictionary<string, Dictionary<string, VanillaRecipe>>();
            this.recipesByObject = new Dictionary<string, Dictionary<Item, VanillaRecipe>>();
            if (VanillaRecipes == null)
            {
                VanillaRecipes = this;
            }

            this.recipesByObjectName = new Dictionary<string, Dictionary<string, VanillaRecipe>>();
            this.recipesByObjectName.Add("Furnace", new Dictionary<string, VanillaRecipe>());

            VanillaRecipe furnace_tinOre = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {ModCore.ObjectManager.resources.getOre("Tin"),5 },
                {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1}
            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.GetItem("TinIngot"), 1), TimeUtilities.GetMinutesFromTime(0,0,50), new StatCost(), false);

            this.recipesByObjectName["Furnace"].Add("Tin Ore", furnace_tinOre);

            VanillaRecipe furnace_bauxiteOre = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {ModCore.ObjectManager.resources.getOre("Bauxite"),5 },
                {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1}
            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.GetItem("AluminumIngot"), 1), TimeUtilities.GetMinutesFromTime(0,1,30), new StatCost(), false);

            this.recipesByObjectName["Furnace"].Add("Bauxite Ore", furnace_bauxiteOre);

            VanillaRecipe furnace_leadOre = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {ModCore.ObjectManager.resources.getOre("Lead"),5 },
                {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1}
            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.GetItem("LeadIngot"), 1), TimeUtilities.GetMinutesFromTime(0,2,0), new StatCost(), false);

            this.recipesByObjectName["Furnace"].Add("Lead Ore", furnace_leadOre);

            VanillaRecipe furnace_silverOre = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {ModCore.ObjectManager.resources.getOre("Silver"),5 },
                {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1}
            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.GetItem("SilverIngot"), 1), TimeUtilities.GetMinutesFromTime(0,3,0), new StatCost(), false);

            this.recipesByObjectName["Furnace"].Add("Silver Ore", furnace_silverOre);

            VanillaRecipe furnace_titaniumOre = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {ModCore.ObjectManager.resources.getOre("Titanium"),5 },
                {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1}
            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.GetItem("TitaniumIngot"), 1), TimeUtilities.GetMinutesFromTime(0,4,0), new StatCost(), false);

            this.recipesByObjectName["Furnace"].Add("Titanium Ore", furnace_titaniumOre);

            VanillaRecipe furnace_prismaticNugget = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {ModCore.ObjectManager.resources.getOre("PrismaticNugget"),7 },
                {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1}
            }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.PrismaticShard,1), 1), TimeUtilities.GetMinutesFromTime(0, 7, 0), new StatCost(), false);

            this.recipesByObjectName["Furnace"].Add("Prismatic Nugget", furnace_prismaticNugget);

            if (ModCore.Configs.vanillaMachineConfig.ExpensiveGemstoneToPrismaticFurnaceRecipe)
            {
                VanillaRecipe furnace_gemsToPrismaticNugget = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Emerald,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Aquamarine,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Ruby,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Amethyst,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Topaz,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Jade,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Diamond,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1}
            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.resources.getOre("PrismaticNugget"), 1), TimeUtilities.GetMinutesFromTime(7, 0, 0), new StatCost(), false);

                this.recipesByObjectName["Furnace"].Add("Emerald", furnace_gemsToPrismaticNugget);
                this.recipesByObjectName["Furnace"].Add("Aquamarine", furnace_gemsToPrismaticNugget);
                this.recipesByObjectName["Furnace"].Add("Ruby", furnace_gemsToPrismaticNugget);
                this.recipesByObjectName["Furnace"].Add("Amethyst", furnace_gemsToPrismaticNugget);
                this.recipesByObjectName["Furnace"].Add("Topaz", furnace_gemsToPrismaticNugget);
                this.recipesByObjectName["Furnace"].Add("Jade", furnace_gemsToPrismaticNugget);
                this.recipesByObjectName["Furnace"].Add("Diamond", furnace_gemsToPrismaticNugget);
            }
            else
            {
                VanillaRecipe furnace_gemsToPrismaticShard = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Emerald,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Aquamarine,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Ruby,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Amethyst,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Topaz,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Jade,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Diamond,1),1},
                {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1}
            }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.PrismaticShard,1), 1), TimeUtilities.GetMinutesFromTime(7, 0, 0), new StatCost(), false);

                this.recipesByObjectName["Furnace"].Add("Emerald", furnace_gemsToPrismaticShard);
                this.recipesByObjectName["Furnace"].Add("Aquamarine", furnace_gemsToPrismaticShard);
                this.recipesByObjectName["Furnace"].Add("Ruby", furnace_gemsToPrismaticShard);
                this.recipesByObjectName["Furnace"].Add("Amethyst", furnace_gemsToPrismaticShard);
                this.recipesByObjectName["Furnace"].Add("Topaz", furnace_gemsToPrismaticShard);
                this.recipesByObjectName["Furnace"].Add("Jade", furnace_gemsToPrismaticShard);
                this.recipesByObjectName["Furnace"].Add("Diamond", furnace_gemsToPrismaticShard);
            }

            VanillaRecipe furnace_steelIngot = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.IronBar,1),1 },
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),5}
            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.GetItem("SteelIngot"), 1), TimeUtilities.GetMinutesFromTime(0, 12, 0), new StatCost(), false);

            this.recipesByObjectName["Furnace"].Add("Iron Bar", furnace_steelIngot);

            VanillaRecipe furnace_brassIngot = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.CopperBar,1),1 },
                {ModCore.ObjectManager.GetItem("AluminumIngot"),1},
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1}
            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.GetItem("BrassIngot"), 1), TimeUtilities.GetMinutesFromTime(0, 6, 0), new StatCost(), false);

            this.recipesByObjectName["Furnace"].Add("Aluminum Ingot", furnace_brassIngot);

            VanillaRecipe furnace_bronzeIngot = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.CopperBar,1),1 },
                {ModCore.ObjectManager.GetItem("TinIngot"),1},
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1}
            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.GetItem("BronzeIngot"), 1), TimeUtilities.GetMinutesFromTime(0, 8, 0), new StatCost(), false);

            this.recipesByObjectName["Furnace"].Add("Tin Ingot", furnace_bronzeIngot);

            VanillaRecipe furnace_electrumIngot = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.GoldBar,1),1 },
                {ModCore.ObjectManager.GetItem("SilverIngot"),1},
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1}
            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.GetItem("ElectrumIngot"), 1), TimeUtilities.GetMinutesFromTime(0, 12, 0), new StatCost(), false);

            this.recipesByObjectName["Furnace"].Add("Silver Ingot", furnace_electrumIngot);

            VanillaRecipe furnace_glass = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1},
                {ModCore.ObjectManager.resources.getResource("Sand"),5}

            }, new KeyValuePair<Item, int>(ModCore.ObjectManager.resources.getResource("Glass"),1), TimeUtilities.GetMinutesFromTime(0, 1, 0), new StatCost(), false);
            this.recipesByObjectName["Furnace"].Add("Sand", furnace_glass);

            ///Sand recipes here.
            VanillaRecipe furnace_bauxiteSand = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1},
                {ModCore.ObjectManager.resources.getResource("BauxiteSand"),5}

           }, new KeyValuePair<Item, int>(ModCore.ObjectManager.GetItem("AluminumIngot"), 1), TimeUtilities.GetMinutesFromTime(0, 1, 30), new StatCost(), false);
            this.recipesByObjectName["Furnace"].Add("Bauxite Sand", furnace_bauxiteSand);

            VanillaRecipe furnace_copperSand = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1},
                {ModCore.ObjectManager.resources.getResource("CopperSand"),5}

           }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.CopperBar,1),1), TimeUtilities.GetMinutesFromTime(0, 0, 30), new StatCost(), false);
            this.recipesByObjectName["Furnace"].Add("Copper Sand", furnace_copperSand);

            VanillaRecipe furnace_ironSand = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1},
                {ModCore.ObjectManager.resources.getResource("IronSand"),5}

           }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.IronBar, 1), 1), TimeUtilities.GetMinutesFromTime(0, 2, 00), new StatCost(), false);
            this.recipesByObjectName["Furnace"].Add("Iron Sand", furnace_ironSand);

            VanillaRecipe furnace_goldSand = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1},
                {ModCore.ObjectManager.resources.getResource("GoldSand"),5}

           }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.GoldBar, 1), 1), TimeUtilities.GetMinutesFromTime(0, 5, 0), new StatCost(), false);
            this.recipesByObjectName["Furnace"].Add("Gold Sand", furnace_goldSand);

            VanillaRecipe furnace_iridiumSand = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1},
                {ModCore.ObjectManager.resources.getResource("IridiumSand"),5}

           }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.IridiumBar, 1), 1), TimeUtilities.GetMinutesFromTime(0, 8, 0), new StatCost(), false);
            this.recipesByObjectName["Furnace"].Add("Iridium Sand", furnace_iridiumSand);

            VanillaRecipe furnace_leadSand = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1},
                {ModCore.ObjectManager.resources.getResource("LeadSand"),5}

           }, new KeyValuePair<Item, int>(ModCore.ObjectManager.resources.getResource("LeadIngot"),1), TimeUtilities.GetMinutesFromTime(0, 2, 0), new StatCost(), false);
            this.recipesByObjectName["Furnace"].Add("Lead Sand", furnace_leadSand);

            VanillaRecipe furnace_silverSand = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1},
                {ModCore.ObjectManager.resources.getResource("SilverSand"),5}

           }, new KeyValuePair<Item, int>(ModCore.ObjectManager.resources.getResource("SilverIngot"), 1), TimeUtilities.GetMinutesFromTime(0, 3, 0), new StatCost(), false);
            this.recipesByObjectName["Furnace"].Add("Silver Sand", furnace_silverSand);

            VanillaRecipe furnace_tinSand = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1},
                {ModCore.ObjectManager.resources.getResource("TinSand"),5}

           }, new KeyValuePair<Item, int>(ModCore.ObjectManager.resources.getResource("TinIngot"), 1), TimeUtilities.GetMinutesFromTime(0, 0, 50), new StatCost(), false);
            this.recipesByObjectName["Furnace"].Add("Tin Sand", furnace_tinSand);

            VanillaRecipe furnace_titaniumSand = new VanillaRecipe(new Dictionary<Item, int>()
            {
                {new StardewValley.Object((int)Enums.SDVObject.Coal,5),1},
                {ModCore.ObjectManager.resources.getResource("TitaniumSand"),5}

           }, new KeyValuePair<Item, int>(ModCore.ObjectManager.resources.getResource("TitaniumIngot"), 1), TimeUtilities.GetMinutesFromTime(0, 4, 0), new StatCost(), false);
            this.recipesByObjectName["Furnace"].Add("Titanium Sand", furnace_titaniumSand);
        }

        /// <summary>
        /// Trys to get a recipe list for a machine based off of the SDV Machine Name.
        /// </summary>
        /// <param name="Machine"></param>
        /// <returns></returns>
        public Dictionary<string,VanillaRecipe> GetRecipesForNamedRecipeBook(StardewValley.Object Machine)
        {
            if (this.recipesByObjectName.ContainsKey(Machine.Name))
            {
                return this.recipesByObjectName[Machine.Name];
            }
            else
            {
                return null;
            }
        }

        public bool DoesARecipeExistForHeldObjectName(StardewValley.Object Machine)
        {
            if (Game1.player.ActiveObject == null) return false;

            Dictionary<string, VanillaRecipe> recipes = this.GetRecipesForNamedRecipeBook(Machine);
            if (recipes == null) return false;

            if (recipes.ContainsKey(Game1.player.ActiveObject.Name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Trys to get a vanilla recipe from the recipe book related to what the player is looking at and 
        /// </summary>
        /// <param name="Machine"></param>
        /// <returns></returns>
        public VanillaRecipe GetVanillaRecipeFromHeldObjectName(StardewValley.Object Machine)
        {
            if (Game1.player.ActiveObject == null) return null;

            Dictionary<string, VanillaRecipe> recipes = this.GetRecipesForNamedRecipeBook(Machine);
            if (recipes == null) return null;

            if (recipes.ContainsKey(Game1.player.ActiveObject.Name))
            {
                return recipes[Game1.player.ActiveObject.Name];
            }
            else
            {
                return null;
            }
        }

        public bool TryToCraftRecipe(StardewValley.Object Machine)
        {
            if (this.DoesARecipeExistForHeldObjectName(Machine))
            {
                VanillaRecipe rec = this.GetVanillaRecipeFromHeldObjectName(Machine);
                bool crafted=rec.craft(Machine);
                if(crafted)this.playCraftingSound(Machine);
                return crafted;
            }
            else
            {
                //ModCore.log("No recipe!");
                return false;
            }
        }

        /// <summary>
        /// Trys to play any additional sounds associated with crafting.
        /// </summary>
        /// <param name="Machine"></param>
        public void playCraftingSound(StardewValley.Object Machine)
        {
            if (Machine.Name == "Furnace")
            {
                Game1.playSound("furnace");
            }
        }
       
    }
}
