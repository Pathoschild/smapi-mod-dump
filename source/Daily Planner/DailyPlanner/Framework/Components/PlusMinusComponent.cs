/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BuildABuddha/StardewDailyPlanner
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace DailyPlanner.Framework
{

    class PlusMinusComponent : OptionsPlusMinus
    {
        private readonly int MinValue;
        private readonly List<string> OptionsList;
        private readonly PlannerMenu Menu;

        /// <summary>
        /// An plus/minus selection component that returns a number from a min value to a max value.
        /// </summary>
        /// <param name="minValue">Minimum value for the slider</param>
        /// <param name="maxValue">Maximum value for the slider</param>
        /// <param name="labelPredicate">Label to apply to the slider</param>
        /// <param name="menu">The menu that this slider is applied to</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PlusMinusComponent(int minValue, int maxValue, string labelPredicate, PlannerMenu menu)
            : base(labelPredicate, 0,
                  Enumerable.Range(minValue, maxValue).ToList().ConvertAll<string>(x => x.ToString()), 
                  Enumerable.Range(minValue, maxValue).ToList().ConvertAll<string>(x => x.ToString()))
        {
            this.MinValue = minValue;
            this.Menu = menu;
            this.OptionsList = Enumerable.Range(minValue, maxValue).ToList().ConvertAll<string>(x => x.ToString());
        }

        public PlusMinusComponent(List<string> optionsList, string labelPredicate, PlannerMenu menu)
            : base(labelPredicate, 0, optionsList, optionsList)
        {
            this.MinValue = 0;
            this.Menu = menu;
            this.OptionsList = optionsList;
        }

        /// <summary>
        /// Called when the player is done adjusting the slider.
        /// </summary>
        /// <param name="x">X pos of click</param>
        /// <param name="y">Y pos of click</param>
        public override void leftClickReleased(int x, int y)
        {
            this.Menu.RefreshRemoveTaskTab();
            base.leftClickReleased(x, y);
        }

        /// <summary>
        /// Returns the output of the slider in int form. If constructed with a string list, returns the index of the output.
        /// </summary>
        /// <returns></returns>
        public int GetOutputInt()
        {
            return this.selected + this.MinValue;
        }

        /// <summary>
        /// Returns the output of the slider in string form. If constructed with a min and max int, converts the output int to a string.
        /// </summary>
        /// <returns></returns>
        public string GetOutputString()
        {
            return this.OptionsList[this.selected + this.MinValue];
        }
    }
}
