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
        /// <param name="producerRule">the producer tule to check</param>
        /// <param name="input">The input to check</param>
        /// <param name="shouldDisplayMessages">If an ingame message should be shown when the input is not enough</param>
        /// <returns>true if the stack if not enough</returns>
        public static bool IsInputStackLessThanRequired(ProducerRule producerRule, Object input, bool shouldDisplayMessages)
        {
            int requiredStack = producerRule.InputStack;
            if (input.Stack < requiredStack)
            {
                if (shouldDisplayMessages)
                {
                    Game1.showRedMessage(DataLoader.Helper.Translation.Get(
                        "Message.Requirement.Amount"
                        , new { amount = requiredStack, objectName = Lexicon.makePlural(input.DisplayName, requiredStack == 1) }
                    ));
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if a farmer has the required fules and stack for a given producer rule.
        /// </summary>
        /// <param name="producerRule">the producer tule to check</param>
        /// <param name="who">The farmer to check</param>
        /// <param name="shouldDisplayMessages">If an ingame message should be shown when the fuel is not enough</param>
        /// <returns></returns>
        public static bool IsAnyFuelStackLessThanRequired(ProducerRule producerRule, Farmer who, bool shouldDisplayMessages)
        {
            foreach (Tuple<int, int> fuel in producerRule.FuelList)
            {
                if (!who.hasItemInInventory(fuel.Item1, fuel.Item2))
                {
                    if (shouldDisplayMessages)
                    {
                        if (fuel.Item1 >= 0)
                        {
                            Dictionary<int, string> objects = DataLoader.Helper.Content.Load<Dictionary<int, string>>("Data\\ObjectInformation",ContentSource.GameContent);
                            var objectName = Lexicon.makePlural(ObjectUtils.GetObjectParameter(objects[fuel.Item1], (int) ObjectParameter.DisplayName), fuel.Item2 == 1);
                            Game1.showRedMessage(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new {amount = fuel.Item2, objectName}));
                        }
                        else
                        {
                            var objectName = ObjectUtils.GetCategoryName(fuel.Item1);
                            Game1.showRedMessage(DataLoader.Helper.Translation.Get("Message.Requirement.Amount", new {amount = fuel.Item2, objectName}));
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Create a Random instance using the seed rules for a given positioned machine.
        /// </summary>
        /// <param name="tileLocation">The position of the machines the random should be created for.</param>
        /// <returns>The random instnace</returns>
        public static Random GetRandomForProducing(Vector2 tileLocation)
        {
            return new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed * 10000000 + Game1.timeOfDay * 10000 + (int)tileLocation.X * 200 + (int)tileLocation.Y);
        }
    }
}
