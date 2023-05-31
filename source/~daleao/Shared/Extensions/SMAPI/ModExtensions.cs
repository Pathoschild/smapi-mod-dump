/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions.SMAPI;

/// <summary>Extensions for the <see cref="Mod"/> class.</summary>
public static class ModExtensions
{
    public static void ValidateMultiplayer(this Mod mod)
    {
        if (!Context.IsMultiplayer || Context.IsOnHostComputer)
        {
            return;
        }

        var host = mod.Helper.Multiplayer.GetConnectedPlayer(Game1.MasterPlayer.UniqueMultiplayerID)!;
        var uniqueId = mod.ModManifest.UniqueID;
        var thisVersion = mod.ModManifest.Version;
        var hostMod = host.GetMod(uniqueId);
        if (hostMod is null)
        {
            Log.W(
                $"{uniqueId} was not installed by the session host. " +
                "Mod features may not work correctly.");
        }
        else
        {
            var hostVersion = hostMod.Version;
            if (!thisVersion.Equals(hostVersion))
            {
                Log.W(
                    $"The session host has a different version of {uniqueId} installed. " +
                    "Mod features may not work correctly." +
                    $"\n\tHost version: {hostMod.Version}" +
                    $"\n\tLocal version: {thisVersion}");
            }
        }
    }
}
