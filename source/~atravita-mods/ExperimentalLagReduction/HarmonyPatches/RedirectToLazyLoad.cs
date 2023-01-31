/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Extensions;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace ExperimentalLagReduction.HarmonyPatches;

[HarmonyPatch]
internal static class RedirectToLazyLoad
{
     /// <summary>
     /// Gets the methods to patch.
     /// </summary>
     /// <returns>An IEnumerable of methods to patch.</returns>
    internal static IEnumerable<MethodBase> TargetMethods()
    {
        yield return typeof(Game1).GetCachedMethod<NPC, GameLocation, Vector2>(nameof(Game1.warpCharacter), ReflectionCache.FlagTypes.StaticFlags);
        yield return typeof(GameLocation).GetCachedMethod(nameof(GameLocation.cleanupBeforePlayerExit), ReflectionCache.FlagTypes.InstanceFlags);
        yield return typeof(NPC).GetCachedMethod(nameof(NPC.wearNormalClothes), ReflectionCache.FlagTypes.InstanceFlags);
        yield break;
    }

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new (original, instructions, ModEntry.ModMonitor, gen);

            helper.ForEachMatch(
                new CodeInstructionWrapper[]
                {
                    new(OpCodes.Callvirt, typeof(AnimatedSprite).GetCachedMethod(nameof(AnimatedSprite.LoadTexture), ReflectionCache.FlagTypes.InstanceFlags)),
                },
                (helper) =>
                {
                    helper.ReplaceInstruction(
                        instruction: new CodeInstruction(OpCodes.Call, typeof(RedirectToLazyLoad).GetCachedMethod(nameof(ResetSprite), ReflectionCache.FlagTypes.StaticFlags)),
                        keepLabels: true);
                    return true;
                });

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Mod crashed while transpiling {original.GetFullName()}\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }

        return null;
    }

    /// <summary>
    /// A call that requests the sprite be reset WITHOUT reloading it immediately.
    /// </summary>
    /// <param name="sprite">sprite to reset.</param>
    /// <param name="textureName">Name of the new texture.</param>
    private static void ResetSprite(this AnimatedSprite sprite, string textureName)
    {
        sprite.textureName.Value = textureName;
    }
}