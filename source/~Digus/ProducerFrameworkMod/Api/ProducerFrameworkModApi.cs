/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using ProducerFrameworkMod.ContentPack;
using ProducerFrameworkMod.Controllers;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;
using Microsoft.Xna.Framework.Input;

namespace ProducerFrameworkMod.Api
{
    public class ProducerFrameworkModApi : IProducerFrameworkModApi
    {
        public List<Dictionary<string, object>> GetRecipes()
        {
            List<ProducerRule> producerRules = ProducerController.GetProducerRules();
            return GetRecipes(producerRules);
        }

        public List<Dictionary<string, object>> GetRecipes(string producerName)
        {
            var key = Game1.bigCraftableData.FirstOrDefault(b => producerName.Equals(b.Value.Name)).Key;
            var producerRules = key != null
                ? ProducerController.GetProducerRules(ItemRegistry.type_bigCraftable + key)
                : null;
            return producerRules != null ? GetRecipes(producerRules) : null;
        }

        private static List<Dictionary<string, object>> GetRecipes(List<ProducerRule> producerRules)
        {
            List<Dictionary<string, object>> returnValue = new();
            foreach (ProducerRule producerRule in producerRules)
            {
                Dictionary<string, object> ruleMap = new();

                ruleMap["MachineID"] = producerRule.ProducerQualifiedItemId;
                ruleMap["InputKey"] = producerRule.InputKey;
                List<Dictionary<string, object>> ingredients = new()
                {
                    new()
                    {
                        {"ID", producerRule.InputKey},
                        {"Count", producerRule.InputStack}
                    }
                };
                producerRule.FuelList.ForEach(f =>
                    ingredients.Add(new() {{"ID", f.Item1}, {"Count", f.Item2}}));
                ruleMap["Ingredients"] = ingredients;

                List<Dictionary<string, object>> exceptIngredients = new();
                producerRule.ExcludeIdentifiers?.ForEach(
                    i => exceptIngredients.Add(new() {{"ID", i}}));
                ruleMap["ExceptIngredients"] = exceptIngredients;

                double probabilities = 0;
                List<Dictionary<string, object>> ruleMapPerOutput = new();
                foreach (OutputConfig outputConfig in producerRule.OutputConfigs)
                {
                    Dictionary<string, object> outputRuleMap = new(ruleMap);
                    outputRuleMap["Output"] = outputConfig.OutputItemId;

                    List<Dictionary<string, object>> fuel = new();

                    List<Dictionary<string, object>> ingredientInput = ingredients;
                    if (outputConfig.RequiredInputStack.HasValue)
                    {
                        ingredientInput = new()
                        {
                            new()
                            {
                                {"ID", producerRule.InputKey},
                                {"Count", outputConfig.RequiredInputStack.Value}
                            }
                        };
                    }
                    outputRuleMap["Ingredients"] = new List<Dictionary<string, object>>(ingredientInput)
                        .Union(outputConfig.FuelList
                            .Select(f => new Dictionary<string, object>() { { "ID", f.Item1 }, { "Count", f.Item2 } })
                            .ToList()
                        ).ToList();

                    List<int> minOutput = new()
                    {
                        outputConfig.OutputStack
                    };
                    if (outputConfig.SilverQualityInput.Probability > 0) minOutput.Add(outputConfig.SilverQualityInput.OutputStack);
                    if (outputConfig.GoldQualityInput.Probability > 0) minOutput.Add(outputConfig.GoldQualityInput.OutputStack);
                    if (outputConfig.IridiumQualityInput.Probability > 0) minOutput.Add(outputConfig.IridiumQualityInput.OutputStack);
                    outputRuleMap["MinOutput"] = minOutput.Min();
                    List<int> maxOutput = new()
                    {
                        outputConfig.OutputMaxStack
                    };
                    if (outputConfig.SilverQualityInput.Probability > 0) maxOutput.Add(outputConfig.SilverQualityInput.OutputMaxStack);
                    if (outputConfig.GoldQualityInput.Probability > 0) maxOutput.Add(outputConfig.GoldQualityInput.OutputMaxStack);
                    if (outputConfig.IridiumQualityInput.Probability > 0) maxOutput.Add(outputConfig.IridiumQualityInput.OutputMaxStack);
                    outputRuleMap["MaxOutput"] = Math.Max(minOutput.Min(), maxOutput.Max());

                    double outputProbability = outputConfig.OutputProbability * 100;
                    outputRuleMap["OutputChance"] = outputProbability;
                    probabilities += outputProbability;

                    //PFM properties.
                    outputRuleMap["MinutesUntilReady"] = outputConfig.MinutesUntilReady ?? producerRule.MinutesUntilReady;
                    outputRuleMap["OutputIdentifier"] = outputConfig.OutputIdentifier;
                    outputRuleMap["KeepInputQuality"] = outputConfig.KeepInputQuality;
                    outputRuleMap["OutputQuality"] = outputConfig.OutputQuality;
                    outputRuleMap["InputPriceBased"] = outputConfig.InputPriceBased;
                    outputRuleMap["OutputPriceIncrement"] = outputConfig.OutputPriceIncrement;
                    outputRuleMap["OutputPriceMultiplier"] = outputConfig.OutputPriceMultiplier;
                    outputRuleMap["OutputName"] = outputConfig.OutputName;
                    outputRuleMap["OutputTranslationKey"] = outputConfig.OutputTranslationKey;
                    outputRuleMap["PreserveType"] = outputConfig.PreserveType;
                    outputRuleMap["KeepInputParentIndex"] = outputConfig.KeepInputParentIndex;
                    outputRuleMap["ReplaceWithInputParentIndex"] = outputConfig.ReplaceWithInputParentIndex;
                    outputRuleMap["RequiredInputQuality"] = outputConfig.RequiredInputQuality;
                    outputRuleMap["RequiredSeason"] = outputConfig.RequiredSeason;
                    outputRuleMap["RequiredWeather"] = outputConfig.RequiredWeather?.Select(w=> w.ToString()).ToList();
                    outputRuleMap["RequiredLocation"] = outputConfig.RequiredLocation;
                    outputRuleMap["RequiredOutdoors"] = outputConfig.RequiredOutdoors;
                    outputRuleMap["RequiredMail"] = outputConfig.RequiredMail;
                    outputRuleMap["RequiredEvent"] = outputConfig.RequiredEvent;
                    outputRuleMap["RequiredInputParentIdentifier"] = outputConfig.RequiredInputParentIdentifier;

                    ruleMapPerOutput.Add(outputRuleMap);
                }

                var outputConfigs = ruleMapPerOutput.FindAll(r => (double) r["OutputChance"] <= 0);
                double increment = (100 - probabilities) / outputConfigs.Count;
                ruleMapPerOutput.FindAll(r => (double) r["OutputChance"] <= 0).ForEach(r => r["OutputChance"] = increment);

                returnValue.AddRange(ruleMapPerOutput);
            }

            return returnValue;
        }

        public List<Dictionary<string, object>> GetRecipes(Object producerObject)
        {
            return GetRecipes(producerObject.Name);
        }

        public List<ProducerRule> GetProducerRules(string producerName)
        {
            var key = Game1.bigCraftableData.FirstOrDefault(b => producerName.Equals(b.Value.Name)).Key;
            return key != null ? ProducerController.GetProducerRules(ItemRegistry.type_bigCraftable + key) : null;
        }

        public List<ProducerRule> GetProducerRules(Object producerObject)
        {
            return GetProducerRules(producerObject.QualifiedItemId);
        }

        public bool AddContentPack(string directory)
        {
            Regex nameToId = new("[^a-zA-Z0-9_.]");
            ProducerFrameworkModEntry.ModMonitor.Log($"Reading content pack called through the API from {directory}");
            IContentPack temp = ProducerFrameworkModEntry.Helper.ContentPacks.CreateFake(directory);
            ManifestData info = temp.ReadJsonFile<ManifestData>("content-pack.json");
            if (info == null)
            {
                ProducerFrameworkModEntry.ModMonitor.Log($"\tNo content-pack.json found in {directory}!", LogLevel.Warn);
                return false;
            }

            string id = info.UniqueID ?? nameToId.Replace(info.Author+"."+info.Name, "");
            IContentPack contentPack = ProducerFrameworkModEntry.Helper.ContentPacks.CreateTemporary(directory, id, info.Name, info.Description, info.Author, new SemanticVersion(info.Version));
            return DataLoader.LoadContentPack(contentPack, new SaveLoadedEventArgs());
        }
    }
}
