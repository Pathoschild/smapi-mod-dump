/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Denifia/StardewMods
**
*************************************************/

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
