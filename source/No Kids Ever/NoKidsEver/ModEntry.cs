using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Events;
using StardewValley.Menus;

namespace NoKidsEver
{    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += this.OnMenuChanged;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox dialogue)
            {
                bool isBabyQuestion =
                    Game1.farmEvent is QuestionEvent
                    && this.Helper.Reflection.GetField<int>(Game1.farmEvent, "whichQuestion").GetValue() == QuestionEvent.pregnancyQuestion;

                if (isBabyQuestion)
                {
                    this.Monitor.Log("Blocked kids question.", LogLevel.Trace);
                    Response no = this.Helper.Reflection.GetField<List<Response>>(dialogue, "responses").GetValue()[1];
                    Game1.currentLocation.answerDialogue(no);
                    dialogue.closeDialogue();
                }
            }
        }

        /* Just my little "thank you" to Pathos for the INSANE level of patience displayed
         * during the countless hours of me pestering them on discord with questions, and all
         * the explanations provided. It could not have been done, by me at least, without their
         * help. Thank you. :D */
    }
}