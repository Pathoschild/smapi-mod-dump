/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

namespace Entoarox.Framework.UI
{
    public interface IComponentCollection : IComponentContainer
    {
        /*********
        ** Accessors
        *********/
        List<IInteractiveMenuComponent> InteractiveComponents { get; }
        List<IMenuComponent> StaticComponents { get; }


        /*********
        ** Methods
        *********/
        bool AcceptsComponent(IMenuComponent component);
        void AddComponent(IMenuComponent component);
        void RemoveComponent(IMenuComponent component);
        void RemoveComponents<T>() where T : IMenuComponent;
        void RemoveComponents(Predicate<IMenuComponent> filter);
        void ClearComponents();
    }
}
