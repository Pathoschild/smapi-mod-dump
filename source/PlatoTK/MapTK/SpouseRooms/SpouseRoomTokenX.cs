/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapTK.SpouseRooms
{
    internal class SpouseRoomTokenX
    {
        internal static readonly HashSet<SpouseRoomPlacement> SpouseRooms = new HashSet<SpouseRoomPlacement>();
        internal static IModHelper Helper;
        const int VanillaCols = 5;
        internal const int MaxSpouseRoomSpotsPerRow = 15;

        public bool IsMutable() => false;
        public bool AllowsInput() => true;
        public bool RequiresInput() => true;
        public bool CanHaveMultipleValues(string input = null) => false;
        public bool UpdateContext() => false;
        public bool IsReady() => true;

        public bool HasBoundedRangeValues(string input, out int min, out int max)
        {
            min = 0;
            max = int.MaxValue;
            return true;
        }

        public virtual IEnumerable<string> GetValues(string input)
        {
            int spot = GetVanillaSpot(input);

            if (spot != -1)
                return GetResult(spot % 5 * 6, spot / 5 * 9);

            if (SpouseRooms.FirstOrDefault(s => s.Name == input) is SpouseRoomPlacement srp)
                spot = srp.Spot;
            else
            {
                SpouseRoomPlacement newSRP = new SpouseRoomPlacement(input, GetNewSpot());
                spot = newSRP.Spot;
                SpouseRooms.Add(newSRP);
            }
            var output = GetResult(spot % MaxSpouseRoomSpotsPerRow * 6, spot / MaxSpouseRoomSpotsPerRow * 9);

            return output;
        }

        protected virtual string[] GetResult(int x, int y)
        {
            return new[] { x.ToString() };
        }

        protected virtual int GetNewSpot()
        {
            int spot = VanillaCols + SpouseRooms.Count;

            while (spot % MaxSpouseRoomSpotsPerRow < VanillaCols)
                spot++;

            return spot;
        }

        protected virtual int GetVanillaSpot(string name)
        {
            switch (name)
            {
                case "Abigail":
                    return 0;
                case "Alex":
                    return 6;
                case "Elliott":
                    return 8;
                case "Emily":
                    return 11;
                case "Haley":
                    return 3;
                case "Harvey":
                    return 7;
                case "Krobus":
                    return 12;
                case "Leah":
                    return 2;
                case "Maru":
                    return 4;
                case "Penny":
                    return 1;
                case "Sam":
                    return 9;
                case "Sebastian":
                    return 5;
                case "Shane":
                    return 10;
                default:
                    return -1;
            }
        }
    }
}
