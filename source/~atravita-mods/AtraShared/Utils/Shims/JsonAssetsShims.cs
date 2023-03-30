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
using System.Text;

using AtraBase.Toolkit;
using AtraBase.Toolkit.Extensions;
using AtraBase.Toolkit.Reflection;
using AtraBase.Toolkit.StringHandler;

using AtraCore.Framework.ReflectionManager;

using AtraShared.Integrations;
using AtraShared.Integrations.Interfaces;
using AtraShared.Utils.Extensions;
using AtraShared.Utils.Shims.JAInternalTypesShims;

using CommunityToolkit.Diagnostics;
using FastExpressionCompiler.LightExpression;
using HarmonyLib;

namespace AtraShared.Utils.Shims;

/// <summary>
/// Holds shims against ja.
/// </summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Fields kept near accessors.")]
public static class JsonAssetsShims
{
    private const int EventID = int.MinValue + 4993;
    private static bool initialized = false;

    private static IMonitor modMonitor = null!;

    #region APIs
    private static IJsonAssetsAPI? jsonAssets;

    /// <summary>
    /// Gets the JA API, if available.
    /// </summary>
    internal static IJsonAssetsAPI? JsonAssets => jsonAssets;

    private static IEPUConditionsChecker? epu;

    /// <summary>
    /// Gets the EPU API, if available.
    /// </summary>
    internal static IEPUConditionsChecker? EPU => epu;
    #endregion

    /// <summary>
    /// Initializes the shims.
    /// </summary>
    /// <param name="monitor">modMonitor instance.</param>
    /// <param name="translation">A translation instance.</param>
    /// <param name="registry">Registry instance.</param>
    public static void Initialize(IMonitor monitor, ITranslationHelper translation, IModRegistry registry)
    {
        if (initialized)
        {
            return;
        }

        Guard.IsNotNull(monitor);
        Guard.IsNotNull(translation);
        Guard.IsNotNull(registry);

        modMonitor = monitor;

        IntegrationHelper integrationHelper = new(monitor, translation, registry, LogLevel.Trace);
        if (integrationHelper.TryGetAPI("spacechase0.JsonAssets", "1.10.6", out jsonAssets)
            && !integrationHelper.TryGetAPI("Cherry.ExpandedPreconditionsUtility", "1.0.1", out epu))
        {
            monitor.Log("ja found but EPU not. EPU conditions will automatically fail.", LogLevel.Info);
        }
        epu?.Initialize(false, registry.ModID);

        initialized = true;
    }

    /// <summary>
    /// Checks to see if an event precondition requires EPU. A condition requires EPU if it starts with ! or is longer than two letters.
    /// </summary>
    /// <param name="condition">Condition to check.</param>
    /// <returns>True if EPU is required.</returns>
    public static bool ConditionRequiresEPU(ReadOnlySpan<char> condition)
        => condition[0] == '!' || condition.GetIndexOfWhiteSpace() > 3;

    /// <summary>
    /// Gets whether or not a JA seed can be sold, looking at the condition string.
    /// Uses EPU if installed.
    /// </summary>
    /// <param name="name">Name of the seed.</param>
    /// <returns>True if is currently available, false otherwise.</returns>
    public static bool IsAvailableSeed(string name)
    {
        Guard.IsNotNullOrWhiteSpace(name);
        if(JACropCache?.TryGetValue(name, out string? conditions) != true)
        {
            return false;
        }
        if(string.IsNullOrWhiteSpace(conditions))
        {
            return true;
        }
        if(epu is not null)
        {
            return epu.CheckConditions(conditions);
        }
        Farm farm = Game1.getFarm();
        bool replace = Game1.player.eventsSeen.Remove(EventID);
        bool ret = farm.checkEventPrecondition($"{EventID}/{conditions}") != -1;
        if (replace)
        {
            Game1.player.eventsSeen.Add(EventID);
        }
        return ret;
    }

    private static Lazy<Dictionary<string, string>?> jaCropCache = new(SetUpJAIntegration);

    /// <summary>
    /// Gets a name->preconditions map of JA crops, or null if JA was not installed/reflection failed.
    /// </summary>
    public static Dictionary<string, string>? JACropCache => jaCropCache.Value;

    private static Dictionary<string, string>? SetUpJAIntegration()
    {
        Type? ja = AccessTools.TypeByName("JsonAssets.Mod");
        if (ja is null)
        {
            return null;
        }

        object? inst = ja.StaticFieldNamed("instance").GetValue(null);
        object? cropdata = ja.InstanceFieldNamed("Crops").GetValue(inst)!;

        if (cropdata is not IReadOnlyList<object> cropList)
        {
            return null;
        }

        try
        {
            Dictionary<string, string> ret = new(cropList.Count);

            foreach (object? crop in cropList)
            {
                if (crop is null)
                {
                    continue;
                }

                string? name = CropDataShims.GetSeedName!(crop);
                if (name is null)
                {
                    continue;
                }

                int price = CropDataShims.GetSeedPurchase!(crop);
                if (price <= 0)
                {
                    // not purchaseable, as far as I can tell.
                    continue;
                }

                IList<string>? requirements = CropDataShims.GetSeedRestrictions!(crop);
                if (requirements is null || requirements.Count == 0)
                {
                    ret[name!] = string.Empty; // no conditions
                    continue;
                }

                StringBuilder sb = StringBuilderCache.Acquire(64);

                foreach (string? requirement in requirements)
                {
                    if (requirement is not null)
                    {
                        foreach (SpanSplitEntry req in requirement.StreamSplit('/'))
                        {
                            if (ConditionRequiresEPU(req) && EPU is null)
                            {
                                modMonitor.Log($"{req} requires EPU, which is not installed", LogLevel.Warn);
                                sb.Clear();
                                StringBuilderCache.Release(sb);
                                goto breakcontinue;
                            }

                            sb.Append(req.Word).Append('/');
                        }
                    }
                }

                if (sb.Length > 0)
                {
                    ret[name!] = sb.ToString(0, sb.Length - 1);
                    modMonitor.DebugOnlyLog($"{name!} - {ret[name!]}");
                }

                StringBuilderCache.Release(sb);
breakcontinue:
                ;
            }

            return ret;
        }
        catch (Exception ex)
        {
            modMonitor.Log($"Something appears to have gone wrong with JA integration:", LogLevel.Error);
            modMonitor.Log(ex.ToString());
            return null;
        }
    }

    #region methods

    private static readonly Lazy<Func<bool>?> isJAInitialized = new(() =>
    {
        Type? ja = AccessTools.TypeByName("JsonAssets.Mod");
        if (ja is null)
        {
            return null;
        }

        MemberExpression? inst = Expression.Field(null, ja.GetCachedField("instance", ReflectionCache.FlagTypes.StaticFlags));
        MemberExpression? isInit = Expression.Field(inst, ja.GetCachedField("DidInit", ReflectionCache.FlagTypes.InstanceFlags));

        return Expression.Lambda<Func<bool>>(isInit).CompileFast();
    });

    /// <summary>
    /// Gets a delegate that checks whether JA is initialized or not.
    /// </summary>
    public static Func<bool>? IsJaInitialized => isJAInitialized.Value;

    #endregion
}
