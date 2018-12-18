using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using StardustCore.Menus;
using StardustCore.ModInfo;
using StardustCore.NetCode;
using StardustCore.Objects;
using StardustCore.Objects.Tools;
using StardustCore.Serialization;
using StardustCore.UIUtilities;
using StardustCore.UIUtilities.SpriteFonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace StardustCore
{
    /*
     *Known issues:
     * Clients have a error on Serialization that says they run across unknown XML elements such as core objects. However, inventories for farmhands and modded objects still get serialized properly.
     * Inventories get wiped after being returned home from a festival.
         */


    public class ModCore : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static IManifest Manifest;
        public static Serialization.SerializationManager SerializationManager;
        public static UIUtilities.TextureManager TextureManager;
        public static Dictionary<string, TextureManager> TextureManagers;


        private Type lastMenuType;

        public ModConfig config;

        public bool playerJustDisconnected;

        public bool justWarped;

        public static string ContentDirectory;
        public override void Entry(IModHelper helper)
        {
            ModHelper = Helper;
            ModMonitor = Monitor;
            Manifest = ModManifest;
            //Unused MetaData information. Works in player inventory but not in chests. Besides who really care where an object is from anyways. Also doesn't work 100% like I intended since it only gets base mod object that this runs from, not extensions?

            //  StardewModdingAPI.Events.GraphicsEvents.OnPostRenderGuiEvent += Metadata.GameEvents_UpdateTick;
            //StardewModdingAPI.Events.ControlEvents.MouseChanged += ControlEvents_MouseChanged;


            StardewModdingAPI.Events.SaveEvents.AfterSave += SaveEvents_AfterSave;
            StardewModdingAPI.Events.SaveEvents.BeforeSave += SaveEvents_BeforeSave;
            StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            StardewModdingAPI.Events.MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            StardewModdingAPI.Events.MenuEvents.MenuClosed += MenuEvents_MenuClosed;

            ModHelper.Events.Multiplayer.PeerContextReceived += Multiplayer_PeerContextReceived;
            ModHelper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
            ModHelper.Events.Multiplayer.PeerDisconnected += Multiplayer_PeerDisconnected;


            playerJustDisconnected = false;

            IlluminateFramework.Colors.initializeColors();
            ContentDirectory = "Content";
            if (!Directory.Exists(ContentDirectory)) Directory.CreateDirectory(Path.Combine(ModHelper.DirectoryPath, "Content"));
            SpriteFonts.initialize();


            TextureManagers = new Dictionary<string, TextureManager>();
            TextureManager = new TextureManager();
            TextureManager.addTexture("Test1", new Texture2DExtended(ModCore.ModHelper, Manifest,Path.Combine("Content", "Graphics", "MultiTest", "Test1.png")));
            TextureManager.addTexture("Test2", new Texture2DExtended(ModCore.ModHelper, Manifest, Path.Combine("Content", "Graphics", "MultiTest", "Test2.png")));
            TextureManager.addTexture("Test3", new Texture2DExtended(ModCore.ModHelper, Manifest, Path.Combine("Content", "Graphics", "MultiTest", "Test3.png")));
            TextureManagers.Add(ModManifest.UniqueID, TextureManager);
            StardewModdingAPI.Events.ControlEvents.KeyPressed += ControlEvents_KeyPressed;

            ModHelper.Events.World.ObjectListChanged += World_ObjectListChanged;

            config = ModHelper.ReadConfig<ModConfig>();
            
            StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;

            Helper.Events.Player.Warped += Player_Warped;

            ModHelper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
            
        }

        private void World_ObjectListChanged(object sender, StardewModdingAPI.Events.ObjectListChangedEventArgs e)
        {
            if (e.Added != null)
            {
                
            }
            else if (e.Removed != null)
            {

            }
        }

        private void Player_Warped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (justWarped) return;
            SerializationManager.cleanUpInventory();
            //SerializationManager.cleanUpWorld();
            //SerializationManager.cleanUpStorageContainers();
            justWarped = true;
        }


        private void GameLoop_ReturnedToTitle(object sender, StardewModdingAPI.Events.ReturnedToTitleEventArgs e)
        {
            if (Game1.player != null)
            {
                SerializationManager.cleanUpInventory();
                //SerializationManager.cleanUpWorld();
                //SerializationManager.cleanUpStorageContainers();
                //Monitor.Log("Saved the player data after returning to title!");
            }
        }

        private void Multiplayer_PeerDisconnected(object sender, StardewModdingAPI.Events.PeerDisconnectedEventArgs e)
        {
            this.playerJustDisconnected = true;

            //ModMonitor.Log("Peer disconnected! Serializing custom objects");

            SerializationManager.cleanUpInventory();
            //SerializationManager.cleanUpWorld();
            //SerializationManager.cleanUpStorageContainers();
            
        }

        private void Multiplayer_ModMessageReceived(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            //ModMonitor.Log("Get messages from"+e.FromModID);
            //ModMonitor.Log("Message type: " + e.Type);

            if (e.FromModID == this.ModManifest.UniqueID)
            {
                if (e.Type == MultiplayerSupport.CleanUpModObjects)
                {
                    SerializationManager.cleanUpInventory();
                    SerializationManager.cleanUpWorld();
                    SerializationManager.cleanUpStorageContainers();
                }
                else if (e.Type == MultiplayerSupport.RestoreModObjects)
                {
                    if (Game1.eventUp) return; //Prevent item duplication.
                    //ModMonitor.Log("Restoring custom objects.");
                    SerializationManager.restoreAllModObjects(SerializationManager.trackedObjectList);
                }
            }
        }

        private void Multiplayer_PeerContextReceived(object sender, StardewModdingAPI.Events.PeerContextReceivedEventArgs e)
        {
           
            if (SerializationManager == null) return;


            if (Game1.eventUp)
            {
                SerializationManager.restoreAllModObjects(SerializationManager.trackedObjectList); //Force a restore and then a serialize save to prevent deletions.
            }
            //ModMonitor.Log("Got peer context. Serialize/remove all custom objects really quick to prevent loading errors.");

            
            SerializationManager.cleanUpInventory();
            //SerializationManager.cleanUpWorld();
            //SerializationManager.cleanUpStorageContainers();

            
        }

        private void MenuEvents_MenuClosed(object sender, StardewModdingAPI.Events.EventArgsClickableMenuClosed e)
        {
                if (this.lastMenuType == null)
                {
                    return;
                }
                else
                {
                /*
                    if (lastMenuType == typeof(StardewValley.Menus.SaveGameMenu) ||lastMenuType==typeof(StardewValley.Menus.ShippingMenu))
                    {
                        SerializationManager.restoreAllModObjects(SerializationManager.trackedObjectList);

                        List<long> playerIds = new List<long>();
                        foreach (Farmer f in Game1.getAllFarmers())
                        {
                            if (f == Game1.player) continue;
                            playerIds.Add(f.uniqueMultiplayerID);

                        }
                        ModHelper.Multiplayer.SendMessage<string>(MultiplayerSupport.RestoreModObjects, MultiplayerSupport.RestoreModObjects, new string[] { ModManifest.UniqueID }, playerIds.ToArray());
                    }
                    */
                    //Only fires in multiplayer since ReadyCheckDialogue only appears in multiplayer
                    if (lastMenuType == typeof(StardewValley.Menus.ReadyCheckDialog) && Game1.player.canMove==false && Game1.player.isInBed)
                    {
                        //ModMonitor.Log("Time to sleep/serialize all objects!");
                        SerializationManager.cleanUpInventory();
                        SerializationManager.cleanUpWorld();
                        SerializationManager.cleanUpStorageContainers();
                        
                        /*
                        List<long> playerIds = new List<long>();
                        foreach (Farmer f in Game1.getAllFarmers())
                        {
                        if (f == null) continue;
                            if (f == Game1.player) continue;
                            playerIds.Add(f.uniqueMultiplayerID);

                        }
                        */
                        //ModHelper.Multiplayer.SendMessage<string>(MultiplayerSupport.CleanUpModObjects, MultiplayerSupport.CleanUpModObjects, new string[] { ModManifest.UniqueID }, playerIds.ToArray());

                }
            }
        }

        private void MenuEvents_MenuChanged(object sender, StardewModdingAPI.Events.EventArgsClickableMenuChanged e)
        {            
            lastMenuType = Game1.activeClickableMenu.GetType();
        }

        /// <summary>
        /// Returns the value of the data snagged by reflection.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetInstanceField(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            /*
            FieldInfo[] meh = type.GetFields(bindFlags);
            foreach(var v in meh)
            {
                if (v.Name == null)
                {
                    continue;
                }
                Monitor.Log(v.Name);
            }
            */
            return field.GetValue(instance);
        }

        public static void SetInstanceField(Type type, object instance, object value, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            field.SetValue(instance, value);
            return;
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (playerJustDisconnected)
            {
                playerJustDisconnected = false;
                if (Game1.activeClickableMenu != null)
                {
                    if (Game1.activeClickableMenu.GetType() == typeof(StardewValley.Menus.TitleMenu)) return;
                }
                //ModMonitor.Log("Restore objects after peer disconnect!");
                SerializationManager.restoreAllModObjects(SerializationManager.trackedObjectList);
            }
            else if (justWarped && Game1.eventUp==false && Game1.activeClickableMenu==null)
            {
                justWarped = false;
                //ModMonitor.Log("Restore objects after player warping!");
                SerializationManager.restoreAllModObjects(SerializationManager.trackedObjectList,true);
            }
        }

        private void ControlEvents_KeyPressed(object sender, StardewModdingAPI.Events.EventArgsKeyPressed e)
        {
            if(e.KeyPressed.ToString()== config.modularMenuKey && Game1.activeClickableMenu==null)
            {
                //Game1.activeClickableMenu = new ModularGameMenu(0);
            }
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            Game1.game1.Disposed += Game1_Disposed;

            string basePath=Path.Combine( ModCore.ModHelper.DirectoryPath, "PlayerData", Game1.player.Name + "_" + Game1.player.uniqueMultiplayerID);

            string invPath = Path.Combine(basePath,"PlayerInventory");
            string worldPath = Path.Combine(basePath, "ObjectsInWorld");
            string trashPath = Path.Combine(basePath,"ModTrashFolder");
            string chestPath = Path.Combine(basePath, "StorageContainers");
            SerializationManager = new SerializationManager(invPath, trashPath, worldPath, chestPath);
            SerializationManager.initializeDefaultSuportedTypes();

            SerializationManager.restoreAllModObjects(SerializationManager.trackedObjectList);

            List<long> playerIds = new List<long>();
            foreach (Farmer f in Game1.getOnlineFarmers())
            {
                if (f.uniqueMultiplayerID == Game1.player.uniqueMultiplayerID) continue;
                playerIds.Add(f.uniqueMultiplayerID);
                ModMonitor.Log("Sending Message to peer ID: "+f.uniqueMultiplayerID.ToString());
            }
            //ModMonitor.Log("Send message to restore mod objects after load!");
            ModHelper.Multiplayer.SendMessage<string>(MultiplayerSupport.RestoreModObjects, MultiplayerSupport.RestoreModObjects, new string[] { ModManifest.UniqueID },playerIds.ToArray());
            /*
            List<KeyValuePair<Vector2, MultiTileComponent>> objs = new List<KeyValuePair<Vector2, MultiTileComponent>>();
            
            MultiTileComponent tile1 = new MultiTileComponent(0,"Tileobj1","A basic tile obj",new Texture2DExtended(ModCore.ModHelper,ModCore.Manifest ,Path.Combine("Content", "Graphics", "MultiTest", "Test1.png")));
            MultiTileComponent tile2 = new MultiTileComponent(0,"Tileobj2", "A basic tile obj", new Texture2DExtended(ModCore.ModHelper, ModCore.Manifest, Path.Combine("Content", "Graphics", "MultiTest", "Test2.png")));
            MultiTileComponent tile3 = new MultiTileComponent(0,"Tileobj3", "A basic tile obj", new Texture2DExtended(ModCore.ModHelper, ModCore.Manifest, Path.Combine("Content", "Graphics", "MultiTest", "Test3.png")));
            objs.Add(new KeyValuePair<Vector2, MultiTileComponent>(new Vector2(0, 0), tile1));
            objs.Add(new KeyValuePair<Vector2, MultiTileComponent>(new Vector2(1, 0), tile2));
            objs.Add(new KeyValuePair<Vector2, MultiTileComponent>(new Vector2(2, 0), tile3));

            MultiTileObject collection= new MultiTileObject("MultiTest", "Trying to get multi object testing working", Vector2.Zero, new Texture2DExtended(ModCore.ModHelper, ModCore.Manifest, Path.Combine("Content", "Graphics", "MultiTest", "Test3.png")), objs, StardustCore.IlluminateFramework.Colors.invertColor(StardustCore.IlluminateFramework.LightColorsList.Purple), "MultiTest");
            

            Game1.player.addItemToInventory(collection);
            */

            /*
            CoreObject testTile = new CoreObject(new Texture2DExtended(ModCore.ModHelper,ModCore.Manifest, Path.Combine("Content", "Graphics", "MultiTest", "Test3.png")),3, Vector2.Zero,9);

            testTile.description = "Hello";
            testTile.Name = "test";
            testTile.displayName = "test";
            Game1.player.addItemToInventory(testTile);
            */
            
        }

        private void Game1_Disposed(object sender, EventArgs e)
        {
            if (Game1.player != null && lastMenuType!= typeof(StardewValley.Menus.TitleMenu))
            {
                //ModMonitor.Log("Serialize all objects on disposing!");
                SerializationManager.cleanUpInventory();
                //SerializationManager.cleanUpWorld();
                //SerializationManager.cleanUpStorageContainers();
            }
        }

        private void SaveEvents_AfterSave(object sender, EventArgs e)
        {
            
            SerializationManager.restoreAllModObjects(SerializationManager.trackedObjectList);

        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            //Call the serialization if alone since the ReadyCheckDialogue menu never shows with just 1 player online.
            if (Game1.IsMultiplayer == false || (Game1.IsMultiplayer && Game1.getOnlineFarmers().Count==1))
            {
                //ModMonitor.Log("Serialize all objects before save when only 1 farmer present!");
                SerializationManager.cleanUpInventory();
                SerializationManager.cleanUpWorld();
                SerializationManager.cleanUpStorageContainers();
            }


        }

        private void ControlEvents_MouseChanged(object sender, StardewModdingAPI.Events.EventArgsMouseStateChanged e)
        {
            //??? 
            return;
            if (Game1.activeClickableMenu == null) return;
            var MouseState = Mouse.GetState();
            if (Game1.activeClickableMenu is StardewValley.Menus.ItemGrabMenu && MouseState.LeftButton == ButtonState.Released)
            {
                (Game1.activeClickableMenu as StardewValley.Menus.ItemGrabMenu).ItemsToGrabMenu.populateClickableComponentList();
                for (int index = 0; index < (Game1.activeClickableMenu as StardewValley.Menus.ItemGrabMenu).ItemsToGrabMenu.inventory.Count; ++index)
                {
                    if ((Game1.activeClickableMenu as StardewValley.Menus.ItemGrabMenu).ItemsToGrabMenu.inventory[index] != null)
                    {
                        (Game1.activeClickableMenu as StardewValley.Menus.ItemGrabMenu).ItemsToGrabMenu.inventory[index].myID += 53910;
                        (Game1.activeClickableMenu as StardewValley.Menus.ItemGrabMenu).ItemsToGrabMenu.inventory[index].upNeighborID += 53910;
                        (Game1.activeClickableMenu as StardewValley.Menus.ItemGrabMenu).ItemsToGrabMenu.inventory[index].rightNeighborID += 53910;
                        (Game1.activeClickableMenu as StardewValley.Menus.ItemGrabMenu).ItemsToGrabMenu.inventory[index].downNeighborID = -7777;
                        (Game1.activeClickableMenu as StardewValley.Menus.ItemGrabMenu).ItemsToGrabMenu.inventory[index].leftNeighborID += 53910;
                        (Game1.activeClickableMenu as StardewValley.Menus.ItemGrabMenu).ItemsToGrabMenu.inventory[index].fullyImmutable = true;
                    }
                }
                // (Game1.activeClickableMenu as ItemGrabMenu).inventory.playerInventory = false;
                // Game1.activeClickableMenu =Game1.activeClickableMenu;//new ItemGrabMenu((Game1.activeClickableMenu as ItemGrabMenu).ItemsToGrabMenu.actualInventory,true,true,null,null,null,null,false,false,true,true,true,1,null,-1,null);
            }
        }
    }
}
