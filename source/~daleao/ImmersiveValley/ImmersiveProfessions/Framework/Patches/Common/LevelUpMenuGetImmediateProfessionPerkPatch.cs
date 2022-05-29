/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

using DaLion.Common.Harmony;
using Events.Content;
using Events.GameLoop;
using Extensions;
using Ultimate;

#endregion using directives

[UsedImplicitly]
internal class LevelUpMenuGetImmediateProfessionPerkPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuGetImmediateProfessionPerkPatch()
    {
        Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Patch to add modded immediate profession perks.</summary>
    [HarmonyPostfix]
    private static void LevelUpMenuGetImmediateProfessionPerkPostfix(int whichProfession)
    {
        if (whichProfession.GetCorrespondingSkill() == SkillType.Combat)
        {
            Game1.player.maxHealth += 5;
            Game1.player.health = Game1.player.maxHealth;
        }

        if (!Enum.IsDefined(typeof(Profession), whichProfession)) return;

        var profession = (Profession) whichProfession;

        // add immediate perks
        if (profession == Profession.Aquarist)
            foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                         (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                         !p.isUnderConstruction()))
                pond.UpdateMaximumOccupancy();

        // subscribe events
        EventManager.EnableAllForProfession(profession);
        if (!Context.IsMainPlayer)
        {
            // request the main player
            if (profession == Profession.Aquarist)
                ModEntry.ModHelper.Multiplayer.SendMessage("Conservationism", "RequestEvent",
                    new[] { ModEntry.Manifest.UniqueID }, new[] { Game1.MasterPlayer.UniqueMultiplayerID });
            else if (profession == Profession.Conservationist)
                ModEntry.ModHelper.Multiplayer.SendMessage("Conservationism", "RequestEvent",
                    new[] {ModEntry.Manifest.UniqueID}, new[] {Game1.MasterPlayer.UniqueMultiplayerID});
        }
        else
        {
            if (profession == Profession.Aquarist) EventManager.Enable(typeof(HostFishPondDataRequestedEvent));
            else if (profession == Profession.Conservationist) EventManager.Enable(typeof(HostConservationismDayEndingEvent));
        }

        if (whichProfession is < 26 or >= 30 || ModEntry.PlayerState.RegisteredUltimate is not null) return;
        
        // register Ultimate
        var newIndex = (UltimateIndex) whichProfession;
        ModEntry.PlayerState.RegisteredUltimate =
#pragma warning disable CS8509
            ModEntry.PlayerState.RegisteredUltimate = newIndex switch
#pragma warning restore CS8509
            {
                UltimateIndex.Frenzy => new Frenzy(),
                UltimateIndex.Ambush => new Ambush(),
                UltimateIndex.Pandemonia => new Pandemonia(),
                UltimateIndex.Blossom => new DeathBlossom()
            };
        Game1.player.WriteData(DataField.UltimateIndex, newIndex.ToString());
    }

    /// <summary>Patch to move bonus health from Defender to Brute.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> LevelUpMenuGetImmediateProfessionPerkTranspiler(
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
                .SetOperand((int) Profession.Brute);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving vanilla Defender health bonus to Brute.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}