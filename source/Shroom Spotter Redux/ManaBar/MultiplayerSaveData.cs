/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using StardewModdingAPI;
using StardewValley;

namespace ManaBar
{
    public class MultiplayerSaveData
    {
        public const string MSG_DATA = "spacechase0.ManaBar.Data";
        public const string MSG_MINIDATA = "spacechase0.ManaBar.MiniData";

        public static string OldFilePath => Path.Combine(Constants.CurrentSavePath, "magic0.2.json");
        public static string SaveKey => "spacechase0.ManaBar.Mana";

        public class PlayerData
        {
            public int mana = 0;
            public int manaCap = 0;
        }
        public Dictionary<long, PlayerData> players = new Dictionary<long, PlayerData>();

        internal static JsonSerializerSettings networkSerializerSettings { get; }  = new JsonSerializerSettings()
        {
            Formatting = Formatting.None,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
        };

        internal void syncMineFull()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((int)1);
                writer.Write(Game1.player.UniqueMultiplayerID);
                writer.Write(JsonConvert.SerializeObject(players[Game1.player.UniqueMultiplayerID], networkSerializerSettings));
                SpaceCore.Networking.BroadcastMessage(MSG_DATA, stream.ToArray());
            }
        }

        internal void syncMineMini()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(players[Game1.player.UniqueMultiplayerID].mana);
                writer.Write(players[Game1.player.UniqueMultiplayerID].manaCap);
                SpaceCore.Networking.BroadcastMessage(MSG_MINIDATA, stream.ToArray());
            }
        }
    }
}
