/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// Provides a path for mods to register bindings.
    /// </summary>
    public interface IModBindingRoot : IProxyBindable
    {
        /// <summary>
        /// Gets the mod which owns this <see cref="IModBindingRoot"/>.
        /// </summary>
        IMod ParentMod { get; }

        /// <summary>
        /// Gets the <see cref="IModKernelFactory"/> that created this <see cref="IModBindingRoot"/>.
        /// </summary>
        IModKernelFactory ParentFactory { get; }
    }
}