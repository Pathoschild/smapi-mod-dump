/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Smoked-Fish/ParticleFramework
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System;


namespace ParticleFramework.Framework.Patches
{
    internal class PatchTemplate
    {
        internal static Harmony _harmony;

        internal PatchTemplate(Harmony modHarmony)
        {
            _harmony = modHarmony;
        }

        /// <summary>
        /// Applies method patches using Harmony for a specified target method.
        /// </summary>
        /// <param name="originalMethod">The name of the original method to patch.</param>
        /// <param name="newMethod">The name of the method to be applied as a patch.</param>
        /// <param name="parameters">Optional parameters for the method.</param>
        public void Patch(Type objectType, string originalMethod, string newMethod, Type[] parameters = null)
        {
            try
            {
                _harmony.Patch(AccessTools.Method(objectType, originalMethod, parameters), postfix: new HarmonyMethod(GetType(), newMethod));
            }
            catch (Exception e)
            {
                ModEntry.monitor.Log($"Issue with Harmony patching for method {originalMethod} with {newMethod}: {e}", LogLevel.Error);
                return;
            }
        }
    }
}
