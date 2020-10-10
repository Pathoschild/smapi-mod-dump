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
    public interface IKeyboardComponent
    {
        /*********
        ** Accessors
        *********/
        bool Selected { get; set; }


        /*********
        ** Public methods
        *********/
        void TextReceived(char chr);
        void TextReceived(string str);
        void CommandReceived(char cmd);
        void SpecialReceived(Keys key);
    }
}
