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
    /// This class is responsible for displaying a congratulations message when the player has 
    /// found all [Lost Books].
    /// </summary>
    internal class LostBookFoundDialogService
    {
        private bool showMessage;

        private bool running;

        private readonly IMonitor monitor;
        private readonly IModEvents events;

        public LostBookFoundDialogService()
        {
            monitor = ModEntry.CommonServices.Monitor;
            events = ModEntry.CommonServices.Events;

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

            events.Display.MenuChanged += OnMenuChanged;
        }

        public void Stop()
        {
            if (!running)
            {
                monitor.Log("[LostBookFoundDialogService] is not running or has already been stopped!", LogLevel.Info);
                return;
            }

            events.Display.MenuChanged -= OnMenuChanged;

            running = false;
        }

        /// <summary>
        /// Called after a dialog has been created/changed/closed. Responsible for showing the 
        /// congratulations message to the player when he/she has found all [Lost Books].
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // menu opened or changed
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

            // menu closed
            else if (e.NewMenu == null && e.OldMenu is DialogueBox && showMessage)
            {
                Game1.drawObjectDialogue(ModEntry.CommonServices.TranslationHelper.Get(Translation.MESSAGE_LIBRARY_BOOKS_COMPLETED));
                showMessage = false;
            }

        }
    }
}
