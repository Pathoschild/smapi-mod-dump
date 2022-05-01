/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/AvoidLosingSpecter
**
*************************************************/

using System.Reflection;
using System.Reflection.Emit;
using AtraBase.Toolkit.Reflection;
using AtraShared.ConstantsAndEnums;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;
using HarmonyLib;
using StardewValley.Tools;

namespace AvoidLosingScepter;

/// <inheritdoc />
internal class ModEntry : Mod
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
                original: typeof(Event).InstanceMethodNamed(nameof(Event.command_minedeath)),
                transpiler: transpiler);
            harmony.Patch(
                original: typeof(Event).InstanceMethodNamed(nameof(Event.command_hospitaldeath)),
                transpiler: transpiler);
        }
        catch (Exception ex)
        {
            ModMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

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
                new(OpCodes.Call, typeof(Game1).StaticPropertyNamed(nameof(Game1.player)).GetGetMethod()),
                new(OpCodes.Callvirt),
                new(SpecialCodeInstructionCases.LdLoc),
                new(OpCodes.Callvirt, typeof(IList<Item>).InstancePropertyNamed("Item").GetGetMethod()),
                new(OpCodes.Isinst, typeof(MeleeWeapon)),
                new(OpCodes.Callvirt),
                new(OpCodes.Ldc_I4_S, 47),
                new(OpCodes.Beq),
            });

            int startindex = helper.Pointer;

            helper.FindNext(new CodeInstructionWrapper[]
            {
                new(OpCodes.Beq),
            })
            .StoreBranchDest()
            .Push()
            .AdvanceToStoredLabel()
            .DefineAndAttachLabel(out Label label)
            .Pop();

            int endindex = helper.Pointer;

            List<CodeInstruction>? copylist = new();
            foreach (CodeInstruction? code in helper.Codes.GetRange(startindex, endindex - startindex - 3))
            {
                copylist.Add(code.Clone());
            }
            copylist.Add(new(OpCodes.Call, typeof(ModEntry).StaticMethodNamed(nameof(ProhibitLosingThisItem))));
            copylist.Add(new(OpCodes.Brtrue_S, label));
            CodeInstruction[]? copy = copylist.ToArray();

            helper.Advance(1)
            .Insert(copy);

            return helper.Render();
        }
        catch (Exception ex)
        {
            ModMonitor.Log($"Mod crashed while transpiling mine death methods:\n\n{ex}", LogLevel.Error);
        }
        return null;
    }
}
