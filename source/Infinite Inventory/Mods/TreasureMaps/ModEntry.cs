/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/unidarkshin/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using System.Timers;
using StardewValley.TerrainFeatures;
using StardewValley.Monsters;
using StardewValley.Locations;

namespace TreasureMaps
{
    /// <summary>The mod entry point.</summary>
    /// 
    public class ModEntry : Mod
    {

        public static Random rnd = new Random(Guid.NewGuid().GetHashCode());
        bool treasure = false;
        GameLocation loc;
        Chest curr;

        String[] clueTypes = { "normal", "hard" };

        string clue;

        Item item;

        Vector2 tile;
        private Timer aTimer = new Timer();
        private Timer aTimer2 = new Timer();
        private Timer aTimer3 = new Timer();

        bool no_mons = false;

        //private MapOverlay m;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += InputEvents_ButtonPressed;
            helper.Events.GameLoop.DayStarted += TimeEvents_AfterDayStarted;
            helper.Events.GameLoop.OneSecondUpdateTicked += GameEvents_OneSecondTick;
            helper.Events.GameLoop.SaveCreating += SaveEvents_BeforeSave;
            helper.Events.Player.Warped += PlayerEvents_Warped;

            helper.ConsoleCommands.Add("tile", "Prints treasure location and tile location.", this.printTile);
            
        }

        private void PlayerEvents_Warped(object sender, WarpedEventArgs e)
        {
            /*if(e.NewLocation is MineShaft && (Game1.player.currentLocation as MineShaft).mineLevel == 10)
            {
                (Game1.player.currentLocation as MineShaft).mineLevel = 219;
            }*/   
            
        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            if (!no_mons)
            {
                rem_mons();
            }
        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {
            
        }

        private void printTile(string arg1, string[] arg2)
        {
            if (tile != null && treasure)
            {
                Monitor.Log($"--------------> {tile.X + ", " + tile.Y}");
                Monitor.Log($"--------------> Player: {Game1.player.getTileLocation()}");
                Monitor.Log($"--------------> Location: {loc.Name}");
            }
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {
            if (treasure)
            {
                treasure = false;
            }

            if (Game1.player.mailReceived.Contains("guildMember") && rnd.NextDouble() < (0.5 + (Game1.player.LuckLevel / 1000)))
            {
                //SetTimer3((int)(rnd.NextDouble() * 1000), 1);
                init_treas_day();
            }

            aTimer2.Enabled = false;

            no_mons = false;

            defaultPL();
        }

        private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && Game1.player.CurrentTool is Hoe && treasure && e.Button.IsUseToolButton() && e.Cursor.GrabTile == tile) // save is loaded
            {
                //this.Monitor.Log($"Treasure Params: {e.Cursor.GrabTile + ", " + tile + ", " + Game1.player.Position}");

                var data = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");

                item = new StardewValley.Object(rand(data.Count - 1, 0), 1);

                if (clue == clueTypes[0])
                {


                    while (item.Category == -4 || item.Category == -9 || item.Category == -7 || item.Category == -12 || item.DisplayName.ToLower().Contains("error"))
                    {
                        item = new StardewValley.Object(rand(data.Count - 1, 0), 1);
                    }
                }
                else
                {   //weap, ring, equipment, tools (HARD)
                    while ((item.Category != -98 && item.Category != -29 && item.Category != -96 && item.Category != -99) || item.DisplayName.ToLower().Contains("error"))
                    {
                        item = new StardewValley.Object(rand(data.Count - 1, 0), 1);
                    }
                }

                
                List<Item> l = new List<Item>();
                l.Add(item);
                curr = new Chest(rand(200, 0), l, tile);
                

                while (!loc.isTileLocationTotallyClearAndPlaceable(tile) || !loc.isTileOnMap(tile))
                {
                    tile = loc.getRandomTile();
                }
                loc.objects.Add(tile, curr);

                HUDMessage message = new HUDMessage($"You find a hidden chest.");

                Game1.addHUDMessage(message);

                message.color = new Color(218, 165, 32);
                message.timeLeft += 2000.0f;
                message.noIcon = true;

                message.update(Game1.currentGameTime);


                treasure = false;

                SetTimer2(5000, 1);

                SetTimer(15000, 1);
            }

        }

        public void init_treas_day()
        {
            
            loc = Game1.locations[rand(Game1.locations.Count - 1, 0)];
            /*while (!Game1.isLocationAccessible(loc.Name))
            {
                loc = Game1.locations[rand(Game1.locations.Count - 1, 0)];
            }*/

            tile = loc.getRandomTile();

            while (!loc.isTileLocationTotallyClearAndPlaceable(tile) || !loc.isTileOnMap(tile))
            {
                tile = loc.getRandomTile();
            }

            //loc.terrainFeatures.Add(tile, new Grass(1,4));
            //HUDMessage message = new HUDMessage($"A secret stash is located in: {loc.Name}\n    -Anonymous");

            HUDMessage message = new HUDMessage($"You find a note lying under your bed.");

            Game1.addHUDMessage(message);

            message.color = new Color(218, 165, 32);
            message.timeLeft += 2500.0f;
            message.noIcon = true;

            message.update(Game1.currentGameTime);

            if(rnd.NextDouble() < (0.1 + (Game1.player.LuckLevel / 100))) 
                clue = clueTypes[1];
            else
                clue = clueTypes[0];

            //Game1.drawDialogueBox("A secret stash is somewhere.");

            treasure = true;

            SetTimer(4000, 2);
        }

        public int rand(int val, int add)
        {
            return (int)(Math.Round((rnd.NextDouble() * val) + add));
        }

        private void SetTimer(int time, int type)
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
            if (type == 1)
                aTimer.Elapsed += OnTimedEvent;
            else if (type == 2)
                aTimer.Elapsed += OnTimedEvent2;

            aTimer.AutoReset = false;
            aTimer.Enabled = true;

            
        }

        private void SetTimer2(int time, int type)
        {
            // Create a timer with a two second interval.
            aTimer2 = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
            if (type == 1)
            {
                aTimer2.Elapsed += OnTimedEvent12;
            }
            else if (type == 2)
            {
                first = true;
                aTimer2.Elapsed += norm;

                HUDMessage message = new HUDMessage($"The ground shakes violently...");

                Game1.addHUDMessage(message);

                message.color = new Color(218, 165, 32);
                message.timeLeft += 1000.0f;
                message.noIcon = true;

                message.update(Game1.currentGameTime);
            }
            else if (type == 3)
            {
                first = true;
                aTimer2.Elapsed += hard;

                HUDMessage message = new HUDMessage($"You feel a strong presence of evil...");

                Game1.addHUDMessage(message);

                message.color = new Color(218, 165, 32);
                message.timeLeft += 2000.0f;
                message.noIcon = true;

                message.update(Game1.currentGameTime);

            }

            if (type == 1)
            aTimer2.AutoReset = false;
            else if (type == 2 || type == 3)
                aTimer2.AutoReset = true;

            aTimer2.Enabled = true;


        }

        private void hard(object sender, ElapsedEventArgs e)
        {
            if (first)
            {
                first = false;

                Vector2 loc2 = Game1.player.currentLocation.getRandomTile();
                while (!(Game1.player.currentLocation.isTileLocationTotallyClearAndPlaceable(loc2)))
                {
                    loc2 = Game1.player.currentLocation.getRandomTile();
                }

                old = Game1.player.currentLocation;
                //int x = (int)loc.X;
                //int y = (int)loc.Y;

                //Monster m = new Monster(monsters[rand.Next(monsters.Length)],loc * (float)Game1.tileSize);
                m = getMonster(rand(8, 0), loc2 * (float)Game1.tileSize);
                m.DamageToFarmer = (int)(m.DamageToFarmer * 5) + (int)(Game1.player.CombatLevel / 2);
                m.Health = (int)(m.Health * 10) + ((Game1.player.CombatLevel / 2) * (m.Health / 5));
                m.focusedOnFarmers = true;
                m.wildernessFarmMonster = true;
                //m.addedSpeed += 3 + (Game1.player.CombatLevel / 4);
                m.Speed += 5 + (Game1.player.CombatLevel / 4);
                m.resilience.Set((int)(m.resilience.Value * 5) + (Game1.player.CombatLevel / 3));
                m.ExperienceGained += (Game1.player.CombatLevel * 5);
                m.Scale *= 3.5f;
                m.willDestroyObjectsUnderfoot = true;


                IList<NPC> characters = Game1.currentLocation.characters;
                characters.Add((NPC)m);

            }

            m.Speed = 5 + (Game1.player.CombatLevel / 4);

            if (m.currentLocation.Name != Game1.currentLocation.Name)
            {
                old.characters.Remove((NPC)m);

                Vector2 loc2 = Game1.player.getTileLocation();
                while (!(Game1.player.currentLocation.isTileLocationTotallyClearAndPlaceable(loc2)) || Game1.player.getTileLocation() == loc2)
                {
                    loc2 += new Vector2(rnd.Next(-4, 4), rnd.Next(-4, 4));
                }

                m.setTileLocation(loc2);
                IList<NPC> characters = Game1.currentLocation.characters;
                characters.Add((NPC)m);

                old = Game1.currentLocation;
            }

            if (rnd.NextDouble() < 0.1)
            {
                m.setInvincibleCountdown(rnd.Next(5, 20));
            }

            if ((Game1.player.health <= 0 || Game1.player.Stamina <= 0))
            {
                if (Game1.player.hasItemInInventoryNamed(item.Name))
                {
                    Game1.player.removeItemFromInventory(item);
                }


                Game1.player.Money = (int)(Game1.player.Money * 0.7);

                aTimer2.Enabled = false;

                if (old.characters.Contains((NPC)m))
                    old.characters.Remove((NPC)m);
                else if (Game1.currentLocation.characters.Contains((NPC)m))
                    Game1.currentLocation.characters.Remove((NPC)m);

                rem_mons();

                no_mons = false;

                defaultPL();

            }
        }

        Monster m;
        GameLocation old;
            
        private void norm(object sender, ElapsedEventArgs e)
        {
            if (first)
            {
                first = false;

                Vector2 loc2 = Game1.player.currentLocation.getRandomTile();
                while (!(Game1.player.currentLocation.isTileLocationTotallyClearAndPlaceable(loc2)))
                {
                    loc2 = Game1.player.currentLocation.getRandomTile();
                }

                old = Game1.player.currentLocation;
                //int x = (int)loc.X;
                //int y = (int)loc.Y;

                //Monster m = new Monster(monsters[rand.Next(monsters.Length)],loc * (float)Game1.tileSize);
                m = getMonster(rand(8, 0), loc2 * (float)Game1.tileSize);
                m.DamageToFarmer = (int)(m.DamageToFarmer * 3) + (int)(Game1.player.CombatLevel / 2);
                m.Health = (int)(m.Health * 3) + ((Game1.player.CombatLevel / 2) * (m.Health / 10));
                m.focusedOnFarmers = true;
                m.wildernessFarmMonster = true;
                //m.addedSpeed += 3 + (Game1.player.CombatLevel / 4);
                m.Speed += 3 + (Game1.player.CombatLevel / 4);
                m.resilience.Set((int)(m.resilience.Value * 2) + (Game1.player.CombatLevel / 3));
                m.ExperienceGained += (Game1.player.CombatLevel * 5);
                m.Scale *= 2.5f;
                m.willDestroyObjectsUnderfoot = true;
                

                IList<NPC> characters = Game1.currentLocation.characters;
                characters.Add((NPC)m);
                
            }

            if(m.currentLocation.Name != Game1.currentLocation.Name)
            {
                old.characters.Remove((NPC)m);

                Vector2 loc2 = Game1.player.getTileLocation();
                while (!(Game1.player.currentLocation.isTileLocationTotallyClearAndPlaceable(loc2)) || Game1.player.getTileLocation() == loc2)
                {
                    loc2 += new Vector2(rnd.Next(-4, 4), rnd.Next(-4, 4));
                }

                m.setTileLocation(loc2);
                IList<NPC> characters = Game1.currentLocation.characters;
                characters.Add((NPC)m);

                old = Game1.currentLocation;
            }

            if (rnd.NextDouble() < 0.1)
            {
                m.setInvincibleCountdown(rnd.Next(5, 20));
            }

            if ((Game1.player.health <= 0 || Game1.player.Stamina <= 0))
            {
                if (Game1.player.hasItemInInventoryNamed(item.Name))
                {
                    Game1.player.removeItemFromInventory(item);
                }


                Game1.player.Money = (int)(Game1.player.Money * 0.7);

                aTimer2.Enabled = false;

                if (old.characters.Contains((NPC)m))
                    old.characters.Remove((NPC)m);
                else if (Game1.currentLocation.characters.Contains((NPC)m))
                    Game1.currentLocation.characters.Remove((NPC)m);

                rem_mons();

                no_mons = false;

                defaultPL();

            }
        }

        private void SetTimer3(int time, int type)
        {
            // Create a timer with a two second interval.
            aTimer3 = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
            if (type == 1)
                aTimer3.Elapsed += startTreasDay;
            

            aTimer3.AutoReset = false;
            aTimer3.Enabled = true;


        }

        bool first = true;

        private void startTreasDay(object sender, ElapsedEventArgs e)
        {
            init_treas_day();
            aTimer3.Enabled = false;
        }

     /*       private void apoc(object sender, ElapsedEventArgs e)
        {
            //rnd = new Random((int)(DateTime.Now.Ticks & 0x0000FFFF));


            if (first)
            {
            
                first = false;

                Game1.globalOutdoorLighting /= 2.0f;
                Game1.outdoorLight = Color.DarkRed * 0.5f;
                Game1.ambientLight = Color.DarkRed * 0.5f;
                Game1.isLightning = true;
                
            }

            
                

                

                if (rnd.NextDouble() < 0.5)
                {
                    //f.faceGeneralDirection(f.currentLocation.getRandomTile());
                    //Game1.playSound("fireball");
                }

                
                if (rnd.NextDouble() < 0.33)
                {
                Game1.player.temporarySpeedBuff = -2.0f;
                }

                if(rnd.NextDouble() < 0.10)
            {
                Game1.player.health -= (int)((rnd.NextDouble()) * 20.0);
                Game1.player.Stamina -= (int)((rnd.NextDouble()) * 35.0);
            }
                
                
            

            if ((Game1.player.health <= 0 || Game1.player.Stamina <= 0))
            {
                if (Game1.player.hasItemInInventoryNamed(item.Name))
                {
                    Game1.player.removeItemFromInventory(item);
                }
                    
                
                Game1.player.money = (int)(Game1.player.money * 0.7);

                aTimer2.Enabled = false;

                defaultPL();

                

                
                
            }




        }
    */
    /*    private void mons(object sender, ElapsedEventArgs e)
        {
            if (first)
            {

                first = false;

                Game1.globalOutdoorLighting /= 2.0f;
                Game1.outdoorLight = Color.DarkRed * 0.5f;
                Game1.ambientLight = Color.DarkRed * 0.5f;
                Game1.isLightning = true;
                             
            }

            if(rnd.NextDouble() <= 0.25)
            {
                Vector2 loc2 = Game1.player.currentLocation.getRandomTile();
                while (!(Game1.player.currentLocation.isTileLocationTotallyClearAndPlaceable(loc2)))
                {
                    loc2 = Game1.player.currentLocation.getRandomTile();
                }
                //int x = (int)loc.X;
                //int y = (int)loc.Y;

                //Monster m = new Monster(monsters[rand.Next(monsters.Length)],loc * (float)Game1.tileSize);
                Monster m = getMonster(rand(7, 0), loc2 * (float)Game1.tileSize);
                m.DamageToFarmer = (int)(m.DamageToFarmer * 3) + (int)(Game1.player.CombatLevel / 2);
                m.Health = (int)(m.Health / 0.5) + ((Game1.player.CombatLevel / 2) * (m.Health / 10));
                m.focusedOnFarmers = true;
                m.wildernessFarmMonster = true;
                m.Speed += rand(3 + Game1.player.CombatLevel, 0);
                m.resilience.Set((int)(m.resilience.Value * 1.5) + (Game1.player.CombatLevel / 3));
                m.ExperienceGained += (Game1.player.CombatLevel / 2);

                IList<NPC> characters = Game1.currentLocation.characters;
                characters.Add((NPC)m);
            }


            if ((Game1.player.health <= 0 || Game1.player.Stamina <= 0))
            {
                if (Game1.player.hasItemInInventoryNamed(item.Name))
                {
                    Game1.player.removeItemFromInventory(item);
                }
               
                Game1.player.money = (int)(Game1.player.money * 0.7);

                aTimer2.Enabled = false;

                defaultPL();





            }
        } */
        
            public void defaultPL()
        {
            Game1.player.health = Game1.player.maxHealth;
            Game1.player.Stamina = Game1.player.MaxStamina;
            Game1.player.temporarySpeedBuff = 0;
        }

        private void OnTimedEvent12(object sender, ElapsedEventArgs e)
        {
            if (clue == clueTypes[0])
            {
                aTimer2.Enabled = false;

                SetTimer2(1000, 2);

                
            }
            if (clue == clueTypes[1])
            {
                aTimer2.Enabled = false;

                SetTimer2(1000, 3);

                
            }
        }

        private void OnTimedEvent2(object sender, ElapsedEventArgs e)
        {
            if(clue == clueTypes[0])
                Game1.drawLetterMessage($"A secret stash is located in: {loc.Name}\n\n    Difficulty: Normal \n\n    -Anonymous");
            else if (clue == clueTypes[1])
                Game1.drawLetterMessage($"A secret stash is located in: {loc.Name}\n\n    Difficulty: Hard \n\n    -Anonymous");

            aTimer.Enabled = false;
            //Monitor.Log("Timer Stopped.");
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            //loc needs to be re initialized?
            loc.updateMap();
           
            if (loc.objects.ContainsKey(tile))// && loc.getObjectAt((int)tile.X, (int)tile.Y) is Chest)
            {
                //Monitor.Log("Obj removed.");
                loc.objects.Remove(tile);
            }
            

            aTimer.Enabled = false;
            
        }

        private Monster getMonster(int x, Vector2 loc)
        {
            Monster m;
            switch (x)
            {
                case 0:
                    m = new Serpent(loc);
                    break;
                case 1:
                    m = new Grub(loc);
                    break;
                case 2:
                    m = new Skeleton(loc);
                    break;
                case 3:
                    m = new LavaCrab(loc);
                    break;
                case 4:
                    m = new Ghost(loc);
                    break;
                case 5:
                    m = new GreenSlime(loc);
                    break;
                case 6:
                    m = new ShadowShaman(loc);
                    break;
                case 7:
                    m = new ShadowBrute(loc);
                    break;
                case 8:
                    m = new Mummy(loc);
                    break;
                default:
                    m = new Monster();
                    break;

                    
            }
            return m;
        }

        public void rem_mons()
        {

            no_mons = true;

            Monitor.Log("Removed Monsters.");
            for (int j = 0; j < Game1.locations.Count; j++)
            {
                //Game1.locations[j].cleanupBeforeSave();

                IList<NPC> characters = Game1.locations[j].characters;
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i] is Monster && (characters[i] as Monster).wildernessFarmMonster)
                    {
                        characters.RemoveAt(i);

                    }
                }
            }
        }




        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>


    }
}