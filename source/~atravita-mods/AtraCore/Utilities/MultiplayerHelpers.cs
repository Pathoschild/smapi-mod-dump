/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraBase.Toolkit.Reflection;
using AtraCore.Framework.QueuePlayerAlert;
using Microsoft.Toolkit.Diagnostics;

namespace AtraCore.Utilities;

/// <summary>
/// Functions to help with handling multiplayer.
/// </summary>
public static class MultiplayerHelpers
{
    private static readonly Lazy<Func<Multiplayer>> MultiplayerLazy = new(() => typeof(Game1).StaticFieldNamed("multiplayer").GetStaticFieldGetter<Multiplayer>());

    /// <summary>
    /// Gets a function that returns the current multiplayer instance.
    /// </summary>
    public static Func<Multiplayer> GetMultiplayer => MultiplayerLazy.Value;

    /// <summary>
    /// Checks if the versions installed of the mod are the same for farmhands.
    /// Prints errors to console if wrong.
    /// </summary>
    /// <param name="multi">Multiplayer helper.</param>
    /// <param name="manifest">Manifest of mod.</param>
    /// <param name="monitor">Logger.</param>
    /// <param name="translation">Translation helper.</param>
    public static void AssertMultiplayerVersions(IMultiplayerHelper multi, IManifest manifest, IMonitor monitor, ITranslationHelper translation)
    {
        Guard.IsNotNull(multi, nameof(multi));
        Guard.IsNotNull(manifest, nameof(manifest));
        Guard.IsNotNull(monitor, nameof(monitor));
        Guard.IsNotNull(translation, nameof(translation));

        if (Context.IsMultiplayer && !Context.IsMainPlayer && !Context.IsSplitScreen)
        {
            if (multi.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID)?.GetMod(manifest.UniqueID) is not IMultiplayerPeerMod hostMod)
            {
                monitor.Log(
                    translation.Get("host-not-installed")
                        .Default("The host does not seem to have this mod installed. Some features may not be available."),
                    LogLevel.Warn);
                PlayerAlertHandler.AddMessage(new HUDMessage($"Mismatched mods {manifest.UniqueID} may cause issues", HUDMessage.error_type));
            }
            else if (!hostMod.Version.Equals(manifest.Version))
            {
                monitor.Log(
                    translation.Get("host-version-different")
                        .Default("The host seems to have a different version of this mod ({{version}}). Some features may not work.")
                        .Tokens(new { version = manifest.Version }),
                    LogLevel.Warn);
                PlayerAlertHandler.AddMessage(new HUDMessage($"Mismatched mod version {manifest.UniqueID} may cause issues", HUDMessage.error_type));
            }
        }
    }
}