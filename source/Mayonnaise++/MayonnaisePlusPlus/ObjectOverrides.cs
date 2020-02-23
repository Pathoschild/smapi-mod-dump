using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Events;
using StardewValley.Network;
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace MayonnaisePlusPlus
{
	internal class ObjectOverrides
	{
		public static int CalculateQualityLevel(Farmer who, int sourceQuality = 0, bool bonus = false) {
			int quality = sourceQuality;
			double threshold = Loader.CONFIG.QualityThreshold;

			if (who.professions.Contains(4)) threshold += 0.2;
			if (bonus) threshold += 0.1;

			var random = new Random((int) Game1.stats.DaysPlayed);
			if (random.NextDouble() > threshold) quality /= 2;

			return quality;
		}

		public static bool PerformObjectDropInAction(ref SObject __instance, ref Item dropInItem, ref bool probe, ref Farmer who, ref bool __result) {
			__result = false;
			if (dropInItem is SObject object1) {
				if (__instance.name.Equals("Mayonnaise Machine")) {
					
					switch (object1.ParentSheetIndex) {
						case 107: // dinosaur egg!
							if (Loader.CONFIG.InfertileEggs) {
								return false;
							}
							// only accept fertile eggs if the infertile eggs option is off
							__instance.heldObject.Value = new SObject(Vector2.Zero, 807, null, false, true, false, false) {
								Quality = CalculateQualityLevel(who, object1.Quality)
							};
							if (!probe) {
								__instance.MinutesUntilReady = 180;
								who.currentLocation.playSound("Ship");
							}
							__result = true;
							break;
						case 174:
						case 182:
							__instance.heldObject.Value = new SObject(Vector2.Zero, 306, null, false, true, false, false) {
								Quality = CalculateQualityLevel(who, object1.Quality, true)
							};
							if (!probe) {
								__instance.MinutesUntilReady = 180;
								who.currentLocation.playSound("Ship");
							}
							__result = true;
							break;
						case 176:
						case 180:
							__instance.heldObject.Value = new SObject(Vector2.Zero, 306, null, false, true, false, false) {
								Quality = CalculateQualityLevel(who, Math.Min(object1.Quality, 2), object1.Quality == 4)
							};
							if (!probe) {
								__instance.MinutesUntilReady = 180;
								who.currentLocation.playSound("Ship");
							}
							__result = true;
							break;
						case 305:
							__instance.heldObject.Value = new SObject(Vector2.Zero, 308, null, false, true, false, false) {
								Quality = CalculateQualityLevel(who, object1.Quality)
							};
							if (!probe) {
								__instance.MinutesUntilReady = 180;
								who.currentLocation.playSound("Ship");
							}
							__result = true;
							break;
						case 442:
							__instance.heldObject.Value = new SObject(Vector2.Zero, 307, null, false, true, false, false) {
								Quality = CalculateQualityLevel(who, object1.Quality)
							};
							if (!probe) {
								__instance.MinutesUntilReady = 180;
								who.currentLocation.playSound("Ship", NetAudio.SoundContext.Default);
							}
							__result = true;
							break;
						default:
							if (object1.ParentSheetIndex == Loader.DATA["Blue Chicken Egg"]) {
								__instance.heldObject.Value = new SObject(Vector2.Zero, Loader.DATA["Blue Mayonnaise"], null, false, true, false, false) {
									Quality = CalculateQualityLevel(who, object1.Quality)
								};
								if (!probe) {
									__instance.MinutesUntilReady = 180;
									who.currentLocation.playSound("Ship", NetAudio.SoundContext.Default);
								}
								__result = true;
							} else if (object1.ParentSheetIndex == Loader.DATA["Dino Egg"]) {
								__instance.heldObject.Value = new SObject(Vector2.Zero, 807, null, false, true, false, false) {
									Quality = CalculateQualityLevel(who, object1.Quality)
								};
								if (!probe) {
									__instance.MinutesUntilReady = 180;
									who.currentLocation.playSound("Ship", NetAudio.SoundContext.Default);
								}
								__result = true;
							}
							break;
					}
				}
				if (__instance.name.Equals("Incubator")) {
					if (__instance.heldObject.Value == null && (object1.Category == -5 || Utility.IsNormalObjectAtParentSheetIndex((Item)object1, 107)) && object1.ParentSheetIndex != Loader.DATA["Dino Egg"]) {
						__instance.heldObject.Value = new SObject(object1.ParentSheetIndex, 1, false, -1, 0);
						if (!probe) {
							who.currentLocation.playSound("coin");
							__instance.MinutesUntilReady = 9000 * object1.ParentSheetIndex == 107 ? 2 : 1;
							if (who.professions.Contains(2))
								__instance.MinutesUntilReady /= 2;
							if (object1.ParentSheetIndex == 180 || object1.ParentSheetIndex == 182 || object1.ParentSheetIndex == 305)
								__instance.ParentSheetIndex += 2;
							else
								++__instance.ParentSheetIndex;
						}
						__result = true;
					}
				}
			}

			return !__result;
		}

		public static bool FarmAnimalDayUpdate(ref FarmAnimal __instance, GameLocation environtment) {
			__instance.controller = (PathFindController)null;
			__instance.health.Value = 3;
			bool flag1 = false;
			if (__instance.home != null && !(__instance.home.indoors.Value as AnimalHouse).animals.ContainsKey((long)__instance.myID) && environtment is Farm) {
				if (!__instance.home.animalDoorOpen.Value) {
					__instance.moodMessage.Value = 6;
					flag1 = true;
					__instance.happiness.Value /= 2;
				} else {
					(environtment as Farm).animals.Remove(__instance.myID.Value);
					(__instance.home.indoors.Value as AnimalHouse).animals.Add(__instance.myID.Value, __instance);
					if (Game1.timeOfDay > 1800 && __instance.controller == null)
						__instance.happiness.Value /= 2;
					environtment = __instance.home.indoors.Value;
					__instance.setRandomPosition(environtment);
					return false;
				}
			}
			++__instance.daysSinceLastLay.Value;
			if (!__instance.wasPet.Value) {
				__instance.friendshipTowardFarmer.Value = Math.Max(0, __instance.friendshipTowardFarmer.Value - (10 - __instance.friendshipTowardFarmer / 200));
				__instance.happiness.Value = (byte)Math.Max(0, __instance.happiness.Value - __instance.happinessDrain.Value * 5);
			}
			__instance.wasPet.Value = false;
			if ((__instance.fullness.Value < 200 || Game1.timeOfDay < 1700) && environtment is AnimalHouse) {
				for (int index = environtment.objects.Count() - 1; index >= 0; --index) {
					KeyValuePair<Vector2, SObject> keyValuePair = environtment.objects.Pairs.ElementAt(index);
					if (keyValuePair.Value.Name.Equals("Hay")) {
						OverlaidDictionary objects = environtment.objects;
						keyValuePair = environtment.objects.Pairs.ElementAt(index);
						Vector2 key = keyValuePair.Key;
						objects.Remove(key);
						__instance.fullness.Value = Byte.MaxValue;
						break;
					}
				}
			}
			var random = new Random((int) (long) __instance.myID / 2 + (int) Game1.stats.DaysPlayed);
			if (__instance.fullness > 200 || random.NextDouble() < (__instance.fullness.Value - 30) / 170.0) {
				++__instance.age.Value;
				if (__instance.age.Value == __instance.ageWhenMature.Value) {
					__instance.Sprite.LoadTexture("Animals\\" + __instance.type.Value);
					if (__instance.type.Value.Contains("Sheep"))
						__instance.currentProduce.Value = __instance.defaultProduceIndex;
					__instance.daysSinceLastLay.Value = 99;
				}
				__instance.happiness.Value = (byte)Math.Min(Byte.MaxValue, __instance.happiness.Value + __instance.happinessDrain.Value * 2);
			}
			if (__instance.fullness.Value < 200) {
				__instance.happiness.Value = (byte)Math.Max(0, __instance.happiness.Value - 100);
				__instance.friendshipTowardFarmer.Value = Math.Max(0, __instance.friendshipTowardFarmer.Value - 20);
			}
			bool flag2 = __instance.daysSinceLastLay.Value >= __instance.daysToLay.Value - (!__instance.type.Value.Equals("Sheep") || !Game1.getFarmer(__instance.ownerID.Value).professions.Contains(3) ? 0 : 1) && random.NextDouble() < __instance.fullness.Value / 200.0 && random.NextDouble() < __instance.happiness.Value / 70.0;
			int parentSheetIndex;
			if (!flag2 || __instance.age.Value < __instance.ageWhenMature.Value) {
				parentSheetIndex = -1;
			} else {
				parentSheetIndex = __instance.defaultProduceIndex.Value;
				if (parentSheetIndex == 107 && Loader.CONFIG.InfertileEggs) parentSheetIndex = Loader.DATA["Dino Egg"];
				if (random.NextDouble() < __instance.happiness.Value / 150.0) {
					float num1 = __instance.happiness.Value > 200 ? __instance.happiness.Value * 1.5f : (__instance.happiness.Value <= 100 ? __instance.happiness.Value - 100 : 0.0f);
					if (__instance.type.Value.Equals("Duck") && random.NextDouble() < (__instance.friendshipTowardFarmer.Value + num1) / 5000.0 + Game1.player.team.AverageDailyLuck((GameLocation)null) + Game1.player.team.AverageLuckLevel((GameLocation)null) * 0.01) {
						parentSheetIndex = __instance.deluxeProduceIndex.Value;
					} else if (__instance.type.Value.Equals("Rabbit") && random.NextDouble() < (__instance.friendshipTowardFarmer.Value + num1) / 5000.0 + Game1.player.team.AverageDailyLuck((GameLocation)null) + Game1.player.team.AverageLuckLevel((GameLocation)null) * 0.02) {
						parentSheetIndex = __instance.deluxeProduceIndex.Value;
						__instance.daysSinceLastLay.Value = 0;
					} else if (__instance.type.Value.Equals("Blue Chicken") && random.NextDouble() < (__instance.friendshipTowardFarmer.Value + (double)num1) / 5000.0 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() * 0.01) {
						parentSheetIndex = Loader.DATA["Blue Chicken Egg"];
					}
					switch (parentSheetIndex) {
						case 176:
							++Game1.stats.ChickenEggsLayed;
							break;
						case 180:
							++Game1.stats.ChickenEggsLayed;
							break;
						case 440:
							++Game1.stats.RabbitWoolProduced;
							break;
						case 442:
							++Game1.stats.DuckEggsLayed;
							break;
					}
					if (random.NextDouble() < (__instance.friendshipTowardFarmer.Value + num1) / 1200.0 && !__instance.type.Value.Equals("Duck") && (!__instance.type.Value.Equals("Rabbit") && !__instance.type.Value.Equals("Blue Chicken") && __instance.deluxeProduceIndex.Value != -1) && __instance.friendshipTowardFarmer.Value >= 200)
						parentSheetIndex = __instance.deluxeProduceIndex.Value;
					double num2 = __instance.friendshipTowardFarmer.Value / 1000.0 - (1.0 - __instance.happiness.Value / 225.0);
					if (!__instance.isCoopDweller() && Game1.getFarmer(__instance.ownerID.Value).professions.Contains(3) || __instance.isCoopDweller() && Game1.getFarmer(__instance.ownerID.Value).professions.Contains(2))
						num2 += 0.33;
					if (num2 >= 0.95 && random.NextDouble() < num2 / 2.0)
						__instance.produceQuality.Value = 4;
					else if (random.NextDouble() < num2 / 2.0)
						__instance.produceQuality.Value = 2;
					else if (random.NextDouble() < num2)
						__instance.produceQuality.Value = 1;
					else
						__instance.produceQuality.Value = 0;
				}
			}
			if (__instance.harvestType == 1 & flag2) {
				__instance.currentProduce.Value = parentSheetIndex;
				parentSheetIndex = -1;
			}
			if (parentSheetIndex != -1 && __instance.home != null) {
				bool flag3 = true;
				foreach (SObject @object in __instance.home.indoors.Value.objects.Values) {
					if (@object.bigCraftable.Value && @object.parentSheetIndex.Value == 165 && @object.heldObject.Value != null) {
						if ((@object.heldObject.Value as Chest).addItem(new SObject(Vector2.Zero, parentSheetIndex, null, false, true, false, false) {
							Quality = __instance.produceQuality.Value}) == null) {
							@object.showNextIndex.Value = true;
							flag3 = false;
							break;
						}
					}
				}
				if (flag3 && !__instance.home.indoors.Value.Objects.ContainsKey(__instance.getTileLocation()))
					__instance.home.indoors.Value.Objects.Add(__instance.getTileLocation(), new SObject(Vector2.Zero, parentSheetIndex, null, false, true, false, true) {
						Quality = __instance.produceQuality.Value
					});
			}
			if (!flag1) {
				if (__instance.fullness.Value < 30)
					__instance.moodMessage.Value = 4;
				else if (__instance.happiness.Value < 30)
					__instance.moodMessage.Value = 3;
				else if (__instance.happiness.Value < 200)
					__instance.moodMessage.Value = 2;
				else
					__instance.moodMessage.Value = 1;
			}
			if (Game1.timeOfDay < 1700)
				__instance.fullness.Value = (byte)Math.Max(0, __instance.fullness.Value - __instance.fullnessDrain.Value * (1700 - Game1.timeOfDay) / 100);
			__instance.fullness.Value = 0;
			if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
				__instance.fullness.Value = 250;
			__instance.reload(__instance.home);

			return false;
		}

		public static bool AddHatchedAnimal(ref AnimalHouse __instance, ref string name) {
			if (__instance.getBuilding() is Coop) {
				foreach (SObject obj in __instance.objects.Values) {
					if (obj.bigCraftable.Value && obj.Name.Contains("Incubator") && (obj.heldObject.Value != null && obj.MinutesUntilReady <= 0) && !__instance.isFull()) {
						string type = "??";
						if (obj.heldObject.Value == null) {
							type = "White Chicken";
						} else {
							switch (obj.heldObject.Value.ParentSheetIndex) {
								case 107:
									type = "Dinosaur";
									break;
								case 174:
								case 176:
									type = "White Chicken";
									break;
								case 180:
								case 182:
									type = "Brown Chicken";
									break;
								case 305:
									type = "Void Chicken";
									break;
								case 442:
									type = "Duck";
									break;
								default:
									if (obj.heldObject.Value.ParentSheetIndex == Loader.DATA["Blue Chicken Egg"])
										type = "Blue Chicken";
									break;
							}
						}

						FarmAnimal farmAnimal = new FarmAnimal(type, Loader.HELPER.Multiplayer.GetNewID(), Game1.player.UniqueMultiplayerID);
						while ((Game1.player.eventsSeen.Contains(3900074) || !type.Equals("Blue Chicken")) && !farmAnimal.type.Value.Equals(type)) {
							farmAnimal = new FarmAnimal(type, farmAnimal.myID.Value, Game1.player.UniqueMultiplayerID);
						}
						farmAnimal.Name = name;
						farmAnimal.displayName = name;
						Building building = __instance.getBuilding();
						farmAnimal.home = building;
						farmAnimal.homeLocation.Value = new Vector2(building.tileX.Value, building.tileY.Value);
						farmAnimal.setRandomPosition(farmAnimal.home.indoors.Value);
						(building.indoors.Value as AnimalHouse).animals.Add(farmAnimal.myID.Value, farmAnimal);
						(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Add(farmAnimal.myID.Value);
						obj.heldObject.Value = null;
						obj.ParentSheetIndex = 101;
						break;
					}
				}
			} else if (Game1.farmEvent != null && Game1.farmEvent is QuestionEvent) {
				var qe = Game1.farmEvent as QuestionEvent;
				FarmAnimal farmAnimal = new FarmAnimal(qe.animal.type.Value, Loader.HELPER.Multiplayer.GetNewID(), Game1.player.UniqueMultiplayerID);
				farmAnimal.Name = name;
				farmAnimal.displayName = name;
				farmAnimal.parentId.Value = qe.animal.myID.Value;
				Building building = __instance.getBuilding();
				farmAnimal.home = building;
				farmAnimal.homeLocation.Value = new Vector2(building.tileX.Value, building.tileY.Value);
				qe.forceProceed = true;
				farmAnimal.setRandomPosition(farmAnimal.home.indoors.Value);
				(building.indoors.Value as AnimalHouse).animals.Add(farmAnimal.myID.Value, farmAnimal);
				(building.indoors.Value as AnimalHouse).animalsThatLiveHere.Add(farmAnimal.myID.Value);
			}
			if (Game1.currentLocation.currentEvent != null)
				++Game1.currentLocation.currentEvent.CurrentCommand;
			Game1.exitActiveMenu();

			return false;
		}
	}
}