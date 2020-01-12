using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod
{
    public class ProducerController
    {
        public static readonly List<string> UnsupportedMachines = new List<string>()
            { "Crystalarium", "Cask", "Incubator", "Slime Incubator", "Hopper", "Chest", "Garden Pot", "Mini-Jukebox"
                , "Workbench", "Bee House", "Worm Bin", "Tapper", "Singing Stone", "Drum Block", "Flute Block"
                , "Slime Ball", "Staircase", "Junimo Kart Arcade System", "Prairie King Arcade System" };

        private static readonly Dictionary<Tuple<string, object>, ProducerRule> RulesRepository = new Dictionary<Tuple<string, object>, ProducerRule>();
        private static readonly Dictionary<string, ProducerConfig> ConfigRepository = new Dictionary<string, ProducerConfig>()
        {
            {
                "Bee House", new ProducerConfig("Bee House",false,true)
            },
            {
                "Furnace", new ProducerConfig("Furnace",true)
            },
            {
                "Loom", new ProducerConfig("Loom",false,true)
            },
            {
                "Charcoal Kiln", new ProducerConfig("Charcoal Kiln",true)
            }
        };

        /// <summary>
        /// Adds or replace a custom producer rule to the game.
        /// You should probably call this method everytime a save game loads, to ensure all custom objects are properly loaded.
        /// Producer rule should be unique per name of producer and input identifier.
        /// </summary>
        /// <param name="producerRule">The producer rule to be added or replaced.</param>
        /// <param name="i18n">Optional i18n object, to look for the output name in case a translation key was used.</param>
        public static void AddProducerItems(ProducerRule producerRule, ITranslationHelper i18n = null)
        {
            AddProducerItems(new List<ProducerRule>() { producerRule }, i18n);
        }

        /// <summary>
        /// Adds or replace a list of custom producer rules to the game.
        /// You should probably call this method everytime a save game loads, to ensure all custom objects are properly loaded.
        /// Producer rules should be unique per name of producer and input identifier.
        /// </summary>
        /// <param name="producerRules">A list of producer rules to be added or replaced.</param>
        /// <param name="i18n">Optional i18n object, to look for the output name in case a translation key was used.</param>
        public static void AddProducerItems(List<ProducerRule> producerRules, ITranslationHelper i18n = null)
        {
            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation", ContentSource.GameContent);
            foreach (var producerRule in producerRules)
            {
                if (string.IsNullOrEmpty(producerRule.ProducerName))
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"The ProducerName property can't be null or empty. This rule will be ignored.", LogLevel.Warn);
                    continue;
                }
                else if (UnsupportedMachines.Contains(producerRule.ProducerName) || producerRule.ProducerName.Contains("arecrow"))
                {
                    if (producerRule.ProducerName == "Crystalarium")
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support Crystalariums. Use Custom Cristalarium Mod instead.", LogLevel.Warn);
                    }
                    else if (producerRule.ProducerName == "Cask")
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support Casks. Use Custom Cask Mod instead.", LogLevel.Warn);
                    }
                    else
                    {
                        ProducerFrameworkModEntry.ModMonitor.Log($"Producer Framework Mod doesn't support {producerRule.ProducerName}. This rule will be ignored.", LogLevel.Warn);
                    }
                }
                else if (string.IsNullOrEmpty(producerRule.InputIdentifier))
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"The InputIdentifier property can't be null or empty. This rule for '{producerRule.ProducerName}' will be ignored.", LogLevel.Warn);
                    continue;
                }
                else
                {
                    object inputIdentifier;
                    if (!int.TryParse(producerRule.InputIdentifier, out var intInputIdentifier))
                    {
                        KeyValuePair<int, string> pair = objects.FirstOrDefault(o => ObjectUtils.IsObjectStringFromObjectName(o.Value, producerRule.InputIdentifier));
                        if (pair.Value != null)
                        {
                            inputIdentifier = pair.Key;
                        }
                        else
                        {
                            inputIdentifier = producerRule.InputIdentifier;
                        }
                    }
                    else
                    {
                        inputIdentifier = intInputIdentifier;
                    }

                    if (producerRule.OutputIdentifier != null)
                    {
                        producerRule.OutputConfigs.Add(new OutputConfig()
                        {
                            OutputIdentifier = producerRule.OutputIdentifier,
                            OutputName = producerRule.OutputName,
                            OutputTranslationKey = producerRule.OutputTranslationKey,
                            PreserveType = producerRule.PreserveType,
                            InputPriceBased = producerRule.InputPriceBased,
                            OutputPriceIncrement = producerRule.OutputPriceIncrement,
                            OutputPriceMultiplier = producerRule.OutputPriceMultiplier,
                            KeepInputQuality = producerRule.KeepInputQuality,
                            OutputQuality = producerRule.OutputQuality,
                            OutputStack = producerRule.OutputStack,
                            OutputMaxStack = producerRule.OutputMaxStack,
                            SilverQualityInput = producerRule.SilverQualityInput,
                            GoldQualityInput = producerRule.GoldQualityInput,
                            IridiumQualityInput = producerRule.IridiumQualityInput
                        });
                    }
                    producerRule.OutputConfigs.AddRange(producerRule.AdditionalOutputs);

                    foreach (OutputConfig outputConfig in producerRule.OutputConfigs)
                    {
                        if (!Int32.TryParse(outputConfig.OutputIdentifier, out int outputIndex))
                        {
                            KeyValuePair<int, string> pair = objects.FirstOrDefault(o => ObjectUtils.IsObjectStringFromObjectName(o.Value, outputConfig.OutputIdentifier));
                            if (pair.Value != null)
                            {
                                outputIndex = pair.Key;
                            }
                            else
                            {
                                ProducerFrameworkModEntry.ModMonitor.Log($"No Output found for '{outputConfig.OutputIdentifier}', producer '{producerRule.ProducerName}' and input '{producerRule.InputIdentifier}'. This rule will be ignored.", LogLevel.Warn);
                                break;
                            }
                        }
                        outputConfig.OutputIndex = outputIndex;
                        if (outputConfig.OutputTranslationKey != null && i18n != null)
                        {
                            string translation = i18n.Get(outputConfig.OutputTranslationKey);
                            if (!translation.Contains("(no translation:"))
                            {
                                NameUtils.AddCustomName(outputConfig.OutputIndex, translation);
                            }
                            else
                            {
                                NameUtils.AddCustomName(outputConfig.OutputIndex, outputConfig.OutputName);
                            }
                        }
                        else if(outputConfig.OutputName != null)
                        {
                            NameUtils.AddCustomName(outputConfig.OutputIndex, outputConfig.OutputName);
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
                                ProducerFrameworkModEntry.ModMonitor.Log($"No fuel found for '{fuel.Key}', producer '{producerRule.ProducerName}' and input '{fuel.Key}'. This rule will be ignored.", LogLevel.Warn);
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

                    RulesRepository[new Tuple<string, object>(producerRule.ProducerName, inputIdentifier)] = producerRule;
                }
            }
        }

        /// <summary>
        /// Adds or replace the producer config.
        /// </summary>
        /// <param name="producersConfig">The producer config to be added or replaced.</param>
        public static void AddProducerConfig(ProducerConfig producersConfig)
        {
            AddProducersConfig(new List<ProducerConfig>(){ producersConfig });
        }

        /// <summary>
        /// Adds or replace the producers config.
        /// </summary>
        /// <param name="producersConfig">A list of producer config to add or replace.</param>
        public static void AddProducersConfig(List<ProducerConfig> producersConfig)
        {
            producersConfig.ForEach(c => ConfigRepository[c.ProducerName] = c);
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
        /// Get the producer config from the name of the producer.
        /// </summary>
        /// <param name="producerName">The producer name</param>
        /// <returns>The producer config</returns>
        public static ProducerConfig GetProducerConfig(string producerName)
        {
            ConfigRepository.TryGetValue(producerName, out ProducerConfig producerConfig);
            return producerConfig;
        }
    }
}
