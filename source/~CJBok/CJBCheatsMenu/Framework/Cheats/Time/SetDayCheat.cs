/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CJBok/SDV-Mods
**
*************************************************/

using System.Collections.Generic;
using CJBCheatsMenu.Framework.Components;
using StardewValley;
using StardewValley.Menus;

namespace CJBCheatsMenu.Framework.Cheats.Time
{
    /// <summary>A cheat which sets the current day.</summary>
    internal class SetDayCheat : BaseDateCheat
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Get the config UI fields to show in the cheats menu.</summary>
        /// <param name="context">The cheat context.</param>
        public override IEnumerable<OptionsElement> GetFields(CheatContext context)
        {
            yield return new CheatsOptionsSlider(
                label: I18n.Date_Day(),
                value: Game1.dayOfMonth,
                minValue: 1,
                maxValue: 28,
                setValue: this.SafelySetDay,
                width: 100
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Safely transition to the given day.</summary>
        /// <param name="day">The day.</param>
        private void SafelySetDay(int day)
        {
            this.SafelySetDate(day, Game1.currentSeason, Game1.year);
        }
    }
}
