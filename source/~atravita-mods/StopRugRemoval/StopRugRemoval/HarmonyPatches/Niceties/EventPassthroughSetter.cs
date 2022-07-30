/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using HarmonyLib;

namespace StopRugRemoval.HarmonyPatches.Niceties;

/// <summary>
/// Forcibly set the pass-through-things flag on every character in an event.
/// There's no reason not to set it and this prevents crashes.
/// </summary>
[HarmonyPatch(typeof(Event))]
internal static class EventPassthroughSetter
{
    [UsedImplicitly]
    [HarmonyPatch("setUpCharacters")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used implicitly.")]
    private static void Postfix(Event __instance)
    {
        foreach (Farmer f in __instance.farmerActors)
        {
            f.ignoreCollisions = true;
        }
        foreach (NPC a in __instance.actors)
        {
            a.isCharging = true;
        }
    }
}