using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod
{
    public class ProducerRuleController
    {
        /// <summary>
        /// Check if an input is excluded by a producer rule.
        /// </summary>
        /// <param name="producerRule">The producer rule to check.</param>
        /// <param name="input">The input to check</param>
        /// <returns>true if should be excluded</returns>
        public static bool IsInputExcluded(ProducerRule producerRule, Object input)
        {
            return producerRule.ExcludeIdentifiers != null && (producerRule.ExcludeIdentifiers.Contains(input.ParentSheetIndex.ToString())
                                                               || producerRule.ExcludeIdentifiers.Contains(input.Name)
                                                               || producerRule.ExcludeIdentifiers.Contains(input.Category.ToString())
                                                               || producerRule.ExcludeIdentifiers.Intersect(input.GetContextTags()).Any());
        }

        /// <summary>
        /// Check if an input has the required stack for the producer rule.
        /// </summary>
        /// <param name="producerRule">the producer rule to check</param>
        /// <param name="input">The input to check</param>
        public static void ValidateIfInputStackLessThanRequired(ProducerRule producerRule, Object input)
        {
            int requiredStack = producerRule.InputStack;
            if (input.Stack < requiredStack)
            {
                throw new RestrictionException(DataLoader.Helper.Translation.Get(
                    "Message.Requirement.Amount"
                    , new { amount = requiredStack, objectName = Lexicon.makePlural(input.DisplayName, requiredStack == 1) }
                ));
            }
        }

        /// <summary>
        /// Check if a farmer has the required fules and stack for a given producer rule.
        /// </summary>
        /// <param name="producerRule">the producer tule to check</param>
        /// <param name="who">The farmer to check</param>
        public static void ValidateIfAnyFuelStackLessThanRequired(ProducerRule producerRule, Farmer who)
        {
            foreach (Tuple<int, int> fuel in producerRule.FuelList)
            {
                if (!who.hasItemInInventory(fuel.Item1, fuel.Item2))
                {
                    if (fuel.Item1 >= 0)
                    {
                        Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation",ContentSource.GameContent);
                        var objectName = Lexicon.makePlural(ObjectUtils.GetObjectParameter(objects[fuel.Item1], (int) ObjectParameter.DisplayName), fuel.Item2 == 1);
                        throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new {amount = fuel.Item2, objectName}));
                    }
                    else
                    {
                        var objectName = ObjectUtils.GetCategoryName(fuel.Item1);
                        throw new RestrictionException(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new {amount = fuel.Item2, objectName}));
                    }
                }
            }
        }

        public static OutputConfig ProduceOutput(ProducerRule producerRule, Object producer,
            Func<int, int, bool> fuelSearch, Farmer who, GameLocation location
            , ProducerConfig producerConfig = null, Object input = null
            , bool probe = false, bool noSoundAndAnimation = false)
        {
            if (who == null)
            {
                who = Game1.getFarmer((long)producer.owner);
            }
            Vector2 tileLocation = producer.TileLocation;
            Random random = ProducerRuleController.GetRandomForProducing(tileLocation);
            OutputConfig outputConfig = OutputConfigController.ChooseOutput(producerRule.OutputConfigs, random, fuelSearch, location, input);
            if (outputConfig != null)
            {
                Object output = OutputConfigController.CreateOutput(outputConfig, input, random);

                producer.heldObject.Value = output;
                if (!probe)
                {
                    OutputConfigController.LoadOutputName(outputConfig, producer.heldObject.Value, input, who);

                    if (!noSoundAndAnimation)
                    {
                        SoundUtil.PlaySound(producerRule.Sounds, location);
                        SoundUtil.PlayDelayedSound(producerRule.DelayedSounds, location);
                    }

                    producer.minutesUntilReady.Value = outputConfig.MinutesUntilReady ?? producerRule.MinutesUntilReady;
                    if (producerRule.SubtractTimeOfDay)
                    {
                        producer.minutesUntilReady.Value = Math.Max(producer.minutesUntilReady.Value - Game1.timeOfDay, 1);
                    }

                    if (producerConfig != null)
                    {
                        producer.showNextIndex.Value = producerConfig.AlternateFrameProducing;
                    }

                    if (producerRule.PlacingAnimation.HasValue && !noSoundAndAnimation)
                    {
                        AnimationController.DisplayAnimation(producerRule.PlacingAnimation.Value,
                            producerRule.PlacingAnimationColor, location, tileLocation,
                            new Vector2(producerRule.PlacingAnimationOffsetX, producerRule.PlacingAnimationOffsetY));
                    }

                    producer.initializeLightSource(tileLocation, false);

                    producerRule.IncrementStatsOnInput.ForEach(s => StatsController.IncrementStardewStats(s, producerRule.InputStack));
                }

            }
            return outputConfig;
        }

        /// <summary>
        /// Create a Random instance using the seed rules for a given positioned machine.
        /// </summary>
        /// <param name="tileLocation">The position of the machines the random should be created for.</param>
        /// <returns>The random instnace</returns>
        public static Random GetRandomForProducing(Vector2 tileLocation)
        {
            return new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed * (int)Game1.stats.DaysPlayed * 1000000531 + (int)tileLocation.X * (int)tileLocation.X * 100207 + (int)tileLocation.Y * (int)tileLocation.Y * 1031 + Game1.timeOfDay/10);
        }

        internal static void ClearProduction(Object producer)
        {
            producer.heldObject.Value = (Object)null;
            producer.readyForHarvest.Value = false;
            producer.showNextIndex.Value = false;
            producer.minutesUntilReady.Value = -1;
        }
    }
}
