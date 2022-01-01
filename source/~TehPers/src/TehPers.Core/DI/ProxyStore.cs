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
using System.Linq;
using Ninject;
using Ninject.Components;
using Ninject.Infrastructure;
using Ninject.Planning.Bindings;

namespace TehPers.Core.DI
{
    public class ProxyStore : NinjectComponent, IProxyStore
    {
        private readonly Multimap<Type, ProxyBinding> proxies;

        public ProxyStore()
        {
            this.proxies = new Multimap<Type, ProxyBinding>();
        }

        public void AddProxy(IBinding binding, IKernel parent)
        {
            _ = parent ?? throw new ArgumentNullException(nameof(parent));
            _ = binding ?? throw new ArgumentNullException(nameof(binding));

            var proxyBinding = new ProxyBinding(binding, parent);
            lock (this.proxies)
            {
                this.proxies.Add(binding.Service, proxyBinding);
            }
        }

        public void RemoveProxies(Type service, Func<ProxyBinding, bool> predicate)
        {
            _ = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _ = service ?? throw new ArgumentNullException(nameof(service));

            lock (this.proxies)
            {
                var serviceProxies = this.proxies[service];

                foreach (var proxy in serviceProxies.ToArray())
                {
                    if (predicate(proxy))
                    {
                        serviceProxies.Remove(proxy);
                    }
                }
            }
        }

        public IEnumerable<ProxyBinding> GetProxies(Type type)
        {
            lock (this.proxies)
            {
                return this.proxies[type];
            }
        }
    }
}