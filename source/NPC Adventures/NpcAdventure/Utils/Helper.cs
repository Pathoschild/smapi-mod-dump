using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using xTile.Dimensions;
using System.Diagnostics.Contracts;
using System.IO;
using NpcAdventure.Compatibility;

namespace NpcAdventure.Utils
{
    internal static partial class Helper
    {
        private static readonly char[] PossiblePathSeparators = new[] { '/', '\\', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }.Distinct().ToArray();
        private static readonly string PreferredPathSeparator = Path.DirectorySeparatorChar.ToString();
        public static bool IsNPCAtTile(GameLocation location, Vector2 tile, NPC whichNPC = null)
        {
            NPC npc = location.isCharacterAtTile(tile);

            if (whichNPC != null && npc != null)
            {
                return whichNPC.Name == npc.Name;
            }

            return npc != null;
        }

        public static bool SpouseHasBeenKissedToday(NPC spouse)
        {
            return spouse.hasBeenKissedToday.Value;
        }

        public static bool IsSpouseMarriedToFarmer(NPC spouse, Farmer farmer)
        {
            return farmer.spouse != null
                   && farmer.spouse.Equals(spouse.Name)
                   && farmer.isMarried()
                   && spouse.isMarried()
                   && spouse.getSpouse()?.spouse == spouse.Name;
        }

        public static bool CanRequestDialog(Farmer farmer, NPC npc, bool overrideKissCheck = false)
        {
            // Can't request dialogue if giftable object is in farmer's hands or npc has current dialogues
            bool forbidden = (farmer.ActiveObject != null && farmer.ActiveObject.canBeGivenAsGift()) || npc.CurrentDialogue.Count > 0;
            bool isMarried = IsSpouseMarriedToFarmer(npc, farmer);
            bool canKiss = isMarried || ((bool)TPMC.Instance?.CustomKissing.CanKissNpc(farmer, npc) && (bool)TPMC.Instance?.CustomKissing.HasRequiredFriendshipToKiss(farmer, npc));

            // Kiss spouse first if she/he facing kissable                     
            forbidden |= canKiss && !SpouseHasBeenKissedToday(npc) && (npc.FacingDirection == 3 || npc.FacingDirection == 1) && !overrideKissCheck;
            // Check for possibly marriage dialogues to show if farmer is married to this spouse
            forbidden |= isMarried && npc.shouldSayMarriageDialogue.Value && npc.currentMarriageDialogue.Count > 0;
            // Can't request dialogue for invisible, sleeaping NPCs or farmer is riding horse
            forbidden |= npc.IsInvisible || npc.isSleeping.Value || farmer.isRidingHorse();
            // If farmer wears mayor's shorts and try request dialogue of Marnie or Lewis, avoid to request dialogue (and play animations)
            forbidden |= farmer.pantsItem.Value != null && farmer.pantsItem.Value.ParentSheetIndex == 15 && (npc.Name.Equals("Lewis") || npc.Name.Equals("Marnie"));
            // Avoid request dialogue if farmer and NPC is know each other
            forbidden |= !farmer.friendshipData.ContainsKey(npc.Name) && Game1.NPCGiftTastes.ContainsKey(npc.Name);

            return !forbidden;
        }
                                          
        public static List<Point> NearPoints(Point p, int distance)
        {
            List<Point> points = new List<Point>();
            for (int x = p.X - distance; x <= p.X + distance; x++)
            {
                for (int y = p.Y - distance; y <= p.Y + distance; y++)
                {
                    if (x == p.X && y == p.Y)
                        continue;
                    points.Add(new Point(x, y));
                }
            }

            return points;
        }

        public static List<Point> SortPointsByNearest(List<Point> nearPoints, Point startTilePoint)
        {
            List<Tuple<Point, float>> nearPointsWithDistance = Helper.MapNearPointsWithDistance(nearPoints, startTilePoint);

            nearPointsWithDistance.Sort(delegate (Tuple<Point, float> p1, Tuple<Point, float> p2) {
                if (p1.Item2 == p2.Item2)
                {
                    return 0;
                }

                return -1 * p1.Item2.CompareTo(p2.Item2);
            });

            return nearPointsWithDistance.ConvertAll<Point>(
                new Converter<Tuple<Point, float>, Point>(
                    delegate (Tuple<Point, float> tp)
                    {
                        return tp.Item1;
                    }
                )
            );
        }

        public static string GetCurrentWeatherName()
        {
            if (Game1.isRaining)
                return "Rainy";
            if (Game1.isSnowing)
                return "Snowy";
            if (Game1.isLightning)
                return "Stormy";
            if (Game1.isDebrisWeather)
                return "Cloudy";

            return "Sunny";
        }

        public static bool IsWalkableTile(GameLocation l, Vector2 tile)
        {
            StardewValley.Object o = l.getObjectAtTile((int)tile.X, (int)tile.Y);
            StardewValley.Objects.Furniture furn = o as StardewValley.Objects.Furniture;
            Fence fence = o as Fence;
            Torch torch = o as Torch;

            return l.isTileOnMap(tile) 
                && !l.isTileOccupiedIgnoreFloors(tile)
                && l.isTilePassable(new Location((int)tile.X, (int)tile.Y), Game1.viewport) 
                && (!(l is Farm) || !((l as Farm).getBuildingAt(tile) != null)) 
                && ((o == null) 
                || (furn != null && furn.furniture_type.Value == 12) 
                || (fence != null && fence.isGate.Value) 
                || (torch != null) 
                || (o.ParentSheetIndex == 590));
        }

        public static float Distance(Point p1, Point p2)
        {
            return Utility.distance(p1.X, p2.X, p1.Y, p2.Y);
        }
        private static List<Tuple<Point, float>> MapNearPointsWithDistance(List<Point> nearPoints, Point startTilePoint)
        {
            List<Tuple<Point, float>> nearPointsWithDistance = new List<Tuple<Point, float>>();

            foreach (Point nearPoint in nearPoints)
            {
                nearPointsWithDistance.Add(new Tuple<Point, float>(nearPoint, Utility.distance(nearPoint.X, startTilePoint.X, nearPoint.Y, startTilePoint.Y)));
            }

            return nearPointsWithDistance;
        }

        public static void WarpTo(NPC follower, Point tilePosition)
        {
            follower.Halt();
            follower.controller = follower.temporaryController = null;
            follower.setTilePosition(tilePosition);
        }

        public static void WarpTo(NPC follower, GameLocation location, Point tilePosition)
        {
            if (follower.currentLocation == location)
                WarpTo(follower, tilePosition);

            follower.Halt();
            follower.controller = follower.temporaryController = null;
            follower.currentLocation?.characters.Remove(follower);
            follower.currentLocation = location;
            follower.setTilePosition(tilePosition);

            location.addCharacter(follower);
        }

        public static Monster GetNearestMonsterToCharacter(Character me, float tileDistance, Func<Monster, bool> extraCondition)
        {
            float thresDistance = tileDistance * 64f;
            SortedDictionary<float, Monster> nearestMonsters = new SortedDictionary<float, Monster>();

            foreach (Character c in me.currentLocation.characters)
            {
                if (!(c is Monster monster))
                    continue;

                float monsterDistance = Helper.Distance(me.GetBoundingBox().Center, monster.GetBoundingBox().Center);

                if (monsterDistance < thresDistance && !nearestMonsters.ContainsKey(monsterDistance) && extraCondition(monster))
                {
                    nearestMonsters.Add(monsterDistance, monster);
                }
            }

            if (nearestMonsters.Count > 0) 
                return nearestMonsters.Values.First();

            return null;
        }

        public static Monster GetNearestMonsterToCharacter(Character me, float tileDistance)
        {
            return GetNearestMonsterToCharacter(me, tileDistance, (m) => true);
        }

        /// <summary>
        /// Checks if spoted monster is a valid monster
        /// </summary>
        /// <param name="monster"></param>
        /// <returns></returns>
        public static bool IsValidMonster(Monster monster)
        {
            if (monster == null)
                return false;

            // Invisible monsters are invalid
            if (monster.IsInvisible)
                return false;

            // Only moving rock crab is valid
            if (monster is RockCrab crab)
                return crab.isMoving();

            // Only unarmored bug is valid
            if (monster is Bug bug)
                return !bug.isArmoredBug.Value;

            // Only live mummy is valid
            if (monster is Mummy mummy)
                return mummy.reviveTimer.Value <= 0;

            // All other monsters all valid
            return true;
        }

        public static Vector2 GetAwayFromCharacterTrajectory(Microsoft.Xna.Framework.Rectangle monsterBox, Character who)
        {
            Microsoft.Xna.Framework.Rectangle boundingBox = who.GetBoundingBox();
            double num1 = (double)-(boundingBox.Center.X - monsterBox.Center.X);
            boundingBox = who.GetBoundingBox();
            float num2 = (float)(boundingBox.Center.Y - monsterBox.Center.Y);
            float num3 = Math.Abs((float)num1) + Math.Abs(num2);
            if ((double)num3 < 1.0)
                num3 = 5f;
            return new Vector2((float)num1 / num3 * (float)(50 + Game1.random.Next(-20, 20)), num2 / num3 * (float)(50 + Game1.random.Next(-20, 20)));
        }

        public static string[] GetSegments(string path, int? limit = null)
        {
            return limit.HasValue
                ? path.Split(PossiblePathSeparators, limit.Value, StringSplitOptions.RemoveEmptyEntries)
                : path.Split(PossiblePathSeparators, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>Normalise path separators in a file path.</summary>
        /// <param name="path">The file path to normalise.</param>
        [Pure]
        public static string NormalisePathSeparators(string path)
        {
            string[] parts = GetSegments(path);
            string normalised = string.Join(PreferredPathSeparator, parts);
            if (path.StartsWith(PreferredPathSeparator))
                normalised = PreferredPathSeparator + normalised; // keep root slash
            return normalised;
        }
    }
}
