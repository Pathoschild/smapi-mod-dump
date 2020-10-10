/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JohnsonNicholas/SDVMods
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using TwilightShards.Stardew.Common;

namespace TwilightShards.TheStarsIncline
{
    public class SDateRange
    {
        public SDate Start;
        public SDate End;

        public SDateRange(SDate s, SDate e)
        {
            Start = s;
            End = e;
        }

        public bool DateWithinRange(SDate r)
        {
            if (r.DaysSinceStart >= Start.DaysSinceStart && r.DaysSinceStart <= End.DaysSinceStart)
                return true;

            return false;
        }
    }

    public class AstrologicalSigns
    {
        private Dictionary<SDateRange, string> Signs;

        public AstrologicalSigns()
        {
            var Signs = new Dictionary<SDateRange, string>
            {
                {new SDateRange(new SDate(1, "spring",1), new SDate(9, "spring",1)), "Aries"},
                {new SDateRange(new SDate(10, "spring",1), new SDate(19, "spring")), "Taurus"},
                {new SDateRange(new SDate(20, "spring",1), new SDate(28, "spring",1)), "Gemini"},
                {new SDateRange(new SDate(1, "summer",1), new SDate(9, "summer",1)), "Cancer"},
                {new SDateRange(new SDate(10, "summer",1), new SDate(19, "summer",1)), "Leo"},
                {new SDateRange(new SDate(20, "summer",1), new SDate(28, "summer",1)), "Virgo"},
                {new SDateRange(new SDate(1, "fall",1), new SDate(9, "fall",1)), "Libra"},
                {new SDateRange(new SDate(10, "fall",1), new SDate(19, "fall",1)), "Scorpio"},
                {new SDateRange(new SDate(20, "fall",1), new SDate(28, "fall",1)), "Sagittarius"},
                {new SDateRange(new SDate(1, "winter",1), new SDate(9, "winter",1)), "Capricorn"},
                {new SDateRange(new SDate(10, "winter",1), new SDate(19, "winter",1)), "Aquarius"},
                {new SDateRange(new SDate(20, "winter",1), new SDate(28, "winter",1)), "Pisces"}
            };
        }

        public string GetCurrentSign()
        {
            SDate currDate = new SDate(Game1.dayOfMonth, Game1.currentSeason, 1);

            foreach (var v in Signs)
            {
                if (v.Key.DateWithinRange(currDate))
                    return v.Value;
            }

            return "Ophiuchus";
        }
    }
}
