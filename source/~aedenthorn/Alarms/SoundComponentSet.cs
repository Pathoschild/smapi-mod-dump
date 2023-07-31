/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Collections.Generic;

namespace Alarms
{
    public class SoundComponentSet
    {
        public ClockSound sound;
        public TextBox hourText;
        public TextBox minText;
        public TextBox soundText;
        public TextBox notificationText;
        public ClickableTextureComponent[] seasonCCs;
        public ClickableComponent[] weekCCs;
        public ClickableComponent[] monthCCs;
        public ClickableTextureComponent notifCC;
        public ClickableTextureComponent soundCC;
        public ClickableTextureComponent enabledBox;
        public ClickableTextureComponent deleteCC;
    }
}