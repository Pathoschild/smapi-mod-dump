/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using DaLion.Common;
using DaLion.Common.Data;
using DaLion.Common.Harmony;
using Events.GameLoop;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetImmediateProfessionPerkPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuGetImmediateProfessionPerkPatch()
    {
        Target = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to add modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuGetImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (!Profession.TryFromValue(whichProfession, out var profession) ||
            whichProfession == Farmer.luckSkill) return;

        if ((Skill)profession.Skill == Farmer.combatSkill)
        {
            Game1.player.maxHealth += 5;
            Game1.player.health = Game1.player.maxHealth;
        }

        // add immediate perks
        if (profession == Profession.Aquarist)
            foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                         (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         !p.isUnderConstruction()))
                pond.UpdateMaximumOccupancy();

        // subscribe events
        ModEntry.EventManager.HookForProfession(profession);
        if (!Context.IsMainPlayer)
        {
            // request the main player
            if (profession == Profession.Aquarist)
                ModEntry.Broadcaster.Message("Conservationism", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
            else if (profession == Profession.Conservationist)
                ModEntry.Broadcaster.Message("Conservationism", "RequestEvent", Game1.MasterPlayer.UniqueMultiplayerID);
        }
        else if (profession == Profession.Conservationist)
        {
            ModEntry.EventManager.Hook<HostConservationismDayEndingEvent>();
        }

        if (whichProfession is < 26 or >= 30 || ModEntry.PlayerState.RegisteredUltimate is not null) return;

        // register Ultimate
        var newIndex = (UltimateIndex)whichProfession;
        ModEntry.PlayerState.RegisteredUltimate =
#pragma warning disable CS8509
            ModEntry.PlayerState.RegisteredUltimate = newIndex switch
#pragma warning restore CS8509
            {
                UltimateIndex.BruteFrenzy => new UndyingFrenzy(),
                UltimateIndex.PoacherAmbush => new Ambush(),
                UltimateIndex.PiperPandemic => new Pandemic(),
                UltimateIndex.DesperadoBlossom => new DeathBlossom()
            };
        ModDataIO.WriteTo(Game1.player, "UltimateIndex", newIndex.ToString());
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuGetImmediateProfessionPerkTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: case <defender_id>:
        /// To: case <brute_id>:

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_S, Farmer.defender)
                )
                .SetOperand(Profession.Brute.Value);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}