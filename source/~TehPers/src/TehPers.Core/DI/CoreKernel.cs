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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Introspection;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using TehPers.Core.Api.Extensions;

namespace TehPers.Core.DI
{
    public abstract class CoreKernel : StandardKernel
    {
        private static readonly MethodInfo resolveGeneric = typeof(CoreKernel)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single(method => method.Name == nameof(CoreKernel.Resolve) && method.IsGenericMethod);

        private static readonly MethodInfo castEnumerableInfo =
            typeof(CoreKernel).GetMethod(
                nameof(CoreKernel.CastEnumerable),
                BindingFlags.NonPublic | BindingFlags.Static
            )
            ?? throw new($"Couldn't find method for {nameof(CoreKernel.castEnumerableInfo)}.");

        private static readonly MethodInfo castArrayInfo = typeof(CoreKernel).GetMethod(
                nameof(CoreKernel.CastArray),
                BindingFlags.NonPublic | BindingFlags.Static
            )
            ?? throw new($"Couldn't find method for {nameof(CoreKernel.castArrayInfo)}.");

        private static readonly MethodInfo castListInfo = typeof(CoreKernel).GetMethod(
                nameof(CoreKernel.CastList),
                BindingFlags.NonPublic | BindingFlags.Static
            )
            ?? throw new($"Couldn't find method for {nameof(CoreKernel.castListInfo)}.");

        protected IBindingPrecedenceComparer BindingPrecedenceComparer { get; }

        protected CoreKernel(INinjectSettings settings, params INinjectModule[] modules)
            : base(settings, modules)
        {
            this.BindingPrecedenceComparer = this.Components.Get<IBindingPrecedenceComparer>();
        }

        public override bool CanResolve(IRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            return this.CanResolve(request, false);
        }

        public override bool CanResolve(IRequest request, bool ignoreImplicitBindings)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            return this.GetSatisfiedBindings(request)
                .Any(binding => !(ignoreImplicitBindings && binding.IsImplicit));
        }

        protected static object Cast(
            IEnumerable<object> services,
            Type serviceType,
            ServiceCollectionType collectionType
        )
        {
            return collectionType switch
            {
                ServiceCollectionType.Enumerable => CoreKernel.castEnumerableInfo
                    .MakeGenericMethod(serviceType)
                    .Invoke(null, new object[] { services })!,
                ServiceCollectionType.Array => CoreKernel.castArrayInfo
                    .MakeGenericMethod(serviceType)
                    .Invoke(null, new object[] { services })!,
                ServiceCollectionType.List => CoreKernel.castListInfo.MakeGenericMethod(serviceType)
                    .Invoke(null, new object[] { services })!,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(collectionType),
                    collectionType,
                    "Unknown enum variant"
                ),
            };
        }

        protected static IEnumerable<T> CastEnumerable<T>(IEnumerable<object> services)
        {
            return services.Cast<T>();
        }

        protected static IEnumerable<T> CastArray<T>(IEnumerable<object> services)
        {
            return services.Cast<T>().ToArray();
        }

        protected static IEnumerable<T> CastList<T>(IEnumerable<object> services)
        {
            return services.Cast<T>().ToList();
        }

        public override IEnumerable<object> Resolve(IRequest request)
        {
            IRequest CreateEnumeratedRequest(Type service)
            {
                if (request is
                    {
                        ParentRequest: { } parentRequest,
                        ParentContext: { } parentContext,
                        Target: { } target
                    })
                {
                    var newRequest = parentRequest.CreateChild(service, parentContext, target);
                    newRequest.IsOptional = true;
                    return newRequest;
                }

                return this.CreateRequest(
                    service,
                    null,
                    request.Parameters.Where(p => p.ShouldInherit),
                    true,
                    false
                );
            }

            // Request is for T[]
            if (request.Service.IsArray)
            {
                var elementType = request.Service.GetElementType()!;
                var enumeratedRequest = CreateEnumeratedRequest(elementType);
                var services = this.Resolve(enumeratedRequest, false);
                return new[]
                {
                    CoreKernel.Cast(services, elementType, ServiceCollectionType.Array)
                };
            }

            // Request is for type without generics
            if (!request.Service.IsGenericType)
            {
                return this.Resolve(request, true);
            }

            var genericTypeDefinition = request.Service.GetGenericTypeDefinition();

            // Request is for IEnumerable<T>
            if (genericTypeDefinition == typeof(IEnumerable<>))
            {
                var elementType = request.Service.GenericTypeArguments[0];
                var enumeratedRequest = CreateEnumeratedRequest(elementType);
                var services = this.Resolve(enumeratedRequest, false);
                return new[]
                {
                    CoreKernel.Cast(services, elementType, ServiceCollectionType.Enumerable)
                };
            }

            // Request is for ICollection<T>, IReadOnlyList<T>, or IList<T>
            if (genericTypeDefinition == typeof(ICollection<>)
                || genericTypeDefinition == typeof(IReadOnlyList<>)
                || genericTypeDefinition == typeof(IList<>))
            {
                var elementType = request.Service.GenericTypeArguments[0];
                var enumeratedRequest = CreateEnumeratedRequest(elementType);
                var services = this.Resolve(enumeratedRequest, false);
                return new[] { CoreKernel.Cast(services, elementType, ServiceCollectionType.List) };
            }

            // Resolve service normally
            return this.Resolve(request, true);
        }

        private IEnumerable<object> Resolve(IRequest request, bool handleMissingBindings)
        {
            return CoreKernel.resolveGeneric.MakeGenericMethod(request.Service)
                .Invoke(this, new object[] { request, handleMissingBindings }) is IEnumerable result
                ? (IEnumerable<object>)result
                : Enumerable.Empty<object>();
        }

        protected virtual IEnumerable<TService> Resolve<TService>(
            IRequest request,
            bool handleMissingBindings
        )
        {
            var satisfiedBindings = this.GetSatisfiedBindings(request).ToArray();

            if (satisfiedBindings.Length == 0)
            {
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

            if (request.IsUnique)
            {
                var firstTwo = satisfiedBindings.Take(2).ToArray();
                if (firstTwo.Length > 1
                    && this.BindingPrecedenceComparer.Compare(firstTwo[0], firstTwo[1]) == 0)
                {
                    if (request.IsOptional && !request.ForceUnique)
                    {
                        return Enumerable.Empty<TService>();
                    }

                    var formattedBindings = satisfiedBindings.Select(
                            binding => binding.Format(this.CreateContext(request, binding))
                        )
                        .ToArray();

                    throw new ActivationException(
                        ExceptionFormatter.CouldNotUniquelyResolveBinding(
                            request,
                            formattedBindings
                        )
                    );
                }

                // TODO: make sure this works when missing a binding for the service - implicit bindings might be weird
                return ((TService)this.CreateContext(request, firstTwo[0]).Resolve()).Yield();
            }

            if (satisfiedBindings.Any(binding => !binding.IsImplicit))
            {
                return satisfiedBindings.Where(binding => !binding.IsImplicit)
                    .Select(binding => this.CreateContext(request, binding).Resolve())
                    .Cast<TService>();
            }

            return satisfiedBindings
                .Select(binding => this.CreateContext(request, binding).Resolve())
                .Cast<TService>();
        }

        public virtual IEnumerable<IBinding> GetSatisfiedBindings(IRequest request)
        {
            var satifiesRequest = this.SatifiesRequest(request);
            return this.GetBindings(request.Service).Where(satifiesRequest);
        }

        protected enum ServiceCollectionType
        {
            Enumerable,
            Array,
            List,
        }
    }
}