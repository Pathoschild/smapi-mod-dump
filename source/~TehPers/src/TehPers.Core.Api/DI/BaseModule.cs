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
using Ninject.Infrastructure.Language;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Ninject.Syntax;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// Base class for modules that automatically tracks bindings.
    /// </summary>
    public abstract class BaseModule : BindingRoot, INinjectModule
    {
        /// <summary>
        /// Gets all the bindings registered by this module.
        /// </summary>
        public ICollection<IBinding> Bindings { get; }

        /// <inheritdoc/>
        public IKernel? Kernel { get; private set; }

        /// <inheritdoc/>
        public string Name => this.GetType().FullName ?? this.GetType().Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseModule"/> class.
        /// </summary>
        protected BaseModule()
        {
            this.Bindings = new List<IBinding>();
        }

        /// <inheritdoc/>
        public override void AddBinding(IBinding binding)
        {
            _ = binding ?? throw new ArgumentNullException(nameof(binding));
            _ = this.Kernel ?? throw new InvalidOperationException("Module has not been loaded yet.");

            this.Kernel.AddBinding(binding);
            this.Bindings.Add(binding);
        }

        /// <inheritdoc/>
        public override void RemoveBinding(IBinding binding)
        {
            _ = binding ?? throw new ArgumentNullException(nameof(binding));
            _ = this.Kernel ?? throw new InvalidOperationException("Module has not been loaded yet.");

            this.Kernel.RemoveBinding(binding);
            this.Bindings.Remove(binding);
        }

        /// <inheritdoc/>
        public virtual void OnLoad(IKernel kernel)
        {
            _ = kernel ?? throw new ArgumentNullException(nameof(kernel));

            this.Kernel = kernel;
            this.Load();
        }

        /// <inheritdoc/>
        public virtual void OnUnload(IKernel kernel)
        {
            _ = kernel ?? throw new ArgumentNullException(nameof(kernel));

            this.Unload();
            this.Bindings.Map(kernel.RemoveBinding);
            this.Kernel = null;
        }

        /// <inheritdoc/>
        public virtual void OnVerifyRequiredModules()
        {
            this.VerifyRequiredModulesAreLoaded();
        }

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Unloads the module from the kernel.
        /// </summary>
        public virtual void Unload()
        {
        }

        /// <summary>
        /// Called after loading the modules. A module can verify here if all other required modules are loaded.
        /// </summary>
        public virtual void VerifyRequiredModulesAreLoaded()
        {
        }
    }
}