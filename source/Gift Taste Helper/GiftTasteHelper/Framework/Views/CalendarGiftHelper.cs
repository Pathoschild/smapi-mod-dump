using System.Collections.Generic;
using System.Diagnostics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace GiftTasteHelper.Framework
{
    internal class CalendarGiftHelper : GiftHelper
    {
        /*********
        ** Properties
        *********/
        /// <summary>The underlying calendar.</summary>
        private readonly Calendar Calendar = new Calendar();
        private readonly Dictionary<int, NPC> Birthdays = new Dictionary<int, NPC>();

        // The currently hovered day.
        private int HoveredDay = Calendar.InvalidDay;
        private string SeasonLastOpenedOn = null;

        /*********
        ** Public methods
        *********/
        public CalendarGiftHelper(IGiftDataProvider dataProvider, GiftConfig config, IReflectionHelper reflection, ITranslationHelper translation)
            : base(GiftHelperType.Calendar, dataProvider, config, reflection, translation) { }

        public override void Init(IClickableMenu menu)
        {
            Debug.Assert(!this.Calendar.IsInitialized, "Calendar is already initialized");

            this.SeasonLastOpenedOn = Game1.currentSeason;

            LoadBirthdays();

            base.Init(menu);
        }

        public override void Reset()
        {
            LoadBirthdays();

            this.SeasonLastOpenedOn = Game1.currentSeason;
        }

        public override bool OnOpen(IClickableMenu menu)
        {
            // The daily quest board logic is also in the billboard, so check for that
            bool isDailyQuestBoard = this.Reflection.GetField<bool>(menu, "dailyQuestBoard").GetValue();
            if (isDailyQuestBoard)
            {
                Utils.DebugLog("[OnOpen] Daily quest board was opened; ignoring.");
                return false;
            }

            Debug.Assert(!this.Calendar.IsOpen);

            // The calendar/billboard's internal data is re-initialized every time it's opened
            // So we need to update ours as well.
            this.Calendar.Init((Billboard)menu, this.Reflection);
            this.Calendar.IsOpen = true;
            this.HoveredDay = Calendar.InvalidDay;

            // This is mainly to handle the season changing with the debug command.
            if (this.SeasonLastOpenedOn != Game1.currentSeason)
            {
                Utils.DebugLog("Rebuilding birthdays since season changed since last opening the calendar");
                Reset();
            }

            Utils.DebugLog("[OnOpen] Opening calendar");

            return base.OnOpen(menu);
        }

        public override void OnResize(IClickableMenu menu)
        {
            if (this.Calendar.IsOpen && this.Calendar.IsInitialized)
            {
                Utils.DebugLog("[OnResize] Re-Initializing calendar");
                this.Calendar.OnResize((Billboard)menu);
            }
        }

        public override void OnClose()
        {
            this.Calendar.IsOpen = false;

            base.OnClose();
        }

        public override void OnCursorMoved(CursorMovedEventArgs e)
        {
            Debug.Assert(this.Calendar.IsOpen, "OnCursorMoved being called but the calendar isn't open");

            // This gets the scaled mouse position
            SVector2 mouse = new SVector2(e.NewPosition.ScreenPixels.X, e.NewPosition.ScreenPixels.Y);

            int hoveredDay = this.Calendar.GetHoveredDayIndex(mouse) + 1; // Days start at one
            if (hoveredDay == this.HoveredDay)
            {
                return;
            }

            this.HoveredDay = hoveredDay;
            if (this.Birthdays.ContainsKey(hoveredDay))
            {
                string npcName = this.Birthdays[hoveredDay].Name;
                this.DrawCurrentFrame = SetSelectedNPC(npcName);
            }
            else
            {
                this.DrawCurrentFrame = false;
            }
        }

        public override void OnDraw()
        {
            // Draw the tooltip
            this.DrawGiftTooltip(this.CurrentGiftDrawData, this.TooltipTitle(), this.Calendar.GetCurrentHoverText());
        }

        private void LoadBirthdays()
        {
            // Store all valid npc birthdays for the current season.
            this.Birthdays.Clear();
            foreach (NPC npc in Utility.getAllCharacters())
            {
                if (npc.Birthday_Season == Game1.currentSeason &&
                    this.GiftDrawDataProvider.HasDataForNpc(npc.Name) &&
                    !this.Birthdays.ContainsKey(npc.Birthday_Day)) // getAllCharacters can contain duplicates (if you break your save)
                {
                    this.Birthdays.Add(npc.Birthday_Day, npc);
                }
            }
        }
    }
}
