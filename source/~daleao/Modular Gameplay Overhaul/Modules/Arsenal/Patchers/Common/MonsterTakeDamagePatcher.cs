/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Common;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterTakeDamagePatcher"/> class.</summary>
    internal MonsterTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Monster>(
            nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    /// <inheritdoc />
    protected override void ApplyImpl(Harmony harmony)
    {
        base.ApplyImpl(harmony);
        foreach (var target in TargetMethods())
        {
            this.Target = target;
            base.ApplyImpl(harmony);
        }
    }

    /// <inheritdoc />
    protected override void UnapplyImpl(Harmony harmony)
    {
        base.UnapplyImpl(harmony);
        foreach (var target in TargetMethods())
        {
            this.Target = target;
            base.UnapplyImpl(harmony);
        }
    }

    [HarmonyTargetMethods]
    private static IEnumerable<MethodBase> TargetMethods()
    {
        var types = new[]
        {
            typeof(AngryRoger), typeof(Bat), typeof(BigSlime), typeof(BlueSquid), typeof(Bug), typeof(Duggy),
            typeof(DwarvishSentry), typeof(Fly), typeof(Ghost), typeof(GreenSlime), typeof(Grub), typeof(LavaCrab),
            typeof(Mummy), typeof(RockCrab), typeof(RockGolem), typeof(ShadowGirl), typeof(ShadowGuy),
            typeof(ShadowShaman), typeof(SquidKid), typeof(Serpent),
        };

        foreach (var type in types)
        {
            yield return type.RequireMethod(
                "takeDamage",
                new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) });
        }
    }

    #region harmony patches

    /// <summary>Record overkill.</summary>
    [HarmonyPrefix]
    private static void MonsterTakeDamagePrefix(Monster __instance, int damage)
    {
        __instance.Set_Overkill(Math.Max(damage - __instance.Health, 0));
    }

    /// <summary>Crits ignore defense, which, btw, actually does something.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MonsterTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: ModEntry.Confi.Arsenal.OverhauledDefense ? DoOverhauledDefense(this, damage) : continue
        // At: start of method
        try
        {
            var doVanillaDefense = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Monster).RequireField(nameof(Monster.resilience))),
                        new CodeInstruction(OpCodes.Call),
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })),
                    },
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .AddLabels(doVanillaDefense)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.OverhauledDefense))),
                        new CodeInstruction(OpCodes.Brfalse_S, doVanillaDefense),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(MonsterTakeDamagePatcher).RequireMethod(nameof(DoOverhauledDefense))),
                        new CodeInstruction(OpCodes.Stloc_0),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    labels)
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_0) })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding overhauled monster defense.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int DoOverhauledDefense(Monster monster, int damage)
    {
        return monster.Get_GotCrit()
            ? damage
            : (int)(damage * (10f / (10f + monster.resilience.Value)));
    }

    #endregion injected subroutines
}
