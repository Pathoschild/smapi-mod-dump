using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace DailyPlanner.Framework
{
    internal class DailyPlannerInputListener : OptionsElement
    {
        /*********
        ** Fields
        *********/
        /// <summary>The Planner helper.</summary>
        private readonly Planner Planner;

        /// <summary>The CheckList helper.</summary>
        private readonly CheckList CheckList;

        /// <summary>The source rectangle for the 'set' button sprite.</summary>
        private readonly Rectangle SetButtonSprite = new Rectangle(294, 428, 21, 11);
        
        /// <summary>Defines if button is used on checklist tasks rather than daily tasks.</summary>
        private readonly bool IsCheckListButton;

        /// <summary>Area on the screen that the button occupies. Used to check if we click on it.</summary>
        private Rectangle SetButtonBounds;

        /// <summary>The original menu, so it can be refreshed.</summary>
        private readonly PlannerMenu PlannerMenu;

        /*********
        ** Public methods
        *********/
        /// <summary>Construct a button with Planner helper.</summary>
        /// <param name="label">The field label.</param>
        /// <param name="slotWidth">The field width.</param>
        /// <param name="planner">The planner helper.</param>
        /// <param name="plannermenu">The PlannerMenu creating this button.</param>
        public DailyPlannerInputListener(string label, int slotWidth, Planner planner, PlannerMenu plannermenu)
          : base(label, -1, -1, slotWidth + 1, 11 * Game1.pixelZoom)
        {
            this.SetButtonBounds = new Rectangle(slotWidth - 28 * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 21 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            this.Planner = planner;
            this.PlannerMenu = plannermenu;
        }

        /// <summary>Construct a button with CheckList helper.</summary>
        /// <param name="label">The field label.</param>
        /// <param name="slotWidth">The field width.</param>
        /// <param name="checkList">The checklist helper.</param>
        /// <param name="plannermenu">The PlannerMenu creating this button.</param>
        public DailyPlannerInputListener(string label, int slotWidth, CheckList checkList, PlannerMenu plannermenu)
          : base(label, -1, -1, slotWidth + 1, 11 * Game1.pixelZoom)
        {
            this.SetButtonBounds = new Rectangle(slotWidth - 28 * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 21 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            this.CheckList = checkList;
            this.PlannerMenu = plannermenu;
            this.IsCheckListButton = true;
        }

        /// <summary>Called when player left clicks on the menu.</summary>
        /// <param name="x">X coordinate of the click.</param>
        /// <param name="y">Y coordinate of the click.</param>
        public override void receiveLeftClick(int x, int y)
        {
            if (this.greyedOut ||!this.SetButtonBounds.Contains(x, y)) { return; }      // Didn't click on button. Do nothing.
            else if (this.IsCheckListButton) { this.CheckList.CompleteTask(label); }    // Clicked on checklist button!
            else { this.Planner.CompleteTask(label); }                                  // Clicked on daily planner button!

            Game1.activeClickableMenu = new PlannerMenu(PlannerMenu);                   // Refresh the planner menu.
            Game1.soundBank.PlayCue("achievement");                                     // Play a sound!
            return;
        }

        public override void draw(SpriteBatch spriteBatch, int slotX, int slotY)
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

            Utility.drawWithShadow(
                spriteBatch, 
                Game1.mouseCursors, 
                new Vector2(this.SetButtonBounds.X + slotX, this.SetButtonBounds.Y + slotY), 
                this.SetButtonSprite, 
                Color.White, 
                0.0f, 
                Vector2.Zero,
                Game1.pixelZoom,
                false, 
                0.15f
                );
            return;
        }
    }
}
