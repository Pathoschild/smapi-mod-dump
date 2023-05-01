/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace StardewArchipelago.GameModifications.EntranceRandomizer
{
    public static class EquivalentWarps
    {
        private static string[] _jojaMartLocations = new[] { "JojaMart", "AbandonedJojaMart", "MovieTheater" };
        private static string[] _trailerLocations = new[] { "Trailer", "Trailer_Big" };

        public static List<string[]> EquivalentAreas = new List<string[]>()
        {
            _jojaMartLocations,
            _trailerLocations,
        };

        public static OneWayEntrance GetCorrectEquivalentWarp(OneWayEntrance chosenWarp)
        {
            if (IsJojaMart(chosenWarp, out var jojaMartCorrectWarp))
            {
                return jojaMartCorrectWarp;
            }

            if (IsTrailer(chosenWarp, out var trailerCorrectWarp))
            {
                return trailerCorrectWarp;
            }

            return chosenWarp;
        }

        private static bool IsJojaMart(OneWayEntrance chosenWarp, out OneWayEntrance correctWarp)
        {
            correctWarp = null;
            if (!_jojaMartLocations.Contains(chosenWarp.DestinationName))
            {
                return false;
            }
            
            if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
            {
                correctWarp = Entrances.TownToMovieTheater.Item1;
                return true;
            }

            const int brokenJojaDoorEventId = 191393;
            if (Utility.HasAnyPlayerSeenEvent(brokenJojaDoorEventId))
            {
                correctWarp = Entrances.TownToAbandonedJojaMart.Item1;
                return true;
            }

            correctWarp = Entrances.TownToJojaMart.Item1;
            return true;
        }

        private static bool IsTrailer(OneWayEntrance chosenWarp, out OneWayEntrance correctWarp)
        {
            correctWarp = null;
            if (!_trailerLocations.Contains(chosenWarp.DestinationName))
            {
                return false;
            }

            if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
            {
                correctWarp = Entrances.TownToTrailerBig.Item1;
                return true;
            }

            correctWarp = Entrances.TownToTrailer.Item1;
            return true;
        }
    }
}
