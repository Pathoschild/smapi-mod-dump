/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Question;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Network;
using StardewValley.Objects;

namespace MachineSpeedChange
{/*
    public class MachineSpeedChange : Mod
    {
        private Dictionary<StardewValley.Object, Item> objectCheck;
        private ModConfig config;
        private bool isObjectCheckPopulated;

        public override void Entry(IModHelper helper)
        {
            isObjectCheckPopulated = false;
            objectCheck = new Dictionary<StardewValley.Object, Item>();
            config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.SaveLoaded += PopulateObjects;
            helper.Events.GameLoop.UpdateTicked += IncreaseMachineSpeed; //MultipleOf(2)
            helper.Events.GameLoop.ReturnedToTitle += ReleaseObjects;
        }

        public void PopulateObjects(object sender, EventArgs e)
        {
            IList<GameLocation> locations = Game1.locations;

            for (int index = 0; index < locations.Count; ++index)
            {
                if (locations[index] is BuildableGameLocation buildableGameLocation)
                {
                    foreach (var b in buildableGameLocation.buildings)
                    {
                        foreach (var key in b.indoors.Value.Objects.Pairs)
                        {
                            if (!key.Value.Name.Equals("Twig") && !key.Value.Name.Equals("Weeds") && (!key.Value.Name.Equals("Stone") && !(key.Value is Chest)) && !(key.Value is Furniture))
                            {
                                objectCheck.Add(key.Value, (Item)key.Value.heldObject.Value);
                            }
                        }
                    }
                }
                foreach (StardewValley.Object key in locations[index].objects.Values)
                {
                    if (!key.Name.Equals("Twig") && !key.Name.Equals("Weeds") && (!key.Name.Equals("Stone") && !(key is Chest)) && !(key is Furniture))
                    {
                        objectCheck.Add(key, (Item)key.heldObject.Value);
                    }
                }
            }
            isObjectCheckPopulated = true;
        }

        public void IncreaseMachineSpeed(object sender, EventArgs e)
        {
            if (!isObjectCheckPopulated)
            {
                return;
            }

            IList<GameLocation> locations = Game1.locations;
            for (int index = 0; index < locations.Count; ++index)
            {
                if (locations[index] is BuildableGameLocation buildableGameLocation)
                {
                    foreach (var b in buildableGameLocation.buildings)
                    {
                        if (b != null && b.indoors.Value.Objects != null)
                        {
                            foreach (var @object in b.indoors.Value.Objects.Pairs)
                            {
                                if (!@object.Value.Name.Equals("Twig") && !@object.Value.Name.Equals("Weeds") &&
                                    (!@object.Value.Name.Equals("Stone") && !(@object.Value is Chest)) &&
                                    !(@object.Value is Furniture))
                                {
                                    ChangeObjectRemainingTime(@object.Value);
                                }
                            }
                        }
                    }
                }

            foreach (var @object in locations[index].objects.Values)
            {
                if (!@object.name.Equals("Twig") && !@object.name.Equals("Weeds") &&
                    (!@object.name.Equals("Stone") && !(@object is Chest)) && !(@object is Furniture))
                {
                    ChangeObjectRemainingTime(@object);
                }
            }
        }
    }
        public void ReleaseObjects(object sender, EventArgs e)
        {
            objectCheck = new Dictionary<StardewValley.Object, Item>();
            isObjectCheckPopulated = false;
        }

        public void ChangeObjectRemainingTime(StardewValley.Object obj)
        {
            Item obj1;
            if (obj.heldObject.Value != null && (!objectCheck.TryGetValue(obj, out obj1) || obj1 == null) && !obj.readyForHarvest.Value)
            {
                obj.MinutesUntilReady = GetRoundedTimeForObject(obj);
                if (objectCheck.ContainsKey(obj))
                {
                    objectCheck[obj] = (Item)obj.heldObject.Value;
                }
                else
                {
                    objectCheck.Add(obj, (Item)obj.heldObject.Value);
                }
            }
            if (objectCheck.ContainsKey(obj))
            {
                if (obj.readyForHarvest.Value)
                {
                    objectCheck[obj] = (Item)null;
                }
                else
                {
                    objectCheck[obj] = (Item)obj.heldObject.Value;
                }
            }
            else if (obj.readyForHarvest.Value)
            {
                objectCheck.Add(obj, (Item)null);
            }
            else
            {
                objectCheck.Add(obj, (Item)obj.heldObject.Value);
            }
        }

        public int GetRoundedTimeForObject(StardewValley.Object obj)
        {
            int minutesUntilReady = obj.MinutesUntilReady;
            int num1 = config.defaultPercentage;
            if (config.useAlwaysDefaultPercentage)
            {
                return minutesUntilReady * num1 / 100;
            }

            if (obj.ParentSheetIndex == 10)
            {
                num1 = config.beeHousePercentage;
            }
            else if (obj.ParentSheetIndex == 12)
            {
                num1 = config.kegPercentage;
            }
            else if (obj.ParentSheetIndex == 13)
            {
                num1 = config.furnacePercentage;
            }
            else if (obj.ParentSheetIndex == 15)
            {
                num1 = config.preserveJarPercentage;
            }
            else if (obj.ParentSheetIndex == 16)
            {
                num1 = config.cheesePressPercentage;
            }
            else if (obj.ParentSheetIndex == 17)
            {
                num1 = config.loomPercentage;
            }
            else if (obj.ParentSheetIndex == 19)
            {
                num1 = config.oilMakerPercentage;
            }
            else if (obj.ParentSheetIndex == 20)
            {
                num1 = config.recyclingMachinePercentage;
            }
            else if (obj.ParentSheetIndex == 21)
            {
                num1 = config.crystalariumPercentage;
            }
            else if (obj.ParentSheetIndex == 24)
            {
                num1 = config.mayonnaiseMachinePercentage;
            }
            else if (obj.ParentSheetIndex == 25)
            {
                num1 = config.seedMakerPercentage;
            }
            else if (obj.ParentSheetIndex == 101)
            {
                num1 = config.incubatorPercentage;
            }
            else if (obj.ParentSheetIndex == 105)
            {
                num1 = config.tapperPercentage;
            }
            else if (obj.ParentSheetIndex == 114)
            {
                num1 = config.charcoalKilnPercentage;
            }
            else if (obj.ParentSheetIndex == 128)
            {
                num1 = config.mushroomBoxPercentage;
            }
            else if (obj.ParentSheetIndex == 154)
            {
                num1 = config.wormBinPercentage;
            }
            else if (obj.ParentSheetIndex == 156)
            {
                num1 = config.slimeIncubatorPercentage;
            }
            else if (obj.ParentSheetIndex == 158)
            {
                num1 = config.slimeEggPressPercentage;
            }
            else if (obj.ParentSheetIndex == 163)
            {
                num1 = config.caskPercentage;
            }

            int num2 = (minutesUntilReady * num1 / 100);
            float num3 = 0.0f;
            if (obj is Cask cask && !objectCheck.ContainsValue((Item)cask.heldObject.Value))
            {
                num3 = cask.daysToMature.Value;
            }

            float num4 = (float)(num3 * (double)num1 / 100.0);
            if (config.timeCanBeZero && num2 >= 0 || num2 > 0 || obj is Cask && (config.timeCanBeZero && num4 >= 0.0 || num4 > 0.0))
            {
                if (obj is Cask cask1 && !objectCheck.ContainsValue((Item)cask1.heldObject.Value))
                {
                    cask1.agingRate.Set(cask1.agingRate.Value / (float)((num1 > 0 ? num1 : 1.0) / 100.0));
                }

                return num2;
            }
            if (obj is Cask cask2 && !objectCheck.ContainsValue((Item)cask2.heldObject.Value))
            {
                cask2.agingRate.Set(cask2.agingRate.Value / (float)((num1 > 0 ? num1 : 1.0) / 100.0));
            }

            return 10;
        }
    }*/
    }
