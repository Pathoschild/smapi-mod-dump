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
using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Planning.Bindings.Resolvers;
using TehPers.Core.Api.DI;

namespace TehPers.Core.DI
{
    public class GlobalKernel : CoreKernel, IGlobalKernel
    {
        public GlobalKernel(params INinjectModule[] modules)
            : this(new NinjectSettings(), modules)
        {
        }

        public GlobalKernel(INinjectSettings settings, params INinjectModule[] modules)
            : base(settings, modules)
        {
        }

        protected override void AddComponents()
        {
            base.AddComponents();
            this.Components.Add<IProxyStore, ProxyStore>();
            this.Components.Add<IBindingResolver, ProxyBindingResolver>();
        }

        public void Proxy(IBinding binding, IKernel parent)
        {
            var proxiedBindingStore = this.Components.Get<IProxyStore>();
            proxiedBindingStore.AddProxy(binding, parent);
        }

        public void Unproxy(IBinding binding)
        {
            var proxiedBindingStore = this.Components.Get<IProxyStore>();
            proxiedBindingStore.RemoveProxies(binding.Service, proxy => proxy.ParentBinding == binding);
        }

        public void Unproxy(Type service, IKernel parent)
        {
            var proxiedBindingStore = this.Components.Get<IProxyStore>();
            proxiedBindingStore.RemoveProxies(service, proxy => proxy.ParentKernel == parent);
        }
    }
}