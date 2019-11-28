using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using StardewValley.Menus;
using System.Linq;

namespace Moongates
{
    public class Mod : StardewModdingAPI.Mod
    {
        internal static Mod instance;

        //master player only handles
        internal static Dictionary<string, MGNPC> Gates = new Dictionary<string, MGNPC>();
        internal static Dictionary<string, GameSpot> Destinations = new Dictionary<string, GameSpot>();

        public List<GameSpot> SpawnPoints;
        internal static string TextureMoongateLunar;
        internal static string TextureMoongateTidal;

        internal static int PlayerCount = 0;

        internal static Random RNG = new Random(Guid.NewGuid().GetHashCode());

        public override void Entry(IModHelper helper)
        {
            instance = this;
            SpawnPoints = new List<GameSpot>();
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.Saving += GameLoop_Saving;

            string filecontents = File.ReadAllText(Helper.DirectoryPath + Path.DirectorySeparatorChar + "spawns.json");
            SpawnPoints = JsonConvert.DeserializeObject<List<GameSpot>>(filecontents);
            TextureMoongateLunar = Helper.Content.GetActualAssetKey("MoongateLunar.png", ContentSource.ModFolder);
            TextureMoongateTidal = Helper.Content.GetActualAssetKey("MoongateTidal.png", ContentSource.ModFolder);

            Helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            Helper.Events.Multiplayer.ModMessageReceived += Multiplayer_ModMessageReceived;
            Helper.Events.Player.Warped += Player_Warped;
        }

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            if(Game1.IsMasterGame) foreach (MGNPC gate in Gates.Values) gate.ResetForMapEntry();
        }

        private void Multiplayer_ModMessageReceived(object sender, ModMessageReceivedEventArgs e)
        {
            if (Game1.IsMasterGame) return;
            if(e.FromModID == Helper.Multiplayer.ModID && e.Type == "destination")
            {
                MessageMoongate msg = e.ReadAs<MessageMoongate>();
                Destinations[msg.gate] = msg.spot;
            }
        }

        private void GameLoop_OneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            //check for new players and broadcast gate positions
            if (Game1.IsMultiplayer && StardewModdingAPI.Context.IsWorldReady) {
                int newPlayerCount = Enumerable.Count(Helper.Multiplayer.GetConnectedPlayers());
                if (newPlayerCount != PlayerCount)
                {
                    if(newPlayerCount > PlayerCount) SendGates();
                    PlayerCount = newPlayerCount;
                }
            }   
        }

        private void SendGates()
        {
            if (Game1.IsMasterGame)
            {
                //send locations to clients
                foreach(var kvp in Destinations)
                {
                    Helper.Multiplayer.SendMessage<MessageMoongate>(new MessageMoongate() { spot = kvp.Value, gate = kvp.Key }, "destination");
                }
            }
        }

        private void GameLoop_Saving(object sender, SavingEventArgs e)
        {
            if (Game1.IsMasterGame)
            {
                foreach (MGNPC gate in Gates.Values)
                {
                    gate.currentLocation.characters.Remove(gate);
                    gate.currentLocation = null;
                }
            }
        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            if (Game1.IsMasterGame)
            {
                Gates["MoongateWax"] = new MGNPC("FarmHouse", "MoongateWax");
                Gates["MoongateFlow"] = new MGNPC("FarmHouse", "MoongateFlow");
                Gates["MoongateWane"] = new MGNPC("FarmHouse", "MoongateWane");
                Gates["MoongateEbb"] = new MGNPC("FarmHouse", "MoongateEbb");

                foreach(var kvp in Gates)
                {
                    var sp = SpawnPoints[RNG.Next(SpawnPoints.Count)];
                    Game1.warpCharacter(kvp.Value, sp.GetGameLocation().Name, sp.GetTileLocation());
                    //Monitor.Log(kvp.Key + " location: " + sp.ToString());
                }

                //pair them

                //lunar gates
                Destinations["MoongateWax"] = new GameSpot(Gates["MoongateWane"].currentLocation, Gates["MoongateWane"].getTileLocationPoint());
                Destinations["MoongateWane"] = new GameSpot(Gates["MoongateWax"].currentLocation, Gates["MoongateWax"].getTileLocationPoint());
                //tidal gates
                Destinations["MoongateEbb"] = new GameSpot(Gates["MoongateFlow"].currentLocation, Gates["MoongateFlow"].getTileLocationPoint());
                Destinations["MoongateFlow"] = new GameSpot(Gates["MoongateEbb"].currentLocation, Gates["MoongateEbb"].getTileLocationPoint());

                SendGates();
            }
        }
    }

    public class MessageMoongate
    {
        public GameSpot spot;
        public string gate;
    }
}