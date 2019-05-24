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
        private readonly List<string> ButtonNames = new List<string>();
        private readonly string ListenerMessage;
        private readonly bool Listening;
        private readonly bool IsCheckListButton;
        private Rectangle SetButtonBounds;

        /// <summary>The original menu, so it can be refreshed.</summary>
        private readonly PlannerMenu PlannerMenu;

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="label">The field label.</param>
        /// <param name="slotWidth">The field width.</param>
        /// <param name="onToggled">The action to perform when the button is toggled.</param>
        public DailyPlannerInputListener(string label, int slotWidth, Planner planner, PlannerMenu plannermenu)
          : base(label, -1, -1, slotWidth + 1, 11 * Game1.pixelZoom)
        {
            this.SetButtonBounds = new Rectangle(slotWidth - 28 * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 21 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            this.Planner = planner;
            this.PlannerMenu = plannermenu;
        }

        public DailyPlannerInputListener(string label, int slotWidth, CheckList checkList, PlannerMenu plannermenu)
          : base(label, -1, -1, slotWidth + 1, 11 * Game1.pixelZoom)
        {
            this.SetButtonBounds = new Rectangle(slotWidth - 28 * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 21 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            this.CheckList = checkList;
            this.PlannerMenu = plannermenu;
            this.IsCheckListButton = true;
        }

        public override void receiveLeftClick(int x, int y)
        {
            if (this.greyedOut ||!this.SetButtonBounds.Contains(x, y))
            {
                return;
            }
            else if (this.IsCheckListButton)
            {
                this.CheckList.CompleteTask(label);
                Game1.activeClickableMenu = new PlannerMenu(PlannerMenu);
                Game1.soundBank.PlayCue("achievement");
            }
            else
            {
                this.Planner.CompleteTask(label);
                Game1.activeClickableMenu = new PlannerMenu(PlannerMenu);
                Game1.soundBank.PlayCue("achievement");
                return;
            }
        }

        public override void draw(SpriteBatch spriteBatch, int slotX, int slotY)
        {
            Utility.drawTextWithShadow(spriteBatch, this.label, Game1.dialogueFont, new Vector2(this.bounds.X + slotX, this.bounds.Y + slotY), this.greyedOut ? Game1.textColor * 0.33f : Game1.textColor, 1f, 0.15f);

            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2(this.SetButtonBounds.X + slotX, this.SetButtonBounds.Y + slotY), this.SetButtonSprite, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.15f);
            if (!this.Listening)
                return;
            spriteBatch.Draw(Game1.staminaRect, new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width, Game1.graphics.GraphicsDevice.Viewport.Height), new Rectangle(0, 0, 1, 1), Color.Black * 0.75f, 0.0f, Vector2.Zero, SpriteEffects.None, 0.999f);
        }
    }
}
