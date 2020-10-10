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

namespace Entoarox.Framework.UI
{
    public interface IComponentContainer
    {
        /*********
        ** Accessors
        *********/
        Rectangle EventRegion { get; }
        Rectangle ZoomEventRegion { get; }


        /*********
        ** Methods
        *********/
        void ResetFocus();
        void GiveFocus(IInteractiveMenuComponent component);
        FrameworkMenu GetAttachedMenu();
    }
}
