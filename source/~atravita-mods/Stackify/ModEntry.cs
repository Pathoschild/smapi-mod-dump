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

using Stackify.Framework;

using StardewValley.Objects;

using AtraUtils = AtraShared.Utils.Utils;

namespace Stackify;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    /// <summary>
    /// Gets the logger for this mod.
    /// </summary>
    internal static IMonitor ModMonitor { get; private set; } = null!;

    /// <summary>
    /// Gets the config instance for this mod.
    /// </summary>
    internal static ModConfig Config { get; private set; } = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        ModMonitor = this.Monitor;
        Config = AtraUtils.GetConfigOrDefault<ModConfig>(helper, this.Monitor);

        this.Monitor.Log($"Starting up: {this.ModManifest.UniqueID} - {typeof(ModEntry).Assembly.FullName}");
    }

    private static bool ShouldStackForQuality(SObject self, SObject other)
    {
        if (self.Quality == other.Quality)
        {
            return true;
        }
        return Config.QualityStackBind.IsDown();
    }

    private static bool ShouldStackForColor(SObject self, SObject other)
    {
        ItemTypeEnum selfType = self.GetItemType();
        ItemTypeEnum otherType = other.GetItemType();

        if (!selfType.HasFlag(ItemTypeEnum.SObject) || !otherType.HasFlag(ItemTypeEnum.SObject))
        {
            return false;
        }

        // if only one of is colored, or both of us are colored and colors don't match.
        if ((selfType.HasFlag(ItemTypeEnum.ColoredSObject) != otherType.HasFlag(ItemTypeEnum.ColoredSObject))
            || (self is ColoredObject colorSelf && other is ColoredObject coloredOther
                && colorSelf.color.Value != coloredOther.color.Value))
        {
            return Config.ColorStackBind.IsDown();
        }
        return true;
    }

    private static IEnumerable<CodeInstruction>? Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen, MethodBase original)
    {
        try
        {
            ILHelper helper = new(original, instructions, ModEntry.ModMonitor, gen);
            helper.FindNext(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Isinst, typeof(ColoredObject)),
                (OpCodes.Ldfld, typeof(ColoredObject).GetCachedField(nameof(ColoredObject.color), ReflectionCache.FlagTypes.InstanceFlags)),
            })
            .FindPrev(new CodeInstructionWrapper[]
            {
                OpCodes.Ldarg_0,
                (OpCodes.Isinst, typeof(ColoredObject)),
            });
            helper.Print();
            return helper.Render();
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Ran into errors transpiling {original.FullDescription()}.\n\n{ex}", LogLevel.Error);
            original?.Snitch(ModEntry.ModMonitor);
        }
        return null;
    }
}
