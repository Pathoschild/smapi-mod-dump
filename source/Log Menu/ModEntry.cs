/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jaredtjahjadi/LogMenu
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace LogMenu
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Properties
        *********/
        // The log of dialogue lines. Tuple is <name, emotion, dialogue line, responses>
        private DialogueQueue<DialogueElement> dialogueList;
        private ModConfig Config; // The mod configuration from the player
        private List<string> responses;
        private string prevAddedDialogue;
        private List<string> hudTexts; // HUD messages to go in and out

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            dialogueList = new(Config.LogLimit);
            hudTexts = new();

            // SMAPI methods
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.MenuChanged += OnMenuChanged;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>The method called after the game is launched. Enables compatibility with Generic Mod Config Menu.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;
            // Register mod to Generic Mod Config Menu
            configMenu.Register(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            
            // Display config options in Generic Mod Config Menu
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.start-from-bottom.name"),
                tooltip: () => Helper.Translation.Get("config.start-from-bottom.tooltip"),
                getValue: () => Config.StartFromBottom,
                setValue: value => Config.StartFromBottom = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.oldest-to-newest.name"),
                tooltip: () => Helper.Translation.Get("config.oldest-to-newest.tooltip"),
                getValue: () => Config.OldestToNewest,
                setValue: value => Config.OldestToNewest = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.non-npc-dialogue.name"),
                tooltip: () => Helper.Translation.Get("config.non-npc-dialogue.tooltip"),
                getValue: () => Config.NonNPCDialogue,
                setValue: value => Config.NonNPCDialogue = value
            );
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.toggle-hud-messages.name"),
                tooltip: () => Helper.Translation.Get("config.toggle-hud-messages.tooltip"),
                getValue: () => Config.ToggleHUDMessages,
                setValue: value => Config.ToggleHUDMessages = value
            );
            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.log-limit.name"),
                tooltip: () => Helper.Translation.Get("config.log-limit.tooltip"),
                getValue: () => Config.LogLimit,
                setValue: value => Config.LogLimit = value
            );
            configMenu.AddKeybind(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.log-menu-button.name"),
                tooltip: () => Helper.Translation.Get("config.log-menu-button.tooltip"),
                getValue: () => Config.LogButton,
                setValue: value => Config.LogButton = value
            );

        }

        /// <summary>When loading a save, the dialogue queue is replaced with an empty one.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e) {
            dialogueList = new(Config.LogLimit);
            hudTexts = new();
        }

        // Handles repeatable dialogue (e.g., repeatedly interacting with objects, some NPC lines, some event lines)
        // by resetting prevAddedDialogue to null
        private void OnMenuChanged(object sender, MenuChangedEventArgs e) { if (e.NewMenu == null) prevAddedDialogue = null; }

        /// <summary>The method invoked when the game updates its state.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {            
            if (!e.IsMultipleOf(15)) return; // Below code runs every quarter of a second

            // If the currently open menu is a dialogue box
            if (Game1.activeClickableMenu is DialogueBox db)
            {
                // Dialogue boxes with questions
                responses = new(); // Reset responses, so responses from previous questions don't get carried over
                if (db.isQuestion)
                {
                    // Converts each response from Response to string, then adds it to responses list variable
                    for (int i = 0; i < db.responses.Length; i++) responses.Add(db.responses[i].responseText);
                    // TODO: Check player's response to question
                    //this.Monitor.Log($"{responseInd}", LogLevel.Debug);
                }

                // Adding dialogue to log
                // In multi-part dialogues, the transitioningBigger check makes sure that incoming dialogue doesn't get logged too early
                string currStr = db.getCurrentString();
                if (prevAddedDialogue != currStr && db.transitioningBigger)
                {
                    int portraitIndex = (db.characterDialogue is null) ? -1 : db.characterDialogue.getPortraitIndex();
                    if (db.isQuestion || (Game1.options.showPortraits && !db.isPortraitBox())) portraitIndex = -2;
                    //Monitor.Log("portraitIndex = " + portraitIndex, LogLevel.Debug);
                    //if(db.characterDialogue is not null) Monitor.Log("Emotion = " + db.characterDialogue.CurrentEmotion, LogLevel.Debug);
                    AddToDialogueList(
                        db.characterDialogue,
                        // Determine portrait index: if no character dialogue or if db doesn't have portrait (e.g., Sam skateboarding) or if character asking question
                        portraitIndex,
                        currStr,
                        responses);
                    prevAddedDialogue = currStr;
                    //prevIsHud = false;
                }
            }

            // HUD messages
            if (!Config.ToggleHUDMessages) return; // Return if toggle HUD messages config option is unchecked
            foreach(HUDMessage h in Game1.hudMessages)
            {
                string hudMsg = h.message;
                Item messageSubject = Helper.Reflection.GetField<Item>(h, "messageSubject").GetValue();
                if (messageSubject != null) return;
                if (!hudTexts.Contains(hudMsg))
                {
                    AddToDialogueList(null, 0, hudMsg);
                    hudTexts.Add(hudMsg);
                }
            }
            for(int i = 0; i < hudTexts.Count; i++) if (!Game1.doesHUDMessageExist(hudTexts[i])) hudTexts.Remove(hudTexts[i]);
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Do nothing if no save file has been loaded yet
            if (!Context.IsWorldReady) return;

            // Upon pressing the Log button
            if (e.Button == Config.LogButton)
            {
                // Only open log menu when game is not paused
                if ((Game1.activeClickableMenu == null || Game1.IsMultiplayer) && !Game1.paused)
                {
                    // Set activeClickableMenu to LogMenu, passing the dialogue list and config options
                    Game1.activeClickableMenu = new LogMenu(dialogueList, Config.StartFromBottom, Config.OldestToNewest);
                    Game1.playSound("bigSelect"); // Play "bloop bleep" sound upon opening menu
                }
                else if(Game1.activeClickableMenu is LogMenu)
                {
                    Game1.exitActiveMenu();
                    Game1.playSound("bigDeSelect"); // Play "bleep bloop" sound upon closing menu
                }
            }

            // If event skipped, add skipped lines to dialogueList
            //if(Game1.currentLocation.currentEvent != null && !Game1.currentLocation.currentEvent.skipped && Game1.currentLocation.currentEvent.skippable)
            //{
            //    foreach(NPC n in Game1.currentLocation.currentEvent.actors)
            //    {
            //        foreach(Dialogue d in n.CurrentDialogue)
            //        {
            //            this.Monitor.Log($"{n.displayName}: {d.dialogues}", LogLevel.Debug);
            //        }
            //        this.Monitor.Log("END OF CURRENT NPC", LogLevel.Debug);
            //    }
            //    this.Monitor.Log("END OF BUTTON PRESS LOG", LogLevel.Debug);
            //}

            // uhh idk how to do this part lol so if anyone knows feel free to help 🙏
            // Check response to an in-game dialogue question upon button click
            //if (Game1.activeClickableMenu is DialogueBox db)
            //{
            //    List<Response> responses = db.responses;
            //    int responseInd = db.selectedResponse;

            //    // Code to check if player pressed button to close dialogue box (??????)
            //    /**
            //     * Pseudocode:
            //     * if (player button == (button to open menu (E or Escape by default)) || no event is playing rn)
            //     *     Player response = responses.count - 1 (usually the leave option)
            //     * Modify existing response text in dialogue list so that chosen response is bold
            //     */

            //    if (responseInd < 0 || responses == null || responseInd > responses.Count || responses[responseInd] == null) return;
            //}
        }

        // Adds provided dialogue line to dialogue list
        private void AddToDialogueList(Dialogue charDiag, int portraitIndex, string dialogue, List<string> responses = null)
        {
            // Replace ^, which represent new line characters in dialogue lines
            dialogue = dialogue.Replace("^", Environment.NewLine);
            if (charDiag is null && Config.NonNPCDialogue is false) return; // If non-NPC dialogue line and non-NPC dialogue config option is false, return
            splitDialogue(charDiag, portraitIndex, dialogue, 5);

            // Handles responses
            if(responses != null && responses.Count > 0)
            {
                dialogue = "> ";
                dialogue += string.Join($"{Environment.NewLine}> ", responses);
                splitDialogue(charDiag, -1, dialogue, 5);
            }
        }

        private void splitDialogue(Dialogue charDiag, int portraitIndex, string dialogue, int limit)
        {
            List<string> brokenUpDialogue = new();
            List<string> splitDialogue = dialogue.Split(Environment.NewLine).ToList();
            int n = splitDialogue.Count - 1;
            // Split up long dialogue lines with more than 4 line breaks
            if (n > limit)
            {
                for(int i = 0; i < n; i += limit)
                {
                    int ind1 = dialogue.IndexOf(splitDialogue[i]);
                    brokenUpDialogue.Add((((n - i) / limit) >= 1) ? dialogue.Substring(ind1, dialogue.IndexOf(splitDialogue[i + limit]) - ind1) : dialogue[ind1..]);
                }
                foreach (string s in brokenUpDialogue) dialogueList.enqueue(new DialogueElement(charDiag, Game1.mouseCursors, portraitIndex, s));
            }
            else
            {
                DialogueElement dialogueElement = new(charDiag, Game1.mouseCursors, portraitIndex, dialogue);
                if (dialogue != "") dialogueList.enqueue(dialogueElement);
            }
        }
    }
}