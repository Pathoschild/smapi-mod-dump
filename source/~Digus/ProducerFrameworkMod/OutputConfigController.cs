
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using ProducerFrameworkMod.ContentPack;
using StardewValley;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod
{
    public class OutputConfigController
    {
        /// <summary>
        /// Choose and an output config from a list of output configs.
        /// </summary>
        /// <param name="producerRuleOutputConfig">The list of outputConfigs to choose from</param>
        /// <param name="random">The random object that should be used if necessary</param>
        /// <returns>The chosen output config</returns>
        public static OutputConfig ChooseOutput(List<OutputConfig> producerRuleOutputConfig, Random random)
        {
            List<OutputConfig> outputConfigs = producerRuleOutputConfig.FindAll(o => o.OutputProbability > 0);
            Double chance = random.NextDouble();
            Double probabilies = 0;
            foreach (OutputConfig outputConfig in outputConfigs)
            {
                probabilies += outputConfig.OutputProbability;
                if (chance - probabilies < 0)
                {
                    return outputConfig;
                }
            }
            outputConfigs = producerRuleOutputConfig.FindAll(o => o.OutputProbability <= 0);
            double increment = (1 - probabilies) / outputConfigs.Count;
            foreach (OutputConfig outputConfig in outputConfigs)
            {
                probabilies += increment;
                if (chance - probabilies < 0)
                {
                    return outputConfig;
                }
            }
            return producerRuleOutputConfig.FirstOrDefault();
        }

        /// <summary>
        /// Create an output from a given output config.
        /// The name is not property calculetad on the criation, should call LoadOutputName for that.
        /// </summary>
        /// <param name="outputConfig">The output config</param>
        /// <param name="input">The input used</param>
        /// <param name="random">The random object that should be used if necessary</param>
        /// <returns>The created output</returns>
        public static Object CreateOutput(OutputConfig outputConfig, Object input, Random random)
        {
            Object output = outputConfig.OutputIndex != 93 && outputConfig.OutputIndex != 94 ? //Torches indexs
                new Object(Vector2.Zero, outputConfig.OutputIndex, (string)null, false, true, false, false) :
                new Torch(Vector2.Zero, outputConfig.OutputStack, outputConfig.OutputIndex);

            if (outputConfig.InputPriceBased)
            {
                output.Price = (int)(outputConfig.OutputPriceIncrement + input.Price * outputConfig.OutputPriceMultiplier);
            }

            output.Quality = outputConfig.KeepInputQuality ? input.Quality : outputConfig.OutputQuality;

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
            if (input.Quality == 4 && chance < outputConfig.IridiumQualityInput.Probability)
            {
                stackConfig = outputConfig.IridiumQualityInput;
            }
            else if (input.Quality == 2 && chance < outputConfig.GoldQualityInput.Probability)
            {
                stackConfig = outputConfig.GoldQualityInput;
            }
            else if (input.Quality == 1 && chance < outputConfig.SilverQualityInput.Probability)
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
                outputName = ObjectUtils.GetPreserveName(outputConfig.PreserveType.Value, input.Name);
                output.preserve.Value = outputConfig.PreserveType;
                inputUsed = true;
            }
            else if (outputConfig.OutputName != null)
            {
                outputName = outputConfig.OutputName
                    .Replace("{inputName}", input.Name)
                    .Replace("{outputName}", output.Name)
                    .Replace("{farmerName}", who.Name)
                    .Replace("{farmName}", who.farmName.Value);
                inputUsed = outputConfig.OutputName.Contains("{inputName}");
            }

            if (outputName != null)
            {
                output.Name = outputName;
            }

            if (inputUsed)
            {
                output.preservedParentSheetIndex.Value = input.parentSheetIndex;
            }

            //Called just to load the display name.
            string loadingDisplayName = output.DisplayName;
        }

    }
}
