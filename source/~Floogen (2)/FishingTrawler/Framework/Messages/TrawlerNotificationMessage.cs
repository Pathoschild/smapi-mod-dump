/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

namespace FishingTrawler.Messages
{
    internal class TrawlerNotificationMessage
    {
        public string Notification { get; set; }

        public TrawlerNotificationMessage()
        {

        }

        public TrawlerNotificationMessage(string notification)
        {
            Notification = notification;
        }
    }
}
