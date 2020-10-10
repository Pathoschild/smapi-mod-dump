/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;

namespace Entoarox.Framework
{
    public interface IContentHandler
    {
        bool IsLoader { get; }
        bool IsInjector { get; }
        bool CanInject<T>(string assetName);
        bool CanLoad<T>(string assetName);
        void Inject<T>(string assetName, ref T asset);
        T Load<T>(string assetName, Func<string, T> loadBase);
    }
}
