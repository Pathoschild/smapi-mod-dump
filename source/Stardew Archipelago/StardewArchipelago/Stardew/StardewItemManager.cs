/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using StardewArchipelago.Constants.Vanilla;
using StardewArchipelago.Stardew.Ids.Vanilla;
using StardewValley;
using StardewValley.GameData.BigCraftables;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Weapons;
using Object = StardewValley.Object;

namespace StardewArchipelago.Stardew
{
    public class StardewItemManager
    {
        private Dictionary<string, StardewItem> _itemsByQualifiedId;
        private Dictionary<string, StardewObject> _objectsById;
        private Dictionary<string, StardewObject> _objectsByName;
        private Dictionary<string, List<StardewObject>> _objectsByColor;
        private Dictionary<string, List<StardewObject>> _objectsByType;
        private Dictionary<string, BigCraftable> _bigCraftablesById;
        private Dictionary<string, BigCraftable> _bigCraftablesByName;
        private Dictionary<string, StardewBoots> _bootsById;
        private Dictionary<string, StardewBoots> _bootsByName;
        private Dictionary<string, StardewFurniture> _furnitureById;
        private Dictionary<string, StardewFurniture> _furnitureByName;
        private Dictionary<string, StardewHat> _hatsById;
        private Dictionary<string, StardewHat> _hatsByName;
        private Dictionary<string, StardewWeapon> _weaponsById;
        private Dictionary<string, StardewWeapon> _weaponsByName;
        private Dictionary<string, StardewCookingRecipe> _cookingRecipesByName;
        private Dictionary<string, StardewCraftingRecipe> _craftingRecipesByName;        

        private List<string> _priorityIds = new()
        {
            "390",
        };

        public Dictionary<string, string> ItemSuffixes = new()
        {
            { "126", " (Green)" },
            { "180", " (Brown)" },
            { "182", " (Brown)" },
        };

        public StardewItemManager()
        {
            InitializeData();
        }

        public IEnumerable<StardewItem> GetAllItems()
        {
            return _itemsByQualifiedId.Values;
        }

        public bool ItemExists(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                return false;
            }

            return _objectsByName.ContainsKey(itemName) ||
                   _bigCraftablesByName.ContainsKey(itemName) ||
                   _bootsByName.ContainsKey(itemName) ||
                   _furnitureByName.ContainsKey(itemName) ||
                   _hatsByName.ContainsKey(itemName) ||
                   _weaponsByName.ContainsKey(itemName);
        }

        public bool ItemExistsById(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            return _objectsById.ContainsKey(itemId) ||
                   _bigCraftablesById.ContainsKey(itemId) ||
                   _bootsById.ContainsKey(itemId) ||
                   _furnitureById.ContainsKey(itemId) ||
                   _hatsById.ContainsKey(itemId) ||
                   _weaponsById.ContainsKey(itemId);
        }

        public bool ItemExistsByQualifiedId(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            return _itemsByQualifiedId.ContainsKey(itemId);
        }

        public StardewItem GetItemByQualifiedId(string itemId)
        {
            return _itemsByQualifiedId.ContainsKey(itemId) ? _itemsByQualifiedId[itemId] : null;
        }

        public bool ObjectExistsById(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            return _objectsById.ContainsKey(itemId);
        }

        public bool ObjectExists(string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                return false;
            }

            if (_objectsByName.ContainsKey(itemName))
            {
                return true;
            }

            return false; // This is where I should do something about complex items like wines and such
        }

        public StardewItem GetItemByName(string itemName)
        {
            if (_objectsByName.ContainsKey(itemName))
            {
                return _objectsByName[itemName];
            }

            if (_bigCraftablesByName.ContainsKey(itemName))
            {
                return _bigCraftablesByName[itemName];
            }

            if (_bootsByName.ContainsKey(itemName))
            {
                return _bootsByName[itemName];
            }

            if (_furnitureByName.ContainsKey(itemName))
            {
                return _furnitureByName[itemName];
            }

            if (_hatsByName.ContainsKey(itemName))
            {
                return _hatsByName[itemName];
            }

            if (_weaponsByName.ContainsKey(itemName))
            {
                return _weaponsByName[itemName];
            }

            throw new ArgumentException($"Item not found: {itemName}");
        }

        public StardewObject GetObjectById(string itemId)
        {
            if (_objectsById.ContainsKey(itemId))
            {
                return _objectsById[itemId];
            }

            throw new ArgumentException($"Item not found: {itemId}");
        }

        public StardewObject GetObjectByName(string itemName)
        {
            if (_objectsByName.ContainsKey(itemName))
            {
                return _objectsByName[itemName];
            }

            throw new ArgumentException($"Item not found: {itemName}");
        }

        public IEnumerable<StardewObject> GetObjectsWithPhrase(string phrase)
        {
            return _objectsByName.Where(x=> x.Key.Contains(phrase, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value); // I do it all for the berry
        }

        public List<StardewObject> GetObjectsByColor(string color)
        {
            if (_objectsByColor.ContainsKey(color))
            {
                return _objectsByColor[color];
            }

            throw new ArgumentException($"Color not supported: {color}");
        }

        public List<StardewObject> GetObjectsByType(string type)
        {
            if (_objectsByType.ContainsKey(type))
            {
                return _objectsByType[type];
            }
            throw new ArgumentException($"Type not found: {type}");
        }

        public BigCraftable GetBigCraftableById(string itemId)
        {
            if (_bigCraftablesById.ContainsKey(itemId))
            {
                return _bigCraftablesById[itemId];
            }

            throw new ArgumentException($"Item not found: {itemId}");
        }

        public StardewBoots GetBootsById(string itemId)
        {
            if (_bootsById.ContainsKey(itemId))
            {
                return _bootsById[itemId];
            }

            throw new ArgumentException($"Item not found: {itemId}");
        }

        public StardewBoots[] GetAllBoots()
        {
            return _bootsByName.Values.ToArray();
        }

        public StardewFurniture GetFurnitureById(string itemId)
        {
            if (_furnitureById.ContainsKey(itemId))
            {
                return _furnitureById[itemId];
            }

            throw new ArgumentException($"Item not found: {itemId}");
        }

        public StardewHat GetHatById(string itemId)
        {
            if (_hatsById.ContainsKey(itemId))
            {
                return _hatsById[itemId];
            }

            throw new ArgumentException($"Item not found: {itemId}");
        }

        public StardewWeapon GetWeaponById(string weaponId)
        {
            if (_weaponsById.ContainsKey(weaponId))
            {
                return _weaponsById[weaponId];
            }

            throw new ArgumentException($"Weapon not found: {weaponId}");
        }

        public StardewWeapon GetWeaponByName(string weaponName)
        {
            if (_weaponsByName.ContainsKey(weaponName))
            {
                return _weaponsByName[weaponName];
            }

            throw new ArgumentException($"Weapon not found: {weaponName}");
        }

        public StardewRecipe GetRecipeByName(string recipeName)
        {
            if (_cookingRecipesByName.ContainsKey(recipeName))
            {
                return _cookingRecipesByName[recipeName];
            }

            if (_craftingRecipesByName.ContainsKey(recipeName))
            {
                return _craftingRecipesByName[recipeName];
            }

            throw new ArgumentException($"Recipe not found: {recipeName}");
        }

        public StardewWeapon[] GetAllWeapons()
        {
            return _weaponsByName.Values.ToArray();
        }

        public StardewRing[] GetAllRings()
        {
            return _objectsByName.Values.OfType<StardewRing>().ToArray();
        }

        private void InitializeData()
        {
            _itemsByQualifiedId = new Dictionary<string, StardewItem>();
            InitializeObjects();
            InitializeBigCraftables();
            InitializeBoots();
            // InitializeClothing(); var allClothingInformation = Game1.clothingInformation;
            InitializeFurniture();
            InitializeHats();
            // InitializeTools();
            InitializeWeapons();
            InitializeCookingRecipes();
            InitializeCraftingRecipes();
        }

        private void InitializeObjects()
        {
            _objectsById = new Dictionary<string, StardewObject>();
            _objectsByName = new Dictionary<string, StardewObject>();
            _objectsByColor = new Dictionary<string, List<StardewObject>>();
            _objectsByType = new Dictionary<string, List<StardewObject>>();
            var allObjectData = DataLoader.Objects(Game1.content);
            foreach (var (id, objectData) in allObjectData)
            {
                var stardewItem = ParseStardewObjectData(id, objectData);

                if (_objectsById.ContainsKey(id))
                {
                    continue;
                }

                if (_objectsByName.ContainsKey(stardewItem.Name))
                {
                    if (_priorityIds.Contains(id))
                    {
                        _objectsById.Add(id, stardewItem);
                        _objectsByName[stardewItem.Name] = stardewItem;
                    }

                    continue;
                }

                AddObjectByColor(objectData, stardewItem);
                AddObjectByType(objectData, stardewItem);

                _objectsById.Add(id, stardewItem);
                _itemsByQualifiedId.Add(stardewItem.GetQualifiedId(), stardewItem);
                AddItemAndAliasesToNamesDictionary(stardewItem);
            }
        }

        private void AddItemAndAliasesToNamesDictionary(StardewObject stardewItem)
        {
            foreach (var aliasGroup in NameAliases.ItemNameAliasGroups)
            {
                if (!aliasGroup.Contains(stardewItem.Name) && !aliasGroup.Contains(stardewItem.Id))
                {
                    continue;
                }

                foreach (var alias in aliasGroup)
                {
                    _objectsByName.Add(alias, stardewItem);
                }

                return;
            }

            _objectsByName.Add(stardewItem.Name, stardewItem);
        }

        private void InitializeBigCraftables()
        {
            _bigCraftablesById = new Dictionary<string, BigCraftable>();
            _bigCraftablesByName = new Dictionary<string, BigCraftable>();
            var allBigCraftablesData = DataLoader.BigCraftables(Game1.content);
            foreach (var (id, bigCraftableData) in allBigCraftablesData)
            {
                var bigCraftable = ParseStardewBigCraftableData(id, bigCraftableData);

                if (_bigCraftablesById.ContainsKey(id) || _bigCraftablesByName.ContainsKey(bigCraftable.Name))
                {
                    continue;
                }

                _bigCraftablesById.Add(id, bigCraftable);
                _bigCraftablesByName.Add(bigCraftable.Name, bigCraftable);
                _itemsByQualifiedId.Add(bigCraftable.GetQualifiedId(), bigCraftable);
            }
        }

        private void InitializeBoots()
        {
            _bootsById = new Dictionary<string, StardewBoots>();
            _bootsByName = new Dictionary<string, StardewBoots>();
            var allBootsInformation = DataLoader.Boots(Game1.content);
            foreach (var (id, bootsInfo) in allBootsInformation)
            {
                var boots = ParseStardewBootsData(id, bootsInfo);

                if (_bootsById.ContainsKey(id) || _bootsByName.ContainsKey(boots.Name))
                {
                    continue;
                }

                _bootsById.Add(id, boots);
                _bootsByName.Add(boots.Name, boots);
                _itemsByQualifiedId.Add(boots.GetQualifiedId(), boots);
            }
        }

        private void InitializeFurniture()
        {
            _furnitureById = new Dictionary<string, StardewFurniture>();
            _furnitureByName = new Dictionary<string, StardewFurniture>();
            var allFurnitureInformation = DataLoader.Furniture(Game1.content);
            foreach (var (id, furnitureInfo) in allFurnitureInformation)
            {
                var furniture = ParseStardewFurnitureData(id, furnitureInfo);

                if (_furnitureById.ContainsKey(id) || _furnitureByName.ContainsKey(furniture.Name))
                {
                    continue;
                }

                _furnitureById.Add(id, furniture);
                _furnitureByName.Add(furniture.Name, furniture);
                if (furniture.IsLupiniPainting)
                {
                    _furnitureByName.Add($"Lupini: {furniture.Name}", furniture);
                    if (furniture.Name.StartsWith("'"))
                    {
                        _furnitureByName.Add($"Lupini: {furniture.Name.Substring(1, furniture.Name.Length - 2)}", furniture);
                    }
                }

                _itemsByQualifiedId.Add(furniture.GetQualifiedId(), furniture);
            }
        }

        private void InitializeHats()
        {
            _hatsById = new Dictionary<string, StardewHat>();
            _hatsByName = new Dictionary<string, StardewHat>();
            var allHatsInformation = DataLoader.Hats(Game1.content);
            foreach (var (id, hatInfo) in allHatsInformation)
            {
                var hat = ParseStardewHatData(id, hatInfo);

                if (_hatsById.ContainsKey(id) || _hatsByName.ContainsKey(hat.Name))
                {
                    continue;
                }

                _hatsById.Add(id, hat);
                _hatsByName.Add(hat.Name, hat);
                _itemsByQualifiedId.Add(hat.GetQualifiedId(), hat);
            }
        }

        private void InitializeWeapons()
        {
            _weaponsById = new Dictionary<string, StardewWeapon>();
            _weaponsByName = new Dictionary<string, StardewWeapon>();
            var allWeaponsInformation = DataLoader.Weapons(Game1.content);
            foreach (var (id, weaponData) in allWeaponsInformation)
            {
                var weapon = ParseStardewWeaponData(id, weaponData);

                if (_weaponsById.ContainsKey(id) || _weaponsByName.ContainsKey(weapon.Name))
                {
                    continue;
                }

                _weaponsById.Add(id, weapon);
                _weaponsByName.Add(weapon.Name, weapon);
                _itemsByQualifiedId.Add(weapon.GetQualifiedId(), weapon);
            }
        }

        private void InitializeCookingRecipes()
        {
            _cookingRecipesByName = new Dictionary<string, StardewCookingRecipe>();
            var allCookingInformation = DataLoader.CookingRecipes(Game1.content);
            foreach (var (recipeName, recipeInfo) in allCookingInformation)
            {
                var recipe = ParseStardewCookingRecipeData(recipeName, recipeInfo);

                if (_cookingRecipesByName.ContainsKey(recipe.ItemName))
                {
                    continue;
                }

                _cookingRecipesByName.Add(recipe.ItemName, recipe);
                if (NameAliases.RecipeNameAliases.ContainsKey(recipe.ItemName))
                {
                    _cookingRecipesByName.Add(NameAliases.RecipeNameAliases[recipe.ItemName], recipe);
                }
            }
        }

        private void InitializeCraftingRecipes()
        {
            _craftingRecipesByName = new Dictionary<string, StardewCraftingRecipe>();
            var allCraftingInformation = DataLoader.CraftingRecipes(Game1.content);
            foreach (var (recipeName, recipeInfo) in allCraftingInformation)
            {
                var recipe = ParseStardewCraftingRecipeData(recipeName, recipeInfo);

                if (_craftingRecipesByName.ContainsKey(recipe.ItemName))
                {
                    continue;
                }

                _craftingRecipesByName.Add(recipe.ItemName, recipe);
            }
        }

        private void AddObjectByColor(ObjectData objectData, StardewObject stardewObject)
        {
            if (objectData.ContextTags is null)
            {
                return; //Its an object with no tags attached, so wouldn't have a color.
            }
            var firstColor = objectData.ContextTags.FirstOrDefault(x => x.Contains("color_"));
            if (firstColor is null)
            {
                return; // There was no color tag found; throw it out.
            }
            if (firstColor == "color_rainbow" || firstColor == "color_white")
            {
                InitializeOrAddColorObject("Blue", stardewObject);
                InitializeOrAddColorObject("Grey", stardewObject);
                InitializeOrAddColorObject("Red", stardewObject);
                InitializeOrAddColorObject("Yellow", stardewObject);
                InitializeOrAddColorObject("Orange", stardewObject);
                InitializeOrAddColorObject("Purple", stardewObject);
                return;
            }
            if (!Colors.ContextToGeneralColor.TryGetValue(firstColor, out var color))
            {
                return; // Not a relevant color
            }
            InitializeOrAddColorObject(color, stardewObject);

        }

        private void InitializeOrAddColorObject(string color, StardewObject stardewObject)
        {
            if (!_objectsByColor.ContainsKey(color))
            {
                _objectsByColor[color] = new List<StardewObject>();
            }
            _objectsByColor[color].Add(stardewObject);
        }

        private void AddObjectByType(ObjectData objectData, StardewObject stardewObject)
        {
            if (!_objectsByType.ContainsKey(objectData.Type))
            {
                _objectsByType[objectData.Type] = new List<StardewObject>();
            }
            _objectsByType[objectData.Type].Add(stardewObject);
        }

        private StardewObject ParseStardewObjectData(string id, ObjectData objectData)
        {
            var name = objectData.Name;
            var sellPrice = objectData.Price;
            var edibility = objectData.Edibility;
            var type = objectData.Type;
            var category = objectData.Category;
            var displayName = objectData.DisplayName;
            var description = objectData.Description;

            name = NormalizeName(id, name);

            if (type == "Ring")
            {
                return new StardewRing(id, name, sellPrice, edibility, type, category, displayName, description);
            }

            return new StardewObject(id, name, sellPrice, edibility, type, category, displayName, description);
        }

        public string NormalizeName(string id, string name)
        {
            if (ItemSuffixes.ContainsKey(id))
            {
                name += ItemSuffixes[id];
            }

            return name;
        }

        private static BigCraftable ParseStardewBigCraftableData(string id, BigCraftableData bigCraftableData)
        {
            var name = bigCraftableData.Name;
            var price = bigCraftableData.Price;
            var description = bigCraftableData.Description;
            var canBePlacedOutdoors = bigCraftableData.CanBePlacedOutdoors;
            var canBePlacedIndoors = bigCraftableData.CanBePlacedIndoors;
            var fragility = bigCraftableData.Fragility;
            var displayName = bigCraftableData.DisplayName;

            var bigCraftable = new BigCraftable(id, name, price, description, canBePlacedOutdoors, canBePlacedIndoors, fragility, displayName);
            return bigCraftable;
        }

        private static StardewBoots ParseStardewBootsData(string id, string bootsInfo)
        {
            var fields = bootsInfo.Split("/");
            var name = fields[0];
            var description = fields[1];
            var sellPrice = int.Parse(fields[2]);
            var addedDefense = int.Parse(fields[3]);
            var addedImmunity = int.Parse(fields[4]);
            var colorIndex = int.Parse(fields[5]);
            var displayName = fields.Length > 6 ? fields[6] : name;

            var bigCraftable = new StardewBoots(id, name, sellPrice, description, addedDefense,
                addedImmunity, colorIndex, displayName);
            return bigCraftable;
        }

        private static StardewFurniture ParseStardewFurnitureData(string id, string furnitureInfo)
        {
            var fields = furnitureInfo.Split("/");
            var name = fields[0];
            var type = fields[1];
            var tilesheetSize = fields[2];
            var boundingBoxSize = fields[3];
            var price = fields[5];
            var rotations = fields.Length > 6 ? fields[6] : "1";
            var displayName = fields.Length > 7 ? fields[7] : name;
            var placementRestriction = fields.Length > 8 ? fields[8] : "";

            var furniture = new StardewFurniture(id, name, type, tilesheetSize, boundingBoxSize, rotations, price, displayName, placementRestriction);
            return furniture;
        }

        private static StardewHat ParseStardewHatData(string id, string hatInfo)
        {
            var fields = hatInfo.Split("/");
            var name = fields[0];
            var description = fields[1];
            var skipHairDraw = fields[2];
            var ignoreHairstyleOffset = bool.Parse(fields[3]);
            var displayName = fields.Length > 4 ? fields[4] : name;

            var hat = new StardewHat(id, name, description, skipHairDraw, ignoreHairstyleOffset, displayName);
            return hat;
        }

        private static StardewWeapon ParseStardewWeaponData(string id, WeaponData weaponData)
        {
            var name = weaponData.Name;
            var description = weaponData.Description;
            var minDamage = weaponData.MinDamage;
            var maxDamage = weaponData.MaxDamage;
            var knockBack = weaponData.Knockback;
            var speed = weaponData.Speed;
            var precision = weaponData.Precision;
            var defence = weaponData.Defense;
            var type = weaponData.Type;
            var mineBaseLevel = weaponData.MineBaseLevel;
            var mineMinLevel = weaponData.MineMinLevel;
            var areaOfEffect = weaponData.AreaOfEffect;
            var criticalChance = weaponData.CritChance;
            var criticalDamage = weaponData.CritMultiplier;
            var displayName = weaponData.DisplayName;

            if (type == 4)
            {
                return new StardewSlingshot(id, name, description, minDamage, maxDamage, knockBack, speed,
                    precision, defence, type, mineBaseLevel, mineMinLevel, areaOfEffect, criticalChance,
                    criticalDamage, displayName, id == WeaponIds.SLINGSHOT ? 500 : (id == WeaponIds.MASTER_SLINGSHOT ? 1000 : 2000));
            }

            var meleeWeapon = new StardewWeapon(id, name, description, minDamage, maxDamage, knockBack, speed,
                precision, defence, type, mineBaseLevel, mineMinLevel, areaOfEffect, criticalChance,
                criticalDamage, displayName);
            return meleeWeapon;
        }

        private static StardewCookingRecipe ParseStardewCookingRecipeData(string recipeName, string recipeInfo)
        {
            var fields = recipeInfo.Split("/");
            var ingredientsField = fields[0].Split(" ");
            var ingredients = new Dictionary<string, int>();
            for (var i = 0; i < ingredientsField.Length - 1; i += 2)
            {
                ingredients.Add(ingredientsField[i], int.Parse(ingredientsField[i + 1]));
            }
            var unusedField = fields[1];
            var yieldField = fields[2].Split(" ");
            var yieldItemId = yieldField[0];
            var yieldAmount = yieldField.Length > 1 ? int.Parse(yieldField[1]) : 1;
            var unlockConditions = fields[3];
            var displayName = fields.Length > 4 ? fields[4] : recipeName;

            var cookingRecipe = new StardewCookingRecipe(recipeName, ingredients, yieldItemId, yieldAmount, unlockConditions, displayName);
            return cookingRecipe;
        }

        private static StardewCraftingRecipe ParseStardewCraftingRecipeData(string recipeName, string recipeInfo)
        {
            var fields = recipeInfo.Split("/");
            var ingredientsField = fields[0].Split(" ");
            var ingredients = new Dictionary<string, int>();
            for (var i = 0; i < ingredientsField.Length - 1; i += 2)
            {
                ingredients.Add(ingredientsField[i], int.Parse(ingredientsField[i + 1]));
            }
            var unusedField = fields[1];
            var yieldField = fields[2].Split(" ");
            var yieldItemId = yieldField[0];
            var yieldAmount = yieldField.Length > 1 ? int.Parse(yieldField[1]) : 1;
            var bigCraftable = fields[3];
            var unlockConditions = fields[4];
            var displayName = fields.Length > 5 ? fields[5] : recipeName;

            var craftingRecipe = new StardewCraftingRecipe(recipeName, ingredients, yieldItemId, yieldAmount, bigCraftable, unlockConditions, displayName);
            return craftingRecipe;
        }

        public void ExportAllItemsMatching(System.Func<Object, bool> condition, string filePath)
        {
            var objectsToExport = new List<string>();

            objectsToExport.AddRange(GetObjectsToExport(condition, _objectsByName));
            objectsToExport.AddRange(GetObjectsToExport(condition, _bigCraftablesByName));
            objectsToExport.AddRange(GetObjectsToExport(condition, _furnitureByName));
            objectsToExport.AddRange(GetObjectsToExport(condition, _hatsByName));
            objectsToExport.AddRange(GetObjectsToExport(condition, _bootsByName));
            objectsToExport.AddRange(GetObjectsToExport(condition, _weaponsByName));

            var objectsAsJson = JsonConvert.SerializeObject(objectsToExport);
            File.WriteAllText(filePath, objectsAsJson);
        }

        private IEnumerable<string> GetObjectsToExport<T>(System.Func<Object, bool> condition, Dictionary<string, T> objectsByName) where T : StardewItem
        {
            foreach (var (name, svItem) in objectsByName)
            {
                var stardewItem = svItem.PrepareForGivingToFarmer(1);
                if (stardewItem is not Object stardewObject)
                {
                    continue;
                }

                if (!condition(stardewObject))
                {
                    continue;
                }

                yield return name;
            }
        }
    }
}
