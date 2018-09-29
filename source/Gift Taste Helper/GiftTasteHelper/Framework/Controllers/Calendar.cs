using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Menus;

namespace GiftTasteHelper.Framework
{
    internal class Calendar
    {
        /*********
        ** Properties
        *********/
        public static int InvalidDay => -1;

        private Billboard Billboard;
        private List<ClickableTextureComponent> CalendarDays;

        /// <summary>Simplifies access to private game code.</summary>
        private IReflectionHelper Reflection;


        /*********
        ** Accessors
        *********/
        public bool IsOpen { get; set; }
        public bool IsInitialized { get; private set; }
        public Rectangle Bounds { get; private set; }


        /*********
        ** Public methods
        *********/
        public void Init(Billboard menu, IReflectionHelper reflection)
        {
            this.Clear();

            this.Billboard = menu;
            this.Reflection = reflection;
            this.Bounds = new Rectangle(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height);
            this.CalendarDays = menu.calendarDays;
            this.IsInitialized = true;
        }

        public void OnResize(Billboard menu)
        {
            if (this.IsInitialized)
            {
                // We seem to lose our billboard ref on re-size, so get it back
                this.Billboard = menu;
                this.Bounds = new Rectangle(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.width, menu.height);
                this.CalendarDays = menu.calendarDays;
            }
            else
            {
                this.Init(menu, this.Reflection);
            }
        }

        public string GetCurrentHoverText()
        {
            return this.Reflection.GetField<string>(this.Billboard, "hoverText").GetValue();
        }

        public int GetHoveredDayIndex(SVector2 mouse)
        {
            if (!this.Bounds.Contains(mouse.ToPoint()))
                return Calendar.InvalidDay;

            for (int i = 0; i < this.CalendarDays.Count; ++i)
            {
                if (this.CalendarDays[i].bounds.Contains(mouse.ToPoint()))
                {
                    return i;
                }
            }
            return Calendar.InvalidDay;
        }

        public string GetHoveredBirthdayNpcName(SVector2 mouse)
        {
            string name = string.Empty;
            if (!this.Bounds.Contains(mouse.ToPoint()))
                return name;

            foreach (ClickableTextureComponent day in this.CalendarDays)
            {
                if (day.bounds.Contains(mouse.ToPoint()))
                {
                    if (day.hoverText.Length > 0 && day.hoverText.Contains("Birthday"))
                    {
                        name = day.hoverText;
                        break;
                    }
                }
            }
            return name;
        }


        /*********
        ** Private methods
        *********/
        private void Clear()
        {
            this.Billboard = null;
            if (this.CalendarDays != null)
            {
                this.CalendarDays.Clear();
                this.CalendarDays = null;
            }
            this.IsOpen = false;
            this.IsInitialized = false;
        }
    }
}
