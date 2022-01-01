/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// The global mod services. This contains the mod kernel factory, for example.
    /// </summary>
    public static class ModServices
    {
        /// <summary>
        /// Gets the <see cref="IModKernelFactory"/> for creating mod kernels. This value is
        /// guaranteed to be <see langword="null"/> if <c>TehPers.Core</c> has not been loaded yet.
        /// To ensure that your mod loads after the core mod, add <c>TehPers.Core</c> as a
        /// dependency in your mod's manifest. If you do not need the core mod to be loaded for
        /// your mod to function, then you may add it as an optional dependency instead.
        /// </summary>
        public static IModKernelFactory? Factory { get; internal set; }
    }
}