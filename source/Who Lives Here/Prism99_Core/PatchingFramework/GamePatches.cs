/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using StardewModdingAPI;
using Prism99_Core.Utilities;
using Microsoft.Xna.Framework;
using StardewValley.Characters;
using StardewValley;
using System.Reflection;

namespace Prism99_Core.PatchingFramework
{

    internal class GamePatches
    {
        public Dictionary<string, List<GamePatch>> Patches;
        private Harmony harmony;
        private SDVLogger logger;
        private bool showComments = true;

        public void Initialize(string UniqueID, SDVLogger olog)
        {
            logger = olog;
            Patches = new Dictionary<string, List<GamePatch>> { };
            harmony = new Harmony(UniqueID);
        }
        public string GetResult(bool useHTML)
        {
            string sNewline = useHTML ? "<br>" : "\n";// Environment.NewLine;

            StringBuilder sbDetails = new StringBuilder("Harmony Patches:" + sNewline);

            foreach (string group in Patches.Keys)
            {
                foreach (GamePatch oPatch in Patches[group])
                {
                    if (!string.IsNullOrEmpty(group))
                    {
                        sbDetails.Append(("Group: " + group + sNewline));
                    }
                    sbDetails.Append("Details:" + sNewline + GetPatchDetails(oPatch) + sNewline);
                    sbDetails.Append("Status: " + (oPatch.Failed ? "Failed" + sNewline + "Failure details: " + oPatch.FailureDetails + sNewline : "Applied" + sNewline));
                    sbDetails.Append(sNewline);
                }
            }


            return sbDetails.ToString();
        }
        public string GetPatchDetails(GamePatch oPatch)
        {
            string details = $"original: {oPatch.Original.ReflectedType.Name}.{oPatch.Original.Name}, redirectedTo: {oPatch.Target.method.DeclaringType.Name}.{oPatch.Target.method.Name}";
            return $"{(oPatch.IsPrefix ? "Prefix" : "Postfix")} patch: {details}";
        }

        public void AddGetPatch(bool prefix, Type original, string originalMethod, Type[] OriginalArgs, Type target, string targetMethod, string description, string group)
        {
            try
            {
                AddPatch(new GamePatch
                {
                    IsPrefix = prefix,
                    //Priority = priority,
                    Original = AccessTools.Property(original, originalMethod).GetMethod,
                    Target = new HarmonyMethod(target, targetMethod),
                    Description = description
                }, group);
            }
            catch (Exception ex)
            {
                logger.Log($"Error adding patch.", LogLevel.Error);
                logger.Log($"Original: {original.Name}.{originalMethod}", LogLevel.Error);
                logger.Log($"Target: {target.Name}.{targetMethod}", LogLevel.Error);
                logger.Log($"Description: {description}", LogLevel.Error);
                logger.Log($"Error: {ex.Message}", LogLevel.Error);
            }
        }

        public void AddSetPatch(bool prefix, Type original, string originalMethod, Type[] OriginalArgs, Type target, string targetMethod, string description, string group)
        {
            try
            {
                AddPatch(new GamePatch
                {
                    IsPrefix = prefix,
                    //Priority = priority,
                    Original = AccessTools.Property(original, originalMethod).SetMethod,
                    Target = new HarmonyMethod(target, targetMethod),
                    Description = description
                }, group);
            }
            catch (Exception ex)
            {
                logger.Log($"Error adding patch.", LogLevel.Error);
                logger.Log($"Original: {original.Name}.{originalMethod}", LogLevel.Error);
                logger.Log($"Target: {target.Name}.{targetMethod}", LogLevel.Error);
                logger.Log($"Description: {description}", LogLevel.Error);
                logger.Log($"Error: {ex.Message}", LogLevel.Error);
            }
        }

        public void AddPatch(bool prefix, int priority, Type original, string originalMethod, Type[] OriginalArgs, Type target, string targetMethod, string description, string group)
        {
            try
            {
                AddPatch(new GamePatch
                {
                    IsPrefix = prefix,
                    Priority = priority,
                    Original = AccessTools.Method(original, originalMethod, OriginalArgs),
                    Target = new HarmonyMethod(target, targetMethod),
                    Description = description
                }, group);
            }
            catch (Exception ex)
            {
                logger.Log($"Error adding patch.", LogLevel.Error);
                logger.Log($"Original: {original.Name}.{originalMethod}", LogLevel.Error);
                logger.Log($"Target: {target.Name}.{targetMethod}", LogLevel.Error);
                logger.Log($"Description: {description}", LogLevel.Error);
                logger.Log($"Error: {ex.Message}", LogLevel.Error);
            }
        }
        public void AddPatch(bool prefix, Type original, string originalMethod, Type[] OriginalArgs, Type target, string targetMethod, string description, string group)
        {
            try
            {
                AddPatch(new GamePatch
                {
                    IsPrefix = prefix,
                    Original = AccessTools.Method(original, originalMethod, OriginalArgs),
                    Target = new HarmonyMethod(target, targetMethod),
                    Description = description
                }, group);
            }
            catch (Exception ex)
            {
                logger.Log($"Error adding patch.", LogLevel.Error);
                logger.Log($"Original: {original.Name}.{originalMethod}", LogLevel.Error);
                logger.Log($"Target: {target.Name}.{targetMethod}", LogLevel.Error);
                logger.Log($"Description: {description}", LogLevel.Error);
                logger.Log($"Error: {ex.Message}", LogLevel.Error);
            }
        }
        public void AddPatch(GamePatch oPatch, string sPatchGroup)
        {
            oPatch.Applied = false;
            if (!Patches.ContainsKey(sPatchGroup))
            {
                Patches.Add(sPatchGroup, new List<GamePatch> { });
            }
            Patches[sPatchGroup].Add(oPatch);
        }

        public void ApplyPatches(string sPatchGroup)
        {
            foreach (string key in Patches.Keys)
            {
                if (string.IsNullOrEmpty(sPatchGroup) || key == sPatchGroup)
                {
                    foreach (GamePatch oPatch in Patches[key])
                    {
                        if (!oPatch.Applied)
                        {
                            string details = "";
                            try
                            {
                                if (oPatch.Original == null)
                                {
                                    oPatch.Failed = true;
                                    oPatch.FailureDetails = $"Original method does not exist.";
                                    logger.Log($"Patch skipped.  The original method does not exist", LogLevel.Error);
                                    logger.Log($"Patch details: {oPatch.Description}", LogLevel.Error);
                                    continue;
                                }
                                details = GetPatchDetails(oPatch);

                                string sFullDetails = details;
                                try
                                {
                                    oPatch.Applied = true;

                                    if (oPatch.Target.method.IsStatic)
                                    {
                                        if (oPatch.IsPrefix)
                                        {
                                            if (oPatch.Priority > -1)
                                            {
                                                oPatch.Target.priority = oPatch.Priority;
                                            }
                                            if (oPatch.Target.method.ReturnType == typeof(bool))
                                            {
                                                harmony.Patch(
                                                original: oPatch.Original,
                                                prefix: oPatch.Target
                                                );
                                            }
                                            else
                                            {
                                                oPatch.Failed = true;
                                                oPatch.FailureDetails = "Patch skipped.  The prefix method does return a bool and the patch would fail";

                                                logger.Log($"Patch skipped.  The prefix method does return a bool and the patch would fail", LogLevel.Error);
                                                logger.Log($"Patch details: {sFullDetails}", LogLevel.Error);
                                            }
                                        }
                                        else
                                        {
                                            harmony.Patch(
                                            original: oPatch.Original,
                                            postfix: oPatch.Target
                                            );
                                        }
                                        logger.Log($"Applied Harmony {sFullDetails}", LogLevel.Debug);
                                        if (showComments)
                                        {
                                            logger.Log($"Pacth purpose: {oPatch.Description}", LogLevel.Debug);
                                        }
                                    }
                                    else
                                    {
                                        oPatch.Failed = true;
                                        oPatch.FailureDetails = "Patch skipped.  Patch skipped. The redirected method is not static and the patch would fail";

                                        logger.Log($"Patch skipped. The redirected method is not static and the patch would fail", LogLevel.Error);
                                        logger.Log($"Patch details: {sFullDetails}", LogLevel.Error);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    logger.Log($"Patch failed. Patch details: {sFullDetails}", LogLevel.Error);
                                    oPatch.Failed = true;
                                    logger.Log($"Error: {ex.Message}", LogLevel.Error);
                                }

                            }
                            catch (Exception ex)
                            {
                                logger.Log($"Error applying patch {sPatchGroup}:{oPatch.Description}", LogLevel.Error);
                                logger.Log($"Error: {ex.Message}", LogLevel.Error);
                            }
                        }

                    }
                }
            }
        }
    }

}