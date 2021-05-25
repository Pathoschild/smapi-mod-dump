/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://sourceforge.net/projects/sdvmod-remind-to-exit/
**
*************************************************/

/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using StardewModdingAPI.Utilities;

namespace RemindToExit
{
    public class ModConfig
    {
        public bool Enabled = true;
        public KeybindList HotKeyActivate = KeybindList.Parse("F7");
        public KeybindList HotKeyCustomMessage = KeybindList.Parse("RightShift+F7");
    }
}
