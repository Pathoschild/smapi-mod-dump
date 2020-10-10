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
    public interface IInteractiveMenuComponent : IMenuComponent
    {
        bool InBounds(Point position, Point offset);
        void LeftClick(Point position, Point offset);
        void RightClick(Point position, Point offset);
        void LeftHeld(Point position, Point offset);
        void LeftUp(Point position, Point offset);
        void HoverIn(Point position, Point offset);
        void HoverOut(Point position, Point offset);
        void HoverOver(Point position, Point offset);
        void FocusLost();
        void FocusGained();
        bool Scroll(int direction, Point position, Point offset);
    }
}
