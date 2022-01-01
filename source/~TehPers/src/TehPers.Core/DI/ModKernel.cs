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
using System.Reflection;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Introspection;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Syntax;
using StardewModdingAPI;
using TehPers.Core.Api.DI;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.DI
{
    internal class ModKernel : CoreKernel, IModKernel
    {
        private readonly GlobalKernel globalKernel;

        public IMod ParentMod { get; }

        public IGlobalKernel GlobalKernel => this.globalKernel;

        public IBindingRoot GlobalProxyRoot { get; }

        public IModKernelFactory ParentFactory { get; }

        public ModKernel(
            IMod parentMod,
            GlobalKernel globalKernel,
            IModKernelFactory parentFactory,
            INinjectSettings settings,
            params INinjectModule[] modules
        )
            : base(settings, modules)
        {
            this.ParentMod = parentMod;
            this.ParentFactory = parentFactory;
            this.globalKernel = globalKernel;
            this.GlobalProxyRoot = new ProxiedBindingRoot(globalKernel, this);
        }

        public override bool CanResolve(IRequest request)
        {
            if (request.Parameters.OfType<GlobalParameter>().Any())
            {
                return this.globalKernel.CanResolve(request);
            }

            return base.CanResolve(request);
        }

        public override bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            if (request.Parameters.OfType<GlobalParameter>().Any())
            {
                return this.globalKernel.CanResolve(request, ignoreImplicitBindings);
            }

            return base.CanResolve(request, ignoreImplicitBindings);
        }

        public override IEnumerable<object> Resolve(IRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            if (ModKernel.ShouldInherit(request))
            {
                return this.globalKernel.Resolve(request);
            }

            return base.Resolve(request);
        }

        private bool IsProxyToThis(IBinding binding)
        {
            while (binding is ProxyBinding proxy)
            {
                if (proxy.ParentKernel == this)
                {
                    return true;
                }

                binding = proxy.ParentBinding;
            }

            return false;
        }

        protected override IEnumerable<TService> Resolve<TService>(IRequest request, bool handleMissingBindings)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var modBindings = this.GetSatisfiedBindings(request).ToArray();
            var bindingGroups = modBindings.Where(binding => !binding.IsImplicit).ToList().Yield()
                .Append(
                    this.globalKernel.GetSatisfiedBindings(request).Where(binding => !this.IsProxyToThis(binding))
                        .ToList()
                )
                .Append(modBindings.Where(binding => binding.IsImplicit).ToList())
                .Where(bindings => bindings.Any())
                .ToList();

            var resolvedServices = new List<TService>();
            foreach (var satisfiedBindings in bindingGroups)
            {
                // Implicit bindings should only be added if there are no other matching bindings
                if (resolvedServices.Any() && satisfiedBindings.Any(binding => binding.IsImplicit))
                {
                    break;
                }

                if (request.IsUnique)
                {
                    if (satisfiedBindings.Count > 1
                        && this.BindingPrecedenceComparer.Compare(satisfiedBindings[0], satisfiedBindings[1]) == 0)
                    {
                        if (request.IsOptional && !request.ForceUnique)
                        {
                            return Enumerable.Empty<TService>();
                        }

                        var formattedBindings = satisfiedBindings
                            .Select(binding => binding.Format(this.CreateContext(request, binding)))
                            .ToArray();

                        throw new ActivationException(
                            ExceptionFormatter.CouldNotUniquelyResolveBinding(request, formattedBindings)
                        );
                    }

                    return ((TService) this.CreateContext(request, satisfiedBindings[0]).Resolve()).Yield();
                }

                var services = satisfiedBindings
                    .Select(binding => this.CreateContext(request, binding).Resolve());
                resolvedServices.AddRange(
                    services
                        .Cast<TService>()
                );
            }

            if (resolvedServices.Any())
            {
                return resolvedServices;
            }

            if (handleMissingBindings && this.HandleMissingBinding(request))
            {
                return this.Resolve<TService>(request, false);
            }

            if (request.IsOptional)
            {
                return Enumerable.Empty<TService>();
            }

            throw new ActivationException(ExceptionFormatter.CouldNotResolveBinding(request));
        }

        private static bool ShouldInherit(IRequest request)
        {
            return request.Parameters.OfType<GlobalParameter>().Any()
                   || (request.Target?.Member.GetCustomAttributes<GlobalAttribute>().Any() ?? false);
        }
    }
}