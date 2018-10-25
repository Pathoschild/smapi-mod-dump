using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewValley.SDKs;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Galaxy.Api;

namespace anticheatviachat
{
    public class ModEntry : Mod
    {
        private int connectedFarmersCount = Game1.otherFarmers.Count;
        private HashSet<long> playerIDs = new HashSet<long>();
        private HashSet<long> playerIDsFromMod = new HashSet<long>();
        //private HashSet<GalaxyID> galaxyIDs = new HashSet<GalaxyID>();
        private bool antiCheatMessageSent = false;
        private int kickTimer = 60; // how long in seconds before checking if a player sent id.

        private string currentPass = "SCAG"; //current code password 4 characters only
        //private string lastFragment = "this is the last sentence in chat";
        //private string chatMessageCheck = "this is to gate the check on the variable lastFragment";


        //connection slots
        private bool slot1Used = false;
        private bool slot2Used = false;
        private bool slot3Used = false;
        private bool slot4Used = false;
        private bool slot5Used = false;
        private bool slot6Used = false;
        private bool slot7Used = false;
        private bool slot8Used = false;
        private bool slot9Used = false;
        private bool slot10Used = false;
        private bool slot11Used = false;
        private bool slot12Used = false;
        private bool slot13Used = false;
        private bool slot14Used = false;
        private bool slot15Used = false;
        private bool slot16Used = false;
        private bool slot17Used = false;
        private bool slot18Used = false;
        private bool slot19Used = false;
        private bool slot20Used = false;
        private bool slot21Used = false;
        private bool slot22Used = false;
        private bool slot23Used = false;
        private bool slot24Used = false;
        private bool slot25Used = false;
        private bool slot26Used = false;
        private bool slot27Used = false;
        private bool slot28Used = false;
        private bool slot29Used = false;
        private bool slot30Used = false;



        //multiplayerID Variables
        private long multiplayerID1;
        private long multiplayerID2;
        private long multiplayerID3;
        private long multiplayerID4;
        private long multiplayerID5;
        private long multiplayerID6;
        private long multiplayerID7;
        private long multiplayerID8;
        private long multiplayerID9;
        private long multiplayerID10;
        private long multiplayerID11;
        private long multiplayerID12;
        private long multiplayerID13;
        private long multiplayerID14;
        private long multiplayerID15;
        private long multiplayerID16;
        private long multiplayerID17;
        private long multiplayerID18;
        private long multiplayerID19;
        private long multiplayerID20;
        private long multiplayerID21;
        private long multiplayerID22;
        private long multiplayerID23;
        private long multiplayerID24;
        private long multiplayerID25;
        private long multiplayerID26;
        private long multiplayerID27;
        private long multiplayerID28;
        private long multiplayerID29;
        private long multiplayerID30;

        //GalaxyID Variables
        /*private GalaxyID galaxyID1;
        private GalaxyID galaxyID2;
        private GalaxyID galaxyID3;
        private GalaxyID galaxyID4;
        private GalaxyID galaxyID5;
        private GalaxyID galaxyID6;
        private GalaxyID galaxyID7;
        private GalaxyID galaxyID8;
        private GalaxyID galaxyID9;
        private GalaxyID galaxyID10;
        private GalaxyID galaxyID11;
        private GalaxyID galaxyID12;
        private GalaxyID galaxyID13;
        private GalaxyID galaxyID14;
        private GalaxyID galaxyID15;
        private GalaxyID galaxyID16;
        private GalaxyID galaxyID17;
        private GalaxyID galaxyID18;
        private GalaxyID galaxyID19;
        private GalaxyID galaxyID20;
        private GalaxyID galaxyID21;
        private GalaxyID galaxyID22;
        private GalaxyID galaxyID23;
        private GalaxyID galaxyID24;
        private GalaxyID galaxyID25;
        private GalaxyID galaxyID26;
        private GalaxyID galaxyID27;
        private GalaxyID galaxyID28;
        private GalaxyID galaxyID29;
        private GalaxyID galaxyID30;*/

        //Private counter for each slot to give time between new player joined and check if received ID
        private bool slot1CountDown = false;
        private bool slot2CountDown = false;
        private bool slot3CountDown = false;
        private bool slot4CountDown = false;
        private bool slot5CountDown = false;
        private bool slot6CountDown = false;
        private bool slot7CountDown = false;
        private bool slot8CountDown = false;
        private bool slot9CountDown = false;
        private bool slot10CountDown = false;
        private bool slot11CountDown = false;
        private bool slot12CountDown = false;
        private bool slot13CountDown = false;
        private bool slot14CountDown = false;
        private bool slot15CountDown = false;
        private bool slot16CountDown = false;
        private bool slot17CountDown = false;
        private bool slot18CountDown = false;
        private bool slot19CountDown = false;
        private bool slot20CountDown = false;
        private bool slot21CountDown = false;
        private bool slot22CountDown = false;
        private bool slot23CountDown = false;
        private bool slot24CountDown = false;
        private bool slot25CountDown = false;
        private bool slot26CountDown = false;
        private bool slot27CountDown = false;
        private bool slot28CountDown = false;
        private bool slot29CountDown = false;
        private bool slot30CountDown = false;

        //ticks for each countdown counter
        private int oneSecondTicks1;
        private int oneSecondTicks2;
        private int oneSecondTicks3;
        private int oneSecondTicks4;
        private int oneSecondTicks5;
        private int oneSecondTicks6;
        private int oneSecondTicks7;
        private int oneSecondTicks8;
        private int oneSecondTicks9;
        private int oneSecondTicks10;
        private int oneSecondTicks11;
        private int oneSecondTicks12;
        private int oneSecondTicks13;
        private int oneSecondTicks14;
        private int oneSecondTicks15;
        private int oneSecondTicks16;
        private int oneSecondTicks17;
        private int oneSecondTicks18;
        private int oneSecondTicks19;
        private int oneSecondTicks20;
        private int oneSecondTicks21;
        private int oneSecondTicks22;
        private int oneSecondTicks23;
        private int oneSecondTicks24;
        private int oneSecondTicks25;
        private int oneSecondTicks26;
        private int oneSecondTicks27;
        private int oneSecondTicks28;
        private int oneSecondTicks29;
        private int oneSecondTicks30;

        //Private counter for each Galaxy ID kick
        /*private bool galaxy1CountDown = false;
        private bool galaxy2CountDown = false;
        private bool galaxy3CountDown = false;
        private bool galaxy4CountDown = false;
        private bool galaxy5CountDown = false;
        private bool galaxy6CountDown = false;
        private bool galaxy7CountDown = false;
        private bool galaxy8CountDown = false;
        private bool galaxy9CountDown = false;
        private bool galaxy10CountDown = false;
        private bool galaxy11CountDown = false;
        private bool galaxy12CountDown = false;
        private bool galaxy13CountDown = false;
        private bool galaxy14CountDown = false;
        private bool galaxy15CountDown = false;
        private bool galaxy16CountDown = false;
        private bool galaxy17CountDown = false;
        private bool galaxy18CountDown = false;
        private bool galaxy19CountDown = false;
        private bool galaxy20CountDown = false;
        private bool galaxy21CountDown = false;
        private bool galaxy22CountDown = false;
        private bool galaxy23CountDown = false;
        private bool galaxy24CountDown = false;
        private bool galaxy25CountDown = false;
        private bool galaxy26CountDown = false;
        private bool galaxy27CountDown = false;
        private bool galaxy28CountDown = false;
        private bool galaxy29CountDown = false;
        private bool galaxy30CountDown = false;*/

        //ticks for each galaxyID countdown counter
        /*private int galaxySecondTicks1;
        private int galaxySecondTicks2;
        private int galaxySecondTicks3;
        private int galaxySecondTicks4;
        private int galaxySecondTicks5;
        private int galaxySecondTicks6;
        private int galaxySecondTicks7;
        private int galaxySecondTicks8;
        private int galaxySecondTicks9;
        private int galaxySecondTicks10;
        private int galaxySecondTicks11;
        private int galaxySecondTicks12;
        private int galaxySecondTicks13;
        private int galaxySecondTicks14;
        private int galaxySecondTicks15;
        private int galaxySecondTicks16;
        private int galaxySecondTicks17;
        private int galaxySecondTicks18;
        private int galaxySecondTicks19;
        private int galaxySecondTicks20;
        private int galaxySecondTicks21;
        private int galaxySecondTicks22;
        private int galaxySecondTicks23;
        private int galaxySecondTicks24;
        private int galaxySecondTicks25;
        private int galaxySecondTicks26;
        private int galaxySecondTicks27;
        private int galaxySecondTicks28;
        private int galaxySecondTicks29;
        private int galaxySecondTicks30;*/

        //dont run kick code if they quit manually
        private bool didQuit1 = false;
        private bool didQuit2 = false;
        private bool didQuit3 = false;
        private bool didQuit4 = false;
        private bool didQuit5 = false;
        private bool didQuit6 = false;
        private bool didQuit7 = false;
        private bool didQuit8 = false;
        private bool didQuit9 = false;
        private bool didQuit10 = false;
        private bool didQuit11 = false;
        private bool didQuit12 = false;
        private bool didQuit13 = false;
        private bool didQuit14 = false;
        private bool didQuit15 = false;
        private bool didQuit16 = false;
        private bool didQuit17 = false;
        private bool didQuit18 = false;
        private bool didQuit19 = false;
        private bool didQuit20 = false;
        private bool didQuit21 = false;
        private bool didQuit22 = false;
        private bool didQuit23 = false;
        private bool didQuit24 = false;
        private bool didQuit25 = false;
        private bool didQuit26 = false;
        private bool didQuit27 = false;
        private bool didQuit28 = false;
        private bool didQuit29 = false;
        private bool didQuit30 = false;

        public override void Entry(IModHelper helper)
        {
            GameEvents.OneSecondTick += this.GameEvents_OneSecondTick;
            GameEvents.FourthUpdateTick += this.GameEvents_FourthUpdateTick;
        }

        public void WhatColorToSayWhenKickBefore()
        {
            Game1.chatBox.activate();
            Game1.chatBox.setText("/color red");
            Game1.chatBox.chatBox.RecieveCommandInput('\r');
        }
        public void WhatToSayWhenKickAfter()
        {
            Game1.chatBox.activate();
            Game1.chatBox.setText("Please Install Latest ServerPack");
            Game1.chatBox.chatBox.RecieveCommandInput('\r');
        }

        private void GameEvents_FourthUpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (antiCheatMessageSent == false)
            {
                Game1.chatBox.activate();
                Game1.chatBox.setText("Anticheat Activated");
                Game1.chatBox.chatBox.RecieveCommandInput('\r');
                antiCheatMessageSent = true;
            }

            //grabs last line of chat which should be player ID
            //if (chatMessageCheck != lastFragment)
            //{
            List<ChatMessage> messages = this.Helper.Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();
            string[] messageDumpString = messages.SelectMany(p => p.message).Select(p => p.message).ToArray();
            string lastFragment = messageDumpString.LastOrDefault()?.Split(':').Last().Trim();
            string cleanFragment = "a";
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //todo put a password checker on first 4 letters for next time servercode is updated, will need to force update
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (lastFragment.Length >= 4 && lastFragment.Substring(0, 4) == $"{currentPass}")
            {
                cleanFragment = lastFragment.Substring(4); //starts a new string at 5th character 
            }

            //chatMessageCheck = lastFragment;
            //}


            if (long.TryParse(cleanFragment, out long lastID))
            {
                if (!playerIDsFromMod.Contains(lastID))
                {
                    this.Monitor.Log($"{lastID}");
                    playerIDsFromMod.Add(lastID);
                }
            }
        }





        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;



            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // if someone new connects to the game store their ID's in am unused variable slot
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (connectedFarmersCount < Game1.otherFarmers.Count)
            {
                //store all game MultiplayerIDs to a list
                playerIDs.Clear();
                foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
                {
                    playerIDs.Add(onlineFarmer.UniqueMultiplayerID);
                }

                //store all game GalaxyIDs to a list
                /*galaxyIDs.Clear();
                IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                foreach (Server server in servers)
                {
                    GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                    if (socket == null)
                        continue;
                    
                        foreach (GalaxyID connection in socket.Connections)
                        {
                            galaxyIDs.Add(connection);
                            this.Monitor.Log("GalaxyID added: " + galaxyIDs.Last().ToString());
                        }
                    
                }*/




                //kick if more than 30 players on
                //disabling for now may use in future
                /*
                if (playerIDs.Count > 30)
                {
                    var removePlayer = playerIDs.Last();
                    //todo kick galaxyID3
                    //kick to void for now
                    Game1.server.sendMessage(removePlayer, new OutgoingMessage((byte)19, removePlayer, new object[0]));
                    Game1.server.playerDisconnected(removePlayer);
                    connectedFarmersCount -= 1;
                }*/






                if (slot1Used == false)
                {
                    multiplayerID1 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/
                    //galaxyID1 = galaxyIDs.LastOrDefault(); /*newest unique GalaxyID in connections()*/
                    connectedFarmersCount += 1;
                    slot1CountDown = true;
                    slot1Used = true;
                    return;
                }

                if (slot2Used == false)
                {
                    multiplayerID2 = playerIDs.LastOrDefault();
                    //galaxyID2 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot2CountDown = true;
                    slot2Used = true;
                    return;
                }

                if (slot3Used == false)
                {
                    multiplayerID3 = playerIDs.LastOrDefault();
                    //galaxyID3 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot3CountDown = true;
                    slot3Used = true;
                    return;
                }
                if (slot4Used == false)
                {
                    multiplayerID4 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID4 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot4CountDown = true;
                    slot4Used = true;
                    return;
                }
                if (slot5Used == false)
                {
                    multiplayerID5 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID5 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot5CountDown = true;
                    slot5Used = true;
                    return;
                }
                if (slot6Used == false)
                {
                    multiplayerID6 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID6 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot6CountDown = true;
                    slot6Used = true;
                    return;
                }
                if (slot7Used == false)
                {
                    multiplayerID7 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID7 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot7CountDown = true;
                    slot7Used = true;
                    return;
                }
                if (slot8Used == false)
                {
                    multiplayerID8 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID8 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot8CountDown = true;
                    slot8Used = true;
                    return;
                }

                if (slot9Used == false)
                {
                    multiplayerID9 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID9 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot9CountDown = true;
                    slot9Used = true;
                    return;
                }
                if (slot10Used == false)
                {
                    multiplayerID10 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID10 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot10CountDown = true;
                    slot10Used = true;
                    return;
                }
                if (slot11Used == false)
                {
                    multiplayerID11 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID11 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot11CountDown = true;
                    slot11Used = true;
                    return;
                }

                if (slot12Used == false)
                {
                    multiplayerID12 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID12 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot12CountDown = true;
                    slot12Used = true;
                    return;
                }

                if (slot13Used == false)
                {
                    multiplayerID13 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID13 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot13CountDown = true;
                    slot13Used = true;
                    return;
                }
                if (slot14Used == false)
                {
                    multiplayerID14 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID14 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot14CountDown = true;
                    slot14Used = true;
                    return;
                }
                if (slot15Used == false)
                {
                    multiplayerID15 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID15 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot15CountDown = true;
                    slot15Used = true;
                    return;
                }
                if (slot16Used == false)
                {
                    multiplayerID16 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID16 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot16CountDown = true;
                    slot16Used = true;
                    return;
                }
                if (slot17Used == false)
                {
                    multiplayerID17 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID17 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot17CountDown = true;
                    slot17Used = true;
                    return;
                }
                if (slot18Used == false)
                {
                    multiplayerID18 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID18 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot18CountDown = true;
                    slot18Used = true;
                    return;
                }
                if (slot19Used == false)
                {
                    multiplayerID19 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID19 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot19CountDown = true;
                    slot19Used = true;
                    return;
                }
                if (slot20Used == false)
                {
                    multiplayerID20 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID20 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot20CountDown = true;
                    slot20Used = true;
                    return;
                }
                if (slot21Used == false)
                {
                    multiplayerID21 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID21 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot21CountDown = true;
                    slot21Used = true;
                    return;
                }

                if (slot22Used == false)
                {
                    multiplayerID22 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID22 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot22CountDown = true;
                    slot22Used = true;
                    return;
                }

                if (slot23Used == false)
                {
                    multiplayerID23 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID23 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot23CountDown = true;
                    slot23Used = true;
                    return;
                }
                if (slot24Used == false)
                {
                    multiplayerID24 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID24 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot24CountDown = true;
                    slot24Used = true;
                    return;
                }
                if (slot25Used == false)
                {
                    multiplayerID25 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID25 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot25CountDown = true;
                    slot25Used = true;
                    return;
                }
                if (slot26Used == false)
                {
                    multiplayerID26 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID26 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot26CountDown = true;
                    slot26Used = true;
                    return;
                }
                if (slot27Used == false)
                {
                    multiplayerID27 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID27 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot27CountDown = true;
                    slot27Used = true;
                    return;
                }
                if (slot28Used == false)
                {
                    multiplayerID28 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID28 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot28CountDown = true;
                    slot28Used = true;
                    return;
                }

                if (slot29Used == false)
                {
                    multiplayerID29 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID29 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot29CountDown = true;
                    slot29Used = true;
                    return;
                }
                if (slot30Used == false)
                {
                    multiplayerID30 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID30 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot30CountDown = true;
                    slot30Used = true;
                    return;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //if someone disconnects from the game find out who it was and open up their used variable slot
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (connectedFarmersCount > Game1.otherFarmers.Count)
            {
                playerIDs.Clear();
                //store all game MultiplayerIDs to a list
                foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
                {
                    playerIDs.Add(onlineFarmer.UniqueMultiplayerID);
                }

                if (slot1Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID1))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID1))
                        {
                            playerIDsFromMod.Remove(multiplayerID1);
                            this.Monitor.Log($"Removing: {multiplayerID1}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit1 = true;
                        slot1Used = false;
                    }
                }
                if (slot2Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID2))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID2))
                        {
                            playerIDsFromMod.Remove(multiplayerID2);
                            this.Monitor.Log($"Removing: {multiplayerID2}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit2 = true;
                        slot2Used = false;
                    }
                }
                if (slot3Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID3))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID3))
                        {
                            playerIDsFromMod.Remove(multiplayerID3);
                            this.Monitor.Log($"Removing: {multiplayerID3}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit3 = true;
                        slot3Used = false;
                    }
                }
                if (slot4Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID4))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID4))
                        {
                            playerIDsFromMod.Remove(multiplayerID4);
                            this.Monitor.Log($"Removing: {multiplayerID4}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit4 = true;
                        slot4Used = false;
                    }
                }
                if (slot5Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID5))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID5))
                        {
                            playerIDsFromMod.Remove(multiplayerID5);
                            this.Monitor.Log($"Removing: {multiplayerID5}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit5 = true;
                        slot5Used = false;
                    }
                }
                if (slot6Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID6))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID6))
                        {
                            playerIDsFromMod.Remove(multiplayerID6);
                            this.Monitor.Log($"Removing: {multiplayerID6}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit6 = true;
                        slot6Used = false;
                    }
                }
                if (slot7Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID7))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID7))
                        {
                            playerIDsFromMod.Remove(multiplayerID7);
                            this.Monitor.Log($"Removing: {multiplayerID7}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit7 = true;
                        slot7Used = false;
                    }
                }
                if (slot8Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID8))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID8))
                        {
                            playerIDsFromMod.Remove(multiplayerID8);
                            this.Monitor.Log($"Removing: {multiplayerID8}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit8 = true;
                        slot8Used = false;
                    }
                }
                if (slot9Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID9))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID9))
                        {
                            playerIDsFromMod.Remove(multiplayerID9);
                            this.Monitor.Log($"Removing: {multiplayerID9}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit9 = true;
                        slot9Used = false;
                    }
                }
                if (slot10Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID10))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID10))
                        {
                            playerIDsFromMod.Remove(multiplayerID10);
                            this.Monitor.Log($"Removing: {multiplayerID10}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit10 = true;
                        slot10Used = false;
                    }
                }
                if (slot11Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID11))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID11))
                        {
                            playerIDsFromMod.Remove(multiplayerID11);
                            this.Monitor.Log($"Removing: {multiplayerID11}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit11 = true;
                        slot11Used = false;
                    }
                }
                if (slot12Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID12))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID12))
                        {
                            playerIDsFromMod.Remove(multiplayerID12);
                            this.Monitor.Log($"Removing: {multiplayerID12}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit12 = true;
                        slot12Used = false;
                    }
                }
                if (slot13Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID13))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID13))
                        {
                            playerIDsFromMod.Remove(multiplayerID13);
                            this.Monitor.Log($"Removing: {multiplayerID13}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit13 = true;
                        slot13Used = false;
                    }
                }
                if (slot14Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID14))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID14))
                        {
                            playerIDsFromMod.Remove(multiplayerID14);
                            this.Monitor.Log($"Removing: {multiplayerID14}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit14 = true;
                        slot14Used = false;
                    }
                }
                if (slot15Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID15))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID15))
                        {
                            playerIDsFromMod.Remove(multiplayerID15);
                            this.Monitor.Log($"Removing: {multiplayerID15}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit15 = true;
                        slot15Used = false;
                    }
                }
                if (slot16Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID16))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID16))
                        {
                            playerIDsFromMod.Remove(multiplayerID16);
                            this.Monitor.Log($"Removing: {multiplayerID16}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit16 = true;
                        slot16Used = false;
                    }
                }
                if (slot17Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID17))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID17))
                        {
                            playerIDsFromMod.Remove(multiplayerID17);
                            this.Monitor.Log($"Removing: {multiplayerID17}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit17 = true;
                        slot17Used = false;
                    }
                }
                if (slot18Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID18))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID18))
                        {
                            playerIDsFromMod.Remove(multiplayerID18);
                            this.Monitor.Log($"Removing: {multiplayerID18}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit18 = true;
                        slot18Used = false;
                    }
                }
                if (slot19Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID19))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID19))
                        {
                            playerIDsFromMod.Remove(multiplayerID19);
                            this.Monitor.Log($"Removing: {multiplayerID19}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit19 = true;
                        slot19Used = false;
                    }
                }
                if (slot20Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID20))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID20))
                        {
                            playerIDsFromMod.Remove(multiplayerID20);
                            this.Monitor.Log($"Removing: {multiplayerID20}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit20 = true;
                        slot20Used = false;
                    }
                }
                if (slot21Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID21))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID21))
                        {
                            playerIDsFromMod.Remove(multiplayerID21);
                            this.Monitor.Log($"Removing: {multiplayerID21}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit21 = true;
                        slot21Used = false;
                    }
                }
                if (slot22Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID22))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID22))
                        {
                            playerIDsFromMod.Remove(multiplayerID22);
                            this.Monitor.Log($"Removing: {multiplayerID22}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit22 = true;
                        slot22Used = false;
                    }
                }
                if (slot23Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID23))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID23))
                        {
                            playerIDsFromMod.Remove(multiplayerID23);
                            this.Monitor.Log($"Removing: {multiplayerID23}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit23 = true;
                        slot23Used = false;
                    }
                }
                if (slot24Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID24))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID24))
                            this.Monitor.Log($"Removing: {multiplayerID24}");
                        {
                            playerIDsFromMod.Remove(multiplayerID24);
                        }
                        connectedFarmersCount -= 1;
                        didQuit24 = true;
                        slot24Used = false;
                    }
                }
                if (slot25Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID25))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID25))
                        {
                            playerIDsFromMod.Remove(multiplayerID25);
                            this.Monitor.Log($"Removing: {multiplayerID25}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit25 = true;
                        slot25Used = false;
                    }
                }
                if (slot26Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID26))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID26))
                        {
                            playerIDsFromMod.Remove(multiplayerID26);
                            this.Monitor.Log($"Removing: {multiplayerID26}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit26 = true;
                        slot26Used = false;
                    }
                }
                if (slot27Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID27))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID27))
                        {
                            playerIDsFromMod.Remove(multiplayerID27);
                            this.Monitor.Log($"Removing: {multiplayerID27}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit27 = true;
                        slot27Used = false;
                    }
                }
                if (slot28Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID28))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID28))
                        {
                            playerIDsFromMod.Remove(multiplayerID28);
                            this.Monitor.Log($"Removing: {multiplayerID28}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit28 = true;
                        slot28Used = false;
                    }
                }
                if (slot29Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID29))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID29))
                        {
                            playerIDsFromMod.Remove(multiplayerID29);
                            this.Monitor.Log($"Removing: {multiplayerID29}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit29 = true;
                        slot29Used = false;
                    }
                }
                if (slot30Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID30))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID30))
                        {
                            playerIDsFromMod.Remove(multiplayerID30);
                            this.Monitor.Log($"Removing: {multiplayerID30}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit30 = true;
                        slot30Used = false;
                    }
                }


            }



            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //if a slot is used countdown until check if they sent their ID through mod, if not kick
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (slot1CountDown == true)
            {

                oneSecondTicks1 += 1;
                if (oneSecondTicks1 >= kickTimer)
                {
                    if (didQuit1 == true)
                    {
                        oneSecondTicks1 = 0;
                        slot1CountDown = false;
                        didQuit1 = false;  
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID1))
                    {

                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID1, new OutgoingMessage((byte)19, multiplayerID1, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID1);
                        //kick Galaxy ID
                        //galaxy1CountDown = true;
                        /////////////////
                       
                        WhatToSayWhenKickAfter();

                        connectedFarmersCount -= 1;
                        slot1Used = false;
                    }
                    oneSecondTicks1 = 0;
                    slot1CountDown = false;
                }
            }

            if (slot2CountDown == true)
            {
                oneSecondTicks2 += 1;
                if (oneSecondTicks2 >= kickTimer)
                {
                    if (didQuit2 == true)
                    {
                        oneSecondTicks2 = 0;
                        slot2CountDown = false;
                        didQuit2 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID2))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID2, new OutgoingMessage((byte)19, multiplayerID2, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID2);

                        //kick Galaxy ID
                        //galaxy2CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();

                        connectedFarmersCount -= 1;
                        slot2Used = false;
                    }
                    oneSecondTicks2 = 0;
                    slot2CountDown = false;
                }
            }

            if (slot3CountDown == true)
            {
                oneSecondTicks3 += 1;
                if (oneSecondTicks3 >= kickTimer)
                {
                    if (didQuit3 == true)
                    {
                        oneSecondTicks3 = 0;
                        slot3CountDown = false;
                        didQuit3 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID3))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID3, new OutgoingMessage((byte)19, multiplayerID3, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID3);

                        //kick Galaxy ID
                        //galaxy3CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();

                        connectedFarmersCount -= 1;
                        slot3Used = false;
                    }
                    oneSecondTicks3 = 0;
                    slot3CountDown = false;
                }
            }

            if (slot4CountDown == true)
            {

                oneSecondTicks4 += 1;
                if (oneSecondTicks4 >= kickTimer)
                {
                    if (didQuit4 == true)
                    {
                        oneSecondTicks4 = 0;
                        slot4CountDown = false;
                        didQuit4 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID4))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID4, new OutgoingMessage((byte)19, multiplayerID4, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID4);

                        //kick Galaxy ID
                        //galaxy4CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot4Used = false;
                    }
                    oneSecondTicks4 = 0;
                    slot4CountDown = false;
                }
            }

            if (slot5CountDown == true)
            {
                oneSecondTicks5 += 1;
                if (oneSecondTicks5 >= kickTimer)
                {
                    if (didQuit5 == true)
                    {
                        oneSecondTicks5 = 0;
                        slot5CountDown = false;
                        didQuit5 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID5))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID5, new OutgoingMessage((byte)19, multiplayerID5, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID5);

                        //kick Galaxy ID
                        //galaxy5CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot5Used = false;
                    }
                    oneSecondTicks5 = 0;
                    slot5CountDown = false;
                }
            }

            if (slot6CountDown == true)
            {
                oneSecondTicks6 += 1;
                if (oneSecondTicks6 >= kickTimer)
                {

                    if (didQuit6 == true)
                    {
                        oneSecondTicks6 = 0;
                        slot6CountDown = false;
                        didQuit6 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID6))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID6, new OutgoingMessage((byte)19, multiplayerID6, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID6);

                        //kick Galaxy ID
                        //galaxy6CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot6Used = false;
                    }
                    oneSecondTicks6 = 0;
                    slot6CountDown = false;
                }
            }
            if (slot7CountDown == true)
            {
                oneSecondTicks7 += 1;
                if (oneSecondTicks7 >= kickTimer)
                {
                    if (didQuit7 == true)
                    {
                        oneSecondTicks7 = 0;
                        slot7CountDown = false;
                        didQuit7 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID7))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID7, new OutgoingMessage((byte)19, multiplayerID7, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID7);

                        //kick Galaxy ID
                        //galaxy7CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot7Used = false;
                    }
                    oneSecondTicks7 = 0;
                    slot7CountDown = false;
                }
            }
            if (slot8CountDown == true)
            {
                oneSecondTicks8 += 1;
                if (oneSecondTicks8 >= kickTimer)
                {
                    if (didQuit8 == true)
                    {
                        oneSecondTicks8 = 0;
                        slot8CountDown = false;
                        didQuit8 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID8))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID8, new OutgoingMessage((byte)19, multiplayerID8, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID8);

                        //kick Galaxy ID
                        //galaxy8CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot8Used = false;
                    }
                    oneSecondTicks8 = 0;
                    slot8CountDown = false;
                }
            }
            if (slot9CountDown == true)
            {
                oneSecondTicks9 += 1;
                if (oneSecondTicks9 >= kickTimer)
                {
                    if (didQuit9 == true)
                    {
                        oneSecondTicks9 = 0;
                        slot9CountDown = false;
                        didQuit9 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID9))
                    {
                        WhatColorToSayWhenKickBefore();
                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID9, new OutgoingMessage((byte)19, multiplayerID9, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID9);

                        //kick Galaxy ID
                        //galaxy9CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot9Used = false;
                    }
                    oneSecondTicks9 = 0;
                    slot9CountDown = false;
                }
            }
            if (slot10CountDown == true)
            {
                oneSecondTicks10 += 1;
                if (oneSecondTicks10 >= kickTimer)
                {
                    if (didQuit10 == true)
                    {
                        oneSecondTicks10 = 0;
                        slot10CountDown = false;
                        didQuit10 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID10))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID10, new OutgoingMessage((byte)19, multiplayerID10, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID10);

                        //kick Galaxy ID
                        //galaxy10CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot10Used = false;
                    }
                    oneSecondTicks10 = 0;
                    slot10CountDown = false;
                }
            }
            if (slot11CountDown == true)
            {

                oneSecondTicks11 += 1;
                if (oneSecondTicks11 >= kickTimer)
                {
                    if (didQuit11 == true)
                    {
                        oneSecondTicks11 = 0;
                        slot11CountDown = false;
                        didQuit11 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID11))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID11, new OutgoingMessage((byte)19, multiplayerID11, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID11);

                        //kick Galaxy ID
                        //galaxy11CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();

                        connectedFarmersCount -= 1;
                        slot11Used = false;
                    }
                    oneSecondTicks11 = 0;
                    slot11CountDown = false;
                }
            }

            if (slot12CountDown == true)
            {
                oneSecondTicks12 += 1;
                if (oneSecondTicks12 >= kickTimer)
                {
                    if (didQuit12 == true)
                    {
                        oneSecondTicks12 = 0;
                        slot12CountDown = false;
                        didQuit12 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID12))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID12, new OutgoingMessage((byte)19, multiplayerID12, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID12);

                        //kick Galaxy ID
                        //galaxy12CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot12Used = false;
                    }
                    oneSecondTicks12 = 0;
                    slot12CountDown = false;
                }
            }

            if (slot13CountDown == true)
            {
                oneSecondTicks13 += 1;
                if (oneSecondTicks13 >= kickTimer)
                {
                    if (didQuit13 == true)
                    {
                        oneSecondTicks13 = 0;
                        slot13CountDown = false;
                        didQuit13 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID13))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID13, new OutgoingMessage((byte)19, multiplayerID13, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID13);

                        //kick Galaxy ID
                        //galaxy13CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot13Used = false;
                    }
                    oneSecondTicks13 = 0;
                    slot13CountDown = false;
                }
            }

            if (slot14CountDown == true)
            {

                oneSecondTicks14 += 1;
                if (oneSecondTicks14 >= kickTimer)
                {
                    if (didQuit14 == true)
                    {
                        oneSecondTicks14 = 0;
                        slot14CountDown = false;
                        didQuit14 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID14))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID14, new OutgoingMessage((byte)19, multiplayerID14, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID14);

                        //kick Galaxy ID
                        //galaxy14CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot14Used = false;
                    }
                    oneSecondTicks14 = 0;
                    slot14CountDown = false;
                }
            }

            if (slot15CountDown == true)
            {
                oneSecondTicks5 += 1;
                if (oneSecondTicks15 >= kickTimer)
                {
                    if (didQuit15 == true)
                    {
                        oneSecondTicks15 = 0;
                        slot15CountDown = false;
                        didQuit15 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID15))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID15, new OutgoingMessage((byte)19, multiplayerID15, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID15);

                        //kick Galaxy ID
                        //galaxy15CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot15Used = false;
                    }
                    oneSecondTicks15 = 0;
                    slot15CountDown = false;
                }
            }

            if (slot16CountDown == true)
            {
                oneSecondTicks16 += 1;
                if (oneSecondTicks16 >= kickTimer)
                {
                    if (didQuit16 == true)
                    {
                        oneSecondTicks16 = 0;
                        slot16CountDown = false;
                        didQuit16 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID16))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID16, new OutgoingMessage((byte)19, multiplayerID16, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID16);

                        //kick Galaxy ID
                        //galaxy16CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot16Used = false;
                    }
                    oneSecondTicks16 = 0;
                    slot16CountDown = false;
                }
            }
            if (slot17CountDown == true)
            {
                oneSecondTicks17 += 1;
                if (oneSecondTicks17 >= kickTimer)
                {
                    if (didQuit17 == true)
                    {
                        oneSecondTicks17 = 0;
                        slot17CountDown = false;
                        didQuit17 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID17))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID17, new OutgoingMessage((byte)19, multiplayerID17, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID17);

                        //kick Galaxy ID
                        //galaxy17CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot17Used = false;
                    }
                    oneSecondTicks17 = 0;
                    slot17CountDown = false;
                }
            }
            if (slot18CountDown == true)
            {
                oneSecondTicks18 += 1;
                if (oneSecondTicks18 >= kickTimer)
                {
                    if (didQuit18 == true)
                    {
                        oneSecondTicks18 = 0;
                        slot18CountDown = false;
                        didQuit18 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID18))
                    {
                        WhatColorToSayWhenKickBefore();
                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID18, new OutgoingMessage((byte)19, multiplayerID18, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID18);

                        //kick Galaxy ID
                        //galaxy18CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot18Used = false;
                    }
                    oneSecondTicks18 = 0;
                    slot18CountDown = false;
                }
            }
            if (slot19CountDown == true)
            {
                oneSecondTicks19 += 1;
                if (oneSecondTicks19 >= kickTimer)
                {
                    if (didQuit19 == true)
                    {
                        oneSecondTicks19 = 0;
                        slot19CountDown = false;
                        didQuit19 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID19))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID19, new OutgoingMessage((byte)19, multiplayerID19, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID19);

                        //kick Galaxy ID
                        //galaxy19CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot19Used = false;
                    }
                    oneSecondTicks19 = 0;
                    slot19CountDown = false;
                }
            }
            if (slot20CountDown == true)
            {
                oneSecondTicks20 += 1;
                if (oneSecondTicks20 >= kickTimer)
                {
                    if (didQuit20 == true)
                    {
                        oneSecondTicks20 = 0;
                        slot20CountDown = false;
                        didQuit20 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID20))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID20, new OutgoingMessage((byte)19, multiplayerID20, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID20);

                        //kick Galaxy ID
                        //galaxy20CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot20Used = false;
                    }
                    oneSecondTicks20 = 0;
                    slot20CountDown = false;
                }
            }
            if (slot21CountDown == true)
            {

                oneSecondTicks21 += 1;
                if (oneSecondTicks21 >= kickTimer)
                {
                    if (didQuit21 == true)
                    {
                        oneSecondTicks21 = 0;
                        slot21CountDown = false;
                        didQuit21 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID21))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID21, new OutgoingMessage((byte)19, multiplayerID21, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID21);

                        //kick Galaxy ID
                        //galaxy21CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();

                        connectedFarmersCount -= 1;
                        slot21Used = false;
                    }
                    oneSecondTicks21 = 0;
                    slot21CountDown = false;
                }
            }

            if (slot22CountDown == true)
            {
                oneSecondTicks22 += 1;
                if (oneSecondTicks22 >= kickTimer)
                {
                    if (didQuit22 == true)
                    {
                        oneSecondTicks22 = 0;
                        slot22CountDown = false;
                        didQuit22 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID22))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID22, new OutgoingMessage((byte)19, multiplayerID22, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID22);

                        //kick Galaxy ID
                        //galaxy22CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot22Used = false;
                    }
                    oneSecondTicks22 = 0;
                    slot22CountDown = false;
                }
            }

            if (slot23CountDown == true)
            {
                oneSecondTicks23 += 1;
                if (oneSecondTicks23 >= kickTimer)
                {
                    if (didQuit23 == true)
                    {
                        oneSecondTicks23 = 0;
                        slot23CountDown = false;
                        didQuit23 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID23))
                    {
                        WhatColorToSayWhenKickBefore();
                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID23, new OutgoingMessage((byte)19, multiplayerID23, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID23);

                        //kick Galaxy ID
                        //galaxy23CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot23Used = false;
                    }
                    oneSecondTicks23 = 0;
                    slot23CountDown = false;
                }
            }

            if (slot24CountDown == true)
            {

                oneSecondTicks24 += 1;
                if (oneSecondTicks24 >= kickTimer)
                {
                    if (didQuit24 == true)
                    {
                        oneSecondTicks24 = 0;
                        slot24CountDown = false;
                        didQuit24 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID24))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID24, new OutgoingMessage((byte)19, multiplayerID24, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID24);

                        //kick Galaxy ID
                        //galaxy24CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot24Used = false;
                    }
                    oneSecondTicks24 = 0;
                    slot24CountDown = false;
                }
            }

            if (slot25CountDown == true)
            {
                oneSecondTicks25 += 1;
                if (oneSecondTicks25 >= kickTimer)
                {
                    if (didQuit25 == true)
                    {
                        oneSecondTicks25 = 0;
                        slot25CountDown = false;
                        didQuit25 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID25))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID25, new OutgoingMessage((byte)19, multiplayerID25, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID25);

                        //kick Galaxy ID
                        //galaxy25CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot25Used = false;
                    }
                    oneSecondTicks25 = 0;
                    slot25CountDown = false;
                }
            }

            if (slot26CountDown == true)
            {
                oneSecondTicks26 += 1;
                if (oneSecondTicks26 >= kickTimer)
                {
                    if (didQuit26 == true)
                    {
                        oneSecondTicks26 = 0;
                        slot26CountDown = false;
                        didQuit26 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID26))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID26, new OutgoingMessage((byte)19, multiplayerID26, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID26);

                        //kick Galaxy ID
                        //galaxy26CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot26Used = false;
                    }
                    oneSecondTicks26 = 0;
                    slot26CountDown = false;
                }
            }
            if (slot27CountDown == true)
            {
                oneSecondTicks27 += 1;
                if (oneSecondTicks27 >= kickTimer)
                {
                    if (didQuit27 == true)
                    {
                        oneSecondTicks27 = 0;
                        slot27CountDown = false;
                        didQuit27 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID27))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID27, new OutgoingMessage((byte)19, multiplayerID27, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID27);

                        //kick Galaxy ID
                        //galaxy27CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot27Used = false;
                    }
                    oneSecondTicks27 = 0;
                    slot27CountDown = false;
                }
            }
            if (slot28CountDown == true)
            {
                oneSecondTicks28 += 1;
                if (oneSecondTicks28 >= kickTimer)
                {
                    if (didQuit28 == true)
                    {
                        oneSecondTicks28 = 0;
                        //slot28CountDown = false;
                        didQuit28 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID28))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID28, new OutgoingMessage((byte)19, multiplayerID28, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID28);

                        //kick Galaxy ID
                        //galaxy28CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot28Used = false;
                    }
                    oneSecondTicks28 = 0;
                    slot28CountDown = false;
                }
            }
            if (slot29CountDown == true)
            {
                oneSecondTicks29 += 1;
                if (oneSecondTicks29 >= kickTimer)
                {
                    if (didQuit29 == true)
                    {
                        oneSecondTicks29 = 0;
                        slot29CountDown = false;
                        didQuit29 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID29))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID29, new OutgoingMessage((byte)19, multiplayerID29, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID29);

                        //kick Galaxy ID
                        //galaxy29CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot29Used = false;
                    }
                    oneSecondTicks29 = 0;
                    slot29CountDown = false;
                }
            }
            if (slot30CountDown == true)
            {
                oneSecondTicks30 += 1;
                if (oneSecondTicks30 >= kickTimer)
                {
                    if (didQuit30 == true)
                    {
                        oneSecondTicks30 = 0;
                        slot30CountDown = false;
                        didQuit30 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID30))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID30, new OutgoingMessage((byte)19, multiplayerID30, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID30);

                        //kick Galaxy ID
                        //galaxy30CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot30Used = false;
                    }
                    oneSecondTicks30 = 0;
                    slot30CountDown = false;
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //countdown from kick until full galaxyID kick
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /*if (galaxy1CountDown == true)
            {
                galaxySecondTicks1 += 1;
                if (galaxySecondTicks1 >= 10)
                {
                    if (galaxyID1 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID1);
                            this.Monitor.Log("Kicking " + galaxyID1.ToString());
                        }
                    }
                    galaxy1CountDown = false;
                    galaxySecondTicks1 = 0;
                }
            }

            if (galaxy2CountDown == true)
            {
                galaxySecondTicks2 += 1;
                if (galaxySecondTicks2 >= 10)
                {
                    if (galaxyID2 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID2);
                            this.Monitor.Log("Kicking " + galaxyID2.ToString());
                        }
                    }
                    galaxy2CountDown = false;
                    galaxySecondTicks2 = 0;
                }
            }

            if (galaxy3CountDown == true)
            {
                galaxySecondTicks3 += 1;
                if (galaxySecondTicks3 >= 10)
                {
                    if (galaxyID3 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID3);
                            this.Monitor.Log("Kicking " + galaxyID3.ToString());
                        }
                    }
                    galaxy3CountDown = false;
                    galaxySecondTicks3 = 0;
                }
            }
            if (galaxy4CountDown == true)
            {
                galaxySecondTicks4 += 1;
                if (galaxySecondTicks4 >= 10)
                {
                    if (galaxyID4 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID4);
                            this.Monitor.Log("Kicking " + galaxyID4.ToString());
                        }
                    }
                    galaxy4CountDown = false;
                    galaxySecondTicks4 = 0;
                }
            }
            if (galaxy5CountDown == true)
            {
                galaxySecondTicks5 += 1;
                if (galaxySecondTicks5 >= 10)
                {
                    if (galaxyID5 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID5);
                            this.Monitor.Log("Kicking " + galaxyID5.ToString());
                        }
                    }
                    galaxy5CountDown = false;
                    galaxySecondTicks5 = 0;
                }
            }

            if (galaxy6CountDown == true)
            {
                galaxySecondTicks6 += 1;
                if (galaxySecondTicks6 >= 10)
                {
                    if (galaxyID6 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID6);
                            this.Monitor.Log("Kicking " + galaxyID6.ToString());
                        }
                    }
                    galaxy6CountDown = false;
                    galaxySecondTicks6 = 0;
                }
            }
            if (galaxy7CountDown == true)
            {
                galaxySecondTicks7 += 1;
                if (galaxySecondTicks7 >= 10)
                {
                    if (galaxyID7 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID7);
                            this.Monitor.Log("Kicking " + galaxyID7.ToString());
                        }
                    }
                    galaxy7CountDown = false;
                    galaxySecondTicks7 = 0;
                }
            }
            if (galaxy8CountDown == true)
            {
                galaxySecondTicks8 += 1;
                if (galaxySecondTicks8 >= 10)
                {
                    if (galaxyID8 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID8);
                            this.Monitor.Log("Kicking " + galaxyID8.ToString());
                        }
                    }
                    galaxy8CountDown = false;
                    galaxySecondTicks8 = 0;
                }
            }
            if (galaxy9CountDown == true)
            {
                galaxySecondTicks9 += 1;
                if (galaxySecondTicks9 >= 10)
                {
                    if (galaxyID9 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID9);
                            this.Monitor.Log("Kicking " + galaxyID9.ToString());
                        }
                    }
                    galaxy9CountDown = false;
                    galaxySecondTicks9 = 0;
                }
            }
            if (galaxy10CountDown == true)
            {
                galaxySecondTicks10 += 1;
                if (galaxySecondTicks10 >= 10)
                {
                    if (galaxyID10 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID10);
                            this.Monitor.Log("Kicking " + galaxyID10.ToString());
                        }
                    }
                    galaxy10CountDown = false;
                    galaxySecondTicks10 = 0;
                }
            }

            if (galaxy11CountDown == true)
            {
                galaxySecondTicks11 += 1;
                if (galaxySecondTicks11 >= 10)
                {
                    if (galaxyID11 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID11);
                            this.Monitor.Log("Kicking " + galaxyID11.ToString());
                        }
                    }
                    galaxy11CountDown = false;
                    galaxySecondTicks11 = 0;
                }
            }
            if (galaxy12CountDown == true)
            {
                galaxySecondTicks12 += 1;
                if (galaxySecondTicks12 >= 10)
                {
                    if (galaxyID12 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID12);
                            this.Monitor.Log("Kicking " + galaxyID12.ToString());
                        }
                    }
                    galaxy12CountDown = false;
                    galaxySecondTicks12 = 0;
                }
            }

            if (galaxy13CountDown == true)
            {
                galaxySecondTicks13 += 1;
                if (galaxySecondTicks13 >= 10)
                {
                    if (galaxyID13 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID13);
                            this.Monitor.Log("Kicking " + galaxyID13.ToString());
                        }
                    }
                    galaxy13CountDown = false;
                    galaxySecondTicks13 = 0;
                }
            }
            if (galaxy14CountDown == true)
            {
                galaxySecondTicks14 += 1;
                if (galaxySecondTicks14 >= 10)
                {
                    if (galaxyID14 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID14);
                            this.Monitor.Log("Kicking " + galaxyID14.ToString());
                        }
                    }
                    galaxy14CountDown = false;
                    galaxySecondTicks14 = 0;
                }
            }
            if (galaxy15CountDown == true)
            {
                galaxySecondTicks15 += 1;
                if (galaxySecondTicks15 >= 10)
                {
                    if (galaxyID15 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID15);
                            this.Monitor.Log("Kicking " + galaxyID15.ToString());
                        }
                    }
                    galaxy15CountDown = false;
                    galaxySecondTicks15 = 0;
                }
            }
            if (galaxy16CountDown == true)
            {
                galaxySecondTicks16 += 1;
                if (galaxySecondTicks16 >= 10)
                {
                    if (galaxyID16 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID16);
                            this.Monitor.Log("Kicking " + galaxyID16.ToString());
                        }
                    }
                    galaxy16CountDown = false;
                    galaxySecondTicks16 = 0;
                }
            }
            if (galaxy17CountDown == true)
            {
                galaxySecondTicks17 += 1;
                if (galaxySecondTicks17 >= 10)
                {
                    if (galaxyID17 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID17);
                            this.Monitor.Log("Kicking " + galaxyID17.ToString());
                        }
                    }
                    galaxy17CountDown = false;
                    galaxySecondTicks17 = 0;
                }
            }
            if (galaxy18CountDown == true)
            {
                galaxySecondTicks18 += 1;
                if (galaxySecondTicks18 >= 10)
                {
                    if (galaxyID18 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID18);
                            this.Monitor.Log("Kicking " + galaxyID18.ToString());
                        }
                    }
                    galaxy18CountDown = false;
                    galaxySecondTicks18 = 0;
                }
            }
            if (galaxy19CountDown == true)
            {
                galaxySecondTicks19 += 1;
                if (galaxySecondTicks19 >= 10)
                {
                    if (galaxyID19 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID19);
                            this.Monitor.Log("Kicking " + galaxyID19.ToString());
                        }
                    }
                    galaxy19CountDown = false;
                    galaxySecondTicks19 = 0;
                }
            }
            if (galaxy20CountDown == true)
            {
                galaxySecondTicks20 += 1;
                if (galaxySecondTicks20 >= 10)
                {
                    if (galaxyID20 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID20);
                            this.Monitor.Log("Kicking " + galaxyID20.ToString());
                        }
                    }
                    galaxy20CountDown = false;
                    galaxySecondTicks20 = 0;
                }
            }
            if (galaxy21CountDown == true)
            {
                galaxySecondTicks21 += 1;
                if (galaxySecondTicks21 >= 10)
                {
                    if (galaxyID21 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID21);
                            this.Monitor.Log("Kicking " + galaxyID21.ToString());
                        }
                    }
                    galaxy21CountDown = false;
                    galaxySecondTicks21 = 0;
                }
            }
            if (galaxy22CountDown == true)
            {
                galaxySecondTicks22 += 1;
                if (galaxySecondTicks22 >= 10)
                {
                    if (galaxyID22 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID22);
                            this.Monitor.Log("Kicking " + galaxyID22.ToString());
                        }
                    }
                    galaxy22CountDown = false;
                    galaxySecondTicks22 = 0;
                }
            }
            if (galaxy23CountDown == true)
            {
                galaxySecondTicks23 += 1;
                if (galaxySecondTicks23 >= 10)
                {
                    if (galaxyID23 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID23);
                            this.Monitor.Log("Kicking " + galaxyID23.ToString());
                        }
                    }
                    galaxy23CountDown = false;
                    galaxySecondTicks23 = 0;
                }
            }
            if (galaxy24CountDown == true)
            {
                galaxySecondTicks24 += 1;
                if (galaxySecondTicks24 >= 10)
                {
                    if (galaxyID24 != null)
using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Locations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StardewValley.SDKs;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Galaxy.Api;

namespace anticheatviachat
{
    public class ModEntry : Mod
    {
        private int connectedFarmersCount = Game1.otherFarmers.Count;
        private HashSet<long> playerIDs = new HashSet<long>();
        private HashSet<long> playerIDsFromMod = new HashSet<long>();
        //private HashSet<GalaxyID> galaxyIDs = new HashSet<GalaxyID>();
        private bool antiCheatMessageSent = false;
        private int kickTimer = 60; // how long in seconds before checking if a player sent id.

        private string currentPass = "SCAK"; //current code password 4 characters only
        //private string lastFragment = "this is the last sentence in chat";
        //private string chatMessageCheck = "this is to gate the check on the variable lastFragment";


        //connection slots
        private bool slot1Used = false;
        private bool slot2Used = false;
        private bool slot3Used = false;
        private bool slot4Used = false;
        private bool slot5Used = false;
        private bool slot6Used = false;
        private bool slot7Used = false;
        private bool slot8Used = false;
        private bool slot9Used = false;
        private bool slot10Used = false;
        private bool slot11Used = false;
        private bool slot12Used = false;
        private bool slot13Used = false;
        private bool slot14Used = false;
        private bool slot15Used = false;
        private bool slot16Used = false;
        private bool slot17Used = false;
        private bool slot18Used = false;
        private bool slot19Used = false;
        private bool slot20Used = false;
        private bool slot21Used = false;
        private bool slot22Used = false;
        private bool slot23Used = false;
        private bool slot24Used = false;
        private bool slot25Used = false;
        private bool slot26Used = false;
        private bool slot27Used = false;
        private bool slot28Used = false;
        private bool slot29Used = false;
        private bool slot30Used = false;



        //multiplayerID Variables
        private long multiplayerID1;
        private long multiplayerID2;
        private long multiplayerID3;
        private long multiplayerID4;
        private long multiplayerID5;
        private long multiplayerID6;
        private long multiplayerID7;
        private long multiplayerID8;
        private long multiplayerID9;
        private long multiplayerID10;
        private long multiplayerID11;
        private long multiplayerID12;
        private long multiplayerID13;
        private long multiplayerID14;
        private long multiplayerID15;
        private long multiplayerID16;
        private long multiplayerID17;
        private long multiplayerID18;
        private long multiplayerID19;
        private long multiplayerID20;
        private long multiplayerID21;
        private long multiplayerID22;
        private long multiplayerID23;
        private long multiplayerID24;
        private long multiplayerID25;
        private long multiplayerID26;
        private long multiplayerID27;
        private long multiplayerID28;
        private long multiplayerID29;
        private long multiplayerID30;

        //GalaxyID Variables
        /*private GalaxyID galaxyID1;
        private GalaxyID galaxyID2;
        private GalaxyID galaxyID3;
        private GalaxyID galaxyID4;
        private GalaxyID galaxyID5;
        private GalaxyID galaxyID6;
        private GalaxyID galaxyID7;
        private GalaxyID galaxyID8;
        private GalaxyID galaxyID9;
        private GalaxyID galaxyID10;
        private GalaxyID galaxyID11;
        private GalaxyID galaxyID12;
        private GalaxyID galaxyID13;
        private GalaxyID galaxyID14;
        private GalaxyID galaxyID15;
        private GalaxyID galaxyID16;
        private GalaxyID galaxyID17;
        private GalaxyID galaxyID18;
        private GalaxyID galaxyID19;
        private GalaxyID galaxyID20;
        private GalaxyID galaxyID21;
        private GalaxyID galaxyID22;
        private GalaxyID galaxyID23;
        private GalaxyID galaxyID24;
        private GalaxyID galaxyID25;
        private GalaxyID galaxyID26;
        private GalaxyID galaxyID27;
        private GalaxyID galaxyID28;
        private GalaxyID galaxyID29;
        private GalaxyID galaxyID30;*/

        //Private counter for each slot to give time between new player joined and check if received ID
        private bool slot1CountDown = false;
        private bool slot2CountDown = false;
        private bool slot3CountDown = false;
        private bool slot4CountDown = false;
        private bool slot5CountDown = false;
        private bool slot6CountDown = false;
        private bool slot7CountDown = false;
        private bool slot8CountDown = false;
        private bool slot9CountDown = false;
        private bool slot10CountDown = false;
        private bool slot11CountDown = false;
        private bool slot12CountDown = false;
        private bool slot13CountDown = false;
        private bool slot14CountDown = false;
        private bool slot15CountDown = false;
        private bool slot16CountDown = false;
        private bool slot17CountDown = false;
        private bool slot18CountDown = false;
        private bool slot19CountDown = false;
        private bool slot20CountDown = false;
        private bool slot21CountDown = false;
        private bool slot22CountDown = false;
        private bool slot23CountDown = false;
        private bool slot24CountDown = false;
        private bool slot25CountDown = false;
        private bool slot26CountDown = false;
        private bool slot27CountDown = false;
        private bool slot28CountDown = false;
        private bool slot29CountDown = false;
        private bool slot30CountDown = false;

        //ticks for each countdown counter
        private int oneSecondTicks1;
        private int oneSecondTicks2;
        private int oneSecondTicks3;
        private int oneSecondTicks4;
        private int oneSecondTicks5;
        private int oneSecondTicks6;
        private int oneSecondTicks7;
        private int oneSecondTicks8;
        private int oneSecondTicks9;
        private int oneSecondTicks10;
        private int oneSecondTicks11;
        private int oneSecondTicks12;
        private int oneSecondTicks13;
        private int oneSecondTicks14;
        private int oneSecondTicks15;
        private int oneSecondTicks16;
        private int oneSecondTicks17;
        private int oneSecondTicks18;
        private int oneSecondTicks19;
        private int oneSecondTicks20;
        private int oneSecondTicks21;
        private int oneSecondTicks22;
        private int oneSecondTicks23;
        private int oneSecondTicks24;
        private int oneSecondTicks25;
        private int oneSecondTicks26;
        private int oneSecondTicks27;
        private int oneSecondTicks28;
        private int oneSecondTicks29;
        private int oneSecondTicks30;

        //Private counter for each Galaxy ID kick
        /*private bool galaxy1CountDown = false;
        private bool galaxy2CountDown = false;
        private bool galaxy3CountDown = false;
        private bool galaxy4CountDown = false;
        private bool galaxy5CountDown = false;
        private bool galaxy6CountDown = false;
        private bool galaxy7CountDown = false;
        private bool galaxy8CountDown = false;
        private bool galaxy9CountDown = false;
        private bool galaxy10CountDown = false;
        private bool galaxy11CountDown = false;
        private bool galaxy12CountDown = false;
        private bool galaxy13CountDown = false;
        private bool galaxy14CountDown = false;
        private bool galaxy15CountDown = false;
        private bool galaxy16CountDown = false;
        private bool galaxy17CountDown = false;
        private bool galaxy18CountDown = false;
        private bool galaxy19CountDown = false;
        private bool galaxy20CountDown = false;
        private bool galaxy21CountDown = false;
        private bool galaxy22CountDown = false;
        private bool galaxy23CountDown = false;
        private bool galaxy24CountDown = false;
        private bool galaxy25CountDown = false;
        private bool galaxy26CountDown = false;
        private bool galaxy27CountDown = false;
        private bool galaxy28CountDown = false;
        private bool galaxy29CountDown = false;
        private bool galaxy30CountDown = false;*/

        //ticks for each galaxyID countdown counter
        /*private int galaxySecondTicks1;
        private int galaxySecondTicks2;
        private int galaxySecondTicks3;
        private int galaxySecondTicks4;
        private int galaxySecondTicks5;
        private int galaxySecondTicks6;
        private int galaxySecondTicks7;
        private int galaxySecondTicks8;
        private int galaxySecondTicks9;
        private int galaxySecondTicks10;
        private int galaxySecondTicks11;
        private int galaxySecondTicks12;
        private int galaxySecondTicks13;
        private int galaxySecondTicks14;
        private int galaxySecondTicks15;
        private int galaxySecondTicks16;
        private int galaxySecondTicks17;
        private int galaxySecondTicks18;
        private int galaxySecondTicks19;
        private int galaxySecondTicks20;
        private int galaxySecondTicks21;
        private int galaxySecondTicks22;
        private int galaxySecondTicks23;
        private int galaxySecondTicks24;
        private int galaxySecondTicks25;
        private int galaxySecondTicks26;
        private int galaxySecondTicks27;
        private int galaxySecondTicks28;
        private int galaxySecondTicks29;
        private int galaxySecondTicks30;*/

        //dont run kick code if they quit manually
        private bool didQuit1 = false;
        private bool didQuit2 = false;
        private bool didQuit3 = false;
        private bool didQuit4 = false;
        private bool didQuit5 = false;
        private bool didQuit6 = false;
        private bool didQuit7 = false;
        private bool didQuit8 = false;
        private bool didQuit9 = false;
        private bool didQuit10 = false;
        private bool didQuit11 = false;
        private bool didQuit12 = false;
        private bool didQuit13 = false;
        private bool didQuit14 = false;
        private bool didQuit15 = false;
        private bool didQuit16 = false;
        private bool didQuit17 = false;
        private bool didQuit18 = false;
        private bool didQuit19 = false;
        private bool didQuit20 = false;
        private bool didQuit21 = false;
        private bool didQuit22 = false;
        private bool didQuit23 = false;
        private bool didQuit24 = false;
        private bool didQuit25 = false;
        private bool didQuit26 = false;
        private bool didQuit27 = false;
        private bool didQuit28 = false;
        private bool didQuit29 = false;
        private bool didQuit30 = false;

        public override void Entry(IModHelper helper)
        {
            GameEvents.OneSecondTick += this.GameEvents_OneSecondTick;
            GameEvents.FourthUpdateTick += this.GameEvents_FourthUpdateTick;
        }

        public void WhatColorToSayWhenKickBefore()
        {
            Game1.chatBox.activate();
            Game1.chatBox.setText("/color red");
            Game1.chatBox.chatBox.RecieveCommandInput('\r');
        }
        public void WhatToSayWhenKickAfter()
        {
            Game1.chatBox.activate();
            Game1.chatBox.setText("Please Install Latest ServerPack");
            Game1.chatBox.chatBox.RecieveCommandInput('\r');
        }

        private void GameEvents_FourthUpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (antiCheatMessageSent == false)
            {
                Game1.chatBox.activate();
                Game1.chatBox.setText("Anticheat Activated");
                Game1.chatBox.chatBox.RecieveCommandInput('\r');
                antiCheatMessageSent = true;
            }

            //grabs last line of chat which should be player ID
            //if (chatMessageCheck != lastFragment)
            //{
            List<ChatMessage> messages = this.Helper.Reflection.GetField<List<ChatMessage>>(Game1.chatBox, "messages").GetValue();
            string[] messageDumpString = messages.SelectMany(p => p.message).Select(p => p.message).ToArray();
            string lastFragment = messageDumpString.LastOrDefault()?.Split(':').Last().Trim();
            string cleanFragment = "a";
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //todo put a password checker on first 4 letters for next time servercode is updated, will need to force update
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (lastFragment.Length >= 4 && lastFragment.Substring(0, 4) == $"{currentPass}")
            {
                cleanFragment = lastFragment.Substring(4); //starts a new string at 5th character 
            }

            //chatMessageCheck = lastFragment;
            //}


            if (long.TryParse(cleanFragment, out long lastID))
            {
                if (!playerIDsFromMod.Contains(lastID))
                {
                    this.Monitor.Log($"{lastID}");
                    playerIDsFromMod.Add(lastID);
                }
            }
        }





        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
                return;



            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // if someone new connects to the game store their ID's in am unused variable slot
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (connectedFarmersCount < Game1.otherFarmers.Count)
            {
                //store all game MultiplayerIDs to a list
                playerIDs.Clear();
                foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
                {
                    playerIDs.Add(onlineFarmer.UniqueMultiplayerID);
                }

                //store all game GalaxyIDs to a list
                /*galaxyIDs.Clear();
                IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                foreach (Server server in servers)
                {
                    GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                    if (socket == null)
                        continue;
                    
                        foreach (GalaxyID connection in socket.Connections)
                        {
                            galaxyIDs.Add(connection);
                            this.Monitor.Log("GalaxyID added: " + galaxyIDs.Last().ToString());
                        }
                    
                }*/




                //kick if more than 30 players on
                //disabling for now may use in future
                /*
                if (playerIDs.Count > 30)
                {
                    var removePlayer = playerIDs.Last();
                    //todo kick galaxyID3
                    //kick to void for now
                    Game1.server.sendMessage(removePlayer, new OutgoingMessage((byte)19, removePlayer, new object[0]));
                    Game1.server.playerDisconnected(removePlayer);
                    connectedFarmersCount -= 1;
                }*/






                if (slot1Used == false)
                {
                    multiplayerID1 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/
                    //galaxyID1 = galaxyIDs.LastOrDefault(); /*newest unique GalaxyID in connections()*/
                    connectedFarmersCount += 1;
                    slot1CountDown = true;
                    slot1Used = true;
                    return;
                }

                if (slot2Used == false)
                {
                    multiplayerID2 = playerIDs.LastOrDefault();
                    //galaxyID2 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot2CountDown = true;
                    slot2Used = true;
                    return;
                }

                if (slot3Used == false)
                {
                    multiplayerID3 = playerIDs.LastOrDefault();
                    //galaxyID3 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot3CountDown = true;
                    slot3Used = true;
                    return;
                }
                if (slot4Used == false)
                {
                    multiplayerID4 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID4 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot4CountDown = true;
                    slot4Used = true;
                    return;
                }
                if (slot5Used == false)
                {
                    multiplayerID5 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID5 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot5CountDown = true;
                    slot5Used = true;
                    return;
                }
                if (slot6Used == false)
                {
                    multiplayerID6 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID6 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot6CountDown = true;
                    slot6Used = true;
                    return;
                }
                if (slot7Used == false)
                {
                    multiplayerID7 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID7 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot7CountDown = true;
                    slot7Used = true;
                    return;
                }
                if (slot8Used == false)
                {
                    multiplayerID8 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID8 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot8CountDown = true;
                    slot8Used = true;
                    return;
                }

                if (slot9Used == false)
                {
                    multiplayerID9 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID9 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot9CountDown = true;
                    slot9Used = true;
                    return;
                }
                if (slot10Used == false)
                {
                    multiplayerID10 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID10 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot10CountDown = true;
                    slot10Used = true;
                    return;
                }
                if (slot11Used == false)
                {
                    multiplayerID11 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID11 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot11CountDown = true;
                    slot11Used = true;
                    return;
                }

                if (slot12Used == false)
                {
                    multiplayerID12 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID12 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot12CountDown = true;
                    slot12Used = true;
                    return;
                }

                if (slot13Used == false)
                {
                    multiplayerID13 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID13 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot13CountDown = true;
                    slot13Used = true;
                    return;
                }
                if (slot14Used == false)
                {
                    multiplayerID14 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID14 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot14CountDown = true;
                    slot14Used = true;
                    return;
                }
                if (slot15Used == false)
                {
                    multiplayerID15 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID15 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot15CountDown = true;
                    slot15Used = true;
                    return;
                }
                if (slot16Used == false)
                {
                    multiplayerID16 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID16 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot16CountDown = true;
                    slot16Used = true;
                    return;
                }
                if (slot17Used == false)
                {
                    multiplayerID17 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID17 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot17CountDown = true;
                    slot17Used = true;
                    return;
                }
                if (slot18Used == false)
                {
                    multiplayerID18 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID18 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot18CountDown = true;
                    slot18Used = true;
                    return;
                }
                if (slot19Used == false)
                {
                    multiplayerID19 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID19 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot19CountDown = true;
                    slot19Used = true;
                    return;
                }
                if (slot20Used == false)
                {
                    multiplayerID20 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID20 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot20CountDown = true;
                    slot20Used = true;
                    return;
                }
                if (slot21Used == false)
                {
                    multiplayerID21 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID21 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot21CountDown = true;
                    slot21Used = true;
                    return;
                }

                if (slot22Used == false)
                {
                    multiplayerID22 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID22 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot22CountDown = true;
                    slot22Used = true;
                    return;
                }

                if (slot23Used == false)
                {
                    multiplayerID23 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID23 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot23CountDown = true;
                    slot23Used = true;
                    return;
                }
                if (slot24Used == false)
                {
                    multiplayerID24 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID24 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot24CountDown = true;
                    slot24Used = true;
                    return;
                }
                if (slot25Used == false)
                {
                    multiplayerID25 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID25 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot25CountDown = true;
                    slot25Used = true;
                    return;
                }
                if (slot26Used == false)
                {
                    multiplayerID26 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID26 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot26CountDown = true;
                    slot26Used = true;
                    return;
                }
                if (slot27Used == false)
                {
                    multiplayerID27 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID27 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot27CountDown = true;
                    slot27Used = true;
                    return;
                }
                if (slot28Used == false)
                {
                    multiplayerID28 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID28 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot28CountDown = true;
                    slot28Used = true;
                    return;
                }

                if (slot29Used == false)
                {
                    multiplayerID29 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID29 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot29CountDown = true;
                    slot29Used = true;
                    return;
                }
                if (slot30Used == false)
                {
                    multiplayerID30 = playerIDs.LastOrDefault();/*newest unique onlineFarmer.UniqueMultiplayerID in Game1.getOnlineFarmers()*/;
                    //galaxyID30 = galaxyIDs.LastOrDefault();
                    connectedFarmersCount += 1;
                    slot30CountDown = true;
                    slot30Used = true;
                    return;
                }
            }

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //if someone disconnects from the game find out who it was and open up their used variable slot
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (connectedFarmersCount > Game1.otherFarmers.Count)
            {
                playerIDs.Clear();
                //store all game MultiplayerIDs to a list
                foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
                {
                    playerIDs.Add(onlineFarmer.UniqueMultiplayerID);
                }

                if (slot1Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID1))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID1))
                        {
                            playerIDsFromMod.Remove(multiplayerID1);
                            this.Monitor.Log($"Removing: {multiplayerID1}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit1 = true;
                        slot1Used = false;
                    }
                }
                if (slot2Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID2))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID2))
                        {
                            playerIDsFromMod.Remove(multiplayerID2);
                            this.Monitor.Log($"Removing: {multiplayerID2}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit2 = true;
                        slot2Used = false;
                    }
                }
                if (slot3Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID3))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID3))
                        {
                            playerIDsFromMod.Remove(multiplayerID3);
                            this.Monitor.Log($"Removing: {multiplayerID3}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit3 = true;
                        slot3Used = false;
                    }
                }
                if (slot4Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID4))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID4))
                        {
                            playerIDsFromMod.Remove(multiplayerID4);
                            this.Monitor.Log($"Removing: {multiplayerID4}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit4 = true;
                        slot4Used = false;
                    }
                }
                if (slot5Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID5))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID5))
                        {
                            playerIDsFromMod.Remove(multiplayerID5);
                            this.Monitor.Log($"Removing: {multiplayerID5}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit5 = true;
                        slot5Used = false;
                    }
                }
                if (slot6Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID6))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID6))
                        {
                            playerIDsFromMod.Remove(multiplayerID6);
                            this.Monitor.Log($"Removing: {multiplayerID6}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit6 = true;
                        slot6Used = false;
                    }
                }
                if (slot7Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID7))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID7))
                        {
                            playerIDsFromMod.Remove(multiplayerID7);
                            this.Monitor.Log($"Removing: {multiplayerID7}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit7 = true;
                        slot7Used = false;
                    }
                }
                if (slot8Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID8))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID8))
                        {
                            playerIDsFromMod.Remove(multiplayerID8);
                            this.Monitor.Log($"Removing: {multiplayerID8}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit8 = true;
                        slot8Used = false;
                    }
                }
                if (slot9Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID9))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID9))
                        {
                            playerIDsFromMod.Remove(multiplayerID9);
                            this.Monitor.Log($"Removing: {multiplayerID9}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit9 = true;
                        slot9Used = false;
                    }
                }
                if (slot10Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID10))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID10))
                        {
                            playerIDsFromMod.Remove(multiplayerID10);
                            this.Monitor.Log($"Removing: {multiplayerID10}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit10 = true;
                        slot10Used = false;
                    }
                }
                if (slot11Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID11))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID11))
                        {
                            playerIDsFromMod.Remove(multiplayerID11);
                            this.Monitor.Log($"Removing: {multiplayerID11}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit11 = true;
                        slot11Used = false;
                    }
                }
                if (slot12Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID12))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID12))
                        {
                            playerIDsFromMod.Remove(multiplayerID12);
                            this.Monitor.Log($"Removing: {multiplayerID12}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit12 = true;
                        slot12Used = false;
                    }
                }
                if (slot13Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID13))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID13))
                        {
                            playerIDsFromMod.Remove(multiplayerID13);
                            this.Monitor.Log($"Removing: {multiplayerID13}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit13 = true;
                        slot13Used = false;
                    }
                }
                if (slot14Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID14))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID14))
                        {
                            playerIDsFromMod.Remove(multiplayerID14);
                            this.Monitor.Log($"Removing: {multiplayerID14}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit14 = true;
                        slot14Used = false;
                    }
                }
                if (slot15Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID15))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID15))
                        {
                            playerIDsFromMod.Remove(multiplayerID15);
                            this.Monitor.Log($"Removing: {multiplayerID15}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit15 = true;
                        slot15Used = false;
                    }
                }
                if (slot16Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID16))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID16))
                        {
                            playerIDsFromMod.Remove(multiplayerID16);
                            this.Monitor.Log($"Removing: {multiplayerID16}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit16 = true;
                        slot16Used = false;
                    }
                }
                if (slot17Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID17))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID17))
                        {
                            playerIDsFromMod.Remove(multiplayerID17);
                            this.Monitor.Log($"Removing: {multiplayerID17}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit17 = true;
                        slot17Used = false;
                    }
                }
                if (slot18Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID18))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID18))
                        {
                            playerIDsFromMod.Remove(multiplayerID18);
                            this.Monitor.Log($"Removing: {multiplayerID18}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit18 = true;
                        slot18Used = false;
                    }
                }
                if (slot19Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID19))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID19))
                        {
                            playerIDsFromMod.Remove(multiplayerID19);
                            this.Monitor.Log($"Removing: {multiplayerID19}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit19 = true;
                        slot19Used = false;
                    }
                }
                if (slot20Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID20))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID20))
                        {
                            playerIDsFromMod.Remove(multiplayerID20);
                            this.Monitor.Log($"Removing: {multiplayerID20}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit20 = true;
                        slot20Used = false;
                    }
                }
                if (slot21Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID21))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID21))
                        {
                            playerIDsFromMod.Remove(multiplayerID21);
                            this.Monitor.Log($"Removing: {multiplayerID21}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit21 = true;
                        slot21Used = false;
                    }
                }
                if (slot22Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID22))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID22))
                        {
                            playerIDsFromMod.Remove(multiplayerID22);
                            this.Monitor.Log($"Removing: {multiplayerID22}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit22 = true;
                        slot22Used = false;
                    }
                }
                if (slot23Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID23))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID23))
                        {
                            playerIDsFromMod.Remove(multiplayerID23);
                            this.Monitor.Log($"Removing: {multiplayerID23}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit23 = true;
                        slot23Used = false;
                    }
                }
                if (slot24Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID24))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID24))
                            this.Monitor.Log($"Removing: {multiplayerID24}");
                        {
                            playerIDsFromMod.Remove(multiplayerID24);
                        }
                        connectedFarmersCount -= 1;
                        didQuit24 = true;
                        slot24Used = false;
                    }
                }
                if (slot25Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID25))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID25))
                        {
                            playerIDsFromMod.Remove(multiplayerID25);
                            this.Monitor.Log($"Removing: {multiplayerID25}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit25 = true;
                        slot25Used = false;
                    }
                }
                if (slot26Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID26))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID26))
                        {
                            playerIDsFromMod.Remove(multiplayerID26);
                            this.Monitor.Log($"Removing: {multiplayerID26}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit26 = true;
                        slot26Used = false;
                    }
                }
                if (slot27Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID27))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID27))
                        {
                            playerIDsFromMod.Remove(multiplayerID27);
                            this.Monitor.Log($"Removing: {multiplayerID27}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit27 = true;
                        slot27Used = false;
                    }
                }
                if (slot28Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID28))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID28))
                        {
                            playerIDsFromMod.Remove(multiplayerID28);
                            this.Monitor.Log($"Removing: {multiplayerID28}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit28 = true;
                        slot28Used = false;
                    }
                }
                if (slot29Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID29))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID29))
                        {
                            playerIDsFromMod.Remove(multiplayerID29);
                            this.Monitor.Log($"Removing: {multiplayerID29}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit29 = true;
                        slot29Used = false;
                    }
                }
                if (slot30Used == true)
                {
                    if (!playerIDs.Contains(multiplayerID30))
                    {
                        if (playerIDsFromMod.Contains(multiplayerID30))
                        {
                            playerIDsFromMod.Remove(multiplayerID30);
                            this.Monitor.Log($"Removing: {multiplayerID30}");
                        }
                        connectedFarmersCount -= 1;
                        didQuit30 = true;
                        slot30Used = false;
                    }
                }


            }



            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //if a slot is used countdown until check if they sent their ID through mod, if not kick
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (slot1CountDown == true)
            {

                oneSecondTicks1 += 1;
                if (oneSecondTicks1 >= kickTimer)
                {
                    if (didQuit1 == true)
                    {
                        oneSecondTicks1 = 0;
                        slot1CountDown = false;
                        didQuit1 = false;  
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID1))
                    {

                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID1, new OutgoingMessage((byte)19, multiplayerID1, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID1);
                        //kick Galaxy ID
                        //galaxy1CountDown = true;
                        /////////////////
                       
                        WhatToSayWhenKickAfter();

                        connectedFarmersCount -= 1;
                        slot1Used = false;
                    }
                    oneSecondTicks1 = 0;
                    slot1CountDown = false;
                }
            }

            if (slot2CountDown == true)
            {
                oneSecondTicks2 += 1;
                if (oneSecondTicks2 >= kickTimer)
                {
                    if (didQuit2 == true)
                    {
                        oneSecondTicks2 = 0;
                        slot2CountDown = false;
                        didQuit2 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID2))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID2, new OutgoingMessage((byte)19, multiplayerID2, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID2);

                        //kick Galaxy ID
                        //galaxy2CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();

                        connectedFarmersCount -= 1;
                        slot2Used = false;
                    }
                    oneSecondTicks2 = 0;
                    slot2CountDown = false;
                }
            }

            if (slot3CountDown == true)
            {
                oneSecondTicks3 += 1;
                if (oneSecondTicks3 >= kickTimer)
                {
                    if (didQuit3 == true)
                    {
                        oneSecondTicks3 = 0;
                        slot3CountDown = false;
                        didQuit3 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID3))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID3, new OutgoingMessage((byte)19, multiplayerID3, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID3);

                        //kick Galaxy ID
                        //galaxy3CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();

                        connectedFarmersCount -= 1;
                        slot3Used = false;
                    }
                    oneSecondTicks3 = 0;
                    slot3CountDown = false;
                }
            }

            if (slot4CountDown == true)
            {

                oneSecondTicks4 += 1;
                if (oneSecondTicks4 >= kickTimer)
                {
                    if (didQuit4 == true)
                    {
                        oneSecondTicks4 = 0;
                        slot4CountDown = false;
                        didQuit4 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID4))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID4, new OutgoingMessage((byte)19, multiplayerID4, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID4);

                        //kick Galaxy ID
                        //galaxy4CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot4Used = false;
                    }
                    oneSecondTicks4 = 0;
                    slot4CountDown = false;
                }
            }

            if (slot5CountDown == true)
            {
                oneSecondTicks5 += 1;
                if (oneSecondTicks5 >= kickTimer)
                {
                    if (didQuit5 == true)
                    {
                        oneSecondTicks5 = 0;
                        slot5CountDown = false;
                        didQuit5 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID5))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID5, new OutgoingMessage((byte)19, multiplayerID5, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID5);

                        //kick Galaxy ID
                        //galaxy5CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot5Used = false;
                    }
                    oneSecondTicks5 = 0;
                    slot5CountDown = false;
                }
            }

            if (slot6CountDown == true)
            {
                oneSecondTicks6 += 1;
                if (oneSecondTicks6 >= kickTimer)
                {

                    if (didQuit6 == true)
                    {
                        oneSecondTicks6 = 0;
                        slot6CountDown = false;
                        didQuit6 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID6))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID6, new OutgoingMessage((byte)19, multiplayerID6, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID6);

                        //kick Galaxy ID
                        //galaxy6CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot6Used = false;
                    }
                    oneSecondTicks6 = 0;
                    slot6CountDown = false;
                }
            }
            if (slot7CountDown == true)
            {
                oneSecondTicks7 += 1;
                if (oneSecondTicks7 >= kickTimer)
                {
                    if (didQuit7 == true)
                    {
                        oneSecondTicks7 = 0;
                        slot7CountDown = false;
                        didQuit7 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID7))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID7, new OutgoingMessage((byte)19, multiplayerID7, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID7);

                        //kick Galaxy ID
                        //galaxy7CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot7Used = false;
                    }
                    oneSecondTicks7 = 0;
                    slot7CountDown = false;
                }
            }
            if (slot8CountDown == true)
            {
                oneSecondTicks8 += 1;
                if (oneSecondTicks8 >= kickTimer)
                {
                    if (didQuit8 == true)
                    {
                        oneSecondTicks8 = 0;
                        slot8CountDown = false;
                        didQuit8 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID8))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID8, new OutgoingMessage((byte)19, multiplayerID8, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID8);

                        //kick Galaxy ID
                        //galaxy8CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot8Used = false;
                    }
                    oneSecondTicks8 = 0;
                    slot8CountDown = false;
                }
            }
            if (slot9CountDown == true)
            {
                oneSecondTicks9 += 1;
                if (oneSecondTicks9 >= kickTimer)
                {
                    if (didQuit9 == true)
                    {
                        oneSecondTicks9 = 0;
                        slot9CountDown = false;
                        didQuit9 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID9))
                    {
                        WhatColorToSayWhenKickBefore();
                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID9, new OutgoingMessage((byte)19, multiplayerID9, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID9);

                        //kick Galaxy ID
                        //galaxy9CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot9Used = false;
                    }
                    oneSecondTicks9 = 0;
                    slot9CountDown = false;
                }
            }
            if (slot10CountDown == true)
            {
                oneSecondTicks10 += 1;
                if (oneSecondTicks10 >= kickTimer)
                {
                    if (didQuit10 == true)
                    {
                        oneSecondTicks10 = 0;
                        slot10CountDown = false;
                        didQuit10 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID10))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID10, new OutgoingMessage((byte)19, multiplayerID10, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID10);

                        //kick Galaxy ID
                        //galaxy10CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot10Used = false;
                    }
                    oneSecondTicks10 = 0;
                    slot10CountDown = false;
                }
            }
            if (slot11CountDown == true)
            {

                oneSecondTicks11 += 1;
                if (oneSecondTicks11 >= kickTimer)
                {
                    if (didQuit11 == true)
                    {
                        oneSecondTicks11 = 0;
                        slot11CountDown = false;
                        didQuit11 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID11))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID11, new OutgoingMessage((byte)19, multiplayerID11, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID11);

                        //kick Galaxy ID
                        //galaxy11CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();

                        connectedFarmersCount -= 1;
                        slot11Used = false;
                    }
                    oneSecondTicks11 = 0;
                    slot11CountDown = false;
                }
            }

            if (slot12CountDown == true)
            {
                oneSecondTicks12 += 1;
                if (oneSecondTicks12 >= kickTimer)
                {
                    if (didQuit12 == true)
                    {
                        oneSecondTicks12 = 0;
                        slot12CountDown = false;
                        didQuit12 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID12))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID12, new OutgoingMessage((byte)19, multiplayerID12, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID12);

                        //kick Galaxy ID
                        //galaxy12CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot12Used = false;
                    }
                    oneSecondTicks12 = 0;
                    slot12CountDown = false;
                }
            }

            if (slot13CountDown == true)
            {
                oneSecondTicks13 += 1;
                if (oneSecondTicks13 >= kickTimer)
                {
                    if (didQuit13 == true)
                    {
                        oneSecondTicks13 = 0;
                        slot13CountDown = false;
                        didQuit13 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID13))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID13, new OutgoingMessage((byte)19, multiplayerID13, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID13);

                        //kick Galaxy ID
                        //galaxy13CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot13Used = false;
                    }
                    oneSecondTicks13 = 0;
                    slot13CountDown = false;
                }
            }

            if (slot14CountDown == true)
            {

                oneSecondTicks14 += 1;
                if (oneSecondTicks14 >= kickTimer)
                {
                    if (didQuit14 == true)
                    {
                        oneSecondTicks14 = 0;
                        slot14CountDown = false;
                        didQuit14 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID14))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID14, new OutgoingMessage((byte)19, multiplayerID14, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID14);

                        //kick Galaxy ID
                        //galaxy14CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot14Used = false;
                    }
                    oneSecondTicks14 = 0;
                    slot14CountDown = false;
                }
            }

            if (slot15CountDown == true)
            {
                oneSecondTicks5 += 1;
                if (oneSecondTicks15 >= kickTimer)
                {
                    if (didQuit15 == true)
                    {
                        oneSecondTicks15 = 0;
                        slot15CountDown = false;
                        didQuit15 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID15))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID15, new OutgoingMessage((byte)19, multiplayerID15, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID15);

                        //kick Galaxy ID
                        //galaxy15CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot15Used = false;
                    }
                    oneSecondTicks15 = 0;
                    slot15CountDown = false;
                }
            }

            if (slot16CountDown == true)
            {
                oneSecondTicks16 += 1;
                if (oneSecondTicks16 >= kickTimer)
                {
                    if (didQuit16 == true)
                    {
                        oneSecondTicks16 = 0;
                        slot16CountDown = false;
                        didQuit16 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID16))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID16, new OutgoingMessage((byte)19, multiplayerID16, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID16);

                        //kick Galaxy ID
                        //galaxy16CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot16Used = false;
                    }
                    oneSecondTicks16 = 0;
                    slot16CountDown = false;
                }
            }
            if (slot17CountDown == true)
            {
                oneSecondTicks17 += 1;
                if (oneSecondTicks17 >= kickTimer)
                {
                    if (didQuit17 == true)
                    {
                        oneSecondTicks17 = 0;
                        slot17CountDown = false;
                        didQuit17 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID17))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID17, new OutgoingMessage((byte)19, multiplayerID17, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID17);

                        //kick Galaxy ID
                        //galaxy17CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot17Used = false;
                    }
                    oneSecondTicks17 = 0;
                    slot17CountDown = false;
                }
            }
            if (slot18CountDown == true)
            {
                oneSecondTicks18 += 1;
                if (oneSecondTicks18 >= kickTimer)
                {
                    if (didQuit18 == true)
                    {
                        oneSecondTicks18 = 0;
                        slot18CountDown = false;
                        didQuit18 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID18))
                    {
                        WhatColorToSayWhenKickBefore();
                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID18, new OutgoingMessage((byte)19, multiplayerID18, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID18);

                        //kick Galaxy ID
                        //galaxy18CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot18Used = false;
                    }
                    oneSecondTicks18 = 0;
                    slot18CountDown = false;
                }
            }
            if (slot19CountDown == true)
            {
                oneSecondTicks19 += 1;
                if (oneSecondTicks19 >= kickTimer)
                {
                    if (didQuit19 == true)
                    {
                        oneSecondTicks19 = 0;
                        slot19CountDown = false;
                        didQuit19 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID19))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID19, new OutgoingMessage((byte)19, multiplayerID19, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID19);

                        //kick Galaxy ID
                        //galaxy19CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot19Used = false;
                    }
                    oneSecondTicks19 = 0;
                    slot19CountDown = false;
                }
            }
            if (slot20CountDown == true)
            {
                oneSecondTicks20 += 1;
                if (oneSecondTicks20 >= kickTimer)
                {
                    if (didQuit20 == true)
                    {
                        oneSecondTicks20 = 0;
                        slot20CountDown = false;
                        didQuit20 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID20))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID20, new OutgoingMessage((byte)19, multiplayerID20, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID20);

                        //kick Galaxy ID
                        //galaxy20CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot20Used = false;
                    }
                    oneSecondTicks20 = 0;
                    slot20CountDown = false;
                }
            }
            if (slot21CountDown == true)
            {

                oneSecondTicks21 += 1;
                if (oneSecondTicks21 >= kickTimer)
                {
                    if (didQuit21 == true)
                    {
                        oneSecondTicks21 = 0;
                        slot21CountDown = false;
                        didQuit21 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID21))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID21, new OutgoingMessage((byte)19, multiplayerID21, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID21);

                        //kick Galaxy ID
                        //galaxy21CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();

                        connectedFarmersCount -= 1;
                        slot21Used = false;
                    }
                    oneSecondTicks21 = 0;
                    slot21CountDown = false;
                }
            }

            if (slot22CountDown == true)
            {
                oneSecondTicks22 += 1;
                if (oneSecondTicks22 >= kickTimer)
                {
                    if (didQuit22 == true)
                    {
                        oneSecondTicks22 = 0;
                        slot22CountDown = false;
                        didQuit22 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID22))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID22, new OutgoingMessage((byte)19, multiplayerID22, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID22);

                        //kick Galaxy ID
                        //galaxy22CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot22Used = false;
                    }
                    oneSecondTicks22 = 0;
                    slot22CountDown = false;
                }
            }

            if (slot23CountDown == true)
            {
                oneSecondTicks23 += 1;
                if (oneSecondTicks23 >= kickTimer)
                {
                    if (didQuit23 == true)
                    {
                        oneSecondTicks23 = 0;
                        slot23CountDown = false;
                        didQuit23 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID23))
                    {
                        WhatColorToSayWhenKickBefore();
                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID23, new OutgoingMessage((byte)19, multiplayerID23, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID23);

                        //kick Galaxy ID
                        //galaxy23CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot23Used = false;
                    }
                    oneSecondTicks23 = 0;
                    slot23CountDown = false;
                }
            }

            if (slot24CountDown == true)
            {

                oneSecondTicks24 += 1;
                if (oneSecondTicks24 >= kickTimer)
                {
                    if (didQuit24 == true)
                    {
                        oneSecondTicks24 = 0;
                        slot24CountDown = false;
                        didQuit24 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID24))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID24, new OutgoingMessage((byte)19, multiplayerID24, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID24);

                        //kick Galaxy ID
                        //galaxy24CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot24Used = false;
                    }
                    oneSecondTicks24 = 0;
                    slot24CountDown = false;
                }
            }

            if (slot25CountDown == true)
            {
                oneSecondTicks25 += 1;
                if (oneSecondTicks25 >= kickTimer)
                {
                    if (didQuit25 == true)
                    {
                        oneSecondTicks25 = 0;
                        slot25CountDown = false;
                        didQuit25 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID25))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID25, new OutgoingMessage((byte)19, multiplayerID25, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID25);

                        //kick Galaxy ID
                        //galaxy25CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot25Used = false;
                    }
                    oneSecondTicks25 = 0;
                    slot25CountDown = false;
                }
            }

            if (slot26CountDown == true)
            {
                oneSecondTicks26 += 1;
                if (oneSecondTicks26 >= kickTimer)
                {
                    if (didQuit26 == true)
                    {
                        oneSecondTicks26 = 0;
                        slot26CountDown = false;
                        didQuit26 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID26))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID26, new OutgoingMessage((byte)19, multiplayerID26, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID26);

                        //kick Galaxy ID
                        //galaxy26CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot26Used = false;
                    }
                    oneSecondTicks26 = 0;
                    slot26CountDown = false;
                }
            }
            if (slot27CountDown == true)
            {
                oneSecondTicks27 += 1;
                if (oneSecondTicks27 >= kickTimer)
                {
                    if (didQuit27 == true)
                    {
                        oneSecondTicks27 = 0;
                        slot27CountDown = false;
                        didQuit27 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID27))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID27, new OutgoingMessage((byte)19, multiplayerID27, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID27);

                        //kick Galaxy ID
                        //galaxy27CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot27Used = false;
                    }
                    oneSecondTicks27 = 0;
                    slot27CountDown = false;
                }
            }
            if (slot28CountDown == true)
            {
                oneSecondTicks28 += 1;
                if (oneSecondTicks28 >= kickTimer)
                {
                    if (didQuit28 == true)
                    {
                        oneSecondTicks28 = 0;
                        //slot28CountDown = false;
                        didQuit28 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID28))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID28, new OutgoingMessage((byte)19, multiplayerID28, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID28);

                        //kick Galaxy ID
                        //galaxy28CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot28Used = false;
                    }
                    oneSecondTicks28 = 0;
                    slot28CountDown = false;
                }
            }
            if (slot29CountDown == true)
            {
                oneSecondTicks29 += 1;
                if (oneSecondTicks29 >= kickTimer)
                {
                    if (didQuit29 == true)
                    {
                        oneSecondTicks29 = 0;
                        slot29CountDown = false;
                        didQuit29 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID29))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID29, new OutgoingMessage((byte)19, multiplayerID29, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID29);

                        //kick Galaxy ID
                        //galaxy29CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot29Used = false;
                    }
                    oneSecondTicks29 = 0;
                    slot29CountDown = false;
                }
            }
            if (slot30CountDown == true)
            {
                oneSecondTicks30 += 1;
                if (oneSecondTicks30 >= kickTimer)
                {
                    if (didQuit30 == true)
                    {
                        oneSecondTicks30 = 0;
                        slot30CountDown = false;
                        didQuit30 = false;
                    }
                    else if (!playerIDsFromMod.Contains(multiplayerID30))
                    {
                        WhatColorToSayWhenKickBefore();

                        Game1.chatBox.activate();
                        Game1.chatBox.setText("You are being kicked by AntiCheat");
                        Game1.chatBox.chatBox.RecieveCommandInput('\r');
                        Game1.server.sendMessage(multiplayerID30, new OutgoingMessage((byte)19, multiplayerID30, new object[0]));
                        Game1.server.playerDisconnected(multiplayerID30);

                        //kick Galaxy ID
                        //galaxy30CountDown = true;
                        /////////////////

                        WhatToSayWhenKickAfter();
                        connectedFarmersCount -= 1;
                        slot30Used = false;
                    }
                    oneSecondTicks30 = 0;
                    slot30CountDown = false;
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //countdown from kick until full galaxyID kick
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            /*if (galaxy1CountDown == true)
            {
                galaxySecondTicks1 += 1;
                if (galaxySecondTicks1 >= 10)
                {
                    if (galaxyID1 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID1);
                            this.Monitor.Log("Kicking " + galaxyID1.ToString());
                        }
                    }
                    galaxy1CountDown = false;
                    galaxySecondTicks1 = 0;
                }
            }

            if (galaxy2CountDown == true)
            {
                galaxySecondTicks2 += 1;
                if (galaxySecondTicks2 >= 10)
                {
                    if (galaxyID2 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID2);
                            this.Monitor.Log("Kicking " + galaxyID2.ToString());
                        }
                    }
                    galaxy2CountDown = false;
                    galaxySecondTicks2 = 0;
                }
            }

            if (galaxy3CountDown == true)
            {
                galaxySecondTicks3 += 1;
                if (galaxySecondTicks3 >= 10)
                {
                    if (galaxyID3 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID3);
                            this.Monitor.Log("Kicking " + galaxyID3.ToString());
                        }
                    }
                    galaxy3CountDown = false;
                    galaxySecondTicks3 = 0;
                }
            }
            if (galaxy4CountDown == true)
            {
                galaxySecondTicks4 += 1;
                if (galaxySecondTicks4 >= 10)
                {
                    if (galaxyID4 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID4);
                            this.Monitor.Log("Kicking " + galaxyID4.ToString());
                        }
                    }
                    galaxy4CountDown = false;
                    galaxySecondTicks4 = 0;
                }
            }
            if (galaxy5CountDown == true)
            {
                galaxySecondTicks5 += 1;
                if (galaxySecondTicks5 >= 10)
                {
                    if (galaxyID5 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID5);
                            this.Monitor.Log("Kicking " + galaxyID5.ToString());
                        }
                    }
                    galaxy5CountDown = false;
                    galaxySecondTicks5 = 0;
                }
            }

            if (galaxy6CountDown == true)
            {
                galaxySecondTicks6 += 1;
                if (galaxySecondTicks6 >= 10)
                {
                    if (galaxyID6 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID6);
                            this.Monitor.Log("Kicking " + galaxyID6.ToString());
                        }
                    }
                    galaxy6CountDown = false;
                    galaxySecondTicks6 = 0;
                }
            }
            if (galaxy7CountDown == true)
            {
                galaxySecondTicks7 += 1;
                if (galaxySecondTicks7 >= 10)
                {
                    if (galaxyID7 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID7);
                            this.Monitor.Log("Kicking " + galaxyID7.ToString());
                        }
                    }
                    galaxy7CountDown = false;
                    galaxySecondTicks7 = 0;
                }
            }
            if (galaxy8CountDown == true)
            {
                galaxySecondTicks8 += 1;
                if (galaxySecondTicks8 >= 10)
                {
                    if (galaxyID8 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID8);
                            this.Monitor.Log("Kicking " + galaxyID8.ToString());
                        }
                    }
                    galaxy8CountDown = false;
                    galaxySecondTicks8 = 0;
                }
            }
            if (galaxy9CountDown == true)
            {
                galaxySecondTicks9 += 1;
                if (galaxySecondTicks9 >= 10)
                {
                    if (galaxyID9 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID9);
                            this.Monitor.Log("Kicking " + galaxyID9.ToString());
                        }
                    }
                    galaxy9CountDown = false;
                    galaxySecondTicks9 = 0;
                }
            }
            if (galaxy10CountDown == true)
            {
                galaxySecondTicks10 += 1;
                if (galaxySecondTicks10 >= 10)
                {
                    if (galaxyID10 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID10);
                            this.Monitor.Log("Kicking " + galaxyID10.ToString());
                        }
                    }
                    galaxy10CountDown = false;
                    galaxySecondTicks10 = 0;
                }
            }

            if (galaxy11CountDown == true)
            {
                galaxySecondTicks11 += 1;
                if (galaxySecondTicks11 >= 10)
                {
                    if (galaxyID11 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID11);
                            this.Monitor.Log("Kicking " + galaxyID11.ToString());
                        }
                    }
                    galaxy11CountDown = false;
                    galaxySecondTicks11 = 0;
                }
            }
            if (galaxy12CountDown == true)
            {
                galaxySecondTicks12 += 1;
                if (galaxySecondTicks12 >= 10)
                {
                    if (galaxyID12 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID12);
                            this.Monitor.Log("Kicking " + galaxyID12.ToString());
                        }
                    }
                    galaxy12CountDown = false;
                    galaxySecondTicks12 = 0;
                }
            }

            if (galaxy13CountDown == true)
            {
                galaxySecondTicks13 += 1;
                if (galaxySecondTicks13 >= 10)
                {
                    if (galaxyID13 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID13);
                            this.Monitor.Log("Kicking " + galaxyID13.ToString());
                        }
                    }
                    galaxy13CountDown = false;
                    galaxySecondTicks13 = 0;
                }
            }
            if (galaxy14CountDown == true)
            {
                galaxySecondTicks14 += 1;
                if (galaxySecondTicks14 >= 10)
                {
                    if (galaxyID14 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID14);
                            this.Monitor.Log("Kicking " + galaxyID14.ToString());
                        }
                    }
                    galaxy14CountDown = false;
                    galaxySecondTicks14 = 0;
                }
            }
            if (galaxy15CountDown == true)
            {
                galaxySecondTicks15 += 1;
                if (galaxySecondTicks15 >= 10)
                {
                    if (galaxyID15 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID15);
                            this.Monitor.Log("Kicking " + galaxyID15.ToString());
                        }
                    }
                    galaxy15CountDown = false;
                    galaxySecondTicks15 = 0;
                }
            }
            if (galaxy16CountDown == true)
            {
                galaxySecondTicks16 += 1;
                if (galaxySecondTicks16 >= 10)
                {
                    if (galaxyID16 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID16);
                            this.Monitor.Log("Kicking " + galaxyID16.ToString());
                        }
                    }
                    galaxy16CountDown = false;
                    galaxySecondTicks16 = 0;
                }
            }
            if (galaxy17CountDown == true)
            {
                galaxySecondTicks17 += 1;
                if (galaxySecondTicks17 >= 10)
                {
                    if (galaxyID17 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID17);
                            this.Monitor.Log("Kicking " + galaxyID17.ToString());
                        }
                    }
                    galaxy17CountDown = false;
                    galaxySecondTicks17 = 0;
                }
            }
            if (galaxy18CountDown == true)
            {
                galaxySecondTicks18 += 1;
                if (galaxySecondTicks18 >= 10)
                {
                    if (galaxyID18 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID18);
                            this.Monitor.Log("Kicking " + galaxyID18.ToString());
                        }
                    }
                    galaxy18CountDown = false;
                    galaxySecondTicks18 = 0;
                }
            }
            if (galaxy19CountDown == true)
            {
                galaxySecondTicks19 += 1;
                if (galaxySecondTicks19 >= 10)
                {
                    if (galaxyID19 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID19);
                            this.Monitor.Log("Kicking " + galaxyID19.ToString());
                        }
                    }
                    galaxy19CountDown = false;
                    galaxySecondTicks19 = 0;
                }
            }
            if (galaxy20CountDown == true)
            {
                galaxySecondTicks20 += 1;
                if (galaxySecondTicks20 >= 10)
                {
                    if (galaxyID20 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID20);
                            this.Monitor.Log("Kicking " + galaxyID20.ToString());
                        }
                    }
                    galaxy20CountDown = false;
                    galaxySecondTicks20 = 0;
                }
            }
            if (galaxy21CountDown == true)
            {
                galaxySecondTicks21 += 1;
                if (galaxySecondTicks21 >= 10)
                {
                    if (galaxyID21 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID21);
                            this.Monitor.Log("Kicking " + galaxyID21.ToString());
                        }
                    }
                    galaxy21CountDown = false;
                    galaxySecondTicks21 = 0;
                }
            }
            if (galaxy22CountDown == true)
            {
                galaxySecondTicks22 += 1;
                if (galaxySecondTicks22 >= 10)
                {
                    if (galaxyID22 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID22);
                            this.Monitor.Log("Kicking " + galaxyID22.ToString());
                        }
                    }
                    galaxy22CountDown = false;
                    galaxySecondTicks22 = 0;
                }
            }
            if (galaxy23CountDown == true)
            {
                galaxySecondTicks23 += 1;
                if (galaxySecondTicks23 >= 10)
                {
                    if (galaxyID23 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID23);
                            this.Monitor.Log("Kicking " + galaxyID23.ToString());
                        }
                    }
                    galaxy23CountDown = false;
                    galaxySecondTicks23 = 0;
                }
            }
            if (galaxy24CountDown == true)
            {
                galaxySecondTicks24 += 1;
                if (galaxySecondTicks24 >= 10)
                {
                    if (galaxyID24 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID24);
                            this.Monitor.Log("Kicking " + galaxyID24.ToString());
                        }
                    }
                    galaxy24CountDown = false;
                    galaxySecondTicks24 = 0;
                }
            }
            if (galaxy25CountDown == true)
            {
                galaxySecondTicks25 += 1;
                if (galaxySecondTicks25 >= 10)
                {
                    if (galaxyID25 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID25);
                            this.Monitor.Log("Kicking " + galaxyID25.ToString());
                        }
                    }
                    galaxy25CountDown = false;
                    galaxySecondTicks25 = 0;
                }
            }
            if (galaxy26CountDown == true)
            {
                galaxySecondTicks26 += 1;
                if (galaxySecondTicks26 >= 10)
                {
                    if (galaxyID26 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID26);
                            this.Monitor.Log("Kicking " + galaxyID26.ToString());
                        }
                    }
                    galaxy26CountDown = false;
                    galaxySecondTicks26 = 0;
                }
            }
            if (galaxy27CountDown == true)
            {
                galaxySecondTicks27 += 1;
                if (galaxySecondTicks27 >= 10)
                {
                    if (galaxyID27 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID27);
                            this.Monitor.Log("Kicking " + galaxyID27.ToString());
                        }
                    }
                    galaxy27CountDown = false;
                    galaxySecondTicks27 = 0;
                }
            }
            if (galaxy28CountDown == true)
            {
                galaxySecondTicks28 += 1;
                if (galaxySecondTicks28 >= 10)
                {
                    if (galaxyID28 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID28);
                            this.Monitor.Log("Kicking " + galaxyID28.ToString());
                        }
                    }
                    galaxy28CountDown = false;
                    galaxySecondTicks28 = 0;
                }
            }
            if (galaxy29CountDown == true)
            {
                galaxySecondTicks29 += 1;
                if (galaxySecondTicks29 >= 10)
                {
                    if (galaxyID29 != null)
                    {
                        IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                        foreach (Server server in servers)
                        {

                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID29);
                            this.Monitor.Log("Kicking " + galaxyID29.ToString());
                        }
                    }
                    galaxy29CountDown = false;
                    galaxySecondTicks29 = 0;
                }
            }
            if (galaxy30CountDown == true)
            {
                galaxySecondTicks30 += 1;
                if (galaxySecondTicks30 >= 10)
                {
                    IEnumerable<Server> servers = this.Helper.Reflection.GetField<List<Server>>(Game1.server, "servers").GetValue();
                    foreach (Server server in servers)
                    {
                        if (galaxyID30 != null)
                        {
                            GalaxySocket socket = this.Helper.Reflection.GetField<object>(server, "server", required: false)?.GetValue() as GalaxySocket;
                            if (socket == null)
                                continue;
                            this.Helper.Reflection.GetMethod(socket, "close").Invoke(galaxyID30);
                            this.Monitor.Log("Kicking " + galaxyID30.ToString());
                        }
                    }
                    galaxy30CountDown = false;
                    galaxySecondTicks30 = 0;
                }
            }*/

        }
    }
}
