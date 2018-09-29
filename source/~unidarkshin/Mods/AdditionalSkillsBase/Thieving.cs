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
using System.Collections;
using System.IO;

namespace AdditionalSkillsBase
{
    public class Thieving
    {
        public Mod instance;
        public static Random rnd;

        public int xp;
        public int level;

        public ModData config;

        double baseChance;
        double mult;

        private Timer aTimer = new Timer();

        public List<int> perks;
        public List<string> perksInfo;

        public Thieving(Mod instance, bool th_c)
        {
            this.instance = instance;
            rnd = new Random();
            perks = new List<int>();
            perksInfo = new List<string>();
            perkStrings();

            readData();

            InputEvents.ButtonPressed += InputEvents_ButtonPressed;
            SaveEvents.BeforeSave += SaveEvents_BeforeSave;

            if (!th_c)
            {
                instance.Helper.ConsoleCommands.Add("th_lev", "Sets your thieving level. Syntax: th_lev <Integer>", this.th_lev);
                instance.Helper.ConsoleCommands.Add("th_print", "Prints Your thieving level.", this.th_print);
                instance.Helper.ConsoleCommands.Add("th_xp", "Sets your thieving xp.", this.th_xp);
                instance.Helper.ConsoleCommands.Add("th_perks", "List the thieving perks gained every 10 levels.", this.th_perks);
            }
        }

        private void th_perks(string arg1, string[] arg2)
        {
            foreach (string s in perksInfo)
            {
                instance.Monitor.Log(s);
            }
            
        }

        public void perkStrings()
        {
            perksInfo.Add("Level 10: 1% chance for random item.");
            perksInfo.Add("Level 20: Debuffs are halved.");
            perksInfo.Add("Level 30: XP Doubled.");
            perksInfo.Add("Level 40: Base pickpocket chance increased to 15% (Default 10%).");
            perksInfo.Add("Level 50: Regenerate health & stamina on successful pickpockets.");
            perksInfo.Add("Level 60: Time slowed movement on failure is halved.");
            perksInfo.Add("Level 70: If you are behind the NPC & they are not moving: increase base by 10%.");
            perksInfo.Add("Level 80: Stolen money is doubled.");
            perksInfo.Add("Level 90: 3% chance for random item.");
            perksInfo.Add("Level 100: Regenerate 50% stamina & health on successful pickpocket.");
        }

        private void th_xp(string arg1, string[] arg2)
        {
            if (int.TryParse(arg2[0], out int l) && l > 0)
            {
                xp = l;

                checkForLevelUp();

                instance.Monitor.Log("Set thieving xp successful.");
            }
            else
            {
                instance.Monitor.Log("Improper syntax.");
            }
        }

        private void th_print(string arg1, string[] arg2)
        {
            instance.Monitor.Log($"Your thieving level is: {level}. You have {xp} / {(level * level * level) + 30} experience.");
        }

        private void th_lev(string arg1, string[] arg2)
        {
            if (int.TryParse(arg2[0], out int l) && l > 0)
            {
                level = l;
                xp = 0;

                instance.Monitor.Log("Set thieving level successful.");

                addPerks();
            }
            else
            {
                instance.Monitor.Log("Improper syntax.");
            }

            
        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            saveData();
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == config.key && Game1.player.addedSpeed >= 0)
            {
                IList<NPC> chars = Game1.player.currentLocation.getCharacters();
                NPC minCh = null;

                float minDist = 100.0f;

                foreach (NPC ch in chars)
                {
                    float dist = Vector2.Distance(Game1.player.getTileLocation(), ch.getTileLocation());

                    if (minCh == null || dist < minDist)
                    {
                        minDist = dist;
                        minCh = ch;
                    }
                }

                if (minCh != null && minDist < 2.0f)
                {
                    attemptPP(minCh);
                }
            }
        }

        public void attemptPP(NPC ch)
        {
            double base2 = 0.0;
            if (!ch.isMoving())
            {
                Vector2 next = ch.getTileLocation();
                int dir = ch.getFacingDirection();

                switch (dir)
                {
                    case 0:
                        next = new Vector2(next.X, next.Y - 1);
                        break;
                    case 1:
                        next = new Vector2(next.X + 1, next.Y);
                        break;
                    case 2:
                        next = new Vector2(next.X, next.Y + 1);
                        break;
                    case 3:
                        next = new Vector2(next.X - 1, next.Y);
                        break;
                    default:
                        break;
                }

                //0 up, 1 right, 2 down, 3 left
                //Game1.chatBox.addMessage($"{Vector2.Distance(Game1.player.getTileLocation(), ch.nextPositionVector2()).ToString()}", Color.Lavender);
                if(Vector2.Distance(Game1.player.getTileLocation(), next) >= 1.8f && perks.Contains(7))
                {
                    base2 = 0.2;
                    //instance.Monitor.Log("------Behind");
                }
                else if(Vector2.Distance(Game1.player.getTileLocation(), next) >= 1.8f && !perks.Contains(7))
                {
                    base2 = 0.1;
                }
                else
                {
                    //instance.Monitor.Log("------NOT Behind");
                }

            }

            if (rnd.NextDouble() < ((baseChance + base2) + (mult * (level / 250.0))))
            {
                if (perks.Contains(10))
                {
                    Game1.player.health = Math.Min(Game1.player.health + (Game1.player.maxHealth / 2), Game1.player.maxHealth);
                    Game1.player.Stamina = Math.Min(Game1.player.Stamina + (Game1.player.MaxStamina / 2), Game1.player.MaxStamina);
                }
                else if (perks.Contains(5))
                {
                    Game1.player.health = Math.Min(Game1.player.health + 6, Game1.player.maxHealth);
                    Game1.player.Stamina = Math.Min(Game1.player.Stamina + 13, Game1.player.MaxStamina);
                }

                

                int m = (int)(rnd.Next(10, 50 + level) * (level / 10.0));

                if (perks.Contains(8))
                    m = m * 2;

                Game1.player.Money += m;

                Game1.chatBox.addMessage($"You stole {m} coins from {ch.displayName}!", Color.DarkTurquoise);

                addXP((int)(rnd.Next(1, 3) * ((level + 1) / 2.0)));

                double ic = 0.01;

                if (perks.Contains(9))
                    ic = 0.03;


                if (perks.Contains(1) && rnd.NextDouble() < ic)
                {
                    var data = Game1.content.Load<Dictionary<int, string>>("Data\\ObjectInformation");

                    StardewValley.Object item = new StardewValley.Object(rnd.Next(0, data.Count - 1), 1);

                    while (item.DisplayName.ToLower().Contains("error"))
                    {
                        item = new StardewValley.Object(rnd.Next(0, data.Count - 1), 1);
                    }

                    Game1.player.addItemToInventory((Item)item);

                    
                    addXP((5 + (level * 5)) + (int)(item.salePrice() * (level / 100.0)));

                    Game1.chatBox.addMessage($"You stole {item.DisplayName} from {ch.displayName}!", Color.DeepPink);
                }
            }
            else
            {
                Game1.chatBox.addMessage($"You were caught pickpocketing {ch.displayName}!", Color.Red);

                if (perks.Contains(2))
                {
                    Game1.player.health -= (int)((5 + rnd.Next(1, 5)) / 2.0);
                    Game1.player.Stamina -= (int)(((10 + rnd.Next(1, 10)) - (int)(Math.Round(level / 12.0))) / 2.0);
                    Game1.player.addedSpeed = (int)(-0.5 * Game1.player.Speed);
                }
                else
                {
                    Game1.player.health -= 5 + rnd.Next(1, 5);
                    Game1.player.Stamina -= ((10 + rnd.Next(1, 10)) - (int)(Math.Round(level / 12.0)));
                    Game1.player.addedSpeed = -1 * Game1.player.Speed;
                }

                SetTimer(Math.Max(1000, 4000 - (level * 20))); //4000

            }
        }

        public void addXP(int x)
        {
            if (perks.Contains(3))
                x = x * 2;

            xp += x;
            Game1.chatBox.addMessage($"You gained {x} thieving experience!", Color.Green);

            checkForLevelUp();
        }

        public void checkForLevelUp()
        {
            if (xp >= ((level * level * level) + 30))
            {
                level++;
                xp = 0;
                Game1.chatBox.addMessage($"You leveled up Thieving!", Color.Violet);

                if (level % 10 == 0)
                    addPerks();
            }

        }

        private void SetTimer(int time)
        {
            // Create a timer with a two second interval.
            if (perks.Contains(6))
            {
                time = (int)(time / 2);
            }

            aTimer = new System.Timers.Timer(time);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += ATimer_Elapsed;

            aTimer.AutoReset = false;
            aTimer.Enabled = true;


        }

        private void ATimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Game1.player.addedSpeed = 0;
            aTimer.Enabled = false;
        }

        public void readData()
        {
            config = instance.Helper.Data.ReadJsonFile<ModData>($"thieving/{Constants.SaveFolderName}.json") ?? new ModData();

            xp = config.xp;
            level = config.level;
            baseChance = config.baseChance;
            mult = config.mult;

            if (!File.Exists($"thieving/{Constants.SaveFolderName}.json"))
                instance.Helper.Data.WriteJsonFile<ModData>($"thieving/{Constants.SaveFolderName}.json", config);

            addPerks();
        }

        public void addPerks()
        {
            for (int i = 1; i < 11; i++)
            {
                if (level >= (i * 10) && !perks.Contains(i))
                    perks.Add(i);
            }

            if (perks.Contains(4))
            {
                baseChance = 0.15;
            }
            else
            {
                baseChance = 0.1;
            }
        }

        public void saveData()
        {
            config.xp = xp;
            config.level = level;

            instance.Helper.Data.WriteJsonFile<ModData>($"thieving/{Constants.SaveFolderName}.json", config);
        }
    }


    public class ModData
    {
        public int xp { get; set; } = 0;
        public int level { get; set; } = 1;
        public SButton key { get; set; } = SButton.P;

        public double baseChance { get; set; } = 0.1;
        public double mult { get; set; } = 1.0;
    }


}
