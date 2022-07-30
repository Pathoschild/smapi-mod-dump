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
using AtraCore.Framework.ReflectionManager;
using AtraShared.ConstantsAndEnums;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Tools;

namespace AvoidLosingScepter;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        this.ApplyPatches(new Harmony(this.ModManifest.UniqueID));
    }

    /// <summary>
    /// Applies the patches for this mod.
    /// </summary>
    /// <param name="harmony">This mod's harmony instance.</param>
    /// <remarks>Delay until GameLaunched in order to patch other mods....</remarks>
    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            HarmonyMethod transpiler = new(typeof(ModEntry), nameof(Transpiler));
            harmony.Patch(
                original: typeof(Event).GetCachedMethod(nameof(Event.command_minedeath), ReflectionCache.FlagTypes.InstanceFlags),
                transpiler: transpiler);
            harmony.Patch(
                original: typeof(Event).GetCachedMethod(nameof(Event.command_hospitaldeath), ReflectionCache.FlagTypes.InstanceFlags),
                transpiler: transpiler);
            if (this.Helper.ModRegistry.IsLoaded("spacechase0.MoonMisadventures"))
            {
                Type? mmMineDeath = AccessTools.TypeByName("MoonMisadventures.Patches.EventNoMineDeathPersistLossPatch");
                MethodInfo? mmMineDeathImpl = AccessTools.Method(mmMineDeath, "Impl");

                if (mmMineDeathImpl is not null)
                {
                    harmony.Patch(
                        original: mmMineDeathImpl,
                        transpiler: transpiler);
                }
                else
                {
                    this.Monitor.Log("Attempt to patch MoonMisadventures for compat failed, this mod will not work.", LogLevel.Error);
                }

                Type? mmHospitalDeath = AccessTools.TypeByName("MoonMisadventures.Patches.EventNoHospitalDeathPersistLossPatch");
                MethodInfo? mmHospitalDeathImpl = AccessTools.Method(mmHospitalDeath, "Impl");

                if (mmHospitalDeathImpl is not null)
                {
                    harmony.Patch(
                        original: mmHospitalDeathImpl,
                        transpiler: transpiler);
                }
                else
                {
                    this.Monitor.Log("Attempt to patch MoonMisadventures for compat failed, this mod will not work.", LogLevel.Error);
                }
            }
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Reviewed.")]
    private static bool ProhibitLosingThisItem(Item item)
        => item is Wand || (item is SObject obj && obj.ParentSheetIndex == 911 && !obj.bigCraftable.Value)
        || item.HasContextTag("atravita_no_loss_on_death") || (item is MeleeWeapon weapon && weapon.isScythe());

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Call, typeof(Game1).GetCachedProperty(nameof(Game1.player), ReflectionCache.FlagTypes.StaticFlags).GetGetMethod()),
                new(OpCodes.Callvirt),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Callvirt, typeof(IList<Item>).GetCachedProperty("Item", ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Isinst, typeof(MeleeWeapon)),
                new(OpCodes.Callvirt),
                new(OpCodes.Ldc_I4_S, 47),
                new(SpecialCodeInstructionCases.Wildcard, (inst) => inst.opcode == OpCodes.Beq || inst.opcode == OpCodes.Beq_S ),
            });

            int startindex = helper.Pointer;

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(SpecialCodeInstructionCases.Wildcard, (inst) => inst.opcode == OpCodes.Beq || inst.opcode == OpCodes.Beq_S ),
            })
            .StoreBranchDest()
            .Push()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label label)
            .Pop();

            int endindex = helper.Pointer;

            List<CodeInstruction>? copylist = new(endindex - startindex);
            foreach (CodeInstruction? code in helper.Codes.GetRange(startindex, endindex - startindex - 3))
            {
                copylist.Add(code.Clone());
            }
            copylist.Add(new(OpCodes.Call, typeof(ModEntry).GetCachedMethod(nameof(ProhibitLosingThisItem), ReflectionCache.FlagTypes.StaticFlags)));
            copylist.Add(new(OpCodes.Brtrue_S, label));
            CodeInstruction[]? copy = copylist.ToArray();

            helper.Advance(1)
            .Insert(copy);

            return helper.Render();
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Mod crashed while transpiling mine death methods:\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
