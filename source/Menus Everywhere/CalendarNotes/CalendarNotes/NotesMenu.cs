using System;
using System.Threading;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;

namespace CalendarNotes
{
    /// <summary> The Notes Menu </summary>
    class NotesMenu : IClickableMenu
    {
        /*********
        ** Properties
        *********/
        /// <summary> Allows for monitoring </summary>
        private IMonitor monitor;

        /*********
        ** Public methods
        *********/
        /// <summary> The constructor for the menu. </summary>
        public NotesMenu(IMonitor m)
        {
            this.monitor = m;
            this.InitializeComponents();
        }

        public override void draw(SpriteBatch SB)
        {
            if (!Game1.options.showMenuBackground)
                SB.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            SB.DrawString(Game1.dialogueFont, this.ConstructNote(Game1.year, Utility.getSeasonNameFromNumber(Utility.getSeasonNumber(Game1.currentSeason)), Game1.dayOfMonth), new Vector2((float)(this.xPositionOnScreen + Game1.tileSize * 3 / 4), (float)(this.yPositionOnScreen + Game1.tileSize * 8 / 5)), Game1.textColor);
            base.draw(SB);
        }

        /*********
        ** Private methods
        *********/
        /// <summary> Initializes the menu components. </summary>
        private void InitializeComponents()
        {
            this.width = 301 * Game1.pixelZoom;
            this.height = 198 * Game1.pixelZoom;
            Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0);
            this.xPositionOnScreen = (int)centeringOnScreen.X;
            this.yPositionOnScreen = (int)centeringOnScreen.Y;
        }

        /// <summary> Constructs the note for a given day. </summary>
        /// <param name="Year"> The Year of the day. </param>
        /// <param name="Month"> The Month of the day. </param>
        /// <param name="Day"> The Day. </param>
        /// <returns> The note. </returns>
        private String ConstructNote(int Year, String Month, int Day)
        {
            String output = "Year " + Year.ToString() + " " + Month + " " + Day.ToString() + " Notes:\n\n";

            return output;
        }

        /// <summary> This method constructs the portion of the note containing the rotating stock of the various stores. </summary>
        /// <param name="Day"> The Day. </param>
        /// <returns> The portion of the note containing the stock for a given day. </returns>
        private String ConstructRotatingStock(int Day)
        {
            return "";
        }
    }
}
