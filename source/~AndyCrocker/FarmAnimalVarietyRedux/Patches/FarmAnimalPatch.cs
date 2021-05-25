/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Menus;
using FarmAnimalVarietyRedux.Models;
using FarmAnimalVarietyRedux.Models.Converted;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="FarmAnimal"/> class.</summary>
    internal class FarmAnimalPatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the <see cref="FarmAnimal()"/> constructor.</summary>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        /// <remarks>This is used to store references to each animal that is created from the serialiser to easily convert them to custom animals when needed.</remarks>
        internal static void ConstructorPrefix(FarmAnimal __instance) => ModEntry.Instance.ParsedAnimals.Add(__instance);

        /// <summary>The transpiler for the <see cref="FarmAnimal(string, long, long)"/> constructor.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to remove the base game subtype handling for default animals as well as removing the <see cref="FarmAnimal.reloadData()"/> call, this is so the subtype can be randomly picked if the animal being created is custom in the <see cref="ConstructorPostFix(string, FarmAnimal)"/>.</remarks>
        internal static IEnumerable<CodeInstruction> ConstructorTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            for (int i = 0; i < instructions.Count(); i++)
            {
                var instruction = instructions.ElementAt(i);

                // don't try to search for the instruction group to skip
                if (i == instructions.Count() - 1)
                {
                    yield return instruction;
                    continue;
                }

                // check if the instruction is loading a string ready to handle base game subtypes
                if ((instruction.opcode == OpCodes.Ldstr && instruction.operand.ToString() == "Chicken")
                    || (instruction.opcode == OpCodes.Ldstr && instruction.operand.ToString() == "Cow"))
                    instruction.operand = "asdfijabndvp;aj"; // change the string to compare to a random string that won't ever be true, meaning we can handle the subtypes ourselves

                // check if the next two instructions are the group that need skipping to disable the reloadData call
                var nextInstruction = instructions.ElementAt(i + 1);
                if (instruction.opcode == OpCodes.Ldarg_0
                    && nextInstruction.opcode == OpCodes.Callvirt && nextInstruction.operand == typeof(FarmAnimal).GetMethod("reloadData", BindingFlags.Public | BindingFlags.Instance))
                {
                    // skip loading both these instructions
                    i += 2;
                    continue;
                }

                yield return instruction;
            }
        }

        /// <summary>The post fix for the <see cref="FarmAnimal(string, long, long)"/> constructor.</summary>
        /// <param name="type">The internal animal type being constructed.</param>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        /// <remarks>This is used to pick a random subtype of a custom animal and to add the saved custom animal data to the animals.</remarks>
        internal static void ConstructorPostFix(string type, FarmAnimal __instance)
        {
            // ensure an internal name is passed, this only occurs when the game tries to load in 2 cows from where FAVR doesn't patch it (I think they are the cows in Marnie's ranch)
            if (!type.Contains('.'))
                type = $"game.{type}";

            ModEntry.Instance.LoadContentPacks();

            // ensure an animal exists with either this type, or has a sub type that has the type
            var isTypeASubtype = false;
            var animal = ModEntry.Instance.Api.GetAnimalByInternalName(type);
            if (animal == null)
            {
                isTypeASubtype = true;
                animal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(type); // this is just used to ensure an animal exists with the passed type as a subtype
                if (animal == null)
                {
                    // this should be an exceptionally rare circumstance if it occures at all and it'll most likely only occur when players delete / add packs mid play through
                    ModEntry.Instance.Monitor.Log($"Cannot find animal or animal type with an internal name of: {type}", LogLevel.Error);
                    ModEntry.Instance.Monitor.Log("This most likely means you've either uninstalled an animal pack, or you've added an animal pack that removes a previous animal, half way through a play through", LogLevel.Info);
                    return;

                    // this is far from ideal as no data will get loaded for the animal, but the player will hopefully notice something is wrong and not save
                    // it's not really avoidable though as we can't fall back on base game animals as they can be removed in packs, and falling back on the first 
                    // available animals doesn't help if there are literally no loaded animals, not to mention that could be exploited to get powerful animals easily

                    // as stated before, this is a very rare case when the animal some how got through the checks in the incubator / purchase animals menu code and 
                    // most likely means the user has butchered their mod list so it's kinda on them anyway
                }
            }

            // set the animal type
            if (isTypeASubtype)
                __instance.type.Value = type;
            else
            {
                // TODO: this is not a good way to choose whether only buyable subtypes should be selected
                var buyingAnimal = Game1.activeClickableMenu is CustomPurchaseAnimalsMenu;

                var subtypes = new List<CustomAnimalType>();
                if (buyingAnimal)
                    subtypes = animal.Subtypes.Where(subtype => subtype.IsBuyable).ToList();
                if (subtypes.Count == 0) // just use all subtypes if there are no buyable subtypes
                    subtypes = animal.Subtypes;

                var subtype = subtypes.ElementAt(Game1.random.Next(subtypes.Count()));
                __instance.type.Value = subtype.InternalName;
            }

            __instance.reloadData();

            // add animal to save persistant data
            var animalSubtypeData = ModEntry.Instance.Api.GetAnimalSubtypeByInternalName(__instance.type.Value);
            if (animalSubtypeData != null && animalSubtypeData.Produce != null)
            {
                var savedProduceData = new List<SavedProduceData>();
                foreach (var produce in animalSubtypeData.Produce)
                    savedProduceData.Add(new SavedProduceData(produce.UniqueName, 0));

                __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/produces"] = JsonConvert.SerializeObject(savedProduceData);
            }
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.reloadData()"/> method.</summary>
        /// <param name="__instance">The <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This is used to load the data of custom animals from FAVR data, instead of the game's original method of loading from the data file.</remarks>
        internal static bool ReloadDataPrefix(FarmAnimal __instance)
        {
            // LoadContentPacks needs to be loaded here, as well as OnSaveLoaded (more on that in a bit), this is because if the save file has any animals in it, this code will get ran.
            // this code requires the full custom animals to be loaded to get data for, they can't be loaded OnGameLaunched or OnLoadStageChanged because Json Assets needs to be available to resolve
            // the ids of custom items. as such the content packs will be loaded as late as possible (here) and we will force Json Asset's initialisation code to run earlier that it should (in LoadContentPacks)
            // this *shouldn't* have any unintended side affects (and I have yet to experience any).
            // LoadContentPacks will also get called OnSaveLoaded because if there are no animals on the farm then this code won't get ran, and as such won't load the content packs, the animal data is required
            // even when no animals are present so the custom animals are loaded when their data is required in the purchase menu
            ModEntry.Instance.LoadContentPacks();

            // TODO: try to find a better solution to converting animals from default to custom when the save is being loaded
            // ensure the animal is custom (this should only be the case when the save has first been initialised and the conversion from default animals to custom animals hasn't occured yet)
            if (!__instance.type.Value.Contains('.'))
                Utilities.ConvertDefaultAnimalToCustomAnimal(ref __instance);

            var animal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(__instance.type);
            if (animal == null)
            {
                ModEntry.Instance.Monitor.Log($"Failed to get animal with a subtype of name: {__instance.type} when loading animal data", LogLevel.Error);
                return false;
            }

            var animalSubtype = ModEntry.Instance.Api.GetAnimalSubtypeByInternalName(__instance.type);
            if (animalSubtype == null)
            {
                ModEntry.Instance.Monitor.Log($"Failed to get animal subtype of name: {__instance.type} when loading animal data", LogLevel.Error);
                return false;
            }

            __instance.ageWhenMature.Value = animalSubtype.DaysTillMature;

            var animalType = __instance.type.Value;
            var splitType = animalType.Split('.');
            var uniqueModId = string.Join(".", splitType.Take(splitType.Length - 1));
            var animalName = splitType.Last();
            if (__instance.age < __instance.ageWhenMature)
                animalType = $"{uniqueModId}.Baby{((animalName.ToLower() == "duck") ? "White Chicken" : animalName)}";

            // set properties that will be used by FAVR
            __instance.GetType().GetField("_displayType", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, animalName);
            __instance.sound.Value = animalSubtype.SoundId;
            __instance.showDifferentTextureWhenReadyForHarvest.Value = ModEntry.Instance.AssetManager.HasDifferentSpriteSheetWhenHarvested(animal.InternalName, animalSubtype.InternalName);
            __instance.Sprite = new AnimatedSprite(Path.Combine("Animals", animalType), 0, animalSubtype.FrontAndBackSpriteWidth, animalSubtype.FrontAndBackSpriteHeight);
            __instance.frontBackSourceRect.Value = new Rectangle(0, 0, animalSubtype.FrontAndBackSpriteWidth, animalSubtype.FrontAndBackSpriteHeight);
            __instance.sidewaysSourceRect.Value = new Rectangle(0, 0, animalSubtype.SideSpriteWidth, animalSubtype.SideSpriteHeight);
            __instance.happinessDrain.Value = animalSubtype.HappinessDrain;
            __instance.Speed = animalSubtype.WalkSpeed;
            __instance.meatIndex.Value = animalSubtype.MeatId;
            __instance.price.Value = animal.AnimalShopInfo?.BuyPrice * 2 ?? 0; // set the default sell price (when BabySellPrice and AdultSellPrice aren't present) to be 2x the purchase price (same as base game), this is because the friendship gets factored into determining the sell price

            __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/fullnessGain"] = animalSubtype.FullnessGain.ToString();
            __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/happinessGain"] = animalSubtype.HappinessGain.ToString();
            __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/autoPetterFriendshipGain"] = animalSubtype.AutoPetterFriendshipGain.ToString();
            __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/handPetFriendshipGain"] = animalSubtype.HandPetFriendshipGain.ToString();
            __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/babySellPrice"] = animalSubtype.BabySellPrice.ToString();
            __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/adultSellPrice"] = animalSubtype.AdultSellPrice.ToString();
            __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/isMale"] = animalSubtype.IsMale.ToString();
            __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/seasonsAllowedOutdoors"] = JsonConvert.SerializeObject(animalSubtype.SeasonsAllowedOutdoors ?? new List<string>());
            __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/buildings"] = JsonConvert.SerializeObject(animal.Buildings ?? new List<string>());
            __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/allowForageRepeats"] = animalSubtype.AllowForageRepeats.ToString();

            // fix stored produce (if an animal was converted from BFAV to FAVR or if a content pack changes half way through a save
            {
                var parsedProduces = new List<SavedProduceData>();
                if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/produces", out var productsString))
                    parsedProduces = JsonConvert.DeserializeObject<List<SavedProduceData>>(productsString);

                // add any produce that are missing
                foreach (var animalSubtypeProduce in animalSubtype.Produce)
                    if (!parsedProduces.Any(parsedProduce => parsedProduce.UniqueName.ToLower() == animalSubtypeProduce.UniqueName.ToLower()))
                        parsedProduces.Add(new SavedProduceData(animalSubtypeProduce.UniqueName, 0));

                // remove any produce that should no longer be there
                for (int i = 0; i < parsedProduces.Count; i++)
                {
                    var parsedProduce = parsedProduces[i];
                    if (!animalSubtype.Produce.Any(produce => produce.UniqueName.ToLower() == parsedProduce.UniqueName.ToLower()))
                        parsedProduces.RemoveAt(i--);
                }

                __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/produces"] = JsonConvert.SerializeObject(parsedProduces);
            }

            // set properties that won't be used by FAVR but are required incase FAVR is uninstalled half way through a save
            __instance.daysToLay.Value = 1;
            __instance.defaultProduceIndex.Value = 176;
            __instance.deluxeProduceIndex.Value = 174;
            __instance.harvestType.Value = 0;
            __instance.buildingTypeILiveIn.Value = "Coop";
            __instance.toolUsedForHarvest.Value = null;

            if (!__instance.isCoopDweller())
                __instance.Sprite.textureUsesFlippedRightForLeft = true;

            return false;
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.pet(Farmer, bool)"/> method.</summary>
        /// <param name="who">The farmer who is petting the animal.</param>
        /// <param name="is_auto_pet">Whether the animal is being petted by an auto-petter.</param>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to add the custom hapiness gains and bed time.</remarks>
        internal static bool PetPrefix(Farmer who, bool is_auto_pet, FarmAnimal __instance)
        {
            // ensure the animal doesn't get petted by multiple autp-petters
            if (is_auto_pet && __instance.wasAutoPet)
                return false;

            // animate farmer and animal when hand petting
            if (!is_auto_pet)
            {
                // ensure the animation shouldn't be paused
                if (who.FarmerSprite.PauseForSingleAnimation)
                    return false;

                who.Halt();
                who.faceGeneralDirection(__instance.Position, 0, false, false);

                // check if the animal is asleep
                if (Game1.timeOfDay >= ModEntry.Instance.Config.BedTime && !__instance.isMoving())
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\FarmAnimals:TryingToSleep", __instance.displayName));
                    return false;
                }

                // animate animal
                __instance.Halt();
                __instance.Sprite.StopAnimation();
                __instance.uniqueFrameAccumulator = -1;
                switch (Game1.player.FacingDirection)
                {
                    case 0:
                        __instance.Sprite.CurrentFrame = 0;
                        break;
                    case 1:
                        __instance.Sprite.CurrentFrame = 12;
                        break;
                    case 2:
                        __instance.Sprite.CurrentFrame = 8;
                        break;
                    case 3:
                        __instance.Sprite.CurrentFrame = 4;
                        break;
                }
            }

            // check if the animal query menu should be opened
            if (__instance.wasPet && who.ActiveObject?.ParentSheetIndex != 178)
            {
                Game1.activeClickableMenu = new AnimalQueryMenu(__instance);
                return false;
            }

            // handle petting logic
            if (!__instance.wasPet)
            {
                // retrieve animal properties
                var happinessGain = 15;
                if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/happinessGain", out var happinessGainString))
                    if (int.TryParse(happinessGainString, out var happinessGainParsed))
                        happinessGain = happinessGainParsed;

                var autoPetterFriendshipGain = 7;
                if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/autoPetterFriendshipGain", out var autoPetterFriendshipGainString))
                    if (int.TryParse(autoPetterFriendshipGainString, out var autoPetterFriendshipGainParsed))
                        autoPetterFriendshipGain = autoPetterFriendshipGainParsed;

                var handPetFriendshipGain = 15;
                if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/handPetFriendshipGain", out var handPetFriendshipGainString))
                    if (int.TryParse(handPetFriendshipGainString, out var behandPetFriendshipGaindTimeParsed))
                        handPetFriendshipGain = behandPetFriendshipGaindTimeParsed;

                // increase animal hapiness
                __instance.happiness.Value = (byte)Math.Min(255, __instance.happiness + happinessGain);

                // increase animal friendship
                if (__instance.wasAutoPet)
                    __instance.friendshipTowardFarmer.Value = Math.Min(1000, __instance.friendshipTowardFarmer + autoPetterFriendshipGain); // animal has been auto pet, and is being hand petted
                else if (is_auto_pet)
                    __instance.friendshipTowardFarmer.Value = Math.Min(1000, __instance.friendshipTowardFarmer + Math.Min(0, handPetFriendshipGain - autoPetterFriendshipGain)); // animal is being auto petted
                else
                    __instance.friendshipTowardFarmer.Value = Math.Min(1000, __instance.friendshipTowardFarmer + handPetFriendshipGain); // animal is being hand petted

                // update pet state
                if (is_auto_pet)
                    __instance.wasAutoPet.Value = true;
                else
                    __instance.wasPet.Value = true;

                // handle misc hand petting perks
                if (!is_auto_pet)
                {
                    __instance.makeSound();
                    who.gainExperience(Farmer.farmingSkill, 5);

                    // professions
                    if ((who.professions.Contains(Farmer.butcher) && __instance.isCoopDweller())
                        || (who.professions.Contains(Farmer.shepherd) && !__instance.isCoopDweller()))
                    {
                        __instance.friendshipTowardFarmer.Value = Math.Min(1000, __instance.friendshipTowardFarmer + handPetFriendshipGain);
                        __instance.happiness.Value = (byte)Math.Min(255, __instance.happiness + happinessGain);
                    }

                    // emote
                    var emoteIndex = 20;
                    if (__instance.wasAutoPet)
                        emoteIndex = 32;
                    __instance.doEmote((__instance.moodMessage == 4) ? 12 : emoteIndex);
                }
            }

            return false;
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.behaviors(GameTime, GameLocation)"/> method.</summary>
        /// <param name="time">The <see cref="GameTime"/> object that contains time data about the game's frame time.</param>
        /// <param name="location">The current location of the <see cref="FarmAnimal"/> being patched.</param>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to add the custom forage functionality.</remarks>
        internal static bool BehaviorsPrefix(GameTime time, GameLocation location, FarmAnimal __instance, ref bool __result)
        {
            // retrieve animal properties
            var buildings = new List<string>();
            if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/buildings", out var buildingsString))
                buildings = JsonConvert.DeserializeObject<List<string>>(buildingsString);

            // ensure animal has a house
            if (__instance.home == null)
            {
                __result = false;
                return false;
            }

            // follow behavior
            if (Game1.IsMasterGame && __instance.isBaby() && __instance.CanFollowAdult())
            {
                // the amount of time before the follow behavior should be updated
                var nextFollowTargetScan = (float)__instance.GetType().GetField("_nextFollowTargetScan", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                nextFollowTargetScan -= (float)time.ElapsedGameTime.TotalSeconds;
                if (nextFollowTargetScan < 0f)
                {
                    // reset follow timer
                    nextFollowTargetScan = Utility.RandomFloat(1f, 3f);
                    __instance.GetType().GetField("_nextFollowTargetScan", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, nextFollowTargetScan);

                    // if theres already path finding going on or if the location isn't the farm, reset the follow target
                    if (__instance.controller != null || !(location is Farm farm))
                    {
                        __instance.GetType().GetField("_followTarget", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, null);
                        __instance.GetType().GetField("_followTargetPosition", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, null);
                    }
                    else
                    {
                        // update follow target position
                        var followTarget = (FarmAnimal)__instance.GetType().GetField("_followTarget", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                        if (followTarget != null)
                        {
                            var followTargetPosition = (Point?)__instance.GetType().GetField("_followTargetPosition", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                            if (!FarmAnimal.GetFollowRange(followTarget).Contains(followTargetPosition.Value))
                                __instance.GetNewFollowPosition();

                            __result = false;
                            return false;
                        }

                        // update follow target
                        foreach (FarmAnimal animal in farm.animals.Values)
                            if (!animal.isBaby() && animal.type.Value == __instance.type.Value && FarmAnimal.GetFollowRange(animal, 4).Contains(Utility.Vector2ToPoint(__instance.getStandingPosition())))
                            {
                                __instance.GetType().GetField("_followTarget", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, animal);
                                __instance.GetNewFollowPosition();

                                __result = false;
                                return false;
                            }
                    }
                }
            }

            // handle eating behavior
            if (__instance.isEating)
            {
                // stop the animal from eating if they're in their house (this was causing the animal to infinitely loop their eating animation)
                if (__instance.currentLocation == __instance.home.indoors.Value)
                {
                    __instance.isEating.Value = false;
                    __instance.Halt();
                    __result = false;
                    return false;
                }

                // don't try to eat if the animal is 'inside' the building (when they're spawned on the farm when let out in the day)
                if (__instance.home != null && __instance.home.getRectForAnimalDoor().Intersects(__instance.GetBoundingBox()))
                {
                    FarmAnimal.behaviorAfterFindingGrassPatch(__instance, location);
                    __instance.isEating.Value = false;
                    __instance.Halt();
                    __result = false;
                    return false;
                }

                // sort out eating animation
                var startingFrame = 24;
                if (buildings.Any(building => building.ToLower().Contains("barn")))
                    startingFrame = 16;

                __instance.Sprite.Animate(time, startingFrame, 4, 100);
                if (__instance.Sprite.CurrentFrame >= startingFrame + 4)
                {
                    __instance.isEating.Value = false;
                    __instance.Sprite.loop = true;
                    __instance.Sprite.CurrentFrame = 0;
                    __instance.faceDirection(2);
                }

                __result = true;
                return false;
            }

            // ensure only the host can run the main behavior code
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
            if (!__instance.isSwimming && location.IsOutdoors && __instance.fullness < 195 && Game1.random.NextDouble() < .002f && FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
            {
                FarmAnimal.NumPathfindingThisTick++;
                __instance.controller = new PathFindController(__instance, location, FarmAnimal.grassEndPointFunction, -1, false, FarmAnimal.behaviorAfterFindingGrassPatch, 200, Point.Zero);
                typeof(FarmAnimal).GetField("_followTarget", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, null);
                typeof(FarmAnimal).GetField("_followTargetPosition", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, null);
            }

            // get the animal inside at bed time
            if (Game1.timeOfDay >= (ModEntry.Instance.Config.BedTime - 200) && location.IsOutdoors && __instance.controller == null && Game1.random.NextDouble() < .002f)
            {
                if (!location.farmers.Any()) // if there are no farmers, just teleport the animal
                {
                    (location as Farm).Animals.Remove(__instance.myID);
                    (__instance.home.indoors.Value as AnimalHouse).Animals.Add(__instance.myID, __instance);
                    __instance.setRandomPosition(__instance.home.indoors);
                    __instance.faceDirection(Game1.random.Next(4));
                    __instance.controller = null;

                    __result = true;
                    return false;
                }

                if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick) // otherwise pathfind them home
                {
                    FarmAnimal.NumPathfindingThisTick++;
                    __instance.controller = new PathFindController(__instance, location, PathFindController.isAtEndPoint, 0, false, null, 200, new Point(__instance.home.tileX + __instance.home.animalDoor.X, __instance.home.tileY + __instance.home.animalDoor.Y));
                    typeof(FarmAnimal).GetField("_followTarget", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, null);
                    typeof(FarmAnimal).GetField("_followTargetPosition", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, null);
                }
            }

            // handle forage behavior
            var animalSubtype = ModEntry.Instance.Api.GetAnimalSubtypeByInternalName(__instance.type);
            if (animalSubtype != null && location.IsOutdoors && !Game1.isRaining && !__instance.isBaby() && Game1.random.NextDouble() < .0002f) // the random is so the animals don't produce all foragables instantly
            {
                var animalProduces = animalSubtype.Produce.Where(product => product.HarvestType == HarvestType.Forage);

                // get all modData products
                var parsedProduces = new List<SavedProduceData>();
                if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/produces", out var productsString))
                    parsedProduces = JsonConvert.DeserializeObject<List<SavedProduceData>>(productsString);

                // get the modData products that are valid to be dropped
                var validProduces = Utilities.GetValidAnimalProduce(animalProduces, __instance);
                var produces = parsedProduces.Where(product => product.DaysLeft <= 0 && validProduces.Any(animalProduct => animalProduct.UniqueName.ToLower() == product.UniqueName.ToLower()));

                // ensure there was a produce that could be dropped
                if (produces.Count() == 0)
                {
                    __result = false;
                    return false;
                }

                // choose the product to drop
                var parsedProduceToDrop = produces.ElementAt(Game1.random.Next(produces.Count()));
                var produceToDrop = validProduces.First(produce => produce.UniqueName.ToLower() == parsedProduceToDrop.UniqueName.ToLower());
                var forageId = -1;
                var shouldDropUpgraded = Utilities.ShouldDropUpgradedProduct(produceToDrop, __instance);
                if (shouldDropUpgraded != null)
                {
                    if (shouldDropUpgraded.Value)
                        forageId = produceToDrop.UpgradedProductId;
                    else
                        forageId = produceToDrop.DefaultProductId;
                }

                // ensure there is atleast one foragable item that is pending to be found
                if (forageId == -1)
                {
                    __result = false;
                    return false;
                }

                // make sure the place is blank for spawning the foraged item
                var boundingBox = __instance.GetBoundingBox();
                for (int corner = 0; corner < 4; ++corner)
                {
                    var cornersOfThisRectangle = Utility.getCornersOfThisRectangle(ref boundingBox, corner);
                    var tilePosition = new Vector2(cornersOfThisRectangle.X / 64f, cornersOfThisRectangle.Y / 64f);
                    if (location.terrainFeatures.ContainsKey(tilePosition) || location.objects.ContainsKey(tilePosition))
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

                // spawn the forage, animate the animal if the player is in the location
                var amount = Utilities.DetermineDropAmount(produceToDrop);
                var quality = Utilities.DetermineProductQuality(__instance, produceToDrop);

                var shouldSpawnAgain = true;
                if (location == Game1.currentLocation)
                    AnimateForage(__instance, (Farmer farmer) => { shouldSpawnAgain = SpawnForagedItem(forageId, amount, quality, produceToDrop.StandardQualityOnly, __instance); });
                else
                    shouldSpawnAgain = SpawnForagedItem(forageId, amount, quality, produceToDrop.StandardQualityOnly, __instance);

                // update parsed products to reset object
                if (!shouldSpawnAgain)
                    parsedProduces.First(produce => produce.UniqueName.ToLower() == produceToDrop.UniqueName.ToLower()).DaysLeft = Utilities.DetermineDaysToProduce(produceToDrop);
                __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/produces"] = JsonConvert.SerializeObject(parsedProduces);
            }

            __result = false;
            return false;
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.CanSwim()"/> method.</summary>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <param name="__result">The return value of the method being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to add the swimming behavior.</remarks>
        internal static bool CanSwimPrefix(FarmAnimal __instance, ref bool __result)
        {
            // get custom animal
            var animal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(__instance.type);
            if (animal == null)
            {
                __result = false;
                return false;
            }

            // check if custom animal can swim
            __result = animal.CanSwim;
            return false;
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.UpdateRandomMovements()"/> method.</summary>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns><see langword="true"/> if the original method should get ran; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This reimplements the original method to add the custom bedtime and custom buildings support.</remarks>
        internal static bool UpdateRandomMovementsPrefix(FarmAnimal __instance)
        {
            // make sure animal doesn't move when sleeping
            if (Game1.timeOfDay >= ModEntry.Instance.Config.BedTime + 100 || __instance.pauseTimer > 0)
                return false;

            // let animal eat when swimming
            if (__instance.fullness.Value < 255 && __instance.IsActuallySwimming() && Game1.random.NextDouble() < .002f && !__instance.isEating.Value)
                __instance.Eat(__instance.currentLocation);

            if (!Game1.IsClient && Game1.random.NextDouble() < .007f && __instance.uniqueFrameAccumulator == -1)
            {
                var newDirection = Game1.random.Next(5);
                if (newDirection != (__instance.FacingDirection + 2) % 4 || __instance.IsActuallySwimming())
                {
                    if (newDirection < 4)
                    {
                        var oldDirection = __instance.FacingDirection;
                        __instance.faceDirection(newDirection);
                        if (!__instance.currentLocation.isOutdoors && __instance.currentLocation.isCollidingPosition(__instance.nextPosition(newDirection), Game1.viewport, __instance))
                        {
                            __instance.faceDirection(oldDirection);
                            return false;
                        }
                    }

                    switch (newDirection)
                    {
                        case 0: __instance.SetMovingUp(true); break;
                        case 1: __instance.SetMovingRight(true); break;
                        case 2: __instance.SetMovingDown(true); break;
                        case 3: __instance.SetMovingLeft(true); break;
                        default:
                            __instance.Halt();
                            __instance.Sprite.StopAnimation();
                            break;
                    }
                }
                else if (__instance.noWarpTimer <= 0)
                {
                    __instance.Halt();
                    __instance.Sprite.StopAnimation();
                }
            }

            if (Game1.IsClient || !__instance.isMoving() || Game1.random.NextDouble() >= .014f || __instance.uniqueFrameAccumulator != -1)
                return false;

            __instance.Halt();
            __instance.Sprite.StopAnimation();

            // randomly animate the animal based on facing direction
            if (Game1.random.NextDouble() < .75f)
            {
                __instance.uniqueFrameAccumulator = 0;

                var isCoopAnimal = true;
                if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/buildings", out var buildingsString))
                    if (!JsonConvert.DeserializeObject<List<string>>(buildingsString).Any(building => building.ToLower().Contains("coop")))
                        isCoopAnimal = false;

                if (isCoopAnimal)
                {
                    __instance.Sprite.CurrentFrame = (__instance.FacingDirection) switch
                    {
                        0 => __instance.Sprite.currentFrame = 20,
                        1 => __instance.Sprite.currentFrame = 18,
                        2 => __instance.Sprite.currentFrame = 16,
                        3 => __instance.Sprite.currentFrame = 22
                    };
                }
                else
                {
                    __instance.Sprite.CurrentFrame = (__instance.FacingDirection) switch
                    {
                        0 => __instance.Sprite.currentFrame = 15,
                        1 => __instance.Sprite.currentFrame = 14,
                        2 => __instance.Sprite.currentFrame = 13,
                        3 => __instance.Sprite.currentFrame = 14
                    };
                }
            }

            __instance.Sprite.UpdateSourceRect();
            return false;
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.makeSound()"/> method.</summary>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to add the custom sound functionality.</remarks>
        internal static bool MakeSoundPrefix(FarmAnimal __instance)
        {
            // ensure sounds should be played
            if (__instance.currentLocation != Game1.currentLocation || Game1.options.muteAnimalSounds)
                return false;

            var animal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(__instance.type);

            if (animal?.CustomSound != null)
                animal.CustomSound.Play();
            else if (!string.IsNullOrEmpty(__instance.sound) && __instance.sound.Value.ToLower() != "none")
            {
                var cue = Game1.soundBank.GetCue(__instance.sound);
                cue.SetVariable("Pitch", 1200 + Game1.random.Next(-200, 201));
                cue.Play();
            }

            return false;
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.getSellPrice()"/> method.</summary>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <param name="__result">The return value of the original method.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This reimplements the original method to add the baby sell price and adult sell price properties.</remarks>
        internal static bool GetSellPricePrefix(FarmAnimal __instance, ref int __result)
        {
            int? babySellPrice = null;
            if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/babySellPrice", out var babySellPriceString))
                if (int.TryParse(babySellPriceString, out var babySellPriceParsed))
                    babySellPrice = babySellPriceParsed;

            int? adultSellPrice = null;
            if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/adultSellPrice", out var adultSellPriceString))
                if (int.TryParse(adultSellPriceString, out var adultSellPriceParsed))
                    adultSellPrice = adultSellPriceParsed;

            // try to determine the sell price based on the baby / adult sell prices
            int sellPrice;
            if (__instance.isBaby())
                sellPrice = babySellPrice ?? adultSellPrice ?? 0;
            else
                sellPrice = adultSellPrice ?? babySellPrice ?? 0;

            if (sellPrice > 0)
            {
                __result = sellPrice;
                return false;
            }

            // fallback to the default game method of calculating sell price
            var friendshipModifier = __instance.friendshipTowardFarmer / 1000f + .3f;
            __result = (int)(__instance.price * friendshipModifier);
            return false;
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.updateWhenNotCurrentLocation(Building, GameTime, GameLocation)"/> method.</summary>
        /// <param name="currentBuilding">The current building the animal is in.</param>
        /// <param name="time">The <see cref="GameTime"/> object that contains time data about the game's frame time.</param>
        /// <param name="environment">The <see cref="GameLocation"/> of the animal.</param>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not net ran.</returns>
        /// <remarks>This reimplements the original method to only allow animals to go outside in seasons that are specified in their content packs.</remarks>
        internal static bool UpdateWhenNotCurrentLocationPrefix(Building currentBuilding, GameTime time, GameLocation environment, FarmAnimal __instance)
        {
            // get private members
            var behaviors = typeof(FarmAnimal).GetMethod("behaviors", BindingFlags.NonPublic | BindingFlags.Instance);
            var doFarmerPushEvent = (NetEvent1Field<int, NetInt>)typeof(FarmAnimal).GetField("doFarmerPushEvent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var doBuildingPokeEvent = (NetEvent0)typeof(FarmAnimal).GetField("doBuildingPokeEvent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var doDiveEvent = (NetEvent0)typeof(FarmAnimal).GetField("doDiveEvent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // poll events
            doFarmerPushEvent.Poll();
            doBuildingPokeEvent.Poll();
            doDiveEvent.Poll();

            // skip if time is paused
            if (!Game1.shouldTimePass())
                return false;

            __instance.update(time, environment, __instance.myID, false);

            // ensure only the host can run the main logic
            if (!Game1.IsMasterGame)
                return false;

            // handle hop
            if (__instance.hopOffset != Vector2.Zero)
            {
                __instance.HandleHop();
                return false;
            }

            // retrieve animal properties
            var seasonsAllowedOutdoors = new List<string> { "spring", "summer", "fall" };
            if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/seasonsAllowedOutdoors", out var seasonsAllowedOutdoorsString))
                seasonsAllowedOutdoors = JsonConvert.DeserializeObject<List<string>>(seasonsAllowedOutdoorsString);

            // check if the animal is able to go outside using the base game algorithm
            var allowedOutside = true;
            if (currentBuilding == null || Game1.random.NextDouble() >= .002f || !currentBuilding.animalDoorOpen || Game1.timeOfDay >= (ModEntry.Instance.Config.BedTime - 270) || Game1.isRaining || environment.farmers.Count > 0)
                allowedOutside = false;

            // ensure animal is allowed outside based on it's SeasonsAllowedOutdoors property
            if (!seasonsAllowedOutdoors.Any(seasonAllowedOutdoors => seasonAllowedOutdoors.ToLower() == Game1.currentSeason.ToLower()))
                allowedOutside = false;

            if (!allowedOutside)
            {
                __instance.UpdateRandomMovements();
                behaviors.Invoke(__instance, new object[] { time, environment });
                return false;
            }

            // get farm location to spawn animal in
            var farm = Game1.getFarm();

            // ensure animal won't be colliding with anything when it leaves the building
            var exitCollisionBox = new Rectangle(
                x: (currentBuilding.tileX + currentBuilding.animalDoor.X) * 64 + 2,
                y: (currentBuilding.tileY + currentBuilding.animalDoor.Y) * 64 + 2,
                width: (__instance.isCoopDweller()) ? 60 : 124,
                height: 60
            );
            var exitCollisionBox2 = new Rectangle(
                x: (currentBuilding.tileX + currentBuilding.animalDoor.X) * 64 + 2,
                y: (currentBuilding.tileY + currentBuilding.animalDoor.Y + 1) * 64 + 2,
                width: (__instance.isCoopDweller()) ? 60 : 124,
                height: 60
            );
            if (farm.isCollidingPosition(exitCollisionBox, Game1.viewport, isFarmer: false, 0, glider: false, __instance, pathfinding: false)
                || farm.isCollidingPosition(exitCollisionBox2, Game1.viewport, isFarmer: false, 0, glider: false, __instance, pathfinding: false))
                return false;

            // remove animal from farm
            if (farm.Animals.ContainsKey(__instance.myID))
                for (int i = farm.Animals.Count() - 1; i >= 0; i--)
                    if (farm.Animals.Pairs.ElementAt(i).Key == __instance.myID)
                    {
                        farm.Animals.Remove(__instance.myID);
                        break;
                    }

            // remove animal from building
            (currentBuilding.indoors.Value as AnimalHouse).Animals.Remove(__instance.myID);

            // add animal to farm, initial values
            farm.Animals.Add(__instance.myID, __instance);
            __instance.faceDirection(2);
            __instance.SetMovingDown(true);
            __instance.Position = new Vector2(currentBuilding.getRectForAnimalDoor().X, (currentBuilding.tileY + currentBuilding.animalDoor.Y) * 64 - (__instance.Sprite.getHeight() * 4 - __instance.GetBoundingBox().Height) + 32);

            // sort out pathfinding
            if (FarmAnimal.NumPathfindingThisTick < FarmAnimal.MaxPathfindingPerTick)
            {
                FarmAnimal.NumPathfindingThisTick++;
                __instance.controller = new PathFindController(
                    c: __instance,
                    location: farm,
                    endFunction: FarmAnimal.grassEndPointFunction,
                    finalFacingDirection: Game1.random.Next(4),
                    eraseOldPathController: false,
                    endBehaviorFunction: FarmAnimal.behaviorAfterFindingGrassPatch,
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
                    y: __instance.controller.pathToEndPoint.Peek().Y * 64 - (__instance.Sprite.getHeight() * 4 - __instance.GetBoundingBox().Height) + 16
                );

                if (!__instance.isCoopDweller())
                    __instance.position.X -= 32;
            }

            __instance.noWarpTimer = 3000;
            currentBuilding.currentOccupants.Value--;
            if (Utility.isOnScreen(__instance.getTileLocationPoint(), 192, farm))
                farm.localSound("sandyStep");

            if (environment.isTileOccupiedByFarmer(__instance.getTileLocation()) != null)
                environment.isTileOccupiedByFarmer(__instance.getTileLocation()).TemporaryPassableTiles.Add(__instance.GetBoundingBox());

            __instance.UpdateRandomMovements();
            behaviors.Invoke(__instance, new object[] { time, environment });

            return false;
        }

        /// <summary>The transpiler for the <see cref="FarmAnimal.updateWhenCurrentLocation(GameTime, GameLocation)"/> method.</summary>
        /// <param name="instructions">The IL instructions.</param>
        /// <returns>The new IL instructions.</returns>
        /// <remarks>This is used to set the offset of animal sprites based off their sourceRect, rather than hard coded to accept only 16x16 animals, when swimming.</remarks>
        internal static IEnumerable<CodeInstruction> UpdateWhenCurrentLocationTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                // this will replace the code: new Point(0, 112)
                // with:                       new Point(0, Sprite.spriteTexture.Height / 2))
                // this is needed so 32x32 animals don't get they sprite sheets messed up when swimming
                if (instruction.opcode == OpCodes.Ldc_I4_S && Convert.ToInt32(instruction.operand) == 112)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(FarmAnimal).GetMethod("get_Sprite"));
                    yield return new CodeInstruction(OpCodes.Ldfld, typeof(AnimatedSprite).GetField("spriteTexture"));
                    yield return new CodeInstruction(OpCodes.Callvirt, typeof(Texture2D).GetMethod("get_Height"));
                    yield return new CodeInstruction(OpCodes.Ldc_I4_2);
                    yield return new CodeInstruction(OpCodes.Div);
                    continue;
                }

                yield return instruction;
            }
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.dayUpdate(GameLocation)"/> method.</summary>
        /// <param name="environtment">The <see cref="GameLocation"/> of the animal.</param>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not net ran.</returns>
        /// <remarks>This reimplements the original method to only allow animals to go outside in seasons that are specified in their content packs.</remarks>
        internal static bool DayUpdatePrefix(GameLocation environtment, FarmAnimal __instance)
        {
            // retrieve animal properties
            var fullnessGain = 255;
            if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/fullnessGain", out var fullnessGainString))
                if (int.TryParse(fullnessGainString, out var fullnessGainParsed))
                    fullnessGain = fullnessGainParsed;

            var happinessGain = 35;
            if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/happinessGain", out var happinessGainString))
                if (int.TryParse(happinessGainString, out var happinessGainParsed))
                    happinessGain = happinessGainParsed;

            // fix daysOwner
            if (__instance.daysOwned < 0)
                __instance.daysOwned.Value = __instance.age;
            __instance.daysOwned.Value++;

            __instance.StopAllActions();
            __instance.health.Value = 3;

            // check if animal is currently outside
            var wasLeftOutLastNight = false;
            if (__instance.home != null && !(__instance.home.indoors.Value as AnimalHouse).Animals.ContainsKey(__instance.myID) && environtment is Farm)
            {
                // check if they were locked out
                if (!__instance.home.animalDoorOpen)
                {
                    __instance.moodMessage.Value = 6;
                    __instance.happiness.Value /= 2;
                    wasLeftOutLastNight = true;
                }
                else // animal is outside but the door was open, move the animal inside
                {
                    (environtment as Farm).Animals.Remove(__instance.myID);
                    (__instance.home.indoors.Value as AnimalHouse).Animals.Add(__instance.myID, __instance);

                    if (Game1.timeOfDay > 1800 && __instance.controller == null)
                        __instance.happiness.Value /= 2;

                    environtment = __instance.home.indoors;
                    __instance.setRandomPosition(environtment);
                    return false; // return as animal shouldn't produce if it didn't make it inside in time
                }
            }

            // ensure the animal was pet; otherwise, reduce happiness and frienship
            if (!__instance.wasPet && !__instance.wasAutoPet)
            {
                __instance.friendshipTowardFarmer.Value = Math.Max(0, __instance.friendshipTowardFarmer - (10 - __instance.friendshipTowardFarmer / 200));
                __instance.happiness.Value = (byte)Math.Max(0, __instance.happiness - __instance.happinessDrain);
            }
            __instance.wasPet.Value = false;
            __instance.wasAutoPet.Value = false;

            // eat hay if there is any
            if (environtment is AnimalHouse)
                for (int i = environtment.Objects.Count() - 1; i >= 0; i--)
                    if (__instance.fullness < 200 && environtment.Objects.Pairs.ElementAt(i).Value.Name.ToLower() == "hay")
                    {
                        environtment.Objects.Remove(environtment.Objects.Pairs.ElementAt(i).Key);
                        __instance.fullness.Value = (byte)Math.Min(byte.MaxValue, __instance.fullness.Value + fullnessGain);

                        if (__instance.fullness >= 200)
                            break;
                    }

            // increase age of animal
            __instance.daysOwned.Value++;
            var random = new Random((int)__instance.myID / 2 + (int)Game1.stats.DaysPlayed);
            if (__instance.fullness > 200 || random.NextDouble() < (__instance.fullness - 30) / 170f)
            {
                __instance.age.Value++;
                if (__instance.age == __instance.ageWhenMature)
                {
                    // chance sprite sheet to be adult
                    __instance.Sprite.LoadTexture(Path.Combine("Animals", __instance.type));
                }

                // increate happiness
                __instance.happiness.Value = (byte)Math.Min(byte.MaxValue, __instance.happiness + happinessGain);
            }

            // decrease animal happiness and friendship if they are hungry
            if (__instance.fullness < 200)
            {
                __instance.happiness.Value = (byte)Math.Max(0, __instance.happiness - 100);
                __instance.friendshipTowardFarmer.Value = Math.Max(0, __instance.friendshipTowardFarmer - 20);
            }

            // ensure animal is an adult and is full and happiness enough to produce
            if (!__instance.isBaby() && Utilities.ShouldAnimalDropToday(__instance))
            {
                var subtype = ModEntry.Instance.Api.GetAnimalSubtypeByInternalName(__instance.type);

                // get all modData products
                var parsedProduces = new List<SavedProduceData>();
                if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/produces", out var producesString))
                    parsedProduces = JsonConvert.DeserializeObject<List<SavedProduceData>>(producesString);

                // decrement each produce's days to produce
                foreach (var parsedProduce in parsedProduces)
                {
                    parsedProduce.DaysLeft = Math.Max(0, parsedProduce.DaysLeft - 1);

                    var animalProduce = subtype.Produce.FirstOrDefault(ap => ap.UniqueName.ToLower() == parsedProduce.UniqueName.ToLower());
                    if (animalProduce == null)
                        continue;

                    // check if the product should be producable (based on season and drop chance)
                    if (parsedProduce.DaysLeft == 0)
                    {
                        // reset the days till next produce if the product is out of season
                        if (!animalProduce.Seasons.Any(season => season.ToLower() == Game1.currentSeason.ToLower()))
                            parsedProduce.DaysLeft = Utilities.DetermineDaysToProduce(animalProduce);

                        // reset the days till next produce if the product fails it's percent chance of dropping
                        if (Game1.random.NextDouble() * 100 + 1 > animalProduce.PercentChance)
                            parsedProduce.DaysLeft = Utilities.DetermineDaysToProduce(animalProduce);
                    }
                }

                // spawn all pending 'lay' produce
                var animalProduces = subtype.Produce.Where(product => product.HarvestType == HarvestType.Lay);

                // get the modData products that have a lay harvest type, and that is pending to drop (zero 'days till next produce' in modData)
                var validProduces = Utilities.GetValidAnimalProduce(animalProduces, __instance);
                var produces = parsedProduces
                    .Where(product => product.DaysLeft <= 0 && validProduces.Any(animalProduct => animalProduct.UniqueName.ToLower() == product.UniqueName.ToLower()));

                // spawn objects
                if (produces.Count() > 0)
                    foreach (var produce in produces)
                    {
                        var animalProduce = animalProduces.FirstOrDefault(ap => ap.UniqueName.ToLower() == produce.UniqueName.ToLower());
                        if (animalProduce == null)
                            continue;

                        // determine item to drop (default or upgraded)
                        var uniqueProduceName = ""; // unique name is kept track to update the saved data
                        var productId = -1;
                        var shouldDropUpgraded = Utilities.ShouldDropUpgradedProduct(animalProduce, __instance);
                        if (shouldDropUpgraded != null)
                        {
                            if (shouldDropUpgraded.Value)
                                (uniqueProduceName, productId) = (animalProduce.UniqueName, animalProduce.UpgradedProductId);
                            else
                                (uniqueProduceName, productId) = (animalProduce.UniqueName, animalProduce.DefaultProductId);
                        }

                        if (productId == -1)
                            continue;

                        // try to spawn the object
                        var amount = Utilities.DetermineDropAmount(animalProduce);
                        if (!SpawnLaidItem(productId, amount, Utilities.DetermineProductQuality(__instance, animalProduce), __instance))
                            continue;

                        // update parsed productss to reset object
                        parsedProduces.First(produce => produce.UniqueName.ToLower() == uniqueProduceName.ToLower()).DaysLeft = Utilities.DetermineDaysToProduce(animalProduce);
                    }

                // update modData
                __instance.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/produces"] = JsonConvert.SerializeObject(parsedProduces);
            }

            // determine mood message for the animal
            if (!wasLeftOutLastNight)
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

            // don't make the animal hungry on festival days
            if (Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
                __instance.fullness.Value = 255;

            // reload animal data
            __instance.reload(__instance.home);

            return false;
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.Eat(GameLocation)"/> method.</summary>
        /// <param name="location">The location of the grass being eaten.</param>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not net ran.</returns>
        /// <remarks>This reimplements the original method to use the content pack specified fullness gain (if there is any).</remarks>
        internal static bool EatPrefix(GameLocation location, FarmAnimal __instance)
        {
            var tilePosition = new Vector2(__instance.GetBoundingBox().Center.X / 64, __instance.GetBoundingBox().Center.Y / 64);
            __instance.isEating.Value = true;

            // eat and destroy grass (if it was fully consumed)
            if (location.terrainFeatures.ContainsKey(tilePosition) && location.terrainFeatures[tilePosition] is Grass grass && grass.reduceBy(__instance.isCoopDweller() ? 2 : 4, tilePosition, location.Equals(Game1.currentLocation)))
                location.terrainFeatures.Remove(tilePosition);

            // retrieve animal properties
            var fullnessGain = 255;
            if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/fullnessGain", out var fullnessGainString))
                if (int.TryParse(fullnessGainString, out var fullnessGainParsed))
                    fullnessGain = fullnessGainParsed;

            // increase animal fullness, happiness, and friendship
            __instance.Sprite.loop = false;
            __instance.fullness.Value = (byte)Math.Min(byte.MaxValue, __instance.fullness.Value + fullnessGain);
            if (__instance.moodMessage != 5 && __instance.moodMessage != 6 && !Game1.isRaining)
            {
                __instance.happiness.Value = byte.MaxValue;
                __instance.friendshipTowardFarmer.Value = Math.Min(1000, __instance.friendshipTowardFarmer + 8);
            }

            return false;
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.isMale()"/> method.</summary>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not net ran.</returns>
        /// <remarks>This reimplements the original method to use the content pack specified gender (if there is any).</remarks>
        internal static bool IsMalePrefix(FarmAnimal __instance, ref bool __result)
        {
            if (__instance.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/isMale", out var isMaleString))
                if (bool.TryParse(isMaleString, out var isMale))
                    __result = isMale;
                else // no gender specified so just have a 50/50 chance
                    __result = new Random((int)__instance.myID).NextDouble() > .5f;

            return false;
        }

        /// <summary>The prefix for the <see cref="FarmAnimal.CanHavePregnancy()"/> method.</summary>
        /// <param name="__instance">The current <see cref="FarmAnimal"/> instance being patched.</param>
        /// <param name="__result">The return value of the original method.</param>
        /// <returns><see langword="false"/>, meaning the original method will not net ran.</returns>
        /// <remarks>This reimplements the original method to allow coop animals to be pregnant and to make sure males can't be pregnant.</remarks>
        internal static bool CanHavePregnancyPrefix(FarmAnimal __instance, ref bool __result)
        {
            __result = !__instance.isMale();
            return false;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Animates the specified animal with the foraging animation.</summary>
        /// <param name="animal">The animal to animate.</param>
        /// <param name="callback">The callback after the animal has finished the forage animation.</param>
        private static void AnimateForage(FarmAnimal animal, AnimatedSprite.endOfAnimationBehavior callback)
        {
            switch (animal.FacingDirection)
            {
                case 0:
                    animal.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(9, 250),
                            new FarmerSprite.AnimationFrame(11, 250),
                            new FarmerSprite.AnimationFrame(9, 250),
                            new FarmerSprite.AnimationFrame(11, 250),
                            new FarmerSprite.AnimationFrame(9, 250),
                            new FarmerSprite.AnimationFrame(11, 250, false, false, callback)
                        });
                    break;
                case 1:
                    animal.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(5, 250),
                            new FarmerSprite.AnimationFrame(7, 250),
                            new FarmerSprite.AnimationFrame(5, 250),
                            new FarmerSprite.AnimationFrame(7, 250),
                            new FarmerSprite.AnimationFrame(5, 250),
                            new FarmerSprite.AnimationFrame(7, 250, false, false, callback)
                        });
                    break;
                case 2:
                    animal.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(1, 250),
                            new FarmerSprite.AnimationFrame(3, 250),
                            new FarmerSprite.AnimationFrame(1, 250),
                            new FarmerSprite.AnimationFrame(3, 250),
                            new FarmerSprite.AnimationFrame(1, 250),
                            new FarmerSprite.AnimationFrame(3, 250, false, false, callback)
                        });
                    break;
                case 3:
                    animal.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>()
                        {
                            new FarmerSprite.AnimationFrame(5, 250, false, true),
                            new FarmerSprite.AnimationFrame(7, 250, false, true),
                            new FarmerSprite.AnimationFrame(5, 250, false, true),
                            new FarmerSprite.AnimationFrame(7, 250, false, true),
                            new FarmerSprite.AnimationFrame(5, 250, false, true),
                            new FarmerSprite.AnimationFrame(7, 250, false, true, callback)
                        });
                    break;
            }

            animal.Sprite.loop = false;
        }

        /// <summary>Spawns a foraged item.</summary>
        /// <param name="id">The id of the object to spawn.</param>
        /// <param name="stackSize">The stack size of the object.</param>
        /// <param name="quality">The quality of the product being produced (4 = iridium, 2 = gold, 1 = silver, 0 = normal)</param>
        /// <param name="standardQualityOnly">Whether the spawned object should have it's quality forced to be normal (this is used to counter the botanist profession).</param>
        /// <param name="animal">The animal to spawn forage produce for.</param>
        /// <returns><see langword="true"/> if the object should be able to spawned again in the same day; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This also updates the modData with the resetted DaysToProduce, this is because the method can be delayed if it's a callback from an animation, this resulted in the modData not being updated and animals endlessly producing forage produce.</remarks>
        private static bool SpawnForagedItem(int id, int stackSize, int quality, bool standardQualityOnly, FarmAnimal animal)
        {
            var objectToSpawn = new StardewValley.Object(animal.getTileLocation(), id, stackSize) { Quality = quality };
            objectToSpawn.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/producedItem"] = ""; // this is used so we can tell if this object should keep it's stack size when being picked up (no value is expected, just the keys presence)
            if (standardQualityOnly)
                objectToSpawn.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/producedShouldBeStandardQuality"] = "";

            if (Utility.spawnObjectAround(Utility.getTranslatedVector2(animal.getTileLocation(), animal.FacingDirection, 1), objectToSpawn, Game1.getFarm()))
            {
                if (id == 430)
                    Game1.stats.TrufflesFound++;

                // check if animal should skip resetting product's DaysLeft
                var allowForageRepeats = true;
                if (animal.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/allowForageRepeats", out var allowForageRepeatsString))
                    if (bool.TryParse(allowForageRepeatsString, out var allowForageRepeatsParsed))
                        allowForageRepeats = allowForageRepeatsParsed;
                if (!allowForageRepeats)
                    return false;

                var random = new Random((int)(animal.myID / 2) + (int)Game1.stats.DaysPlayed + Game1.timeOfDay);
                return random.NextDouble() <= animal.friendshipTowardFarmer / 1500f;
            }

            return true;
        }

        /// <summary>Spawns a laid item.</summary>
        /// <param name="id">The id of the object to spawn.</param>
        /// <param name="stackSize">The stack size of the object.</param>
        /// <param name="quality">The quality of the product being produced (4 = iridium, 2 = gold, 1 = silver, 0 = normal).</param>
        /// <param name="animal">The animal to spawn lay produce for.</param>
        /// <returns><see langword="true"/> if the object was successfully spawned; otherwise, <see langword="false"/>.</returns>
        public static bool SpawnLaidItem(int id, int stackSize, int quality, FarmAnimal animal)
        {
            var needToPlaceProduce = true;
            var objectToSpawn = new StardewValley.Object(Vector2.Zero, id, null, false, true, false, true) { Stack = stackSize, Quality = quality };

            // check if the product can be put in an autograbber
            foreach (var environmentObject in animal.home.indoors.Value.Objects.Values)
            {
                // ensure object is an auto grabber
                if (!environmentObject.bigCraftable || environmentObject.ParentSheetIndex != 165 || environmentObject.heldObject != null)
                    continue;

                // try to place the full object in the auto grabber
                if ((objectToSpawn = (environmentObject.heldObject.Value as Chest).addItem(objectToSpawn) as StardewValley.Object) == null)
                {
                    environmentObject.showNextIndex.Value = true;
                    needToPlaceProduce = false;
                    return true;
                }
            }

            // spawn the object if there was no valid auto grabber and there is a valid space under the animal
            animal.setRandomPosition(animal.home.indoors.Value); // set a random position so if there are multiple 'lay' produce, they can all spawn
            if (needToPlaceProduce && !animal.home.indoors.Value.Objects.ContainsKey(animal.getTileLocation()))
            {
                objectToSpawn.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/producedItem"] = ""; // this is used so we can tell if this object should keep it's stack size when being picked up (no value is expected, just the keys presence)
                animal.home.indoors.Value.Objects.Add(animal.getTileLocation(), objectToSpawn);
                return true;
            }

            return false;
        }
    }
}
