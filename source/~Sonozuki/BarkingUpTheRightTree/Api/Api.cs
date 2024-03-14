/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using BarkingUpTheRightTree.Models.Converted;
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
        public bool AddTree(string name, Texture2D texture, (float DaysBetweenProduce, string Product, int Amount) tappedProduct, string wood, bool dropsSap, string seed, int requiredToolLevel, bool isStumpInWinter, List<(int DaysBetweenProduce, string Product, int Amount, string[] Seasons)> shakingProducts, List<string> includeIfModIsPresent, List<string> excludeIfModIsPresent, (int DaysBetweenProduce, string Product, int Amount) barkProduct, string modName, float unfertilisedGrowthChance = .2f, float fertilisedGrowthChance = 1)
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
            ModEntry.Instance.RawCustomTrees.Add((id, new ParsedCustomTree(name, tappedProductObject, wood, dropsSap, seed, requiredToolLevel, isStumpInWinter, shakingProductObjects, includeIfModIsPresent, excludeIfModIsPresent, barkProductObject, unfertilisedGrowthChance, fertilisedGrowthChance), texture));

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
        public int GetIdByName(string name) => ModEntry.Instance.RawCustomTrees.FirstOrDefault(customTree => customTree.Data.Name == name).Id;

        /// <inheritdoc/>
        public bool GetRawTreeById(int id, out (int Id, ParsedCustomTree Data, Texture2D Texture) rawTree)
        {
            rawTree = default;

            // try to get tree by id
            if (!ModEntry.Instance.RawCustomTrees.Any(customTree => customTree.Id == id))
                return false;

            rawTree = ModEntry.Instance.RawCustomTrees.FirstOrDefault(customTree => customTree.Id == id);
            return true;
        }

        /// <inheritdoc/>
        public bool GetRawTreeByName(string name, out (int Id, ParsedCustomTree Data, Texture2D Texture) rawTree)
        {
            var id = GetIdByName(name);
            return GetRawTreeById(id, out rawTree);
        }

        /// <inheritdoc/>
        public bool GetTreeById(int id, out CustomTree tree)
        {
            tree = ModEntry.Instance.CustomTrees.FirstOrDefault(customTree => customTree.Id == id);
            return tree != null;
        }

        /// <inheritdoc/>
        public bool GetTreeByName(string name, out CustomTree tree)
        {
            var id = GetIdByName(name);
            return GetTreeById(id, out tree);
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
                if (!GetTreeById(tree.treeType, out var customTree))
                    return false;

                tree.modData[$"{ModEntry.Instance.ModManifest.UniqueID}/daysTillBarkHarvest"] = customTree.BarkProduct.DaysBetweenProduce.ToString();
            }
            return true;
        }
    }
}
