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
    using System.Reflection;

    internal class PendingService
    {
        private readonly ConstructorInfo _constructor;

        public PendingService(Type serviceType)
        {
            this._constructor = serviceType.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[]
                {
                    typeof(ServiceManager),
                },
                null);
        }

        public IService Create(ServiceManager serviceManager)
        {
            return (IService)this._constructor.Invoke(
                new object[]
                {
                    serviceManager,
                });
        }
    }
}