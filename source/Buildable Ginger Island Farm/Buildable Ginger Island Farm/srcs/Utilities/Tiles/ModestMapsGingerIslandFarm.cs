/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mouahrara/BuildableGingerIslandFarm
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BuildableGingerIslandFarm.Utilities
{
	internal class TilesModestMapsGingerIslandFarmUtility
	{
		internal static HashSet<Point> GetInaccessibleAreasTiles()
		{
			HashSet<Point> tiles = TilesDefaultUtility.GetInaccessibleAreasTiles();

			tiles.Add(new Point(103, 41));
			tiles.Add(new Point(104, 41));
			tiles.Add(new Point(105, 41));
			tiles.Add(new Point(106, 41));
			tiles.Add(new Point(107, 41));
			tiles.Add(new Point(108, 41));
			return tiles;
		}

		internal static HashSet<Point> GetFarmAreaTiles()
		{
			HashSet<Point> tiles = TilesDefaultUtility.GetFarmAreaTiles();

			tiles.Add(new Point(79, 7));
			tiles.Add(new Point(80, 7));
			tiles.Add(new Point(81, 7));
			tiles.Add(new Point(82, 7));
			tiles.Add(new Point(83, 7));
			tiles.Add(new Point(84, 7));
			tiles.Add(new Point(85, 7));
			tiles.Add(new Point(86, 7));
			tiles.Add(new Point(87, 7));
			tiles.Add(new Point(88, 7));
			tiles.Add(new Point(89, 7));
			tiles.Add(new Point(90, 7));
			tiles.Add(new Point(79, 8));
			tiles.Add(new Point(80, 8));
			tiles.Add(new Point(81, 8));
			tiles.Add(new Point(82, 8));
			tiles.Add(new Point(83, 8));
			tiles.Add(new Point(84, 8));
			tiles.Add(new Point(85, 8));
			tiles.Add(new Point(86, 8));
			tiles.Add(new Point(87, 8));
			tiles.Add(new Point(88, 8));
			tiles.Add(new Point(89, 8));
			tiles.Add(new Point(90, 8));
			tiles.Add(new Point(81, 9));
			tiles.Add(new Point(82, 9));
			tiles.Add(new Point(83, 9));
			tiles.Add(new Point(84, 9));
			tiles.Add(new Point(85, 9));
			tiles.Add(new Point(86, 9));
			tiles.Add(new Point(87, 9));
			tiles.Add(new Point(88, 9));
			tiles.Add(new Point(89, 9));
			tiles.Add(new Point(90, 9));
			tiles.Add(new Point(91, 9));
			tiles.Add(new Point(92, 9));
			tiles.Add(new Point(89, 10));
			tiles.Add(new Point(90, 10));
			tiles.Add(new Point(91, 10));
			tiles.Add(new Point(92, 10));
			tiles.Add(new Point(91, 11));
			tiles.Add(new Point(92, 11));
			tiles.Add(new Point(91, 12));
			tiles.Add(new Point(92, 12));
			tiles.Add(new Point(81, 39));
			tiles.Add(new Point(82, 39));
			tiles.Add(new Point(101, 39));
			tiles.Add(new Point(101, 40));
			tiles.Add(new Point(97, 41));
			tiles.Add(new Point(98, 41));
			tiles.Add(new Point(99, 41));
			tiles.Add(new Point(101, 41));
			tiles.Add(new Point(55, 42));
			tiles.Add(new Point(56, 42));
			tiles.Add(new Point(57, 42));
			tiles.Add(new Point(58, 42));
			tiles.Add(new Point(59, 42));
			tiles.Add(new Point(60, 42));
			tiles.Add(new Point(61, 42));
			tiles.Add(new Point(62, 42));
			tiles.Add(new Point(63, 42));
			tiles.Add(new Point(64, 42));
			tiles.Add(new Point(65, 42));
			tiles.Add(new Point(70, 42));
			tiles.Add(new Point(86, 42));
			tiles.Add(new Point(87, 42));
			tiles.Add(new Point(88, 42));
			tiles.Add(new Point(89, 42));
			tiles.Add(new Point(90, 42));
			tiles.Add(new Point(91, 42));
			tiles.Add(new Point(92, 42));
			tiles.Add(new Point(93, 42));
			tiles.Add(new Point(94, 42));
			tiles.Add(new Point(95, 42));
			tiles.Add(new Point(96, 42));
			tiles.Add(new Point(97, 42));
			tiles.Add(new Point(98, 42));
			tiles.Add(new Point(101, 42));
			tiles.Add(new Point(19, 43));
			tiles.Add(new Point(49, 43));
			tiles.Add(new Point(55, 43));
			tiles.Add(new Point(56, 43));
			tiles.Add(new Point(57, 43));
			tiles.Add(new Point(58, 43));
			tiles.Add(new Point(59, 43));
			tiles.Add(new Point(60, 43));
			tiles.Add(new Point(61, 43));
			tiles.Add(new Point(62, 43));
			tiles.Add(new Point(63, 43));
			tiles.Add(new Point(64, 43));
			tiles.Add(new Point(65, 43));
			tiles.Add(new Point(70, 43));
			tiles.Add(new Point(71, 43));
			tiles.Add(new Point(72, 43));
			tiles.Add(new Point(73, 43));
			tiles.Add(new Point(74, 43));
			tiles.Add(new Point(75, 43));
			tiles.Add(new Point(76, 43));
			tiles.Add(new Point(77, 43));
			tiles.Add(new Point(78, 43));
			tiles.Add(new Point(79, 43));
			tiles.Add(new Point(80, 43));
			tiles.Add(new Point(81, 43));
			tiles.Add(new Point(82, 43));
			tiles.Add(new Point(83, 43));
			tiles.Add(new Point(84, 43));
			tiles.Add(new Point(85, 43));
			tiles.Add(new Point(86, 43));
			tiles.Add(new Point(87, 43));
			tiles.Add(new Point(88, 43));
			tiles.Add(new Point(89, 43));
			tiles.Add(new Point(90, 43));
			tiles.Add(new Point(91, 43));
			tiles.Add(new Point(92, 43));
			tiles.Add(new Point(93, 43));
			tiles.Add(new Point(94, 43));
			tiles.Add(new Point(95, 43));
			tiles.Add(new Point(96, 43));
			tiles.Add(new Point(97, 43));
			tiles.Add(new Point(98, 43));
			tiles.Add(new Point(101, 43));
			tiles.Add(new Point(32, 44));
			tiles.Add(new Point(33, 44));
			tiles.Add(new Point(34, 44));
			tiles.Add(new Point(35, 44));
			tiles.Add(new Point(50, 44));
			tiles.Add(new Point(51, 44));
			tiles.Add(new Point(57, 44));
			tiles.Add(new Point(58, 44));
			tiles.Add(new Point(59, 44));
			tiles.Add(new Point(60, 44));
			tiles.Add(new Point(70, 44));
			tiles.Add(new Point(71, 44));
			tiles.Add(new Point(72, 44));
			tiles.Add(new Point(73, 44));
			tiles.Add(new Point(74, 44));
			tiles.Add(new Point(75, 44));
			tiles.Add(new Point(76, 44));
			tiles.Add(new Point(77, 44));
			tiles.Add(new Point(78, 44));
			tiles.Add(new Point(79, 44));
			tiles.Add(new Point(80, 44));
			tiles.Add(new Point(81, 44));
			tiles.Add(new Point(82, 44));
			tiles.Add(new Point(83, 44));
			tiles.Add(new Point(84, 44));
			tiles.Add(new Point(85, 44));
			tiles.Add(new Point(86, 44));
			tiles.Add(new Point(87, 44));
			tiles.Add(new Point(88, 44));
			tiles.Add(new Point(89, 44));
			tiles.Add(new Point(90, 44));
			tiles.Add(new Point(91, 44));
			tiles.Add(new Point(92, 44));
			tiles.Add(new Point(93, 44));
			tiles.Add(new Point(94, 44));
			tiles.Add(new Point(95, 44));
			tiles.Add(new Point(96, 44));
			tiles.Add(new Point(97, 44));
			tiles.Add(new Point(98, 44));
			tiles.Add(new Point(101, 44));
			tiles.Add(new Point(33, 45));
			tiles.Add(new Point(34, 45));
			tiles.Add(new Point(35, 45));
			tiles.Add(new Point(36, 45));
			tiles.Add(new Point(70, 45));
			tiles.Add(new Point(71, 45));
			tiles.Add(new Point(72, 45));
			tiles.Add(new Point(73, 45));
			tiles.Add(new Point(74, 45));
			tiles.Add(new Point(75, 45));
			tiles.Add(new Point(76, 45));
			tiles.Add(new Point(77, 45));
			tiles.Add(new Point(78, 45));
			tiles.Add(new Point(79, 45));
			tiles.Add(new Point(80, 45));
			tiles.Add(new Point(81, 45));
			tiles.Add(new Point(82, 45));
			tiles.Add(new Point(83, 45));
			tiles.Add(new Point(84, 45));
			tiles.Add(new Point(85, 45));
			tiles.Add(new Point(86, 45));
			tiles.Add(new Point(87, 45));
			tiles.Add(new Point(88, 45));
			tiles.Add(new Point(89, 45));
			tiles.Add(new Point(90, 45));
			tiles.Add(new Point(91, 45));
			tiles.Add(new Point(92, 45));
			tiles.Add(new Point(93, 45));
			tiles.Add(new Point(94, 45));
			tiles.Add(new Point(95, 45));
			tiles.Add(new Point(96, 45));
			tiles.Add(new Point(97, 45));
			tiles.Add(new Point(98, 45));
			tiles.Add(new Point(101, 45));
			tiles.Add(new Point(29, 46));
			tiles.Add(new Point(33, 46));
			tiles.Add(new Point(34, 46));
			tiles.Add(new Point(35, 46));
			tiles.Add(new Point(65, 46));
			tiles.Add(new Point(70, 46));
			tiles.Add(new Point(71, 46));
			tiles.Add(new Point(72, 46));
			tiles.Add(new Point(73, 46));
			tiles.Add(new Point(74, 46));
			tiles.Add(new Point(75, 46));
			tiles.Add(new Point(76, 46));
			tiles.Add(new Point(77, 46));
			tiles.Add(new Point(78, 46));
			tiles.Add(new Point(79, 46));
			tiles.Add(new Point(80, 46));
			tiles.Add(new Point(81, 46));
			tiles.Add(new Point(82, 46));
			tiles.Add(new Point(83, 46));
			tiles.Add(new Point(84, 46));
			tiles.Add(new Point(85, 46));
			tiles.Add(new Point(86, 46));
			tiles.Add(new Point(87, 46));
			tiles.Add(new Point(88, 46));
			tiles.Add(new Point(89, 46));
			tiles.Add(new Point(90, 46));
			tiles.Add(new Point(91, 46));
			tiles.Add(new Point(92, 46));
			tiles.Add(new Point(93, 46));
			tiles.Add(new Point(94, 46));
			tiles.Add(new Point(95, 46));
			tiles.Add(new Point(96, 46));
			tiles.Add(new Point(97, 46));
			tiles.Add(new Point(98, 46));
			tiles.Add(new Point(101, 46));
			tiles.Add(new Point(34, 47));
			tiles.Add(new Point(101, 47));
			tiles.Add(new Point(101, 48));
			tiles.Add(new Point(101, 49));
			tiles.Add(new Point(101, 50));
			tiles.Add(new Point(101, 51));
			tiles.Add(new Point(101, 52));
			tiles.Add(new Point(101, 53));
			tiles.Add(new Point(101, 54));
			tiles.Add(new Point(26, 55));
			tiles.Add(new Point(29, 55));
			tiles.Add(new Point(33, 55));
			tiles.Add(new Point(101, 55));
			tiles.Add(new Point(30, 56));
			tiles.Add(new Point(31, 56));
			tiles.Add(new Point(101, 56));
			tiles.Add(new Point(101, 57));
			tiles.Add(new Point(29, 58));
			tiles.Add(new Point(30, 58));
			tiles.Add(new Point(31, 58));
			tiles.Add(new Point(101, 58));
			tiles.Add(new Point(29, 59));
			tiles.Add(new Point(101, 59));
			tiles.Add(new Point(101, 60));
			tiles.Add(new Point(29, 61));
			tiles.Add(new Point(101, 61));
			tiles.Add(new Point(29, 62));
			tiles.Add(new Point(101, 62));
			tiles.Add(new Point(29, 63));
			tiles.Add(new Point(101, 63));
			tiles.Add(new Point(29, 64));
			tiles.Add(new Point(30, 64));
			tiles.Add(new Point(101, 64));
			tiles.Add(new Point(30, 65));
			tiles.Add(new Point(101, 65));
			tiles.Add(new Point(30, 66));
			tiles.Add(new Point(31, 66));
			tiles.Add(new Point(101, 66));
			tiles.Add(new Point(30, 67));
			tiles.Add(new Point(31, 67));
			tiles.Add(new Point(101, 67));
			tiles.Add(new Point(30, 68));
			tiles.Add(new Point(31, 68));
			tiles.Add(new Point(32, 68));
			tiles.Add(new Point(51, 68));
			tiles.Add(new Point(52, 68));
			tiles.Add(new Point(53, 68));
			tiles.Add(new Point(54, 68));
			tiles.Add(new Point(55, 68));
			tiles.Add(new Point(56, 68));
			tiles.Add(new Point(57, 68));
			tiles.Add(new Point(58, 68));
			tiles.Add(new Point(59, 68));
			tiles.Add(new Point(99, 68));
			tiles.Add(new Point(100, 68));
			tiles.Add(new Point(101, 68));
			tiles.Add(new Point(31, 69));
			tiles.Add(new Point(32, 69));
			tiles.Add(new Point(33, 69));
			tiles.Add(new Point(51, 69));
			tiles.Add(new Point(52, 69));
			tiles.Add(new Point(53, 69));
			tiles.Add(new Point(54, 69));
			tiles.Add(new Point(55, 69));
			tiles.Add(new Point(56, 69));
			tiles.Add(new Point(57, 69));
			tiles.Add(new Point(99, 69));
			tiles.Add(new Point(100, 69));
			tiles.Add(new Point(101, 69));
			tiles.Add(new Point(31, 70));
			tiles.Add(new Point(32, 70));
			tiles.Add(new Point(33, 70));
			tiles.Add(new Point(34, 70));
			tiles.Add(new Point(35, 70));
			tiles.Add(new Point(52, 70));
			tiles.Add(new Point(53, 70));
			tiles.Add(new Point(54, 70));
			tiles.Add(new Point(55, 70));
			tiles.Add(new Point(56, 70));
			tiles.Add(new Point(64, 70));
			tiles.Add(new Point(65, 70));
			tiles.Add(new Point(86, 70));
			tiles.Add(new Point(87, 70));
			tiles.Add(new Point(99, 70));
			tiles.Add(new Point(100, 70));
			tiles.Add(new Point(101, 70));
			tiles.Add(new Point(32, 71));
			tiles.Add(new Point(33, 71));
			tiles.Add(new Point(34, 71));
			tiles.Add(new Point(35, 71));
			tiles.Add(new Point(36, 71));
			tiles.Add(new Point(37, 71));
			tiles.Add(new Point(38, 71));
			tiles.Add(new Point(39, 71));
			tiles.Add(new Point(40, 71));
			tiles.Add(new Point(41, 71));
			tiles.Add(new Point(42, 71));
			tiles.Add(new Point(43, 71));
			tiles.Add(new Point(44, 71));
			tiles.Add(new Point(45, 71));
			tiles.Add(new Point(46, 71));
			tiles.Add(new Point(47, 71));
			tiles.Add(new Point(48, 71));
			tiles.Add(new Point(49, 71));
			tiles.Add(new Point(51, 71));
			tiles.Add(new Point(52, 71));
			tiles.Add(new Point(53, 71));
			tiles.Add(new Point(54, 71));
			tiles.Add(new Point(55, 71));
			tiles.Add(new Point(56, 71));
			tiles.Add(new Point(63, 71));
			tiles.Add(new Point(64, 71));
			tiles.Add(new Point(65, 71));
			tiles.Add(new Point(70, 71));
			tiles.Add(new Point(71, 71));
			tiles.Add(new Point(81, 71));
			tiles.Add(new Point(82, 71));
			tiles.Add(new Point(83, 71));
			tiles.Add(new Point(84, 71));
			tiles.Add(new Point(85, 71));
			tiles.Add(new Point(86, 71));
			tiles.Add(new Point(87, 71));
			tiles.Add(new Point(88, 71));
			tiles.Add(new Point(89, 71));
			tiles.Add(new Point(99, 71));
			tiles.Add(new Point(100, 71));
			tiles.Add(new Point(101, 71));
			tiles.Add(new Point(35, 72));
			tiles.Add(new Point(36, 72));
			tiles.Add(new Point(37, 72));
			tiles.Add(new Point(38, 72));
			tiles.Add(new Point(39, 72));
			tiles.Add(new Point(40, 72));
			tiles.Add(new Point(41, 72));
			tiles.Add(new Point(42, 72));
			tiles.Add(new Point(43, 72));
			tiles.Add(new Point(44, 72));
			tiles.Add(new Point(45, 72));
			tiles.Add(new Point(46, 72));
			tiles.Add(new Point(47, 72));
			tiles.Add(new Point(48, 72));
			tiles.Add(new Point(49, 72));
			tiles.Add(new Point(50, 72));
			tiles.Add(new Point(51, 72));
			tiles.Add(new Point(52, 72));
			tiles.Add(new Point(53, 72));
			tiles.Add(new Point(54, 72));
			tiles.Add(new Point(55, 72));
			tiles.Add(new Point(56, 72));
			tiles.Add(new Point(63, 72));
			tiles.Add(new Point(64, 72));
			tiles.Add(new Point(65, 72));
			tiles.Add(new Point(70, 72));
			tiles.Add(new Point(71, 72));
			tiles.Add(new Point(72, 72));
			tiles.Add(new Point(73, 72));
			tiles.Add(new Point(80, 72));
			tiles.Add(new Point(81, 72));
			tiles.Add(new Point(82, 72));
			tiles.Add(new Point(83, 72));
			tiles.Add(new Point(84, 72));
			tiles.Add(new Point(85, 72));
			tiles.Add(new Point(86, 72));
			tiles.Add(new Point(87, 72));
			tiles.Add(new Point(88, 72));
			tiles.Add(new Point(89, 72));
			tiles.Add(new Point(90, 72));
			tiles.Add(new Point(91, 72));
			tiles.Add(new Point(99, 72));
			tiles.Add(new Point(100, 72));
			tiles.Add(new Point(101, 72));
			tiles.Add(new Point(44, 73));
			tiles.Add(new Point(45, 73));
			tiles.Add(new Point(46, 73));
			tiles.Add(new Point(47, 73));
			tiles.Add(new Point(48, 73));
			tiles.Add(new Point(49, 73));
			tiles.Add(new Point(50, 73));
			tiles.Add(new Point(51, 73));
			tiles.Add(new Point(52, 73));
			tiles.Add(new Point(53, 73));
			tiles.Add(new Point(54, 73));
			tiles.Add(new Point(55, 73));
			tiles.Add(new Point(59, 73));
			tiles.Add(new Point(60, 73));
			tiles.Add(new Point(61, 73));
			tiles.Add(new Point(62, 73));
			tiles.Add(new Point(63, 73));
			tiles.Add(new Point(64, 73));
			tiles.Add(new Point(65, 73));
			tiles.Add(new Point(70, 73));
			tiles.Add(new Point(71, 73));
			tiles.Add(new Point(72, 73));
			tiles.Add(new Point(73, 73));
			tiles.Add(new Point(74, 73));
			tiles.Add(new Point(75, 73));
			tiles.Add(new Point(76, 73));
			tiles.Add(new Point(77, 73));
			tiles.Add(new Point(78, 73));
			tiles.Add(new Point(79, 73));
			tiles.Add(new Point(80, 73));
			tiles.Add(new Point(81, 73));
			tiles.Add(new Point(82, 73));
			tiles.Add(new Point(83, 73));
			tiles.Add(new Point(84, 73));
			tiles.Add(new Point(85, 73));
			tiles.Add(new Point(86, 73));
			tiles.Add(new Point(87, 73));
			tiles.Add(new Point(88, 73));
			tiles.Add(new Point(89, 73));
			tiles.Add(new Point(90, 73));
			tiles.Add(new Point(91, 73));
			tiles.Add(new Point(92, 73));
			tiles.Add(new Point(93, 73));
			tiles.Add(new Point(99, 73));
			tiles.Add(new Point(100, 73));
			tiles.Add(new Point(101, 73));
			tiles.Add(new Point(45, 74));
			tiles.Add(new Point(46, 74));
			tiles.Add(new Point(47, 74));
			tiles.Add(new Point(48, 74));
			tiles.Add(new Point(49, 74));
			tiles.Add(new Point(50, 74));
			tiles.Add(new Point(51, 74));
			tiles.Add(new Point(52, 74));
			tiles.Add(new Point(53, 74));
			tiles.Add(new Point(54, 74));
			tiles.Add(new Point(55, 74));
			tiles.Add(new Point(56, 74));
			tiles.Add(new Point(57, 74));
			tiles.Add(new Point(58, 74));
			tiles.Add(new Point(59, 74));
			tiles.Add(new Point(60, 74));
			tiles.Add(new Point(61, 74));
			tiles.Add(new Point(62, 74));
			tiles.Add(new Point(63, 74));
			tiles.Add(new Point(64, 74));
			tiles.Add(new Point(65, 74));
			tiles.Add(new Point(70, 74));
			tiles.Add(new Point(71, 74));
			tiles.Add(new Point(72, 74));
			tiles.Add(new Point(73, 74));
			tiles.Add(new Point(74, 74));
			tiles.Add(new Point(75, 74));
			tiles.Add(new Point(76, 74));
			tiles.Add(new Point(77, 74));
			tiles.Add(new Point(78, 74));
			tiles.Add(new Point(79, 74));
			tiles.Add(new Point(80, 74));
			tiles.Add(new Point(81, 74));
			tiles.Add(new Point(82, 74));
			tiles.Add(new Point(83, 74));
			tiles.Add(new Point(84, 74));
			tiles.Add(new Point(85, 74));
			tiles.Add(new Point(86, 74));
			tiles.Add(new Point(87, 74));
			tiles.Add(new Point(88, 74));
			tiles.Add(new Point(89, 74));
			tiles.Add(new Point(90, 74));
			tiles.Add(new Point(91, 74));
			tiles.Add(new Point(92, 74));
			tiles.Add(new Point(93, 74));
			tiles.Add(new Point(94, 74));
			tiles.Add(new Point(95, 74));
			tiles.Add(new Point(96, 74));
			tiles.Add(new Point(97, 74));
			tiles.Add(new Point(98, 74));
			tiles.Add(new Point(101, 74));
			tiles.Add(new Point(101, 83));
			tiles.Add(new Point(52, 85));
			tiles.Add(new Point(53, 85));
			tiles.Add(new Point(54, 85));
			tiles.Add(new Point(55, 85));
			tiles.Add(new Point(52, 86));
			tiles.Add(new Point(53, 86));
			tiles.Add(new Point(54, 86));
			tiles.Add(new Point(55, 86));
			tiles.Add(new Point(52, 87));
			tiles.Add(new Point(53, 87));
			tiles.Add(new Point(54, 87));
			tiles.Add(new Point(55, 87));
			tiles.Add(new Point(52, 88));
			tiles.Add(new Point(53, 88));
			tiles.Add(new Point(54, 88));
			tiles.Add(new Point(55, 88));
			tiles.Add(new Point(52, 89));
			tiles.Add(new Point(53, 89));
			tiles.Add(new Point(54, 89));
			tiles.Add(new Point(55, 89));
			return tiles;
		}

		internal static HashSet<Point> GetSlimeAreaTiles()
		{
			HashSet<Point> tiles = TilesDefaultUtility.GetSlimeAreaTiles();

			tiles.Add(new Point(64, 29));
			tiles.Add(new Point(58, 37));
			tiles.Add(new Point(59, 37));
			tiles.Add(new Point(60, 37));
			tiles.Add(new Point(61, 37));
			tiles.Add(new Point(62, 37));
			tiles.Add(new Point(63, 37));
			return tiles;
		}
	}
}
