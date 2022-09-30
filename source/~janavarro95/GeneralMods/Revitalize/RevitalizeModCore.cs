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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using xTile.Dimensions;
using Omegasis.Revitalize.Framework.Utilities;
using Omegasis.Revitalize.Framework.Managers;
using Omegasis.Revitalize.Framework.Configs;
using Omegasis.Revitalize.Framework.Constants.ItemIds.Objects;
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.Environment;
using Omegasis.Revitalize.Framework.Hacks;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.SaveData;
using Omegasis.Revitalize.Framework.World;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Shops;
using Omegasis.Revitalize.Framework.Constants.ItemIds.Items;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Graphics;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Content;
using Omegasis.Revitalize.Framework.Constants.ItemIds.Items.BlueprintIds;

namespace Omegasis.Revitalize
{

    // TODO:
    /*
    // -Make this mod able to load content packs for easier future modding
    //
    //  -Multiple Lights On Object
    //  -Illumination Colors
    //  Furniture:
    //      -rugs 
    //      -tables
    //      -lamps
    //      -dressers/other storage containers 
    //      -fun interactables
    //          -Arcade machines
    //      -More crafting tables 
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
    */

    public class RevitalizeModCore : Mod, IAssetEditor
    {

        public static RevitalizeModCore Instance;

        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static IManifest Manifest;

        public static PlayerInfo playerInfo;

        public static ConfigManager Configs;

        public static SaveDataManager SaveDataManager;



        public static ModContentManager ModContentManager;

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            ModHelper = helper;
            ModMonitor = this.Monitor;
            Manifest = this.ModManifest;
            ModContentManager = new ModContentManager();
            Configs = new ConfigManager();

            ModContentManager.initializeModContent(this.ModManifest);

            //Adds in event handling for the mod.
            ModHelper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            ModHelper.Events.GameLoop.SaveCreated += this.GameLoop_SaveCreated;

            ModHelper.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
            ModHelper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;
            ModHelper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;

            ModHelper.Events.Player.Warped +=ModContentManager.objectManager.resources.OnPlayerLocationChanged;
            ModHelper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
            ModHelper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;

            ModHelper.Events.Input.ButtonPressed += ObjectInteractionHacks.Input_CheckForObjectInteraction;

            ModHelper.Events.Display.RenderedWorld += ObjectInteractionHacks.Render_RenderCustomObjectsHeldInMachines;
            //ModHelper.Events.Display.Rendered += MenuHacks.EndOfDay_OnMenuChanged;
            ModHelper.Events.Display.MenuChanged += ShopUtilities.OnNewMenuOpened;

            ModHelper.Events.Display.MenuChanged += ModContentManager.mailManager.onNewMenuOpened ;
            //ModHelper.Events.GameLoop.Saved += MenuHacks.EndOfDay_CleanupForNewDay;
            ModHelper.Events.Input.ButtonPressed += ObjectInteractionHacks.ResetNormalToolsColorOnLeftClick;

            ModHelper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;


            SaveDataManager = new SaveDataManager();
            playerInfo = new PlayerInfo();
        }

        ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~///
        ///                     Initialize Mod Content                     ///
        ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//


        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            RevitalizeModCore.log("Hello world!");

            ModContentManager.loadContentOnGameLaunched();

            Serializer.SerializeTypesForXMLUsingSpaceCore();
        }

        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
        }



        /// <summary>
        /// What happens when a new day starts.
        /// </summary>
        /// <param name="senderm"></param>
        /// <param name="e"></param>
        private void GameLoop_DayStarted(object senderm, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            ModContentManager.objectManager.resources.DailyResourceSpawn(senderm, e);
            ShopUtilities.OnNewDay(senderm, e);
            ModContentManager.mailManager.tryToAddMailToMailbox();
        }

        /// <summary>
        /// Called when the day is ending. At this point the save data should all be saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameLoop_DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            SaveDataManager.save();
        }

        private void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e)
        {
            SaveDataManager.loadOrCreateSaveData();
            //ModContentManager.mailManager.tryToAddMailToMailbox();



            /*
            //HACKS
            //Game1.player.Money = 100_000;

            Game1.player.addItemsByMenuIfNecessary(new List<Item>()
            {


                            ObjectManager.getItem(Machines.ElectricFurnace),
                            ObjectManager.getItem(Machines.NuclearFurnace),
                            ObjectManager.getItem(Machines.MagicalFurnace),
                            ObjectManager.getItem(Enums.SDVObject.CopperOre,999),
                            ObjectManager.getItem(Enums.SDVObject.BatteryPack,999),
                            ObjectManager.getItem(MiscItemIds.RadioactiveFuel,999),
                            ObjectManager.getItem(Enums.SDVBigCraftable.Chest,1),

              //ObjectManager.getItem(Enums.SDVBigCraftable.Furnace),
            }) ;
            */
            Game1.player.addItemToInventoryBool(ModContentManager.objectManager.getItem(CraftingStations.WorkStation_Id));
            Game1.player.addItemToInventoryBool(ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.Workbench_AnvilCraftingRecipeBlueprint));
            Game1.player.addItemToInventoryBool(ModContentManager.objectManager.getItem(Enums.SDVObject.IronBar, 20));

            Framework.World.WorldUtilities.WorldUtility.InitializeGameWorld();

        }

        private void GameLoop_SaveCreated(object sender, StardewModdingAPI.Events.SaveCreatedEventArgs e)
        {
            SaveDataManager.loadOrCreateSaveData();
            ModContentManager.mailManager.tryToAddMailToMailbox();
            Framework.World.WorldUtilities.WorldUtility.InitializeGameWorld();
        }


        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            DarkerNight.SetDarkerColor();
            playerInfo.update();
        }

        private void GameLoop_TimeChanged(object sender, StardewModdingAPI.Events.TimeChangedEventArgs e)
        {
            DarkerNight.CalculateDarkerNightColor();
        }

        public static void logWarning(object message)
        {
            ModMonitor.Log(message.ToString(), LogLevel.Warn);
        }

        /// <summary>
        ///Logs information to the console.
        /// </summary>
        /// <param name="message"></param>
        public static void log(object message, bool StackTrace = true)
        {
            if (StackTrace)
                ModMonitor.Log(message.ToString() + " " + getFileDebugInfo());
            else
                ModMonitor.Log(message.ToString());
        }

        public static void logWithFullStackTrace(object message)
        {
            ModMonitor.Log(message.ToString() + " " + getFileDebugInfo(true));
        }

        public static void log(object message, bool StackTrace, params string[] strs)
        {
            message = string.Format(message.ToString(),strs);
            if (StackTrace)
                ModMonitor.Log(message.ToString() + " " + getFileDebugInfo());
            else
                ModMonitor.Log(message.ToString());
        }

        public static string getFileDebugInfo(bool fullStackTrace=false)
        {
            if (fullStackTrace)
            {
                return new System.Diagnostics.StackTrace(true).ToString();
            }
            else
            {
                string currentFile = new System.Diagnostics.StackTrace(true).GetFrame(2).GetFileName();
                int currentLine = new System.Diagnostics.StackTrace(true).GetFrame(2).GetFileLineNumber();
                return currentFile + " line:" + currentLine;
            }

        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (ModContentManager.mailManager != null)
            {
                return ModContentManager.mailManager.canEditAsset(asset);
            }
            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (ModContentManager.mailManager != null)
            {
                ModContentManager.mailManager.editMailAsset(asset);
            }

        }
    }
}
