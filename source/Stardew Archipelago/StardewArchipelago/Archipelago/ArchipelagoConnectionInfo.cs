/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

namespace StardewArchipelago.Archipelago
{
    public class ArchipelagoConnectionInfo
    {
        public string HostUrl { get; private set; }
        public int Port { get; private set; }
        public string SlotName { get; private set; }
        public bool? DeathLink { get; set; }
        public string Password { get; private set; }

        public ArchipelagoConnectionInfo(string hostUrl, int port, string slotName, bool? deathLink, string password = null)
        {
            HostUrl = hostUrl;
            Port = port;
            SlotName = slotName;
            DeathLink = deathLink;
            Password = password;
        }
    }
}
