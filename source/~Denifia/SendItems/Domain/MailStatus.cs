namespace Denifia.Stardew.SendItems.Domain
{
    public enum MailStatus
    {
        Unknown = 0,
        Composed, // Letter made but not picked up by delivery service
        Posted, // Sent to the server for delivery
        Delivered, // Delivered to a local farmer (this will show as a waiting letter)
        Read // Player has read the letter
    }
}
