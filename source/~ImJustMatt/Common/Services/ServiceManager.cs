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
    using System.Linq;
    using Helpers;
    using StardewModdingAPI;

    /// <summary>
    ///     Service manager to request shared services.
    /// </summary>
    internal class ServiceManager
    {
        private readonly IDictionary<string, IService> _services = new Dictionary<string, IService>();

        public ServiceManager(IModHelper helper, IManifest manifest)
        {
            ServiceManager.Instance ??= this;
            this.Helper = helper;
            this.ModManifest = manifest;
        }

        private static ServiceManager Instance { get; set; }
        public IModHelper Helper { get; }
        public IManifest ModManifest { get; }

        /// <summary>
        ///     Request a service by name and type.
        /// </summary>
        /// <param name="serviceName">The name of the service to request.</param>
        /// <typeparam name="TServiceType">The type of service to request.</typeparam>
        /// <returns>Returns a service of the given type.</returns>
        /// <exception cref="ArgumentException">No valid service can be found.</exception>
        public TServiceType GetByName<TServiceType>(string serviceName) where TServiceType : IService
        {
            return (TServiceType)this._services[serviceName];
        }

        /// <summary>
        ///     Request a service by type.
        /// </summary>
        /// <typeparam name="TServiceType">The type of service to request.</typeparam>
        /// <returns>Returns a service of the given type.</returns>
        /// <exception cref="NullReferenceException">No valid service can be found.</exception>
        public TServiceType GetByType<TServiceType>() where TServiceType : IService
        {
            return this._services.Values.OfType<TServiceType>().Single();
        }

        public void Create(IEnumerable<Type> types)
        {
            IList<IService> pendingServices = new List<IService>();
            foreach (var type in types)
            {
                var service = new PendingService(type).Create(this);
                Log.Trace($"Registering service {service.ServiceName}.", true);
                this._services.Add(service.ServiceName, service);
                pendingServices.Add(service);
            }

            for (var i = pendingServices.Count - 1; i >= 0; i--)
            {
                pendingServices[i].ResolveDependencies(this);
                pendingServices.RemoveAt(i);
            }
        }

        /// <summary>
        ///     Request a service by type.
        /// </summary>
        /// <typeparam name="TServiceType">The type of service to request.</typeparam>
        /// <returns>Returns a service of the given type.</returns>
        /// <exception cref="ArgumentException">No valid service can be found.</exception>
        public List<TServiceType> GetAll<TServiceType>() where TServiceType : IService
        {
            return this._services.Values.OfType<TServiceType>().ToList();
        }
    }
}