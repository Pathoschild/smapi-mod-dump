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
using System.Runtime.CompilerServices;
using AtraBase.Toolkit;
using AtraCore.Framework.ReflectionManager;
using AtraShared.Niceties;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using Microsoft.Xna.Framework.Audio;

namespace StopRugRemoval.HarmonyPatches.Niceties.CrashHandling;

/// <summary>
/// Adds in a check to see if the music cue exists before trying to play it.
/// </summary>
[HarmonyPatch(typeof(Game1))]
internal static class PreventMusicCrash
{
    [MethodImpl(TKConstants.Hot)]
    private static bool ReplaceMusicCueIfNecessary(string cue)
    {
        if (cue.Equals("none"))
        {
            return true;
        }
        if (Game1.soundBank is null || !Context.IsWorldReady || Game1.soundBank is DummySoundBank)
        {
            return false;
        }
        if (Game1.soundBank is SoundBankWrapper soundBank)
        {
            try
            {
                SoundBank? soundBankImpl = SoundBankWrapperHandler.GetActualSoundBank(soundBank);
                if (!SoundBankWrapperHandler.HasCue(soundBankImpl, cue))
                {
                    ModEntry.ModMonitor.Log($"Suppressing unknown cue {cue}", LogLevel.Info);
                    Game1.showRedMessage($"Music cue {cue} suppressed");
                    return true;
                }
            }
            catch (Exception ex)
            {
                ModEntry.ModMonitor.Log($"Failed in checking if the cue {cue} exists in the soundbank\n\n{ex}", LogLevel.Error);
            }
        }
        else
        {
            ModEntry.ModMonitor.LogOnce($"Stardew's implementation of soundbank seems to have changed since I wrote this. {Game1.soundBank.GetType()}", LogLevel.Debug);
        }
        return false;
    }

    [HarmonyPatch(nameof(Game1.updateMusic))]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Ldsfld, typeof(Game1).GetCachedField(nameof(Game1.requestedMusicTrack), ReflectionCache.FlagTypes.StaticFlags)),
                new(SpecialCodeInstructionCases.StLoc),
            })
            .Advance(1);

            CodeInstruction? ldloc = helper.CurrentInstruction.ToLdLoc();

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(ldloc),
                new(OpCodes.Ldstr, "none"),
                new(OpCodes.Callvirt, typeof(string).GetCachedMethod(nameof(string.Equals), ReflectionCache.FlagTypes.InstanceFlags, new[] { typeof(string) } )),
                new(OpCodes.Brfalse_S),
            })
            .Advance(1)
            .Remove(1)
            .ReplaceInstruction(OpCodes.Call, typeof(PreventMusicCrash).GetCachedMethod(nameof(ReplaceMusicCueIfNecessary), ReflectionCache.FlagTypes.StaticFlags));

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into error transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
