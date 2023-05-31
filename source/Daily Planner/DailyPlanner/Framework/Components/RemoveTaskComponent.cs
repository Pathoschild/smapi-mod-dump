/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BuildABuddha/StardewDailyPlanner
**
*************************************************/

using DailyPlanner.Framework.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Text;

namespace DailyPlanner.Framework
{
    internal class RemoveTaskComponent : OptionsElement
    {
        /*********
        ** Fields
        *********/
        /// <summary>The Planner helper.</summary>
        private readonly Planner Planner;

        /// <summary>The source rectangle for the 'set' button sprite.</summary>
        private readonly Rectangle XButtonSprite = new(338, 494, 11, 11);

        /// <summary>Area on the screen that the button occupies. Used to check if we click on it.</summary>
        private Rectangle SetButtonBounds;

        /// <summary>The original menu, so it can be refreshed.</summary>
        private readonly PlannerMenu PlannerMenu;

        private readonly int Season;
        private readonly TaskType Type;
        private readonly int Day;
        private readonly string TaskName;

        /*********
        ** Public methods
        *********/
        /// <summary>Construct a button with Planner helper.</summary>
        /// <param name="season">The season of the task.</param>
        /// <param name="type">The type of task (Daily, Weekly, OnDate).</param>
        /// <param name="date">The date of the task in int form (1-28)</param>
        /// <param name="taskName">The name of the task.</param>
        /// <param name="slotWidth">The field width.</param>
        /// <param name="planner">The planner helper.</param>
        /// <param name="plannermenu">The PlannerMenu creating this button.</param>
        public RemoveTaskComponent(TaskType type, int season, int date, string taskName, int slotWidth, Planner planner, PlannerMenu plannermenu)
          : base("", -1, -1, slotWidth + 1, 11 * Game1.pixelZoom)
        {
            this.SetButtonBounds = new Rectangle(slotWidth - 18 * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 11 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            this.Planner = planner;
            this.PlannerMenu = plannermenu;
            this.Season = season;
            this.Type = type;
            this.Day = date;
            this.TaskName = taskName;
        }

        /// <summary>Construct a button with Planner helper.</summary>
        /// <param name="type">The type of task (Daily, Weekly, OnDate).</param>
        /// <param name="date">The date of the task in int form (1-28)</param>
        /// <param name="taskName">The name of the task.</param>
        /// <param name="slotWidth">The field width.</param>
        /// <param name="planner">The planner helper.</param>
        public RemoveTaskComponent(TaskType type, int date, string taskName, int slotWidth, Planner planner, PlannerMenu plannermenu)
          : base("", -1, -1, slotWidth + 1, 11 * Game1.pixelZoom)
        {
            this.SetButtonBounds = new Rectangle(slotWidth - 18 * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 11 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            this.Planner = planner;
            this.PlannerMenu = plannermenu;
            this.Season = 0;
            this.Type = type;
            this.Day = date;
            this.TaskName = taskName;
        }

        /// <summary>Called when player left clicks on the menu.</summary>
        /// <param name="x">X coordinate of the click.</param>
        /// <param name="y">Y coordinate of the click.</param>
        public override void receiveLeftClick(int x, int y)
        {
            if (this.greyedOut || !this.SetButtonBounds.Contains(x, y)) { return; }             // Didn't click on button. Do nothing.
            else { this.Planner.RemoveTask(this.Season, this.Type, this.Day, this.TaskName); }  // Clicked on daily planner button!

            this.PlannerMenu.RefreshRemoveTaskTab();                                            // Refresh the planner menu.
            Game1.soundBank.PlayCue("achievement");                                             // Play a sound!
            return;
        }

        private string GenerateLabel()
        {
            if (this.Type != TaskType.OnDate)
            {
                return $"{this.Planner.TaskTypeToString(this.Type)}, {this.Planner.SeasonIndexToName(this.Season)}: {this.TaskName}";
            }
            else
            {
                return $"{this.Planner.TaskTypeToString(this.Type)}: {this.TaskName}";
            }
        }

        public override void draw(SpriteBatch spriteBatch, int slotX, int slotY, IClickableMenu context = null)
        {
            Utility.drawTextWithShadow(
                spriteBatch,
                this.GenerateLabel(),
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
                this.XButtonSprite,
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
