using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using TehPers.CoreMod.Api;
using TehPers.CoreMod.Api.Conflux.Matching;
using TehPers.CoreMod.Api.ContentLoading;
using TehPers.CoreMod.Api.Drawing.Sprites;
using TehPers.CoreMod.Api.Extensions;
using TehPers.CoreMod.Api.Items;
using TehPers.CoreMod.Api.Items.Crafting.Recipes;
using TehPers.CoreMod.Api.Items.Crafting.Recipes.Parts;
using TehPers.CoreMod.Api.Items.Recipes;
using TehPers.CoreMod.ContentLoading;
using TehPers.CoreMod.ContentPacks.Data;
using TehPers.CoreMod.ContentPacks.Data.Converters;

namespace TehPers.CoreMod.ContentPacks {
    internal class ContentPackLoader {
        private readonly ICoreApi _api;

        public ContentPackLoader(ICoreApi api) {
            this._api = api;
        }

        public void LoadContentPacks() {
            // Get all content packs
            IContentPack[] contentPacks = this._api.Owner.Helper.ContentPacks.GetOwned().ToArray();
            this._api.Owner.Monitor.Log($"Found {contentPacks.Length} content pack{(contentPacks.Length == 1 ? "s" : "")}", LogLevel.Trace);

            // If there aren't any, then just skip this step
            if (!contentPacks.Any()) {
                return;
            }

            this._api.Owner.Monitor.Log($"Loading {contentPacks.Length} content pack(s):", LogLevel.Info);
            foreach (IContentPack contentPack in contentPacks) {
                try {
                    this.LoadContentPack(contentPack);
                    this._api.Owner.Monitor.Log($" - {contentPack.Manifest.UniqueID} loaded successfully", LogLevel.Info);
                } catch (Exception ex) {
                    this._api.Owner.Monitor.Log($" - {contentPack.Manifest.UniqueID} failed to load: {ex}", LogLevel.Error);
                }
            }
        }

        private void LoadContentPack(IContentPack contentPack) {
            // Create the content source for this pack
            ContentPackContentSource contentSource = new ContentPackContentSource(contentPack);

            // Create the translation helper for this pack
            ContentPackTranslationHelper translationHelper = new ContentPackTranslationHelper(this._api, contentSource);

            // Load all content files
            ContentPackDataInfo[] data = this.LoadData(contentSource).ToArray();

            // Load all the items from the content pack
            this.LoadObjects(contentPack, contentSource, translationHelper, data);
            this.LoadWeapons(contentPack, contentSource, translationHelper, data);
            this.LoadRecipes(contentPack, data);
        }

        private IEnumerable<ContentPackDataInfo> LoadData(IContentSource contentSource) => this.LoadData(contentSource, new HashSet<string>(), "", "content.json");
        private IEnumerable<ContentPackDataInfo> LoadData(IContentSource contentSource, ISet<string> loadedPaths, string configDir, string configName) {
            // Combine the directory and file name
            string fullPath = Path.Combine(configDir, configName);

            // Check if this data has already been loaded
            if (!loadedPaths.Add(fullPath)) {
                return Enumerable.Empty<ContentPackDataInfo>();
            }

            // Set up all the custom converters
            void SetSerializerSettings(JsonSerializerSettings settings) {
                settings.Converters.Add(new PossibleConfigValuesDataJsonConverter());
                // settings.Converters.Add(new ContentPackValueJsonConverter());
                settings.Converters.Add(new SColorJsonConverter());
                settings.Converters.Add(new RecipeData.PartJsonConverter());
            }

            // Check if this path points to proper content pack data
            if (!(this._api.Json.ReadJson<ContentPackData>(fullPath, contentSource, SetSerializerSettings) is ContentPackData curData)) {
                throw new Exception($"Invalid content file: {fullPath}");
            }

            // Get all child include paths
            IEnumerable<ContentPackDataInfo> childIncludes = from relativeChildPath in curData.Include
                                                             let fullChildPath = Path.Combine(configDir, relativeChildPath)
                                                             let childDir = Path.GetDirectoryName(fullChildPath)
                                                             let childName = Path.GetFileName(fullChildPath)
                                                             from child in this.LoadData(contentSource, loadedPaths, childDir, childName)
                                                             select child;

            // Return all the paths
            return new ContentPackDataInfo(configDir, configName, curData).Yield().Concat(childIncludes);
        }

        private Dictionary<string, PossibleConfigValuesData> GetConfigStructure(IEnumerable<ContentPackDataInfo> sources) {
            var configPairs = (from source in sources
                               from kv in source.Content.Config
                               select new { Key = kv.Key, PossibleValues = kv.Value, Source = source }).ToArray();

            // Create exceptions for conflicting config keys
            Exception[] exceptions = (from pairGroup in this.GetDuplicateGroups(configPairs, pair => pair.Key)
                                      select new Exception($"Config option '{pairGroup.Key}' is being registered by multiple content files: {string.Join(", ", pairGroup.Select(pair => $"'{pair.Source}'"))}")).ToArray();

            // Throw the exceptions
            if (exceptions.Any()) {
                if (exceptions.Length > 1) {
                    throw new AggregateException(exceptions);
                }

                throw exceptions.First();
            }

            return configPairs.ToDictionary(pair => pair.Key, pair => pair.PossibleValues);
        }

        private void LoadObjects(IContentPack contentPack, IContentSource contentSource, ICoreTranslationHelper translationHelper, IEnumerable<ContentPackDataInfo> sources) {
            var objects = (from source in sources
                           from entry in source.Content.Objects
                           select new { Source = source, Name = entry.Key, Data = entry.Value }).ToArray();

            // Create exceptions for conflicting item names
            Exception[] exceptions = (from itemGroup in this.GetDuplicateGroups(objects, item => item.Name)
                                      select new Exception($"Object '{itemGroup.Key}' is being registered by multiple content files: {string.Join(", ", itemGroup.Select(item => $"'{item.Source.FullPath}'"))}")).ToArray();

            // Throw the exceptions
            if (exceptions.Any()) {
                if (exceptions.Length > 1) {
                    throw new AggregateException(exceptions);
                }

                throw exceptions.First();
            }

            // Create each object
            foreach (var obj in objects) {
                ItemKey key = new ItemKey(contentPack.Manifest, obj.Name);

                // Create the sprite for the object
                ISprite sprite = this.CreateSprite(contentSource, obj.Source, obj.Name, obj.Data);
                if (obj.Data.Tint != Color.White) {
                    sprite = new TintedSprite(sprite, obj.Data.Tint);
                }

                // Create the object's manager
                Category category = new Category(obj.Data.CategoryNumber, obj.Data.CategoryName);
                IModObject manager = obj.Data.Buffs.HasValue.Match<bool, IModObject>()
                    .When(true, () => new ModFood(translationHelper, sprite, key.LocalKey, obj.Data.Cost, obj.Data.Edibility, category, false, obj.Data.Buffs.Value))
                    .When(false, () => new ModObject(translationHelper, sprite, key.LocalKey, obj.Data.Cost, category, obj.Data.Edibility))
                    .ElseThrow();

                // Register the object
                this._api.Items.CommonRegistry.Objects.Register(key, manager);
                this._api.Owner.Monitor.Log($" - {key} registered (object)", LogLevel.Trace);
            }
        }

        private void LoadWeapons(IContentPack contentPack, IContentSource contentSource, ICoreTranslationHelper translationHelper, IEnumerable<ContentPackDataInfo> sources) {
            var weapons = (from source in sources
                           from entry in source.Content.Weapons
                           select new { Source = source, Name = entry.Key, Data = entry.Value }).ToArray();

            // Create exceptions for conflicting item names
            Exception[] exceptions = (from itemGroup in this.GetDuplicateGroups(weapons, item => item.Name)
                                      select new Exception($"Weapon '{itemGroup.Key}' is being registered by multiple content files: {string.Join(", ", itemGroup.Select(item => $"'{item.Source.FullPath}'"))}")).ToArray();

            // Throw the exceptions
            if (exceptions.Any()) {
                if (exceptions.Length > 1) {
                    throw new AggregateException(exceptions);
                }

                throw exceptions.First();
            }

            // Create each weapon
            foreach (var weapon in weapons) {
                ItemKey key = new ItemKey(contentPack.Manifest, weapon.Name);

                // Create the sprite for the object
                ISprite sprite = this.CreateSprite(contentSource, weapon.Source, weapon.Name, weapon.Data);
                if (weapon.Data.Tint != Color.White) {
                    sprite = new TintedSprite(sprite, weapon.Data.Tint);
                }

                // Create the weapon's manager
                ModWeapon manager = new ModWeapon(translationHelper, key.LocalKey, sprite, weapon.Data.Type, weapon.Data.MinDamage, weapon.Data.MaxDamage, weapon.Data.Knockback, weapon.Data.Speed, weapon.Data.Accuracy, weapon.Data.Defense, weapon.Data.AreaOfEffect, weapon.Data.CritChance, weapon.Data.CritMultiplier);

                // Register the object
                this._api.Items.CommonRegistry.Weapons.Register(key, manager);
                this._api.Owner.Monitor.Log($" - {key} registered (weapon)", LogLevel.Trace);
            }
        }

        private void LoadRecipes(IContentPack contentPack, IEnumerable<ContentPackDataInfo> sources) {
            var recipes = (from source in sources
                           from entry in source.Content.Recipes
                           select new { Source = source, Data = entry }).ToArray();

            // Create each recipe
            foreach (var recipe in recipes) {
                // Create each ingredient
                IRecipePart[] ingredients = recipe.Data.Ingredients.Select(ConvertPart).ToArray();

                // Create each result
                IRecipePart[] results = recipe.Data.Results.Select(ConvertPart).ToArray();

                // Create the recipe
                ISprite sprite = results.FirstOrDefault()?.Sprite ?? ingredients.FirstOrDefault()?.Sprite ?? throw new InvalidOperationException("Unable to create a sprite for a recipe.");
                string recipeName = this._api.Items.RegisterCraftingRecipe(new ModRecipe(this._api.TranslationHelper, sprite, results, ingredients, recipe.Data.Name, recipe.Data.IsCooking));

                // TODO: Add conditions when the recipe can be added
                this._api.Owner.Helper.Events.GameLoop.SaveLoaded += (sender, args) => {
                    // Get the target dictionary
                    NetStringDictionary<int, NetInt> targetDictionary = recipe.Data.IsCooking ? Game1.player.craftingRecipes : Game1.player.cookingRecipes;

                    // Add the recipe to the target
                    if (!targetDictionary.ContainsKey(recipeName)) {
                        targetDictionary.Add(recipeName, 0);
                    }
                };

                this._api.Owner.Monitor.Log($" - Added recipe: {recipeName}, Ingredients: {ingredients.Length}, Results: {results.Length}", LogLevel.Debug);
            }

            IRecipePart ConvertPart(RecipeData.Part part) {
                return part.Match<RecipeData.Part, IRecipePart>()
                    .When<RecipeData.ModPart>(modPart => {
                        if (!this._api.Items.Delegator.TryParseKey(modPart.Id, out ItemKey key)) {
                            key = new ItemKey(contentPack.Manifest, modPart.Id);
                        }

                        return new ModRecipePart(this._api, key, part.Quantity);
                    })
                    .When<RecipeData.GamePart>(gamePart => gamePart.Type == RecipeData.ItemType.OBJECT, gamePart => new SObjectRecipePart(this._api, gamePart.Id, gamePart.Quantity))
                    .When<RecipeData.GamePart>(gamePart => gamePart.Type == RecipeData.ItemType.BIGCRAFTABLE, gamePart => new BigCraftableRecipePart(this._api, gamePart.Id, gamePart.Quantity))
                    .When<RecipeData.GamePart>(gamePart => gamePart.Type == RecipeData.ItemType.WEAPON, gamePart => new WeaponRecipePart(this._api, gamePart.Id, gamePart.Quantity))
                    .When<RecipeData.GamePart>(gamePart => gamePart.Type == RecipeData.ItemType.HAT, gamePart => new HatRecipePart(this._api, gamePart.Id, gamePart.Quantity))
                    .ElseThrow(_ => new InvalidOperationException($"Unknown part type: {part.GetType().FullName}"));
            }
        }

        private ISprite CreateSprite(IContentSource contentSource, ContentPackDataInfo source, string key, ItemData objectData) {
            // TODO: Create each possible sprite (in the case of tokens and conditional values)

            // Try to get the sprite location
            if (!(objectData.Texture is string spriteLocation)) {
                throw new Exception($"{key} must have a valid sprite location.");
            }

            // Try to load the sprite
            spriteLocation = Path.Combine(source.Directory, spriteLocation);
            if (!(contentSource.Load<Texture2D>(spriteLocation) is Texture2D spriteTexture)) {
                throw new Exception($"{key}'s sprite location is invalid: {spriteLocation}");
            }

            return this._api.Items.CreateSprite(spriteTexture, objectData.FromArea);
        }

        private IEnumerable<IGrouping<TKey, T>> GetDuplicateGroups<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector) {
            return from item in source // For each item
                   group item by keySelector(item) into g // Group the source paths by the name of the item
                   where g.Skip(1).Any() // Where there's a conflict
                   select g;
        }

        private class ContentPackDataInfo {
            public string FullPath { get; }
            public string Directory { get; }
            public string FileName { get; }
            public ContentPackData Content { get; }

            public ContentPackDataInfo(string directory, string fileName, ContentPackData content) {
                this.FullPath = Path.Combine(directory, fileName);
                this.Directory = directory;
                this.FileName = fileName;
                this.Content = content;
            }
        }
    }
}
