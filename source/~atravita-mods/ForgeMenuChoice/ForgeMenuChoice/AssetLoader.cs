/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.StringHandler;
using AtraShared.Utils.Extensions;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using AtraUtils = AtraShared.Utils.Utils;

namespace ForgeMenuChoice;

/// <summary>
/// Loads and manages assets used by this mod.
/// </summary>
public static class AssetLoader
{
    private const string ASSETPREFIX = "Mods/atravita_ForgeMenuChoice_";
    private static readonly string UiTexturePath = PathUtilities.NormalizePath("assets/Forge-Buttons.png");
    private static IAssetName uiAssetPath = null!;
    private static IAssetName tooltipDataPath = null!;
    private static Lazy<Texture2D> uiElementLazy = new(() => ModEntry.GameContentHelper.Load<Texture2D>(uiAssetPath));
    private static Lazy<Dictionary<string, string>> tooltipDataLazy = new(GrabAndWrapTooltips);

    /// <summary>
    /// Gets the textures for the UI elements used by this mod.
    /// </summary>
    internal static Texture2D UIElement => uiElementLazy.Value;

    /// <summary>
    /// Gets a dictionary for the tooltip data.
    /// </summary>
    internal static Dictionary<string, string> TooltipData => tooltipDataLazy.Value;

    /// <summary>
    /// Gets the location of enchantment names.
    /// </summary>
    internal static string ENCHANTMENT_NAMES_LOCATION { get; } = PathUtilities.NormalizeAssetName("Strings/EnchantmentNames");

    /// <summary>
    /// Initializes the asset manager.
    /// </summary>
    /// <param name="parser">GameContentHelper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        uiAssetPath = parser.ParseAssetName(ASSETPREFIX + "Forge_Buttons");
        tooltipDataPath = parser.ParseAssetName(ASSETPREFIX + "Tooltip_Data");
    }

    /// <summary>
    /// Refreshes the Lazys.
    /// </summary>
    /// <param name="assets">Which assets to refresh? Leave null to refresh all.</param>
    internal static void Refresh(IReadOnlySet<IAssetName>? assets = null)
    {
        if (uiElementLazy.IsValueCreated && (assets is null || assets.Contains(uiAssetPath)))
        {
            uiElementLazy = new(() => ModEntry.GameContentHelper.Load<Texture2D>(uiAssetPath));
        }
        if (tooltipDataLazy.IsValueCreated && (assets is null || assets.Contains(tooltipDataPath)))
        {
            tooltipDataLazy = new(GrabAndWrapTooltips);
        }
    }

    /// <summary>
    /// Handles loading assets from this mod.
    /// </summary>
    /// <param name="e">Event args.</param>
    internal static void OnLoadAsset(AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.Equals(uiAssetPath))
        {
            e.LoadFromModFile<Texture2D>(UiTexturePath, AssetLoadPriority.Low);
        }
        else if (e.NameWithoutLocale.Equals(tooltipDataPath))
        {
            e.LoadFrom(GenerateToolTips, AssetLoadPriority.Low);
        }
    }

    private static Dictionary<string, string> GenerateToolTips()
    {
        Dictionary<string, string> tooltipdata = new();

        // Do nothing if the world is not ready yet.
        // For some reason trying to load the translations too early jacks them up
        // and suddenly enchantments aren't translated in other languages.
        if (!ModEntry.Config.EnableTooltipAutogeneration || !Context.IsWorldReady)
        {
            return tooltipdata;
        }

        // the journal scrap 1008 is the only in-game descriptions of enchantments. We'll need to grab data from there.
        try
        {
            IDictionary<int, string> secretnotes = ModEntry.GameContentHelper.Load<Dictionary<int, string>>(@"Data\SecretNotes");
            StreamSplit secretNote8 = secretnotes[1008].StreamSplit('^', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // The secret note, of course, has its data in the localized name. We'll need to map that to the internal name.
            // Using a dictionary with a StringComparer for the user's current language to make that a little easier.
            Dictionary<string, string> tooltipmap = new(AtraUtils.GetCurrentLanguageComparer(ignoreCase: true));
            foreach (ReadOnlySpan<char> str in secretNote8)
            {
                // Asian languages use a different colon.
                int index = str.IndexOfAny(':', '：');
                if (index > 0)
                {
                    tooltipmap[str[..index].Trim().ToString()] = str[(index + 1)..].Trim().ToString();
                }
            }

            // For each enchantment, look up its description from the secret note and
            // and prepopulate the data file with that.
            // Russian needs to be handled seperately.
            foreach (BaseEnchantment enchantment in BaseEnchantment.GetAvailableEnchantments())
            {
                if (ModEntry.TranslationHelper.TryGetTranslation("enchantment." + enchantment.GetName(), out Translation i18nname))
                {
                    tooltipdata[enchantment.GetName()] = i18nname;
                }
                else if (tooltipmap.TryGetValue(enchantment.GetDisplayName(), out string? val))
                {
                    tooltipdata[enchantment.GetName()] = val;
                }
                else if (Game1.content.GetCurrentLanguage() == LocalizedContentManager.LanguageCode.ru)
                {
                    string[] splits = enchantment.GetDisplayName().Split();
                    foreach (string i in splits)
                    {
                        if (i != "чары" && tooltipmap.TryGetValue(i, out string? value))
                        {
                            tooltipdata[enchantment.GetName()] = value;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Error: journal scrap 9 not found.\n\n{ex}", LogLevel.Error);
        }
        return tooltipdata;
    }

    private static Dictionary<string, string> GrabAndWrapTooltips()
    {
        Dictionary<string, string> tooltips = ModEntry.GameContentHelper.Load<Dictionary<string, string>>(tooltipDataPath);
        foreach ((string k, string v) in tooltips)
        {
            tooltips[k] = ModEntry.StringUtils.ParseAndWrapText(v, Game1.smallFont, 300);
        }
        return tooltips;
    }
}
