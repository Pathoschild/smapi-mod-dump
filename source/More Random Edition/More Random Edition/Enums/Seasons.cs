/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Chikakoo/stardew-valley-randomizer
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace Randomizer
{
    /// <summary>
    /// An enum for the seasons
    /// </summary>
    public enum Seasons
	{
		Spring,
		Summer,
		Fall,
		Winter
	}

	public class SeasonFunctions
	{
        /// <summary>
        /// Gets the season of the game as one of the enum values
        /// </summary>
        /// <returns>The current season</returns>
        public static Seasons GetCurrentSeason()
		{
			string currentSeason = Game1.currentSeason.ToLower();
			switch(currentSeason)
			{
				case "spring": return Seasons.Spring;
                case "summer": return Seasons.Summer;
                case "fall": return Seasons.Fall;
                case "winter": return Seasons.Winter;
				default:
					Globals.ConsoleError($"Tried to parse unexpected season string: {currentSeason}!");
					return Seasons.Spring; // Default to something
            }
		}

        /// <summary>
        /// Gets a random color that fits the given season
        /// </summary>
        /// <param name="season">The season</param>
        /// <returns>The chosen color</returns>
        public static Color GetRandomColorForSeason(Seasons season)
        {
            Range SpringHueRange = new(100, 155);
            Range SummerHueRange = new(50, 65);
            Range FallHueRange = new(10, 40);
            Range WinterHueRange = new(180, 260);

            return season switch
            {
                Seasons.Spring => ImageManipulator.GetRandomColor(SpringHueRange),
                Seasons.Summer => ImageManipulator.GetRandomColor(SummerHueRange),
                Seasons.Fall => ImageManipulator.GetRandomColor(FallHueRange),
                _ => ImageManipulator.GetRandomColor(WinterHueRange)
            };
        }

        /// <summary>
        /// Gets a random color for a list of given seasons
        /// 1 in list: calls the normal function for it
        /// 2 in list: averages them out
        /// 3+ in list: uses CyanAndBlue (passes Winter over)
        /// </summary>
        /// <param name="season">The season</param>
        /// <returns>The chosen color</returns>
        public static Color GetRandomColorForSeasons(List<Seasons> seasons)
        {
            switch (seasons.Count)
            {
                case 1:
                    return GetRandomColorForSeason(seasons[0]);
                case 2:
                    var color1 = GetRandomColorForSeason(seasons[0]);
                    var color2 = GetRandomColorForSeason(seasons[1]);
                    return ImageManipulator.AverageColors(color1, color2);
                default:
                    return GetRandomColorForSeason(Seasons.Winter);
            }
        }
    }
}
