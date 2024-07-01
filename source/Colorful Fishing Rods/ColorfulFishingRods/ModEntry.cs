/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/6480k/colorful-fishing-rods
**
*************************************************/

namespace ColorfulFishingRods;

using ColorfulFishingRods.Framework;

using HarmonyLib;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using StardewModdingAPI.Events;

using StardewValley.Tools;

/// <inheritdoc />
[HarmonyPatch(typeof(FishingRod))]
[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names should not begin with underscore", Justification = "Preference.")]
internal sealed class ModEntry : Mod
{
    private static Dictionary<string, Color>? _cache = null;
    private static IAssetName colorAsset = null!;

    private static int errorDelay = 0;

    private static IMonitor modMonitor = null!;

    private static ModConfig config = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        modMonitor = this.Monitor; // assign monitor to a static field so we can use it.
        colorAsset = helper.GameContent.ParseAssetName("Mods/ColorfulFishingRods");
        try
        {
            config = helper.ReadConfig<ModConfig>();
        }
        catch (Exception ex)
        {
            this.Monitor.Log($"Failed to read config, using default: {ex}.", LogLevel.Warn);
            config = new ();
        }

        helper.Events.Content.AssetRequested += this.OnAssetRequested;
        helper.Events.Content.AssetsInvalidated += this.OnAssetInvalidated;

        new Harmony(this.ModManifest.UniqueID).PatchAll(typeof(ModEntry).Assembly); // prefer this overload, the other one breaks sometimes.
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(colorAsset))
        {
            e.LoadFrom(static () => new Dictionary<string, Color>(), AssetLoadPriority.Exclusive);
        }
    }

    private void OnAssetInvalidated(object? sender, AssetsInvalidatedEventArgs e)
    {
        if (e.NamesWithoutLocale.Contains(colorAsset))
        {
            _cache = null;
        }
    }

    [HarmonyPatch(nameof(FishingRod.getColor))]
    private static void Postfix(FishingRod __instance, ref Color __result)
    {
        try
        {
            if (_cache is null)
            {
                if (errorDelay > 0)
                {
                    --errorDelay;
                    return;
                }
                _cache = Game1.content.Load<Dictionary<string, Color>>(colorAsset.BaseName);
            }
        }
        catch (ContentLoadException)
        {
            modMonitor.Log($"Failed to load asset {colorAsset.BaseName}.", LogLevel.Error);
            errorDelay = 30_000;
            return;
        }
        catch (Exception ex)
        {
            modMonitor.Log($"Error overriding fishing rod colors: {ex}", LogLevel.Error);
            return;
        }

        if (config.Map.TryGetValue(__instance.QualifiedItemId, out Color color) || _cache.TryGetValue(__instance.QualifiedItemId, out color))
        {
            __result = color;
        }
    }
}