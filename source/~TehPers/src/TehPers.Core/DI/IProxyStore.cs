/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Ninject;
using Ninject.Components;
using Ninject.Planning.Bindings;

namespace TehPers.Core.DI
{
    public interface IProxyStore : INinjectComponent
    {
        void AddProxy(IBinding binding, IKernel parent);

        IEnumerable<ProxyBinding> GetProxies(Type type);

        void RemoveProxies(Type service, Func<ProxyBinding, bool> predicate);
    }
}