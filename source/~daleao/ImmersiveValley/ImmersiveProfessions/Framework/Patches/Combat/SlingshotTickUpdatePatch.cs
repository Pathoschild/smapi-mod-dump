/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using Extensions;
using HarmonyLib;
using Sounds;
using StardewValley.Tools;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotTickUpdatePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotTickUpdatePatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.tickUpdate));
    }

    /// <summary>Patch to overcharge Slingshot for Desperado.</summary>
    [HarmonyPostfix]
    internal static void SlingshotTickUpdatePostfix(Slingshot __instance, Farmer who)
    {
        if (!who.HasProfession(Profession.Desperado) || who.CurrentTool != __instance || !who.usingSlingshot) return;

        var overcharge = __instance.GetDesperadoOvercharge(who);
        if (overcharge <= 0f) return;

        who.jitterStrength = Math.Max(0f, overcharge - 0.5f);

        if (Game1.soundBank is null) return;

        SFX.SinWave ??= Game1.soundBank.GetCue("SinWave");
        if (!SFX.SinWave.IsPlaying) SFX.SinWave.Play();

        SFX.SinWave.SetVariable("Pitch", 2400f * overcharge);
    }
}