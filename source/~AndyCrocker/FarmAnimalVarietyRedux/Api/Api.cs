/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Models.Converted;
using FarmAnimalVarietyRedux.Models.Parsed;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Object = StardewValley.Object;

namespace FarmAnimalVarietyRedux
{
    /// <summary>Provides basic animal apis.</summary>
    public class Api : IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public bool IsInternalName(string name) => name.Contains('.');

        /// <inheritdoc/>
        public string GetInternalName(string name)
        {
            if (IsInternalName(name))
                return name;

            // try to get an internal name
            foreach (var customAnimal in ModEntry.Instance.CustomAnimals)
                if (customAnimal.Name.ToLower() == name.ToLower())
                    return customAnimal.InternalName;

            return null;
        }

        /// <inheritdoc/>
        public void SkipErrorMessagesForTools(params string[] toolNames) => ModEntry.Instance.SkipErrorMessagesForTools.AddRange(toolNames);

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="modUniqueId"/> or <paramref name="animal"/> is <see langword="null"/>.</exception>
        public void AddAnimal(string modUniqueId, ParsedCustomAnimal animal, SoundEffect customSound)
        {
            // validate
            if (modUniqueId == null)
                throw new ArgumentNullException(nameof(modUniqueId));

            if (animal == null)
                throw new ArgumentNullException(nameof(animal));

            var internalAnimalName = $"{modUniqueId}.{animal.Name}";

            if (ModEntry.Instance.Api.GetAnimalByInternalName(internalAnimalName) != null)
            {
                ModEntry.Instance.Monitor.Log($"An animal with the internal name: {internalAnimalName} already exists", LogLevel.Error);
                return;
            }

            if (animal.Subtypes == null || animal.Subtypes.Count == 0)
            {
                ModEntry.Instance.Monitor.Log($"Cannot create animal: {internalAnimalName} as there was no subtypes", LogLevel.Error);
                return;
            }

            if (animal.Action != Action.Add)
            {
                ModEntry.Instance.Monitor.Log($"Tried to add an animal that had an action other then 'Add'", LogLevel.Error);
                return;
            }

            // ensure animal info is valid if the animal is buyable
            if ((animal.IsBuyable ?? true) && (animal.AnimalShopInfo == null || animal.AnimalShopInfo.BuyPrice <= 0 || string.IsNullOrEmpty(animal.AnimalShopInfo.Description)))
            {
                ModEntry.Instance.Monitor.Log($"Animal: {animal.Name} is set to buyable but the shop info is invalid", LogLevel.Error);
                animal.IsBuyable = false;
            }

            // convert objects
            var animalShopInfo = new AnimalShopInfo(animal.AnimalShopInfo?.Description, animal.AnimalShopInfo?.BuyPrice ?? -1);
            var animalTypes = new List<CustomAnimalType>();
            foreach (var subtype in animal.Subtypes)
            {
                var internalSubtypeName = $"{modUniqueId}.{subtype.Name}";
                AddSubtype(internalAnimalName, internalSubtypeName, subtype, animalTypes);
            }

            if (animalTypes.Count == 0)
            {
                ModEntry.Instance.Monitor.Log($"Cannot create animal: {internalAnimalName} as there were no valid subtypes", LogLevel.Error);
                return;
            }

            ModEntry.Instance.CustomAnimals.Add(new CustomAnimal(internalAnimalName, animal.Name, animal.IsBuyable ?? true, animal.CanSwim ?? false, animalShopInfo, animalTypes, customSound, animal.Buildings));
            return;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="modUniqueId"/> or <paramref name="animal"/> is <see langword="null"/>.</exception>
        public void EditAnimal(string modUniqueId, ParsedCustomAnimal animal, SoundEffect customSound)
        {
            // validate
            if (modUniqueId == null)
                throw new ArgumentNullException(nameof(modUniqueId));

            if (animal == null)
                throw new ArgumentNullException(nameof(animal));

            if (string.IsNullOrEmpty(animal.InternalName))
            {
                ModEntry.Instance.Monitor.Log($"Cannot edit animal: {animal.Name} as no internal name was specified", LogLevel.Error);
                return;
            }

            if (animal.Action != Action.Edit)
            {
                ModEntry.Instance.Monitor.Log($"Tried to edit an animal that had an action other then 'Edit'", LogLevel.Error);
                return;
            }

            var animalToEdit = ModEntry.Instance.Api.GetAnimalByInternalName(animal.InternalName);
            if (animalToEdit == null)
            {
                ModEntry.Instance.Monitor.Log($"No animal could be found with the internal name of: {animal.InternalName}", LogLevel.Error);
                return;
            }

            // apply edits to the copy of the animal
            animalToEdit.AnimalShopInfo.BuyPrice = (animal.AnimalShopInfo?.BuyPrice > 0) ? animal.AnimalShopInfo.BuyPrice : animalToEdit.AnimalShopInfo?.BuyPrice ?? 0;
            animalToEdit.AnimalShopInfo.Description = animal.AnimalShopInfo?.Description ?? animalToEdit.AnimalShopInfo?.Description;

            animalToEdit.Name = animal.Name ?? animalToEdit.Name;
            if (animal.IsBuyable.HasValue)
                animalToEdit.IsBuyable = animal.IsBuyable.Value && animalToEdit.AnimalShopInfo?.BuyPrice > 0;
            animalToEdit.CanSwim = animal.CanSwim ?? animalToEdit.CanSwim;
            animalToEdit.CustomSound = customSound ?? animalToEdit.CustomSound;
            animalToEdit.Buildings = animal.Buildings ?? animalToEdit.Buildings;

            // order the subtypes by action first (so they get added first, then edited, then deleted)
            var subtypeEdits = animal.Subtypes.OrderBy(subtype => subtype.Action);
            foreach (var subtype in subtypeEdits)
                switch (subtype.Action)
                {
                    case (Action.Add):
                        AddSubtype(animal.InternalName, $"{modUniqueId}.{subtype.Name}", subtype, animalToEdit.Subtypes);
                        break;
                    case (Action.Edit):
                        EditSubtype(animal.InternalName, subtype.InternalName, subtype);
                        break;
                    case (Action.Delete):
                        DeleteSubtype(animal.InternalName, subtype.InternalName);
                        break;
                    default:
                        ModEntry.Instance.Monitor.Log($"Action: {subtype.Action} on animal: {animal.InternalName} subtype: {subtype.InternalName} is invalid", LogLevel.Error);
                        break;
                }
        }

        /// <inheritdoc/>
        public void DeleteAnimal(string internalName)
        {
            // validate
            if (internalName == null)
                throw new ArgumentNullException(nameof(internalName));

            var animal = ModEntry.Instance.Api.GetAnimalByInternalName(internalName);
            if (animal == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find animal with internal name of: {internalName} while trying to delete it", LogLevel.Error);
                return;
            }

            // delete subtype
            ModEntry.Instance.CustomAnimals.Remove(animal);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="recipe"/> is <see langword="null"/>.</exception>
        public void AddIncubatorRecipe(ParsedIncubatorRecipe recipe)
        {
            // validate
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe));

            if (string.IsNullOrEmpty(recipe.InputId) || recipe.InputId == "-1")
            {
                ModEntry.Instance.Monitor.Log("Cannot create an incubator recipe without an input item id", LogLevel.Error);
                return;
            }

            if (string.IsNullOrEmpty(recipe.InternalAnimalName))
            {
                ModEntry.Instance.Monitor.Log("Cannot create an incubator recipe without an animal name", LogLevel.Error);
                return;
            }

            var animal = ModEntry.Instance.Api.GetAnimalByInternalName(recipe.InternalAnimalName)
                ?? ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(recipe.InternalAnimalName);
            if (animal == null)
            {
                ModEntry.Instance.Monitor.Log($"No animal or animal type could be found with the internal name of: {recipe.InternalAnimalName}", LogLevel.Error);
                return;
            }

            ModEntry.Instance.CustomIncubatorRecipes.Add(new IncubatorRecipe(recipe.IncubatorType, ResolveToken(recipe.InputId), recipe.Chance, recipe.MinutesTillDone, recipe.InternalAnimalName));
        }

        /// <inheritdoc/>
        public List<Object> GetAllAnimalObjects() => ConvertAnimalsToObjects(ModEntry.Instance.CustomAnimals).ToList();

        /// <inheritdoc/>
        public List<Object> GetAllBuyableAnimalObjects()
        {
            var buyableAnimals = ModEntry.Instance.CustomAnimals.Where(animal => animal.IsBuyable && animal.Subtypes.Any(subtype => subtype.IsBuyable));
            return ConvertAnimalsToObjects(buyableAnimals).ToList();
        }


        /*********
        ** Internal Methods
        *********/
        /// <inheritdoc/>
        CustomAnimal IApi.GetAnimalByInternalName(string internalAnimalName) => ModEntry.Instance.CustomAnimals.FirstOrDefault(customAnimal => customAnimal.InternalName.ToLower() == internalAnimalName.ToLower());

        /// <inheritdoc/>
        CustomAnimal IApi.GetAnimalByInternalSubtypeName(string internalSubtypeName) => ModEntry.Instance.CustomAnimals.FirstOrDefault(customAnimal => customAnimal.Subtypes.Any(subtype => subtype.InternalName.ToLower() == internalSubtypeName.ToLower()));

        /// <inheritdoc/>
        CustomAnimalType IApi.GetAnimalSubtypeByInternalName(string internalSubtypeName) => ModEntry.Instance.CustomAnimals.SelectMany(customAnimal => customAnimal.Subtypes).FirstOrDefault(subtype => subtype.InternalName.ToLower() == internalSubtypeName.ToLower());

        /// <inheritdoc/>
        CustomAnimalType IApi.GetAnimalSubtypeByName(string subtypeName) => ModEntry.Instance.CustomAnimals.SelectMany(customAnimal => customAnimal.Subtypes).FirstOrDefault(subtype => subtype.Name.ToLower() == subtypeName.ToLower());


        /*********
        ** Private Methods
        *********/
        /// <summary>Validates and adds a subtype to a collection.</summary>
        /// <param name="internalAnimalName">The internal name of the animal.</param>
        /// <param name="internalSubtypeName">The internal name of the animal subtype.</param>
        /// <param name="subtypeToAdd">The subtype to add.</param>
        /// <param name="subtypeSprites">The subtype sprites and corresponding names of the animal type.</param>
        /// <param name="animalTypes">The collection to add the created subtype to.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="internalAnimalName"/>, <paramref name="internalSubtypeName"/>, <paramref name="subtypeToAdd"/>, <paramref name="subtypeSprites"/>, or <paramref name="animalTypes"/> is <see langword="null"/>.</exception>
        private void AddSubtype(string internalAnimalName, string internalSubtypeName, ParsedCustomAnimalType subtypeToAdd, List<CustomAnimalType> animalTypes)
        {
            // validate
            if (internalAnimalName == null)
                throw new ArgumentNullException(nameof(internalAnimalName));

            if (internalSubtypeName == null)
                throw new ArgumentNullException(nameof(internalSubtypeName));

            if (subtypeToAdd == null)
                throw new ArgumentNullException(nameof(subtypeToAdd));

            if (animalTypes == null)
                throw new ArgumentNullException(nameof(animalTypes));

            if (ModEntry.Instance.Api.GetAnimalSubtypeByInternalName(internalSubtypeName) != null)
            {
                ModEntry.Instance.Monitor.Log($"A subtype with the internal name: {internalSubtypeName} already exists", LogLevel.Error);
                return;
            }

            // ensure the subtype being added has a valid action
            if (subtypeToAdd.Action != Action.Add)
            {
                ModEntry.Instance.Monitor.Log($"Cannot have a subtype action other than 'Add' when adding an animal (in subtype: {subtypeToAdd.InternalName})", LogLevel.Error);
                return;
            }

            // convert produce data
            var produce = new List<AnimalProduce>();
            if (subtypeToAdd.Produce != null)
                foreach (var produceToAdd in subtypeToAdd.Produce)
                    AddProduce(produceToAdd, produce);

            animalTypes.Add(new CustomAnimalType(internalSubtypeName, subtypeToAdd.Name, subtypeToAdd.IsBuyable ?? true, subtypeToAdd.IsIncubatable ?? true, produce, subtypeToAdd.AllowForageRepeats ?? true, subtypeToAdd.DaysTillMature ?? 3, subtypeToAdd.SoundId, subtypeToAdd.FrontAndBackSpriteWidth ?? 16, subtypeToAdd.FrontAndBackSpriteHeight ?? 16, subtypeToAdd.SideSpriteWidth ?? 16, subtypeToAdd.SideSpriteHeight ?? 16, ResolveToken(subtypeToAdd.MeatId), subtypeToAdd.HappinessDrain ?? 7, subtypeToAdd.FullnessGain ?? 255, subtypeToAdd.HappinessGain ?? (byte)(40 - subtypeToAdd.HappinessDrain), subtypeToAdd.AutoPetterFriendshipGain ?? 7, subtypeToAdd.HandPetFriendshipGain ?? 15, subtypeToAdd.WalkSpeed ?? 2, subtypeToAdd.BabySellPrice, subtypeToAdd.AdultSellPrice, subtypeToAdd.IsMale, subtypeToAdd.SeasonsAllowedOutdoors));
        }

        /// <summary>Edits a subtype.</summary>
        /// <param name="internalAnimalName">The internal name of the animal containing the subtype to change.</param>
        /// <param name="internalSubtypeName">The internal name of the subtype to change.</param>
        /// <param name="newSubtypeValues">The new subtype values.</param>
        /// <param name="subtypeSprites">The subtype sprites and corresponding names of the animal type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="internalAnimalName"/>, <paramref name="internalSubtypeName"/>, or <paramref name="newSubtypeValues"/> is <see langword="null"/>.</exception>
        private void EditSubtype(string internalAnimalName, string internalSubtypeName, ParsedCustomAnimalType newSubtypeValues)
        {
            // validate
            if (internalAnimalName == null)
                throw new ArgumentNullException(nameof(internalAnimalName));

            if (internalSubtypeName == null)
                throw new ArgumentNullException(nameof(internalSubtypeName));

            if (newSubtypeValues == null)
                throw new ArgumentNullException(nameof(newSubtypeValues));

            var animal = ModEntry.Instance.Api.GetAnimalByInternalName(internalAnimalName);
            if (animal == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find animal with internal name of: {internalAnimalName} while editing a subtype with internal name of: {internalSubtypeName} while trying to edit it", LogLevel.Error);
                return;
            }

            var subtype = animal.Subtypes.FirstOrDefault(st => st.InternalName.ToLower() == internalSubtypeName.ToLower());
            if (subtype == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find animal subtype with internal name of: {internalSubtypeName} in animal with internal name of: {internalAnimalName} while trying to edit it", LogLevel.Error);
                return;
            }

            // edit subtype
            subtype.Name = newSubtypeValues.Name ?? subtype.Name;
            subtype.IsBuyable = newSubtypeValues.IsBuyable ?? subtype.IsBuyable;
            subtype.IsIncubatable = newSubtypeValues.IsIncubatable ?? subtype.IsIncubatable;
            subtype.AllowForageRepeats = newSubtypeValues.AllowForageRepeats ?? subtype.AllowForageRepeats;
            subtype.DaysTillMature = newSubtypeValues.DaysTillMature ?? subtype.DaysTillMature;
            subtype.SoundId = newSubtypeValues.SoundId ?? subtype.SoundId;
            subtype.FrontAndBackSpriteWidth = newSubtypeValues.FrontAndBackSpriteWidth ?? subtype.FrontAndBackSpriteWidth;
            subtype.FrontAndBackSpriteHeight = newSubtypeValues.FrontAndBackSpriteHeight ?? subtype.FrontAndBackSpriteHeight;
            subtype.SideSpriteWidth = newSubtypeValues.SideSpriteWidth ?? subtype.SideSpriteWidth;
            subtype.SideSpriteHeight = newSubtypeValues.SideSpriteHeight ?? subtype.SideSpriteHeight;
            subtype.MeatId = (newSubtypeValues.MeatId != null) ? ResolveToken(newSubtypeValues.MeatId) : subtype.MeatId;
            subtype.HappinessDrain = newSubtypeValues.HappinessDrain ?? subtype.HappinessDrain;
            subtype.FullnessGain = newSubtypeValues.FullnessGain ?? subtype.FullnessGain;
            subtype.HappinessGain = newSubtypeValues.HappinessGain ?? subtype.HappinessGain;
            subtype.AutoPetterFriendshipGain = newSubtypeValues.AutoPetterFriendshipGain ?? subtype.AutoPetterFriendshipGain;
            subtype.HandPetFriendshipGain = newSubtypeValues.HandPetFriendshipGain ?? subtype.HandPetFriendshipGain;
            subtype.WalkSpeed = newSubtypeValues.WalkSpeed ?? subtype.WalkSpeed;
            subtype.BabySellPrice = newSubtypeValues.BabySellPrice ?? subtype.BabySellPrice;
            subtype.AdultSellPrice = newSubtypeValues.AdultSellPrice ?? subtype.AdultSellPrice;
            subtype.IsMale = newSubtypeValues.IsMale ?? subtype.IsMale;
            subtype.SeasonsAllowedOutdoors = newSubtypeValues.SeasonsAllowedOutdoors ?? subtype.SeasonsAllowedOutdoors;

            // order the produce by action first (so they get added first, then edited, then deleted)
            if (newSubtypeValues.Produce != null)
                foreach (var produce in newSubtypeValues.Produce.OrderBy(produce => produce.Action))
                    switch (produce.Action)
                    {
                        case (Action.Add):
                            AddProduce(produce, subtype.Produce);
                            break;
                        case (Action.Edit):
                            EditProduce(internalAnimalName, internalSubtypeName, produce);
                            break;
                        case (Action.Delete):
                            DeleteProduce(internalAnimalName, internalSubtypeName, produce.UniqueName);
                            break;
                        default:
                            ModEntry.Instance.Monitor.Log($"Action: {produce.Action} on animal: {animal.InternalName} subtype: {subtype.InternalName} produce: <DefaultProductId: {produce.DefaultProductId}, UpgradedProductId: {produce.UpgradedProductId}> is invalid", LogLevel.Error);
                            break;
                    }
        }
        
        /// <summary>Deletes a subtype.</summary>
        /// <param name="internalAnimalName">The internal name of the animal containing the subtype to change.</param>
        /// <param name="internalSubtypeName">The internal name of the subtype to change.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="internalAnimalName"/> or <paramref name="internalSubtypeName"/> is <see langword="null"/>.</exception>
        private void DeleteSubtype(string internalAnimalName, string internalSubtypeName)
        {
            // validate
            if (internalAnimalName == null)
                throw new ArgumentNullException(nameof(internalAnimalName));

            if (internalSubtypeName == null)
                throw new ArgumentNullException(nameof(internalSubtypeName));

            var animal = ModEntry.Instance.Api.GetAnimalByInternalName(internalAnimalName);
            if (animal == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find animal with internal name of: {internalAnimalName} while editing a subtype with internal name of: {internalSubtypeName} while trying to delete it", LogLevel.Error);
                return;
            }

            var subtype = animal.Subtypes.FirstOrDefault(st => st.InternalName.ToLower() == internalSubtypeName.ToLower());
            if (subtype == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find animal subtype with internal name of: {internalSubtypeName} in animal with internal name of: {internalAnimalName} while trying to delete it", LogLevel.Error);
                return;
            }

            // delete subtype
            animal.Subtypes.Remove(subtype);
        }

        /// <summary>Adds a produce to a collection.</summary>
        /// <param name="produceToAdd">The produce to add.</param>
        /// <param name="produces">The collection to add the produce to.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="produceToAdd"/> or <paramref name="produces"/> is <see langword="null"/>.</exception>
        private void AddProduce(ParsedAnimalProduce produceToAdd, List<AnimalProduce> produces)
        {
            // validate
            if (produceToAdd == null)
                throw new ArgumentNullException(nameof(produceToAdd));

            if (produces == null)
                throw new ArgumentNullException(nameof(produces));

            // add produce
            // ensure the produce being added has a valid action
            if (produceToAdd.Action != Action.Add)
            {
                ModEntry.Instance.Monitor.Log($"Cannot have a subtype action other than 'Add' when adding an animal", LogLevel.Error);
                return;
            }

            // ensure produce doesn't already exist
            if (produces.Any(produce => produce.UniqueName.ToLower() == produceToAdd.UniqueName.ToLower()))
            {
                ModEntry.Instance.Monitor.Log($"Cannot add produce to animal as animal already has a produce with the unique name: {produceToAdd.UniqueName}", LogLevel.Error);
                return;
            }

            // ensure a tool is specified if needed
            if (produceToAdd.HarvestType == HarvestType.Tool && string.IsNullOrWhiteSpace(produceToAdd.ToolName))
            {
                ModEntry.Instance.Monitor.Log($"Cannot add produce to animal as no tool was specified but the harvest type is 'Tool'", LogLevel.Error);
                return;
            }

            var defaultProductId = ResolveToken(produceToAdd.DefaultProductId ?? "-1");
            var upgradedProductId = ResolveToken(produceToAdd.UpgradedProductId ?? "-1");
            produces.Add(new AnimalProduce(produceToAdd.UniqueName, defaultProductId, produceToAdd.DefaultProductMinFriendship ?? 0, produceToAdd.DefaultProductMaxFriendship ?? 1000, upgradedProductId, produceToAdd.UpgradedProductMinFriendship ?? 200, produceToAdd.UpgradedProductMaxFriendship ?? 1000, produceToAdd.PercentChanceForUpgradedProduct, produceToAdd.UpgradedProductIsRare ?? false, produceToAdd.HarvestType ?? HarvestType.Lay, produceToAdd.DaysToProduce ?? 1, produceToAdd.ProduceFasterWithCoopMaster ?? false, produceToAdd.ProduceFasterWithShepherd ?? false, produceToAdd.ToolName, produceToAdd.ToolHarvestSound, produceToAdd.Amount ?? 1, produceToAdd.Seasons, produceToAdd.PercentChance ?? 100, produceToAdd.PercentChanceForOneExtra ?? 0, produceToAdd.RequiresMale, produceToAdd.RequiresCoopMaster, produceToAdd.RequiresShepherd, produceToAdd.StandardQualityOnly ?? false, produceToAdd.DoNotAllowDuplicates ?? false, produceToAdd.ShowHarvestableSpriteSheet ?? true));
        }

        /// <summary>Edits a produce.</summary>
        /// <param name="internalAnimalName">The internal name of the animal containing the subtype to change.</param>
        /// <param name="internalSubtypeName">The internal name of the subtype to change.</param>
        /// <param name="newProduceValues">The new produce values.</param>
        private void EditProduce(string internalAnimalName, string internalSubtypeName, ParsedAnimalProduce newProduceValues)
        {
            // validate
            if (internalAnimalName == null)
                throw new ArgumentNullException(nameof(internalAnimalName));

            if (internalSubtypeName == null)
                throw new ArgumentNullException(nameof(internalSubtypeName));

            if (newProduceValues == null)
                throw new ArgumentNullException(nameof(newProduceValues));

            var animal = ModEntry.Instance.Api.GetAnimalByInternalName(internalAnimalName);
            if (animal == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find animal with internal name of: {internalAnimalName} while editing a subtype with internal name of: {internalSubtypeName}", LogLevel.Error);
                return;
            }

            var subtype = animal.Subtypes.FirstOrDefault(st => st.InternalName.ToLower() == internalSubtypeName.ToLower());
            if (subtype == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find animal subtype with internal name of: {internalSubtypeName} in animal with internal name of: {internalAnimalName} while trying to edit it", LogLevel.Error);
                return;
            }

            var produce = subtype.Produce.FirstOrDefault(p => p.UniqueName.ToLower() == newProduceValues.UniqueName.ToLower());
            if (produce == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find produce with a unique name of: {newProduceValues.UniqueName} in animal subtype with internal name of: {internalSubtypeName} in animal with internal name of: {internalAnimalName} while trying to edit it", LogLevel.Error);
                return;
            }

            // edit produce
            produce.DefaultProductMinFriendship = newProduceValues.DefaultProductMinFriendship ?? produce.DefaultProductMinFriendship;
            produce.DefaultProductMaxFriendship = newProduceValues.DefaultProductMaxFriendship ?? produce.DefaultProductMaxFriendship;
            produce.UpgradedProductMinFriendship = newProduceValues.UpgradedProductMinFriendship ?? produce.UpgradedProductMinFriendship;
            produce.UpgradedProductMaxFriendship = newProduceValues.UpgradedProductMaxFriendship ?? produce.UpgradedProductMaxFriendship;
            produce.PercentChanceForUpgradedProduct = newProduceValues.PercentChanceForUpgradedProduct ?? produce.PercentChanceForUpgradedProduct;
            produce.UpgradedProductIsRare = newProduceValues.UpgradedProductIsRare ?? produce.UpgradedProductIsRare;
            produce.HarvestType = newProduceValues.HarvestType ?? produce.HarvestType;
            produce.DaysToProduce = newProduceValues.DaysToProduce ?? produce.DaysToProduce;
            produce.ProduceFasterWithCoopMaster = newProduceValues.ProduceFasterWithCoopMaster ?? produce.ProduceFasterWithCoopMaster;
            produce.ProduceFasterWithShepherd = newProduceValues.ProduceFasterWithShepherd ?? produce.ProduceFasterWithShepherd;
            produce.ToolName = newProduceValues.ToolName ?? produce.ToolName;
            produce.ToolHarvestSound = newProduceValues.ToolHarvestSound ?? produce.ToolHarvestSound;
            produce.Amount = newProduceValues.Amount ?? produce.Amount;
            produce.Seasons = newProduceValues.Seasons ?? produce.Seasons;
            produce.PercentChance = newProduceValues.PercentChance ?? produce.PercentChance;
            produce.PercentChanceForOneExtra = newProduceValues.PercentChanceForOneExtra ?? produce.PercentChanceForOneExtra;
            produce.RequiresMale = newProduceValues.RequiresMale ?? produce.RequiresMale;
            produce.RequiresCoopMaster = newProduceValues.RequiresCoopMaster ?? produce.RequiresCoopMaster;
            produce.RequiresShepherd = newProduceValues.RequiresShepherd ?? produce.RequiresShepherd;
            produce.StandardQualityOnly = newProduceValues.StandardQualityOnly ?? produce.StandardQualityOnly;
            produce.DoNotAllowDuplicates = newProduceValues.DoNotAllowDuplicates ?? produce.DoNotAllowDuplicates;
            produce.ShowHarvestableSpriteSheet = newProduceValues.ShowHarvestableSpriteSheet ?? produce.ShowHarvestableSpriteSheet;
        }

        /// <summary>Deletes a produce.</summary>
        /// <param name="internalAnimalName">The internal name of the animal containing the subtype to change.</param>
        /// <param name="internalSubtypeName">The internal name of the subtype to change.</param>
        /// <param name="uniqueName">The unique name of the produce to delete.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="internalAnimalName"/> or <paramref name="internalSubtypeName"/> is <see langword="null"/>.</exception>
        private void DeleteProduce(string internalAnimalName, string internalSubtypeName, string uniqueName)
        {
            // validate
            if (internalAnimalName == null)
                throw new ArgumentNullException(nameof(internalAnimalName));

            if (internalSubtypeName == null)
                throw new ArgumentNullException(nameof(internalSubtypeName));

            var animal = ModEntry.Instance.Api.GetAnimalByInternalName(internalAnimalName);
            if (animal == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find animal with internal name of: {internalAnimalName} while editing a subtype with internal name of: {internalSubtypeName}", LogLevel.Error);
                return;
            }

            var subtype = animal.Subtypes.FirstOrDefault(st => st.InternalName.ToLower() == internalSubtypeName.ToLower());
            if (subtype == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find animal subtype with internal name of: {internalSubtypeName} in animal with internal name of: {internalAnimalName} while trying to edit it", LogLevel.Error);
                return;
            }

            var produce = subtype.Produce.FirstOrDefault(p => p.UniqueName.ToLower() == uniqueName.ToLower());
            if (produce == null)
            {
                ModEntry.Instance.Monitor.Log($"Couldn't find produce with a unique name of: {uniqueName} in animal subtype with internal name of: {internalSubtypeName} in animal with internal name of: {internalAnimalName} while trying to edit it", LogLevel.Error);
                return;
            }

            // delete produce
            subtype.Produce.Remove(produce);
        }

        /// <summary>Converts a token into a numerical id.</summary>
        /// <param name="token">The token to convert.</param>
        /// <returns>A numerical id.</returns>
        private int ResolveToken(string token)
        {
            // ensure token exists
            if (string.IsNullOrEmpty(token))
                return -1;

            // ensure it's actually a token
            if (!token.Contains(":"))
            {
                // check the inputted value is a number
                if (int.TryParse(token, out int id))
                {
                    return id;
                }
                else
                {
                    ModEntry.Instance.Monitor.Log($"The value: '{token}' isn't a valid token and isn't a number", LogLevel.Error);
                    return -1;
                }
            }

            // ensure there are enough sections of the token to be valid
            var splitToken = token.Split(':');
            if (splitToken.Length != 3)
            {
                ModEntry.Instance.Monitor.Log("Invalid number of arguments passed. Correct layout is: '[uniqueId]:[apiMethodName]:[valueToPass]'", LogLevel.Error);
                return -1;
            }

            var uniqueId = splitToken[0];
            var apiMethodName = splitToken[1];
            var valueToPass = splitToken[2];

            // ensure an api could be found with the unique id
            var api = ModEntry.Instance.Helper.ModRegistry.GetApi(uniqueId);
            if (api == null)
            {
                ModEntry.Instance.Monitor.Log($"No api could be found provided by: {uniqueId}", LogLevel.Error);
                return -1;
            }

            // ensure the api has the correct method
            var apiMethodInfo = api.GetType().GetMethod(apiMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (apiMethodInfo == null)
            {
                ModEntry.Instance.Monitor.Log($"No api method with the name: {apiMethodName} could be found for api provided by: {uniqueId}", LogLevel.Error);
                return -1;
            }

            // ensure the api returned a valid value
            if (!int.TryParse(apiMethodInfo.Invoke(api, new[] { valueToPass }).ToString(), out var apiResult) || apiResult == -1)
            {
                ModEntry.Instance.Monitor.Log($"No valid value was returned from method: {apiMethodName} in api provided by: {uniqueId} with a passed value of: {valueToPass}", LogLevel.Error);
                return -1;
            }

            return apiResult;
        }

        /// <summary>Converts custom animals to <see cref="Object"/>s.</summary>
        /// <param name="animals">The custom animals to convert.</param>
        /// <returns>The converted animals as objects.</returns>
        /// <remarks>This is needed as the purchase animals menu expects objects.</remarks>
        private IEnumerable<Object> ConvertAnimalsToObjects(IEnumerable<CustomAnimal> animals)
        {
            foreach (var animal in animals)
            {
                var animalObject = new Object(1, 1, price: animal.AnimalShopInfo.BuyPrice) 
                { 
                    Name = animal.InternalName,
                    displayName = animal.Name 
                };

                // check if animal has a valid home
                var hasValidHome = false;
                foreach (var building in animal.Buildings)
                    if (Game1.getFarm().isBuildingConstructed(building))
                    {
                        hasValidHome = true;
                        break;
                    }

                if (!hasValidHome)
                    animalObject.Type = $"Requires construction of a {Utilities.ConstructBuildingString(animal.Buildings)}";
                else
                    animalObject.Type = null;

                yield return animalObject;
            }
        }
    }
}
