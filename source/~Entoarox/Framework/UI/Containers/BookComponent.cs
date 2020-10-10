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
using Microsoft.Xna.Framework;

namespace Entoarox.Framework.UI
{
    internal class BookComponent : IComponentContainer
    {
        /*********
        ** Accessors
        *********/
        public Rectangle EventRegion => throw new NotImplementedException();
        public Rectangle ZoomEventRegion => throw new NotImplementedException();


        /*********
        ** Public methods
        *********/
        public FrameworkMenu GetAttachedMenu()
        {
            throw new NotImplementedException();
        }

        public void GiveFocus(IInteractiveMenuComponent component)
        {
            throw new NotImplementedException();
        }

        public void ResetFocus()
        {
            throw new NotImplementedException();
        }
    }
}
