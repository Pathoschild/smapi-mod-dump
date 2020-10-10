/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using StardewModdingAPI;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MoreTrees.Models
{
    /// <summary>Represents a tree content.json file.</summary>
    public class TreeData
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The item the tree drops when using a tap on it.</summary>
        public TimedProduct TappingProduct { get; set; }

        /// <summary>The item the tree drops when it gets cut down.</summary>
        public string WoodProduct { get; set; }

        /// <summary>The item to plant to grow it.</summary>
        public string Seed { get; set; }

        /// <summary>The items the tree can drop whenever it's shaked.</summary>
        public List<TimedProduct> ShakingProducts { get; set; }

        /// <summary>The tree will only get loaded if one of the listed mods is present.</summary>
        public List<string> IncludeIfModIsPresent { get; set; }

        /// <summary>The tree will only get loaded if none of the listed mods are present.</summary>
        public List<string> ExcludeIfModIsPresent { get; set; }

        /// <summary>Whether this tree required the user to have the extended mode of MoreTrees.</summary>
        public bool RequiresExtendedMode { get; set; }

        /// <summary>The item the tree drops when using the 'Barn Remover' tool on it.</summary>
        public TimedProduct BarkProduct { get; set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>Resolve all the API tokens.</summary>
        public void ResolveTokens()
        {
            if (TappingProduct != null)
                TappingProduct.Product = ResolveToken(TappingProduct.Product).ToString();

            if (WoodProduct != null)
                WoodProduct = ResolveToken(WoodProduct).ToString();

            if (Seed != null)
                Seed = ResolveToken(Seed).ToString();

            if (BarkProduct != null)
                BarkProduct.Product = ResolveToken(BarkProduct.Product).ToString();

            foreach (var shakingProduct in ShakingProducts)
                shakingProduct.Product = ResolveToken(shakingProduct.Product).ToString();
        }

        /// <summary>Determine whether the tree data is valid.</summary>
        /// <returns>Whether the tree data is valid.</returns>
        public bool IsValid()
        {
            var isValid = true;

            // TappingProduct.DaysBetweenProduce
            if (TappingProduct.DaysBetweenProduce < 0)
            {
                ModEntry.Instance.Monitor.Log("Failed to validate 'TappingProduct.DaysBetweenProduce': can't be negative", LogLevel.Error);
                isValid = false;
            }

            // TappingProduct.Produce
            if (int.TryParse(TappingProduct.Product, out int tappingProductId))
            {
                if (tappingProductId < 0)
                {
                    ModEntry.Instance.Monitor.Log("Failed to validate 'TappingProduct.Product': an id below 0 isn't valid", LogLevel.Error);
                    isValid = false;
                }
            }
            else
            {
                ModEntry.Instance.Monitor.Log("Failed to validate 'TappingProduct.Product': wasn't a number", LogLevel.Error);
                isValid = false;
            }

            // WoodProduct
            if (int.TryParse(WoodProduct, out int woodProductId))
            {
                if (woodProductId < 0)
                {
                    ModEntry.Instance.Monitor.Log("Failed to validate 'WoodProduct': an id below 0 isn't valid", LogLevel.Error);
                    isValid = false;
                }
            }
            else
            {
                ModEntry.Instance.Monitor.Log("Failed to validate 'WoodProduct': wasn't a number", LogLevel.Error);
                isValid = false;
            }

            // Seed
            if (int.TryParse(Seed, out int seedId))
            {
                if (seedId < 0)
                {
                    ModEntry.Instance.Monitor.Log("Failed to validate 'Seed': an id below 0 isn't valid", LogLevel.Error);
                    isValid = false;
                }
            }
            else
            {
                ModEntry.Instance.Monitor.Log("Failed to validate 'Seed': wasn't a number", LogLevel.Error);
                isValid = false;
            }

            // BarkProduct.DaysBetweenProduce
            if (BarkProduct.DaysBetweenProduce < 0)
            {
                ModEntry.Instance.Monitor.Log("Failed to validate 'BarkProduct.DaysBetweenProduce': can't be negative", LogLevel.Error);
                isValid = false;
            }

            // BarkProduct.Product
            if (int.TryParse(BarkProduct.Product, out int barkProductId))
            {
                if (barkProductId < 0)
                {
                    ModEntry.Instance.Monitor.Log("Failed to validate 'BarkProduct': an id below 0 isn't valid", LogLevel.Error);
                    isValid = false;
                }
            }
            else
            {
                ModEntry.Instance.Monitor.Log("Failed to validate 'BarkProduct': wasn't a number", LogLevel.Error);
                isValid = false;
            }

            // ShakingProducts
            foreach (var shakingProduct in ShakingProducts)
            {
                // shakingProduct.DaysBetweenDropping
                if (shakingProduct.DaysBetweenProduce < 0)
                {
                    ModEntry.Instance.Monitor.Log("Failed to validate 'ShakingProduct.DaysBetweenDropping': must be atleast 1", LogLevel.Error);
                    isValid = false;
                }

                // shakingProduct.Produce
                if (int.TryParse(shakingProduct.Product, out int shakingProductId))
                {
                    if (shakingProductId < 0)
                    {
                        ModEntry.Instance.Monitor.Log("Failed to validate 'TappingProduct.Product': an id below 0 isn't valid", LogLevel.Error);
                        isValid = false;
                    }
                }
                else
                {
                    ModEntry.Instance.Monitor.Log("Failed to validate 'TappingProduct.Product': wasn't a number", LogLevel.Error);
                    isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>Convert a potential token into a numerical id.</summary>
        /// <param name="token">The potential token string.</param>
        /// <returns>A numerical id.</returns>
        private int ResolveToken(string token)
        {
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
                    ModEntry.Instance.Monitor.Log($"The value: {token} isn't a valid token and isn't a number");
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

            string uniqueId = splitToken[0];
            string apiMethodName = splitToken[1];
            string valueToPass = splitToken[2];

            // ensure an api could be found with the unique id
            object api = ModEntry.Instance.Helper.ModRegistry.GetApi(uniqueId);
            if (api == null)
            {
                ModEntry.Instance.Monitor.Log($"No api could be found provided by: {uniqueId}", LogLevel.Error);
                return -1;
            }

            // ensure the api has the correct method
            var apiMethodInfo = ModEntry.Instance.Helper.Reflection.GetMethod(api, apiMethodName);
            if (apiMethodInfo == null)
            {
                ModEntry.Instance.Monitor.Log($"No api method with the name: {apiMethodName} could be found for api provided by: {uniqueId}", LogLevel.Error);
                return -1;
            }

            // ensure the api returned a value
            int apiResult = apiMethodInfo.Invoke<int>(valueToPass);
            if (apiResult == -1)
            {
                ModEntry.Instance.Monitor.Log($"No value was returned from method: {apiMethodName} in api provided by: {uniqueId} with a passed value of: {valueToPass}", LogLevel.Error);
                return -1;
            }

            return apiResult;
        }
    }
}
