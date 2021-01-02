/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

using BarkingUpTheRightTree.Models.Parsed;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;

namespace BarkingUpTheRightTree
{
    /// <summary>Provides basic tree apis.</summary>
    public class Api : IApi
    {
        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public bool AddTree(string name, Texture2D texture, (float DaysBetweenProduce, string Product, int Amount) tappedProduct, string wood, bool dropsSap, string seed, int requiredToolLevel, List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)> shakingProducts, List<string> includeIfModIsPresent, List<string> excludeIfModIsPresent, (int DaysBetweenProduce, string Product, int Amount) barkProduct, string modName, float unfertilisedGrowthChance = .2f, float fertilisedGrowthChance = 1)
        {
            // validate
            if (string.IsNullOrEmpty(name))
            {
                ModEntry.Instance.Monitor.Log("Cannot create a tree with a blank name", LogLevel.Error);
                return false;
            }

            if (texture == null)
            {
                ModEntry.Instance.Monitor.Log("Cannot create a tree without a texture", LogLevel.Error);
                return false;
            }

            // ensure the tree hasn't been added by another mod
            if (ModEntry.Instance.RawCustomTrees.Any(tree => tree.Data.Name.ToLower() == name.ToLower()))
            {
                // get the name of the mod that's already added the tree
                var otherModName = ModEntry.Instance.TreesByMod.FirstOrDefault(treesByMod => treesByMod.Value.Contains(name.ToLower())).Key;

                ModEntry.Instance.Monitor.Log($"A tree by the name: {name} has already been added by {otherModName}.", LogLevel.Warn);
                ModEntry.Instance.Monitor.Log($"If you're using the API directly, prefix the tree name with your mod unique id.", LogLevel.Info);
                return false;
            }

            // ensure the tree can be loaded (using IncludeIfModIsPresent)
            if (includeIfModIsPresent != null && includeIfModIsPresent.Count > 0)
            {
                var loadTree = false;
                foreach (var requiredMod in includeIfModIsPresent)
                {
                    if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded(requiredMod))
                        continue;

                    loadTree = true;
                    break;
                }

                if (!loadTree)
                {
                    ModEntry.Instance.Monitor.Log("Tree won't get loaded as no mods specified in 'IncludeIfModIsPresent' were present.", LogLevel.Info);
                    return false;
                }
            }

            // ensure the tree can be loaded (using ExcludeIfModIsPresent)
            if (excludeIfModIsPresent != null && excludeIfModIsPresent.Count > 0)
            {
                var loadTree = true;
                foreach (var unwantedMod in excludeIfModIsPresent)
                {
                    if (!ModEntry.Instance.Helper.ModRegistry.IsLoaded(unwantedMod))
                        continue;

                    loadTree = false;
                    break;
                }

                if (!loadTree)
                {
                    ModEntry.Instance.Monitor.Log("Tree won't get loaded as a mod specified in 'ExcludeIfModIsPresent' was present.", LogLevel.Info);
                    return false;
                }
            }

            // create objects
            var tappedProductObject = new ParsedTapperTimedProduct(tappedProduct.DaysBetweenProduce, tappedProduct.Product, tappedProduct.Amount);
            var barkProductObject = new ParsedTimedProduct(barkProduct.DaysBetweenProduce, barkProduct.Product, barkProduct.Amount);
            var shakingProductObjects = new List<ParsedSeasonalTimedProduct>();
            foreach (var (DaysBetweenProduce, Product, Amount, Seasons) in shakingProducts)
                shakingProductObjects.Add(new ParsedSeasonalTimedProduct(DaysBetweenProduce, Product, Amount, Seasons));

            // add tree
            var id = ModEntry.Instance.GetPersitantId(name);
            if (id == -1)
            {
                ModEntry.Instance.Monitor.Log($"Failed to add tree: {name} because host player doesn't have a tree with that name loaded.", LogLevel.Error);
                return false;
            }
            ModEntry.Instance.RawCustomTrees.Add((id, new ParsedCustomTree(name, tappedProductObject, wood, dropsSap, seed, requiredToolLevel, shakingProductObjects, includeIfModIsPresent, excludeIfModIsPresent, barkProductObject, unfertilisedGrowthChance, fertilisedGrowthChance), texture));

            // register the tree as being added by the mod
            if (!ModEntry.Instance.TreesByMod.ContainsKey(modName))
                ModEntry.Instance.TreesByMod[modName] = new List<string>();
            ModEntry.Instance.TreesByMod[modName].Add(name.ToLower());

            // recreate the converted trees if a save has already been loaded
            if (Context.IsWorldReady)
                ModEntry.Instance.ConvertRawTrees();

            return true;
        }

        /// <inheritdoc/>
        public IEnumerable<(int Id, Texture2D Texture, (float DaysBetweenProduce, string Product, int Amount) TappedProduct, string Wood, bool DropsSap, string Seed, int RequiredToolLevel, List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)> ShakingProducts, List<string> IncludeIfModIsPresent, List<string> ExcludeIfModIsPresent, (int DaysBetweenProduce, string Product, int Amount) BarkProduct, float UnfertilisedGrowthChance, float FertilisedGrowthChance)> GetAllRawTrees()
        {
            foreach (var rawTree in ModEntry.Instance.RawCustomTrees)
            {
                var tappedProduct = (rawTree.Data.TappedProduct?.DaysBetweenProduce ?? 0, rawTree.Data.TappedProduct?.Product, rawTree.Data.TappedProduct?.Amount ?? 0);
                var barkProduct = (rawTree.Data.BarkProduct?.DaysBetweenProduce ?? 0, rawTree.Data.BarkProduct?.Product, rawTree.Data.BarkProduct?.Amount ?? 0);
                var shakingProducts = new List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)>();
                foreach (var shakingProduct in rawTree.Data.ShakingProducts)
                    shakingProducts.Add((shakingProduct.DaysBetweenProduce, shakingProduct.Product, shakingProduct.Amount, shakingProduct.Seasons));

                yield return (rawTree.Id, rawTree.Texture, tappedProduct, rawTree.Data.Wood, rawTree.Data.DropsSap, rawTree.Data.Seed, rawTree.Data.RequiredToolLevel, shakingProducts, rawTree.Data.IncludeIfModIsPresent, rawTree.Data.ExcludeIfModIsPresent, barkProduct, rawTree.Data.UnfertilisedGrowthChance, rawTree.Data.FertilisedGrowthChance);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<(int Id, Texture2D Texture, (float DaysBetweenProduce, int Product, int Amount) TappedProduct, int Wood, bool DropsSap, int Seed, int RequiredToolLevel, List<(int DaysBetweenProduce, int Product, int Amount, string[] Seasons)> ShakingProducts, List<string> IncludeIfModIsPresent, List<string> ExcludeIfModIsPresent, (int DaysBetweenProduce, int Product, int Amount) BarkProduct, float UnfertilisedGrowthChance, float FertilisedGrowthChance)> GetAllTrees()
        {
            foreach (var tree in ModEntry.Instance.CustomTrees)
            {
                var tappedProduct = (tree.TappedProduct?.DaysBetweenProduce ?? 0, tree.TappedProduct?.Product ?? -1, tree.TappedProduct?.Amount ?? 0);
                var barkProduct = (tree.BarkProduct?.DaysBetweenProduce ?? 0, tree.BarkProduct?.Product ?? -1, tree.BarkProduct?.Amount ?? 0);
                var shakingProducts = new List<(int DaysBetweenProduce, int Product, int Amount, string[])>();
                foreach (var shakingProduct in tree.ShakingProducts)
                    shakingProducts.Add((shakingProduct.DaysBetweenProduce, shakingProduct.Product, shakingProduct.Amount, shakingProduct.Seasons));

                yield return (tree.Id, tree.Texture, tappedProduct, tree.Wood, tree.DropsSap, tree.Seed, tree.RequiredToolLevel, shakingProducts, tree.IncludeIfModIsPresent, tree.ExcludeIfModIsPresent, barkProduct, tree.UnfertilisedGrowthChance, tree.FertilisedGrowthChance);
            }
        }

        /// <inheritdoc/>
        public int GetIdByName(string name) => ModEntry.Instance.RawCustomTrees.FirstOrDefault(customTree => customTree.Data.Name == name).Id;

        /// <inheritdoc/>
        public bool GetRawTreeById(int id, out string name, out Texture2D texture, out (float DaysBetweenProduce, string Product, int Amount) tappedProduct, out string wood, out bool dropsSap, out string seed, out int requiredToolLevel, out List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)> shakingProducts, out List<string> includeIfModIsPresent, out List<string> excludeIfModIsPresent, out (int DaysBetweenProduce, string Product, int Amount) barkProduct, out float unfertilisedGrowthChance, out float fertilisedGrowthChance)
        {
            // set default values
            name = default;
            texture = default;
            tappedProduct = (default, default, default);
            wood = default;
            dropsSap = default;
            seed = default;
            requiredToolLevel = default;
            shakingProducts = default;
            includeIfModIsPresent = default;
            excludeIfModIsPresent = default;
            barkProduct = default;
            unfertilisedGrowthChance = .2f;
            fertilisedGrowthChance = 1;

            // try to get tree by id
            if (!ModEntry.Instance.RawCustomTrees.Any(customTree => customTree.Id == id))
                return false;
            var customTree = ModEntry.Instance.RawCustomTrees.FirstOrDefault(customTree => customTree.Id == id);

            // create tuples
            var tappedProductTuple = (customTree.Data.TappedProduct.DaysBetweenProduce, customTree.Data.TappedProduct.Product, customTree.Data.TappedProduct.Amount);
            var barkProductTuple = (customTree.Data.BarkProduct.DaysBetweenProduce, customTree.Data.BarkProduct.Product, customTree.Data.BarkProduct.Amount);
            var shakingProductsTuples = new List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)>();
            foreach (var shakingProduct in customTree.Data.ShakingProducts)
                shakingProductsTuples.Add((shakingProduct.DaysBetweenProduce, shakingProduct.Product, shakingProduct.Amount, shakingProduct.Seasons));

            // set values
            name = customTree.Data.Name;
            texture = customTree.Texture;
            tappedProduct = tappedProductTuple;
            wood = customTree.Data.Wood;
            dropsSap = customTree.Data.DropsSap;
            seed = customTree.Data.Seed;
            requiredToolLevel = customTree.Data.RequiredToolLevel;
            shakingProducts = shakingProductsTuples;
            includeIfModIsPresent = customTree.Data.IncludeIfModIsPresent != null ? new List<string>(customTree.Data.IncludeIfModIsPresent) : null; // copy to new list to break reference
            excludeIfModIsPresent = customTree.Data.ExcludeIfModIsPresent != null ? new List<string>(customTree.Data.ExcludeIfModIsPresent) : null; // copy to new list to break reference
            barkProduct = barkProductTuple;
            unfertilisedGrowthChance = customTree.Data.UnfertilisedGrowthChance;
            fertilisedGrowthChance = customTree.Data.FertilisedGrowthChance;
            return true;
        }

        /// <inheritdoc/>
        public bool GetRawTreeByName(string name, out int id, out Texture2D texture, out (float DaysBetweenProduce, string Product, int Amount) tappedProduct, out string wood, out bool dropsSap, out string seed, out int requiredToolLevel, out List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)> shakingProducts, out List<string> includeIfModIsPresent, out List<string> excludeIfModIsPresent, out (int DaysBetweenProduce, string Product, int Amount) barkProduct, out float unfertilisedGrowthChance, out float fertilisedGrowthChance)
        {
            id = GetIdByName(name);
            return GetRawTreeById(id, out _, out texture, out tappedProduct, out wood, out dropsSap, out seed, out requiredToolLevel, out shakingProducts, out includeIfModIsPresent, out excludeIfModIsPresent, out barkProduct, out unfertilisedGrowthChance, out fertilisedGrowthChance);
        }

        /// <inheritdoc/>
        public bool GetTreeById(int id, out string name, out Texture2D texture, out (float DaysBetweenProduce, int Product, int Amount) tappedProduct, out int wood, out bool dropsSap, out int seed, out int requiredToolLevel, out List<(int DaysBetweenProduce, int Product, int Amount, string[] Seasons)> shakingProducts, out List<string> includeIfModIsPresent, out List<string> excludeIfModIsPresent, out (int DaysBetweenProduce, int Product, int Amount) barkProduct, out float unfertilisedGrowthChance, out float fertilisedGrowthChance)
        {
            // set default values
            name = default;
            texture = default;
            tappedProduct = (default, -1, default);
            wood = default;
            dropsSap = default;
            seed = default;
            requiredToolLevel = default;
            shakingProducts = default;
            includeIfModIsPresent = default;
            excludeIfModIsPresent = default;
            barkProduct = default;
            unfertilisedGrowthChance = .2f;
            fertilisedGrowthChance = 1;

            // try to get tree by id
            var customTree = ModEntry.Instance.CustomTrees.FirstOrDefault(customTree => customTree.Id == id);
            if (customTree == null)
                return false;

            // create tuples
            var tappedProductTuple = (customTree.TappedProduct.DaysBetweenProduce, customTree.TappedProduct.Product, customTree.TappedProduct.Amount);
            var barkProductTuple = (customTree.BarkProduct.DaysBetweenProduce, customTree.BarkProduct.Product, customTree.BarkProduct.Amount);
            var shakingProductsTuples = new List<(int DaysBetweenProduce, int Product, int Amount, string[] Seasons)>();
            foreach (var shakingProduct in customTree.ShakingProducts)
                shakingProductsTuples.Add((shakingProduct.DaysBetweenProduce, shakingProduct.Product, shakingProduct.Amount, shakingProduct.Seasons));

            // set values
            name = customTree.Name;
            texture = customTree.Texture;
            tappedProduct = tappedProductTuple;
            wood = customTree.Wood;
            dropsSap = customTree.DropsSap;
            seed = customTree.Seed;
            requiredToolLevel = customTree.RequiredToolLevel;
            shakingProducts = shakingProductsTuples;
            includeIfModIsPresent = customTree.IncludeIfModIsPresent != null ? new List<string>(customTree.IncludeIfModIsPresent) : null; // copy to new list to break reference
            excludeIfModIsPresent = customTree.ExcludeIfModIsPresent != null ? new List<string>(customTree.ExcludeIfModIsPresent) : null; // copy to new list to break reference
            barkProduct = barkProductTuple;
            unfertilisedGrowthChance = customTree.UnfertilisedGrowthChance;
            fertilisedGrowthChance = customTree.FertilisedGrowthChance;
            return true;
        }

        /// <inheritdoc/>
        public bool GetTreeByName(string name, out int id, out Texture2D texture, out (float DaysBetweenProduce, int Product, int Amount) tappedProduct, out int wood, out bool dropsSap, out int seed, out int requiredToolLevel, out List<(int DaysBetweenProduce, int Product, int Amount, string[] Seasons)> shakingProducts, out List<string> includeIfModIsPresent, out List<string> excludeIfModIsPresent, out (int DaysBetweenProduce, int Product, int Amount) barkProduct, out float unfertilisedGrowthChance, out float fertilisedGrowthChance)
        {
            id = GetIdByName(name);
            return GetTreeById(id, out _, out texture, out tappedProduct, out wood, out dropsSap, out seed, out requiredToolLevel, out shakingProducts, out includeIfModIsPresent, out excludeIfModIsPresent, out barkProduct, out unfertilisedGrowthChance, out fertilisedGrowthChance);
        }

        /// <inheritdoc/>
        public bool GetBarkState(string locationName, Vector2 tileLocation)
        {
            // ensure location exists
            var location = Game1.getLocationFromName(locationName);
            if (location == null)
                return false;

            // get the tree at the tile location
            if (!location.terrainFeatures.TryGetValue(tileLocation, out var terrainFeature))
                return false;

            if (!(terrainFeature is Tree tree))
                return false;

            // get bark state
            tree.modData.TryGetValue($"{ModEntry.Instance.ModManifest.UniqueID}/daysTillBarkHarvest", out var daysTillBarkHarvest);
            return daysTillBarkHarvest == "0";
        }

        /// <inheritdoc/>
        public bool SetBarkState(string locationName, Vector2 tileLocation, bool hasBark)
        {
            // ensure location exists
            var location = Game1.getLocationFromName(locationName);
            if (location == null)
                return false;

            // get the tree at the tile location
            if (!location.terrainFeatures.TryGetValue(tileLocation, out var terrainFeature))
                return false;

            if (!(terrainFeature is Tree tree))
                return false;

            // set bark state
            if (hasBark)
                tree.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillBarkHarvest"] = "0";
            else
            {
                if (!GetTreeById(tree.treeType, out _, out _, out _, out _, out _, out _, out _, out _, out _, out _, out var barkProduct, out _, out _))
                    return false;

                tree.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillBarkHarvest"] = barkProduct.DaysBetweenProduce.ToString();
            }
            return true;
        }
    }
}
