using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.IO;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Utilities;

using StardewValley.Network;


namespace anticheatviachat
{
    public class ModEntry : Mod
    {
        private int connectedFarmers = Game1.otherFarmers.Count;
        private HashSet<long> playerIDs = new HashSet<long>();
        private HashSet<long> playerIDsFromMod = new HashSet<long>();

        //connection slots
        public bool slot1Used = false;
        public bool slot2Used = false;
        public bool slot3Used = false;

        //multiplayerID Variables
        public long multiplayerID1;
        public long multiplayerID2;
        public long multiplayerID3;

        //GalaxyID Variables
        public long galaxyID1;
        public long galaxyID2;
        public long galaxyID3;

        //Private counter for each slot to give time between new player joined and check if received ID
        private bool slot1CountDown = false;
        private bool slot2CountDown = false;
        private bool slot3CountDown = false;


        public override void Entry(IModHelper helper)
        {
            GameEvents.OneSecondTick += this.GameEvents_OneSecondTick;
            GameEvents.FourthUpdateTick += this.GameEvents_FourthUpdateTick;
        }

        private void GameEvents_FourthUpdateTick (object sender, EventArgs e)
        {
            //grabs last line of chat which should be player ID
            List<ChatMessage> messages = this.Helper.Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();
            string[] messageDumpString = messages.SelectMany(p => p.message).Select(p => p.message).ToArray();
            lastFragment = messageDumpString.Last().Split(':').Last().Trim();

            if (long.TryParse(lastFragment, out long lastID))
            {
                this.Monitor.Log($"{lastID}");
                playerIDsFromMod.Add(lastID);
            }
        }





        private void GameEvents_OneSecondTick(object sender, EventArgs e)  
        {
            // if someone new connects to the game store their ID's in am unused variable slot
            if (connectedFarmersCount < Game1.otherFarmers.Count)
            {
                if (slot1Used = false)
                {
                    multiplayerID1 = 1/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    galaxyID1 = 1/*newest unique GalaxyID in connections()*/;
                    connectedFarmersCount += 1;
                    slot1CountDown = true;
                    slot1Used = true;
                    return;
                }

                if (slot2Used = false)
                {
                    multiplayerID2 = 2/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    galaxyID2 = 2/*newest unique GalaxyID in connections()*/;
                    connectedFarmersCount += 1;
                    slot2CountDown = true;
                    slot2Used = true;
                    return;
                }

                if (slot3Used = false)
                {
                    multiplayerID3 = 3/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    galaxyID3 = 3/*newest unique GalaxyID in connections()*/;
                    connectedFarmersCount += 1;
                    slot3CountDown = true;
                    slot3Used = true;
                    return;
                }
            }

            //if someone disconnects from the game find out who it was and open up their used variable slot
            if (connectedFarmersCount > Game1.otherFarmers.Count)
            {
                playerIDs.Clear();
                //store all game MultiplayerIDs to a list
                foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
                {
                    playerIDs.Add(onlineFarmer.UniqueMultiplayerID);
                }

                if (slot1Used = true)
                {
                    if (!playerIDs.Contains(multiplayerID1))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID1))
                        {
                            playerIDsFromMod.Remove(multiplayerID1);
                        }
                        connectedFarmersCount -= 1;
                        slot1Used = false;
                    }
                }

                if (slot2Used = true)
                {
                    if (!playerIDs.Contains(multiplayerID2))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID2))
                        {
                            playerIDsFromMod.Remove(multiplayerID2);
                        }
                        connectedFarmersCount -= 1;
                        slot2Used = false;
                    }
                }

                if (slot3Used = true)
                {
                    if (!playerIDs.Contains(multiplayerID3))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID3))
                        {
                            playerIDsFromMod.Remove(multiplayerID3);
                        }
                        connectedFarmersCount -= 1;
                        slot3Used = false;
                    }
                }

            }
            
            //if a slot is used countdown until check if they sent their ID through mod, if not kick
            if (slot1CountDown == true)
            {
                oneSecondTicks += 1;
                if (oneSecondTicks >= 30)
                {
                    if(!playerIDsFromMod.Contains(multiplayerID1))
                    {
                        //kick galaxyID1
                        //kick to void for now
                        Game1.server.sendMessage(multiplayerID1, new OutgoingMessage((byte)19, multiplayerID1, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID1);

                        slot1Used = false;
                    }
                    oneSecondTicks = 0;
                    slot1CountDown = false;
                }
            }

            if (slot2CountDown == true)
            {
                oneSecondTicks += 1;
                if (oneSecondTicks >= 30)
                {
                    if (!playerIDsFromMod.Contains(multiplayerID2))
                    {
                        //kick galaxyID1
                        //kick to void for now
                        Game1.server.sendMessage(multiplayerID2, new OutgoingMessage((byte)19, multiplayerID2, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID2);


                        slot2Used = false;
                    }
                    oneSecondTicks = 0;
                    slot2CountDown = false;
                }
            }

            if (slot3CountDown == true)
            {
                oneSecondTicks += 1;
                if (oneSecondTicks >= 30)
                {
                    if (!playerIDsFromMod.Contains(multiplayerID3))
                    {
                        //kick galaxyID1
                        //kick to void for now
                        Game1.server.sendMessage(multiplayerID3, new OutgoingMessage((byte)19, multiplayerID3, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID3);
                        slot3Used = false;
                    }
                    oneSecondTicks = 0;
                    slot3CountDown = false;
                }
            }


        }
    }
}
