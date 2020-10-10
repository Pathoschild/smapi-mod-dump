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

namespace Entoarox.Framework.Interface
{
    public interface IHotkeyComponent : IDynamicComponent
    {
        /*********
        ** Methods
        *********/
        /// <summary>This event is triggered whenever a keyboard key is pressed. If <see cref="IDynamicComponent.Enabled" /> or <see cref="IComponent.Visible" /> is false, this event will not trigger. If a component is both <see cref="IHotkeyComponent" /> and <see cref="IInputComponent" /> then unless <see cref="IInputComponent.Selected" /> is false, input events happen instead of hotkey events.</summary>
        /// <param name="key">The key pressed.</param>
        bool ReceiveHotkey(Keys key);
    }
}
