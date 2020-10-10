/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace Entoarox.Framework.Interface
{
    public abstract class BaseComponentContainer : BaseComponent, IComponentContainer
    {
        /*********
        ** Accessors
        *********/
#pragma warning disable CS0618 // Type or member is obsolete
        public InterfaceMenu Menu => this.Owner.Menu;
#pragma warning restore CS0618 // Type or member is obsolete
        public Rectangle InnerBounds => this.OuterBounds;
        public IDynamicComponent FocusComponent => null;


        /*********
        ** Public methods
        *********/
        public abstract bool HasFocus(IDynamicComponent component);
        public abstract bool TabBack();
        public abstract bool TabNext();


        /*********
        ** Protected methods
        *********/
        protected BaseComponentContainer(string name, Rectangle bounds, int layer)
            : base(name, bounds, layer) { }
    }
}
