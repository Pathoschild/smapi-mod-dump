using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using SilentOak.Patching.Exceptions;
using SilentOak.Patching.Extensions;

namespace SilentOak.Patching
{
    /// <summary>
    /// Helper for managing and applying patches.
    /// </summary>
    public static class PatchManager
    {
        /*************
         * Properties
         *************/

        /// <summary>Gets the underlying patcher.</summary>
        /// <value>The underlying patcher.</value>
        private static HarmonyInstance Patcher { get; } = HarmonyInstance.Create("silentoak.qualityproducts");


        /*****************
         * Public methods
         *****************/

        /// <summary>Applies all the given patches.</summary>
        /// <param name="patches">Patches.</param>
        public static void ApplyAll(params Type[] patches)
        {
            foreach (Type patch in patches)
            {
                Apply(patch);
            }
        }

        /// <summary>Applies the given patch</summary>
        /// <param name="patch"></param>
        public static void Apply(Type patch)
        {
            MethodInfo prefixPatch = patch.GetMethod("Prefix", BindingFlags.Public | BindingFlags.Static);
            MethodInfo postfixPatch = patch.GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);

            PatchData patchData;
            try
            {
                patchData = GetPatchData(patch);
                if (patchData.Exception != null)
                {
                    throw patchData.Exception;
                }
            }
            catch (FormatException e)
            {
                throw new FormatException($"Invalid version expression in {patch.FullName}", e);
            }

            foreach (MethodBase methodToPatch in patchData.MethodsToPatch)
            {
                Patcher.Patch(
                    original: methodToPatch,
                    prefix: prefixPatch != null ? new HarmonyMethod(prefixPatch) : null,
                    postfix: postfixPatch != null ? new HarmonyMethod(postfixPatch) : null,
                    transpiler: null
                );
            }
        }


        /*******************
         * Internal methods
         *******************/

        /// <summary>Gets the patch data for the given patch</summary>
        /// <returns>The patch data.</returns>
        /// <param name="patch">Patch.</param>
        internal static PatchData GetPatchData(Type patch)
        {
            if (!(AccessTools.Field(patch, "PatchData")?.GetValue(null) is PatchData patchData))
            {
                throw new MissingAttributeException(patch.FullName, "PatchData");
            }

            return patchData;
        }
    }
}
