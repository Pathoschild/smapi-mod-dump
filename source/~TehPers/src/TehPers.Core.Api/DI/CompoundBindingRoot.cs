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
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// A binding root that automatically adds global proxy bindings for an additional set of services whenever a service is bound.
    /// </summary>
    public class CompoundBindingRoot : IBindingRoot
    {
        private readonly IBindingRoot originalRoot;
        private readonly Type[] autoServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundBindingRoot"/> class.
        /// </summary>
        /// <param name="originalRoot">The binding root to create bindings for.</param>
        /// <param name="autoServices">The services which will have bindings created automatically for them.</param>
        public CompoundBindingRoot(IBindingRoot originalRoot, params Type[] autoServices)
        {
            this.originalRoot = originalRoot ?? throw new ArgumentNullException(nameof(originalRoot));
            this.autoServices = autoServices ?? throw new ArgumentNullException(nameof(autoServices));
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T> Bind<T>()
        {
            var syntax = this.originalRoot.Bind<T>();
            foreach (var service in this.autoServices)
            {
                this.originalRoot.AddBinding(new Binding(service, syntax.BindingConfiguration));
            }

            return syntax;
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2> Bind<T1, T2>()
        {
            var syntax = this.originalRoot.Bind<T1, T2>();
            foreach (var service in this.autoServices)
            {
                this.originalRoot.AddBinding(new Binding(service, syntax.BindingConfiguration));
            }

            return syntax;
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2, T3> Bind<T1, T2, T3>()
        {
            var syntax = this.originalRoot.Bind<T1, T2, T3>();
            foreach (var service in this.autoServices)
            {
                this.originalRoot.AddBinding(new Binding(service, syntax.BindingConfiguration));
            }

            return syntax;
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2, T3, T4> Bind<T1, T2, T3, T4>()
        {
            var syntax = this.originalRoot.Bind<T1, T2, T3, T4>();
            foreach (var service in this.autoServices)
            {
                this.originalRoot.AddBinding(new Binding(service, syntax.BindingConfiguration));
            }

            return syntax;
        }

        /// <inheritdoc/>
        public IBindingToSyntax<object> Bind(params Type[] services)
        {
            var syntax = this.originalRoot.Bind(services);
            foreach (var service in this.autoServices)
            {
                this.originalRoot.AddBinding(new Binding(service, syntax.BindingConfiguration));
            }

            return syntax;
        }

        /// <inheritdoc/>
        public void Unbind<T>()
        {
            this.originalRoot.Unbind<T>();
        }

        /// <inheritdoc/>
        public void Unbind(Type service)
        {
            this.originalRoot.Unbind(service);
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1> Rebind<T1>()
        {
            return this.originalRoot.Rebind<T1>();
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2> Rebind<T1, T2>()
        {
            return this.originalRoot.Rebind<T1, T2>();
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2, T3> Rebind<T1, T2, T3>()
        {
            return this.originalRoot.Rebind<T1, T2, T3>();
        }

        /// <inheritdoc/>
        public IBindingToSyntax<T1, T2, T3, T4> Rebind<T1, T2, T3, T4>()
        {
            return this.originalRoot.Rebind<T1, T2, T3, T4>();
        }

        /// <inheritdoc/>
        public IBindingToSyntax<object> Rebind(params Type[] services)
        {
            return this.originalRoot.Rebind(services);
        }

        /// <inheritdoc/>
        public void AddBinding(IBinding binding)
        {
            this.originalRoot.AddBinding(binding);
        }

        /// <inheritdoc/>
        public void RemoveBinding(IBinding binding)
        {
            this.originalRoot.RemoveBinding(binding);
        }
    }
}