/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Runtime.CompilerServices;

namespace AeroCore.Generics
{
    [ModInit]
    public abstract class LazyAsset
    {
        internal Func<string> getPath;
        internal bool ignoreLocale;

        internal static readonly ConditionalWeakTable<LazyAsset, IModHelper> Watchers = new();
        internal static void Init()
        {
            ModEntry.helper.Events.Content.AssetsInvalidated += CheckWatchers;
        }
        internal static void CheckWatchers(object _, AssetsInvalidatedEventArgs ev)
        {
            foreach ((var asset, var helper) in Watchers) 
            {
                string path = asset.getPath();
                foreach (var name in asset.ignoreLocale ? ev.NamesWithoutLocale : ev.Names)
                {
                    if (name.IsEquivalentTo(path))
                    {
                        asset.Reload(); break;
                    }
                }
            }
        }
        public abstract void Reload();
    }
    public class LazyAsset<T> : LazyAsset
    {
        private readonly IModHelper helper;
        private T cached = default;
        private bool isCached = false;

        public T Value => GetAsset();
        public string LastError { get; private set; } = null;
        public bool CatchErrors { get; set; } = false;
        public event Action<LazyAsset<T>> AssetReloaded;

        public LazyAsset(IModHelper Helper, Func<string> AssetPath, bool IgnoreLocale = true)
        {
            getPath = AssetPath;
            helper = Helper;
            ignoreLocale = IgnoreLocale;

            Watchers.Add(this, Helper);
        }
        public T GetAsset()
        {
            if (!isCached)
            {
                LastError = null;
                isCached = true;
                if (CatchErrors)
                {
                    try
                    {
                        cached = helper.GameContent.Load<T>(getPath());
                    }
                    catch (Exception e)
                    {
                        LastError = e.ToString();
                        cached = default;
                    }
                }
                else
                {
                    cached = helper.GameContent.Load<T>(getPath());
                }
            }
            return cached;
        }
        public override void Reload()
        {
            cached = default;
            isCached = false;
            LastError = null;
            AssetReloaded?.Invoke(this);
        }
    }
}
