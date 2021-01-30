/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/F1r3w477/TheseModsAintLoyal
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Reflection;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Locations;
using StardewValley.Characters;
using System.Collections.Generic;
using System.Collections;
using System.Timers;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Objects;
using System.Text;
using System.Threading.Tasks;
using System.Linq;


namespace MoodyPlanetEvolution
{
    public class ModEntry : Mod
    {
        public static Mod instance;
        public Random wnd;
        int alpha = -1;
        public List<int> hashofm;
        public List<int> blackhash;
        public Mood mood;
        LEModApi api;
        bool mpdebug;
        List<Mood> Moods = new List<Mood>();
        int[] ws = new int[] {700, 250, 45, 5 };
        public ModEntry MP;
        public int totalWeight = 0;
        public override void Entry(IModHelper helper)
        {
            instance = this;
            MP = this;
            hashofm = new List<int>();
            blackhash = new List<int>();
            wnd = new Random();
            mpdebug = false;

            helper.Events.GameLoop.DayStarted += TimeEvents_AfterDayStarted;
            helper.Events.GameLoop.OneSecondUpdateTicked += GameEvents_OneSecondTick;
            helper.Events.GameLoop.GameLaunched += GameEvents_FirstUpdateTick;

            helper.ConsoleCommands.Add("mood", "Tells player world mood.", this.tellMood);
            helper.ConsoleCommands.Add("moodmultis", "Tells player world MoodMultipliers.", this.tellMultipliers);
            helper.ConsoleCommands.Add("mm", "Tells player world MoodMultipliers.", this.tellMultipliers);
            helper.ConsoleCommands.Add("mpdebug", "Turns debug on and off", this.MPDebug);

            Moods.Add(new Mood("Happy", ws[0], new double[] { 1.0, 1.0, 1.0, 1.1, 1.0, 1.0 }, -1.0, Color.Green, MP));                                   //T1
            Moods.Add(new Mood("Content", ws[1], new double[] { 1.0, 1.0, 1.0, 1.2, 1.0, 1.0 }, -1.0, Color.CornflowerBlue, MP));                        //T2
            Moods.Add(new Mood("Untroubled", ws[2], new double[] { 0.85, 0.9, 1.0, 1.25, 1.0, 1.0 }, -1.0, Color.Purple, MP));                           //T3
            Moods.Add(new Mood("Sad", ws[0], new double[] { 1.0, 1.0, 1.0, 1.1, 1.0, 1.0 }, -1.0, Color.Green, MP));                                     //T1
            Moods.Add(new Mood("Gloomy", ws[1], new double[] { 1.0, 1.0, 1.0, 0.9, 1.0, 1.0 }, -1.0, Color.CornflowerBlue, MP));                         //T2
            Moods.Add(new Mood("Depressed", ws[2], new double[] { 1.0, 1.0, 1.0, 0.65, 0.85, 0.8 }, -1.0, Color.Purple, MP));                            //T3
            Moods.Add(new Mood("Annoyed", ws[0], new double[] { 1.8, 2.0, 1.85, 2.2, 1.0, 1.25 }, -1.0, Color.Green, MP));                               //T1
            Moods.Add(new Mood("Angry", ws[1], new double[] { 2.0, 3.0, 2.0, 2.45, 1.25, 1.75 }, -1.0, Color.CornflowerBlue, MP));                       //T2
            Moods.Add(new Mood("Furious", ws[2], new double[] { 2.5, 4.25, 2.0, 2.75, 1.5, 2.0 }, -1.0, Color.Purple, MP));                              //T3
            Moods.Add(new Mood("Mellow", ws[0], new double[] { 1.0, 0.9, 1.0, 1.15, 1.0, 0.9 }, -1.0, Color.Green, MP));                                 //T1
            Moods.Add(new Mood("Serene", ws[1], new double[] { 0.95, 0.8, 1.0, 1.25, 1.68, 1.8 }, -1.0, Color.CornflowerBlue, MP));                      //T2
            Moods.Add(new Mood("Enlightned", ws[2], new double[] { 0.85, 0.75, 0.9, 1.45, 1.0, 0.9 }, -1.0, Color.Purple, MP));                          //T3
            Moods.Add(new Mood("Indifferent", ws[0], new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 }, -1.0, Color.Green, MP));                             //T1
            Moods.Add(new Mood("Uncaring", ws[1], new double[] { 1.0, 1.0, 1.0, 1.0, 1.25, 1.25 }, -1.0, Color.CornflowerBlue, MP));                     //T2
            Moods.Add(new Mood("Uninterested", ws[2], new double[] { 1.0, 1.0, 1.0, 1.0, 2.0, 2.0 }, -1.0, Color.Purple, MP));                           //T3
            Moods.Add(new Mood("Tired", ws[0], new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 0.85 }, -1.0, Color.Green, MP));                                  //T1
            Moods.Add(new Mood("Restless", ws[1], new double[] { 1.0, 1.0, 1.0, 1.0, 1.0, 0.6 }, -1.0, Color.CornflowerBlue, MP));                       //T2
            Moods.Add(new Mood("Anxious", ws[2], new double[] { 0.9, 0.9, 1.0, 1.0, 0.9, 0.5 }, -1.0, Color.Purple, MP));                                //T3
            Moods.Add(new Mood("Loved", ws[0], new double[] { 1.0, 0.9, 1.0, 1.35, 1.0, 1.0 }, -1.0, Color.Green, MP));                                  //T1
            Moods.Add(new Mood("Cherished", ws[1], new double[] { 1.0, 0.75, 1.0, 1.65, 1.0, 1.0 }, -1.0, Color.CornflowerBlue, MP));                    //T2
            Moods.Add(new Mood("Adored", ws[2], new double[] { 1.0, 0.2, 1.0, 1.95, 1.0, 1.0 }, -1.0, Color.Purple, MP));                                //T3
            Moods.Add(new Mood("Mysterious", ws[0], new double[] { -1.0, -1.0, -1.0, -1.0, -1.0, -1.0 }, -1.0, Color.Green, MP));                        //T1
            Moods.Add(new Mood("Cryptic", ws[1], new double[] { -1.0, -1.0, -1.0, -1.0, -1.0, -1.0 }, -1.0, Color.CornflowerBlue, MP));                  //T2
            Moods.Add(new Mood("Unexplainable", ws[2], new double[] { -1.0, -1.0, -1.0, -1.0, -1.0, -1.0 }, -1.0, Color.Purple, MP));                    //T3
            Moods.Add(new Mood("Arrogant", ws[0], new double[] { 4.0, 0.9, 1.0, 2.3, 1.0, 1.0 }, -1.0, Color.Green, MP));                                //T1
            Moods.Add(new Mood("Narcissistic", ws[1], new double[] { 6.0, 0.7, 1.0, 2.6, 1.0, 1.0 }, -1.0, Color.CornflowerBlue, MP));                   //T2
            Moods.Add(new Mood("Egotistical", ws[2], new double[] { 9.0, 0.6, 1.0, 3.0, 1.15, 1.15 }, -1.0, Color.Purple, MP));                          //T3
            Moods.Add(new Mood("Crazy", ws[0], new double[] { 3.5, 1.85, 1.5, 3.3, 1.0, 1.8 }, -1.0, Color.Green, MP));                                  //T1
            Moods.Add(new Mood("Irrational", ws[1], new double[] { 5.0, 2.9, 2.0, 4.65, 1.65, 2.3 }, -1.0, Color.CornflowerBlue, MP));                   //T2
            Moods.Add(new Mood("Insane", ws[2], new double[] { 6.0, 4.0, 2.0, 6.0, 2.0, 2.6 }, -1.0, Color.Purple, MP));                                 //T3
            Moods.Add(new Mood("Holy shit just sleep again.", ws[3], new double[] { 15.0, 10.0, 4.0, 40.0, 3.0, 3.0 }, -1.0, Color.Black, MP));          //T9

            foreach (Mood m in Moods)
            {
                totalWeight = m.weight + totalWeight;
            }
        }

        private void MPDebug(string arg1, string[] arg2)
        {
            if (mpdebug)
            {
                mpdebug = false;
                Monitor.Log("-MOODY--PLANET-> Debug Turned Off. <--DEBUG--");
            }
            else if (!mpdebug)
            {
                mpdebug = true;
                Monitor.Log("-MOODY--PLANET-> Debug Turned On. <--DEBUG--");
            }

        }

        private void tellMultipliers(string arg1, string[] arg2)
        {
            Monitor.Log($"Health: {mood.modifiers[0]}, Resilience: {mood.modifiers[1]}, Slipperiness: {mood.modifiers[2]}, Combat XP Bonus: {mood.modifiers[3]}, " +
                $"Scale: {mood.modifiers[4]}, Speed: {mood.modifiers[5]} ");
        }

        private void TimeEvents_AfterDayStarted(object sender, EventArgs e)
        {

            if (Context.IsWorldReady)
            {
                if (Context.IsWorldReady)
                {
                    hashofm.RemoveRange(0, hashofm.Count);
                    blackhash.RemoveRange(0, blackhash.Count);
                    if (mpdebug)
                        Monitor.Log($"-MOODY--PLANET->Removed Monsters from List ||| {blackhash.Count}, |/| {hashofm.Count} |||<--DEBUG--");
                }

                int rnum = wnd.Next(0, totalWeight);

                foreach (Mood m in Moods)
                {
                    if (rnum < m.weight)
                    {
                        mood = m;
                        break;
                    }
                    rnum = rnum - m.weight;
                }
                if (mood.name == "Mysterious")
                {
                    mood.randomizedModifiers(1.0);
                }
                else if (mood.name == "Cryptic")
                {
                    mood.randomizedModifiers(4.0);
                }
                else if (mood.name == "Unexplainable")
                {
                    mood.randomizedModifiers(6.0);
                }

                if (mood.spawnRate != -1.0)
                    api.Spawn_Rate(mood.spawnRate);

                DisplayMood();
            }
        }

        private void GameEvents_FirstUpdateTick(object sender, EventArgs e)
        {
            if (this.Helper.ModRegistry.IsLoaded("Unidarkshin.LevelExtender"))
            {
                api = this.Helper.ModRegistry.GetApi<LEModApi>("Unidarkshin.LevelExtender");
            }

        }

        private void tellMood(string command, string[] args)
        {
            this.Monitor.Log(mood.name);
        }

        private void GameEvents_OneSecondTick(object sender, EventArgs e)
        {

            if (Context.IsWorldReady)
            {
                int y = 0;

                foreach (NPC m in Game1.player.currentLocation.characters.OfType<NPC>())
                {

                    if (m.IsMonster && !hashofm.Contains(m.GetHashCode()) && !blackhash.Contains(m.GetHashCode()))
                    {
                        hashofm.Add(m.GetHashCode());
                        y++;

                    }

                }
                if (alpha == -1)
                {
                    alpha = y;
                }
                else
                {
                    if (alpha != y)
                    {
                        alpha = y;
                        rem_mons();
                    }
                    else
                    {

                    }
                }
            }
        }

        public void rem_mons()
        {

            foreach (NPC m in Game1.player.currentLocation.characters.OfType<NPC>())

            {


                if (m.IsMonster && hashofm.Contains(m.GetHashCode()) && !blackhash.Contains(m.GetHashCode()))
                {
                    Monster H = (m as Monster);
                    H.Health = (int)(H.Health * mood.modifiers[0]);
                    H.resilience.Value = (int)(H.resilience.Value * mood.modifiers[1]);
                    H.Slipperiness = (int)(H.Slipperiness * mood.modifiers[2]);
                    H.ExperienceGained = (int)(H.ExperienceGained * mood.modifiers[3]);
                    H.Scale = (int)(H.Scale * mood.modifiers[4]);
                    H.Speed = (int)(H.Speed * mood.modifiers[5]);
                    blackhash.Add(m.GetHashCode());
                    hashofm.Remove(m.GetHashCode());
                    if (mpdebug)
                    {
                        Monitor.Log("-MOODY--PLANET-> Applied status changes to monsters in this location. <--DEBUG--");
                        Monitor.Log($"-MOODY--PLANET-> {H.Name} <--DEBUG--");
                        Monitor.Log($"-MOODY--PLANET-> {H.Health} <--DEBUG--");
                    }
                }
            }


        }

        public void DisplayMood()
        {
            if (mpdebug)
            {
                Monitor.Log($"Health: {mood.modifiers[0]}, Resilience: {mood.modifiers[1]}, Slipperiness: {mood.modifiers[2]}, Combat XP Bonus: {mood.modifiers[3]}, " +
                $"Scale: {mood.modifiers[4]}, Speed: {mood.modifiers[5]}  < --DEBUG--");
                Monitor.Log($"-MOODY--PLANET-> World Mood : {mood.name} <--DEBUG--");
            }
            HUDMessage message = new HUDMessage($"The world is {mood.name} today!", mood.moodColor, 8000.0f);
            Game1.addHUDMessage(message);
            message.noIcon = true;
            message.update(Game1.currentGameTime);
        }

    }

    public class Mood
    {
        public ModEntry MP;
        public string name;
        public int weight;
        public double[] modifiers;
        public double spawnRate;
        public Color moodColor;

        public Mood(string name, int weight, double[] modifiers, double spawnRate, Color moodColor, ModEntry MP)
        {
            this.name = name;
            this.weight = weight;
            this.modifiers = modifiers;
            this.spawnRate = spawnRate;
            this.moodColor = moodColor;
            this.MP = MP;
        }

        public void randomizedModifiers(double rate) 
        {
            for (int i = 0; i < modifiers.Length; i++)
            {
                modifiers[i] = Math.Round((MP.wnd.NextDouble() * rate) + 1.0, 3);
            }
        }
    }
}
