using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.IO;
using StardewValley.Locations;

namespace MultipleBedLocations
{
 
    public class PlayerData
    {
      public string gameLocationName;
      public int xPos;
      public int yPos;
      public int direction;
      public PlayerData()
        {
            gameLocationName = "";
            xPos = 0;
            yPos = 0;
            direction = 0;
        }

      
    }

    public class Class1 :Mod
    {
        public static string playerDataPath;
        public static string fileName = "newBedLocation.txt";
        Dictionary<string, List<Point>> bedLocations;
     public bool hasFarmerLeftPoint;
        public static PlayerData playerData;
        public static bool sleepingWarp;

        public static bool warpOnce;

        public override void Entry(IModHelper helper)
        {
            StardewModdingAPI.Events.GameEvents.UpdateTick += GameEvents_UpdateTick;
            StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            StardewModdingAPI.Events.GraphicsEvents.OnPreRenderHudEvent += warpPlayer;
            StardewModdingAPI.Events.TimeEvents.DayOfMonthChanged += TimeEvents_DayOfMonthChanged;
            warpOnce = false;
            hasFarmerLeftPoint = true;
            playerData = new PlayerData();
            sleepingWarp = false;
            bedLocations = new Dictionary<string, List<Point>>();

            bedLocations.Add("BusStop", new List<Point>()
            {
                //new Point(2,3),
                new Point(10,11),
                new Point(13,15)
                
            }
            );
           
           
        }

        private void TimeEvents_DayOfMonthChanged(object sender, StardewModdingAPI.Events.EventArgsIntChanged e)
        {
            if (Game1.player == null) return;

            if((Game1.player.currentLocation is FarmHouse)){
                File.Delete(Path.Combine(playerDataPath, fileName));
            }

            if (sleepingWarp == true)
            {
                loadConfigForWarp();
                sleepingWarp = false;
            }
        }

        private void warpPlayer(object sender, EventArgs e)
        {
            if (warpOnce == false)
            {
                if (Game1.player.currentLocation.name != playerData.gameLocationName && Game1.player.getTileX() != playerData.xPos && playerData.yPos != Game1.player.getTileY() && playerData.gameLocationName != "")
                {
                    hasFarmerLeftPoint = false;
                    Game1.warpFarmer(playerData.gameLocationName, playerData.xPos, playerData.yPos, playerData.direction);
                    Game1.player.faceDirection(playerData.direction);
                    Game1.player.facingDirection = playerData.direction;
                }
                if (Game1.player.currentLocation.name == playerData.gameLocationName && Game1.player.getTileX() == playerData.xPos && playerData.yPos == Game1.player.getTileY())
                {
                    playerData.gameLocationName = "";
                    playerData.yPos = 0;
                    playerData.xPos = 0;
                    playerData.direction = 0;
                }
                warpOnce = true;
            }
        }

        public static void loadConfigForWarp()
        {
            try
            {
                string[] fileContents = File.ReadAllLines(Path.Combine(playerDataPath, fileName));
                playerData.gameLocationName = fileContents[1];
                playerData.xPos = Convert.ToInt32(fileContents[3]);
                playerData.yPos = Convert.ToInt32(fileContents[5]);
                playerData.direction = Convert.ToInt32(fileContents[7]);
                Game1.warpFarmer(playerData.gameLocationName, playerData.xPos, playerData.yPos, false);
                Game1.player.faceDirection(0);
            }
            catch (Exception err)
            {

            }
            warpOnce = false;
        }


        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            playerDataPath = Path.Combine(Helper.DirectoryPath, "PlayerData", Game1.player.name);

            if (!Directory.Exists(playerDataPath))
            {
                Directory.CreateDirectory(Path.Combine(Helper.DirectoryPath, "PlayerData", Game1.player.name));
            }
            try
            {
                string[] fileContents = File.ReadAllLines(Path.Combine(playerDataPath, fileName));
                playerData.gameLocationName = fileContents[1];
                playerData.xPos = Convert.ToInt32(fileContents[3]);
                playerData.yPos = Convert.ToInt32(fileContents[5]);
                playerData.direction = Convert.ToInt32(fileContents[7]);
                Game1.warpFarmer(playerData.gameLocationName, playerData.xPos, playerData.yPos, false);
                Game1.player.faceDirection(0);
            }
            catch(Exception err)
            {

            }
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.player == null || Game1.player.currentLocation == null ) return;
            if (Game1.eventUp == true) return;
            if (Game1.currentLocation.currentEvent != null) return;

  


            if (bedLocations.ContainsKey(Game1.player.currentLocation.name))
            {
              //  Log.AsyncG(Game1.player.getTileLocationPoint());
                List<Point> pointList;
                bool b = bedLocations.TryGetValue(Game1.player.currentLocation.name, out pointList);
                if (b == false) return; //somehow something must have messed up.
                Point p = Game1.player.getTileLocationPoint();
                if (pointList.Contains(p))
                {
                    if (hasFarmerLeftPoint == true)
                    {
                        hasFarmerLeftPoint = false;
                        question(p);
                    }
                    
                }
                else
                {
                    hasFarmerLeftPoint = true;
                }
            }
        }

        public void question(Point p)
        {
            Log.AsyncC("YAY YOU ARE HERE!");
            Question.createQuestionDialogue("Sleep here?", new Response[3] { new Response("Yes", "Yes"), new Response("No", "No"), new Response("ElseWhere","ElseWhere") }, (a, b) =>
            {
              if (b.ToString() == "Yes")
                {                  
                    return "Yes";
                }
                if (b.ToString() == "No")
                {
                    Game1.player.currentLocation.lastQuestionKey = "";
                    Log.AsyncG(b.ToString());
                    return "No";
                }
                if (b.ToString() == "ElseWhere")
                {

                    return "ElseWhere";
                }
                Log.AsyncG("NULL");
                return "";
            });

        }

        public static void sleepHere(string mapLocationName,Point p)
        {
            Log.AsyncC(Path.Combine(Class1.playerDataPath, Class1.fileName));
            try
            {
                string[] fileContents = new string[8]
                {
                            "Location Name:",
                          mapLocationName,
                            "xPosition:",
                          p.X.ToString(),
                            "yPosition",
                          p.Y.ToString(),
                           "Direction",
                           Game1.player.facingDirection.ToString()
                };
                File.WriteAllLines(Path.Combine(Class1.playerDataPath, Class1.fileName), fileContents);
            }
            catch (Exception e)
            {
                Log.AsyncColour(e, ConsoleColor.DarkCyan);
            }
            Game1.player.currentLocation.lastQuestionKey = "";
            Farmer.doSleepEmote(Game1.player);
            Game1.NewDay(600f);
            Game1.player.mostRecentBed = StardewValley.Utility.PointToVector2(p);
        }

        public static void sleepElseWhere(string mapLocationName, Point p, int facingDirection)
        {
            Game1.warpFarmer(mapLocationName, p.X, p.Y, Game1.player.facingDirection);
            Log.AsyncC(Path.Combine(Class1.playerDataPath, Class1.fileName));
            try
            {
                string[] fileContents = new string[8]
                {
                            "Location Name:",
                          mapLocationName,
                            "xPosition:",
                          p.X.ToString(),
                            "yPosition",
                          p.Y.ToString(),
                           "Direction",
                           facingDirection.ToString()
                };
                File.WriteAllLines(Path.Combine(Class1.playerDataPath, Class1.fileName), fileContents);
            }
            catch (Exception e)
            {
                Log.AsyncColour(e, ConsoleColor.DarkCyan);
            }

            Game1.player.currentLocation.lastQuestionKey = "";
            Farmer.doSleepEmote(Game1.player);
            Game1.NewDay(600f);
            Game1.player.mostRecentBed = StardewValley.Utility.PointToVector2(p);
            //loadConfigForWarp();
            sleepingWarp = true;
        }
    }
}
