/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/WeaponsOnDisplay
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using xTile.Dimensions;
using xTile.ObjectModel;
using xTile.Tiles;
using System.Reflection;
using StardewValley.Mods;
using StardewValley.Buildings;
using StardewValley.Audio;
using StardewValley.Monsters;
using StardewValley.GameData;

namespace WeaponsOnDisplay
{
	public class GameLocationPatches
	{
		public static bool checkAction_Prefix(ref GameLocation __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
		{
			GameLocation location = __instance;
			ModHooks hooks = (ModHooks) typeof(Game1).GetField("hooks", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			__result = hooks.OnGameLocation_CheckAction(__instance, tileLocation, viewport, who, delegate
			{
				who.ignoreItemConsumptionThisFrame = false;
			Microsoft.Xna.Framework.Rectangle tileRect = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);
			if (!location.objects.ContainsKey(new Vector2((float)tileLocation.X, (float)tileLocation.Y)) && location.CheckPetAnimal(tileRect, who))
			{
				return true;
			}
			using (List<Building>.Enumerator enumerator = location.buildings.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.doAction(new Vector2((float)tileLocation.X, (float)tileLocation.Y), who))
					{
						return true;
					}
				}
			}
			if (who.IsSitting())
			{
				who.StopSitting(true);
				return true;
			}
			foreach (Farmer farmer in location.farmers)
			{
				if (farmer != Game1.player && farmer.GetBoundingBox().Intersects(tileRect) && farmer.checkAction(who, location))
				{
					return true;
				}
			}
			if (location.currentEvent != null && location.currentEvent.isFestival)
			{
				return location.currentEvent.checkAction(tileLocation, viewport, who);
			}
			foreach (NPC i in location.characters)
			{
				if (i != null && !i.IsMonster && (!who.isRidingHorse() || !(i is Horse)) && i.GetBoundingBox().Intersects(tileRect) && i.checkAction(who, location))
				{
					if (who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, false) || who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, true))
					{
						who.faceGeneralDirection(i.getStandingPosition(), 0, false, false);
					}
					return true;
				}
			}
			int tid = location.getTileIndexAt(tileLocation, "Buildings");
			if (location.NameOrUniqueName.Equals("SkullCave") && (tid == 344 || tid == 349))
			{
				if (Game1.player.team.SpecialOrderActive("QiChallenge10"))
				{
					who.doEmote(40);
					return false;
				}
				if (!Game1.player.team.completedSpecialOrders.Contains("QiChallenge10"))
				{
					who.doEmote(8);
					return false;
				}
				if (!Game1.player.team.toggleSkullShrineOvernight.Value)
				{
					if (!Game1.player.team.skullShrineActivated.Value)
					{
						location.createQuestionDialogue(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_NotYetHard"), location.createYesNoResponses(), "ShrineOfSkullChallenge");
					}
					else
					{
						Game1.player.team.toggleSkullShrineOvernight.Value = true;
						Game1.showGlobalMessage(Game1.content.LoadString("Strings\\Locations:ChallengeShrine_Activated"));
						Game1.Multiplayer.globalChatInfoMessage(Game1.player.team.skullShrineActivated.Value ? "HardModeSkullCaveDeactivated" : "HardModeSkullCaveActivated", new string[] { who.Name });
						location.playSound(Game1.player.team.skullShrineActivated.Value ? "skeletonStep" : "serpentDie", null, null, SoundContext.Default);
					}
				}
				else if (Game1.player.team.toggleSkullShrineOvernight.Value && Game1.player.team.skullShrineActivated.Value)
				{
					Game1.player.team.toggleSkullShrineOvernight.Value = false;
					Game1.showGlobalMessage(Game1.content.LoadString("Strings\\UI:PendingProposal_Canceling"));
					location.playSound("skeletonStep", null, null, SoundContext.Default);
				}
				return true;
			}
			else
			{
				foreach (ResourceClump stump in location.resourceClumps)
				{
					if (stump.getBoundingBox().Intersects(tileRect) && stump.performUseAction(new Vector2((float)tileLocation.X, (float)tileLocation.Y)))
					{
						return true;
					}
				}

				Vector2 tilePos = new Vector2((float)tileLocation.X, (float)tileLocation.Y);
				if (location.objects.TryGetValue(tilePos, out StardewValley.Object obj))
				{
					bool isErrorItem = ItemRegistry.GetDataOrErrorItem(obj.QualifiedItemId).IsErrorItem;
					if (obj.Type != null || isErrorItem)
					{
						if (who.isRidingHorse() && !(obj is Fence))
						{
							return false;
						}
						if (tilePos == who.Tile && !obj.isPassable())
						{
							Fence fence = obj as Fence;
							if (fence == null || !fence.isGate.Value)
							{
								Tool t = ItemRegistry.Create<Tool>("(T)Pickaxe", 1, 0, false);
								t.DoFunction(Game1.currentLocation, -1, -1, 0, who);
								if (obj.performToolAction(t))
								{
									obj.performRemoveAction();
									obj.dropItem(location, who.GetToolLocation(false), Utility.PointToVector2(who.StandingPixel));
									Game1.currentLocation.Objects.Remove(tilePos);
									return true;
								}
								t = ItemRegistry.Create<Tool>("(T)Axe", 1, 0, false);
								t.DoFunction(Game1.currentLocation, -1, -1, 0, who);
								if (location.objects.TryGetValue(tilePos, out obj) && obj.performToolAction(t))
								{
									obj.performRemoveAction();
									obj.dropItem(location, who.GetToolLocation(false), Utility.PointToVector2(who.StandingPixel));
									Game1.currentLocation.Objects.Remove(tilePos);
									return true;
								}
								if (!location.objects.TryGetValue(tilePos, out obj))
								{
									return true;
								}
							}
						}
						if (location.objects.TryGetValue(tilePos, out obj) && (obj.Type == "Crafting" || obj.Type == "interactive"))
						{
							if (who.ActiveObject == null && obj.checkForAction(who, false))
							{
								return true;
							}
							if (location.objects.TryGetValue(tilePos, out obj))
							{
								if (who.CurrentItem == null)
								{
									return obj.checkForAction(who, false);
								}
								StardewValley.Object old_held_object = obj.heldObject.Value;
								obj.heldObject.Value = null;
								bool probe_returned_true = obj.performObjectDropInAction(who.CurrentItem, true, who, false);
								obj.heldObject.Value = old_held_object;
								bool perform_returned_true = obj.performObjectDropInAction(who.CurrentItem, false, who, true);
								if ((probe_returned_true || perform_returned_true) && who.isMoving())
								{
									Game1.haltAfterCheck = false;
								}
								if (who.ignoreItemConsumptionThisFrame)
								{
									return true;
								}
								if (perform_returned_true)
								{
									who.reduceActiveItemByOne();
									return true;
								}
								return obj.checkForAction(who, false) || probe_returned_true;
							}
						}
						else if (location.objects.TryGetValue(tilePos, out obj) && (obj.IsSpawnedObject || isErrorItem))
						{
							int oldQuality = obj.Quality;
							Random r = Utility.CreateDaySaveRandom((double)tilePos.X, (double)(tilePos.Y * 777f), 0.0);
							if (who.professions.Contains(16) && obj.isForage())
							{
								obj.Quality = 4;
							}
							else if (obj.isForage())
							{
								if (r.NextDouble() < (double)((float)who.ForagingLevel / 30f))
								{
									obj.Quality = 2;
								}
								else if (r.NextDouble() < (double)((float)who.ForagingLevel / 15f))
								{
									obj.Quality = 1;
								}
							}
							if (obj.questItem.Value && obj.questId.Value != null && obj.questId.Value != "0" && !who.hasQuest(obj.questId.Value))
							{
								return false;
							}
							if (who.couldInventoryAcceptThisItem(obj))
							{
								if (who.IsLocalPlayer)
								{
									location.localSound("pickUpItem", null, null, SoundContext.Default);
									DelayedAction.playSoundAfterDelay("coin", 300, null, null, -1, false);
								}
								who.animateOnce(279 + who.FacingDirection);
								if (!location.isFarmBuildingInterior())
								{
									if (obj.isForage())
									{
										if (obj.SpecialVariable == 724519)
										{
											who.gainExperience(2, 2);
											who.gainExperience(0, 3);
										}
										else
										{
											who.gainExperience(2, 7);
										}
									}
									if (obj.ItemId.Equals("789") && location.Name.Equals("LewisBasement"))
									{
										Bat b = new Bat(Vector2.Zero, -789);
										b.focusedOnFarmers = true;
										Game1.changeMusicTrack("none", false, MusicContext.Default);
										location.playSound("cursed_mannequin", null, null, SoundContext.Default);
										location.characters.Add(b);
									}
								}
								else
								{
									who.gainExperience(0, 5);
								}
								who.addItemToInventoryBool(obj.getOne(), false);
								Stats stats = Game1.stats;
								uint itemsForaged = stats.ItemsForaged;
								stats.ItemsForaged = itemsForaged + 1U;
								if (who.professions.Contains(13) && r.NextDouble() < 0.2 && !obj.questItem.Value && who.couldInventoryAcceptThisItem(obj) && !location.isFarmBuildingInterior())
								{
									who.addItemToInventoryBool(obj.getOne(), false);
									who.gainExperience(2, 7);
								}
								location.objects.Remove(tilePos);
								return true;
							}
							obj.Quality = oldQuality;
						}
					}
				}

				if (who.isRidingHorse())
				{
					who.mount.checkAction(who, location);
					return true;
				}
				foreach (KeyValuePair<Vector2, TerrainFeature> v in location.terrainFeatures.Pairs)
				{
					if (v.Value.getBoundingBox().Intersects(tileRect) && v.Value.performUseAction(v.Key))
					{
						Game1.haltAfterCheck = false;
						return true;
					}
				}
				if (location.largeTerrainFeatures != null)
				{
					foreach (LargeTerrainFeature f in location.largeTerrainFeatures)
					{
						if (f.getBoundingBox().Intersects(tileRect) && f.performUseAction(f.Tile))
						{
							Game1.haltAfterCheck = false;
							return true;
						}
					}
				}

				Tile tile = location.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
				string action = null;
				if (tile != null && tile.Properties.TryGetValue("Action", out PropertyValue actionValue))
				{
					action = actionValue;
				}

				if (tile == null || action == null)
				{
					action = location.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings", false);
				}
				if (action != null)
				{
					NPC characterAtTile = location.isCharacterAtTile(tilePos + new Vector2(0f, 1f));
					if (location.currentEvent == null && characterAtTile != null && !characterAtTile.IsInvisible && !characterAtTile.IsMonster && (!who.isRidingHorse() || !(characterAtTile is Horse)))
					{
						Point characterPixel = characterAtTile.StandingPixel;
						if (Utility.withinRadiusOfPlayer(characterPixel.X, characterPixel.Y, 1, who) && characterAtTile.checkAction(who, location))
						{
							if (who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, who.IsCarrying()))
							{
								who.faceGeneralDirection(Utility.PointToVector2(characterPixel), 0, false, false);
							}
							return true;
						}
					}
					return location.performAction(action, who, tileLocation);
				}
				if (tile != null && location.checkTileIndexAction(tile.TileIndex))
				{
					return true;
				}
				foreach (MapSeat seat in location.mapSeats)
				{
					if (seat.OccupiesTile(tileLocation.X, tileLocation.Y) && !seat.IsBlocked(location))
					{
						who.BeginSitting(seat);
						return true;
					}
				}
				Point vectOnWall = new Point(tileLocation.X * 64, (tileLocation.Y - 1) * 64);
				bool didRightClick = Game1.didPlayerJustRightClick(false);
				Furniture paintingFound = null;
				foreach (Furniture f2 in location.furniture)
				{
					if (f2.boundingBox.Value.Contains((int)(tilePos.X * 64f), (int)(tilePos.Y * 64f)) && f2.furniture_type.Value != 12)
					{
						if (!didRightClick)
						{
							return f2.clicked(who);
						}
						if (who.ActiveObject != null && f2.performObjectDropInAction(who.ActiveObject, false, who, false))
						{
							return true;
						}

						if (who.CurrentTool != null && (who.CurrentTool is MeleeWeapon || who.CurrentTool is Slingshot) && f2.performObjectDropInAction(who.CurrentTool, false, who))
						{
							return true;
						}
						return f2.checkForAction(who, false);
					}
					else if (f2.furniture_type.Value == 6 && f2.boundingBox.Value.Contains(vectOnWall))
					{
						paintingFound = f2;
					}
				}
				if (paintingFound == null)
				{
					return Game1.didPlayerJustRightClick(true) && location.animals.Length > 0 && location.CheckInspectAnimal(tileRect, who);
				}
				if (didRightClick)
				{
					return (who.ActiveObject != null && paintingFound.performObjectDropInAction(who.ActiveObject, false, who, false)) || paintingFound.checkForAction(who, false);
				}
				return paintingFound.clicked(who);
			}
		});

			return false;
		}
	}
}