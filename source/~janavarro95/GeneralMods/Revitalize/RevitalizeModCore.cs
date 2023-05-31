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
using Omegasis.Revitalize.Framework.Constants;
using Omegasis.Revitalize.Framework.Crafting;
using Omegasis.Revitalize.Framework.Environment;
using Omegasis.Revitalize.Framework.Hacks;
using Omegasis.Revitalize.Framework.Player;
using Omegasis.Revitalize.Framework.SaveData;
using Omegasis.Revitalize.Framework.World;
using Omegasis.Revitalize.Framework.World.WorldUtilities.Shops;
using Omegasis.Revitalize.Framework.Constants.PathConstants.Graphics;
using Omegasis.Revitalize.Framework.Constants.PathConstants;
using Omegasis.Revitalize.Framework.Content;
using Omegasis.Revitalize.Framework.Constants.Ids.Objects;
using Omegasis.Revitalize.Framework.Constants.Ids.Items;
using Omegasis.Revitalize.Framework.World.Objects.Items.Utilities;
using Netcode;
using Omegasis.Revitalize.Framework.World.Objects.Machines.Furnaces;
using Omegasis.Revitalize.Framework.World.Buildings;
using Omegasis.Revitalize.Framework.HUD;
using Omegasis.Revitalize.Framework.Constants.Ids.Items.BlueprintIds;

namespace Omegasis.Revitalize
{

    // TODO:
    /*
    // -Make this mod able to load content packs for easier future modding

    //      -Mayo Maker
    //      -Cheese Maker
            -Yogurt Maker
                   -Fruit yogurts (artisan good)
    //      -Auto fisher
    //      -Auto Preserves
    //      -Auto Keg
    //      -Auto Cask

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

    public class RevitalizeModCore : Mod
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

            //Save events.
            ModHelper.Events.GameLoop.SaveLoaded += this.GameLoop_SaveLoaded;
            ModHelper.Events.GameLoop.SaveCreated += this.GameLoop_SaveCreated;


            //Player Events.
            ModHelper.Events.Player.Warped += ModContentManager.objectManager.resources.OnPlayerLocationChanged;
            ModHelper.Events.Player.InventoryChanged += PlayerUtilities.OnItemAddedToPlayersInventory;

            //Game time change events.
            ModHelper.Events.GameLoop.TimeChanged += this.GameLoop_TimeChanged;
            ModHelper.Events.GameLoop.DayStarted += this.GameLoop_DayStarted;
            ModHelper.Events.GameLoop.DayEnding += this.GameLoop_DayEnding;

            //Input events.
            ModHelper.Events.Input.ButtonPressed += ObjectInteractionHacks.Input_CheckForObjectInteraction;
            //ModHelper.Events.GameLoop.Saved += MenuHacks.EndOfDay_CleanupForNewDay;
            ModHelper.Events.Input.ButtonPressed += ObjectInteractionHacks.ResetNormalToolsColorOnLeftClick;

            //Render events.
            ModHelper.Events.Display.RenderedWorld += ObjectInteractionHacks.Render_RenderCustomObjectsHeldInMachines;

            //Menu Events.
            ModHelper.Events.Display.MenuChanged += ShopUtilities.OnNewMenuOpened;
            ModHelper.Events.Display.MenuChanged += ModContentManager.mailManager.onNewMenuOpened;

            //Game Loop events.
            ModHelper.Events.GameLoop.GameLaunched += this.GameLoop_GameLaunched;
            ModHelper.Events.GameLoop.ReturnedToTitle += this.GameLoop_ReturnedToTitle;
            ModHelper.Events.GameLoop.UpdateTicked += this.GameLoop_UpdateTicked;


            ModHelper.Events.Content.AssetRequested += this.Content_AssetRequested;

            SaveDataManager = new SaveDataManager();
            playerInfo = new PlayerInfo();
        }



        ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~///
        ///                     Initialize Mod Content                     ///
        ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//


        private void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {

            ModContentManager.loadContentOnGameLaunched();

            Serializer.SerializeTypesForXMLUsingSpaceCore();
        }

        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {

            DimensionalStorageUnitBuilding.CachedDimensionalStorageUnitBuilding = null;

            //Do more cleanup here...

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

            //Used to also check if a player has items that would do things such as unlocking a crafting recipe when loading a day.
            PlayerUtilities.CheckForInventoryItem(Game1.player.Items);

            ModContentManager.mailManager.tryToAddAllMailToMailbox();



            this.warpToWalnutRoom();
        }

        /// <summary>
        /// Used just for testing purposes.
        /// </summary>
        private void warpToWalnutRoom()
        {
            if (false)
            {
                Game1.warpFarmer("QiNutRoom", 7, 8, 0);
                Game1.player.QiGems = 100;
            }
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

            //HACKS
            //Game1.player.Money = 100_000;

            Game1.player.ForagingLevel = 5;

            Game1.player.addItemsByMenuIfNecessary(new List<Item>()
            {
                ModContentManager.objectManager.getItem(StorageIds.LargeItemVault),
                                ModContentManager.objectManager.getItem(MachineIds.Windmill),
                                ModContentManager.objectManager.getItem(FarmingObjectIds.AutomaticTreeFarm),
              /*


                                                        ModContentManager.objectManager.getItem(StorageIds.HugeItemVault),
                                                        
                ModContentManager.objectManager.getItem(FarmingObjects.HayMaker),

                            ModContentManager.objectManager.getItem(MachineIds.ElectricAdvancedGeodeCrusher),
                            
                            ModContentManager.objectManager.getItem(Enums.SDVObject.OmniGeode,100),
                            ModContentManager.objectManager.getItem(Enums.SDVObject.BatteryPack,100),
                            ModContentManager.objectManager.getItem(Enums.SDVBigCraftable.Chest,10),
              */

            });
            //Game1.player.addItemToInventoryBool(ModContentManager.objectManager.getItem(WorkbenchBlueprintIds.Workbench_AnvilCraftingRecipeBlueprint));
            //Game1.player.addItemToInventoryBool(ModContentManager.objectManager.getItem(Enums.SDVObject.IronBar, 20));



            Framework.World.WorldUtilities.WorldUtility.InitializeGameWorld();

        }

        private void GameLoop_SaveCreated(object sender, StardewModdingAPI.Events.SaveCreatedEventArgs e)
        {
            SaveDataManager.loadOrCreateSaveData();
            ModContentManager.mailManager.tryToAddAllMailToMailbox();
            Framework.World.WorldUtilities.WorldUtility.InitializeGameWorld();
        }


        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            DarkerNight.SetDarkerColor();
            playerInfo.update();
            HudUtilities.OnGameUpdateTicked();
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
            message = string.Format(message.ToString(), strs);
            if (StackTrace)
                ModMonitor.Log(message.ToString() + " " + getFileDebugInfo());
            else
                ModMonitor.Log(message.ToString());
        }

        public static string getFileDebugInfo(bool fullStackTrace = false)
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

        private void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (ModContentManager.mailManager != null)
            {
                ModContentManager.mailManager.editMailAsset(e);
            }
        }
    }
}
