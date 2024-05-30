/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using ProducerFrameworkMod.ContentPack;
using ProducerFrameworkMod.Utils;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Force.DeepCloner;
using StardewValley;
using StardewValley.GameData.Objects;
using Object = StardewValley.Object;
using System.Xml.Linq;

namespace ProducerFrameworkMod.Controllers
{
    public class ProducerController
    {
        public static readonly List<string> UnsupportedMachines = new()
        {
            "(BC)0", "(BC)1", "(BC)2", "(BC)3", "(BC)4", "(BC)5", "(BC)6", "(BC)7", "(BC)8", "(BC)21", "(BC)56", "(BC)62", "(BC)71", "(BC)94", "(BC)99", "(BC)101", "(BC)105", "(BC)110", "(BC)113", "(BC)126", "(BC)127", "(BC)130", "(BC)136", "(BC)137", "(BC)138", "(BC)139", "(BC)140", "(BC)141", "(BC)156", "(BC)159", "(BC)163", "(BC)165", "(BC)167", "(BC)208", "(BC)209", "(BC)238", "(BC)239", "(BC)247", "(BC)264", "(BC)275", "(O)PotOfGold", "(BC)MiniForge", "(BC)StatueOfTheDwarfKing", "(BC)StatueOfBlessings", "(BC)TextSign", "(O)464", "(O)463"
        };

        private static readonly Dictionary<Tuple<string, object>, ProducerRule> RulesRepository = new();
        private static readonly Dictionary<string, ProducerConfig> ConfigRepository = new();

        private const string WarningNotBugPrefix = "This is just a Warning, don't report this as a bug. ";

        /// <summary>
        /// Adds or replace a custom producer rule to the game.
        /// You should probably call this method everytime a save game loads, to ensure all custom objects are properly loaded.
        /// Producer rule should be unique per name of producer and input identifier.
        /// </summary>
        /// <param name="producerRule">The producer rule to be added or replaced.</param>
        /// <param name="i18n">Optional i18n object, to look for the output name in case a translation key was used.</param>
        /// <param name="modUniqueId">The mod unique id.</param>
        public static void AddProducerItems(ProducerRule producerRule, ITranslationHelper i18n = null, string modUniqueId = null)
        {
            AddProducerItems(new List<ProducerRule>() { producerRule }, i18n, modUniqueId);
        }

        /// <summary>
        /// Adds or replace a list of custom producer rules to the game.
        /// You should probably call this method everytime a save game loads, to ensure all custom objects are properly loaded.
        /// Producer rules should be unique per name of producer and input identifier.
        /// </summary>
        /// <param name="producerRules">A list of producer rules to be added or replaced.</param>
        /// <param name="i18n">Optional i18n object, to look for the output name in case a translation key was used.</param>
        /// <param name="modUniqueId">The mod unique id.</param>
        public static void AddProducerItems(List<ProducerRule> producerRules, ITranslationHelper i18n = null, string modUniqueId = null)
        {
            if (i18n != null)
            {
                ObjectTranslationExtension.TranslationHelpers[modUniqueId] = i18n;
            }
            foreach (var producerRule in producerRules)
            {
                try {
                    producerRule.ModUniqueID = modUniqueId;
                    if (!FindAndValidateProducer(producerRule))
                    {
                        //Do nothing, already logged.
                    }
                    else if (string.IsNullOrEmpty(producerRule.InputIdentifier) && (GetProducerConfig(producerRule.ProducerQualifiedItemId)?.NoInputStartMode == null))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"The InputIdentifier property can't be null or empty if there is no config for 'NoInputStartMode' for producer '{producerRule.ProducerIdentification}'. This rule will be ignored.", producerRule.WarningsLogLevel);
                    }
                    else if ((!string.IsNullOrEmpty(producerRule.InputIdentifier) && GetProducerConfig(producerRule.ProducerQualifiedItemId)?.NoInputStartMode != null))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"The InputIdentifier property can't have a value if there is a config for 'NoInputStartMode' for producer '{producerRule.ProducerIdentification}'. This rule will be ignored.", producerRule.WarningsLogLevel);
                    } 
                    else
                    {
                        if (!string.IsNullOrEmpty(producerRule.InputIdentifier))
                        {
                            producerRule.InputKey = FindObjectKey(producerRule.InputIdentifier) ?? producerRule.InputIdentifier;
                        }

                        if (producerRule.OutputIdentifier != null)
                        {
                            producerRule.OutputConfigs.Add(new OutputConfig()
                            {
                                ModUniqueID = producerRule.ModUniqueID,
                                OutputIdentifier = producerRule.OutputIdentifier,
                                OutputName = producerRule.OutputName,
                                OutputTranslationKey = producerRule.OutputTranslationKey,
                                OutputGenericParentName = producerRule.OutputGenericParentName,
                                OutputGenericParentNameTranslationKey = producerRule.OutputGenericParentNameTranslationKey,
                                PreserveType = producerRule.PreserveType,
                                KeepInputParentIndex = producerRule.KeepInputParentIndex,
                                ReplaceWithInputParentIndex = producerRule.ReplaceWithInputParentIndex,
                                InputPriceBased = producerRule.InputPriceBased,
                                OutputPriceIncrement = producerRule.OutputPriceIncrement,
                                OutputPriceMultiplier = producerRule.OutputPriceMultiplier,
                                KeepInputQuality = producerRule.KeepInputQuality,
                                OutputQuality = producerRule.OutputQuality,
                                OutputStack = producerRule.OutputStack,
                                OutputMaxStack = producerRule.OutputMaxStack,
                                SilverQualityInput = producerRule.SilverQualityInput,
                                GoldQualityInput = producerRule.GoldQualityInput,
                                IridiumQualityInput = producerRule.IridiumQualityInput,
                                OutputColorConfig = producerRule.OutputColorConfig
                            });
                        }
                        producerRule.OutputConfigs.AddRange(producerRule.AdditionalOutputs);

                        foreach (OutputConfig outputConfig in producerRule.OutputConfigs)
                        {
                            outputConfig.ModUniqueID = producerRule.ModUniqueID;
                            outputConfig.OutputItemId = FindObjectKey(outputConfig.OutputIdentifier, true);
                            if(outputConfig.OutputItemId == null)
                            {
                                ProducerFrameworkModEntry.ModMonitor.Log($"{WarningNotBugPrefix}No Output found for '{outputConfig.OutputIdentifier}', producer '{producerRule.ProducerIdentification}' and input '{producerRule.InputIdentifier}'. This rule will be ignored.", producerRule.WarningsLogLevel);
                                break;
                            }
                            
                            foreach (var fuel in outputConfig.RequiredFuel)
                            {
                                var fuelItemId = FindObjectKey(fuel.Key);
                                if (fuelItemId == null)
                                {
                                    ProducerFrameworkModEntry.ModMonitor.Log($"{WarningNotBugPrefix}No required fuel found for '{fuel.Key}', producer '{producerRule.ProducerIdentification}' and input '{producerRule.InputIdentifier}'. This rule will be ignored.", producerRule.WarningsLogLevel);
                                    outputConfig.OutputItemId = null;
                                    break;
                                }
                                outputConfig.FuelList.Add(new Tuple<string, int>(fuelItemId, fuel.Value));
                            }
                        }
                        if (producerRule.OutputConfigs.Any(p => p.OutputItemId == null))
                        {
                            continue;
                        }

                        if (producerRule.FuelIdentifier != null)
                        {
                            producerRule.AdditionalFuel[producerRule.FuelIdentifier] = producerRule.FuelStack;
                        }
                        foreach (var fuel in producerRule.AdditionalFuel)
                        {
                            string fuelItemId = FindObjectKey(fuel.Key);
                            if (fuelItemId == null)
                            {
                                ProducerFrameworkModEntry.ModMonitor.Log($"{WarningNotBugPrefix}No fuel found for '{fuel.Key}', producer '{producerRule.ProducerIdentification}' and input '{producerRule.InputIdentifier}'. This rule will be ignored.", producerRule.WarningsLogLevel);
                                break;
                            }
                            producerRule.FuelList.Add(new Tuple<string, int>(fuelItemId, fuel.Value));
                        }
                        if (producerRule.AdditionalFuel.Count != producerRule.FuelList.Count)
                        {
                            continue;
                        }

                        if (producerRule.PlacingAnimationColorName != null)
                        {
                            try
                            {
                                producerRule.PlacingAnimationColor = DataLoader.Helper.Reflection.GetProperty<Color>(typeof(Color), producerRule.PlacingAnimationColorName).GetValue();
                            }
                            catch (Exception)
                            {
                                ProducerFrameworkModEntry.ModMonitor.Log($"Color '{producerRule.PlacingAnimationColorName}' isn't valid. Check XNA Color Chart for valid names. It'll use the default color '{producerRule.PlacingAnimationColor}'.");
                            }
                        }
                        AddRuleToRepository(producerRule);
                    }
                }
                catch (Exception e)
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Unexpected error for rule from '{producerRule?.ModUniqueID}', producer '{producerRule?.ProducerIdentification}' and input '{producerRule?.InputIdentifier}'. This rule will be ignored.", LogLevel.Error);
                    ProducerFrameworkModEntry.ModMonitor.Log(e.Message + "\n" + e.StackTrace);
                }
                foreach (var n in producerRule.AdditionalProducerNames)
                {
                    ProducerRule newProducerRule = null;
                    try {
                        newProducerRule = producerRule.DeepClone();
                        newProducerRule.ProducerName = n;
                        newProducerRule.ProducerQualifiedItemId = null;
                        newProducerRule.AdditionalProducerNames.Clear();
                        newProducerRule.AdditionalProducerQualifiedItemId.Clear();
                        if (FindAndValidateProducer(newProducerRule))
                        {
                            AddRuleToRepository(newProducerRule);
                        }
                    }
                    catch (Exception e)
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"Unexpected error for rule from '{newProducerRule?.ModUniqueID}', producer '{newProducerRule?.ProducerIdentification}' and input '{newProducerRule?.InputIdentifier}'. This rule will be ignored.", LogLevel.Error);
                        ProducerFrameworkModEntry.ModMonitor.Log(e.Message + "\n" + e.StackTrace);
                    }
                }   
                producerRule.AdditionalProducerNames.Clear();
                foreach (var n in producerRule.AdditionalProducerQualifiedItemId)
                {
                    ProducerRule newProducerRule = null;
                    try
                    {
                        newProducerRule = producerRule.DeepClone();
                        newProducerRule.ProducerQualifiedItemId = n;
                        newProducerRule.ProducerName = null;
                        newProducerRule.AdditionalProducerNames.Clear();
                        newProducerRule.AdditionalProducerQualifiedItemId.Clear();
                        if (FindAndValidateProducer(newProducerRule))
                        {
                            AddRuleToRepository(newProducerRule);
                        }
                    }
                    catch (Exception e)
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"Unexpected error for rule from '{newProducerRule?.ModUniqueID}', producer '{newProducerRule.AdditionalProducerQualifiedItemId}' and input '{newProducerRule?.InputIdentifier}'. This rule will be ignored.", LogLevel.Error);
                        ProducerFrameworkModEntry.ModMonitor.Log(e.Message + "\n" + e.StackTrace);
                    }
                }
                producerRule.AdditionalProducerQualifiedItemId.Clear();
            }
        }

        private static void AddRuleToRepository(ProducerRule producerRule)
        {
            Tuple<string, object> ruleKey = new (producerRule.ProducerQualifiedItemId, producerRule.InputKey);
            if (RulesRepository.TryGetValue(ruleKey, out var oldRule))
            {
                if (oldRule.ModUniqueID != producerRule.ModUniqueID)
                {
                    if (oldRule.OverrideMod.Contains(producerRule.ModUniqueID) &&
                        producerRule.OverrideMod.Contains(oldRule.ModUniqueID))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Both mod '{oldRule.ModUniqueID}' and '{producerRule.ModUniqueID}' are saying they should override the rule for producer '{producerRule.ProducerIdentification}' and input '{producerRule.InputIdentifier}'. You should report the problem to these mod's authors. Rule from mod '{oldRule.ModUniqueID}' will be used.",
                            LogLevel.Warn);
                        return;
                    }
                    else if (producerRule.OverrideMod.Contains(oldRule.ModUniqueID))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Mod '{producerRule.ModUniqueID}' is overriding mod '{oldRule.ModUniqueID}' rule for producer '{producerRule.ProducerIdentification}' and input '{producerRule.InputIdentifier}'.",
                            LogLevel.Debug);
                    }
                    else
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Mod '{producerRule.ModUniqueID}' can't override mod '{oldRule.ModUniqueID}' rule for producer '{producerRule.ProducerIdentification}' and input '{producerRule.InputIdentifier}'. This rule will be ignored.",
                            LogLevel.Debug);
                        return;
                    }
                }
            }
            RulesRepository[ruleKey] = producerRule;
        }

        /// <summary>
        /// Adds or replace the producer config.
        /// </summary>
        /// <param name="producersConfig">The producer config to be added or replaced.</param>
        /// <param name="modUniqueId">The mod unique id.</param>
        /// 
        public static void AddProducerConfig(ProducerConfig producersConfig, string modUniqueId = null)
        {
            AddProducersConfig(new List<ProducerConfig>(){ producersConfig }, modUniqueId);
        }

        /// <summary>
        /// Adds or replace the producers config.
        /// </summary>
        /// <param name="producersConfig">A list of producer config to add or replace.</param>
        /// <param name="modUniqueId">The mod unique id.</param>
        public static void AddProducersConfig(List<ProducerConfig> producersConfig, string modUniqueId =  null)
        {
            producersConfig.ForEach(producerConfig =>
            {
                producerConfig.ModUniqueID = modUniqueId;
                if (!FindAndValidateProducer(producerConfig))
                {
                    //Do nothing, already logged.
                }
                else
                {
                    if (producerConfig?.LightSource is LightSourceConfig lightSource)
                    {
                        producerConfig.LightSource.Color = new Color(lightSource.ColorRed, lightSource.ColorGreen, lightSource.ColorBlue, lightSource.ColorAlpha);
                    }

                    if (producerConfig.ProducingAnimation != null)
                    {
                        foreach (var animation in producerConfig.ProducingAnimation.AdditionalAnimations)
                        {
                            var outputIndex = FindObjectKey(animation.Key);
                            if (outputIndex == null)
                            {
                                ProducerFrameworkModEntry.ModMonitor.Log($"{WarningNotBugPrefix}No object found for '{animation.Key}', producer '{producerConfig.ProducerIdentification}'. This animation will be ignored.", LogLevel.Debug);
                                break;
                            }
                            producerConfig.ProducingAnimation.AdditionalAnimationsId[outputIndex] = animation.Value;
                        }
                    }

                    if (producerConfig.ReadyAnimation != null)
                    {
                        foreach (var animation in producerConfig.ReadyAnimation.AdditionalAnimations)
                        {
                            var outputIndex = FindObjectKey(animation.Key);
                            if (outputIndex == null)
                            {
                                ProducerFrameworkModEntry.ModMonitor.Log($"{WarningNotBugPrefix}No object found for '{animation.Key}', producer '{producerConfig.ProducerIdentification}'. This animation will be ignored.", LogLevel.Debug);
                                break;
                            }
                            producerConfig.ReadyAnimation.AdditionalAnimationsId[outputIndex] = animation.Value;
                        }
                    }
                    AddConfigToRepository(producerConfig);

                    foreach (var n in producerConfig.AdditionalProducerNames)
                    {
                        ProducerConfig newProducerConfig = null;
                        try
                        {
                            newProducerConfig = producerConfig.DeepClone();
                            newProducerConfig.ProducerName = n;
                            newProducerConfig.ProducerQualifiedItemId = null;
                            newProducerConfig.AdditionalProducerNames.Clear();
                            newProducerConfig.AdditionalProducerQualifiedItemId.Clear();
                            if (FindAndValidateProducer(newProducerConfig))
                            {
                                AddConfigToRepository(newProducerConfig);
                            }
                        }
                        catch (Exception e)
                        {
                            ProducerFrameworkModEntry.ModMonitor.Log($"Unexpected error for config from '{newProducerConfig?.ModUniqueID}', producer '{newProducerConfig?.ProducerIdentification}'. This config will be ignored.", LogLevel.Error);
                            ProducerFrameworkModEntry.ModMonitor.Log(e.Message + "\n" + e.StackTrace);
                        }
                    }
                    producerConfig.AdditionalProducerNames.Clear();
                    foreach (var n in producerConfig.AdditionalProducerQualifiedItemId)
                    {
                        ProducerConfig newProducerConfig = null;

                        try
                        {
                            newProducerConfig = producerConfig.DeepClone();
                            newProducerConfig.ProducerQualifiedItemId = n;
                            newProducerConfig.ProducerName = null;
                            newProducerConfig.AdditionalProducerNames.Clear();
                            newProducerConfig.AdditionalProducerQualifiedItemId.Clear();
                            if (FindAndValidateProducer(newProducerConfig))
                            {
                                AddConfigToRepository(newProducerConfig);
                            }
                        }
                        catch (Exception e)
                        {
                            ProducerFrameworkModEntry.ModMonitor.Log($"Unexpected error for config from '{newProducerConfig?.ModUniqueID}', producer '{newProducerConfig?.ProducerIdentification}'. This config will be ignored.", LogLevel.Error);
                            ProducerFrameworkModEntry.ModMonitor.Log(e.Message + "\n" + e.StackTrace);
                        }
                    }
                    producerConfig.AdditionalProducerQualifiedItemId.Clear();
                }
            });
        }

        private static bool FindAndValidateProducer(ProducerData producer)
        {
            if (producer.ProducerQualifiedItemId == null)
            {
                if (String.IsNullOrEmpty(producer.ProducerName))
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"The ProducerQualifiedItemId and ProducerName property can't both be null or empty. This rule will be ignored.", LogLevel.Warn);
                    return false;
                }
                producer.ProducerQualifiedItemId = StardewValley.DataLoader.BigCraftables(Game1.content)
                    .Where(bc => bc.Value.Name.Equals(producer.ProducerName))
                    .Select(bc => ItemRegistry.type_bigCraftable + bc.Key)
                    .FirstOrDefault();
                if (producer.ProducerQualifiedItemId == null)
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"{WarningNotBugPrefix}No producer found for ProducerName {producer.ProducerName}. This rule will be ignored.", LogLevel.Warn);
                    return false;
                }

            }
            if (UnsupportedMachines.Contains(producer.ProducerQualifiedItemId))
            {
                if (producer.ProducerQualifiedItemId == "(BC)21")
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support Crystalariums. Use Custom Cristalarium Mod instead.",
                        LogLevel.Warn);
                }
                else if (producer.ProducerName == "(BC)163")
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support Casks. Use Custom Cask Mod instead.", LogLevel.Warn);
                }
                else
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support {producer.ProducerName ?? producer.ProducerQualifiedItemId}. This rule will be ignored.", LogLevel.Warn);
                }
                return false;
            }
            return true;
        }

        private static void AddConfigToRepository(ProducerConfig producerConfig)
        {
            if (ConfigRepository.TryGetValue(producerConfig.ProducerQualifiedItemId, out var oldConfig))
            {
                if (oldConfig.ModUniqueID != null && oldConfig.ModUniqueID != producerConfig.ModUniqueID)
                {
                    if (oldConfig.OverrideMod.Contains(producerConfig.ModUniqueID) &&
                        producerConfig.OverrideMod.Contains(oldConfig.ModUniqueID))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Both mod '{oldConfig.ModUniqueID}' and '{producerConfig.ModUniqueID}' are saying they should override the config for producer '{producerConfig.ProducerIdentification}'. You should report the problem to these mod's authors. Config from mod '{oldConfig.ModUniqueID}' will be used.",
                            LogLevel.Warn);
                        return;
                    }
                    else if (producerConfig.OverrideMod.Contains(oldConfig.ModUniqueID))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Mod '{producerConfig.ModUniqueID}' is overriding mod '{oldConfig.ModUniqueID}' config for producer '{producerConfig.ProducerIdentification}'.",
                            LogLevel.Debug);
                    }
                    else
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Mod '{producerConfig.ModUniqueID}' can't override mod '{oldConfig.ModUniqueID}' config for producer '{producerConfig.ProducerIdentification}'. This rule will be ignored.",
                            LogLevel.Debug);
                        return;
                    }
                }
            }
            ConfigRepository[producerConfig.ProducerQualifiedItemId] = producerConfig;
        }

        public static string FindObjectKey(string identifier, bool nullOnNegative = false)
        {
            string itemId = null;
            if (identifier == "Stone")
            {
                itemId = "(O)390";
            }
            else if (ItemRegistry.Exists(identifier))
            {
                itemId = ItemRegistry.QualifyItemId(identifier);
            }
            else if (!Int32.TryParse(identifier, out int index))
            {
                var pair = Game1.objectData.FirstOrDefault(o => identifier.Equals(o.Value.Name));
                if (pair.Value != null)
                {
                    itemId = ItemRegistry.type_object + pair.Key;
                }
            }
            else
            {
                if (index < 0)
                    itemId = nullOnNegative ? null : index.ToString();
                else
                    itemId = ItemRegistry.type_object + index;
            }

            return itemId;
        }

        /// <summary>
        /// Get a producer rule for a given producer name and input.
        /// </summary>
        /// <param name="producerQualifiedItemId">Qualified Item Id of the producer</param>
        /// <param name="input">Input object of a rule</param>
        /// <returns>The producer config</returns>
        public static ProducerRule GetProducerItem(string producerQualifiedItemId, Object input)
        {
            ProducerRule value;
            if (input == null)
            {
                RulesRepository.TryGetValue(new Tuple<string, object>(producerQualifiedItemId, null), out value);
            }
            else
            {
                RulesRepository.TryGetValue(new Tuple<string, object>(producerQualifiedItemId, input.QualifiedItemId), out value);
                if (value == null)
                {
                    foreach (string tag in input.GetContextTags())
                    {
                        if (RulesRepository.TryGetValue(new Tuple<string, object>(producerQualifiedItemId, tag), out value))
                        {
                            break;
                        }
                    }
                }
                if (value == null)
                {
                    RulesRepository.TryGetValue(new Tuple<string, object>(producerQualifiedItemId, input.Category.ToString()), out value);
                }
            }
            return value;
        }

        /// <summary>
        /// Check if a producer rule for a given producer exist.
        /// </summary>
        /// <param name="producerQualifiedItemId">The Qualified Item Id of the producer</param>
        /// <returns>true if there is a rule for the producer</returns>
        public static bool HasProducerRule(string producerQualifiedItemId)
        {
            return RulesRepository.Keys.Any(k => k.Item1 == producerQualifiedItemId);
        }

        /// <summary>
        /// Check if a producer rule with input for a given producer exist.
        /// </summary>
        /// <param name="producerQualifiedItemId">The Qualified Item Id of the producer</param>
        /// <returns>true if there is a rule with input for the producer</returns>
        public static bool HasProducerRuleWithInput(string producerQualifiedItemId)
        {
            return RulesRepository.Keys.Any(k => k.Item1 == producerQualifiedItemId && k.Item2 != null);
        }

        /// <summary>
        /// Check if a producer config for a given producer exist.
        /// </summary>
        /// <param name="producerQualifiedItemId">The Qualified Item Id of the producer</param>
        /// <returns>true if there is a config for the producer</returns>
        public static bool HasProducerConfig(string producerQualifiedItemId)
        {
            return ConfigRepository.Keys.Any(k => k == producerQualifiedItemId);
        }

        /// <summary>
        /// Get the producer config from the name of the producer.
        /// </summary>
        /// <param name="producerQualifiedItemId">The producer Qualified Item Id</param>
        /// <returns>The producer config</returns>
        public static ProducerConfig GetProducerConfig(string producerQualifiedItemId)
        {
            ConfigRepository.TryGetValue(producerQualifiedItemId, out ProducerConfig producerConfig);
            return producerConfig;
        }

        public static List<ProducerRule> GetProducerRules(string producerQualifiedItemId)
        {
            return RulesRepository
                .Where(e => e.Key.Item1 == producerQualifiedItemId)
                .Select(e=>e.Value)
                .ToList();
        }

        public static List<ProducerRule> GetProducerRules()
        {
            return RulesRepository.Values.ToList();
        }
    }
}
