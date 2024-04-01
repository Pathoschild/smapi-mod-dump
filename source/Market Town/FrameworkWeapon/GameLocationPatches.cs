/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/congha22/foodstore
**
*************************************************/

//using Microsoft.Xna.Framework;
//using StardewValley;
//using StardewValley.Characters;
//using StardewValley.Objects;
//using StardewValley.TerrainFeatures;
//using StardewValley.Tools;
//using System;
//using System.Collections.Generic;
//using xTile.Dimensions;
//using xTile.ObjectModel;
//using xTile.Tiles;
//using System.Reflection;
//using StardewValley.Mods;

//namespace MarketTown
//{
//    public class GameLocationPatches
//    {

//        public static bool checkAction_Prefix(ref GameLocation __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
//        {
//            GameLocation location = __instance;
//            ModHooks hooks = (ModHooks)typeof(Game1).GetField("hooks", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
//            __result = hooks.OnGameLocation_CheckAction(__instance, tileLocation, viewport, who, delegate
//            {
//                if (who.IsSitting())
//                {
//                    who.StopSitting();
//                    return true;
//                }
//                Microsoft.Xna.Framework.Rectangle value = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);
//                foreach (Farmer current in location.farmers)
//                {
//                    if (current != Game1.player && current.GetBoundingBox().Intersects(value) && current.checkAction(who, location))
//                    {
//                        return true;
//                    }
//                }
//                if (location.currentEvent != null && location.currentEvent.isFestival)
//                {
//                    return location.currentEvent.checkAction(tileLocation, viewport, who);
//                }
//                foreach (NPC current2 in location.characters)
//                {
//                    if (current2 != null && !current2.IsMonster && (!who.isRidingHorse() || !(current2 is Horse)) && current2.GetBoundingBox().Intersects(value) && current2.checkAction(who, location))
//                    {
//                        if (who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, carrying: false) || who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, carrying: true))
//                        {
//                            who.faceGeneralDirection(current2.getStandingPosition(), 0, opposite: false, useTileCalculations: false);
//                        }
//                        return true;
//                    }
//                }
//                //if (who.IsLocalPlayer && who.currentUpgrade != null && location.name.Equals("Farm") && tileLocation.Equals(new Location((int)(who.currentUpgrade.positionOfCarpenter.X + 32f) / 64, (int)(who.currentUpgrade.positionOfCarpenter.Y + 32f) / 64)))
//                //{
//                //    if (who.currentUpgrade.daysLeftTillUpgradeDone == 1)
//                //    {
//                //        Game1.DrawDialogue(Game1.getCharacterFromName("Robin"), Game1.content.LoadString("Data\\ExtraDialogue:Farm_RobinWorking_ReadyTomorrow"));
//                //    }
//                //    else
//                //    {
//                //        Game1.DrawDialogue(Game1.getCharacterFromName("Robin"), Game1.content.LoadString("Data\\ExtraDialogue:Farm_RobinWorking" + (Game1.random.Next(2) + 1)));
//                //    }
//                //}
//                foreach (ResourceClump current3 in location.resourceClumps)
//                {
//                    if (current3.getBoundingBox().Intersects(value) && current3.performUseAction(new Vector2(tileLocation.X, tileLocation.Y)))
//                    {
//                        return true;
//                    }
//                }
//                Vector2 vector = new Vector2(tileLocation.X, tileLocation.Y);
//                if (location.objects.ContainsKey(vector) && location.objects[vector].Type != null)
//                {
//                    if (who.isRidingHorse() && !(location.objects[vector] is Fence))
//                    {
//                        return false;
//                    }
//                    if (vector.Equals(who.Tile) && !location.objects[vector].isPassable())
//                    {
//                        Tool tool = new Pickaxe();
//                        tool.DoFunction(Game1.currentLocation, -1, -1, 0, who);
//                        if (location.objects[vector].performToolAction(tool))
//                        {
//                            location.objects[vector].performRemoveAction();
//                            location.objects[vector].dropItem(location, who.GetToolLocation(), new Vector2(who.GetBoundingBox().Center.X, who.GetBoundingBox().Center.Y));
//                            Game1.currentLocation.Objects.Remove(vector);
//                            return true;
//                        }
//                        tool = new Axe();
//                        tool.DoFunction(Game1.currentLocation, -1, -1, 0, who);
//                        if (location.objects.ContainsKey(vector) && location.objects[vector].performToolAction(tool))
//                        {
//                            location.objects[vector].performRemoveAction();
//                            location.objects[vector].dropItem(location, who.GetToolLocation(), new Vector2(who.GetBoundingBox().Center.X, who.GetBoundingBox().Center.Y));
//                            Game1.currentLocation.Objects.Remove(vector);
//                            return true;
//                        }
//                        if (!location.objects.ContainsKey(vector))
//                        {
//                            return true;
//                        }
//                        Game1.chatBox.addInfoMessage("smt");
//                    }
//                    if (location.objects.TryGetValue(vector, out StardewValley.Object value1) && (value1.Type == "Crafting" || value1.Type == "interactive"))
//                    {
//                        if (who.ActiveObject == null && location.objects[vector].checkForAction(who))
//                        {
//                            return true;
//                        }
//                        if (location.objects.ContainsKey(vector))
//                        {
//                            if (who.CurrentItem != null)
//                            {
//                                Game1.chatBox.addInfoMessage("current item " + who.CurrentItem.Name);
//                                StardewValley.Object value2 = location.objects[vector].heldObject.Value;
//                                location.objects[vector].heldObject.Value = null;
//                                bool flag = location.objects[vector].performObjectDropInAction(who.CurrentItem, probe: true, who);
//                                location.objects[vector].heldObject.Value = value2;
//                                bool flag2 = location.objects[vector].performObjectDropInAction(who.CurrentItem, probe: false, who);
//                                if (flag | flag2 && who.isMoving())
//                                {
//                                    Game1.haltAfterCheck = false;
//                                }
//                                if (flag2)
//                                {
//                                    who.reduceActiveItemByOne();
//                                    return true;
//                                }
//                                return location.objects[vector].checkForAction(who) | flag;
//                            }
//                            return location.objects[vector].checkForAction(who);
//                        }
//                    }
//                    else if (location.objects.ContainsKey(vector) && (bool)location.objects[vector].isSpawnedObject)
//                    {
//                        int quality = location.objects[vector].quality;
//                        Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)vector.X + (int)vector.Y * 777);
//                        if (who.professions.Contains(16) && location.objects[vector].isForage())
//                        {
//                            location.objects[vector].Quality = 4;
//                        }
//                        else if (location.objects[vector].isForage())
//                        {
//                            if (random.NextDouble() < (double)(who.ForagingLevel / 30f))
//                            {
//                                location.objects[vector].Quality = 2;
//                            }
//                            else if (random.NextDouble() < (double)(who.ForagingLevel / 15f))
//                            {
//                                location.objects[vector].Quality = 1;
//                            }
//                        }
//                        if ((bool)location.objects[vector].questItem && location.objects[vector].questId.Value != "0" && !who.hasQuest(location.objects[vector].questId))
//                        {
//                            return false;
//                        }
//                        if (who.couldInventoryAcceptThisItem(location.objects[vector]))
//                        {
//                            if (who.IsLocalPlayer)
//                            {
//                                location.localSound("pickUpItem");
//                                DelayedAction.playSoundAfterDelay("coin", 300);
//                            }
//                            who.animateOnce(279 + who.FacingDirection);
//                            if (!location.isFarmBuildingInterior())
//                            {
//                                if (location.objects[vector].isForage())
//                                {
//                                    who.gainExperience(2, 7);
//                                }
//                            }
//                            else
//                            {
//                                who.gainExperience(0, 5);
//                            }
//                            who.addItemToInventoryBool(location.objects[vector].getOne());
//                            Game1.stats.ItemsForaged++;
//                            if (who.professions.Contains(13) && random.NextDouble() < 0.2 && !location.objects[vector].questItem && who.couldInventoryAcceptThisItem(location.objects[vector]) && !location.isFarmBuildingInterior())
//                            {
//                                who.addItemToInventoryBool(location.objects[vector].getOne());
//                                who.gainExperience(2, 7);
//                            }
//                            location.objects.Remove(vector);
//                            return true;
//                        }
//                        location.objects[vector].Quality = quality;
//                    }
//                }
//                if (who.isRidingHorse())
//                {
//                    who.mount.checkAction(who, location);
//                    return true;
//                }
//                foreach (MapSeat current4 in location.mapSeats)
//                {
//                    if (current4.OccupiesTile(tileLocation.X, tileLocation.Y) && !current4.IsBlocked(location))
//                    {
//                        who.BeginSitting(current4);
//                        return true;
//                    }
//                }
//                foreach (KeyValuePair<Vector2, TerrainFeature> current5 in location.terrainFeatures.Pairs)
//                {
//                    if (current5.Value.getBoundingBox().Intersects(value) && current5.Value.performUseAction(current5.Key))
//                    {
//                        Game1.haltAfterCheck = false;
//                        return true;
//                    }
//                }
//                if (location.largeTerrainFeatures != null)
//                {
//                    foreach (LargeTerrainFeature current6 in location.largeTerrainFeatures)
//                    {
//                        if (current6.getBoundingBox().Intersects(value) && current6.performUseAction(new Vector2(current6.netTilePosition.X, current6.netTilePosition.Y)))
//                        {
//                            Game1.haltAfterCheck = false;
//                            return true;
//                        }
//                    }
//                }
//                string text = null;
//                Tile tile = location.map.GetLayer("Buildings").PickTile(new Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
//                if (tile != null)
//                {
//                    tile.Properties.TryGetValue("Action", out PropertyValue value3);
//                    if (value3 != null)
//                    {
//                        text = value3.ToString();
//                    }
//                }
//                if (text == null)
//                {
//                    text = location.doesTileHaveProperty(tileLocation.X, tileLocation.Y, "Action", "Buildings");
//                }
//                NPC nPC = location.isCharacterAtTile(vector + new Vector2(0f, 1f));
//                if (text != null)
//                {
//                    if (location.currentEvent == null && nPC != null && !nPC.IsInvisible && !nPC.IsMonster && (!who.isRidingHorse() || !(nPC is Horse)) && Utility.withinRadiusOfPlayer(nPC.StandingPixel.X, nPC.StandingPixel.Y, 1, who) && nPC.checkAction(who, location))
//                    {
//                        if (who.FarmerSprite.IsPlayingBasicAnimation(who.FacingDirection, who.IsCarrying()))
//                        {
//                            who.faceGeneralDirection(nPC.getStandingPosition(), 0, opposite: false, useTileCalculations: false);
//                        }
//                        return true;
//                    }
//                    return location.performAction(text, who, tileLocation);
//                }
//                if (tile != null && location.checkTileIndexAction(tile.TileIndex))
//                {
//                    return true;
//                }
//                Point value4 = new Point(tileLocation.X * 64, (tileLocation.Y - 1) * 64);
//                bool flag3 = Game1.didPlayerJustRightClick();
//                foreach (Furniture current7 in location.furniture)
//                {
//                    if (current7.boundingBox.Value.Contains((int)(vector.X * 64f), (int)(vector.Y * 64f)) && (int)current7.furniture_type != 12)
//                    {
//                        if (flag3)
//                        {
//                            if (who.ActiveObject != null && current7.performObjectDropInAction(who.ActiveObject, probe: false, who))
//                            {
//                                return true;
//                            }

//                            if (who.CurrentTool != null && who.CurrentTool is MeleeWeapon && current7.performObjectDropInAction(who.CurrentTool, probe: false, who))
//                            {
//                                Game1.chatBox.addInfoMessage("is weapon");
//                                return true;
//                            }
//                            return current7.checkForAction(who);
//                        }
//                        return current7.clicked(who);
//                    }
//                    if ((int)current7.furniture_type == 6 && current7.boundingBox.Value.Contains(value4))
//                    {
//                        if (flag3)
//                        {
//                            if (who.ActiveObject != null && current7.performObjectDropInAction(who.ActiveObject, probe: false, who))
//                            {
//                                return true;
//                            }
//                            return current7.checkForAction(who);
//                        }
//                        return current7.clicked(who);
//                    }
//                }
//                return false;
//            });

//            return false;
//        }

//    }
//}