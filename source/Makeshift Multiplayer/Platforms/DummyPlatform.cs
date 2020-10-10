/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMP
**
*************************************************/

using StardewValleyMP.Connections;
using System.Collections.Generic;

namespace StardewValleyMP.Platforms
{
    class DummyPlatform : IPlatform
    {
        public DummyPlatform()
        {
            Log.info("Using dummy platform.");
        }

        public override string getName()
        {
            return "";
        }

        public override void update()
        {
        }

        public override List< Friend > getFriends()
        {
            return new List<Friend>();
        }

        public override List<Friend> getOnlineFriends()
        {
            return new List<Friend>();
        }

        public override IConnection connectToFriend(Friend other)
        {
            return null;
        }
    }
}
