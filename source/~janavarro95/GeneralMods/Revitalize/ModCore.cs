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
using Revitalize.Framework;
using Revitalize.Framework.Configs;
using Revitalize.Framework.Crafting;
using Revitalize.Framework.Environment;
using Revitalize.Framework.Hacks;
using Revitalize.Framework.Illuminate;
using Revitalize.Framework.Menus;
using Revitalize.Framework.Minigame.SeasideScrambleMinigame;
using Revitalize.Framework.Objects;
using Revitalize.Framework.Objects.Extras;
using Revitalize.Framework.Objects.Furniture;
using Revitalize.Framework.Player;
using Revitalize.Framework.Utilities;
using StardewModdingAPI;
using StardewValley;
using StardustCore.Animations;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.MenuComponents.ComponentsV2.Buttons;
using Animation = StardustCore.Animations.Animation;

namespace Revitalize
{

    //Bugs:
    //  -Chair tops cut off objects
    // -load content MUST be enabled for the table to be placed??????
    // TODO:
    /*
    // -Make this mod able to load content packs for easier future modding
    //
    //  -Multiple Lights On Object
    //  -Illumination Colors
    //  Furniture:
    //      -rugs (done, needs factory info/sprite)
    //      -tables (done)
    //      -lamps (done)
    //      -chairs (done)
    //      -benches (done but needs factory info/sprite)
    //      -dressers/other storage containers (Done!)
    //      -fun interactables
    //          -Arcade machines
    //      -More crafting tables (done)
    //      -Baths (see chairs but swimming)
    //
    //  -Machines
    //      !=Energy
    //            Generators:
                  -solar
                  -burnable
                  -watermill
                  -windmill
                  -crank (costs stamina)
                  Storage:
                  -Batery Pack
             -Mini-greenhouse
                   -takes fertilizer which can do things like help crops grow or increase prodcuction yield/quality.
                   -takes crop/extended crop seeds
                   -takes sprinklers
                   -has grid (1x1, 2x2, 3x3, 4x4, 5x5) system for growing crops/placing sprinkers
                   -sprinkers auto water crops
                   -can auto harvest
                   -hover over crop to see it's info
                   -can be upgraded to grow crops from specific seasons with season stones (spring,summer, fall winter) (configurable if they are required)
                   -Add in season stone recipe

    //      -Furnace
    //      -Seed Maker
    //      -Stone Quarry
    //      -Mayo Maker
    //      -Cheese Maker
            -Yogurt Maker
                   -Fruit yogurts (artisan good)
    //      -Auto fisher
    //      -Auto Preserves
    //      -Auto Keg
    //      -Auto Cask
    //      -Calcinator (oil+stone: produces titanum?)
    //  -Materials
    //      -Tin/Bronze/Alluminum/Silver?Platinum/Etc (all but platinum: may add in at a later date)
            -titanium (d0ne)
            -Alloys!
                -Brass (done)
                -Electrum (done)
                -Steel (done)
                -Bronze (done)
            -Mythrill
            
            -Star Metal
            -Star Steel
            -Cobalt
        -Liquids
            -oil
            -water
            -coal
            -juice???
            -lava?

        -Dyes!
            -Dye custom objects certain colors!
            -Rainbow Dye -(set a custom object to any color)
            -red, green, blue, yellow, pink, etc
            -Make dye from flowers/coal/algee/minerals/gems (black), etc
                -soapstone (washes off dye)
                -Lunarite (white)
        Dye Machine
            -takes custom object and dye
            -dyes the object
            -can use water to wash off dye.
            -maybe dye stardew valley items???
            -Dyed Wool (Artisan good)

        Menus:
    //  -Crafting Menu
    //  -Item Grab Menu (Extendable) (Done!)
    //   -Yes/No Dialogue Box
    //   -Multi Choice dialogue box


    //  -Gift Boxes

    //  Magic!
    //      -Alchemy Bags
    //      -Transmutation
    //      -Effect Crystals
    //      -Spell books
    //      -Potions!
    //      -Magic Meter
    //      -Connected chests (3 digit color code) much like Project EE2 from MC
    //
    //
    //  -Food
            -multi flavored sodas

    //  -Bigger chests
    //
    //  Festivals
    //      -Firework festival
    //      -Horse Racing Festival
            -Valentines day (Maybe make this just one holiday)
                -Spring. Male to female gifts.
                -Winter. Female to male gifts. 
    //  Stargazing???
    //      -Moon Phases+DarkerNight
    //  Bigger/Better Museum?
    // 
    //  Equippables!
    //      -accessories that provide buffs/regen/friendship
    //      -braclets/rings/broaches....more crafting for these???
    //      
    //  Music???
    //      -IDK maybe add in instruments???
    //      
    //  More buildings????
    //  
    //  More Animals???
    //  
    //  Readable Books?
    //  
    //  Custom NPCs for shops???
    //
    //  Minigames:
    //      Frisbee Minigame?
    //      HorseRace Minigame/Betting?
    //  
    //  Locations:
            -Make extra bus stop sign that travels between new towns/locations.
    //      -Small Island Home?
    //      -New town inspired by FOMT;Mineral Town/The Valley HM DS
    //
    //  More crops
    //      -RF Crops
    //      -HM Crops
    //
    //  More monsters
    //  -boss fights
    //
    //  More dungeons??

    //  More NPCS?

        Accessories
        (recover hp/stamina,max hp,more friendship ,run faster, take less damage, etc)
            -Neckalces
            -Broaches
            -Earings
            -Pendants


        make chat notification when people are sleeping
    */

    public class ModCore : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static IManifest Manifest;

        /// <summary>
        /// Keeps track of custom objects.
        /// </summary>
        public static ObjectManager ObjectManager;

        /// <summary>
        /// Keeps track of all of the extra object groups.
        /// </summary>
        public static Dictionary<string, MultiTiledObject> ObjectGroups;

        public static PlayerInfo playerInfo;

        public static Serializer Serializer;

        public static Dictionary<GameLocation, MultiTiledObject> ObjectsToDraw;
        public static VanillaRecipeBook VanillaRecipeBook;

        public static Dictionary<Guid, CustomObject> CustomObjects;
        public static Dictionary<Guid, Item> CustomItems;

        public static ConfigManager Configs;
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = this.Monitor;
            Manifest = this.ModManifest;
            Configs = new ConfigManager();

            this.createDirectories();
            this.initailizeComponents();
            Serializer = new Serializer();
            playerInfo = new PlayerInfo();
            CustomObjects = new Dictionary<Guid, CustomObject>();
            CustomItems = new Dictionary<Guid, Item>();

            //Loads in textures to be used by the mod.
            this.loadInTextures();

            //Loads in objects to be use by the mod.
            ObjectGroups = new Dictionary<string, MultiTiledObject>();
            ObjectManager = new ObjectManager(Manifest);
            ObjectsToDraw = new Dictionary<GameLocation, MultiTiledObject>();

            //Adds in event handling for the mod.
            ModHelper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            ModHelper.Events.GameLoop.SaveLoaded += CraftingRecipeBook.AfterLoad_LoadRecipeBooks;
            ModHelper.Events.GameLoop.Saving += CraftingRecipeBook.BeforeSave_SaveRecipeBooks;

            ModHelper.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
            ModHelper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            ModHelper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;

            ModHelper.Events.Player.Warped += ObjectManager.resources.OnPlayerLocationChanged;
            ModHelper.Events.GameLoop.DayStarted += ObjectManager.resources.DailyResourceSpawn;
            ModHelper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
            ModHelper.Events.Input.ButtonPressed += ObjectInteractionHacks.Input_CheckForObjectInteraction;

            ModHelper.Events.GameLoop.DayEnding += Serializer.DayEnding_CleanUpFilesForDeletion;
            ModHelper.Events.Display.RenderedWorld += ObjectInteractionHacks.Render_RenderCustomObjectsHeldInMachines;
            //ModHelper.Events.Display.Rendered += MenuHacks.EndOfDay_OnMenuChanged;
            //ModHelper.Events.GameLoop.Saved += MenuHacks.EndOfDay_CleanupForNewDay;
            ModHelper.Events.Multiplayer.ModMessageReceived += MultiplayerUtilities.GetModMessage;
            ModHelper.Events.Input.ButtonPressed += ObjectInteractionHacks.ResetNormalToolsColorOnLeftClick;

            ModHelper.Events.Input.ButtonPressed += this.Input_ButtonPressed1;

            ModHelper.Events.Display.MenuChanged += MenuHacks.RecreateFarmhandInventory;

            ObjectManager.loadInItems();
            //Adds in recipes to the mod.
            VanillaRecipeBook = new VanillaRecipeBook();
            CraftingRecipeBook.CraftingRecipesByGroup = new Dictionary<string, CraftingRecipeBook>();
        }

        private void Input_ButtonPressed1(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.MouseLeft)
            {
                if (Game1.player != null)
                {
                    if (Game1.activeClickableMenu != null || Game1.eventUp || Game1.currentMinigame != null) return;
                    if (Game1.player.ActiveObject is CustomObject)
                    {
                        if ((Game1.player.ActiveObject as CustomObject).canBePlacedHere(Game1.player.currentLocation, Game1.currentCursorTile))
                        {
                            CustomObject o = (CustomObject)Game1.player.ActiveObject;
                            o.placementAction(Game1.currentLocation, (int)Game1.currentCursorTile.X * Game1.tileSize, (int)Game1.currentCursorTile.Y * Game1.tileSize, Game1.player);
                            //o.performObjectDropInAction(Game1.player.ActiveObject, true, Game1.player);
                            Game1.player.reduceActiveItemByOne();
                            playerInfo.justPlacedACustomObject = true;
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Loads in textures to be used by the mod.
        /// </summary>
        private void loadInTextures()
        {
            TextureManager.AddTextureManager(Manifest, "Furniture");
            TextureManager.GetTextureManager(Manifest, "Furniture").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Objects", "Furniture"));
            TextureManager.AddTextureManager(Manifest, "Machines");
            TextureManager.GetTextureManager(Manifest, "Machines").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Objects", "Machines"));
            TextureManager.AddTextureManager(Manifest, "InventoryMenu");
            TextureManager.GetTextureManager(Manifest, "InventoryMenu").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Menus", "InventoryMenu"));
            TextureManager.AddTextureManager(Manifest, "Resources.Ore");
            TextureManager.GetTextureManager(Manifest, "Resources.Ore").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Objects", "Resources", "Ore"));
            TextureManager.AddTextureManager(Manifest, "Items.Resources.Misc");
            TextureManager.GetTextureManager(Manifest, "Items.Resources.Misc").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Items", "Resources", "Misc"));
            TextureManager.AddTextureManager(Manifest, "Items.Resources.Ore");
            TextureManager.GetTextureManager(Manifest, "Items.Resources.Ore").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Items", "Resources", "Ore"));
            TextureManager.AddTextureManager(Manifest, "Tools");
            TextureManager.GetTextureManager(Manifest, "Tools").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Items", "Tools"));

            TextureManager.AddTextureManager(Manifest, "Menus");
            TextureManager.GetTextureManager(Manifest, "Menus").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Menus", "Misc"));

            TextureManager.AddTextureManager(Manifest, "Menus.EnergyMenu");
            TextureManager.GetTextureManager(Manifest, "Menus.EnergyMenu").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Menus", "EnergyMenu"));

            TextureManager.AddTextureManager(Manifest, "CraftingMenu");
            TextureManager.GetTextureManager(Manifest, "CraftingMenu").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Menus", "CraftingMenu"));

            TextureManager.AddTextureManager(Manifest, "HUD");
            TextureManager.GetTextureManager(Manifest, "HUD").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "HUD"));

            TextureManager.AddTextureManager(Manifest, "Objects.Crafting");
            TextureManager.GetTextureManager(Manifest, "Objects.Crafting").searchForTextures(ModHelper, this.ModManifest, Path.Combine("Content", "Graphics", "Objects", "Crafting"));
        }

        private void Input_ButtonPressed(object sender, StardewModdingAPI.Events.ButtonPressedEventArgs e)
        {
            /*
            if(e.Button== SButton.U)
            {
                Game1.currentMinigame = new Revitalize.Framework.Minigame.SeasideScrambleMinigame.SeasideScramble();
            }
            */
            if (e.Button == SButton.U)
            {
                CraftingMenuV1 craft = new CraftingMenuV1(100, 100, 600, 800, Color.White, Game1.player.Items.ToList());
                craft.addInCraftingPageTab("Default", new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(ModCore.Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f));
                craft.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new List<CraftingRecipeComponent>()
                {
                    //Inputs here
                   new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("SteelIngot"),20)
                }, new CraftingRecipeComponent(ModCore.ObjectManager.GetItem("Anvil"), 1)), null, new Vector2(), new Rectangle(0, 0, 32, 32), 1f, false, Color.White), "Default");
                craft.currentTab = "Default";
                craft.sortRecipes();
                Game1.activeClickableMenu = craft;
            }
            /*
            if (e.Button == SButton.Y)
            {
                //Game1.activeClickableMenu = new ItemGrabMenu(Game1.player.Items,false,true, new InventoryMenu.highlightThisItem(InventoryMenu.highlightAllItems),);
                List<Item> newItems = new List<Item>()
                {
                    new StardewValley.Object(184,10)
                };

                Game1.activeClickableMenu = new Revitalize.Framework.Menus.InventoryTransferMenu(100, 100, 500, 500, newItems, 36);
            }
            */

            if (e.Button == SButton.U)
            {
                /*
                CraftingMenuV1 menu= new Framework.Menus.CraftingMenuV1(100, 100, 400, 700, Color.White, Game1.player.Items);
                menu.addInCraftingPageTab("Default",new AnimatedButton(new StardustCore.Animations.AnimatedSprite("Default Tab", new Vector2(100 + 48, 100 + (24 * 4)), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Menus", "MenuTabHorizontal"), new Animation(0, 0, 24, 24)), Color.White), new Rectangle(0, 0, 24, 24), 2f));

                menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new Dictionary<Item, int>()
                {
                    //Inputs here
                    {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1 },
                }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.Wool, 1), 1)), null, new Vector2(), new Rectangle(0,0,16,16), 4f, true, Color.White),"Default");
                menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new Dictionary<Item, int>()
                {
                    //Inputs here
                    {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1 },
                }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.FairyRose, 1), 1)), null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), "Default");
                menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new Dictionary<Item, int>()
                {
                    //Inputs here
                    {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1 },
                }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.PrismaticShard, 1), 1)), null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), "Default");
                menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new Dictionary<Item, int>()
                {
                    //Inputs here
                    {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1 },
                }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.OakResin, 1), 1)), null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), "Default");
                menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new Dictionary<Item, int>()
                {
                    //Inputs here
                    {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1 },
                }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.ChocolateCake, 1), 1)), null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), "Default");
                menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new Dictionary<Item, int>()
                {
                    //Inputs here
                    {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1 },
                }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.QualitySprinkler, 1), 1)), null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), "Default");
                menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new Dictionary<Item, int>()
                {
                    //Inputs here
                    {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1 },
                }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.JackOLantern, 1), 1)), null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), "Default");
                menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new Dictionary<Item, int>()
                {
                    //Inputs here
                    {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1 },
                }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.WildPlum, 1), 1)), null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), "Default");
                menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new Dictionary<Item, int>()
                {
                    //Inputs here
                    {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1 },
                }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.Egg, 1), 1)), null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), "Default");
                menu.addInCraftingRecipe(new Framework.Menus.MenuComponents.CraftingRecipeButton(new Recipe(new Dictionary<Item, int>()
                {
                    //Inputs here
                    {new StardewValley.Object((int)Enums.SDVObject.Coal,1),1 },
                }, new KeyValuePair<Item, int>(new StardewValley.Object((int)Enums.SDVObject.BakedFish, 1), 1)), null, new Vector2(), new Rectangle(0, 0, 16, 16), 4f, true, Color.White), "Default");


                menu.currentTab = "Default";
                menu.sortRecipes();

                if (Game1.activeClickableMenu == null) Game1.activeClickableMenu = menu;
                */
            }
        }

        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            Serializer.returnToTitle();
            ObjectManager = new ObjectManager(Manifest);
        }
        /// <summary>
        /// Must be enabled for the tabled to be placed????
        /// </summary>
        private void loadContent()
        {

            MultiTiledComponent obj = new MultiTiledComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.MultiTiledComponent.Test", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(MultiTiledComponent), Color.White), new BasicItemInformation("CoreObjectTest", "Omegasis.TEST1", "YAY FUN!", "Omegasis.Revitalize.MultiTiledComponent.Test", Color.White, -300, 0, false, 300, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "Oak Chair"), new Animation(new Rectangle(0, 0, 16, 16))), Color.White, false, null, null));
            MultiTiledComponent obj2 = new MultiTiledComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.MultiTiledComponent.Test", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(MultiTiledComponent), Color.White), new BasicItemInformation("CoreObjectTest2", "Omegasis.TEST2", "Some fun!", "Omegasis.Revitalize.MultiTiledComponent.Test", Color.White, -300, 0, false, 300, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "Oak Chair"), new Animation(new Rectangle(0, 16, 16, 16))), Color.White, false, null, null));
            MultiTiledComponent obj3 = new MultiTiledComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.MultiTiledComponent.Test", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(MultiTiledComponent), Color.White), new BasicItemInformation("CoreObjectTest3", "Omegasis.TEST3", "NoFun", "Omegasis.Revitalize.MultiTiledComponent.Test", Color.White, -300, 0, false, 100, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "Oak Chair"), new Animation(new Rectangle(0, 32, 16, 16))), Color.Red, false, null, null));


            obj3.info.lightManager.addLight(new Vector2(Game1.tileSize), new LightSource(4, new Vector2(0, 0), 2.5f, Color.Orange.Invert()), obj3);

            MultiTiledObject bigObject = new MultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.MultiTiledComponent.Test", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(MultiTiledObject), Color.White), new BasicItemInformation("MultiTest", "Omegasis.BigTiledTest", "A really big object", "Omegasis.Revitalize.MultiTiledObject", Color.Blue, -300, 0, false, 500, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(), Color.White, false, null, null));

            bigObject.addComponent(new Vector2(0, 0), obj);
            bigObject.addComponent(new Vector2(1, 0), obj2);
            bigObject.addComponent(new Vector2(2, 0), obj3);

            /*
            Recipe pie = new Recipe(new Dictionary<Item, int>()
            {
                [bigObject] = 1
            }, new KeyValuePair<Item, int>(new Furniture(3, Vector2.Zero), 1), new StatCost(100, 50, 0, 0));
            */

            ObjectManager.miscellaneous.Add("Omegasis.BigTiledTest", bigObject);


            Framework.Objects.Furniture.RugTileComponent rug1 = new RugTileComponent(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Basic.Rugs.TestRug", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(RugTileComponent), Color.White), new BasicItemInformation("Rug Tile", "Omegasis.Revitalize.Furniture.Basic.Rugs.TestRug", "A rug tile", "Rug", Color.Brown, -300, 0, false, 100, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "Oak Chair"), new Animation(new Rectangle(0, 0, 16, 16))), Color.White, true, null, null));

            Framework.Objects.Furniture.RugMultiTiledObject rug = new RugMultiTiledObject(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Basic.Rugs.TestRug", TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), typeof(RugMultiTiledObject), Color.White, false), new BasicItemInformation("Simple Rug Test", "Omegasis.Revitalize.Furniture.Basic.Rugs.TestRug", "A simple rug for testing", "Rugs", Color.Brown, -300, 0, false, 500, true, true, TextureManager.GetTexture(Manifest, "Furniture", "Oak Chair"), new AnimationManager(), Color.White, true, null, null));

            rug.addComponent(new Vector2(0, 0), rug1);

            ObjectManager.miscellaneous.Add("Omegasis.Revitalize.Furniture.Rugs.RugTest", rug);


            SeasideScramble sscGame = new SeasideScramble();
            ArcadeCabinetTile ssc1 = new ArcadeCabinetTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), typeof(ArcadeCabinetTile), Color.White), new BasicItemInformation("Seaside Scramble Arcade Game", "Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", "A arcade to play Seaside Scramble!", "Arcades", Color.LimeGreen, -300, 0, false, 100, true, true, TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "SeasideScrambleArcade"), new Animation(new Rectangle(0, 0, 16, 16)), new Dictionary<string, List<Animation>>()
            {
                {"Animated",new List<Animation>()
                {
                    new Animation(0,0,16,16,60),
                    new Animation(16,0,16,16,60)
                }
                }

            }, "Animated"), Color.White, false, null, null), new Framework.Objects.InformationFiles.Furniture.ArcadeCabinetInformation(sscGame, false));
            ArcadeCabinetTile ssc2 = new ArcadeCabinetTile(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), typeof(ArcadeCabinetTile), Color.White), new BasicItemInformation("Seaside Scramble Arcade Game", "Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", "A arcade to play Seaside Scramble!", "Arcades", Color.LimeGreen, -300, 0, false, 100, true, true, TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Furniture", "SeasideScrambleArcade"), new Animation(new Rectangle(0, 16, 16, 16)), new Dictionary<string, List<Animation>>()
            {
                {"Animated",new List<Animation>()
                {
                    new Animation(0,16,16,16,60),
                    new Animation(16,16,16,16,60)
                }
                }

            }, "Animated"), Color.White, false, null, null), new Framework.Objects.InformationFiles.Furniture.ArcadeCabinetInformation(sscGame, false));

            ArcadeCabinetOBJ sscCabinet = new ArcadeCabinetOBJ(PyTKHelper.CreateOBJData("Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), typeof(ArcadeCabinetOBJ), Color.White, true), new BasicItemInformation("Seaside Scramble Arcade Game", "Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", "A arcade to play Seaside Scramble!", "Arcades", Color.LimeGreen, -300, 0, false, 500, true, true, TextureManager.GetTexture(Manifest, "Furniture", "SeasideScrambleArcade"), new AnimationManager(), Color.White, true, null, null));
            sscCabinet.addComponent(new Vector2(0, 0), ssc1);
            sscCabinet.addComponent(new Vector2(0, 1), ssc2);


            ObjectManager.miscellaneous.Add("Omegasis.Revitalize.Furniture.Arcade.SeasideScramble", sscCabinet);

            //ModCore.log("Added in SSC!");

        }

        private void createDirectories()
        {
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Configs"));

            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics"));
            //Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics","Furniture"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics", "Furniture", "Chairs"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics", "Furniture", "Lamps"));
            Directory.CreateDirectory(Path.Combine(this.Helper.DirectoryPath, "Content", "Graphics", "Furniture", "Tables"));
        }

        /// <summary>
        /// Initialize all modular components for this mod.
        /// </summary>
        private void initailizeComponents()
        {
            DarkerNight.InitializeConfig();
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (playerInfo.justPlacedACustomObject == true) playerInfo.justPlacedACustomObject = false;
            DarkerNight.SetDarkerColor();
            playerInfo.update();
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            DarkerNight.CalculateDarkerNightColor();
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            //this.loadContent();


            Serializer.afterLoad();
            ShopHacks.AddInCustomItemsToShops();
            ObjectInteractionHacks.AfterLoad_RestoreTrackedMachines();


            // Game1.player.addItemToInventory(GetObjectFromPool("Omegasis.BigTiledTest"));
            //Game1.player.addItemToInventory(ObjectManager.getChair("Omegasis.Revitalize.Furniture.Chairs.OakChair"));

            Game1.player.addItemToInventoryBool(ObjectManager.GetItem("Workbench"));


            MultiTiledObject batteryBin = (MultiTiledObject)ModCore.ObjectManager.GetItem("BatteryBin", 1);
            batteryBin.dyeColor(Framework.Illuminate.ColorsList.Lime);

            //PickaxeExtended pick = new PickaxeExtended(new BasicItemInformation("My First Pickaxe", "Omegasis.Revitalize.Items.Tools.MyFirstPickaxe", "A testing pickaxe. Does it work?", "Tool", Color.SlateGray, 0, 0, false, 500, false, false, TextureManager.GetTexture(Manifest, "Tools", "Pickaxe"), new AnimationManager(TextureManager.GetExtendedTexture(Manifest, "Tools", "Pickaxe"), new Animation(0, 0, 16, 16)), Color.White, true, null, null),2,TextureManager.GetExtendedTexture(Manifest,"Tools","TestingPickaxeWorking"));
            Game1.player.addItemsByMenuIfNecessary(new List<Item>()
            {
                new StardewValley.Object((int)Enums.SDVObject.Coal,100),
                ModCore.ObjectManager.GetItem("SteelIngot", 20),
                ModCore.ObjectManager.GetItem("TrashCan",1),
                ModCore.ObjectManager.resources.getResource("Sand",5),
                ModCore.ObjectManager.GetItem("Anvil",1),
                ModCore.ObjectManager.GetItem("SolarPanelTier1",1),
                ModCore.ObjectManager.GetItem("SolarArrayTier1",1),
                new StardewValley.Object(Vector2.Zero,(int)Enums.SDVBigCraftable.Furnace,false),
                ModCore.ObjectManager.GetItem("CopperWire",10),
                batteryBin,
                ModCore.ObjectManager.GetItem("Capacitor",1),
                ModCore.ObjectManager.GetItem("ChargingStation",1),
                new StardewValley.Object((int)Enums.SDVObject.CopperOre,10),
                ModCore.ObjectManager.GetTool("ChainsawV1"),
                ModCore.ObjectManager.GetItem("MiningDrillMachineV1"),
                ModCore.ObjectManager.GetItem("AlloyFurnace"),
                new StardewValley.Object((int)Enums.SDVObject.IronBar,100),
                ModCore.ObjectManager.GetItem("WaterPumpV1"),
                ModCore.ObjectManager.GetItem("SteamBoilerV1"),
                ModCore.ObjectManager.GetItem("IronPipe",100),
                ModCore.ObjectManager.GetItem("SteamEngineV1"),
                ModCore.ObjectManager.GetItem("WindmillV1"),
                ModCore.ObjectManager.GetItem("WindmillV2")
            });
        }

        /*
        public static Item GetObjectFromPool(string objName)
        {
            if (customObjects.ContainsKey(objName))
            {
                CustomObject i = (CustomObject)customObjects[objName].getOne();
                return i;
            }
            else
            {
                throw new Exception("Object Key name not found: " + objName);
            }
        }
        */

        /// <summary>
        ///Logs information to the console.
        /// </summary>
        /// <param name="message"></param>
        public static void log(object message, bool StackTrace = true)
        {
            if (StackTrace)
            {
                ModMonitor.Log(message.ToString() + " " + getFileDebugInfo());
            }
            else
            {
                ModMonitor.Log(message.ToString());
            }
        }

        public static string getFileDebugInfo()
        {
            string currentFile = new System.Diagnostics.StackTrace(true).GetFrame(2).GetFileName();
            int currentLine = new System.Diagnostics.StackTrace(true).GetFrame(2).GetFileLineNumber();
            return currentFile + " line:" + currentLine;
        }

        public static bool IsNullOrDefault<T>(T argument)
        {
            // deal with normal scenarios
            if (argument == null) return true;
            if (object.Equals(argument, default(T))) return true;

            // deal with non-null nullables
            Type methodType = typeof(T);
            if (Nullable.GetUnderlyingType(methodType) != null) return false;

            // deal with boxed value types
            Type argumentType = argument.GetType();
            if (argumentType.IsValueType && argumentType != methodType)
            {
                object obj = Activator.CreateInstance(argument.GetType());
                return obj.Equals(argument);
            }

            return false;
        }
    }
}
