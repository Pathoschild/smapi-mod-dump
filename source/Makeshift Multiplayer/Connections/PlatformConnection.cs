/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using StardewValleyMP.Platforms;
using Steamworks;
using System;
using System.IO;

namespace StardewValleyMP.Connections
{
    public abstract class PlatformConnection : IConnection
    {
        public Friend friend { get; protected set; }

        public bool accepted { get; protected set; }

        // Ugh, wish I could just do friend SteamPlatform; or something
        protected PlatformConnection( Friend theFriend, bool alreadyConnected = false )
        {
            friend = theFriend;
            accepted = alreadyConnected;
        }

        ~PlatformConnection()
        {
            disconnect();
        }

        public abstract bool isConnected();
        public abstract void disconnect();
        public abstract void accept();

        public abstract Stream getStream();
    }
}