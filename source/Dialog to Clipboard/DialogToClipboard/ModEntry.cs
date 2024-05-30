/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/dlrdlrdlr/DialogToClipboard
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace DialogToClipboard
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }

        /**********
        ** Private Variables
        ***********/

        List<Type> excludedMenus = new List<Type>() {typeof(ItemGrabMenu)};
        IClickableMenu? currentMenu = null;
        bool inMenu = false;
        String GrabbedText = "";
        bool debug = false;
        /*********
        ** Private methods
        *********/

        /// <summary>Raised when player changes menu.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if(!Context.IsWorldReady)
            {
                return;
            }
            if(e.NewMenu == null && e.OldMenu != null)
            {
                inMenu = false;
                GrabbedText = "";
                return;
            }
            currentMenu = e.NewMenu;
            inMenu = true;
            
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            grabText();
        }
        /// <summary>Raised at each Tick of the game.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            
            this.grabText();
        }        
        
        private void grabText()
        {
            if(!Context.IsWorldReady)
            {
                return;
            }

            if(!inMenu || currentMenu == null )
            {
                return;
            }
            string newText = "";
            Type menuType =  currentMenu.GetType();
            if(excludedMenus.Contains(menuType))
            {
                return;            
            }
            else if(menuType.Equals(typeof(DialogueBox)))
            {
                DialogueBox curMenu = (DialogueBox)currentMenu;
                newText = curMenu.getCurrentString();
            }
            else if(menuType.Equals(typeof(QuestLog)))
            {
                QuestLog qlog = (QuestLog)currentMenu;
                IReflectedField<IQuest> currentQuest = this.Helper.Reflection.GetField<IQuest>(qlog,"_shownQuest");
                if(currentQuest != null && currentQuest.GetValue() != null)
                {
                    newText = currentQuest.GetValue().GetName() + ":" + currentQuest.GetValue().GetDescription();
                }
                else
                {
                    return;
                }
            }
            else
            {debugPrint(menuType.ToString());
                return;
            }
            if(GrabbedText == newText)
            {
                return;
            }
            GrabbedText = newText;
            this.print(processText(GrabbedText));
        }
        /// <summary>Used to strip special characters from menu text.</summary>
        /// <param name="text">The input text to be processed</param>
        private String processText(string text)
        {
            String newText = text.Replace("^", "\n");
            return newText;
        }
        private void print(string text)
        {
            if(text != "")
            {
                StardewValley.DesktopClipboard.SetText(text);
            }
            debugPrint(text);
        }
        /// <summary>
        ///  Used for debug purposes only
        /// </summary>
        /// <param name="text">Text printed to debug log</param>
        private void debugPrint(string text)
        {
            if(!debug)
            {
                return;
            }
            this.Monitor.Log(text, LogLevel.Debug);
        }
    }
}