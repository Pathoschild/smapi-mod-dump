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
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StardewModdingAPI;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod.Controllers
{
    public class OutputConfigController
    {
        /// <summary>
        /// Choose and an output config from a list of output configs.
        /// </summary>
        /// <param name="producerRuleOutputConfig">The list of outputConfigs to choose from</param>
        /// <param name="random">The random object that should be used if necessary</param>
        /// <param name="fuelSearch">Function to search the fuel for the production. Receive the fuel id and stack amount. Should return true if there is enough fuel.</param>
        /// <param name="location">The location of the producer.</param>
        /// <param name="input">The input Object</param>
        /// <returns>The chosen output config</returns>
        /// <exception cref="RestrictionException">If there is no output config option, an exception is throw with the restriction message.</exception>
        public static OutputConfig ChooseOutput(List<OutputConfig> producerRuleOutputConfig, Random random, Func<int,int,bool> fuelSearch, GameLocation location, Object input = null)
        {
            List<OutputConfig> filteredOutputConfigs = FilterOutputConfig(producerRuleOutputConfig, o => o.RequiredInputQuality.Count == 0 || o.RequiredInputQuality.Any(q => q == input?.Quality), "Quality");
            filteredOutputConfigs = FilterOutputConfig(filteredOutputConfigs, o => o.FuelList.All(f => fuelSearch(f.Item1, f.Item2)), "Fuel");
            filteredOutputConfigs = FilterOutputConfig(filteredOutputConfigs, o => o.RequiredSeason.Count == 0 || o.RequiredSeason.Any(q => q == location.GetSeasonForLocation()), "Season");
            filteredOutputConfigs = FilterOutputConfig(filteredOutputConfigs, o => o.RequiredWeather.Count == 0 || o.RequiredWeather.Any(q => q == GameUtils.GetCurrentWeather()), "Weather");
            filteredOutputConfigs = FilterOutputConfig(filteredOutputConfigs, o => o.RequiredLocation.Count == 0 || o.RequiredLocation.Any(q => q == location.Name), "Location");
            filteredOutputConfigs = FilterOutputConfig(filteredOutputConfigs, o => o.RequiredMail.Count == 0 || o.RequiredMail.Any(q => Game1.player.mailReceived.Contains(q)), "Mail");
            filteredOutputConfigs = FilterOutputConfig(filteredOutputConfigs, o => o.RequiredEvent.Count == 0 || o.RequiredEvent.Any(q => Game1.player.eventsSeen.Contains(q)), "Event");
            filteredOutputConfigs = FilterOutputConfig(filteredOutputConfigs, o => o.RequiredOutdoors == null || o.RequiredOutdoors == location.IsOutdoors, "Location");
            if (input != null)
            {
                Object parent = null;
                if (input.preservedParentSheetIndex.Value > 0)
                {
                    parent = new Object(input.preservedParentSheetIndex.Value, 1);
                }
                filteredOutputConfigs = FilterOutputConfig(
                    filteredOutputConfigs
                    , o => o.RequiredInputParentIdentifier.Count == 0 
                           || o.RequiredInputParentIdentifier.Any(
                               q => q == input.preservedParentSheetIndex.Value.ToString() 
                                    || q == parent?.Name
                                    || q == parent?.Category.ToString()
                                    || parent?.HasContextTag(q) == true)
                    , "InputParent",input);
            }
            

            List<OutputConfig> outputConfigs = filteredOutputConfigs.FindAll(o => o.OutputProbability > 0);
            Double chance = random.NextDouble();
            Double probabilities = 0;
            foreach (OutputConfig outputConfig in outputConfigs)
            {
                probabilities += outputConfig.OutputProbability;
                if (chance - probabilities < 0)
                {
                    return outputConfig;
                }
            }
            outputConfigs = filteredOutputConfigs.FindAll(o => o.OutputProbability <= 0);
            double increment = (1 - probabilities) / outputConfigs.Count;
            foreach (OutputConfig outputConfig in outputConfigs)
            {
                probabilities += increment;
                if (chance - probabilities < 0)
                {
                    return outputConfig;
                }
            }
            return filteredOutputConfigs.FirstOrDefault();
        }

        private static List<OutputConfig> FilterOutputConfig(List<OutputConfig> outputConfigs, Predicate<OutputConfig> filterPredicate, string messageSuffix, Object inputForName =  null)
        {
            List<OutputConfig> result = outputConfigs.FindAll(filterPredicate);
            if (result.Count == 0)
            {
                Translation translation;
                if (inputForName != null)
                {
                    translation = DataLoader.Helper.Translation.Get($"Message.Requirement.{messageSuffix}", new { objectName = new Object(inputForName.ParentSheetIndex,1).DisplayName});
                }
                else
                {
                    translation = DataLoader.Helper.Translation.Get($"Message.Requirement.{messageSuffix}");
                }
                if (translation.HasValue())
                {
                    throw new RestrictionException(translation);
                }
                else
                {
                    throw new RestrictionException();
                }
            }
            return result;
        }

        /// <summary>
        /// Create an output from a given output config.
        /// The name is not properly calculated on the creation, should call LoadOutputName for that.
        /// </summary>
        /// <param name="outputConfig">The output config</param>
        /// <param name="input">The input used</param>
        /// <param name="random">The random object that should be used if necessary</param>
        /// <returns>The created output</returns>
        public static Object CreateOutput(OutputConfig outputConfig, Object input, Random random)
        {
            Object output;
            if (outputConfig.OutputIndex == 93 || outputConfig.OutputIndex == 94)
            {
                output = new Torch(Vector2.Zero, outputConfig.OutputStack, outputConfig.OutputIndex);
            }
            else if (outputConfig.OutputColorConfig is ColoredObjectConfig coloredObjectConfig)
            {
                switch (coloredObjectConfig.Type)
                {
                    case ColorType.ObjectColor when input is ColoredObject coloredObject:
                        output = new ColoredObject(outputConfig.OutputIndex, outputConfig.OutputStack, coloredObject.color.Value);
                        break;
                    case ColorType.ObjectDyeColor when TailoringMenu.GetDyeColor(input) is Color color:
                        output = new ColoredObject(outputConfig.OutputIndex, outputConfig.OutputStack, color);
                        break;
                    case ColorType.DefinedColor:
                    default:
                        output = new ColoredObject(outputConfig.OutputIndex, outputConfig.OutputStack, new Color(coloredObjectConfig.Red, coloredObjectConfig.Green, coloredObjectConfig.Blue));
                        break;
                }
            }
            else
            {
                output = new Object(Vector2.Zero, outputConfig.OutputIndex, null, false, true, false, false);
            }

            if (outputConfig.InputPriceBased)
            {
                output.Price = (int)(outputConfig.OutputPriceIncrement + (input?.Price??0) * outputConfig.OutputPriceMultiplier);
            }
            else
            {
                output.Price = (int)(outputConfig.OutputPriceIncrement + (output?.Price??0) * outputConfig.OutputPriceMultiplier);
            }

            output.Quality = outputConfig.KeepInputQuality ? input?.Quality??0 : outputConfig.OutputQuality;

            output.Stack = GetOutputStack(outputConfig, input, random);

            return output;
        }

        /// <summary>
        /// Get the output stack for a given output config, input and random instance.
        /// </summary>
        /// <param name="outputConfig">The output config</param>
        /// <param name="input">The input used</param>
        /// <param name="random">The random object that should be used if necessary</param>
        /// <returns>The stack</returns>
        public static int GetOutputStack(OutputConfig outputConfig, Object input, Random random)
        {
            double chance = random.NextDouble();
            StackConfig stackConfig;
            if (input?.Quality == 4 && chance < outputConfig.IridiumQualityInput.Probability)
            {
                stackConfig = outputConfig.IridiumQualityInput;
            }
            else if (input?.Quality == 2 && chance < outputConfig.GoldQualityInput.Probability)
            {
                stackConfig = outputConfig.GoldQualityInput;
            }
            else if (input?.Quality == 1 && chance < outputConfig.SilverQualityInput.Probability)
            {
                stackConfig = outputConfig.SilverQualityInput;
            }
            else
            {
                stackConfig = new StackConfig(outputConfig.OutputStack, outputConfig.OutputMaxStack);
            }
            return random.Next(stackConfig.OutputStack, Math.Max(stackConfig.OutputStack, stackConfig.OutputMaxStack + 1));
        }

        /// <summary>
        /// Create and set the output for a giving output config, output object, input object and a given farmer.
        /// If the farmer is not passed, the default one will be used.
        /// </summary>
        /// <param name="outputConfig">The output config</param>
        /// <param name="output">The output to load the name on</param>
        /// <param name="input">The input used</param>
        /// <param name="who">The farmer to be used</param>
        public static void LoadOutputName(OutputConfig outputConfig, Object output, Object input, Farmer who = null)
        {
            string outputName = null;
            bool inputUsed = false;
            who = who ?? Game1.player;
            if (outputConfig.PreserveType.HasValue)
            {
                outputName = ObjectUtils.GetPreserveName(outputConfig.PreserveType.Value, input?.Name??"");
                output.preserve.Value = outputConfig.PreserveType;
                inputUsed = true;
            }
            else if (output.parentSheetIndex == 340)
            {
                outputName = $"{input?.Name ?? "Wild"} Honey";
                inputUsed = true;
            }
            else if (outputConfig.OutputName != null)
            {
                string inputName = input?.Name ?? outputConfig.OutputGenericParentName ?? "";

                if (outputConfig.KeepInputParentIndex)
                {
                    if (input?.preservedParentSheetIndex.Value == -1)
                    {
                        inputName = outputConfig.OutputGenericParentName;
                    }
                    else if (input?.preservedParentSheetIndex.Value > 0)
                    {
                        inputName = ObjectUtils.GetObjectParameter(Game1.objectInformation[input.preservedParentSheetIndex.Value], (int) ObjectParameter.Name);
                    }
                }
                else if (input?.preservedParentSheetIndex.Value > 0)
                {
                    inputName = ObjectUtils.GetObjectParameter(Game1.objectInformation[input.ParentSheetIndex], (int)ObjectParameter.Name);
                }

                outputName = outputConfig.OutputName
                    .Replace("{inputName}", inputName)
                    .Replace("{outputName}", output.Name)
                    .Replace("{farmerName}", who.Name)
                    .Replace("{farmName}", who.farmName.Value);

                inputUsed = outputConfig.OutputName.Contains("{inputName}") ;
            }

            if (outputName != null)
            {
                Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
                output.Name = regex.Replace(outputName, " ").Trim();
            }

            if (inputUsed)
            {
                output.preservedParentSheetIndex.Value = input == null ? -1 : outputConfig.KeepInputParentIndex && input.preservedParentSheetIndex.Value != 0 ? input.preservedParentSheetIndex.Value : input.ParentSheetIndex;
            }

            //Called just to load the display name.
            string loadingDisplayName = output.DisplayName;
        }

    }
}
