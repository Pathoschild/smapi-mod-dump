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
using AtraCore.Framework.ReflectionManager;
using CommunityToolkit.Diagnostics;

namespace AtraCore.Utilities;

/// <summary>
/// Functions to help with handling multi-player.
/// </summary>
public static class MultiplayerHelpers
{
    private static readonly Lazy<Func<Multiplayer>> MultiplayerLazy = new(
        () => typeof(Game1).GetCachedField("multiplayer", ReflectionCache.FlagTypes.StaticFlags)
                                      .GetStaticFieldGetter<Multiplayer>());

    /// <summary>
    /// Gets a function that returns the current multi-player instance.
    /// </summary>
    public static Func<Multiplayer> GetMultiplayer => MultiplayerLazy.Value;

    /// <summary>
    /// Checks if the versions installed of the mod are the same for farmhands.
    /// Prints errors to console if wrong.
    /// </summary>
    /// <param name="multi">Multi-player helper.</param>
    /// <param name="manifest">Manifest of mod.</param>
    /// <param name="monitor">Logger.</param>
    /// <param name="translation">Translation helper.</param>
    /// <returns>Whether mod version matches or not.</returns>
    public static bool AssertMultiplayerVersions(IMultiplayerHelper multi, IManifest manifest, IMonitor monitor, ITranslationHelper translation)
    {
        Guard.IsNotNull(multi);
        Guard.IsNotNull(manifest);
        Guard.IsNotNull(monitor);
        Guard.IsNotNull(translation);

        if (Context.IsMultiplayer && !Context.IsMainPlayer && !Context.IsSplitScreen)
        {
            if (multi.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID)?.GetMod(manifest.UniqueID) is not IMultiplayerPeerMod hostMod)
            {
                monitor.Log(
                    translation.Get("host-not-installed")
                        .Default("The host does not seem to have this mod installed. Some features may not be available."),
                    LogLevel.Warn);
                PlayerAlertHandler.AddMessage(new HUDMessage($"Mismatched mods {manifest.UniqueID} may cause issues", HUDMessage.error_type));
                return false;
            }
            else if (!hostMod.Version.Equals(manifest.Version))
            {
                monitor.Log(
                    translation.Get("host-version-different")
                        .Default("The host seems to have a different version of this mod ({{version}}). Some features may not work.")
                        .Tokens(new { version = manifest.Version }),
                    LogLevel.Warn);
                PlayerAlertHandler.AddMessage(new HUDMessage($"Mismatched mod version {manifest.UniqueID} may cause issues", HUDMessage.error_type));
                return false;
            }
        }

        return true;
    }

    public static bool CheckMultiplayerPeer(IMultiplayerPeer peer, IManifest manifest, IMonitor monitor, ITranslationHelper translation)
    {
        Guard.IsNotNull(peer);
        Guard.IsNotNull(manifest);
        Guard.IsNotNull(monitor);
        Guard.IsNotNull(translation);

        if (peer.GetMod(manifest.UniqueID) is not IMultiplayerPeerMod otherMod)
        {
            monitor.Log(
                translation.Get("peer-not-installed")
                    .Default("{{peer}} does not seem to have this mod installed. This will probably cause issues.")
                    .Tokens(new { peer = peer.PlayerID } ),
                LogLevel.Warn);
            PlayerAlertHandler.AddMessage(new HUDMessage($"Mismatched mods {manifest.UniqueID} may cause issues", HUDMessage.error_type));
            return false;
        }
        else if (!otherMod.Version.Equals(manifest.Version))
        {
            monitor.Log(
                translation.Get("peer-version-different")
                        .Default("Peer {{peer}} seems to have a different version of this mod ({{version}}). This will probably cause issues.")
                        .Tokens(new { peer = peer.PlayerID, version = manifest.Version }),
                LogLevel.Warn);
            PlayerAlertHandler.AddMessage(new HUDMessage($"Mismatched mod version {manifest.UniqueID} may cause issues", HUDMessage.error_type));
            return false;
        }
        return true;
    }
}