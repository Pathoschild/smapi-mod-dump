/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraShared.Caching;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace StopRugRemoval;

/// <summary>
/// Handles editing assets.
/// </summary>
internal static class AssetEditor
{
    private static IAssetName saloonEvents = null!;
    private static IAssetName betIconsPath = null!;
    private static Lazy<Texture2D> betIconLazy = new(static () => Game1.content.Load<Texture2D>(betIconsPath.BaseName));

    private static PerScreen<TickCache<bool>> hasSeenSaloonEvent = new(
        () => new (static () => Game1.player?.eventsSeen?.Contains(40) == true));

    /// <summary>
    /// Gets the bet button textures.
    /// </summary>
    internal static Texture2D BetIcon => betIconLazy.Value;

    /// <summary>
    /// Initializes assets for this mod.
    /// </summary>
    /// <param name="parser">GameContentHelper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        saloonEvents = parser.ParseAssetName("Data/Events/Saloon");
        betIconsPath = parser.ParseAssetName("Mods/atravita_StopRugRemoval_BetIcons");
    }

    /// <summary>
    /// Refreshes lazies.
    /// </summary>
    /// <param name="assets">IReadOnlySet of assets to refresh.</param>
    internal static void Refresh(IReadOnlySet<IAssetName>? assets = null)
    {
        if (betIconLazy.IsValueCreated && (assets is null || assets.Contains(betIconsPath)))
        {
            betIconLazy = new(static () => Game1.content.Load<Texture2D>(betIconsPath.BaseName));
        }
    }

    /// <summary>
    /// Applies edits.
    /// </summary>
    /// <param name="e">Event args.</param>
    /// <param name="directoryPath">The absolute path to the mod.</param>
    internal static void Edit(AssetRequestedEventArgs e, string directoryPath)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }
        if (e.NameWithoutLocale.IsEquivalentTo(betIconsPath))
        { // The BET1k/10k icons have to be localized, so they're in the i18n folder.
            string filename = "BetIcons.png";

            if (Game1.content.GetCurrentLanguage() is not LocalizedContentManager.LanguageCode.en)
            {
                string localeFilename;
                LocalizedContentManager.LanguageCode locale = Game1.content.GetCurrentLanguage();
                if (locale != LocalizedContentManager.LanguageCode.mod)
                {
                    localeFilename = $"BetIcons.{Game1.content.LanguageCodeString(locale)}.png";
                }
                else
                {
                    localeFilename = $"BetIcons.{LocalizedContentManager.CurrentModLanguage.LanguageCode}.png";
                }
                if (File.Exists(Path.Combine(directoryPath, "i18n", localeFilename)))
                {
                    filename = localeFilename;
                }
            }
            e.LoadFromModFile<Texture2D>(Path.Combine("i18n", filename), AssetLoadPriority.Low);
        }
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested"/>
    /// <remarks>
    /// Handles editing the saloon event to give the player a choice about alcohol.
    /// </remarks>
    internal static void EditSaloonEvent(AssetRequestedEventArgs e)
    {
        if (ModEntry.Config.Enabled && ModEntry.Config.EditElliottEvent && !hasSeenSaloonEvent.Value.GetValue() && e.NameWithoutLocale.IsEquivalentTo(saloonEvents))
        {
            e.Edit(EditSaloonImpl, AssetEditPriority.Late);
        }
    }

    /// <inheritdoc cref="AssetRequestedEventArgs.Edit(Action{IAssetData}, AssetEditPriority, string?)"/>
    private static void EditSaloonImpl(IAssetData asset)
    {
        IAssetDataForDictionary<string, string>? editor = asset.AsDictionary<string, string>();

        if (editor.Data.ContainsKey("atravita_elliott_nodrink"))
        {// event has been edited already?
            return;
        }
        foreach ((string key, string value) in editor.Data)
        {
            if (key.StartsWith("40/", StringComparison.OrdinalIgnoreCase))
            {
                int index = value.IndexOf("speak Elliott");
                int second = value.IndexOf("speak Elliott", index + 1);
                int nextslash = value.IndexOf('/', second);
                if (nextslash > -1)
                {
                    string initial = value[..nextslash] + $"/question fork1 \"#{I18n.Drink()}#{I18n.Nondrink()}\"/fork atravita_elliott_nodrink/";
                    string remainder = value[(nextslash + 1)..];

                    editor.Data["atravita_elliott_nodrink"] = remainder.Replace("346", "350");
                    editor.Data[key] = initial + remainder;
                }
                return;
            }
        }
    }
}