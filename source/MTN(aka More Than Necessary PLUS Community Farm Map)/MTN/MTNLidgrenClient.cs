using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN
{
    public class MTNLidgrenClient : LidgrenClient
    {
        public ulong uploadByteCount = 0;

        public MTNLidgrenClient(string address) : base(address)
        {

        }

        protected override void receiveServerIntroduction(BinaryReader msg)
        {
            sendMessage(50, new object[] {
                Utilities.composeModList()
            });
            Game1.otherFarmers.Roots[Game1.player.UniqueMultiplayerID] = (Game1.player.NetFields.Root as NetFarmerRoot);
            NetFarmerRoot f = Memory.multiplayer.readFarmer(msg);
            long id = f.Value.UniqueMultiplayerID;
            Game1.serverHost = f;
            Game1.serverHost.Value.teamRoot = Memory.multiplayer.readObjectFull<FarmerTeam>(msg);
            Game1.otherFarmers.Roots.Add(id, f);
            Game1.player.teamRoot = Game1.serverHost.Value.teamRoot;
            Game1.netWorldState = Memory.multiplayer.readObjectFull<IWorldState>(msg);
            Game1.netWorldState.Clock.InterpolationTicks = 0;
            Game1.netWorldState.Value.WriteToGame1();

            setUpGame();
            if (Game1.chatBox != null)
            {
                Game1.chatBox.listPlayers();
            }
        }

        protected override void setUpGame()
        {
            GameLocation home = Game1.getLocationFromName(Game1.player.homeLocation.Value);
            Game1.currentLocation = home;
            Game1.player.currentLocation = home;
            Game1.player.updateFriendshipGifts(Game1.Date);
            Game1.gameMode = 3;
            Game1.multiplayerMode = 1;
            Game1.client = this;
            readyToPlay = true;
            Game1.fadeClear();
            Game1.currentLocation.resetForPlayerEntry();
            Game1.exitActiveMenu();
            if (!Game1.player.isCustomized)
            {
                Game1.activeClickableMenu = new CharacterCustomization(CharacterCustomization.Source.NewFarmhand);
            }
        }

        public override void sendPlayerIntroduction() {
            if (getUserID() != "") {
                Game1.player.userID.Value = getUserID();
            }
            (Game1.player.NetFields.Root as NetRoot<Farmer>).MarkClean();
            
            sendMessage(2, new object[]
            {
                Memory.multiplayer.writeObjectFullBytes<Farmer>(Game1.player.NetFields.Root as NetFarmerRoot, null)
            });
        }

        public override void sendMessage(OutgoingMessage message) {
            base.sendMessage(message);
            if (message.Data != null) {
                for (int i = 0; i < message.Data.Count; i++) {
                    uploadByteCount += (ulong)(message.Data[i].ToString()).Length;
                }
            }
        }

        public ulong readUploadAmount() {
            ulong results = uploadByteCount;
            uploadByteCount = 0;
            return results;
        }
    }
}
