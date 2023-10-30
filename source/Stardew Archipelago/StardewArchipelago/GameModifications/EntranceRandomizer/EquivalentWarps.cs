/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewValley;

namespace StardewArchipelago.GameModifications.EntranceRandomizer
{
    public class EquivalentWarps
    {
        private const string jojaMart = "JojaMart";
        private const string abandonedJojaMart = "AbandonedJojaMart";
        private const string movieTheater = "MovieTheater";
        private const string trailer = "Trailer";
        private const string trailerBig = "Trailer_Big";
        private const string beach = "Beach";
        private const string beachNightMarket = "BeachNightMarket";
        private static string[] _jojaMartLocations = { jojaMart, abandonedJojaMart, movieTheater };
        private static string[] _trailerLocations = { trailer, trailerBig };
        private static string[] _beachLocations = { beach, beachNightMarket };

        public List<string[]> EquivalentAreas = new()
        {
            _jojaMartLocations,
            _trailerLocations,
            _beachLocations,
        };

        public EquivalentWarps()
        {
        }

        public string GetDefaultEquivalentEntrance(string entrance)
        {
            if (entrance.Contains(EntranceManager.TRANSITIONAL_STRING))
            {
                var parts = entrance.Split(EntranceManager.TRANSITIONAL_STRING);
                var area1 = parts[0];
                var area2 = parts[1];
                var defaultArea1 = GetDefaultEquivalentEntrance(area1);
                var defaultArea2 = GetDefaultEquivalentEntrance(area2);
                return $"{defaultArea1}{EntranceManager.TRANSITIONAL_STRING}{defaultArea2}";
            }

            if (IsJojaMart(entrance, out _))
            {
                return jojaMart;
            }

            if (IsTrailer(entrance, out _))
            {
                return trailer;
            }

            if (IsBeach(entrance, out _))
            {
                return beach;
            }

            return entrance;
        }

        public string GetCorrectEquivalentEntrance(string entrance)
        {
            if (entrance.Contains(EntranceManager.TRANSITIONAL_STRING))
            {
                var parts = entrance.Split(EntranceManager.TRANSITIONAL_STRING);
                var area1 = parts[0];
                var area2 = parts[1];
                var correctArea1 = GetCorrectEquivalentEntrance(area1);
                var correctArea2 = GetCorrectEquivalentEntrance(area2);
                return $"{correctArea1}{EntranceManager.TRANSITIONAL_STRING}{correctArea2}";
            }

            if (IsJojaMart(entrance, out var jojaMartCorrectEntrance))
            {
                return jojaMartCorrectEntrance;
            }

            if (IsTrailer(entrance, out var trailerCorrectEntrance))
            {
                return trailerCorrectEntrance;
            }

            if (IsBeach(entrance, out var beachCorrectEntrance))
            {
                return beachCorrectEntrance;
            }

            return entrance;
        }

        private bool IsJojaMart(string area, out string correctArea)
        {
            foreach (var jojaMartLocation in _jojaMartLocations)
            {
                if (!area.Equals(jojaMartLocation, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (Utility.doesMasterPlayerHaveMailReceivedButNotMailForTomorrow("ccMovieTheater"))
                {
                    correctArea = area.Replace(jojaMartLocation, movieTheater);
                    return true;
                }

                const int brokenJojaDoorEventId = 191393;
                if (Utility.HasAnyPlayerSeenEvent(brokenJojaDoorEventId))
                {
                    correctArea = area.Replace(jojaMartLocation, abandonedJojaMart);
                    return true;
                }

                correctArea = area.Replace(jojaMartLocation, jojaMart);
                return true;
            }

            correctArea = area;
            return false;
        }

        private bool IsTrailer(string area, out string correctArea)
        {
            foreach (var trailerLocation in _trailerLocations)
            {
                if (!area.Equals(trailerLocation, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
                {
                    correctArea = area.Replace(trailerLocation, trailerBig);
                    return true;
                }

                correctArea = area.Replace(trailerLocation, trailer);
                return true;
            }

            correctArea = area;
            return false;
        }

        private bool IsBeach(string area, out string correctArea)
        {
            foreach (var beachLocations in _beachLocations)
            {
                if (!area.Equals(beachLocations, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (Game1.dayOfMonth >= 15 && Game1.dayOfMonth <= 17 && Game1.currentSeason.Equals("winter", StringComparison.OrdinalIgnoreCase))
                {
                    correctArea = area.Replace(beachLocations, beachNightMarket);
                    return true;
                }

                correctArea = area.Replace(beachLocations, beach);
                return true;
            }

            correctArea = area;
            return false;
        }
    }
}
