/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Locations;

namespace BNC
{
    static class MineBuffManager
    {
        
        public static String CurrentAugment = null;
        public static bool finishedLoop = false;

        public static Dictionary<String, AugmentOption> Augments = new Dictionary<string, AugmentOption>();

        public static List<Monster> queue = new List<Monster>();

        private static DateTime startTime = DateTime.Now;
  

        public static void Init()
        {
            Augments.Add("health", new AugmentOption("health", "Monster Health Increase!", "Monster have 1.5x more health."));
            Augments.Add("regen", new AugmentOption("regen", "Health Regen!", "Player regens health over time."));
            Augments.Add("stamina", new AugmentOption("stamina", "Stamina Regen!", "Player regens stamina over time."));
            Augments.Add("attack", new AugmentOption("attack", "Monster Attack Increase!", "Monster have 1.5x more attack power"));
            Augments.Add("exp", new AugmentOption("exp", "Monsters Extra Exp!", "Monster have 1.5x more experiance."));
            Augments.Add("blood", new AugmentOption("blood", "Bleeding Out!", "Player loses health over time."));
            Augments.Add("tired", new AugmentOption("tired", "Getting Tired!", "Player loses stamina over time."));
            //Augments.Add("slimed", new AugmentOption("slimed", "Slime Spawner", "Slimes spawn around the player"));

            // not entirely working
            //Augments.Add("crabs", new AugmentOption("crabs", "You have crabs!", "Spawns Crabs on each level"));
            //Augments.Add("extra", new AugmentOption("extra", "More Mobs", "More mobs per level."));
        }

        public static void mineLevelChanged(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation is MineShaft mine)
            {
                if (shouldUpdateAugment(e))
                {
                    if (TwitchIntergration.isConnected())
                    {
                        if (!TwitchIntergration.hasMinePollStarted)
                            TwitchIntergration.StartMinePoll(getRandomBuff(3));
                    }
                    else
                    {
                        AugmentOption aug = getRandomBuff(1)[0];
                        CurrentAugment = aug.id;
                        Game1.addHUDMessage(new HUDMessage(aug.DisplayName + ": " + aug.desc, null));
                    }
                }

                if (CurrentAugment != null)
                    UpdateLocation(e);
            }

        }

        public static AugmentOption[] getRandomBuff(int count)
        {
            List<AugmentOption> list = Enumerable.ToList(Augments.Values);
            List<AugmentOption> returnList = new List<AugmentOption>();
            while (returnList.Count < 3)
            {
                int num = Game1.random.Next(list.Count);
                AugmentOption item = list[num];
                if (item != null && !returnList.Contains(item))
                    returnList.Add(item);
            }
            return returnList.ToArray();
        }

        public static void UpdateMineAugments()
        {

        }

        public static bool shouldUpdateAugment(WarpedEventArgs e)
        {
            if (e.NewLocation is MineShaft mine)
            {
                if (lastAugment() && (mine.mineLevel % BNC_Core.config.Mine_Augment_Every_x_Levels == 0 || CurrentAugment == null || mine.mineLevel == 1))
                {
                    startTime = DateTime.Now;
                    return true;
                }
            }

            return false;
        }


        public static bool lastAugment()
        {
            if (DateTime.Now > startTime.AddSeconds(120))
                return true;
            return false;
        }

        // Runs on each mine location update
        public static void UpdateLocation(WarpedEventArgs e)
        {

            if (!(e.NewLocation is MineShaft mine))
                return;

            foreach (NPC npc in Game1.player.currentLocation.characters)
            {
                if (!(npc is Monster)) continue;
                Monster m = (Monster)npc;
                switch (CurrentAugment)
                {
                    case "health":
                        BoostHealth(m);
                        break;
                    case "harder":
                        Harden(m);
                        break;
                    case "exp":
                        BoostExp(m);
                        break;
                    case "crabs":
                        int cnt = Game1.random.Next(2) + 1;
                        for (int i = 0; i < cnt; i++) 
                            queue.Add(new RockCrab(Vector2.Zero));
                        break;
                    case "extra":
                        foreach (NPC n in Game1.currentLocation.characters)
                        {
                            if (!(n is Monster)) continue;
                            int flag = Game1.random.Next(1);
                            if (flag.Equals(1))
                            {
                                int type = Game1.random.Next(1);
                                switch (type)
                                {
                                    case 0:
                                        queue.Add(new GreenSlime(Vector2.Zero));
                                        break;
                                    case 1:
                                        queue.Add(new RockCrab(Vector2.Zero));
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private static int augmentTick = 0;
        // Runs ever second
        public static void UpdateTick()
        {
            GameLocation location = Game1.player.currentLocation;

            if (!(location is MineShaft))
                return;

            if (queue.Count > 0)
            {
                
                bool flag = location.addCharacterAtRandomLocation(queue[0]);
                if (flag)
                    queue.RemoveAt(0);
            }

            switch (CurrentAugment)
            {
                case "regen":
                    if (augmentTick++ > 0)
                    {
                        if(Game1.player.health+1 <= Game1.player.maxHealth)
                            Game1.player.health += 1;

                        augmentTick = 0;
                    }
                    break;
                case "blood":
                    if (augmentTick++ > 1)
                    {
                        if (Game1.player.health > (Game1.player.maxHealth * 0.25))
                            Game1.player.health -= 1;
                        augmentTick = 0;
                    }
                    break;
                case "stamina":
                    if (augmentTick++ > 0)
                    {
                        if (Game1.player.Stamina + 2 <= Game1.player.MaxStamina)
                            Game1.player.Stamina += 2;
                        augmentTick = 0;
                    }
                    break;
                case "tired":
                    if (augmentTick++ > 1)
                    {
                        if (Game1.player.Stamina > (Game1.player.MaxStamina * 0.25))
                            Game1.player.Stamina -= 2;
                        augmentTick = 0;
                    }
                    break;
                default:
                    break;

            }

        }

        private static void BoostHealth(Monster m)
        {
            m.MaxHealth = (int)Math.Round(m.MaxHealth * 1.5);
            m.Health = m.MaxHealth;
        }

        private static void BoostExp(Monster m)
        {
            m.ExperienceGained = (int)Math.Round(m.ExperienceGained * 1.5);
        }

        private static void Harden(Monster m)
        {
            m.DamageToFarmer = (int)Math.Round(m.DamageToFarmer * 1.5);
        }


        public class AugmentOption
        {
            public String DisplayName;
            public String id;
            public String desc;
            public bool isNegative;

            public AugmentOption(String id, String name, String desc, bool isNegative = false)
            {
                this.DisplayName = name;
                this.id = id;
                this.desc = desc;
                this.isNegative = isNegative;
            }

        }
    }
}
