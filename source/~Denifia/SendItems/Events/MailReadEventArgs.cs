using System;

namespace Denifia.Stardew.SendItems.Events
{
    public class MailReadEventArgs : EventArgs
    {
        public Guid Id { get; set; }
    }
}
