using System.Collections.Generic;
using System.Linq;
using BusLocations.Framework;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BusLocations
{
    /// <summary>The mod entry class.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The loaded bus locations.</summary>
        private BusLoc[] Locations;

        /// <summary>The available bus choices.</summary>
        private Response[] Choices;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;

            // get content packs
            IList<BusLoc> locations = new List<BusLoc>();
            foreach (var pack in this.Helper.ContentPacks.GetOwned())
            {
                this.Monitor.Log($"Reading content pack: {pack.Manifest.Name} {pack.Manifest.Version} from {pack.DirectoryPath}");
                locations.Add(pack.ReadJsonFile<BusLoc>("content.json"));
            }
            this.Locations = locations.ToArray();

            // cache choices
            IList<Response> choices = new List<Response>(this.Locations.Length);
            for (int i = 0; i < this.Locations.Length; i++)
            {
                var location = this.Locations[i];
                choices.Add(new Response(i.ToString(), $"{location.DisplayName} ({location.TicketPrice}g)"));
            }
            choices.Add(new Response("Cancel", "Cancel"));
            this.Choices = choices.ToArray();
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised when a button is pressed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!(Context.IsWorldReady && (e.Button.IsActionButton() || !Game1.currentLocation.Name.Contains("BusStop")) && Game1.currentLocation.doesTileHaveProperty(7, 11, "Action", "Buildings") == "BusTicket" && (e.Cursor.GrabTile.X == 7 && (e.Cursor.GrabTile.Y == 11 || e.Cursor.GrabTile.Y == 10))))
                return;

            this.Helper.Input.Suppress(e.Button);
            if (Game1.MasterPlayer.mailReceived.Contains("ccVault"))
                Game1.currentLocation.createQuestionDialogue("Where would you like to go?", Choices, DialogueAction);
            else
                Game1.drawObjectDialogue("Out of service");
        }

        /// <summary>Method that gets ran after the QuestionDialogue</summary>
        /// <param name="who">The player</param>
        /// <param name="whichAnswer">The Answer</param>
        private void DialogueAction(Farmer who, string whichAnswer)
        {
            if (whichAnswer == "Cancel")
                return;
            int index = int.Parse(whichAnswer);
            NPC characterFromName = Game1.getCharacterFromName("Pam");
            if (Game1.player.Money >= Locations[index].TicketPrice && Game1.currentLocation.characters.Contains(characterFromName) && characterFromName.getTileLocation() == new Vector2(11, 10))
            {
                Game1.player.Money -= Locations[index].TicketPrice;
                Game1.player.Halt();
                Game1.player.freezePause = 700;
                Game1.warpFarmer(Locations[index].MapName, Locations[index].DestinationX, Locations[index].DestinationY, Locations[index].ArrivalFacing);
            }
            else if (Game1.player.Money < Locations[index].TicketPrice)
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            else
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NoDriver"));
        }
    }
}
