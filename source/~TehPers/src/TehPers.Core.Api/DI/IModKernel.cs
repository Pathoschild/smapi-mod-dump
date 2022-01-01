/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Ninject;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// A factory capable of creating any type of object based on bindings.
    /// It is a child of the global kernel, so any missing bindings will be resolved by the global kernel.
    /// </summary>
    public interface IModKernel : IKernel, IModBindingRoot
    {
        /// <summary>
        /// Gets the global kernel. Services without bindings in this <see cref="IModKernel"/> are resolved by the global kernel.
        /// </summary>
        IGlobalKernel GlobalKernel { get; }
    }
}