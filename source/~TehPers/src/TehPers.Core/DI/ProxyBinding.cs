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
using System.Linq;
using Ninject;
using Ninject.Activation;
using Ninject.Activation.Caching;
using Ninject.Activation.Providers;
using Ninject.Infrastructure;
using Ninject.Parameters;
using Ninject.Planning;
using Ninject.Planning.Bindings;

namespace TehPers.Core.DI
{
    public class ProxyBinding : Binding
    {
        public IBinding ParentBinding { get; }

        public IKernel ParentKernel { get; }

        public ProxyBinding(IBinding binding, IKernel parentKernel)
            : base(binding.Service, new ProxyBindingConfiguration(binding.BindingConfiguration))
        {
            this.ParentBinding = binding ?? throw new ArgumentNullException(nameof(binding));
            this.ParentKernel = parentKernel ?? throw new ArgumentNullException(nameof(parentKernel));
            this.Condition = binding.Condition;
            this.ScopeCallback = StandardScopeCallbacks.Transient;
            this.ProviderCallback = this.ResolveParent;
        }

        private IProvider ResolveParent(IContext context)
        {
            var childRequest = context.Request switch
            {
                { Target: { } target } req => req.CreateChild(req.Service, context, target),
                { } req => new Request(req.Service, req.Constraint, Enumerable.Empty<IParameter>(), req.GetScope, req.IsOptional, req.IsUnique),
                _ => throw new InvalidOperationException("Context cannot have a null request"),
            };

            var cache = this.ParentKernel.Components.Get<ICache>();
            var planner = this.ParentKernel.Components.Get<IPlanner>();
            var pipeline = this.ParentKernel.Components.Get<IPipeline>();
            var childContext = new Context(this.ParentKernel, childRequest, this.ParentBinding, cache, planner, pipeline);
            return new CallbackProvider<object>(_ => childContext.Resolve());
        }
    }
}