/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BuildABuddha/StardewDailyPlanner
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace DailyPlanner.Framework
{
    internal class PlannerListComponent : OptionsElement
    {
        /*********
        ** Fields
        *********/
        /// <summary>The Planner helper.</summary>
        private readonly Planner Planner;

        /// <summary>The CheckList helper.</summary>
        private readonly CheckList CheckList;

        /// <summary>The source rectangle for the 'set' button sprite.</summary>
        private readonly Rectangle DoneButtonSprite = new(441, 411, 24, 13);

        /// <summary>Defines if button is used on checklist tasks rather than daily tasks.</summary>
        private readonly bool IsCheckListButton;

        /// <summary>Area on the screen that the button occupies. Used to check if we click on it.</summary>
        private Rectangle DoneButtonBounds;

        /// <summary>The original menu, so it can be refreshed.</summary>
        private readonly PlannerMenu PlannerMenu;

        private readonly bool HasButton;

        /*********
        ** Public methods
        *********/
        /// <summary>Construct a button with Planner helper.</summary>
        /// <param name="label">The field label.</param>
        /// <param name="slotWidth">The field width.</param>
        /// <param name="planner">The planner helper.</param>
        /// <param name="plannermenu">The PlannerMenu creating this button.</param>
        public PlannerListComponent(string label, int slotWidth, Planner planner, PlannerMenu plannermenu, bool includeButton = true)
          : base(label, -1, -1, slotWidth + 1, 11 * Game1.pixelZoom)
        {
            this.DoneButtonBounds = new Rectangle(
                slotWidth - (DoneButtonSprite.Width + 7) * Game1.pixelZoom, 
                -1 + Game1.pixelZoom * 3,
                DoneButtonSprite.Width * Game1.pixelZoom, 
                DoneButtonSprite.Height * Game1.pixelZoom);
            this.Planner = planner;
            this.PlannerMenu = plannermenu;
            this.HasButton = includeButton;
        }

        /// <summary>Construct a button with CheckList helper.</summary>
        /// <param name="label">The field label.</param>
        /// <param name="slotWidth">The field width.</param>
        /// <param name="checkList">The checklist helper.</param>Refactoring & documentation
        /// <param name="plannermenu">The PlannerMenu creating this button.</param>
        public PlannerListComponent(string label, int slotWidth, CheckList checkList, PlannerMenu plannermenu, bool includeButton = true)
          : base(label, -1, -1, slotWidth + 1, 11 * Game1.pixelZoom)
        {
            this.DoneButtonBounds = new Rectangle(
                slotWidth - (DoneButtonSprite.Width + 7) * Game1.pixelZoom, 
                -1 + Game1.pixelZoom * 3, 
                DoneButtonSprite.Width * Game1.pixelZoom, 
                DoneButtonSprite.Height * Game1.pixelZoom);
            this.CheckList = checkList;
            this.PlannerMenu = plannermenu;
            this.IsCheckListButton = true;
            this.HasButton = includeButton;
        }

        /// <summary>Called when player left clicks on the menu.</summary>
        /// <param name="x">X coordinate of the click.</param>
        /// <param name="y">Y coordinate of the click.</param>
        public override void receiveLeftClick(int x, int y)
        {
            if (this.HasButton)
            {
                if (this.greyedOut ||!this.DoneButtonBounds.Contains(x, y)) { return; }      // Didn't click on button. Do nothing.
                else if (this.IsCheckListButton) { this.CheckList.CompleteTask(label); }    // Clicked on checklist button!
                else { this.Planner.CompleteTask(label); }                                  // Clicked on daily planner button!

                Game1.activeClickableMenu = new PlannerMenu(PlannerMenu);                   // Refresh the planner menu.
                Game1.soundBank.PlayCue("achievement");                                     // Play a sound!
            }
            return;
        }

        public override void draw(SpriteBatch spriteBatch, int slotX, int slotY, IClickableMenu context = null)
        {
            Utility.drawTextWithShadow(
                spriteBatch,
                this.label,
                Game1.dialogueFont,
                new Vector2(
                    this.bounds.X + slotX,
                    this.bounds.Y + slotY),
                this.greyedOut ? Game1.textColor * 0.33f : Game1.textColor,
                1f,
                0.15f);

            if (this.HasButton)
            {
                Utility.drawWithShadow(
                    spriteBatch,
                    Game1.mouseCursors,
                    new Vector2(this.DoneButtonBounds.X + slotX, this.DoneButtonBounds.Y + slotY),
                    this.DoneButtonSprite,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    Game1.pixelZoom,
                    false,
                    0.15f);
            }

            return;
        }
    }
}
