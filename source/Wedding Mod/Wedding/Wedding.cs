using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile;

namespace WeddingMod
{
    /// <summary>The main entry point for the mod.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The player's tile position in the wedding map.</summary>
        private readonly Vector2 PlayerPixels = new Vector2(1732, 4052);


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;

        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.weddingToday)
            {
                // Lets clean this up some
                string question = this.Helper.Translation.Get("Wedding_Question");
                Response[] options = {
                    new Response("On the Beach.",this.Helper.Translation.Get("Wedding_Beach")),
                    new Response("In the Forest, near the river.",this.Helper.Translation.Get("Wedding_Forest")),
                    new Response("In the Secret Woods.",this.Helper.Translation.Get("Wedding_Wood")),
                    new Response("Where the Flower Dance takes place!",this.Helper.Translation.Get("Wedding_Flower"))
                };

                /*
                   I changed up the createQuestionDialogue, to make it easier to see. Basically, it pulls the question from the question string, and the responses from the options list, then turns it into an array. The answer part is a method that is called when the player clicks on a response.
                */
                Game1.player.currentLocation.createQuestionDialogue(
                    question,
                    options,
                    OnAnswered
                );
            }
        }

        /// <summary>The method that is called inside of the createQuestionDialogue</summary>
        /// <param name="who">The player.</param>
        /// <param name="ans">The answer key.</param>
        private void OnAnswered(Farmer who, string ans)
        {
            string mapPath = null;
            switch (ans)
            {
                case "On the Beach.":
                    mapPath = "assets/WeddingBeach.tbin";
                    break;

                case "In the Forest, near the river.":
                    mapPath = "assets/WeddingForest.tbin";
                    break;

                case "In the Secret Woods.":
                    mapPath = "assets/WeddingWoods.tbin";
                    break;

                case "Where the Flower Dance takes place!":
                    mapPath = "assets/WeddingFlower.tbin";
                    break;
            }

            if (mapPath != null)
            {
                this.SetUpMaps(mapPath);
                Game1.showGlobalMessage(mapPath);
            }
        }

        /// <summary>Set up the marriage map and move the current wedding event.</summary>
        /// <param name="mapPath">The map path to load.</param>
        private void SetUpMaps(string mapPath)
        {
            this.Monitor.Log("The wedding event has started!");

            // create temporary location
            this.Helper.Content.Load<Map>(mapPath); // make sure map is loaded before game accesses it
            string mapKey = this.Helper.Content.GetActualAssetKey(mapPath);
            var newLocation = new GameLocation(mapKey, "Temp");

            // move everything to new location
            var oldLocation = Game1.currentLocation;
            Game1.player.currentLocation = Game1.currentLocation = newLocation;
            newLocation.resetForPlayerEntry();
            newLocation.currentEvent = oldLocation.currentEvent;
            newLocation.Map.LoadTileSheets(Game1.mapDisplayDevice);

            // move player position within map
            Game1.player.Position = this.PlayerPixels;

            // add temp sprites
            newLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(540, 1196, 98, 54), 99999f, 1, 99999, new Vector2(25f, 60f) * 64f + new Vector2(0.0f, -64f), false, false, 1f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, false));
            newLocation.TemporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(540, 1250, 98, 25), 99999f, 1, 99999, new Vector2(25f, 60f) * 64f + new Vector2(0.0f, 54f) * 4f + new Vector2(0.0f, -64f), false, false, 0.0f, 0.0f, Color.White, 4f, 0.0f, 0.0f, 0.0f, false));
        }
    }
}

