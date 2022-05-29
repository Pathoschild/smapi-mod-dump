/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ceruleandeep/CeruleanStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MarketDay.Shop;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using xTile;
using xTile.ObjectModel;
using xTile.Tiles;

namespace MarketDay.Utility
{
	public static class Schedule
	{
		public static Dictionary<int, List<string>> NPCInteractions = new();

		private static Stack<Point> findPathForNPCSchedules(
			Point startPoint,
			Point endPoint,
			GameLocation location,
			int limit)
		{
			PriorityQueue priorityQueue = new PriorityQueue();
			HashSet<int> intSet = new HashSet<int>();
			int num = 0;
			priorityQueue.Enqueue(new PathNode(startPoint.X, startPoint.Y, 0, null),
				Math.Abs(endPoint.X - startPoint.X) + Math.Abs(endPoint.Y - startPoint.Y));
			PathNode pathNode1 = (PathNode) priorityQueue.Peek();
			int layerWidth = location.map.Layers[0].LayerWidth;
			int layerHeight = location.map.Layers[0].LayerHeight;
			while (!priorityQueue.IsEmpty())
			{
				PathNode pathNode2 = priorityQueue.Dequeue();
				if (pathNode2.x == endPoint.X && pathNode2.y == endPoint.Y)
					return PathFindController.reconstructPath(pathNode2);
				intSet.Add(pathNode2.id);
				for (int index = 0; index < 4; ++index)
				{
					int x = pathNode2.x + Directions[index, 0];
					int y = pathNode2.y + Directions[index, 1];
					int hash = PathNode.ComputeHash(x, y);
					if (!intSet.Contains(hash))
					{
						PathNode p = new PathNode(x, y, pathNode2);
						p.g = (byte) (pathNode2.g + 1U);
						if (p.x == endPoint.X && p.y == endPoint.Y || p.x >= 0 && p.y >= 0 &&
							(p.x < layerWidth && p.y < layerHeight) &&
							!isPositionImpassableForNPCSchedule(location, p.x, p.y))
						{
							int priority = p.g +
							               getPreferenceValueForTerrainType(location, p.x, p.y) +
							               (Math.Abs(endPoint.X - p.x) + Math.Abs(endPoint.Y - p.y) +
							                (p.x == pathNode2.x && p.x == pathNode1.x ||
							                 p.y == pathNode2.y && p.y == pathNode1.y
								                ? -2
								                : 0));
							if (!priorityQueue.Contains(p, priority))
								priorityQueue.Enqueue(p, priority);
						}
					}
				}

				pathNode1 = pathNode2;
				++num;
				if (num >= limit)
					return null;
			}

			return null;
		}

		private static readonly sbyte[,] Directions = {{-1, 0}, {1, 0}, {0, 1}, {0, -1}};

		private static bool isPositionImpassableForNPCSchedule(GameLocation loc, int x, int y)
		{
			Tile tile = loc.Map.GetLayer("Buildings").Tiles[x, y];
			if (tile != null && tile.TileIndex != -1)
			{
				tile.TileIndexProperties.TryGetValue("Action", out var propertyValue);
				if (propertyValue == null)
					tile.Properties.TryGetValue("Action", out propertyValue);
				if (propertyValue != null)
				{
					string str = propertyValue.ToString();
					if (str.StartsWith("LockedDoorWarp") || !str.Contains("Door") && !str.Contains("Passable"))
						return true;
				}
				else if (loc.doesTileHaveProperty(x, y, "Passable", "Buildings") == null &&
				         loc.doesTileHaveProperty(x, y, "NPCPassable", "Buildings") == null)
					return true;
			}

			if (loc.doesTileHaveProperty(x, y, "NoPath", "Back") != null)
				return true;
			foreach (Warp warp in loc.warps)
			{
				if (warp.X == x && warp.Y == y)
					return true;
			}

			return loc.isTerrainFeatureAt(x, y);
		}

		private static int getPreferenceValueForTerrainType(GameLocation l, int x, int y)
		{
			string str = l.doesTileHaveProperty(x, y, "Type", "Back");
			if (str != null)
			{
				string lower = str.ToLower();
				if (lower == "stone")
					return -7;
				if (lower == "wood")
					return -4;
				if (lower == "dirt")
					return -2;
				if (lower == "grass")
					return -1;
			}

			return 0;
		}

		public static Dictionary<int, SchedulePathDescription> parseMasterSchedule(NPC npc, string rawData)
		{
			var defaultPosition = MarketDay.helper.Reflection.GetField<NetVector2>(npc, "defaultPosition");
			var previousEndPoint = MarketDay.helper.Reflection.GetField<Point>(npc, "previousEndPoint");
			var _lastLoadedScheduleKey = MarketDay.helper.Reflection.GetField<string>(npc, "_lastLoadedScheduleKey");
			var getLocationRoute = MarketDay.helper.Reflection.GetMethod(npc, "getLocationRoute");

			MarketDay.Log($"parseMasterSchedule {npc.Name}: {rawData}", LogLevel.Trace, true);

			string[] scriptParts = rawData.Split('/');
			Dictionary<int, SchedulePathDescription> masterSchedule = new Dictionary<int, SchedulePathDescription>();
			int num = 0;
			if (scriptParts[0].Contains("GOTO"))
			{
				string text = scriptParts[0].Split(' ')[1];
				if (text.ToLower().Equals("season"))
				{
					text = Game1.currentSeason;
				}

				try
				{
					scriptParts = npc.getMasterScheduleRawData()[text].Split('/');
				}
				catch (Exception)
				{
					return npc.parseMasterSchedule(npc.getMasterScheduleEntry("spring"));
				}
			}

			if (scriptParts[0].Contains("NOT"))
			{
				string[] array2 = scriptParts[0].Split(' ');
				if (array2[1].ToLower() == "friendship")
				{
					int i = 2;
					bool flag = false;
					for (; i < array2.Length; i += 2)
					{
						string text2 = array2[i];
						int result = 0;
						if (int.TryParse(array2[i + 1], out result))
						{
							foreach (Farmer allFarmer in Game1.getAllFarmers())
							{
								if (allFarmer.getFriendshipHeartLevelForNPC(text2) >= result)
								{
									flag = true;
									break;
								}
							}
						}

						if (flag)
						{
							break;
						}
					}

					if (flag)
					{
						return npc.parseMasterSchedule(npc.getMasterScheduleEntry("spring"));
					}

					num++;
				}
			}
			else if (scriptParts[0].Contains("MAIL"))
			{
				string item = scriptParts[0].Split(' ')[1];
				num = !Game1.MasterPlayer.mailReceived.Contains(item) &&
				      !NetWorldState.checkAnywhereForWorldStateID(item)
					? num + 1
					: num + 2;
			}

			if (scriptParts[num].Contains("GOTO"))
			{
				string text3 = scriptParts[num].Split(' ')[1];
				if (text3.ToLower().Equals("season"))
				{
					text3 = Game1.currentSeason;
				}
				else if (text3.ToLower().Equals("no_schedule"))
				{
					npc.followSchedule = false;
					return null;
				}

				return npc.parseMasterSchedule(npc.getMasterScheduleEntry(text3));
			}

			Point startPoint = (npc.isMarried()
				? new Point(0, 23)
				: new Point((int) defaultPosition.GetValue().X / 64, (int) defaultPosition.GetValue().Y / 64));
			string startLoc = (npc.isMarried() ? "BusStop" : npc.DefaultMap);
			int time = 610;
			string text5 = npc.DefaultMap;
			int num2 = (int) (defaultPosition.GetValue().X / 64f);
			int num3 = (int) (defaultPosition.GetValue().Y / 64f);
			bool flag2 = false;
			for (int j = num; j < scriptParts.Length; j++)
			{
				if (scriptParts.Length <= 1)
				{
					break;
				}
				
				// MarketDay.Log($"{j} {scriptParts[j]}", LogLevel.Debug);

				int num4 = 0;
				string[] scriptWords = scriptParts[j].Split(' ');
				int stepTime;
				bool timeIsArrival = false;
				string stepTimeStr = scriptWords[num4];
				if (stepTimeStr.Length > 0 && scriptWords[num4][0] == 'a')
				{
					timeIsArrival = true;
					stepTimeStr = stepTimeStr[1..];
				}

				stepTime = Convert.ToInt32(stepTimeStr);
				num4++;
				string endLocationName = scriptWords[num4];
				string endBehavior = null;
				string endMessage = null;
				int endingX = 0;
				int endingY = 0;
				int faceDirection = 2;
				if (endLocationName == "bed")
				{
					if (npc.isMarried())
					{
						endLocationName = "BusStop";
						endingX = -1;
						endingY = 23;
						faceDirection = 3;
					}
					else
					{
						string text7 = null;
						if (npc.hasMasterScheduleEntry("default"))
						{
							text7 = npc.getMasterScheduleEntry("default");
						}
						else if (npc.hasMasterScheduleEntry("spring"))
						{
							text7 = npc.getMasterScheduleEntry("spring");
						}

						if (text7 != null)
						{
							try
							{
								string[] array4 = text7.Split('/')[^1].Split(' ');
								endLocationName = array4[1];
								if (array4.Length > 3)
								{
									if (!int.TryParse(array4[2], out endingX) || !int.TryParse(array4[3], out endingY))
									{
										text7 = null;
									}
								}
								else
								{
									text7 = null;
								}
							}
							catch (Exception)
							{
								text7 = null;
							}
						}

						if (text7 == null)
						{
							endLocationName = text5;
							endingX = num2;
							endingY = num3;
						}
					}

					num4++;
					Dictionary<string, string> dictionary2 =
						Game1.content.Load<Dictionary<string, string>>("Data\\animationDescriptions");
					string text8 = npc.Name.ToLower() + "_sleep";
					if (dictionary2.ContainsKey(text8))
					{
						endBehavior = text8;
					}
				}
				else
				{
					if (int.TryParse(endLocationName, out var _))
					{
						endLocationName = startLoc;
						num4--;
					}

					num4++;
					endingX = Convert.ToInt32(scriptWords[num4]);
					num4++;
					endingY = Convert.ToInt32(scriptWords[num4]);
					num4++;
					try
					{
						if (scriptWords.Length > num4)
						{
							if (int.TryParse(scriptWords[num4], out faceDirection))
							{
								num4++;
							}
							else
							{
								faceDirection = 2;
							}
						}
					}
					catch (Exception)
					{
						faceDirection = 2;
					}
				}

				// look ahead one schedule step to see how much slack time we might have
				int nextStepTime = 2600;
				int minutesAvailable = 0;
				if (j < scriptParts.Length - 1)
				{
					string[] lookahead = scriptParts[j + 1].Split(' ');
					string nextStepTimeStr = lookahead[0];
					if (nextStepTimeStr.Length > 0 && nextStepTimeStr[0] == 'a')
					{
						nextStepTimeStr = stepTimeStr.Substring(1);
					}

					nextStepTime = Convert.ToInt32(nextStepTimeStr);
					minutesAvailable =
						Math.Max(0, StardewValley.Utility.ConvertTimeToMinutes(nextStepTime) -
						            StardewValley.Utility.ConvertTimeToMinutes(stepTime));
				}

				// MarketDay.Log($"parseMasterSchedule: {npc.Name} {stepTime} {nextStepTime} {startLoc} {endLocationName}", LogLevel.Debug);


				if (changeScheduleForLocationAccessibility(npc, ref endLocationName, ref endingX, ref endingY,
					ref faceDirection))
				{
					if (npc.getMasterScheduleRawData().ContainsKey("default"))
					{
						return npc.parseMasterSchedule(npc.getMasterScheduleEntry("default"));
					}

					return npc.parseMasterSchedule(npc.getMasterScheduleEntry("spring"));
				}

				if (num4 < scriptWords.Length)
				{
					if (scriptWords[num4].Length > 0 && scriptWords[num4][0] == '"')
					{
						endMessage = scriptParts[j].Substring(scriptParts[j].IndexOf('"'));
					}
					else
					{
						endBehavior = scriptWords[num4];
						num4++;
						if (num4 < scriptWords.Length && scriptWords[num4].Length > 0 && scriptWords[num4][0] == '"')
						{
							endMessage = scriptParts[j].Substring(scriptParts[j].IndexOf('"')).Replace("\"", "");
						}
					}
				}

				if (stepTime == 0)
				{
					flag2 = true;
					text5 = endLocationName;
					num2 = endingX;
					num3 = endingY;
					startLoc = endLocationName;
					startPoint.X = endingX;
					startPoint.Y = endingY;
					previousEndPoint.SetValue(new Point(endingX, endingY));
					continue;
				}

				// here we add to the master schedule
				// which is an opportunity to add another stop for owners or spouses
				// if the location route takes them via the Town

				var shop = RouteViaOwnedShop(npc, startLoc, endLocationName, getLocationRoute, time, nextStepTime, minutesAvailable, startPoint, endBehavior, endMessage, endingX, endingY, faceDirection, masterSchedule, ref stepTime);

				if (shop is null)
				{
					var schedulePathDescription = pathfindToNextScheduleLocation(npc, stepTime,
						nextStepTime, true, startLoc, startPoint.X, startPoint.Y, endLocationName, endingX, endingY,
						faceDirection, endBehavior, endMessage);
					if (timeIsArrival)
					{
						var minutes = Minutes(schedulePathDescription);
						stepTime = Math.Max(
							StardewValley.Utility.ConvertMinutesToTime(
								StardewValley.Utility.ConvertTimeToMinutes(stepTime) - minutes), time);
					}
					
					if (stepTime == 0) MarketDay.Log($"stepTime is 0, watch for problems", LogLevel.Warn);
					if (masterSchedule.ContainsKey(stepTime)) MarketDay.Log($"stepTime {stepTime} already in schedule, watch for problems", LogLevel.Warn);

					masterSchedule.Add(stepTime, schedulePathDescription);
				}
				
				startPoint.X = endingX;
				startPoint.Y = endingY;
				startLoc = endLocationName;
				time = stepTime;
			}

			if (Game1.IsMasterGame && flag2)
			{
				Game1.warpCharacter(npc, text5, new Point(num2, num3));
			}

			if (_lastLoadedScheduleKey.GetValue() != null && Game1.IsMasterGame)
			{
				npc.dayScheduleName.Value = _lastLoadedScheduleKey.GetValue();
			}

			return masterSchedule;
		}

		private static GrangeShop RouteViaOwnedShop(NPC npc, string startLoc, string endLocationName,
			IReflectedMethod getLocationRoute, int time, int nextStepTime, int minutesAvailable, Point startPoint,
			string endBehavior, string endMessage, int endingX, int endingY, int faceDirection, Dictionary<int, SchedulePathDescription> masterSchedule,
			ref int stepTime)
		{
			if (startLoc is null || endLocationName is null) return null;

			// sorry, Krobus is not permitted in town
			if (npc.Name == "Krobus") return null;

			var viaLocations = getLocationRoute.Invoke<List<string>>(startLoc, endLocationName);
			if (viaLocations is null || !viaLocations.Contains("Town")) return null;
			
			// see if they own a shop or their spouse owns a shop
			MapUtility.ShopOwners.TryGetValue(npc.Name, out var shop);
			if (shop is null && npc.getSpouse() is not null && npc.getSpouse().Name is not null)
				MapUtility.ShopOwners.TryGetValue(npc.getSpouse().Name, out shop);

			if (shop is null) return null;

			var startToTown = pathfindToNextScheduleLocation(npc, stepTime, nextStepTime, false, startLoc,
				startPoint.X, startPoint.Y, "Town", (int) shop.OwnerTile.X, (int) shop.OwnerTile.Y, 2, null,
				null);
			var arriveInTownAt =
				StardewValley.Utility.ConvertMinutesToTime(StardewValley.Utility.ConvertTimeToMinutes(stepTime) +
				                                           Minutes(startToTown));
			var townToEnd = pathfindToNextScheduleLocation(npc, arriveInTownAt, nextStepTime, true, "Town",
				(int) shop.OwnerTile.X, (int) shop.OwnerTile.Y, endLocationName, endingX, endingY, faceDirection,
				endBehavior, endMessage);

			var leaveTownBy =
				StardewValley.Utility.ConvertMinutesToTime(
					StardewValley.Utility.ConvertTimeToMinutes(nextStepTime) - Minutes(townToEnd));
			if (leaveTownBy > MarketDay.Config.ClosingTime * 100)
			{
				// they don't need to leave the market before it closes
				// so have them stay until the end and go straight to next scheduled destination
				townToEnd = pathfindToNextScheduleLocation(npc, arriveInTownAt, nextStepTime, false, "Town",
					(int) shop.OwnerTile.X, (int) shop.OwnerTile.Y, endLocationName, endingX, endingY,
					faceDirection, endBehavior, endMessage);
				leaveTownBy = MarketDay.Config.ClosingTime * 100;
			}

			if (arriveInTownAt > MarketDay.Config.ClosingTime * 100)
			{
				// MarketDay.Log(
				// 	$"no time for that... would arrive at {arriveInTownAt} which is after closing {MarketDay.Config.ClosingTime * 100}",
				// 	LogLevel.Info);
				shop = null;
			}
			else if (arriveInTownAt >= leaveTownBy)
			{
				// MarketDay.Log(
				// 	$"no time for that... would arrive at {arriveInTownAt} but we have to leave at {leaveTownBy}",
				// 	LogLevel.Info);
				shop = null;
			}
			else
			{
				// MarketDay.Log($"We can do this... arrive at {arriveInTownAt} leave at {leaveTownBy}",
				// 	LogLevel.Info);
				AddWorkingAt(npc, arriveInTownAt, shop, leaveTownBy);
				masterSchedule.Add(stepTime, startToTown);
				masterSchedule.Add(leaveTownBy, townToEnd);
				stepTime = leaveTownBy;
			}

			return shop;
		}

		private static void AddWorkingAt(NPC npc, int stepTime, GrangeShop shop, int leaveTownBy)
		{
			if (!NPCInteractions.ContainsKey(stepTime)) NPCInteractions[stepTime] = new List<string>();
			NPCInteractions[stepTime].Add($"{npc.displayName} working at {shop.ShopName} until {leaveTownBy}");
		}

		private static void AddVisit(NPC npc, int stepTime, int leaveTownBy, string source)
		{
			if (!NPCInteractions.ContainsKey(stepTime)) NPCInteractions[stepTime] = new List<string>();
			NPCInteractions[stepTime].Add($"{npc.displayName} visiting the market");
		}

		private static double TravelTime(IEnumerable<Point> route)
		{
			int pixels = 0;
			Point? nextTile = null;
			foreach (Point tile in route)
			{
				if (!nextTile.HasValue)
				{
					nextTile = tile;
					continue;
				}

				if (Math.Abs(nextTile.Value.X - tile.X) + Math.Abs(nextTile.Value.Y - tile.Y) == 1)
				{
					pixels += 64;
				}

				nextTile = tile;
			}

			var minutes = pixels / 96.0;
			return minutes;
		}
		
		private static int Minutes(IEnumerable<Point> route)
		{
			var minutes = (int) Math.Round(TravelTime(route) / 10.0) * 10;
			return minutes;
		}

		private static int Minutes(SchedulePathDescription schedulePathDescription)
		{
			return Minutes(schedulePathDescription.route);
		}

		private static bool changeScheduleForLocationAccessibility(
			NPC npc,
			ref string locationName,
			ref int tileX,
			ref int tileY,
			ref int facingDirection)
		{
			string str = locationName;
			if (str != "JojaMart" && str != "Railroad")
			{
				if (str == "CommunityCenter") return !Game1.isLocationAccessible(locationName);
			}
			else if (!Game1.isLocationAccessible(locationName))
			{
				if (!npc.hasMasterScheduleEntry(locationName + "_Replacement"))
					return true;
				string[] strArray = npc.getMasterScheduleEntry(locationName + "_Replacement").Split(' ');
				locationName = strArray[0];
				tileX = Convert.ToInt32(strArray[1]);
				tileY = Convert.ToInt32(strArray[2]);
				facingDirection = Convert.ToInt32(strArray[3]);
			}

			return false;
		}

		private static SchedulePathDescription pathfindToNextScheduleLocation(NPC npc, int stepTime, int nextStepTime,
			bool visitShops, string startingLocation, int startingX, int startingY, string endingLocation, int endingX,
			int endingY, int finalFacingDirection, string endBehavior, string endMessage)
		{
			Stack<Point> stack = new Stack<Point>();
			Point startPoint = new Point(startingX, startingY);

			// MarketDay.Log($"pfNSL: {npc.Name} {startingLocation} {stepTime} -> {endingLocation} {nextStepTime}", LogLevel.Debug);

			//     private List<string> getLocationRoute(string startingLocation, string endingLocation)
			var getLocationRoute = MarketDay.helper.Reflection.GetMethod(npc, "getLocationRoute");

			//     private Stack<Point> addToStackForSchedule(Stack<Point> original, Stack<Point> toAdd)
			var addToStackForSchedule = MarketDay.helper.Reflection.GetMethod(npc, "addToStackForSchedule");

			List<string> routeViaLocations = !startingLocation.Equals(endingLocation, StringComparison.Ordinal)
				? getLocationRoute.Invoke<List<string>>(startingLocation, endingLocation)
				: null;
			if (routeViaLocations != null)
			{
				for (int i = 0; i < routeViaLocations.Count; i++)
				{
					GameLocation locationFromName = Game1.getLocationFromName(routeViaLocations[i]);
					if (locationFromName.Name.Equals("Trailer") &&
					    Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
					{
						locationFromName = Game1.getLocationFromName("Trailer_Big");
					}

					var divert = visitShops
					             && locationFromName.Name == "Town"
					             && stepTime < MarketDay.Config.ClosingTime * 100
					             && nextStepTime > MarketDay.Config.OpeningTime * 100;
					// MarketDay.Log($"pfNSL:     {i} {locationFromName.Name} divert: {divert}", LogLevel.Debug);

					if (i < routeViaLocations.Count - 1)
					{
						Point warpPointTo = locationFromName.getWarpPointTo(routeViaLocations[i + 1]);
						if (warpPointTo.Equals(Point.Zero) || startPoint.Equals(Point.Zero))
						{
							throw new Exception("schedule pathing tried to find a warp point that doesn't exist.");
						}
						
						var maxDuration = StardewValley.Utility.ConvertTimeToMinutes(nextStepTime) - StardewValley.Utility.ConvertTimeToMinutes(stepTime);
						
						var newPoints = divert
							? pathFindViaGrangeShops(startPoint, warpPointTo, locationFromName, 30000, maxDuration)
							: findPathForNPCSchedules(startPoint, warpPointTo, locationFromName, 30000);

						if (divert) AddVisit(npc, stepTime, nextStepTime, "pfNSL via Town"); 

						stack = addToStackForSchedule.Invoke<Stack<Point>>(stack, newPoints);
						startPoint = locationFromName.getWarpPointTarget(warpPointTo, npc);
					}
					else
					{
						stack = addToStackForSchedule.Invoke<Stack<Point>>(stack,
							PathFindController.findPathForNPCSchedules(startPoint, new Point(endingX, endingY),
								locationFromName, 30000));
					}
				}
			}
			else if (startingLocation.Equals(endingLocation, StringComparison.Ordinal))
			{
				GameLocation locationFromName2 = Game1.getLocationFromName(startingLocation);
				if (locationFromName2.Name.Equals("Trailer") &&
				    Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade"))
				{
					locationFromName2 = Game1.getLocationFromName("Trailer_Big");
				}

				// MarketDay.Log($"pathfindToNextScheduleLocation: {npc.Name} {stepTime} {nextStepTime} - {locationFromName2.Name}", LogLevel.Debug);
				if (visitShops
				    && locationFromName2.Name == "Town"
				    && stepTime < MarketDay.Config.ClosingTime * 100
				    && nextStepTime > MarketDay.Config.OpeningTime * 100)
				{
					var maxDuration = StardewValley.Utility.ConvertTimeToMinutes(nextStepTime) - StardewValley.Utility.ConvertTimeToMinutes(stepTime);
					// MarketDay.Log($"    Diverting {npc.Name} via the market (B)", LogLevel.Trace);
					stack = pathFindViaGrangeShops(startPoint, new Point(endingX, endingY), locationFromName2, 30000, maxDuration);
					AddVisit(npc, stepTime, nextStepTime, "pfNSL within Town");
				}
				else
				{
					stack = findPathForNPCSchedules(startPoint, new Point(endingX, endingY), locationFromName2, 30000);
				}

			}

			return new SchedulePathDescription(stack, finalFacingDirection, endBehavior, endMessage);
		}

		private static List<string> getLocationRoute(NPC npc, string startingLocation, string endingLocation)
		{
			// 			routesFromLocationToLocation = new List<List<string>>();

			var routesFromLocationToLocation = MarketDay.helper.Reflection
				.GetField<List<List<string>>>(npc, "routesFromLocationToLocation").GetValue();

			foreach (List<string> item in routesFromLocationToLocation)
			{
				if (item.First().Equals(startingLocation, StringComparison.Ordinal) &&
				    item.Last().Equals(endingLocation, StringComparison.Ordinal) &&
				    ((int) npc.Gender == 0 || !item.Contains<string>("BathHouse_MensLocker", StringComparer.Ordinal)) &&
				    ((int) npc.Gender != 0 || !item.Contains<string>("BathHouse_WomensLocker", StringComparer.Ordinal)))
				{
					return item;
				}
			}

			return null;
		}

		public static Stack<Point> pathFindViaGrangeShops(Point startPoint, Point endPoint, GameLocation location,
			int limit, int maxDuration)
		{
			const int STOP_DURATION_MINS = 5;
			
			Debug.Assert(location is Town, "don't pathfind via shops if it's not the town");
			
			MarketDay.Log(
				$"pathFindViaGrangeShops {location.Name} {startPoint} -> {endPoint} max duration {maxDuration}",
				LogLevel.Trace, true);

			Stack<Point> path = new Stack<Point>();
			double duration = 0;

			var placesToVisit = PlacesToVisit(startPoint);
			if (placesToVisit.Count < 2) return path;

			var waypoints = string.Join(", ", placesToVisit);
			MarketDay.Log($"    Waypoints: {waypoints}", LogLevel.Trace, true);

			// work backwards through the waypoints
			placesToVisit.Reverse();

			var thisEndPoint = endPoint;
			var i = 0;
			foreach (var (wptX, wptY) in placesToVisit)
			{
				i++;
				var thisStartPoint = new Point(wptX, wptY);
				var originalPath = findPathForNPCSchedules(thisStartPoint, thisEndPoint, location, limit);
				if (originalPath is null || originalPath.Count == 0) continue;

				duration += TravelTime(originalPath) + STOP_DURATION_MINS;

				// MarketDay.Log($"    wpt {i} of {placesToVisit.Count}  seg len {originalPath.Count} mins {TravelTime(originalPath):0.0}  {duration:0.0} {maxDuration}  {wptX} {wptY}", LogLevel.Debug);
				
				if (duration > maxDuration)
					originalPath = findPathForNPCSchedules(startPoint, thisEndPoint, location, limit);

				var legPath = originalPath.ToList();
				legPath.Reverse();
				foreach (var pt in legPath) path.Push(pt);
				thisEndPoint = thisStartPoint;

				if (duration > maxDuration) break;
			}

			return path;
		}

		/// <summary>
		/// Compare two points based on their distance to a third point
		/// </summary>
		/// <param name="start"></param>
		/// <returns></returns>
		private static Comparison<Point> DistanceToPoint(Point start)
		{
			int Comparator(Point p1, Point p2)
			{
				var (startX, startY) = start;
				var d1 = Math.Pow(startX - p1.X, 2) + Math.Pow(startY - p1.Y, 2);
				var d2 = Math.Pow(startX - p2.X, 2) + Math.Pow(startY - p2.Y, 2);
				return (int) (d1 - d2);
			}

			return Comparator;
		}

		/// <summary>
		/// List of shops for NPCs to stand in front of, in order of distance from start point.
		/// Totally unoptimized solution to a travelling salesperson situation,
		/// but who expects NPCs to act in a globally-efficient way?
		/// </summary>
		/// <param name="startPoint"></param>
		/// <returns></returns>
		private static List<Point> PlacesToVisit(Point startPoint)
		{
			var placesToVisit = new List<Point> {startPoint};

			var available = MapUtility.ShopTiles.Keys.Select(shopLoc => shopLoc.ToPoint()).ToList();

			while (available.Count > 0)
			{
				var current = placesToVisit[^1];

				// sort shops according to distance from current location
				available.Sort(DistanceToPoint(current));

				// we could choose a random shop amongst the closest 2 or 3 in the list
				// but you know what? the visitPoint randomizing probably introduces enough fuzz
				// and we can just pick whatever is closest
				var next = available.First();
				available.Remove(next);

				var visitPoint = new Point((int) next.X + Game1.random.Next(3), (int) next.Y + 4);

				if (Game1.random.NextDouble() < MarketDay.Config.StallVisitChance) placesToVisit.Add(visitPoint);
			}


			return placesToVisit;
		}

		/// <summary>
		/// Generate a complete schedule for NPCs without a schedule this day
		/// </summary>
		/// <param name="npc"></param>
		/// <param name="dayOfMonth"></param>
		/// <returns></returns>
		public static Dictionary<int, SchedulePathDescription> getScheduleWhenNoDefault(NPC npc, int dayOfMonth)
		{
			// do they have a shop of their own?
			if (MapUtility.ShopOwners.TryGetValue(npc.Name, out var shop))
			{
				// MarketDay.Log($"getScheduleWhenNoDefault: {npc.Name} has shop {shop.ShopName}", LogLevel.Debug);
				var there = $"a{MarketDay.Config.OpeningTime * 100} Town {shop.OwnerTile.X} {shop.OwnerTile.Y} 2";
				var back = $"{MarketDay.Config.ClosingTime * 100} BusStop -1 23 3";
				var schedule = $"{there}/{back}";
				return parseMasterSchedule(npc, schedule);
			}

			// are they married to a player?
			if (npc.isMarried())
			{
				// MarketDay.Log($"getScheduleWhenNoDefault: {npc.Name} is married", LogLevel.Debug);
				var spouse = npc.getSpouse()?.Name;
				
				if (spouse is not null && MapUtility.ShopOwners.TryGetValue(spouse, out var spouseShop))
				{
					// MarketDay.Log($"getScheduleWhenNoDefault: {npc.Name} married to owner of shop {spouseShop.ShopName}", LogLevel.Trace);
					var there = $"a{MarketDay.Config.OpeningTime * 100} Town {spouseShop.OwnerTile.X} {spouseShop.OwnerTile.Y} 2";
					var back = $"{MarketDay.Config.ClosingTime * 100} BusStop -1 23 3";
					var schedule = $"{there}/{back}";
					return parseMasterSchedule(npc, schedule);
				}
			}

			return new Dictionary<int, SchedulePathDescription>();
		}
	}
}