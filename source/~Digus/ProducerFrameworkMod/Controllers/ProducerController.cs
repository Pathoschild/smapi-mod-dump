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
using System.Linq;
using Force.DeepCloner;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod.Controllers
{
    public class ProducerController
    {
        public static readonly List<string> UnsupportedMachines = new List<string>()
            { "Crystalarium", "Cask", "Incubator", "Slime Incubator", "Hopper", "Chest", "Garden Pot", "Mini-Jukebox"
                , "Workbench", "Tapper", "Singing Stone", "Drum Block", "Flute Block", "Statue Of Endless Fortune"
                , "Slime Ball", "Staircase", "Junimo Kart Arcade System", "Prairie King Arcade System" };

        private static readonly Dictionary<Tuple<string, object>, ProducerRule> RulesRepository = new Dictionary<Tuple<string, object>, ProducerRule>();
        private static readonly Dictionary<string, ProducerConfig> ConfigRepository = new Dictionary<string, ProducerConfig>()
        {
            {
                "Furnace", new ProducerConfig("Furnace",true)
            },
            {
                "Loom", new ProducerConfig("Loom",false,true)
            },
            {
                "Charcoal Kiln", new ProducerConfig("Charcoal Kiln",true)
            },
            {
                "Keg", new ProducerConfig("Keg", new Dictionary<StardewStats, string>(){{StardewStats.BeveragesMade,null}})
            },
            {
                "Preserves Jar", new ProducerConfig("Preserves Jar", new Dictionary<StardewStats, string>(){{StardewStats.PreservesMade,null}})
            },
            {
                "Cheese Press", new ProducerConfig("Cheese Press", new Dictionary<StardewStats, string>(){{StardewStats.GoatCheeseMade, "426" },{StardewStats.CheeseMade, null } })
            }
        };

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
            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
            foreach (var producerRule in producerRules)
            {
                try {
                    producerRule.ModUniqueID = modUniqueId;
                    if (!ValidateRuleProducerName(producerRule.ProducerName))
                    {
                        //Do nothing, already logged.
                    }
                    else if (string.IsNullOrEmpty(producerRule.InputIdentifier) && (GetProducerConfig(producerRule.ProducerName)?.NoInputStartMode == null))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"The InputIdentifier property can't be null or empty if there is no config for 'NoInputStartMode' for producer '{producerRule.ProducerName}'. This rule will be ignored.", LogLevel.Warn);
                    }
                    else if ((!string.IsNullOrEmpty(producerRule.InputIdentifier) && GetProducerConfig(producerRule.ProducerName)?.NoInputStartMode != null))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"The InputIdentifier property can't have a value if there is a config for 'NoInputStartMode' for producer '{producerRule.ProducerName}'. This rule will be ignored.", LogLevel.Warn);
                    } 
                    else
                    {
                        if (!string.IsNullOrEmpty(producerRule.InputIdentifier))
                        {
                            if (producerRule.InputIdentifier == "Stone")
                            {
                                producerRule.InputKey = 390;
                            }
                            else if (!Int32.TryParse(producerRule.InputIdentifier, out var intInputIdentifier))
                            {
                                KeyValuePair<int, string> pair = objects.FirstOrDefault(o => ObjectUtils.IsObjectStringFromObjectName(o.Value, producerRule.InputIdentifier));
                                if (pair.Value != null)
                                {
                                    producerRule.InputKey = pair.Key;
                                }
                                else
                                {
                                    producerRule.InputKey = producerRule.InputIdentifier;
                                }
                            }
                            else
                            {
                                producerRule.InputKey = intInputIdentifier;
                            }
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
                            if (!Int32.TryParse(outputConfig.OutputIdentifier, out int outputIndex))
                            {
                                KeyValuePair<int, string> pair = objects.FirstOrDefault(o => ObjectUtils.IsObjectStringFromObjectName(o.Value, outputConfig.OutputIdentifier));
                                if (pair.Value != null)
                                {
                                    outputIndex = pair.Key;
                                }
                                else
                                {
                                    ProducerFrameworkModEntry.ModMonitor.Log($"No Output found for '{outputConfig.OutputIdentifier}', producer '{producerRule.ProducerName}' and input '{producerRule.InputIdentifier}'. This rule will be ignored.", producerRule.WarningsLogLevel);
                                    break;
                                }
                            }
                            outputConfig.OutputIndex = outputIndex;


                            foreach (var fuel in outputConfig.RequiredFuel)
                            {
                                if (!Int32.TryParse(fuel.Key, out int fuelIndex))
                                {
                                    KeyValuePair<int, string> pair = objects.FirstOrDefault(o =>
                                        ObjectUtils.IsObjectStringFromObjectName(o.Value, fuel.Key));
                                    if (pair.Value != null)
                                    {
                                        fuelIndex = pair.Key;
                                    }
                                    else
                                    {
                                        ProducerFrameworkModEntry.ModMonitor.Log(
                                            $"No required fuel found for '{fuel.Key}', producer '{producerRule.ProducerName}' and input '{fuel.Key}'. This rule will be ignored.",
                                            producerRule.WarningsLogLevel);
                                        //This is done to abort the rule.
                                        outputConfig.OutputIndex = -1;
                                        break;
                                    }
                                }
                                outputConfig.FuelList.Add(new Tuple<int, int>(fuelIndex, fuel.Value));
                            }
                        }
                        if (producerRule.OutputConfigs.Any(p => p.OutputIndex < 0))
                        {
                            continue;
                        }

                        if (producerRule.FuelIdentifier != null)
                        {
                            producerRule.AdditionalFuel[producerRule.FuelIdentifier] = producerRule.FuelStack;
                        }
                        foreach (var fuel in producerRule.AdditionalFuel)
                        {
                            if (!Int32.TryParse(fuel.Key, out int fuelIndex))
                            {
                                KeyValuePair<int, string> pair = objects.FirstOrDefault(o => ObjectUtils.IsObjectStringFromObjectName(o.Value, fuel.Key));
                                if (pair.Value != null)
                                {
                                    fuelIndex = pair.Key;
                                }
                                else
                                {
                                    ProducerFrameworkModEntry.ModMonitor.Log($"No fuel found for '{fuel.Key}', producer '{producerRule.ProducerName}' and input '{producerRule.InputIdentifier}'. This rule will be ignored.", producerRule.WarningsLogLevel);
                                    break;
                                }
                            }
                            producerRule.FuelList.Add(new Tuple<int, int>(fuelIndex, fuel.Value));
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
                    ProducerFrameworkModEntry.ModMonitor.Log($"Unexpected error for rule from '{producerRule?.ModUniqueID}', producer '{producerRule?.ProducerName}' and input '{producerRule?.InputIdentifier}'. This rule will be ignored.", LogLevel.Error);
                    ProducerFrameworkModEntry.ModMonitor.Log(e.Message + "\n" + e.StackTrace);
                }
                foreach (var n in producerRule.AdditionalProducerNames)
                {
                    ProducerRule newProducerRule = null;
                    try {
                        newProducerRule = producerRule.DeepClone();
                        newProducerRule.ProducerName = n;
                        newProducerRule.AdditionalProducerNames.Clear();
                        if (ValidateRuleProducerName(newProducerRule.ProducerName))
                        {
                            AddRuleToRepository(newProducerRule);
                        }
                    }
                    catch (Exception e)
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"Unexpected error for rule from '{newProducerRule?.ModUniqueID}', producer '{newProducerRule?.ProducerName}' and input '{newProducerRule?.InputIdentifier}'. This rule will be ignored.", LogLevel.Error);
                        ProducerFrameworkModEntry.ModMonitor.Log(e.Message + "\n" + e.StackTrace);
                    }
                }   
                producerRule.AdditionalProducerNames.Clear();
            }
        }

        private static bool ValidateRuleProducerName(string producerName)
        {
            if (String.IsNullOrEmpty(producerName))
            {
                ProducerFrameworkModEntry.ModMonitor.Log($"The ProducerName property can't be null or empty. This rule will be ignored.", LogLevel.Warn);
                return false;
            }
            else if (UnsupportedMachines.Contains(producerName) || producerName.Contains("arecrow"))
            {
                if (producerName == "Crystalarium")
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support Crystalariums. Use Custom Cristalarium Mod instead.",
                        LogLevel.Warn);
                }
                else if (producerName == "Cask")
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support Casks. Use Custom Cask Mod instead.", LogLevel.Warn);
                }
                else
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support {producerName}. This rule will be ignored.", LogLevel.Warn);
                }
                return false;
            }
            return true;
        }

        private static void AddRuleToRepository(ProducerRule producerRule)
        {
            Tuple<string, object> ruleKey = new Tuple<string, object>(producerRule.ProducerName, producerRule.InputKey);
            if (RulesRepository.ContainsKey(ruleKey))
            {
                ProducerRule oldRule = RulesRepository[ruleKey];
                if (oldRule.ModUniqueID != producerRule.ModUniqueID)
                {
                    if (oldRule.OverrideMod.Contains(producerRule.ModUniqueID) &&
                        producerRule.OverrideMod.Contains(oldRule.ModUniqueID))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Both mod '{oldRule.ModUniqueID}' and '{producerRule.ModUniqueID}' are saying they should override the rule for producer '{producerRule.ProducerName}' and input '{producerRule.InputIdentifier}'. You should report the problem to these mod's authors. Rule from mod '{oldRule.ModUniqueID}' will be used.",
                            LogLevel.Warn);
                        return;
                    }
                    else if (producerRule.OverrideMod.Contains(oldRule.ModUniqueID))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Mod '{producerRule.ModUniqueID}' is overriding mod '{oldRule.ModUniqueID}' rule for producer '{producerRule.ProducerName}' and input '{producerRule.InputIdentifier}'.",
                            LogLevel.Debug);
                    }
                    else
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Mod '{producerRule.ModUniqueID}' can't override mod '{oldRule.ModUniqueID}' rule for producer '{producerRule.ProducerName}' and input '{producerRule.InputIdentifier}'. This rule will be ignored.",
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
            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
            producersConfig.ForEach(producerConfig =>
            {
                producerConfig.ModUniqueID = modUniqueId;
                if (!ValidateConfigProducerName(producerConfig.ProducerName))
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
                            if (!Int32.TryParse(animation.Key, out int outputIndex))
                            {
                                KeyValuePair<int, string> pair = objects.FirstOrDefault(o => ObjectUtils.IsObjectStringFromObjectName(o.Value, animation.Key));
                                if (pair.Value != null)
                                {
                                    outputIndex = pair.Key;
                                }
                                else
                                {
                                    ProducerFrameworkModEntry.ModMonitor.Log($"No object found for '{animation.Key}', producer '{producerConfig.ProducerName}'. This animation will be ignored.", LogLevel.Debug);
                                    break;
                                }
                            }
                            producerConfig.ProducingAnimation.AdditionalAnimationsId[outputIndex] = animation.Value;
                        }
                    }

                    if (producerConfig.ReadyAnimation != null)
                    {
                        foreach (var animation in producerConfig.ReadyAnimation.AdditionalAnimations)
                        {
                            if (!Int32.TryParse(animation.Key, out int outputIndex))
                            {
                                KeyValuePair<int, string> pair = objects.FirstOrDefault(o => ObjectUtils.IsObjectStringFromObjectName(o.Value, animation.Key));
                                if (pair.Value != null)
                                {
                                    outputIndex = pair.Key;
                                }
                                else
                                {
                                    ProducerFrameworkModEntry.ModMonitor.Log($"No object found for '{animation.Key}', producer '{producerConfig.ProducerName}'. This animation will be ignored.", LogLevel.Debug);
                                    break;
                                }
                            }
                            producerConfig.ReadyAnimation.AdditionalAnimationsId[outputIndex] = animation.Value;
                        }
                    }
                    AddConfigToRepository(producerConfig);

                    foreach (var n in producerConfig.AdditionalProducerNames)
                    {
                        ProducerConfig newProducerConfig = null;
                        
                        newProducerConfig = producerConfig.DeepClone();
                        newProducerConfig.ProducerName = n;
                        newProducerConfig.AdditionalProducerNames.Clear();
                        if (ValidateConfigProducerName(newProducerConfig.ProducerName))
                        {
                            AddConfigToRepository(newProducerConfig);
                        }
                    }
                    producerConfig.AdditionalProducerNames.Clear();
                }
            });
        }

        private static bool ValidateConfigProducerName(string producerName)
        {
            if (String.IsNullOrEmpty(producerName))
            {
                ProducerFrameworkModEntry.ModMonitor.Log($"The ProducerName property can't be null or empty. This rule will be ignored.", LogLevel.Warn);
                return false;
            }
            else if (UnsupportedMachines.Contains(producerName) || producerName.Contains("arecrow"))
            {
                if (producerName == "Crystalarium")
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support Crystalariums. Use Custom Cristalarium Mod instead.", LogLevel.Warn);
                }
                else if (producerName == "Cask")
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support Casks. Use Custom Cask Mod instead.", LogLevel.Warn);
                }
                else
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support {producerName}. This config will be ignored.", LogLevel.Warn);
                }

                return false;
            }
            return true;
        }

        private static void AddConfigToRepository(ProducerConfig producerConfig)
        {
            if (ConfigRepository.ContainsKey(producerConfig.ProducerName))
            {
                ProducerConfig oldConfig = ConfigRepository[producerConfig.ProducerName];
                if (oldConfig.ModUniqueID != null && oldConfig.ModUniqueID != producerConfig.ModUniqueID)
                {
                    if (oldConfig.OverrideMod.Contains(producerConfig.ModUniqueID) &&
                        producerConfig.OverrideMod.Contains(oldConfig.ModUniqueID))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Both mod '{oldConfig.ModUniqueID}' and '{producerConfig.ModUniqueID}' are saying they should override the config for producer '{producerConfig.ProducerName}'. You should report the problem to these mod's authors. Config from mod '{oldConfig.ModUniqueID}' will be used.",
                            LogLevel.Warn);
                        return;
                    }
                    else if (producerConfig.OverrideMod.Contains(oldConfig.ModUniqueID))
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Mod '{producerConfig.ModUniqueID}' is overriding mod '{oldConfig.ModUniqueID}' config for producer '{producerConfig.ProducerName}'.",
                            LogLevel.Debug);
                    }
                    else
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log(
                            $"Mod '{producerConfig.ModUniqueID}' can't override mod '{oldConfig.ModUniqueID}' config for producer '{producerConfig.ProducerName}'. This rule will be ignored.",
                            LogLevel.Debug);
                        return;
                    }
                }
            }
            ConfigRepository[producerConfig.ProducerName] = producerConfig;
        }

        /// <summary>
        /// Get a producer rule for a given producer name and input.
        /// </summary>
        /// <param name="producerName">Name of the producer</param>
        /// <param name="input">Input object of a rule</param>
        /// <returns>The producer config</returns>
        public static ProducerRule GetProducerItem(string producerName, Object input)
        {
            ProducerRule value;
            if (input == null)
            {
                RulesRepository.TryGetValue(new Tuple<string, object>(producerName, null), out value);
            }
            else
            {
                RulesRepository.TryGetValue(new Tuple<string, object>(producerName, input.ParentSheetIndex), out value);
                if (value == null)
                {
                    foreach (string tag in input.GetContextTagList())
                    {
                        if (RulesRepository.TryGetValue(new Tuple<string, object>(producerName, tag), out value))
                        {
                            break;
                        }
                    }
                }
                if (value == null)
                {
                    RulesRepository.TryGetValue(new Tuple<string, object>(producerName, input.Category), out value);
                }
            }
            return value;
        }

        /// <summary>
        /// Check if a producer rule for a given producer exist.
        /// </summary>
        /// <param name="producerName">The name of the producer</param>
        /// <returns>true if there is a rule for the producer</returns>
        public static bool HasProducerRule(string producerName)
        {
            return RulesRepository.Keys.Any(k => k.Item1 == producerName);
        }

        /// <summary>
        /// Check if a producer rule with input for a given producer exist.
        /// </summary>
        /// <param name="producerName">The name of the producer</param>
        /// <returns>true if there is a rule with input for the producer</returns>
        public static bool HasProducerRuleWithInput(string producerName)
        {
            return RulesRepository.Keys.Any(k => k.Item1 == producerName && k.Item2 != null);
        }

        /// <summary>
        /// Get the producer config from the name of the producer.
        /// </summary>
        /// <param name="producerName">The producer name</param>
        /// <returns>The producer config</returns>
        public static ProducerConfig GetProducerConfig(string producerName)
        {
            ConfigRepository.TryGetValue(producerName, out ProducerConfig producerConfig);
            return producerConfig;
        }

        public static List<ProducerRule> GetProducerRules(string producerName)
        {
            return RulesRepository
                .Where(e => e.Key.Item1 == producerName)
                .Select(e=>e.Value)
                .ToList();
        }

        public static List<ProducerRule> GetProducerRules()
        {
            return RulesRepository.Values.ToList();
        }
    }
}
