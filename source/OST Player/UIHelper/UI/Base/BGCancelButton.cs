/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ProfeJavix/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;

namespace UIHelper
{
    internal class BGCancelButton : BGButton
    {
        internal BGCancelButton(Rectangle bounds, Action closeAction, Action retAction) : base(bounds, closeAction, retAction)
        {
            btnSrc = new(192, 256, 64, 64);
        }
    }
}
