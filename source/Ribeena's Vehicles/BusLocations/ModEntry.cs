/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/BusLocations
**
*************************************************/

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

        /// <summary>Api for integrating other mods</summary>
        private BusStopEventsApi busStopEvents;

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

            this.busStopEvents = new BusStopEventsApi();
        }

        public override object GetApi()
        {
            return this.busStopEvents;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised when a button is pressed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {

            if (!Context.IsWorldReady)
                return;

            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                if (e.Button != SButton.MouseLeft)
                    return;
                if (e.Cursor.GrabTile != e.Cursor.Tile)
                    return;
            }
            else if (!e.Button.IsActionButton())
                return;

            if (!Game1.currentLocation.Name.Contains("BusStop"))
                return;

            if (Game1.currentLocation.doesTileHaveProperty(7, 11, "Action", "Buildings") != "BusTicket")
                return;

            if (!(e.Cursor.GrabTile.X == 7 && (e.Cursor.GrabTile.Y == 11 || e.Cursor.GrabTile.Y == 10)))
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
                if (busStopEvents.HasJumpToOverride())
                {
                    //Let the other mod handle warping
                    busStopEvents.SetGoingTo(Locations[index]);
                    busStopEvents.JumpToLocation();
                } else { 
                    Game1.player.Halt();
                    Game1.player.freezePause = 700;
                    Game1.warpFarmer(Locations[index].MapName, Locations[index].DestinationX, Locations[index].DestinationY, Locations[index].ArrivalFacing);
                }
            }
            else if (Game1.player.Money < Locations[index].TicketPrice)
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
            else
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NoDriver"));
        }
    }
}
