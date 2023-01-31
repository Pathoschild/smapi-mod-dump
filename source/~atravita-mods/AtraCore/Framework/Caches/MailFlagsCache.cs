/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Netcode;

namespace AtraCore.Framework.Caches;
internal static class MailFlagsCache
{
    private static List<WeakReference<NetList<string, NetString>>> listsToFree = new();

    /// <summary>
    /// Gets a hashset of the current player's mail flags.
    /// </summary>
    internal static HashSet<string> CurrentPlayerMailFlags { get; } = new();

    internal static HashSet<string>? HostPlayerMailFlags { get; private set; } = null;

    internal static HashSet<string>? AllPlayersMailFlags { get; private set; } = null;

    internal static void Watch()
    {
        if (Game1.getAllFarmhands().Any())
        {
            // argh watcher code.
        }
        else
        {
            HostPlayerMailFlags = CurrentPlayerMailFlags;
            AllPlayersMailFlags = CurrentPlayerMailFlags;
        }

        Game1.player.mailReceived.OnElementChanged += ThisPlayerWatcher;
    }

    internal static void Unwatch()
    {
        foreach(WeakReference<NetList<string, NetString>> reference in listsToFree)
        {
            if (reference.TryGetTarget(out NetList<string, NetString>? target))
            {
                target.OnElementChanged -= ThisPlayerWatcher;
            }
        }
    }

    private static void ThisPlayerWatcher(NetList<string, NetString> list, int index, string oldValue, string newValue)
    {
        if (!string.IsNullOrWhiteSpace(oldValue))
        {
            CurrentPlayerMailFlags.Remove(oldValue);
        }
        if (!string.IsNullOrWhiteSpace(newValue))
        {
            CurrentPlayerMailFlags.Add(newValue);
        }
    }
}
