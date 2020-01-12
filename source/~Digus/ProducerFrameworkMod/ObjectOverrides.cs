using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using ProducerFrameworkMod.ContentPack;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Network;
using Enumerable = System.Linq.Enumerable;
using Object = StardewValley.Object;

namespace ProducerFrameworkMod
{

    internal class ObjectOverrides
    {
        [HarmonyPriority(800)]
        internal static bool PerformObjectDropInAction(Object __instance, Item dropInItem, bool probe, Farmer who, ref bool __result)
        {
            if (__instance.isTemporarilyInvisible || !(dropInItem is Object))
                return false;
            Object input = (Object) dropInItem;

            if (ProducerController.GetProducerItem(__instance.name, input) is ProducerRule producerRule)
            {
                if (__instance.heldObject.Value != null && !__instance.name.Equals("Crystalarium") || (bool)((NetFieldBase<bool, NetBool>)input.bigCraftable))
                {
                    return true;
                }
                if (ProducerRuleController.IsInputExcluded(producerRule, input))
                {
                    return true;
                }

                if ((bool)((NetFieldBase<bool, NetBool>)__instance.bigCraftable) && !probe && (__instance.heldObject.Value == null))
                {
                    __instance.scale.X = 5f;
                }

                bool shouldDisplayMessages = !probe && who.IsLocalPlayer;

                if (ProducerRuleController.IsInputStackLessThanRequired(producerRule, input, shouldDisplayMessages))
                {
                    return false;
                }
                
                if (ProducerRuleController.IsAnyFuelStackLessThanRequired(producerRule, who, shouldDisplayMessages))
                {
                    return false;
                }

                Vector2 tileLocation = __instance.tileLocation.Value;
                Random random = ProducerRuleController.GetRandomForProducing(tileLocation);
                OutputConfig outputConfig = OutputConfigController.ChooseOutput(producerRule.OutputConfigs, random);

                Object output = OutputConfigController.CreateOutput(outputConfig, input, random);

                __instance.heldObject.Value = output;

                if (!probe)
                {
                    OutputConfigController.LoadOutputName(outputConfig, __instance.heldObject.Value, input, who);

                    GameLocation currentLocation = who.currentLocation;
                    PlaySound(producerRule.Sounds, currentLocation);
                    PlayDelayedSound(producerRule.DelayedSounds, currentLocation);

                    __instance.minutesUntilReady.Value = producerRule.MinutesUntilReady;

                    if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig)
                    {
                        __instance.showNextIndex.Value = producerConfig.AlternateFrameProducing;
                    }

                    if (producerRule.PlacingAnimation.HasValue)
                    {
                        AnimationController.DisplayAnimation(producerRule.PlacingAnimation.Value, producerRule.PlacingAnimationColor, currentLocation, tileLocation, new Vector2( producerRule.PlacingAnimationOffsetX, producerRule.PlacingAnimationOffsetY));
                    }

                    __instance.initializeLightSource(tileLocation, false);

                    foreach (var fuel in producerRule.FuelList)
                    {
                        RemoveItemsFromInventory(who, fuel.Item1, fuel.Item2);
                    }

                    producerRule.IncrementStatsOnInput.ForEach(s=> StatsController.IncrementStardewStats(s, producerRule.InputStack));

                    input.Stack -= producerRule.InputStack;
                    __result = input.Stack <= 0;
                }
                else
                {
                    __result = true;
                }
                return false;
            }

            return true;
        }
        private static void PlaySound(List<string> soundList, GameLocation currentLocation)
        {
            soundList.ForEach(s =>
            {
                try
                {
                    currentLocation.playSound(s, NetAudio.SoundContext.Default);
                }
                catch (Exception)
                {
                    ProducerFrameworkModEntry.ModMonitor.Log($"Error trying to play sound '{s}'.");
                }
            });
        }

        private static void PlayDelayedSound(List<Dictionary<string,int>> delayedSoundList, GameLocation currentLocation)
        {
            foreach (Dictionary<string, int> dictionary in delayedSoundList)
            {
                foreach (KeyValuePair<string, int> pair in dictionary)
                {
                    DelayedAction.playSoundAfterDelay(pair.Key, pair.Value, (GameLocation)null, -1);
                }
            }
        }

        private static bool RemoveItemsFromInventory(Farmer farmer, int index, int stack)
        {
            if (farmer.hasItemInInventory(index, stack, 0))
            {
                for (int index1 = 0; index1 < farmer.items.Count; ++index1)
                {
                    if (farmer.items[index1] != null && farmer.items[index1] is Object object1 && (object1.ParentSheetIndex == index || object1.Category == index))
                    {
                        if (farmer.items[index1].Stack > stack)
                        {
                            farmer.items[index1].Stack -= stack;
                            return true;
                        }
                        stack -= farmer.items[index1].Stack;
                        farmer.items[index1] = (Item)null;
                    }
                    if (stack <= 0)
                        return true;
                }
            }
            return false;
        }

        internal static void checkForAction(Object __instance, bool justCheckingForActivity, bool __result)
        {
            if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig && __instance.heldObject.Value == null && __instance.MinutesUntilReady <= 0)
            {
                __instance.showNextIndex.Value = false;
            }
        }

        internal static void minutesElapsed(Object __instance)
        {
            if (ProducerController.GetProducerConfig(__instance.Name) is ProducerConfig producerConfig && __instance.heldObject.Value != null && __instance.MinutesUntilReady <= 0)
            {
                __instance.showNextIndex.Value = producerConfig.AlternateFrameWhenReady;
            }
        }

        internal static bool LoadDisplayName(Object __instance, ref string __result)
        {
            if (NameUtils.HasCustomNameForIndex(__instance.ParentSheetIndex) && !__instance.preserve.Value.HasValue && __instance.ParentSheetIndex != 463 && __instance.ParentSheetIndex != 464)
            {
                IDictionary<int, string> objects = Game1.objectInformation;
                string translation = NameUtils.GetCustomNameFromIndex(__instance.ParentSheetIndex);
                
                if (objects.TryGetValue(__instance.ParentSheetIndex, out var instanceData) && ObjectUtils.GetObjectParameter(instanceData, (int)ObjectParameter.Name) != __instance.Name)
                {
                    if (translation.Contains("{outputName}"))
                    {
                        translation = translation.Replace("{outputName}",ObjectUtils.GetObjectParameter(instanceData, (int) ObjectParameter.DisplayName));
                    }
                }
                else
                {
                    __result = __instance.Name;
                    return false;
                }
                
                if (translation.Contains("{inputName}"))
                {
                    if (objects.TryGetValue(__instance.preservedParentSheetIndex.Value, out var preservedData))
                    {
                        translation = translation.Replace("{inputName}", ObjectUtils.GetObjectParameter(preservedData,(int)ObjectParameter.DisplayName));
                    }
                    else
                    {
                        __result = __instance.Name;
                        return false;
                    }
                }
                if (translation.Contains("{farmName}"))
                {
                    translation = translation.Replace("{farmName}", Game1.player.farmName.Value);
                }
                if (translation.Contains("{farmerName}"))
                {
                    string farmerName = Game1.getAllFarmers().FirstOrDefault(f => __instance.Name.Contains(f.name))?.Name ?? Game1.player.Name;
                    translation = translation.Replace("{farmerName}", farmerName);
                }
                __result = translation;
                return false;
            }
            return true;
        }
    }
}