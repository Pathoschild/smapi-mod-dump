/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

using SAML.Events;

namespace SAML.Utilities
{
    public interface INotifyCollectionChanged
    {
        /// <summary>
        /// An event which fires when the items in a collection have changed
        /// </summary>
        event CollectionChangedEventHandler? CollectionChanged;
    }
}
