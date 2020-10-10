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
    public abstract class BaseInputComponent : BaseDynamicComponent, IInputComponent
    {
        /*********
        ** Accessors
        *********/
        public bool Selected { get; set; }


        /*********
        ** Public methods
        *********/
        public abstract void ReceiveInput(char input);


        /*********
        ** Protected methods
        *********/
        protected BaseInputComponent(string name, Rectangle bounds, int layer)
            : base(name, bounds, layer) { }
    }
}
