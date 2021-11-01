/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace CommonHarmony.Interfaces
{
    /// <summary>
    ///     Service to handle creation/invocation of an event.
    /// </summary>
    /// <typeparam name="TEventArgs">The event argument type.</typeparam>
    internal interface IEventHandlerService<in TEventArgs>
    {
        /// <summary>
        ///     Adds a handler for the event managed by this service.
        /// </summary>
        /// <param name="handler">The event handler to add.</param>
        void AddHandler(TEventArgs handler);

        /// <summary>
        ///     Removed a handler for the event managed by this service.
        /// </summary>
        /// <param name="handler">The event handler to add.</param>
        void RemoveHandler(TEventArgs handler);
    }
}