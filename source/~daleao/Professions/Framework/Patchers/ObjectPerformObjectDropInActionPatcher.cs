/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformObjectDropInActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformObjectDropInActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ObjectPerformObjectDropInActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performObjectDropInAction));
    }

    #region harmony patches

    /// <summary>Patch to remember initial machine state.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static bool ObjectPerformObjectDropInActionPrefix(SObject __instance, out bool __state, bool probe)
    {
        __state = __instance.heldObject.Value !=
                  null && !probe; // remember whether this machine was already holding an object
        return true; // run original logic
    }

    /// <summary>Patch to increase Artisan production + integrate Quality Artisan Products + Immersive Diary Yield tweak.</summary>
    [HarmonyPostfix]
    private static void ObjectPerformObjectDropInActionPostfix(
        SObject __instance, bool __state, Item dropInItem, Farmer who)
    {
        // if there was an object inside before running the original method, or if the machine is not an artisan machine, or if the machine is still empty after running the original method, then do nothing
        if (__state || !__instance.IsArtisanMachine() || __instance.heldObject.Value is not { } output ||
            dropInItem is not SObject input)
        {
            return;
        }

        HandleImmersiveDairyYield(__instance, input, ref output);

        var user = who;
        var owner = __instance.GetOwner();
        var r = new Random(Guid.NewGuid().GetHashCode());

        // artisan users can preserve the input quality (golden egg is always best quality)
        if (user.HasProfession(Profession.Artisan) && input.QualifiedItemId != QualifiedObjectIds.GoldenEgg)
        {
            output.Quality = input.Quality;
            if (!user.HasProfession(Profession.Artisan, true))
            {
                output.Quality = input.Quality;
                if (r.NextDouble() > who.FarmingLevel / 30d)
                {
                    output.Quality = (int)((ObjectQuality)output.Quality).Decrement();
                    if (r.NextDouble() > who.FarmingLevel / 15d)
                    {
                        output.Quality = (int)((ObjectQuality)output.Quality).Decrement();
                    }
                }
            }
        }

        // artisan-owned machines work faster and may upgrade quality
        if (!owner.HasProfessionOrLax(Profession.Artisan))
        {
            return;
        }

        if (output.Quality < SObject.bestQuality && Game1.random.NextBool(0.05))
        {
            output.Quality += output.Quality == SObject.highQuality ? 2 : 1;
        }

        __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 10;
    }

    #endregion harmony patches

    #region injections

    private static void HandleImmersiveDairyYield(SObject machine, SObject input, ref SObject output)
    {
        // large milk/eggs give double output at normal quality
        if (input.Category is SObject.EggCategory or SObject.MilkCategory && input.Name.ContainsAnyOf("Large", "L.") &&
            Config.ImmersiveDairyYield)
        {
            output.Stack = 2;
            output.Quality = SObject.lowQuality;
        }
        else if (machine.QualifiedItemId == QualifiedBigCraftableIds.MayonnaiseMachine)
        {
            switch (input.QualifiedItemId)
            {
                // ostrich mayonnaise keeps giving x10 output but doesn't respect input quality without Artisan
                case QualifiedObjectIds.OstrichEgg:
                    if (Config.EnableGoldenOstrichMayo)
                    {
                        output = ItemRegistry.Create<SObject>($"{UniqueId}/OstrichMayo");
                    }
                    else if (Config.ImmersiveDairyYield)
                    {
                        output.Quality = SObject.lowQuality;
                    }

                    break;
                // golden mayonnaise gives single output but maxes quality
                case QualifiedObjectIds.GoldenEgg:
                    if (Config.EnableGoldenOstrichMayo)
                    {
                        output = ItemRegistry.Create<SObject>($"{UniqueId}/GoldenMayo");
                    }
                    else if (Config.ImmersiveDairyYield)
                    {
                        output.Stack = 1;
                        output.Quality = SObject.bestQuality;
                    }

                    break;
            }
        }
    }

    #endregion injections
}
