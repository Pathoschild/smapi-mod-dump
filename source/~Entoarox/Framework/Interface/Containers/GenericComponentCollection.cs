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
using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public class GenericComponentCollection : BaseComponentCollection
    {
        /*********
        ** Public methods
        *********/
        public GenericComponentCollection(string name, Rectangle bounds, int layer = 0)
            : base(name, bounds, layer) { }

        public GenericComponentCollection(string name, Rectangle bounds, IEnumerable<IComponent> components, int layer = 0)
            : base(name, bounds, layer)
        {
            foreach (IComponent component in components)
                this.AddComponent(component);
        }
    }
}
