using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using SObject = StardewValley.Object;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="FarmAnimal"/> class.</summary>
    internal static class FarmAnimalPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The transpile for the constructor so it doesn't call ReloadData() before the PostFix below can sort out custom animal types.</summary>
        /// <remarks>Transpile is used instead of prefix as the prefix was called before the field's default values were set.</remarks>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        internal static IEnumerable<CodeInstruction> ConstructorTranspile(IEnumerable<CodeInstruction> instructions)
        {
            var codeInstructions = instructions.ToList();
            var skipNextInstruction = false;
            var patchCompleted = false;

            for (int i = 0; i < codeInstructions.Count; i++)
            {
                if (skipNextInstruction)
                {
                    skipNextInstruction = false;
                    continue;
                }

                var instruction = codeInstructions[i];

                if (!patchCompleted)
                {
                    var nextInstruction = codeInstructions[i + 1];

                    if (nextInstruction.operand is MethodInfo)
                    {
                        // ensure to remove both instructions so stack doesn't get corrupt
                        if (instruction.opcode == OpCodes.Ldarg_0 && nextInstruction.opcode == OpCodes.Callvirt && (nextInstruction.operand as MethodInfo).Name == "reloadData")
                        {
                            skipNextInstruction = true;
                            patchCompleted = true;
                            continue;
                        }
                    }
                }

                yield return instruction;
            }
        }

        /// <summary>The post fix for the constructor.</summary>
        /// <param name="type">The animal type being constructed.</param>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        internal static void ConstructorPostFix(string type, FarmAnimal __instance)
        {
            // check if the type is custom
            var animal = ModEntry.Animals.Where(animal => animal.Name == type).FirstOrDefault();
            if (animal != null)
            {
                var subType = animal.SubTypes[Game1.random.Next(animal.SubTypes.Count())];
                __instance.type.Value = subType.Name;
            }

            __instance.reloadData();
        }

        /// <summary>The prefix for the ReloadData method.</summary>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool ReloadDataPrefix(FarmAnimal __instance)
        {
            // load content packs here instead of the OnSaveLoaded/OnLoadStageChanged event. this is because json assets need to be loaded first (for api object for products) (JA is loaded OnLoadStageChanged LoadStage.LoadParsed). 
            // whenever I made a OnLoadStageChanged event it would run before JAs causing api objects to not exist
            // this code is ran before the next load stage event is fired. when this code is first ran JA should be fully initialised though.
            if (!ModEntry.Instance.ContentPacksLoaded)
            {
                ModEntry.Instance.LoadContentPacks();
                ModEntry.Instance.ContentPacksLoaded = true;
            }

            string data;
            Game1.content.Load<Dictionary<string, string>>("Data\\FarmAnimals").TryGetValue(__instance.type.Value, out data);
            if (data == null)
            {
                ModEntry.ModMonitor.Log($"Couldn't find farm animal datastring for animal: {__instance.type.Value}", LogLevel.Error);
                return false;
            }

            string[] dataSplit = data.Split('/');
            __instance.daysToLay.Value = Convert.ToByte(dataSplit[0]);
            __instance.ageWhenMature.Value = Convert.ToByte(dataSplit[1]);
            __instance.defaultProduceIndex.Value = string.IsNullOrEmpty(dataSplit[2]) ? -1 : Convert.ToInt32(dataSplit[2]);
            __instance.deluxeProduceIndex.Value = string.IsNullOrEmpty(dataSplit[3]) ? -1 : Convert.ToInt32(dataSplit[3]);
            __instance.sound.Value = dataSplit[4].Equals("none") ? (string)null : dataSplit[4];
            __instance.showDifferentTextureWhenReadyForHarvest.Value = Convert.ToBoolean(dataSplit[14]);
            __instance.buildingTypeILiveIn.Value = dataSplit[15];

            string animalType = __instance.type;
            if (__instance.age < __instance.ageWhenMature)
                animalType = "Baby" + (__instance.type.Value.Equals("Duck") ? "White Chicken" : __instance.type.Value);
            else if (__instance.showDifferentTextureWhenReadyForHarvest && __instance.currentProduce <= 0)
                animalType = "Sheared" + __instance.type.Value;

            __instance.Sprite = new AnimatedSprite($"Animals\\{animalType}", 0, Convert.ToInt32(dataSplit[16]), Convert.ToInt32(dataSplit[17]));
            __instance.frontBackSourceRect.Value = new Microsoft.Xna.Framework.Rectangle(0, 0, Convert.ToInt32(dataSplit[16]), Convert.ToInt32(dataSplit[17]));
            __instance.sidewaysSourceRect.Value = new Microsoft.Xna.Framework.Rectangle(0, 0, Convert.ToInt32(dataSplit[18]), Convert.ToInt32(dataSplit[19]));
            __instance.fullnessDrain.Value = Convert.ToByte(dataSplit[20]);
            __instance.happinessDrain.Value = Convert.ToByte(dataSplit[21]);
            __instance.meatIndex.Value = Convert.ToInt32(dataSplit[23]);
            __instance.price.Value = Convert.ToInt32(dataSplit[24]);

            // get the animal data to set the custom walk speed
            foreach (var animal in ModEntry.Animals)
            {
                if (animal.SubTypes.Where(subType => subType.Name.ToLower() == __instance.type.Value.ToLower()).Any())
                {
                    __instance.Speed = animal.Data.WalkSpeed;
                }
            }

            if (!__instance.isCoopDweller())
                __instance.Sprite.textureUsesFlippedRightForLeft = true;

            return false;
        }

        /// <summary>The prefix for the DayUpdate method.</summary>
        /// <param name="environtment">The current location of the animal.</param>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool DayUpdatePrefix(GameLocation environtment, FarmAnimal __instance)
        {
            var random = new Random((int)(__instance.myID / 2 + Game1.stats.DaysPlayed));
            __instance.controller = null;
            __instance.health.Value = 3;

            var hasBeenLockedOutside = false;
            // check if the animal is currently outside
            if (__instance.home != null && !(__instance.home.indoors.Value as AnimalHouse).animals.ContainsKey(__instance.myID) && environtment is Farm)
            {
                // check if they were locked outside
                if (!__instance.home.animalDoorOpen)
                {
                    __instance.moodMessage.Value = 6; // locked outside mood message
                    __instance.happiness.Value /= (byte)2;
                    hasBeenLockedOutside = true;
                }
                else // animal is outside but door is open, move the animal inside
                {
                    (environtment as Farm).animals.Remove(__instance.myID);
                    (__instance.home.indoors.Value as AnimalHouse).animals.Add(__instance.myID, __instance);

                    if (Game1.timeOfDay > 1800)
                        __instance.happiness.Value /= 2;

                    environtment = __instance.home.indoors;
                    __instance.setRandomPosition(environtment);

                    return false;
                }
            }

            __instance.daysSinceLastLay.Value++;
            if (!__instance.wasPet) // check if the animal wasn't pet, if it wasn't then reduce happiness and friendship
            {
                __instance.friendshipTowardFarmer.Value = Math.Max(0, __instance.friendshipTowardFarmer - (10 - __instance.friendshipTowardFarmer / 200));
                __instance.happiness.Value = (byte)Math.Max(0, __instance.happiness - __instance.happinessDrain * 5);
            }
            __instance.wasPet.Value = false;

            // check if there is hay to eat
            if ((__instance.fullness < 200 || Game1.timeOfDay < 1700) && environtment is AnimalHouse)
            {
                for (int index = environtment.objects.Count() - 1; index >= 0; --index)
                {
                    var environmentObjects = environtment.objects.Pairs.ElementAt(index); // Key: location - Value: object
                    if (environmentObjects.Value.Name == "Hay")
                    {
                        // remove object and restore fullness
                        var objects = environtment.objects;
                        environmentObjects = environtment.objects.Pairs.ElementAt(index);
                        var location = environmentObjects.Key;
                        objects.Remove(location);
                        __instance.fullness.Value = byte.MaxValue;
                        break;
                    }
                }
            }

            // increase age of animal
            if (__instance.fullness > 200 || random.NextDouble() < (__instance.fullness - 30) / 170.0)
            {
                __instance.age.Value++;
                if (__instance.age == __instance.ageWhenMature)
                {
                    // change sprite sheet to be adult
                    __instance.Sprite.LoadTexture(Path.Combine("Animals", __instance.type.Value));

                    // make sheep harvestable
                    if (__instance.type.Value.Contains("Sheep"))
                        __instance.currentProduce.Value = __instance.defaultProduceIndex;

                    // ensure all animals are redy to produce - this is so the player doesn't need to wait a couple of days for them to produce
                    __instance.daysSinceLastLay.Value = 99;
                }

                // increase animal happiness - they increase happiness when full, even if not pet
                __instance.happiness.Value = (byte)Math.Min(byte.MaxValue, __instance.happiness + __instance.happinessDrain * 2);
            }

            // decrease animal happiness and friendship if they are hungry
            if (__instance.fullness.Value < 200)
            {
                __instance.happiness.Value = (byte)Math.Max(0, __instance.happiness - 100);
                __instance.friendshipTowardFarmer.Value = Math.Max(0, __instance.friendshipTowardFarmer - 20);
            }

            // determine whether the animal can produce an item today
            bool canProduceItem = __instance.daysSinceLastLay >= __instance.daysToLay - (!__instance.type.Value.Equals("Sheep") || !Game1.getFarmer((long)__instance.ownerID).professions.Contains(3) ? 0 : 1) && random.NextDouble() < __instance.fullness / 200.0 && random.NextDouble() < __instance.happiness / 70.0;

            var producedItemId = -1;
            if (canProduceItem && !__instance.isBaby()) // check whether the animal can produce an item and which one
            {
                producedItemId = __instance.defaultProduceIndex.Value;
                if (producedItemId == -1) // animal is a custom animal
                {
                    foreach (var animal in ModEntry.Animals)
                    {
                        var subType = animal.SubTypes.Where(subType => subType.Name.ToLower() == __instance.type.Value.ToLower()).FirstOrDefault();
                        if (subType != null)
                        {
                            producedItemId = subType.Produce.GetRandomDefault(out var harvestType);
                            __instance.harvestType.Value = (byte)harvestType;
                            break;
                        }
                    }
                }

                if (random.NextDouble() < __instance.happiness / 150.0)
                {
                    // create a frienship modifier based off of animal happiness - this is so deluxe item drops are also determine from animal happiness, not just friendship
                    float frienshipModifier = __instance.happiness > 200 ? __instance.happiness * 1.5f : __instance.happiness <= 100 ? __instance.happiness - 100 : 0.0f;

                    // determine deluxe products for Ducks and Rabbits separately as their deluxe products are rare drops, unlike other animals
                    if (__instance.type.Value.Equals("Duck") && random.NextDouble() < (__instance.friendshipTowardFarmer + frienshipModifier) / 5000.0 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() * 0.01)
                        producedItemId = __instance.deluxeProduceIndex;
                    else if (__instance.type.Value.Equals("Rabbit") && random.NextDouble() < (__instance.friendshipTowardFarmer + frienshipModifier) / 5000.0 + Game1.player.team.AverageDailyLuck() + Game1.player.team.AverageLuckLevel() * 0.02)
                        producedItemId = __instance.deluxeProduceIndex;

                    __instance.daysSinceLastLay.Value = 0;

                    // increase game stats for items produced - this only applies to items layable by default animals
                    switch (producedItemId)
                    {
                        case 176:
                            Game1.stats.ChickenEggsLayed++;
                            break;
                        case 180:
                            Game1.stats.ChickenEggsLayed++;
                            break;
                        case 440:
                            Game1.stats.RabbitWoolProduced++;
                            break;
                        case 442:
                            Game1.stats.DuckEggsLayed++;
                            break;
                    }

                    // determine whether deluxe product should be produced, exclude Ducks and Rabbits as they've been handled above
                    if (!__instance.type.Value.Equals("Duck") && !__instance.type.Value.Equals("Rabbit"))
                    {
                        if (random.NextDouble() < (__instance.friendshipTowardFarmer + frienshipModifier) / 1200.0 && __instance.friendshipTowardFarmer >= 200)
                        {
                            if (__instance.deluxeProduceIndex == -1) // animal is a custom animal
                            {
                                foreach (var animal in ModEntry.Animals)
                                {
                                    var subType = animal.SubTypes.Where(subType => subType.Name.ToLower() == __instance.type.Value.ToLower()).FirstOrDefault();
                                    if (subType != null)
                                    {
                                        var deluxeId = subType.Produce.GetRandomDeluxe(out var harvestType);
                                        if (deluxeId != -1) // only change to a deluxe product if one could be found (not all animals have deluxe produce)
                                        {
                                            producedItemId = deluxeId;
                                            __instance.harvestType.Value = (byte)harvestType;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    double productQualityChance = __instance.friendshipTowardFarmer / 1000.0 - (1.0 - __instance.happiness / 225.0);

                    // if the farmer has a related profession, increase the chance of getting a high quality drop
                    if (!__instance.isCoopDweller() && Game1.getFarmer(__instance.ownerID).professions.Contains(3) || __instance.isCoopDweller() && Game1.getFarmer(__instance.ownerID).professions.Contains(2))
                        productQualityChance += 0.33;

                    // determine quality of produced item
                    if (productQualityChance >= 0.95 && random.NextDouble() < productQualityChance / 2.0)
                        __instance.produceQuality.Value = 4;
                    else if (random.NextDouble() < productQualityChance / 2.0)
                        __instance.produceQuality.Value = 2;
                    else if (random.NextDouble() < productQualityChance)
                        __instance.produceQuality.Value = 1;
                    else
                        __instance.produceQuality.Value = 0;
                }
            }

            // setup harvest type with a tool - this is so it's produce doesn't spawn in the animal house, instead must be manually harvested
            if (__instance.harvestType == 1 & canProduceItem)
            {
                __instance.currentProduce.Value = producedItemId;
                producedItemId = -1;
            }

            // ensure animal has an item ready to spawn and a valid home to spawn it in
            if (producedItemId != -1 && __instance.home != null)
            {
                var needsToPlaceProduce = true; // whether the animal needs to place there object - used for determining the the produce has been placed in an autograbber
                var producedItem = new SObject(Vector2.Zero, producedItemId, null, false, true, false, false) { Quality = __instance.produceQuality };

                // check if the animal house has an auto grabber, if so spawn the item in there
                foreach (SObject environmentObject in __instance.home.indoors.Value.objects.Values)
                {
                    if (environmentObject.bigCraftable && environmentObject.parentSheetIndex == 165 && environmentObject.heldObject.Value != null)
                    {
                        if ((environmentObject.heldObject.Value as Chest).addItem(producedItem) == null) // if addItem returns null it mean's all the items could be placed in the autograbber
                        {
                            environmentObject.showNextIndex.Value = true;
                            needsToPlaceProduce = false;
                            break;
                        }
                    }
                }

                // spawn the object if there was no valid auto grabber and there is a valid space under the animal
                if (needsToPlaceProduce && !__instance.home.indoors.Value.Objects.ContainsKey(__instance.getTileLocation()))
                    __instance.home.indoors.Value.Objects.Add(__instance.getTileLocation(), producedItem);
            }

            // calculate the mood message for the animal
            if (!hasBeenLockedOutside) // ensure they haven't been locked outside, this is because a mood message would have been set already
            {
                if (__instance.fullness < 30)
                    __instance.moodMessage.Value = 4;
                else if (__instance.happiness < 30)
                    __instance.moodMessage.Value = 3;
                else if (__instance.happiness < 200)
                    __instance.moodMessage.Value = 2;
                else
                    __instance.moodMessage.Value = 1;
            }

            // make animal hungry
            __instance.fullness.Value = 0;

            // if it's a festival today, don't make animal hungry - festivals go on for most of the day so making them hungry would be unfair to the player
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
                __instance.fullness.Value = 250;

            // reload animal data
            __instance.reload(__instance.home);

            return false;
        }
    }
}
