/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Enums;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
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
            var animal = ModEntry.Instance.Api.GetAnimalByName(type);
            if (animal != null)
            {
                var subType = animal.Data.Types[Game1.random.Next(animal.Data.Types.Count())];
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

            // TODO: merge
            var test = Game1.content.Load<Dictionary<string, string>>(Path.Combine("Data", "FarmAnimals"));
            test.TryGetValue(__instance.type.Value, out string data);
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

            __instance.Sprite = new AnimatedSprite(Path.Combine("Animals", animalType), 0, Convert.ToInt32(dataSplit[16]), Convert.ToInt32(dataSplit[17]));
            __instance.frontBackSourceRect.Value = new Microsoft.Xna.Framework.Rectangle(0, 0, Convert.ToInt32(dataSplit[16]), Convert.ToInt32(dataSplit[17]));
            __instance.sidewaysSourceRect.Value = new Microsoft.Xna.Framework.Rectangle(0, 0, Convert.ToInt32(dataSplit[18]), Convert.ToInt32(dataSplit[19]));
            __instance.fullnessDrain.Value = Convert.ToByte(dataSplit[20]);
            __instance.happinessDrain.Value = Convert.ToByte(dataSplit[21]);
            __instance.meatIndex.Value = Convert.ToInt32(dataSplit[23]);
            __instance.price.Value = Convert.ToInt32(dataSplit[24]);

            // get the animal data to set the custom walk speed
            var animal = ModEntry.Instance.Api.GetAnimalBySubTypeName(__instance.type.Value);
            if (animal != null)
            {
                __instance.Speed = animal.Data.WalkSpeed;
            }

            if (!__instance.isCoopDweller())
                __instance.Sprite.textureUsesFlippedRightForLeft = true;

            return false;
        }

        /// <summary>The prefix for the Behaviors method.</summary>
        /// <param name="time">The GameTime object that contains time data about the game's frame time.</param>
        /// <param name="location">The current location of the <see cref="FarmAnimal"/> being patched.</param>
        /// <param name="__result">The return value of the original Bahaviors method.</param>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        public static bool BehaviorsPrefix(GameTime time, GameLocation location, ref bool __result, FarmAnimal __instance)
        {
            // ensure animal has a house
            if (__instance.home == null)
            {
                __result = false;
                return false;
            }

            // get isEating memeber
            var isEating = (NetBool)typeof(FarmAnimal).GetField("isEating", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            if (isEating)
            {
                if (__instance.home != null && __instance.home.getRectForAnimalDoor().Intersects(__instance.GetBoundingBox()))
                {
                    FarmAnimal.behaviorAfterFindingGrassPatch(__instance, location);
                    isEating.Value = false;
                    __instance.Halt();
                    __result = false;
                    return false;
                }

                // sort out animation
                if (__instance.buildingTypeILiveIn.Contains("Barn"))
                {
                    __instance.Sprite.Animate(time, 16, 4, 100f);
                    if (__instance.Sprite.currentFrame >= 20)
                    {
                        isEating.Value = false;
                        __instance.Sprite.loop = true;
                        __instance.Sprite.currentFrame = 0;
                        __instance.faceDirection(2);
                    }
                }
                else
                {
                    __instance.Sprite.Animate(time, 24, 4, 100f);
                    if (__instance.Sprite.currentFrame >= 28)
                    {
                        isEating.Value = false;
                        __instance.Sprite.loop = true;
                        __instance.Sprite.currentFrame = 0;
                        __instance.faceDirection(2);
                    }
                }

                // set isEating member
                typeof(FarmAnimal).GetField("isEating", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, isEating);

                __result = true;
                return false;
            }

            // only let the main behavior code be ran by the host
            if (Game1.IsClient)
            {
                __result = false;
                return false;
            }

            // ensure animal isn't currently on a path
            if (__instance.controller != null)
            {
                __result = true;
                return false;
            }

            // make animal go to grass if it's hungry
            if (location.IsOutdoors && __instance.fullness < 195 && (Game1.random.NextDouble() < 0.002 && FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick))
            {
                FarmAnimal.NumPathfindingThisTick++;
                __instance.controller = new PathFindController(__instance, location, new PathFindController.isAtEnd(FarmAnimal.grassEndPointFunction), -1, false, new PathFindController.endBehavior(FarmAnimal.behaviorAfterFindingGrassPatch), 200, Point.Zero, true);
            }

            // teleport the animals inside at night
            if (Game1.timeOfDay >= 1700 && location.IsOutdoors && (__instance.controller == null && Game1.random.NextDouble() < 0.002))
            {
                if (location.farmers.Count == 0)
                {
                    (location as Farm).animals.Remove(__instance.myID);
                    (__instance.home.indoors.Value as AnimalHouse).animals.Add(__instance.myID, __instance);
                    __instance.setRandomPosition(__instance.home.indoors);
                    __instance.faceDirection(Game1.random.Next(4));
                    __instance.controller = null;

                    __result = true;
                    return false;
                }

                if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
                {
                    ++FarmAnimal.NumPathfindingThisTick;
                    __instance.controller = new PathFindController(__instance, location, new PathFindController.isAtEnd(PathFindController.isAtEndPoint), 0, false, null, 200, new Point(__instance.home.tileX + __instance.home.animalDoor.X, __instance.home.tileY + __instance.home.animalDoor.Y), true);
                }
            }

            // check if animal can forage for produce
            if (location.IsOutdoors && !Game1.isRaining && __instance.currentProduce != -1 && !__instance.isBaby() && Game1.random.NextDouble() < 0.0002)
            {
                // check if animal has produce that can be foraged
                var subType = ModEntry.Instance.Api.GetAnimalSubTypeByName(__instance.type);
                if (subType != null)
                {
                    // get the forage items count
                    var productCount = subType.Produce?.AllSeasons?.Products.Where(product => product.HarvestType == HarvestType.Forage).Select(product => product.Id).Count() ?? 0;
                    switch (Game1.currentSeason)
                    {
                        case "spring":
                            {
                                productCount += subType.Produce?.Spring?.Products.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).Count() ?? 0;
                                productCount += subType.Produce?.Spring?.DeluxeProducts.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).Count() ?? 0;
                                break;
                            }
                        case "summer":
                            {
                                productCount = subType.Produce?.Spring?.Products.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).Count() ?? 0;
                                productCount = subType.Produce?.Spring?.DeluxeProducts.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).Count() ?? 0;
                                break;
                            }
                        case "fall":
                            {
                                productCount = subType.Produce?.Spring?.Products.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).Count() ?? 0;
                                productCount = subType.Produce?.Spring?.DeluxeProducts.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).Count() ?? 0;
                                break;
                            }
                        case "winter":
                            {
                                productCount = subType.Produce?.Spring?.Products.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).Count() ?? 0;
                                productCount = subType.Produce?.Spring?.DeluxeProducts.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).Count() ?? 0;
                                break;
                            }
                    }

                    // ensure there is a valid product for the animal
                    if (productCount == 0)
                    {
                        __result = false;
                        return false;
                    }
                }

                // amke sure the place is blank for spawning the foraged item
                var boundingBox = __instance.GetBoundingBox();
                for (int corner = 0; corner < 4; ++corner)
                {
                    var cornersOfThisRectangle = Utility.getCornersOfThisRectangle(ref boundingBox, corner);
                    var position = new Vector2(cornersOfThisRectangle.X / 64f, cornersOfThisRectangle.Y / 64f);
                    if (location.terrainFeatures.ContainsKey(position) || location.objects.ContainsKey(position))
                    {
                        __result = false;
                        return false;
                    }
                }

                // play forage sounds
                if (Game1.player.currentLocation.Equals(location))
                {
                    DelayedAction.playSoundAfterDelay("dirtyHit", 450, null, -1);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 900, null, -1);
                    DelayedAction.playSoundAfterDelay("dirtyHit", 1350, null, -1);
                }

                // animate the animal is the player is there
                var findTruffle = typeof(FarmAnimal).GetMethod("findTruffle", BindingFlags.NonPublic | BindingFlags.Instance);
                if (location.Equals(Game1.currentLocation))
                {
                    switch (__instance.FacingDirection)
                    {
                        case 0:
                            __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(9, 250),
                                    new FarmerSprite.AnimationFrame(11, 250),
                                    new FarmerSprite.AnimationFrame(9, 250),
                                    new FarmerSprite.AnimationFrame(11, 250),
                                    new FarmerSprite.AnimationFrame(9, 250),
                                    new FarmerSprite.AnimationFrame(11, 250, false, false, new AnimatedSprite.endOfAnimationBehavior((farmer) => { findTruffle.Invoke(__instance, new object[] { farmer }); }), false)
                                });
                            break;
                        case 1:
                            __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(5, 250),
                                    new FarmerSprite.AnimationFrame(7, 250),
                                    new FarmerSprite.AnimationFrame(5, 250),
                                    new FarmerSprite.AnimationFrame(7, 250),
                                    new FarmerSprite.AnimationFrame(5, 250),
                                    new FarmerSprite.AnimationFrame(7, 250, false, false, new AnimatedSprite.endOfAnimationBehavior((farmer) => { findTruffle.Invoke(__instance, new object[] { farmer }); }), false)
                                });
                            break;
                        case 2:
                            __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(1, 250),
                                    new FarmerSprite.AnimationFrame(3, 250),
                                    new FarmerSprite.AnimationFrame(1, 250),
                                    new FarmerSprite.AnimationFrame(3, 250),
                                    new FarmerSprite.AnimationFrame(1, 250),
                                    new FarmerSprite.AnimationFrame(3, 250, false, false, new AnimatedSprite.endOfAnimationBehavior((farmer) => { findTruffle.Invoke(__instance, new object[] { farmer }); }), false)
                                });
                            break;
                        case 3:
                            __instance.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                                {
                                    new FarmerSprite.AnimationFrame(5, 250, false, true, (AnimatedSprite.endOfAnimationBehavior)null, false),
                                    new FarmerSprite.AnimationFrame(7, 250, false, true, (AnimatedSprite.endOfAnimationBehavior)null, false),
                                    new FarmerSprite.AnimationFrame(5, 250, false, true, (AnimatedSprite.endOfAnimationBehavior)null, false),
                                    new FarmerSprite.AnimationFrame(7, 250, false, true, (AnimatedSprite.endOfAnimationBehavior)null, false),
                                    new FarmerSprite.AnimationFrame(5, 250, false, true, (AnimatedSprite.endOfAnimationBehavior)null, false),
                                    new FarmerSprite.AnimationFrame(7, 250, false, true, new AnimatedSprite.endOfAnimationBehavior((farmer) => { findTruffle.Invoke(__instance, new object[] { farmer }); }), false)
                                });
                            break;
                    }

                    __instance.Sprite.loop = false;
                }
                else
                {
                    findTruffle.Invoke(__instance, new object[] { Game1.player });
                }
            }

            __result = false;
            return false;
        }

        /// <summary>The prefix for the FindTruffle method.</summary>
        /// <remarks>Although the method is called 'FindTruffle' it's responsible for all produce that is foraged.</remarks>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        public static bool FindTrufflePrefix(FarmAnimal __instance)
        {
            // detemine the item to drop
            var productId = -1;
            var subType = ModEntry.Instance.Api.GetAnimalSubTypeByName(__instance.type);
            if (subType != null)
            {
                // get all the foragable items
                var foragableItems = subType.Produce?.AllSeasons?.Products?.Where(product => product.HarvestType == HarvestType.Forage).Select(product => product.Id).ToList();
                switch (Game1.currentSeason)
                {
                    case "spring":
                        {
                            var products = subType.Produce?.Spring?.Products?.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).ToList();
                            if (products != null)
                                foragableItems.AddRange(products);

                            break;
                        }
                    case "summer":
                        {
                            var products = subType.Produce?.Summer?.Products?.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).ToList();
                            if (products != null)
                                foragableItems.AddRange(products);

                            break;
                        }
                    case "fall":
                        {
                            var products = subType.Produce?.Fall?.Products?.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).ToList();
                            if (products != null)
                                foragableItems.AddRange(products);

                            break;
                        }
                    case "winter":
                        {
                            var products = subType.Produce?.Winter?.Products?.Where(produce => produce.HarvestType == HarvestType.Forage).Select(produce => produce.Id).ToList();
                            if (products != null)
                                foragableItems.AddRange(products);

                            break;
                        }
                }

                if (foragableItems.Count == 0)
                    return false;

                var productStringId = foragableItems[Game1.random.Next(foragableItems.Count)];
                if (!int.TryParse(productStringId, out productId))
                    return false;
            }

            // try to spawn the product around the animal
            if (Utility.spawnObjectAround(Utility.getTranslatedVector2(__instance.getTileLocation(), __instance.FacingDirection, 1f), new SObject(__instance.getTileLocation(), productId, 1), Game1.getFarm(), true))
            {
                if (productId == 430)
                    ++Game1.stats.TrufflesFound;
            }

            // if the player is a high friendship, increase the possiblility of skipping resetting the currentProduce - this means high friendshipped animals produce more
            if (Game1.random.NextDouble() <= __instance.friendshipTowardFarmer / 1500.0)
                return false;

            __instance.currentProduce.Value = -1;
            return false;
        }

        /// <summary>The prefix for the MakeSound method.</summary>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns>True meaning the original method will get ran.</returns>
        internal static bool MakeSoundPrefix(FarmAnimal __instance)
        {
            var animalData = ModEntry.Instance.Api.GetAnimalBySubTypeName(__instance.type);
            if (animalData.Data.SoundEffect != null)
            {
                animalData.Data.SoundEffect.Play();
                return false;
            }

            return true;
        }

        /// <summary>The prefix for the UpdateWhenNotCurrentLocation method.</summary>
        /// <param name="currentBuilding">The current building the animal is in.</param>
        /// <param name="time">The GameTime object that contains time data about the game's frame time.</param>
        /// <param name="environment">The <see cref="GameLocation"/> of the animal.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool UpdateWhenNotCurrentLocationPrefix(Building currentBuilding, GameTime time, GameLocation environment, FarmAnimal __instance)
        {
            var behaviors = typeof(FarmAnimal).GetMethod("behaviors", BindingFlags.NonPublic | BindingFlags.Instance);
            var doFarmerPushEvent = (NetEvent1Field<int, NetInt>)typeof(FarmAnimal).GetField("doFarmerPushEvent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var doBuildingPokeEvent = (NetEvent0)typeof(FarmAnimal).GetField("doBuildingPokeEvent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            doFarmerPushEvent.Poll();
            doBuildingPokeEvent.Poll();

            // skip if time is paused
            if (!Game1.shouldTimePass())
                return false;

            __instance.update(time, environment, __instance.myID, false);

            // ensure this is the game host (not the farmhands)
            if (!Game1.IsMasterGame)
                return false;

            // check if the animal is able to go outside
            if (currentBuilding == null || Game1.random.NextDouble() > .002 || !currentBuilding.animalDoorOpen || Game1.timeOfDay >= 1630 || Game1.isRaining || environment.farmers.Count > 0)
            {
                behaviors.Invoke(__instance, new object[] { time, environment });
                return false;
            }

            // check for special season conditions on the animal
            var animal = ModEntry.Instance.Api.GetAnimalBySubTypeName(__instance.type);
            if (animal != null)
            {
                // convert season string value into enum
                Season season = Season.Spring;
                switch (Game1.currentSeason)
                {
                    case "summer":
                        season = Season.Summer;
                        break;
                    case "fall":
                        season = Season.Fall;
                        break;
                    case "winter":
                        season = Season.Winter;
                        break;
                }

                if (!animal.Data.SeasonsAllowedOutdoors.Contains(season))
                {
                    behaviors.Invoke(__instance, new object[] { time, environment });
                    return false;
                }
            }

            // get the farm location to spawn the animal in
            Farm farm = (Farm)Game1.getLocationFromName("Farm");

            // ensure the animal won't be colliding with anything when they leave the house
            var exitCollisionBox = new Rectangle(
                x: (currentBuilding.tileX + currentBuilding.animalDoor.X) * 64 + 2,
                y: (currentBuilding.tileY + currentBuilding.animalDoor.Y) * 64 + 2,
                width: __instance.isCoopDweller() ? 60 : 124,
                height: 60
            );

            if (farm.isCollidingPosition(exitCollisionBox, Game1.viewport, false, 0, false, __instance, false, false, false))
                return false;

            // remove animal from farm
            if (farm.animals.ContainsKey(__instance.myID))
            {
                for (int index = farm.animals.Count() - 1; index >= 0; --index)
                {
                    if (farm.animals.Pairs.ElementAt(index).Key.Equals(__instance.myID))
                    {
                        farm.animals.Remove(__instance.myID);
                        break;
                    }
                }
            }

            // remove animal from building
            (currentBuilding.indoors.Value as AnimalHouse).animals.Remove(__instance.myID);

            // add animal to farm, initial values
            farm.animals.Add(__instance.myID, __instance);
            __instance.faceDirection(2);
            __instance.SetMovingDown(true);
            __instance.Position = new Vector2((float)currentBuilding.getRectForAnimalDoor().X, (float)((currentBuilding.tileY + currentBuilding.animalDoor.Y) * 64 - (__instance.Sprite.getHeight() * 4 - __instance.GetBoundingBox().Height) + 32));

            // sort out path finding
            if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
            {
                ++FarmAnimal.NumPathfindingThisTick;
                __instance.controller = new PathFindController(
                    c: __instance,
                    location: farm,
                    endFunction: new PathFindController.isAtEnd(FarmAnimal.grassEndPointFunction),
                    finalFacingDirection: Game1.random.Next(4),
                    eraseOldPathController: false,
                    endBehaviorFunction: new PathFindController.endBehavior(FarmAnimal.behaviorAfterFindingGrassPatch),
                    limit: 200,
                    endPoint: Point.Zero
                );
            }

            if (__instance.controller == null || __instance.controller.pathToEndPoint == null || __instance.controller.pathToEndPoint.Count < 3)
            {
                __instance.SetMovingDown(true);
                __instance.controller = null;
            }
            else
            {
                __instance.faceDirection(2);
                __instance.Position = new Vector2(
                    x: __instance.controller.pathToEndPoint.Peek().X * 64,
                    y: (__instance.controller.pathToEndPoint.Peek().Y * 64 - (__instance.Sprite.getHeight() * 4 - __instance.GetBoundingBox().Height) + 16)
                );

                if (!__instance.isCoopDweller())
                    __instance.position.X -= 32f;
            }

            __instance.noWarpTimer = 3000;
            currentBuilding.currentOccupants.Value--;
            if (Utility.isOnScreen(__instance.getTileLocationPoint(), 192, farm))
                farm.localSound("sandyStep");

            if (environment.isTileOccupiedByFarmer(__instance.getTileLocation()) != null)
                environment.isTileOccupiedByFarmer(__instance.getTileLocation()).TemporaryPassableTiles.Add(__instance.GetBoundingBox());

            behaviors.Invoke(__instance, new object[] { time, environment });
            return false;
        }

        /// <summary>The prefix for the DayUpdate method.</summary>
        /// <param name="environtment">The current location of the animal.</param>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool DayUpdatePrefix(GameLocation environtment, FarmAnimal __instance) // TODO: check if I can correct the mispelling from the game code with Harmony still working
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
                var subType = ModEntry.Instance.Api.GetAnimalSubTypeByName(__instance.type.Value);
                if (subType != null)
                {
                    var numberOfHearts = (int)(__instance.friendshipTowardFarmer / 195f);
                    producedItemId = subType.Produce.GetRandomDefault(numberOfHearts, out var harvestType);
                    __instance.harvestType.Value = (byte)harvestType;
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
                            var numberOfHearts = (int)(__instance.friendshipTowardFarmer / 195f);
                            var customSubType = ModEntry.Instance.Api.GetAnimalSubTypeByName(__instance.type.Value);
                            var deluxeId = customSubType.Produce.GetRandomDeluxe(numberOfHearts, out var harvestType);
                            if (deluxeId != -1) // only change to a deluxe product if one could be found (not all animals have deluxe produce)
                            {
                                producedItemId = deluxeId;
                                __instance.harvestType.Value = (byte)harvestType;
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
            if (__instance.harvestType == 1 && canProduceItem)
            {
                __instance.currentProduce.Value = producedItemId;
                producedItemId = -1;
            }

            // ensure animal has an item ready to spawn and a valid home to spawn it in
            if (producedItemId != -1 && __instance.home != null)
            {
                var needsToPlaceProduce = true; // whether the animal needs to place there object - used for determining the the produce has been placed in an autograbber

                // check if the animal house has an auto grabber, if so spawn the item in there
                foreach (SObject environmentObject in __instance.home.indoors.Value.objects.Values)
                {
                    if (environmentObject.bigCraftable && environmentObject.parentSheetIndex == 165 && environmentObject.heldObject.Value != null)
                    {
                        var producedItem = new SObject(Vector2.Zero, producedItemId, null, false, true, false, false) { Quality = __instance.produceQuality };
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
                {
                    var producedItem = new SObject(Vector2.Zero, producedItemId, null, false, true, false, true) { Quality = __instance.produceQuality };
                    __instance.home.indoors.Value.Objects.Add(__instance.getTileLocation(), producedItem);
                }
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
