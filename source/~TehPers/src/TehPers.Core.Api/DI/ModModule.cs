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
using Ninject.Syntax;
using StardewModdingAPI;

namespace TehPers.Core.Api.DI
{
    /// <inheritdoc cref="BaseModule"/>
    /// <inheritdoc cref="IModModule"/>
    public abstract class ModModule : BaseModule, IModModule
    {
        /// <inheritdoc/>
        public IBindingRoot GlobalProxyRoot => this.Kernel.GlobalProxyRoot;

        /// <inheritdoc/>
        public new IModKernel Kernel { get; private set; }

        /// <inheritdoc/>
        protected override IKernel KernelInstance => this.Kernel;

        /// <inheritdoc/>
        public IModKernelFactory ParentFactory => this.Kernel.ParentFactory;

        /// <inheritdoc/>
        public IMod ParentMod => this.Kernel.ParentMod;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModModule"/> class.
        /// </summary>
        protected ModModule()
        {
            // Kernel should not be used until after `OnLoad` is called
            // TODO: is there a better way to do this?
            this.Kernel = null!;
        }

        /// <inheritdoc/>
        public override void Unbind(Type service)
        {
            this.Kernel.Unbind(service);
        }

        /// <inheritdoc/>
        public override void OnLoad(IKernel kernel)
        {
            _ = kernel ?? throw new ArgumentNullException(nameof(kernel));

            if (kernel is not IModKernel modKernel)
            {
                throw new InvalidOperationException(
                    $"Types that inherit {nameof(ModModule)} can only be loaded into types that implement {nameof(IModKernel)}."
                );
            }

            this.Kernel = modKernel;
            base.OnLoad(kernel);
        }

        /// <inheritdoc/>
        public override void OnUnload(IKernel kernel)
        {
            base.OnUnload(kernel);

            // Kernel should not be used after this point
            // TODO: is there a better way to do this?
            this.Kernel = null!;
        }
    }
}