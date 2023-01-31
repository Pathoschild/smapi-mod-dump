/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Collections.Concurrent;

using AtraShared.Niceties;

using Microsoft.Xna.Framework.Graphics;

namespace GrowableGiantCrops.Framework;

/// <summary>
/// Handles caching and invalidating assets for this mod.
/// </summary>
internal static class AssetCache
{
    private static readonly ConcurrentDictionary<string, WeakReference<AssetHolder>> Cache = new();
    private static readonly HashSet<IAssetName> Failed = new(BaseAssetNameComparer.Instance);

    private static IGameContentHelper gameContent = null!;

    internal static void Initialize(IGameContentHelper gameContentHelper)
    {
        gameContent = gameContentHelper;
    }

    internal static AssetHolder? Get(string key)
    {
        IAssetName parsed = gameContent.ParseAssetName(key);
        return Get(parsed);
    }

    internal static AssetHolder? Get(IAssetName parsed)
    {
        if (Cache.TryGetValue(parsed.BaseName, out WeakReference<AssetHolder>? weakref) && weakref.TryGetTarget(out AssetHolder? holder))
        {
            return holder;
        }

        if (Failed.Contains(parsed))
        {
            return null;
        }

        try
        {
            Texture2D texture = gameContent.Load<Texture2D>(parsed);
            if (!texture.IsDisposed)
            {
                AssetHolder newHolder = new(parsed, texture);
                Cache[parsed.BaseName] = new (newHolder);
                return newHolder;
            }
        }
        catch (Exception ex)
        {
            Failed.Add(parsed);
            ModEntry.ModMonitor.LogOnce($"Failed to load {parsed}.", LogLevel.Error);
            ModEntry.ModMonitor.Log(ex.ToString());
        }

        return null;
    }

    internal static void Refresh(IReadOnlySet<IAssetName>? assets)
    {
        if (assets is null)
        {
            foreach ((string? key, WeakReference<AssetHolder>? holder) in Cache)
            {
                if (!holder.TryGetTarget(out AssetHolder? target))
                {
                    Cache.TryRemove(key, out _);
                }
            }

            Failed.Clear();
        }
        else
        {
            foreach (IAssetName asset in assets)
            {
                if (Cache.TryGetValue(asset.BaseName, out WeakReference<AssetHolder>? holder) && !holder.TryGetTarget(out _))
                {
                    Cache.TryRemove(asset.BaseName, out WeakReference<AssetHolder> _);
                }
                Failed.Remove(asset);
            }
        }
    }

    internal static void Ready(IAssetName asset)
    {
        if (Cache.TryGetValue(asset.BaseName, out WeakReference<AssetHolder>? holder))
        {
            if (holder.TryGetTarget(out AssetHolder? target))
            {
                target.Refresh();
            }
            else
            {
                Cache.TryRemove(asset.BaseName, out WeakReference<AssetHolder> _);
            }
        }
        Failed.Remove(asset);
    }
}
