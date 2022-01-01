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
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;

namespace TehPers.Core.DI
{
    public class ProxyBindingResolver : NinjectComponent, IBindingResolver
    {
        private readonly IProxyStore proxyStore;

        public ProxyBindingResolver(IProxyStore proxyStore)
        {
            this.proxyStore = proxyStore ?? throw new ArgumentNullException(nameof(proxyStore));
        }

        public IEnumerable<IBinding> Resolve(Multimap<Type, IBinding> bindings, Type service)
        {
            return this.proxyStore.GetProxies(service);
        }
    }
}