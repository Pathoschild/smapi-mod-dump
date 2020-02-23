using Microsoft.Xna.Framework;
using NpcAdventure.Loader;
using NpcAdventure.Model;
using NpcAdventure.Utils;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Buffs
{
    class BuffManager
    {
        private const int DURATION = 1200000;
        private const int COMPANION_BUFF_ID = -0x1A4;
        private const int COMPANION_CYCLE_BUFF_ID = -0x1A6;
        private readonly IMonitor monitor;
        private int currentProstheticIndex;

        public BuffManager(NPC companion, Farmer farmer, IContentLoader contentLoader, IMonitor monitor)
        {
            this.Companion = companion;
            this.Farmer = farmer;
            this.ContentLoader = contentLoader;
            this.monitor = monitor;
            this.Prosthetics = new List<Buff>();
        }

        public NPC Companion { get; }
        public Farmer Farmer { get; }
        private IContentLoader ContentLoader { get; }
        public List<Buff> Prosthetics { get; private set; }

        public void AssignBuffs()
        {
            var buffData = this.ContentLoader.LoadStrings("Data/Buffs");

            if (!buffData.ContainsKey(this.Companion.Name))
                return;

            int[] buffAttrs = Utility.parseStringToIntArray(buffData[this.Companion.Name], '/');
            string desc = this.ContentLoader.LoadString($"Strings/Buffs:{this.Companion.Name}").Replace("#", Environment.NewLine);
            bool married = Helper.IsSpouseMarriedToFarmer(this.Companion, this.Farmer);

            if (buffAttrs.Length < 12)
            {
                this.monitor.Log($"Invalid buffs for {this.Companion.Name}", LogLevel.Error);
                return;
            }

            if (married && this.Farmer.getFriendshipHeartLevelForNPC(this.Companion.Name) >= 10)
            {
                string wifeOrHusband = this.ContentLoader.LoadString($"Strings/Strings:spouse{(this.Companion.Gender == 1 ? "Wife" : "Husband")}");
                desc += ("##" + this.ContentLoader.LoadString("Strings/Strings:spouseBuffDescription", wifeOrHusband + " " + this.Companion.displayName)).Replace("#", Environment.NewLine);
                buffAttrs[4] += 1; // extra luck
                buffAttrs[8] += 1; // extra magnetic radius
            }

            // Main buff indicator
            Buff buff = new Buff(desc, DURATION, this.Companion.Name, 21)
            {
                displaySource = this.Companion.displayName,
            };

            // Stat buffs
            Buff statBuff = new Buff(
                buffAttrs[0], buffAttrs[1], buffAttrs[2], buffAttrs[3],
                buffAttrs[4], buffAttrs[5], buffAttrs[6], buffAttrs[7],
                buffAttrs[8], buffAttrs[9], buffAttrs[10], buffAttrs[11],
                30, this.Companion.Name, this.Companion.displayName)
            {
                millisecondsDuration = DURATION,
                glow = married ? new Color(114, 0, 0, 50) : Color.White,
            };

            var keys = from k in buffData.Keys where k.StartsWith(this.Companion.Name + "~") select k;

            // Cycle buffs (if any is defined)
            foreach (var k in keys)
            {
                int[] cycleBuffAttrs = Utility.parseStringToIntArray(buffData[k], '/');

                if (buffAttrs.Length < 12)
                {
                    this.monitor.Log($"Invalid buffs for {this.Companion.Name}", LogLevel.Error);
                    continue;
                }

                Buff cycleBuff = new Buff(
                    cycleBuffAttrs[0], cycleBuffAttrs[1], cycleBuffAttrs[2], cycleBuffAttrs[3],
                    cycleBuffAttrs[4], cycleBuffAttrs[5], cycleBuffAttrs[6], cycleBuffAttrs[7],
                    cycleBuffAttrs[8], cycleBuffAttrs[9], cycleBuffAttrs[10], cycleBuffAttrs[11],
                    30, this.Companion.Name, this.Companion.displayName)
                {
                    millisecondsDuration = DURATION,
                };

                this.Prosthetics.Add(cycleBuff);
            }

            Game1.buffsDisplay.addOtherBuff(buff);
            Game1.buffsDisplay.addOtherBuff(statBuff);
            statBuff.which = buff.which = COMPANION_BUFF_ID;

            this.currentProstheticIndex = 0;

            if (this.Prosthetics.Count > 0)
            {
                var firstCycle = this.Prosthetics.First();
                Game1.buffsDisplay.addOtherBuff(firstCycle);
                firstCycle.which = COMPANION_CYCLE_BUFF_ID;
            }

            Game1.buffsDisplay.syncIcons();
        }

        public void ReleaseBuffs()
        {
            this.Prosthetics.Clear();
            CleanBuffs(COMPANION_BUFF_ID);
            CleanBuffs(COMPANION_CYCLE_BUFF_ID);

            Game1.buffsDisplay.syncIcons();
        }

        private static void CleanBuffs(int which)
        {
            List<Buff> otherBuffs = Game1.buffsDisplay.otherBuffs;
            List<Buff> toClean = new List<Buff>();

            for (int i = 0; i < otherBuffs.Count; i++)
            {
                if (otherBuffs[i] == null || otherBuffs[i].which != which)
                    continue;

                otherBuffs[i].removeBuff();
                toClean.Add(otherBuffs[i]);
            }

            // Clean icons in registry
            foreach (Buff buff in toClean)
                otherBuffs.Remove(buff);
        }

        public bool HasProsthetics()
        {
            return this.Prosthetics.Count > 0;
        }

        public void ChangeBuff()
        {
            if (!this.HasProsthetics())
                return;

            this.currentProstheticIndex = ++this.currentProstheticIndex % this.Prosthetics.Count;
            CleanBuffs(COMPANION_CYCLE_BUFF_ID);

            Buff buff = this.Prosthetics[this.currentProstheticIndex];
            buff.millisecondsDuration = DURATION;
            buff.which = -1;
            Game1.buffsDisplay.addOtherBuff(buff);
            buff.which = COMPANION_CYCLE_BUFF_ID;
            Game1.buffsDisplay.syncIcons();
            Game1.playSound("powerup");
        }
    }
}
