/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.Framework.Interface
{
    public interface IInputComponent : IDynamicComponent
    {
        /*********
        ** Accessors
        *********/
        /// <summary>Input components will only receive input events while this value is true.</summary>
        bool Selected { get; set; }


        /*********
        ** Methods
        *********/
        /// <summary>Triggers when a new character is input by the user. Conversion of keyboard input to the correct character values is handled by the framework. If <see cref="Selected" />, <see cref="IDynamicComponent.Enabled" /> or <see cref="IComponent.Visible" /> is false, this event will not trigger.</summary>
        /// <param name="input">The input character.</param>
        void ReceiveInput(char input);
    }
}
