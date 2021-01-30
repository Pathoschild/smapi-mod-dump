/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using iTile.Core.Logic.SaveSystem;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTile.Core.Logic.Network
{
    public class NetworkManager : Manager
    {
        public static readonly string[] toModIDs = new string[] { iTile.ModID };
        public const string sessionDataKey = "SessionData";
        public const string pasteActionKey = "PasteAction";
        public const string restoreActionKey = "RestoreAction";

        public NetworkManager()
        {
            Init();
        }

        public override void Init()
        {
            SubscribeEvents();
        }

        public void SubscribeEvents()
        {
            Helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
            Helper.Events.Multiplayer.ModMessageReceived += OnModMessage;
        }

        private void OnModMessage(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            if (e.FromModID != iTile.ModID)
                return;

            SaveManager saveMng = CoreManager.Instance.saveManager;
            if (!Context.IsMainPlayer && e.Type == sessionDataKey)
            {
                SaveProfile session = e.ReadAs<SaveProfile>();
                if (session != null)
                {
                    saveMng.InitSessionExternal(session);
                }
            }
            else if (e.Type == pasteActionKey)
            {
                Packet data = e.ReadAs<Packet>();
                string locName = data.LocationName;
                if (data == null || data.TileProfile == null || string.IsNullOrEmpty(locName))
                    return;

                if (saveMng.session != null)
                {
                    saveMng.session.GetLocationProfileSafe(locName).HandleTileReplacement(data.TileProfile, false);
                }
            }
            else if (e.Type == restoreActionKey)
            {
                Packet data = e.ReadAs<Packet>();
                string locName = data.LocationName;
                if (data == null || data.LayerId == null || string.IsNullOrEmpty(locName))
                    return;

                if (saveMng.session != null)
                {
                    saveMng.session.GetLocationProfileSafe(locName).RestoreTile(data.Position, data.LayerId, false);
                }
            }
        }

        private void OnPeerConnected(object sender, StardewModdingAPI.Events.PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                return;

            Helper.Multiplayer.SendMessage(CoreManager.Instance.saveManager.session, sessionDataKey, toModIDs, new[] { e.Peer.PlayerID });
        }
    }
}