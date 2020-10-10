/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Denifia/StardewMods
**
*************************************************/

using System;
using StardewValley;

namespace Denifia.Stardew.SendItems.Events
{
    public class MailComposedEventArgs : EventArgs
    {
        public string ToFarmerId { get; set; }
        public Item Item { get; set; }
    }
}