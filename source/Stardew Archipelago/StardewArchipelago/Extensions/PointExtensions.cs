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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewArchipelago.GameModifications.EntranceRandomizer;

namespace StardewArchipelago.Extensions
{
    public static class PointExtensions
    {
        public static int GetTotalDistance(this Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }

        public static bool IsCloseEnough(this Point point1, Point point2, int acceptableDistance)
        {
            return GetTotalDistance(point1, point2) <= acceptableDistance;
        }

        public static FacingDirection GetFacingAwayFrom(this Point currentPoint, Point otherPoint)
        {
            if (currentPoint == otherPoint)
            {
                return FacingDirection.Down;
            }

            if (currentPoint.Y == otherPoint.Y)
            {
                return currentPoint.X > otherPoint.X ? FacingDirection.Right : FacingDirection.Left;
            }

            return currentPoint.Y > otherPoint.Y ? FacingDirection.Down : FacingDirection.Up;
        }

        public static Point GetAveragePoint(this List<Point> pointGroup)
        {
            var totalX = 0.0;
            var totalY = 0.0;
            foreach (var point in pointGroup)
            {
                totalX += point.X;
                totalY += point.Y;
            }

            var averageX = (int)Math.Round(totalX / pointGroup.Count);
            var averageY = (int)Math.Round(totalY / pointGroup.Count);
            return new Point(averageX, averageY);
        }

        public static List<Point> RemoveDuplicates(this IEnumerable<Point> points, bool eliminateAdjacents)
        {
            if (!eliminateAdjacents)
            {
                return points.Distinct().ToList();
            }

            var groups = new List<List<Point>>();
            foreach (var point in points)
            {
                var chosenGroups = groups.Where(group => group.Any(x => x.GetTotalDistance(point) <= 1)).ToList();

                if (!chosenGroups.Any())
                {
                    var newGroup = new List<Point> { point };
                    groups.Add(newGroup);
                    continue;
                }

                if (chosenGroups.Count == 1)
                {
                    chosenGroups[0].Add(point);
                    continue;
                }

                var fusedGroup = new List<Point>();
                foreach (var group in chosenGroups)
                {
                    fusedGroup.AddRange(group);
                    groups.Remove(group);
                }
                fusedGroup.Add(point);
                groups.Add(fusedGroup);
            }

            var averagePoints = new List<Point>();
            foreach (var group in groups)
            {
                averagePoints.Add(group.GetAveragePoint());
            }

            return averagePoints;
        }
    }
}
