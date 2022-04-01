/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StopRugRemoval
**
*************************************************/

using System.Reflection;
using AtraBase.Toolkit.Reflection;
using AtraShared.Utils;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace StopRugRemoval.HarmonyPatches.BombHandling;

/// <summary>
/// Class to hold patches against answerDialogueAction to handle bomb confirmation.
/// </summary>
[HarmonyPatch]
internal static class ConfirmBomb
{
    /// <summary>
    /// Saves whether or not the user has confirmed that they want to place bombs for the map.
    /// </summary>
    internal static readonly PerScreen<bool> HaveConfirmed = new(createNewState: () => false);

    /// <summary>
    /// Saves the location the user was trying to place the bomb.
    /// </summary>
    internal static readonly PerScreen<Vector2> BombLocation = new(createNewState: () => Vector2.Zero);

    /// <summary>
    /// Saves which bomb the user was trying to place.
    /// </summary>
    internal static readonly PerScreen<int> WhichBomb = new(createNewState: () => 0);

    /// <summary>
    /// Defines the methods for which to patch.
    /// </summary>
    /// <returns>Methods to patch.</returns>
    [UsedImplicitly]
    internal static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (Type t in typeof(GameLocation).GetAssignableTypes(publiconly: true, includeAbstract: false))
        {
            if (t.DeclaredInstanceMethodNamedOrNull(nameof(GameLocation.answerDialogueAction), new Type[] { typeof(string), typeof(string[]) }) is MethodBase method
                && method.DeclaringType == t)
            {
                yield return method;
            }
        }
    }

    /// <summary>
    /// Prefixes answerDialogueAction to handle the bomb questions.
    /// </summary>
    /// <param name="__instance">Gamelocation.</param>
    /// <param name="__0">question_answer.</param>
    /// <param name="__result">Result to substitute in for original function.</param>
    /// <returns>True to continue to vanilla function, false to skip.</returns>
    [UsedImplicitly]
    [SuppressMessage("StyleCop", "SA1313", Justification = "Style prefered by Harmony")]
    internal static bool Prefix(GameLocation __instance, string __0, ref bool __result)
    {
        try
        {
            switch (__0)
            {
                case "atravitaInteractionTweaksBombs_BombsArea":
                    HaveConfirmed.Value = true;
                    goto case "atravitaInteractionTweaksBombs_BombsYes";
                case "atravitaInteractionTweaksBombs_BombsYes":
                    Game1.player.reduceActiveItemByOne();
                    GameLocationUtils.ExplodeBomb(__instance, WhichBomb.Value, BombLocation.Value, ModEntry.Multiplayer);
                    break;
                case "atravitaInteractionTweaksBombs_BombsNo":
                    break;
                default:
                    return true;
            }
            __result = true;
            return false;
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into issues in prefix for confirming bombs.\n\n{ex}", LogLevel.Error);
        }
        return true;
    }
}