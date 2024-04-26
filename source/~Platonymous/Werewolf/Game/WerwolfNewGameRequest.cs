/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

namespace LandGrants.Game
{
    public class WerwolfNewGameRequest : WerwolfMPMessage
    {
        public long Host { get; set; }
        public WerwolfUpdate InitialUpdate { get; set; }
        public override string Type { get; set; } = "NewGame";

        public Config Config { get; set; }

        public string GameInfo { get; set; }

        public WerwolfNewGameRequest()
        {

        }

        public WerwolfNewGameRequest(long host, long player, WerwolfGame game, WerwolfUpdate initialUpdate, Config config, string gameInfo)
           : base(player,host,game)
        {
            Host = host;
            InitialUpdate = initialUpdate;
            Config = config;
            GameInfo = gameInfo;
        }
    }
}
