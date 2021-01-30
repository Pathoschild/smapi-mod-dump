/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/ExpandedStorage
**
*************************************************/

using Harmony;
using HarmonyPatch = ExpandedStorage.Framework.Patches.HarmonyPatch;

namespace ExpandedStorage.Framework
{
    internal class Patcher
    {
        private readonly string _uniqueId;

        internal Patcher(string uniqueId)
        {
            _uniqueId = uniqueId;
        }

        internal void ApplyAll(params HarmonyPatch[] patches)
        {
            var harmony = HarmonyInstance.Create(_uniqueId);

            foreach (var patch in patches)
            {
                patch.Apply(harmony);
            }
        }
    }
}