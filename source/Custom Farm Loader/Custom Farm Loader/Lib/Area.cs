/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Custom_Farm_Loader.Lib.Enums;
using Microsoft.Xna.Framework;
using StardewValley;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

namespace Custom_Farm_Loader.Lib
{
    public class Area
    {
        public List<Rectangle> Exclude = new List<Rectangle>();
        public List<Rectangle> Include = new List<Rectangle>();

        public bool parseAttribute(JProperty property)
        {
            string name = property.Name;
            string value = property.Value.ToString();

            switch (name.ToLower()) {
                case "position":
                    Include.Add(new Rectangle(int.Parse(value.Split(",")[0]), int.Parse(value.Split(",")[1]), 1, 1));
                    break;
                case "area":
                    Include.Add(parseArea(value));
                    break;
                case "areainclude":
                    UtilityMisc.parseStringArray(property).ForEach(e => Include.Add(parseArea(e)));
                    break;
                case "areaexclude":
                    UtilityMisc.parseStringArray(property).ForEach(e => Exclude.Add(parseArea(e)));
                    break;
                default:
                    return false;
            }
            return true;
        }

        private static Rectangle parseArea(string value)
        {
            if(!value.Contains(","))
                throw new Exception($"Not a valid Area/Position: '{value}'");

            if (value.Contains(";")) {
                var startString = value.Split(";")[0];
                var endString = value.Split(";")[1];
                Point startPosition = new Point(int.Parse(startString.Split(",")[0]), int.Parse(startString.Split(",")[1]));
                Point endPosition = new Point(int.Parse(endString.Split(",")[0]), int.Parse(endString.Split(",")[1]));
                //Swap AreaBegin and AreaEnd when they're not aligned as expected
                if (startPosition.X > endPosition.X) {
                    startPosition.X = endPosition.X;
                    endPosition.X = int.Parse(startString.Split(",")[0]);
                }
                if (startPosition.Y > endPosition.Y) {
                    startPosition.Y = endPosition.Y;
                    endPosition.Y = int.Parse(startString.Split(",")[1]);
                }

                return new Rectangle(startPosition.X, startPosition.Y, 1 + endPosition.X - startPosition.X, 1 + endPosition.Y - startPosition.Y);
            } else {
                Point startPosition = new Point(int.Parse(value.Split(",")[0]), int.Parse(value.Split(",")[1]));

                return new Rectangle(startPosition.X, startPosition.Y, 1, 1);
            }

        }

        public bool isTileIncluded(Vector2 v)
        {
            bool tileIncluded = Include.Count == 0;

            foreach (Rectangle rectangle in Include)
                if (rectangle.Contains(v)) { tileIncluded = true; break; }

            if (!tileIncluded)
                return false;

            foreach (Rectangle rectangle in Exclude)
                if (rectangle.Contains(v)) { tileIncluded = false; break; }

            return tileIncluded;
        }
    }
}
