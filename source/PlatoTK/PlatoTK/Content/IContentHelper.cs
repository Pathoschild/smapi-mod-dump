/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using System;
using System.Collections.Generic;

namespace PlatoTK.Content
{
    public interface IContentHelper
    {
        IInjectionHelper Injections { get; }
        ITextureHelper Textures { get; }

        ISaveIndex GetSaveIndex(string id,
            Func<IDictionary<int, string>> loadData,
            Func<ISaveIndexHandle, bool> validateValue,
            Action<ISaveIndexHandle> injectValue,
            int minIndex = 13000);

        ISaveIndex GetSaveIndex(string id,
            string dataSource,
            Func<ISaveIndexHandle, bool> validateValue,
            Action<ISaveIndexHandle> injectValue,
            int minIndex = 13000);

        ISaveIndex GetSaveIndex(string id,
            IPlatoHelper helper,
            int minIndex = 13000);

        IMapHelper Maps { get; }
    }
}
