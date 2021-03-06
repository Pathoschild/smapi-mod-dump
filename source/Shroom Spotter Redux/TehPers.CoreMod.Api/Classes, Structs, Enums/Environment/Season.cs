/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System;

namespace TehPers.CoreMod.Api.Environment {
    [Flags]
    public enum Season {
        Spring = 1,
        Summer = 2,
        Fall = 4,
        Winter = 8,

        None = 0,
        All = Season.Spring | Season.Summer | Season.Fall | Season.Winter
    }
}