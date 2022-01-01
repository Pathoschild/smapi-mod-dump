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
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Targets;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// Forces an <see cref="IModKernel"/> to rely on the global <see cref="IKernel"/> to resolve a service.
    /// </summary>
    public class GlobalParameter : IParameter
    {
        /// <inheritdoc/>
        public string Name => nameof(GlobalParameter);

        /// <inheritdoc/>
        public bool ShouldInherit => false;

        /// <inheritdoc/>
        public bool Equals(IParameter? other)
        {
            return other is GlobalParameter;
        }

        /// <inheritdoc/>
        public object GetValue(IContext context, ITarget target)
        {
            return this;
        }
    }
}