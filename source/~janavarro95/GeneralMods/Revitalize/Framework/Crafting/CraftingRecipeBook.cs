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
using Microsoft.Xna.Framework;
using Revitalize.Framework.Menus;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Objects.Machines;
using Revitalize.Framework.Utilities;
using StardewValley;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;

namespace Revitalize.Framework.Crafting
{
    public class CraftingRecipeBook
    {
        /// <summary>
        /// Organizes crafting recipes by group. So a workbench would have a workbench crafting book, and anvil has different recipes, etc.
        /// </summary>
        public static Dictionary<string, CraftingRecipeBook> CraftingRecipesByGroup;


        /// <summary>
        /// All of the crafting recipes contained by this crafting list.
        /// </summary>
        public Dictionary<string, UnlockableCraftingRecipe> craftingRecipes;


        /// <summary>
        /// All of the crafting tabs to be used for the menu.
        /// </summary>
        public Dictionary<string, AnimatedButton> craftingMenuTabs;

        /// <summary>
        /// Which group of crafting recipes this book belongs to.
        /// </summary>
        public string craftingGroup;

        public string defaultTab;

        public CraftingRecipeBook()
        {

        }

        public CraftingRecipeBook(string CraftingGroup)
        {
            this.craftingGroup = CraftingGroup;
            this.craftingRecipes = new Dictionary<string, UnlockableCraftingRecipe>();
            this.craftingMenuTabs = new Dictionary<string, AnimatedButton>();
        }

        /// <summary>
        /// Adds in a new crafting recipe.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Recipe"></param>
        public void addCraftingRecipe(string Name, UnlockableCraftingRecipe Recipe)
        {
            if (this.craftingRecipes.ContainsKey(Name) == false)
            {
                this.craftingRecipes.Add(Name, Recipe);
            }
            else
            {
                throw new Exception("This crafting book already contains a recipe with the same id!");
            }
        }

        /// <summary>
        /// Adds in a crafting recipe.
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Recipe"></param>
        /// <param name="Unlocked">Is this recipe already unlocked?</param>
        public void addCraftingRecipe(string Name, Recipe Recipe, bool Unlocked)
        {
            UnlockableCraftingRecipe recipe = new UnlockableCraftingRecipe(this.craftingGroup, Recipe, Unlocked);
            this.addCraftingRecipe(Name, recipe);
        }

        public void addInCraftingTab(string TabName, AnimatedButton TabSprite, bool IsDefaultTab)
        {
            if (this.craftingMenuTabs.ContainsKey(TabName))
            {
                throw new Exception("A tab with the same name already exists!");
            }
            else
            {
                this.craftingMenuTabs.Add(TabName, TabSprite);
            }
            if (IsDefaultTab)
            {
                this.defaultTab = TabName;
            }
        }

        /// <summary>
        /// Gets the crafting recipe by it's name.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public UnlockableCraftingRecipe getCraftingRecipe(string Name)
        {
            if (this.craftingRecipes.ContainsKey(Name))
            {
                return this.craftingRecipes[Name];
            }
            else return null;
        }

        /// <summary>
        /// Checks to see if a crafting recipe has been unlocked.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public bool hasUnlockedCraftingRecipe(string Name)
        {
            UnlockableCraftingRecipe recipe = this.getCraftingRecipe(Name);
            if (recipe == null) return false;
            else return recipe.hasUnlocked;
        }

        /// <summary>
        /// Unlocks the crating recipe so that it can be shown in the menu.
        /// </summary>
        /// <param name="Name"></param>
        public void unlockRecipe(string Name)
        {
            UnlockableCraftingRecipe recipe = this.getCraftingRecipe(Name);
            if (recipe == null) return;
            else recipe.unlock();
        }

        /// <summary>
        /// Opens up a crafting menu from this crafting book.
        /// </summary>
        public void openCraftingMenu()
        {
            CraftingMenuV1 menu = new Framework.Menus.CraftingMenuV1(100, 100, 400, 700, Color.White, Game1.player.Items);
            //menu.addInCraftingPageTab("Default", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f));

            foreach (KeyValuePair<string, AnimatedButton> pair in this.craftingMenuTabs)
            {
                menu.addInCraftingPageTab(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<string, UnlockableCraftingRecipe> pair in this.craftingRecipes)
            {
                if (pair.Value.hasUnlocked)
                {
                    menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(pair.Value.recipe, null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), pair.Value.whichTab);
                    //ModCore.log("Add in a crafting recipe to the menu!");
                }
                else
                {
                    ModCore.log("Recipe is locked!");
                }
            }
            menu.currentTab = this.defaultTab;
            menu.sortRecipes();
            if (Game1.activeClickableMenu == null) Game1.activeClickableMenu = menu;
        }

        public CraftingMenuV1 getCraftingMenuForMachine(int x, int y, int width, int height,ref IList<Item> Items,ref IList<Item> Output,Machine Machine)
        {
            CraftingMenuV1 menu = new Framework.Menus.CraftingMenuV1(x, y, width, height, Color.White, ref Items,ref Output,Machine);
            //menu.addInCraftingPageTab("Default", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f));

            foreach (KeyValuePair<string, AnimatedButton> pair in this.craftingMenuTabs)
            {
                menu.addInCraftingPageTab(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<string, UnlockableCraftingRecipe> pair in this.craftingRecipes)
            {
                if (pair.Value.hasUnlocked)
                {
                    menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(pair.Value.recipe, null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), pair.Value.whichTab);
                    //ModCore.log("Add in a crafting recipe to the menu!");
                }
                else
                {
                    ModCore.log("Recipe is locked!");
                }
            }
            menu.currentTab = this.defaultTab;
            menu.sortRecipes();
            return menu;
        }

        public void openCraftingMenu(int x, int y, int width, int height, ref IList<Item> items)
        {
            CraftingMenuV1 menu = new Framework.Menus.CraftingMenuV1(x, y, width, height, Color.White, items);
            //menu.addInCraftingPageTab("Default", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f));

            foreach (KeyValuePair<string, AnimatedButton> pair in this.craftingMenuTabs)
            {
                menu.addInCraftingPageTab(pair.Key, pair.Value);
            }

            foreach (KeyValuePair<string, UnlockableCraftingRecipe> pair in this.craftingRecipes)
            {
                if (pair.Value.hasUnlocked)
                {
                    menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(pair.Value.recipe, null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), pair.Value.whichTab);
                    //ModCore.log("Add in a crafting recipe to the menu!");
                }
                else
                {
                    ModCore.log("Recipe is locked!");
                }
            }
            menu.currentTab = this.defaultTab;
            menu.sortRecipes();
            if (Game1.activeClickableMenu == null) Game1.activeClickableMenu = menu;
        }

        #region
        //~~~~~~~~~~~~~~~~~~~~//
        //  Static Functions  //
        //~~~~~~~~~~~~~~~~~~~~//


        public static void BeforeSave_SaveRecipeBooks(object o, StardewModdingAPI.Events.SavingEventArgs e)
        {
            if (!Directory.Exists(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"))) Directory.CreateDirectory(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"));
            string[] directories = Directory.GetDirectories(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"));
            string playerData = Path.Combine(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"), PlayerUtilities.GetUniqueCharacterString());
            string objectPath = Path.Combine(playerData, "RecipeInformation");
            Directory.CreateDirectory(objectPath);
            string[] objectFiles = Directory.GetFiles(objectPath);

            foreach (KeyValuePair<string, CraftingRecipeBook> book in CraftingRecipeBook.CraftingRecipesByGroup)
            {
                string recipePath = Path.Combine(objectPath, book.Key + ".json");
                ModCore.Serializer.Serialize(recipePath, book.Value);
            }
        }

        public static void AfterLoad_LoadRecipeBooks(object o, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            if (!Directory.Exists(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"))) Directory.CreateDirectory(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"));
            string[] directories = Directory.GetDirectories(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"));
            string playerData = Path.Combine(Path.Combine(Revitalize.ModCore.ModHelper.DirectoryPath, "SaveData"), PlayerUtilities.GetUniqueCharacterString());
            string objectPath = Path.Combine(playerData, "RecipeInformation");
            Directory.CreateDirectory(objectPath);
            string[] objectFiles = Directory.GetFiles(objectPath);
            foreach (string file in objectFiles)
            {
                CraftingRecipeBook book = ModCore.Serializer.Deserialize<CraftingRecipeBook>(file);
                string fileName = Path.GetFileNameWithoutExtension(file);
                CraftingRecipeBook.CraftingRecipesByGroup.Add(fileName, book);
            }

            InitializeRecipeBooks();

            
            for(int bookIndex=0;bookIndex<CraftingRecipesByGroup.Count;bookIndex++)
            {

                KeyValuePair<string, CraftingRecipeBook> pair = CraftingRecipesByGroup.ElementAt(bookIndex);
                for(int recipeIndex=0;recipeIndex<pair.Value.craftingRecipes.Count;recipeIndex++)
                {
                    KeyValuePair<string, UnlockableCraftingRecipe> recipe = pair.Value.craftingRecipes.ElementAt(recipeIndex);
                    for (int i = 0; i < recipe.Value.recipe.ingredients.Count; i++)
                    {
                        if (recipe.Value.recipe.ingredients[i].item is MultiTiledObject)
                        {
                            //ModCore.log("Found a multi tiled object as an output!");
                            //ModCore.log("Found a multi tiled object!");
                            Type t = recipe.Value.recipe.ingredients[i].item.GetType();
                            string id = (recipe.Value.recipe.ingredients[i].item as MultiTiledObject).info.id;
                            recipe.Value.recipe.ingredients[i].item = ModCore.ObjectManager.getItemByIDAndType(id, t);
                        }
                    }
                    for (int i = 0; i < recipe.Value.recipe.outputs.Count; i++)
                    {
                        if (recipe.Value.recipe.outputs[i].item is MultiTiledObject)
                        {
                            //ModCore.log("Found a multi tiled object as an output!");
                            //ModCore.log("Found a multi tiled object!");
                            Type t = recipe.Value.recipe.outputs[i].item.GetType();
                            string id = (recipe.Value.recipe.outputs[i].item as MultiTiledObject).info.id;
                            recipe.Value.recipe.outputs[i].item = ModCore.ObjectManager.getItemByIDAndType(id, t);

                            //ModCore.log("Components are: "+(recipe.Value.recipe.outputs[i].item as MultiTiledObject).objects.Count);
                        }
                    }
                }
            }

            
        }

        private static void InitializeRecipeBooks()
        {

            //~~~~~~~~~~~~~~~~~~~//
            // Workbench Recipes //
            //~~~~~~~~~~~~~~~~~~~//
            #region
            CraftingRecipeBook WorkbenchRecipes = new CraftingRecipeBook("Workbench");
            WorkbenchRecipes.addInCraftingTab("Default", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f),true);
            WorkbenchRecipes.addInCraftingTab("Furniture", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Furniture Tab", new Vector2(100 + 48, 100 + (24 * 4)*2), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f), false);

            WorkbenchRecipes.addCraftingRecipe("Anvil", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
                {
                    //Inputs here
                   new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteelIngot"),20)
                }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("Anvil"), 1)), true));
            WorkbenchRecipes.addCraftingRecipe("Pickaxe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Stone,20),20),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Wood,10),10)
            },new CraftingRecipeComponent(new StardewValley.Tools.Pickaxe() {UpgradeLevel=0},1)),true));
            WorkbenchRecipes.addCraftingRecipe("Axe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Stone,20),20),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Wood,10),10)
            }, new CraftingRecipeComponent(new StardewValley.Tools.Axe() { UpgradeLevel = 0 }, 1)), true));
            WorkbenchRecipes.addCraftingRecipe("Hoe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Stone,20),20),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Wood,10),10)
            }, new CraftingRecipeComponent(new StardewValley.Tools.Hoe() { UpgradeLevel = 0 }, 1)), true));
            WorkbenchRecipes.addCraftingRecipe("Watering Can", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Stone,20),20),
            }, new CraftingRecipeComponent(new StardewValley.Tools.WateringCan() { UpgradeLevel = 0 }, 1)), true));
            WorkbenchRecipes.addCraftingRecipe("Copper Wire", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.CopperBar,1),1),
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("CopperWire"),2),null,0),true));
            WorkbenchRecipes.addCraftingRecipe("Alloy Furnace", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Clay,20),10),
                new CraftingRecipeComponent(ModCore.ObjectManager.resources.getResource("Sand"), 10)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("AlloyFurnace"), 1), null, 0), true));
            WorkbenchRecipes.addCraftingRecipe("Sand Box", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Wood,100),100),
                new CraftingRecipeComponent(ModCore.ObjectManager.resources.getResource("Sand"), 25)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SandBox"), 1), null, 0), true));
            WorkbenchRecipes.addCraftingRecipe("Battery Bin", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Wood,100),100),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("ElectrumIngot"),10)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("BatteryBin"), 1), null, 0), true));
            WorkbenchRecipes.addCraftingRecipe("Capacitor", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Wood,50),50),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.CopperBar,10),10)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("Capacitor"), 1), null, 0), true));
            WorkbenchRecipes.addCraftingRecipe("Charging Station", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Wood,100),100),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,10),10),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("CopperWire"), 20),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("Capacitor"), 1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("ChargingStation"), 1), null, 0), true));

            WorkbenchRecipes.addCraftingRecipe("Oak Chair", new UnlockableCraftingRecipe("Furniture", new Recipe(new List<CraftingRecipeComponent>(),new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("Oak Chair"),1),new StatCost(0,0,100,0),0), true));

            if (CraftingRecipesByGroup.ContainsKey(WorkbenchRecipes.craftingGroup))
            {
                foreach(KeyValuePair<string,AnimatedButton> pair in WorkbenchRecipes.craftingMenuTabs)
                {
                    if (CraftingRecipesByGroup[WorkbenchRecipes.craftingGroup].craftingMenuTabs.ContainsKey(pair.Key))
                    {

                    }
                    else
                    {
                        CraftingRecipesByGroup[WorkbenchRecipes.craftingGroup].craftingMenuTabs.Add(pair.Key, pair.Value);
                    }
                }
                foreach(KeyValuePair<string, UnlockableCraftingRecipe> recipe in WorkbenchRecipes.craftingRecipes)
                {
                    if (CraftingRecipesByGroup[WorkbenchRecipes.craftingGroup].craftingRecipes.ContainsKey(recipe.Key))
                    {

                    }
                    else
                    {
                        CraftingRecipesByGroup[WorkbenchRecipes.craftingGroup].craftingRecipes.Add(recipe.Key, recipe.Value); //Add in new recipes automatically without having to delete the old crafting recipe book.
                    }
                }
            }
            else
            {
                CraftingRecipesByGroup.Add("Workbench", WorkbenchRecipes);
            }
            #endregion

            //~~~~~~~~~~~~~~~~~~//
            //   Anvil Recipes  //
            //~~~~~~~~~~~~~~~~~~//

            CraftingRecipeBook AnvilRecipes = new CraftingRecipeBook("Anvil");
            AnvilRecipes.addInCraftingTab("Default", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f), true);

            //~~~~~~~~~~~~~~~~~~~~~~~//
            //Alternate Vanilla Tools//
            //~~~~~~~~~~~~~~~~~~~~~~~//
            #region
            //Watering Cans
            AnvilRecipes.addCraftingRecipe("Bronze Watering Can", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("BronzeIngot",15),15),
                new CraftingRecipeComponent(new StardewValley.Tools.WateringCan(){ UpgradeLevel=0},1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("BronzeWateringCan"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Hardened Watering Can", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteelIngot",10),10),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("LeadIngot",5),5),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("BronzeWateringCan"),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("HardenedWateringCan"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Titanium Watering Can", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("TitaniumIngot",15),15),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("HardenedWateringCan"),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("TitaniumWateringCan"), 1)), true));

            //Pickaxes
            AnvilRecipes.addCraftingRecipe("Bronze Pickaxe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("BronzeIngot",15),15),
                new CraftingRecipeComponent(new StardewValley.Tools.Pickaxe(){ UpgradeLevel=0},1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("BronzePickaxe"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Hardened Pickaxe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteelIngot",10),10),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("LeadIngot",5),5),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("BronzePickaxe"),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("HardenedPickaxe"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Titanium Pickaxe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("TitaniumIngot",15),15),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("HardenedPickaxe"),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("TitaniumPickaxe"), 1)), true));

            //Axes
            AnvilRecipes.addCraftingRecipe("Bronze Axe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("BronzeIngot",15),15),
                new CraftingRecipeComponent(new StardewValley.Tools.Axe(){ UpgradeLevel=0},1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("BronzeAxe"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Hardened Axe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteelIngot",10),10),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("LeadIngot",5),5),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("BronzeAxe"),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("HardenedAxe"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Titanium Axe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("TitaniumIngot",15),15),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("HardenedAxe"),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("TitaniumAxe"), 1)), true));

            ///Plows
            AnvilRecipes.addCraftingRecipe("Bronze Hoe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("BronzeIngot",15),15),
                new CraftingRecipeComponent(new StardewValley.Tools.Hoe(){ UpgradeLevel=0},1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("BronzeHoe"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Hardened Hoe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteelIngot",10),10),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("LeadIngot",5),5),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("BronzeHoe"),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("HardenedHoe"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Titanium Hoe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("TitaniumIngot",15),15),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("HardenedHoe"),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("TitaniumHoe"), 1)), true));


            #endregion

            AnvilRecipes.addCraftingRecipe("Trash Can", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,5),5)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("TrashCan"), 1)),true));

            AnvilRecipes.addCraftingRecipe("Iron Pipe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,2),2)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("IronPipe"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Solar Panel", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,10),10),
                new CraftingRecipeComponent(ModCore.ObjectManager.resources.getResource("Glass"),4),
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SolarPanelTier1"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Solar Array", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,45),45),
                new CraftingRecipeComponent(ModCore.ObjectManager.resources.getResource("Glass"),20),
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SolarArrayTier1"), 1)), true));

            ///Alt solar array crafting recipe.
            AnvilRecipes.addCraftingRecipe("Solar Array Alt. Recipe", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,5),5),
                new CraftingRecipeComponent(ModCore.ObjectManager.resources.getResource("Glass"),4),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SolarPanelTier1"), 4)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SolarArrayTier1"), 1)), true));

            AnvilRecipes.addCraftingRecipe("LightHouse", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,5),5),
                new CraftingRecipeComponent(ModCore.ObjectManager.resources.getResource("Glass"),5),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Wood,10), 10)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("Lighthouse"), 1)), true));


            AnvilRecipes.addCraftingRecipe("Grinder", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,10),10),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteelIngot"),30),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.CopperBar,10), 10),
                new CraftingRecipeComponent(new StardewValley.Objects.Chest(true),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("Grinder"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Mining Drill V1", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteelIngot",10),10),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("BrassIngot",10),10),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.BatteryPack,1),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("MiningDrillV1"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Chainsaw V1", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteelIngot",10),10),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("BronzeIngot",10),10),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.BatteryPack,1),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetTool("ChainsawV1"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Water Pump V1", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,15),15)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("WaterPumpV1"), 1)), true));

            AnvilRecipes.addCraftingRecipe("Steam Boiler V1", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>()
            {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,25),25),
                new CraftingRecipeComponent(new StardewValley.Objects.Chest(true),1)

            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteamBoilerV1"), 1)), true));

            if (CraftingRecipesByGroup.ContainsKey(AnvilRecipes.craftingGroup))
            {
                foreach (KeyValuePair<string, AnimatedButton> pair in AnvilRecipes.craftingMenuTabs)
                {
                    if (CraftingRecipesByGroup[AnvilRecipes.craftingGroup].craftingMenuTabs.ContainsKey(pair.Key))
                    {

                    }
                    else
                    {
                        CraftingRecipesByGroup[AnvilRecipes.craftingGroup].craftingMenuTabs.Add(pair.Key, pair.Value);
                    }
                }
                foreach (KeyValuePair<string, UnlockableCraftingRecipe> recipe in AnvilRecipes.craftingRecipes)
                {
                    if (CraftingRecipesByGroup[AnvilRecipes.craftingGroup].craftingRecipes.ContainsKey(recipe.Key))
                    {

                    }
                    else
                    {
                        CraftingRecipesByGroup[AnvilRecipes.craftingGroup].craftingRecipes.Add(recipe.Key, recipe.Value); //Add in new recipes automatically without having to delete the old crafting recipe book.
                    }
                }
            }
            else
            {
                CraftingRecipesByGroup.Add("Anvil", AnvilRecipes);
            }

            //~~~~~~~~~~~~~~~~~~~~~~~//
            // Alloy Furnace Recipes //
            //~~~~~~~~~~~~~~~~~~~~~~~//
            CraftingRecipeBook AlloyFurnaceRecipes = new CraftingRecipeBook("AlloyFurnace");
            AlloyFurnaceRecipes.addInCraftingTab("Default", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f), true);


            AlloyFurnaceRecipes.addCraftingRecipe("BrassIngot", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>() {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.CopperBar,1),1),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("AluminumIngot"),1),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Coal,5),1)
            },new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("BrassIngot"), 1),null, TimeUtilities.GetMinutesFromTime(0, 3, 0)), true));

            AlloyFurnaceRecipes.addCraftingRecipe("BronzeIngot", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>() {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.CopperBar,1),1),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("TinIngot"),1),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Coal,5),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("BronzeIngot"), 1), null, TimeUtilities.GetMinutesFromTime(0, 4, 0)), true));

            AlloyFurnaceRecipes.addCraftingRecipe("SteelIngot", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>() {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.IronBar,1),1),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Coal,5),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteelIngot"), 1), null, TimeUtilities.GetMinutesFromTime(0, 6, 0)), true));

            AlloyFurnaceRecipes.addCraftingRecipe("ElectrumIngot", new UnlockableCraftingRecipe("Default", new Recipe(new List<CraftingRecipeComponent>() {
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.GoldBar,1),1),
                new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SilverIngot"),1),
                new CraftingRecipeComponent(new StardewValley.Object((int)Enums.SDVObject.Coal,5),1)
            }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("ElectrumIngot"), 1), null, TimeUtilities.GetMinutesFromTime(0, 4, 0)), true));

            if (CraftingRecipesByGroup.ContainsKey(AlloyFurnaceRecipes.craftingGroup))
            {
                foreach (KeyValuePair<string, UnlockableCraftingRecipe> recipe in AlloyFurnaceRecipes.craftingRecipes)
                {
                    if (CraftingRecipesByGroup[AlloyFurnaceRecipes.craftingGroup].craftingRecipes.ContainsKey(recipe.Key))
                    {

                    }
                    else
                    {
                        CraftingRecipesByGroup[AlloyFurnaceRecipes.craftingGroup].craftingRecipes.Add(recipe.Key, recipe.Value); //Add in new recipes automatically without having to delete the old crafting recipe book.
                    }
                }
            }
            else
            {
                CraftingRecipesByGroup.Add("AlloyFurnace", AlloyFurnaceRecipes);
            }
        }

        #endregion
    }
}
