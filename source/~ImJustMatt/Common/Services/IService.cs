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
    internal interface IService
    {
        /// <summary>
        ///     Gets the name of the service.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        ///     Resolves all requested dependencies.
        /// </summary>
        public void ResolveDependencies(ServiceManager serviceManager);
    }
}