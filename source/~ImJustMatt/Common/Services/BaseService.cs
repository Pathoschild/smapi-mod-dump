/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace Common.Services
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Encapsulates services that support the features of this mod.
    /// </summary>
    internal abstract class BaseService : IService
    {
        private readonly Dictionary<Type, IList<Action<IService>>> _pendingDependencies = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseService" /> class.
        /// </summary>
        /// <param name="serviceName">The name of the service.</param>
        private protected BaseService(string serviceName)
        {
            this.ServiceName = serviceName;
        }

        /// <inheritdoc />
        public string ServiceName { get; }

        /// <inheritdoc />
        public void ResolveDependencies(ServiceManager serviceManager)
        {
            foreach (var dependency in this._pendingDependencies)
            {
                var service = (IService)typeof(ServiceManager).GetMethod(nameof(ServiceManager.GetByType))?.MakeGenericMethod(dependency.Key).Invoke(
                    serviceManager,
                    new object[]
                    {
                    });

                foreach (var handler in dependency.Value)
                {
                    handler(service);
                }
            }
        }

        private protected void AddDependency<TServiceType>(Action<IService> handler) where TServiceType : IService
        {
            var type = typeof(TServiceType);
            if (!this._pendingDependencies.TryGetValue(type, out var handlers))
            {
                handlers = new List<Action<IService>>();
                this._pendingDependencies.Add(type, handlers);
            }

            handlers.Add(handler);
        }
    }
}