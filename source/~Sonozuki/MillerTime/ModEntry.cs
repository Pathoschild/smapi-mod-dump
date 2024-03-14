/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using MillerTime.Models.Converted;
using MillerTime.Models.Parsed;
using MillerTime.Patches;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MillerTime
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The recipes of the mill.</summary>
        public List<Recipe> Recipes { get; } = new List<Recipe>();

        /// <summary>The singleton instance of <see cref="MillerTime.ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Public Methods
        *********/
        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Instance = this;

            ApplyHarmonyPatches();
            LoadDefaultRecipes();

            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Invoked when the player loads a save.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        /// <remarks>This is used to load the content packs.</remarks>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // clear all old recipes as to not duplicate
            Recipes.Clear();
            LoadDefaultRecipes();

            // load content packs
            foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
            {
                try
                {
                    this.Monitor.Log($"Loading content pack: {contentPack.Manifest.Name}", LogLevel.Info);

                    // ensure content.json exists
                    var contentFile = Path.Combine(contentPack.DirectoryPath, "content.json");
                    if (!File.Exists(contentFile))
                    {
                        this.Monitor.Log($"Couldn't find content.json file, skipping", LogLevel.Error);
                        continue;
                    }

                    // load recipes
                    var recipes = contentPack.ReadJsonFile<List<ParsedRecipe>>("content.json") ?? new List<ParsedRecipe>();
                    foreach (var recipe in recipes)
                    {
                        // resolve and validate input and ouput items
                        var inputId = ResolveToken(recipe.InputId);
                        var outputId = ResolveToken(recipe.Output?.Id ?? "-1");
                        if (inputId == -1 || outputId == -1)
                        {
                            this.Monitor.Log($"Cannot load recipe: {recipe} as the input or output was invalid, this recipe will be skipped", LogLevel.Error);
                            continue;
                        }

                        // ensure no recipes with this input id already exist
                        if (Recipes.Any(r => r.InputId == inputId))
                        {
                            this.Monitor.Log($"A recipe with an input product id of: {inputId} has already been added, this recipe will be skipped", LogLevel.Warn);
                            continue;
                        }

                        Recipes.Add(new Recipe(inputId, new Output(outputId, recipe.Output.Amount)));
                    }
                }
                catch (Exception ex)
                {
                    this.Monitor.Log($"Failed to load content pack: {ex}", LogLevel.Error);
                }
            }
        }

        /// <summary>Applies the harmony patches.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new Harmony instance for patching game code
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            harmony.Patch(
                original: AccessTools.Method(typeof(Mill), nameof(Mill.doAction)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(MillPatch), nameof(MillPatch.DoActionPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Mill), nameof(Mill.dayUpdate)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(MillPatch), nameof(MillPatch.DayUpdatePrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Mill), nameof(Mill.draw)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(MillPatch), nameof(MillPatch.DrawPrefix)))
            );
        }

        /// <summary>Loads the default game mill products.</summary>
        private void LoadDefaultRecipes()
        {
            Recipes.AddRange(new List<Recipe>()
            {
                new Recipe(271, new Output(423, 1)), // unmilled rice/rice
                new Recipe(262, new Output(246, 1)), // wheat/flour
                new Recipe(284, new Output(245, 3))  // beet/sugar
            });
        }

        /// <summary>Converts a token into a numerical id.</summary>
        /// <param name="token">The token to convert.</param>
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
                    this.Monitor.Log($"The value: '{token}' isn't a valid token and isn't a number", LogLevel.Error);
                    return -1;
                }
            }

            // ensure there are enough sections of the token to be valid
            var splitToken = token.Split(':');
            if (splitToken.Length != 3)
            {
                this.Monitor.Log("Invalid number of arguments passed. Correct layout is: '[uniqueId]:[apiMethodName]:[valueToPass]'", LogLevel.Error);
                return -1;
            }

            var uniqueId = splitToken[0];
            var apiMethodName = splitToken[1];
            var valueToPass = splitToken[2];

            // ensure an api could be found with the unique id
            var api = this.Helper.ModRegistry.GetApi(uniqueId);
            if (api == null)
            {
                this.Monitor.Log($"No api could be found provided by: {uniqueId}", LogLevel.Error);
                return -1;
            }

            // ensure the api has the correct method
            var apiMethodInfo = api.GetType().GetMethod(apiMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (apiMethodInfo == null)
            {
                this.Monitor.Log($"No api method with the name: {apiMethodName} could be found for api provided by: {uniqueId}", LogLevel.Error);
                return -1;
            }

            // ensure the api returned a valid value
            if (!int.TryParse(apiMethodInfo.Invoke(api, new[] { valueToPass }).ToString(), out var apiResult) || apiResult == -1)
            {
                this.Monitor.Log($"No valid value was returned from method: {apiMethodName} in api provided by: {uniqueId} with a passed value of: {valueToPass}", LogLevel.Error);
                return -1;
            }

            return apiResult;
        }
    }
}
