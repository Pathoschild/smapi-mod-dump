/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Input;

namespace Entoarox.Framework.UI
{
    public abstract class BaseKeyboardFormComponent : BaseFormComponent, IKeyboardComponent
    {
        /*********
        ** Accessors
        *********/
        public bool Selected { get; set; }


        /*********
        ** Public methods
        *********/
        public virtual void TextReceived(char chr) { }

        public virtual void TextReceived(string str) { }

        public virtual void CommandReceived(char cmd) { }

        public virtual void SpecialReceived(Keys key) { }
    }
}
