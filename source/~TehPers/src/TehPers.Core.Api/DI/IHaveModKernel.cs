/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Ninject.Infrastructure;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// Indicates that an object has a reference to an <see cref="IModKernel"/>.
    /// </summary>
    public interface IHaveModKernel : IHaveKernel
    {
        /// <summary>
        /// Gets the kernel.
        /// </summary>
        new IModKernel Kernel { get; }
    }
}