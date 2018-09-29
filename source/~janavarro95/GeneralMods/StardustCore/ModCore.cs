using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardustCore.ModInfo;
using StardustCore.Serialization;
using StardustCore.UIUtilities.SpriteFonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore
{
    public class ModCore : Mod
    {
        public static IModHelper ModHelper;
        public static IMonitor ModMonitor;
        public static Serialization.SerializationManager SerializationManager;

        public static string ContentDirectory;
        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            //Unused MetaData information. Works in player inventory but not in chests. Besides who really care where an object is from anyways. Also doesn't work 100% like I intended since it only gets base mod object that this runs from, not extensions?

            //  StardewModdingAPI.Events.GraphicsEvents.OnPostRenderGuiEvent += Metadata.GameEvents_UpdateTick;
            //StardewModdingAPI.Events.ControlEvents.MouseChanged += ControlEvents_MouseChanged;
            string invPath = Path.Combine(ModCore.ModHelper.DirectoryPath, "PlayerData", Game1.player.Name, "PlayerInventory");
            string worldPath = Path.Combine(ModCore.ModHelper.DirectoryPath, Game1.player.Name, "ObjectsInWorld"); ;
            string trashPath = Path.Combine(ModCore.ModHelper.DirectoryPath, "ModTrashFolder");
            string chestPath = Path.Combine(ModCore.ModHelper.DirectoryPath, "StorageContainers");
            SerializationManager = new SerializationManager(invPath, trashPath, worldPath,chestPath);

            StardewModdingAPI.Events.SaveEvents.AfterSave += SaveEvents_AfterSave;
            StardewModdingAPI.Events.SaveEvents.BeforeSave += SaveEvents_BeforeSave;
            StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;

            IlluminateFramework.Colors.initializeColors();
            ContentDirectory = Path.Combine(ModHelper.DirectoryPath, "Content");
            if (!Directory.Exists(ContentDirectory)) Directory.CreateDirectory(ContentDirectory);
            SpriteFonts.initialize();
            
          
        }

        

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            SerializationManager.restoreAllModObjects(SerializationManager.trackedObjectList);
            
        }

        private void SaveEvents_AfterSave(object sender, EventArgs e)
        {
            SerializationManager.restoreAllModObjects(SerializationManager.trackedObjectList);

        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            SerializationManager.cleanUpInventory();
            SerializationManager.cleanUpWorld();
            SerializationManager.cleanUpStorageContainers();
        }

        private void ControlEvents_MouseChanged(object sender, StardewModdingAPI.Events.EventArgsMouseStateChanged e)
        {
       
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
