/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewArchipelago.Items.Mail;
using StardewArchipelago.Stardew;
using StardewModdingAPI;

namespace StardewArchipelago.Archipelago.Gifting
{
    public interface IGiftHandler
    {
        GiftSender Sender { get; }
        void Initialize(IMonitor monitor, ArchipelagoClient archipelago, StardewItemManager itemManager, Mailman mail);
        bool HandleGiftItemCommand(string message);
        void ReceiveAllGiftsTomorrow();
        void ExportAllGifts(string filePath);
    }
}
