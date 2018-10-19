using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Translation = StardewMods.ArchaeologyHouseContentManagementHelper.Common.Translation;

namespace StardewMods.ArchaeologyHouseContentManagementHelper.Framework.Services
{
    /// <summary>
    /// This class is responsible for firing the [All Lost Books found] message.
    /// </summary>
    internal class LostBookFoundDialogService
    {
        private bool showMessage;

        private bool running;

        private IMonitor monitor;

        public LostBookFoundDialogService()
        {
            monitor = ModEntry.CommonServices.Monitor;

            running = false;
        }

        public void Start()
        {
            if (running)
            {
                monitor.Log("[LostBookFoundDialogService] is already running!", LogLevel.Info);
                return;
            }

            running = true;

            MenuEvents.MenuChanged += MenuEvents_MenuChanged;
            MenuEvents.MenuClosed += MenuEvents_MenuClosed;
        }

        public void Stop()
        {
            if (!running)
            {
                monitor.Log("[LostBookFoundDialogService] is not running or has already been stopped!", LogLevel.Info);
                return;
            }

            MenuEvents.MenuChanged -= MenuEvents_MenuChanged;
            MenuEvents.MenuClosed -= MenuEvents_MenuClosed;

            running = false;
        }     

        private void MenuEvents_MenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            if (e.NewMenu is DialogueBox box)
            {
                var mostRecentlyGrabbed = Game1.player.mostRecentlyGrabbedItem;
                if (mostRecentlyGrabbed != null && mostRecentlyGrabbed.ParentSheetIndex == StardewMods.Common.StardewValley.Constants.ID_GAME_OBJECT_LOST_BOOK)
                {
                    List<string> dialogues = ModEntry.CommonServices.ReflectionHelper.GetField<List<string>>(box, "dialogues").GetValue();
                    if (dialogues.Count == 1 && dialogues[0].Equals(mostRecentlyGrabbed.checkForSpecialItemHoldUpMeessage()) 
                        && LibraryMuseumHelper.LibraryBooks == LibraryMuseumHelper.TotalLibraryBooks)
                    {
                        showMessage = true;
                    }
                }
            }
        }

        private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            if (e.PriorMenu is DialogueBox box && showMessage)
            {
                Game1.drawObjectDialogue(ModEntry.CommonServices.TranslationHelper.Get(Translation.MESSAGE_LIBRARY_BOOKS_COMPLETED));
                showMessage = false;
            }
        }
    }
}
