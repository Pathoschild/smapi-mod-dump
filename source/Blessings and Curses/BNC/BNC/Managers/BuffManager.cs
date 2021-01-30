/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GenDeathrow/SDV_BlessingsAndCurses
**
*************************************************/

using BNC.Configs;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BNC
{
    public static class BuffManager
    {
        public static Dictionary<string, BuffOption> CommonBuffs = new Dictionary<string, BuffOption>();
        public static Dictionary<string, BuffOption> CombatBuffs = new Dictionary<string, BuffOption>();

        public static Random rand = new Random();

        public static List<BuffOption> queuedBuff = new List<BuffOption>();

        public static void Init()
        {
            AddBuff(new BuffOption("greenthumb", "Green Thumb").add_farming(4).addShortDesc("Buff Farming +4"));
            AddBuff(new BuffOption("brownthumb", "Brown Thumb", false).add_farming(-4).addShortDesc("Debuff Farming -4"));
            AddBuff(new BuffOption("goodfish","Good Fishing Advice").add_fishing(5).addShortDesc("Buff Fishing +5"));
            AddBuff(new BuffOption("badfish", "Bad Fishing Advice", false).add_fishing(-5).addShortDesc("Debuff Fishing -5"));
            AddBuff(new BuffOption("w2", "+2 Weapon").add_attack(2).addShortDesc("Buff Attack +2"));
            AddBuff(new BuffOption("w4", "+4 Weapon").add_attack(4).addShortDesc("Buff Attack +4"));
            AddBuff(new BuffOption("bombluck", "Bomb Luck!", false).add_defense(-1).addShortDesc("Bombs sometimes spawn around the player. Thats some bad luck...."));
            AddBuff(new BuffOption("slimer", "Slimes!!", false).add_defense(1).addShortDesc("Slimes randomly spawn around you."));
            AddBuff(new BuffOption("beserk","Beserker", true, 600).add_attack(6).addShortDesc("Buff Attack +6").setGlow(Color.OrangeRed));
            AddBuff(new BuffOption("rusted","Rusted Weapon", false).add_attack(-2).addShortDesc("Debuff Attack -2"));
            AddBuff(new BuffOption("broken","Broken Weapon", false).add_attack(-4).addShortDesc("Debuff Attack -4"));
            AddBuff(new BuffOption("missing","Missing Weapon", false).add_attack(-8).addShortDesc("Debuff Attack -8"));
            AddBuff(new BuffOption("a2","+2 Armor").add_defense(2).addShortDesc("Buff Defense +2"));
            AddBuff(new BuffOption("a3","+3 Armor").add_defense(3).addShortDesc("Buff Defense +3"));
            AddBuff(new BuffOption("a-2","-2 Armor", false).add_defense(-2).addShortDesc("Debuff Defense -2"));
            AddBuff(new BuffOption("a-3", "-3 Armor", false).add_defense(-3).addShortDesc("Debuff Defense -3"));
            AddBuff(new BuffOption("hippie", "Hippie").add_foraging(2).addShortDesc("Buff Foraging +2"));
            AddBuff(new BuffOption("vegan", "Vegan").add_foraging(4).addShortDesc("Buff Foraging +4"));
            AddBuff(new BuffOption("lumber", "Lumberjack").add_foraging(8).addShortDesc("Buff Foraging +8"));
            AddBuff(new BuffOption("yuppie", "Yuppies", false).add_foraging(-2).addShortDesc("Debuff Foraging -2"));
            AddBuff(new BuffOption("carnivore","Carnivore", false).add_foraging(-4).addShortDesc("Debuff Foraging -4"));
            AddBuff(new BuffOption("worksout", "Works Out", false).add_maxStamina(80).addShortDesc("Buff Stamina +80"));
            AddBuff(new BuffOption("potato", "Couch Potato").add_maxStamina(-80).addShortDesc("Debuff Stamina -80"));
            AddBuff(new BuffOption("bionic","Bionic Legs", true, 600).add_speed(4).addShortDesc("Buff Speed +4").setGlow(Color.LightBlue));
            AddBuff(new BuffOption("short", "I'm Just Short", false, 600).add_speed(-2).addShortDesc("Debuff Speed -2"));
            AddBuff(new BuffOption("clover", "Lucky").add_luck(2).addShortDesc("Buff Luck +2"));
            AddBuff(new BuffOption("stepped", "UnLucky", false).add_luck(-2).addShortDesc("Debuff Luck -2"));
            AddBuff(new BuffOption("lottery", "Lottery Winner").add_luck(4).addShortDesc("Buff Luck +4"));
            AddBuff(new BuffOption("mirror", "Broke a Mirror", false).add_luck(-4).addShortDesc("Debuff Luck -8"));
        }

        public static void AddBuff(BuffOption buff)
        {
            if (!CommonBuffs.ContainsKey(buff.id))
                CommonBuffs.Add(buff.id, buff);
        }

        public static void AddComBatBuff(BuffOption buff)
        {
            if (!CombatBuffs.ContainsKey(buff.id))
                CombatBuffs.Add(buff.id, buff);
        }

        public static BuffOption[] getRandomBuff(int count)
        {
            List<BuffOption> list = Enumerable.ToList(CommonBuffs.Values);
            List<BuffOption> returnList = new List<BuffOption>();
            while(returnList.Count < 3)
            {
                BuffOption item = list[rand.Next(list.Count)];

                if(item != null && !returnList.Contains(item))
                    returnList.Add(item);
            }
            return returnList.ToArray(); 
        }
        /*
        public static void UpdateTick()
        {
            foreach (var item in Game1.player.appliedSpecialBuffs())
            {
                if(item == "bombs")
                {
                    if(rand.NextDouble() < 0.05f)
                    {
                        Item bomb = Utility.getItemFromStandardTextDescription($"O 287 1", Game1.player);
                        BombEvent.updateQueue.Enqueue(bomb);
                    }
                }
            }
        }
        */
        public static BuffOption getIDtoBuff(string id)
        {
            if (CommonBuffs.ContainsKey(id))
            {
                return CommonBuffs[id];
            }
            else if (CombatBuffs.ContainsKey(id))
            {
                return CombatBuffs[id];
            }
            return null;
        }

        public static void AddBuffToQueue(BuffOption buff) {
            queuedBuff.Add(buff);
        }


        public static void UpdateTick()
        {
            if(queuedBuff.Count() >= 1)
            {
                if(buffPlayer(queuedBuff[0]))
                    queuedBuff.RemoveAt(0);
            }

        }

        public static void UpdateDay()
        {

            if (BNC_Core.BNCSave.nextBuffDate <= -1)
            {
                int[] Min_Max = Config.getBuffMinMax();
                BNC_Core.BNCSave.nextBuffDate = setNextBuffDay(Min_Max[0], Min_Max[1]);
            }
            if (BNC_Core.BNCSave.nextBuffDate-- == 0)
            {
                BuffOption[] buffs = getRandomBuff(3);

                if (Config.ShowDebug())
                    foreach (BuffOption buff in buffs)
                        BNC_Core.Logger.Log($"buff selecting from {buff.displayName}");

                if (TwitchIntergration.isConnected())
                {
                    TwitchIntergration.StartBuffPoll(buffs);
                }
                else
                {
                    BuffOption selected = buffs[rand.Next(buffs.Count() - 1)];
                    if (Config.ShowDebug())
                        BNC_Core.Logger.Log($"Selected {selected.displayName}");
                    buffPlayer(selected);
                    int[] Min_Max = Config.getBuffMinMax();
                    BNC_Core.BNCSave.nextBuffDate = setNextBuffDay(Min_Max[0], Min_Max[1]);
                }
            }

        }

        public static void Update()
        {
            foreach (var buff in Game1.buffsDisplay.otherBuffs.ToList())
            {
                if (buff == null && buff.source != null) continue;

                if (buff.source.Equals("Bomb Luck!"))
                {
                    double i = rand.NextDouble();
                    if (i < 0.01d)
                    {
                        Item bomb = Utility.getItemFromStandardTextDescription($"O 287 1", Game1.player);
                        Actions.BombEvent.updateQueue.Enqueue(bomb);
                    }
                }

                if (buff.source.Equals("Slimes!!"))
                {
                    double i = rand.NextDouble();
                    if (i < 0.05d)
                    {
                        Spawner.addMonsterToSpawn(new GreenSlime(Vector2.Zero), "");
                    }
                }
            }
        }

        public static bool buffPlayer(BuffOption buff)
        {
            List<StardewValley.Buff> Duppiclates = Game1.buffsDisplay.otherBuffs.Where(b => b.source == buff.displayName).ToList();
            if (Duppiclates.Count() > 0)
            {
                BNC_Core.Logger.Log("Found Duplicates", StardewModdingAPI.LogLevel.Debug);

                foreach (StardewValley.Buff buffitem in Game1.buffsDisplay.otherBuffs.ToArray())
                {
                        if(buffitem.source == buff.displayName)
                        {
                            buff.CombineBuffs(buffitem);
                            Game1.buffsDisplay.otherBuffs.Remove(buffitem);
                        }
                }
            }


            Buff buffselected = new Buff(buff.farming, buff.fishing, buff.mining, 0, 0, buff.foraging, buff.crafting, buff.maxStamina, buff.magneticRadius, buff.speed, buff.defense, buff.attack, buff.duration, buff.displayName, buff.displayName);
            if (buff.color != Color.White)
                buffselected.glow = buff.color;

            Game1.buffsDisplay.addOtherBuff(buffselected);

            if (buff.Equals(CommonBuffs["potato"]))
            {
                if(Game1.player.Stamina > Game1.player.MaxStamina) Game1.player.Stamina = Game1.player.MaxStamina;
            }

            Game1.addHUDMessage(new HUDMessage(buff.shortdesc, buff.isBuff ? 4 : 3));

            return true;
        }

        public static int setNextBuffDay(int number1, int number2)
        {
            if (number1.Equals(number2)) return number1;

            int min = Math.Min(number1, number2);
            int max = Math.Max(number1, number2);
            int random = min != max ? rand.Next(max - min) + min : rand.Next(min - 1) + 1;
            return random;
        }


        public class BuffOption
        {
            //Overrides to use a vanilla buff
            public int which = -1;
            internal string hudMessage { get; set; }

            public int farming { get; set; }
            public int fishing { get; set; }
            public int mining { get; set; }
            public int luck { get; set; }
            public int foraging { get; set; }
            public int crafting { get; set; }
            public int maxStamina { get; set; }
            public int magneticRadius { get; set; }
            public int speed { get; set; }
            public int defense { get; set; }
            public int attack { get; set; }
            public int duration { get; set; }
            public bool isBuff { get; set; }
            public string displayName { get; set; }
            public string id { get; set; }
            public string description { get; set; } = "null";
            public string shortdesc { get; set; } = "null";
            public Color color { get; set; } = Color.White;

            public BuffOption(string id, string name, bool isBuff = true, int duration = 1200)
            {
                this.id = id;
                this.displayName = name;
                this.duration = duration;
                this.isBuff = isBuff;
            }

            public BuffOption addDescription(string desc)
            {
                this.description = description;
                return this;
            }

            public BuffOption addShortDesc(string desc)
            {
                this.shortdesc = desc;
                return this;
            }
            public BuffOption add_farming(int value)
            {
                this.farming = value;
                return this;
            }

            public BuffOption add_fishing(int value)
            {
                this.fishing = value;
                return this;
            }

            public BuffOption add_mining(int value)
            {
                this.mining = value;
                return this;
            }

            public BuffOption add_luck(int value)
            {
                this.luck = value;
                return this;
            }

            public BuffOption add_foraging(int value)
            {
                this.foraging = value;
                return this;
            }

            public BuffOption add_crafting(int value)
            {
                this.crafting = value;
                return this;
            }

            public BuffOption add_maxStamina(int value)
            {
                this.maxStamina = value;
                return this;
            }

            public BuffOption add_magneticRadius(int value)
            {
                this.magneticRadius = value;
                return this;
            }

            public BuffOption add_speed(int value)
            {
                this.speed = value;
                return this;
            }

            public BuffOption add_defense(int value)
            {
                this.defense = value;
                return this;
            }

            public BuffOption add_attack(int value)
            {
                this.attack = value;
                return this;
            }

            public BuffOption setDuration(int value)
            {
                this.duration = value;
                return this;
            }

            public BuffOption setGlow(Color color)
            {
                this.color = color;
                return this;
            }

            public BuffOption CombineBuffs(Buff buff)
            {
                this.farming += buff.buffAttributes[0];
                this.fishing += buff.buffAttributes[1];
                this.mining += buff.buffAttributes[2];
                this.luck += buff.buffAttributes[4];
                this.foraging += buff.buffAttributes[5];
                this.crafting += buff.buffAttributes[6];
                this.maxStamina += buff.buffAttributes[7];
                this.magneticRadius += buff.buffAttributes[8];
                this.speed += buff.buffAttributes[9];
                this.defense += buff.buffAttributes[10];
                this.attack += buff.buffAttributes[11];

                if (speed > 20)
                    this.speed = 20;

                return this;
            }


        }

    }
}
