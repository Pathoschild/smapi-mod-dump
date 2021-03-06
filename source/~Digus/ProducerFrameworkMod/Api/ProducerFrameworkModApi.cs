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
using Object = StardewValley.Object;

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
            List<ProducerRule> producerRules = ProducerController.GetProducerRules(producerName);
            return GetRecipes(producerRules);
        }

        private static List<Dictionary<string, object>> GetRecipes(List<ProducerRule> producerRules)
        {
            Dictionary<string, int> machineCache = new Dictionary<string, int>();
            Dictionary<int, string> bigObjects = ProducerFrameworkModEntry.Helper.Content.Load<Dictionary<int, string>>("Data\\BigCraftablesInformation", ContentSource.GameContent);
            List<Dictionary<string, object>> returnValue = new List<Dictionary<string, object>>();
            foreach (ProducerRule producerRule in producerRules)
            {
                //TODO Handle context_tag
                if (!(producerRule.InputKey is int))
                    continue;

                Dictionary<string, object> ruleMap = new Dictionary<string, object>();
                if (machineCache.ContainsKey(producerRule.ProducerName))
                {
                    ruleMap["MachineID"] = machineCache[producerRule.ProducerName];
                }
                else
                {
                    bigObjects.FirstOrDefault(o => o.Value.StartsWith(producerRule.ProducerName + "/"));
                    KeyValuePair<int, string> pair = bigObjects.FirstOrDefault(o => o.Value.StartsWith(producerRule.ProducerName + "/"));
                    if (pair.Value != null)
                    {
                        ruleMap["MachineID"] = pair.Key;
                        machineCache[producerRule.ProducerName] = pair.Key;
                    }
                    else
                    {
                        continue;
                    }
                }

                ruleMap["InputKey"] = producerRule.InputKey;
                List<Dictionary<string, object>> ingredients = new List<Dictionary<string, object>>
                {
                    new Dictionary<string, object>()
                    {
                        {"ID", producerRule.InputKey},
                        {"Count", producerRule.InputStack}
                    }
                };
                producerRule.FuelList.ForEach(f =>
                    ingredients.Add(new Dictionary<string, object>() {{"ID", f.Item1}, {"Count", f.Item2}}));
                ruleMap["Ingredients"] = ingredients;

                List<Dictionary<string, object>> exceptIngredients = new List<Dictionary<string, object>>();
                producerRule.ExcludeIdentifiers?.ForEach(
                    i => exceptIngredients.Add(new Dictionary<string, object>() {{"ID", i}}));
                ruleMap["ExceptIngredients"] = exceptIngredients;

                double probabilities = 0;
                List<Dictionary<string, object>> ruleMapPerOutput = new List<Dictionary<string, object>>();
                foreach (OutputConfig outputConfig in producerRule.OutputConfigs)
                {
                    Dictionary<string, object> outputRuleMap = new Dictionary<string, object>(ruleMap);
                    outputRuleMap["Output"] = outputConfig.OutputIndex;

                    List<Dictionary<string, object>> fuel = new List<Dictionary<string, object>>();

                    List<Dictionary<string, object>> ingredientInput = ingredients;
                    if (outputConfig.RequiredInputStack.HasValue)
                    {
                        ingredientInput = new List<Dictionary<string, object>>
                        {
                            new Dictionary<string, object>()
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

                    List<int> minOutput = new List<int>
                    {
                        outputConfig.OutputStack
                    };
                    if (outputConfig.SilverQualityInput.Probability > 0) minOutput.Add(outputConfig.SilverQualityInput.OutputStack);
                    if (outputConfig.GoldQualityInput.Probability > 0) minOutput.Add(outputConfig.GoldQualityInput.OutputStack);
                    if (outputConfig.IridiumQualityInput.Probability > 0) minOutput.Add(outputConfig.IridiumQualityInput.OutputStack);
                    outputRuleMap["MinOutput"] = minOutput.Min();
                    List<int> maxOutput = new List<int>
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
            return ProducerController.GetProducerRules(producerName);
        }

        public List<ProducerRule> GetProducerRules(Object producerObject)
        {
            return GetProducerRules(producerObject.Name);
        }

        public bool AddContentPack(string directory)
        {
            Regex nameToId = new Regex("[^a-zA-Z0-9_.]");
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
