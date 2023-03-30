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

using StardewValley.Buildings;

namespace MoreItemsOnFishPonds;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    private const string Context = "atravita.AllowOnFishPonds";

    private static IMonitor modMonitor = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        modMonitor = this.Monitor;
        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");
        this.ApplyPatches(new(this.ModManifest.UniqueID));

#if DEBUG
        helper.Events.Content.AssetRequested += static (_, e) =>
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/ObjectContextTags"))
            {
                e.Edit(
                    (asset) =>
                {
                    var data = asset.GetData<Dictionary<string, string>>();
                    if (data.TryGetValue("Big Green Cane", out string? str))
                    {
                        data["Big Green Cane"] = str + ", " + Context;
                    }
                    else
                    {
                        data["Big Green Cane"] = Context;
                    }
                }, StardewModdingAPI.Events.AssetEditPriority.Late);
            }
        };
#endif
    }

    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            harmony.Patch(
                original: typeof(FishPond).GetCachedMethod(nameof(FishPond.performActiveObjectDropInAction), ReflectionCache.FlagTypes.InstanceFlags),
                transpiler: new(typeof(ModEntry).GetCachedMethod(nameof(Transpiler), ReflectionCache.FlagTypes.StaticFlags)));
        }
        catch (Exception ex)
        {
            modMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:Split parameters should start on line after declaration", Justification = "Reviewed.")]
    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, modMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            { // who.ActiveObject.Name.Contains.("Sign")
                OpCodes.Ldarg_1,
                (OpCodes.Callvirt, typeof(Farmer).GetCachedProperty(nameof(Farmer.ActiveObject), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                (OpCodes.Callvirt, typeof(Item).GetCachedProperty(nameof(Item.Name), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                (OpCodes.Ldstr, "Sign"),
                (OpCodes.Callvirt, typeof(string).GetCachedMethod<string>(nameof(string.Contains), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Brfalse,
            })
            .Push()
            .Advance(6)
            .DefineAndAttachLabel(out Label jumppoint)
            .Pop()
            .GetLabels(out IList<Label>? labels)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, typeof(Farmer).GetCachedProperty(nameof(Farmer.ActiveObject), ReflectionCache.FlagTypes.InstanceFlags).GetGetMethod()),
                new(OpCodes.Ldstr, Context),
                new(OpCodes.Callvirt, typeof(Item).GetCachedMethod<string>(nameof(Item.HasContextTag), ReflectionCache.FlagTypes.InstanceFlags)),
                new(OpCodes.Brtrue, jumppoint),
            }, withLabels: labels);

            // helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            modMonitor.Log($"Ran into error transpiling {original.FullDescription()}\n\n{ex}", LogLevel.Error);
            original.Snitch(modMonitor);
        }
        return null;
    }
}