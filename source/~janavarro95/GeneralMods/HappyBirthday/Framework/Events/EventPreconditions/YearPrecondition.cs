/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.StardustCore.Events.Preconditions;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework.Events.EventPreconditions
{
    public class YearPrecondition : EventPrecondition
    {
        public const string EventPreconditionId = "Omegasis.HappyBirthday.Framework.EventPreconditions.YearPrecondition";

        /// <summary>
        /// Determines how the <see cref="Game1.year"/> value should be in terms of <see cref="yearValue"/>'s value.
        /// </summary>
        public enum YearPreconditionType
        {
            LessThan,
            LessThanOrEqualTo,
            EqualTo,
            GreaterTo,
            GreaterThanOrEqualTo
        }

        public YearPreconditionType yearPreconditionType;

        public int yearValue;

        public YearPrecondition()
        {

        }

        public YearPrecondition(int year, YearPreconditionType yearPreconditionType)
        {
            this.yearValue = year;
            this.yearPreconditionType = yearPreconditionType;
        }

        public override string ToString()
        {
            return EventPreconditionId + " " + this.yearValue.ToString() + " " +this.yearPreconditionType.ToString();
        }

        public override bool meetsCondition()
        {
            switch (this.yearPreconditionType)
            {
                case YearPreconditionType.LessThan:
                    return Game1.year < this.yearValue;
                case YearPreconditionType.LessThanOrEqualTo:
                    return Game1.year <= this.yearValue;
                case YearPreconditionType.EqualTo:
                    return Game1.year == this.yearValue;
                case YearPreconditionType.GreaterTo:
                    return Game1.year > this.yearValue;
                case YearPreconditionType.GreaterThanOrEqualTo:
                    return Game1.year >= this.yearValue;
                default:
                    return false;
            }
        }

    }
}
