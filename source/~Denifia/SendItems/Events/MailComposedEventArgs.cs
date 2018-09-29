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