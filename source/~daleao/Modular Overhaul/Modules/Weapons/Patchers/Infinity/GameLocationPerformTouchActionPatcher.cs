/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformTouchActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationPerformTouchActionPatcher"/> class.</summary>
    internal GameLocationPerformTouchActionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.performTouchAction));
    }

    #region harmony patches

    /// <summary>Apply new galaxy sword conditions.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationPerformTouchActionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (Game1.player.ActiveObject != null && Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, 74) && !Game1.player.mailReceived.Contains("galaxySword"))
        // To: if (DoesPlayerMeetGalaxyConditions())
        //     -- and also
        // Injected: this.playSound("thunder");
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.ActiveObject))),
                    })
                .StripLabels(out var labels)
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse) })
                .GetOperand(out var didNotMeetConditions)
                .Return()
                .Count(new[] { new CodeInstruction(OpCodes.Brtrue) }, out var count)
                .Remove(count)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocationPerformTouchActionPatcher)
                                .RequireMethod(nameof(DoesPlayerMeetGalaxyConditions))),
                        new CodeInstruction(OpCodes.Brfalse, didNotMeetConditions),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldstr, "thunder"),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocation).RequireMethod(nameof(GameLocation.playSound))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting custom galaxy sword conditions.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool DoesPlayerMeetGalaxyConditions()
    {
        var player = Game1.player;
        if (player.ActiveObject is null ||
            !Utility.IsNormalObjectAtParentSheetIndex(player.ActiveObject, SObject.prismaticShardIndex))
        {
            return false;
        }

        if (!WeaponsModule.Config.InfinityPlusOne)
        {
            return true;
        }

        if (player.Items.Sum(item =>
                item is SObject { ParentSheetIndex: SObject.iridiumBar } iridium ? iridium.Stack : 0) <
            WeaponsModule.Config.IridiumBarsPerGalaxyWeapon)
        {
            Game1.drawObjectDialogue(I18n.Prestige_Dogstatue_Dismiss());
            return false;
        }

        var obtained = player.Read(DataKeys.GalaxyArsenalObtained).ParseList<int>();
        return obtained.Count < 4 && player.ActiveObject.Stack >= obtained.Count + 1;
    }

    #endregion injected subroutines
}
