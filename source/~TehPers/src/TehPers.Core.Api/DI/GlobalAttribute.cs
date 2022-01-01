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
using Ninject.Planning.Bindings;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// Forces a <see cref="IModKernel"/> to inherit a service's implementation from the global <see cref="IKernel"/>.
    /// </summary>
    public class GlobalAttribute : ConstraintAttribute
    {
        /// <inheritdoc />
        public override bool Matches(IBindingMetadata metadata)
        {
            // This is handled within the kernel
            return true;
        }
    }
}
