/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using Ninject;

namespace TehPers.Core.Api.DI
{
    /// <summary>
    /// Simple service factory for lazily retrieving/constructing instances of services.
    /// </summary>
    /// <typeparam name="TService">The type of service to retrieve.</typeparam>
    public interface ISimpleFactory<out TService>
    {
        /// <summary>
        /// Gets a single instance of the service. If more than one binding exists for the service, this throws an <see cref="ActivationException"/> instead.
        /// </summary>
        /// <returns>A single instance of the service.</returns>
        TService GetSingle();

        /// <summary>
        /// Gets every instance of the service. Services are activated (if necessary) when the returned <see cref="IEnumerable{T}"/> is enumerated.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of every instance of the service.</returns>
        IEnumerable<TService> GetAll();
    }
}
