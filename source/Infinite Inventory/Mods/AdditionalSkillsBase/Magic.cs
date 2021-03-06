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
using System.Collections;
using System.IO;

namespace AdditionalSkillsBase
{
    public class Magic
    {
        public Mod instance;
        public static Random rnd;

        public int xp;
        public int level;

        public MagicData config;

        public bool mg_c;
        double baseChance;
        double mult;

        private Timer aTimer = new Timer();

        public List<int> perks;
        public List<string> perksInfo;

        public List<int> skills;
        public List<string> skillsInfo;

        public Magic(Mod instance)
        {
            this.instance = instance;
            rnd = new Random();
            //perky perks
            perks = new List<int>();
            perksInfo = new List<string>();
            perkStrings();
            //skilly skills
            skills = new List<int>();
            skillsInfo = new List<string>();
            skillStrings();

            //Friend
            readData();//Friend
            //Friend
            

            instance.Helper.Events.Input.ButtonPressed += InputEvents_ButtonPressed;
            instance.Helper.Events.GameLoop.SaveCreating += SaveEvents_BeforeSave;
            instance.Helper.Events.GameLoop.OneSecondUpdateTicked += TimeEvents_OneSecondTick;
            instance.Helper.Events.GameLoop.SaveCreated += SaveEvents_AfterSave;
            instance.Helper.Events.Player.Warped += Player_Warped;
            instance.Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;

            if (mg_c)
            {
                instance.Helper.ConsoleCommands.Add("mg_lev", "Sets your magic level. Syntax: th_lev <Integer>", this.mg_lev);
                instance.Helper.ConsoleCommands.Add("mg_setxp", "Sets your magic xp.", this.mg_setxp);

            }
                instance.Helper.ConsoleCommands.Add("mg_print", "Prints Your magic level and xp for next level.", this.mg_print);
                instance.Helper.ConsoleCommands.Add("mg_perks", "List the magic  perks gained every 10 levels.", this.mg_perks);
                instance.Helper.ConsoleCommands.Add("mg_xp", "See your magic xp.", this.mg_xp);
        }



        private void mg_perks(string arg1, string[] arg2)
        {
            foreach (string s in perksInfo)
            {
                instance.Monitor.Log(s);
            }

        }

        public void perkStrings()
        {
            perksInfo.Add("Level 10: Transmute fish into experience. Cost of fish / 10");
            perksInfo.Add("Level 20: Mana cost 10% less");
            perksInfo.Add("Level 30: XP Bonus for day for meditating doubled.");
            perksInfo.Add("Level 40: Base pickpocket chance increased to 15% (Default 10%).");
            perksInfo.Add("Level 50: Regenerate health & stamina on successful pickpockets.");
            perksInfo.Add("Level 60: Time slowed movement on failure is halved.");
            perksInfo.Add("Level 70: If you are behind the NPC & they are not moving: increase base by 10%.");
            perksInfo.Add("Level 80: Stolen money is doubled.");
            perksInfo.Add("Level 90: 3% chance for random item.");
            perksInfo.Add("Level 100: Regenerate 50% stamina & health on successful pickpocket.");
        }
        public void skillStrings()
        {
            skillsInfo.Add("BOOB)");
        }

            private void mg_setxp(string arg1, string[] arg2)
        {
            if (int.TryParse(arg2[0], out int l) && l > 0)
            {
                xp = l;

                checkForLevelUp();

                instance.Monitor.Log("Set magic xp successful.");
            }
            else
            {
                instance.Monitor.Log("Improper syntax.");
            }
        }

        private void mg_xp(string arg1, string[] arg2)
        {
        
                instance.Monitor.Log($"XP = {xp}" );
            
        }

        private void mg_print(string arg1, string[] arg2)
        {
            instance.Monitor.Log($"Your magic level is: {level}. You have {xp} / {(level * level * level) + 30} experience.");
        }

        private void mg_lev(string arg1, string[] arg2)
        {
            if (int.TryParse(arg2[0], out int l) && l > 0)
            {
                level = l;
                xp = 0;

                instance.Monitor.Log("Set magic level successful.");

                addPerks();
            }
            else
            {
                instance.Monitor.Log("Improper syntax.");
            }


        }

        private void GameLoop_DayStarted(object sender, DayStartedEventArgs e)
        {
            hasMed = false;
        }

        public bool hasWarped = false;
        public bool hasMed = false;

        private void Player_Warped(object sender, WarpedEventArgs e)
        {
            hasWarped = true;
        }

        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {
            saveData();
        }

        private void SaveEvents_AfterSave(object sender, EventArgs e)
        {
            
        }

        DateTime oldTime = new DateTime();

        private void InputEvents_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            


            if (e.Button == config.key && Game1.player.addedSpeed == 0 && hasMed == false)
            {
                if (hasWarped)
                {
                    oldTime = DateTime.Now;
                    Game1.player.canMove = true;
                    hasWarped = false;
                }

                if (Game1.player.CanMove && Context.CanPlayerMove && !Game1.player.isInBed)
                {
                    Game1.player.CanMove = false;
                    oldTime = DateTime.Now;
                }
                else if (!Game1.player.CanMove && !Context.CanPlayerMove && !Game1.player.isInBed)
                {
                    Game1.player.CanMove = true;
                    int ms = (int)((DateTime.Now - oldTime).TotalMilliseconds);
                    int xp = (int)((ms / 1000.0) * 1.0);
                    addXP(xp);
                    oldTime = DateTime.Now;
                    hasMed = true;
                    
                }
            }
            
        }

        

        private void TimeEvents_OneSecondTick(object sender, EventArgs e)
        {

        }

      
        public void addXP(int x)
        {
            if (perks.Contains(3))
                x = x * 2;

            xp += x;
            Game1.chatBox.addMessage($"You gained {x} magic experience!", Color.Green);

            checkForLevelUp();
        }

        public void checkForLevelUp()
        {
            if (xp >= ((level * level * level) + 30))
            {
                level++;
                xp = 0;
                Game1.chatBox.addMessage($"You leveled up Magic!", Color.Violet);

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
            config = instance.Helper.Data.ReadJsonFile<MagicData>($"magic/{Constants.SaveFolderName}.json") ?? new MagicData();

            xp = config.xp;
            level = config.level;
            baseChance = config.baseChance;
            mult = config.mult;
            mg_c = config.cheating;

            if (!File.Exists($"thieving/{Constants.SaveFolderName}.json"))
                instance.Helper.Data.WriteJsonFile<MagicData>($"magic/{Constants.SaveFolderName}.json", config);

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

            instance.Helper.Data.WriteJsonFile<MagicData>($"magic/{Constants.SaveFolderName}.json", config);
        }
    }


    public class MagicData //Changed from ModData (I think thats fine)
    {
        public int xp { get; set; } = 0;
        public int level { get; set; } = 1;
        public SButton key { get; set; } = SButton.Q;
        public double baseChance { get; set; } = 0.1;
        public double mult { get; set; } = 1.0;
        public bool cheating { get; set; }  = false;
    }


}
