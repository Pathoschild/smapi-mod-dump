/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using Ninject.Syntax;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// Indicates that an object has a proxy root for exposing services.
    /// </summary>
    public interface IProxyBindable : IBindingRoot
    {
        /// <summary>
        /// Gets an <see cref="IBindingRoot"/> which automatically creates proxy bindings in the global kernel when bindings are created.
        /// Dependencies registered in it are visible to all mods, however they are resolved by your <see cref="IModKernel"/>.
        /// </summary>
        IBindingRoot GlobalProxyRoot { get; }
    }
}