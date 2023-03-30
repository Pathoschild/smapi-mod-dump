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
using AtraBase.Toolkit.Reflection;

using AtraCore.Framework.ReflectionManager;

using AtraShared.ConstantsAndEnums;
using AtraShared.Utils;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.HarmonyHelper;

using HarmonyLib;

namespace FixPigRandom;

/// <inheritdoc />
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Reviewed.")]
internal sealed class ModEntry : Mod
{
    private static readonly Dictionary<long, Random> Cache = new();

    private static IMonitor modMonitor = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        modMonitor = this.Monitor;
        helper.Events.GameLoop.DayEnding += static (_, _) => Cache.Clear();
        helper.Events.GameLoop.GameLaunched += (_, _) => this.ApplyPatches(new(this.ModManifest.UniqueID));

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");
    }

    private void ApplyPatches(Harmony harmony)
    {
        try
        {
            harmony.Patch(
                original: typeof(FarmAnimal).GetCachedMethod("findTruffle", ReflectionCache.FlagTypes.InstanceFlags),
                transpiler: new(typeof(ModEntry).GetCachedMethod(nameof(Transpiler), ReflectionCache.FlagTypes.StaticFlags)));

            if (this.Helper.ModRegistry.Get("Paritee.BetterFarmAnimalVariety") is IModInfo bfav
                && bfav.Manifest.Version.IsNewerThan("3.2.3"))
            {
                this.Monitor.Log("Patching bfav for compat", LogLevel.Info);

                Type? patch = AccessTools.TypeByName("BetterFarmAnimalVariety.Framework.Patches.FarmAnimal.FindTruffle");
                if (patch is not null)
                {
                    harmony.Patch(
                        original: patch.StaticMethodNamed("ShouldStopFindingProduce"),
                        transpiler: new(typeof(ModEntry).GetCachedMethod(nameof(BFAVTranspiler), ReflectionCache.FlagTypes.StaticFlags)));
                }
                else
                {
                    this.Monitor.Log("BFAV could not be patched for compat, this mod will probably not work.", LogLevel.Warn);
                }
            }
        }
        catch (Exception ex)
        {
            modMonitor.Log(string.Format(ErrorMessageConsts.HARMONYCRASH, ex), LogLevel.Error);
        }
        harmony.Snitch(this.Monitor, harmony.Id, transpilersOnly: true);
    }

    [MethodImpl(TKConstants.Hot)]
    private static Random GetRandom(FarmAnimal pig)
        => GetRandom(pig.myID.Value);

    [MethodImpl(TKConstants.Hot)]
    private static Random GetRandom(long id)
    {
        try
        {
            if (!Cache.TryGetValue(id, out Random? random))
            {
                unchecked
                {
                    modMonitor.DebugOnlyLog($"Cache miss: {id}", LogLevel.Info);
                    Cache[id] = random = RandomUtils.GetSeededRandom(2, (int)(id >> 1));
                }
            }
            return random;
        }
        catch (Exception ex)
        {
            modMonitor.Log($"Failed while trying to generate random for pig {id}:\n\n{ex}", LogLevel.Error);
        }

        return Game1.random;
    }

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, modMonitor, gen);

            helper.FindNext(new CodeInstructionWrapper[]
            { // find the creation of the random and replace it with our own.
                OpCodes.Ldarg_0,
                (OpCodes.Ldfld, typeof(FarmAnimal).GetCachedField(nameof(FarmAnimal.myID), ReflectionCache.FlagTypes.InstanceFlags)),
                OpCodes.Call, // this is an op_Impl
                OpCodes.Conv_I4,
            })
            .Advance(1)
            .RemoveUntil(new CodeInstructionWrapper[]
            {
                (OpCodes.Callvirt, typeof(Random).GetCachedMethod(nameof(Random.NextDouble), ReflectionCache.FlagTypes.InstanceFlags, Type.EmptyTypes)),
            })
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(ModEntry).GetCachedMethod<FarmAnimal>(nameof(GetRandom), ReflectionCache.FlagTypes.StaticFlags)),
            });

#if DEBUG
            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldc_I4_M1,
                (OpCodes.Callvirt, typeof(Netcode.NetFieldBase<int, Netcode.NetInt>).GetCachedProperty("Value", ReflectionCache.FlagTypes.InstanceFlags).GetSetMethod()),
            })
            .Advance(2)
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Ldsfld, typeof(ModEntry).GetCachedField(nameof(modMonitor), ReflectionCache.FlagTypes.StaticFlags)),
                new(OpCodes.Ldstr, "Truffles Over"),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Callvirt, typeof(IMonitor).GetCachedMethod(nameof(IMonitor.Log), ReflectionCache.FlagTypes.InstanceFlags)),
            });
#endif

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

    private static IEnumerable<CodeInstruction>? BFAVTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, modMonitor, gen);
            Type farmAnimal = AccessTools.TypeByName("BetterFarmAnimalVariety.Framework.Decorators.FarmAnimal")
                                ?? ReflectionThrowHelper.ThrowMethodNotFoundException<Type>("BFAV farm animal");

            helper.FindNext(new CodeInstructionWrapper[]
            { // find the creation of the random and replace it with our own.
                OpCodes.Ldarg_0,
                OpCodes.Ldind_Ref,
                (OpCodes.Callvirt, farmAnimal.GetCachedMethod("GetUniqueId", ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .Advance(3)
            .RemoveUntil(new CodeInstructionWrapper[]
            {
                (OpCodes.Callvirt, typeof(Random).GetCachedMethod(nameof(Random.NextDouble), ReflectionCache.FlagTypes.InstanceFlags, Type.EmptyTypes)),
            })
            .Insert(new CodeInstruction[]
            {
                new(OpCodes.Call, typeof(ModEntry).GetCachedMethod<long>(nameof(GetRandom), ReflectionCache.FlagTypes.StaticFlags)),
            });

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