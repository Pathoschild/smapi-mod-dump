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
using Ninject.Parameters;
using Ninject.Syntax;
using StardewModdingAPI;

namespace TehPers.Core.Api.DI
{
    /// <summary>Factory for creating an <see cref="IModKernel"/> for a <see cref="IMod"/>.</summary>
    public interface IModKernelFactory
    {
        /// <summary>
        /// Gets the global service container. Injected mod APIs can be found here.
        /// </summary>
        /// <example>
        /// To retrieve a mod's injected API:
        /// 
        /// <code>
        /// var api = kernelFactory.GlobalKernel.Get&lt;SomeModApi&gt;();
        /// </code>
        /// </example>
        IResolutionRoot GlobalServices { get; }

        /// <summary>
        /// Adds a processor for all created <see cref="IModKernel"/>s. This processor is applied
        /// to all existing kernels and all kernels created in the future.
        /// </summary>
        /// <param name="processor"></param>
        void AddKernelProcessor(Action<IModKernel> processor);

        /// <summary>
        /// Gets the <see cref="IModKernel"/> for your <see cref="IMod"/>. This
        /// <see cref="IModKernel"/> is specific to your <see cref="IMod"/> and can only see
        /// dependencies registered to it and the global <see cref="IKernel"/>. Use
        /// <see cref="ResolutionExtensions.Get{T}(IResolutionRoot, IParameter[])"/> to get a
        /// service from the <see cref="IModKernel"/>.
        /// </summary>
        /// <param name="owner">The owner of the <see cref="IModKernel"/>.</param>
        /// <returns>The <see cref="IModKernel"/> for your <see cref="IMod"/>.</returns>
        IModKernel GetKernel(IMod owner);
    }
}