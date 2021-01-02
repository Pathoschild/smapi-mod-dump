/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System.Collections.Generic;

namespace Entoarox.Utilities.UI.Interfaces
{
    public interface IComponentContainer : IEnumerable<IComponent>
    {
        IComponentMenu Menu { get; }
        IComponentContainer Components { get; }

        IComponent this[string componentId] { get; }

        void Add(IComponent component);
        void Remove(IComponent component);
        bool Contains(IComponent component);
        void Remove(string componentId);
        bool Contains(string componentId);
        void MarkDirty();
    }
}
