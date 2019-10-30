using Microsoft.Xna.Framework;
using NpcAdventure.Loader;
using NpcAdventure.Model;
using NpcAdventure.Utils;
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

        public BuffManager(NPC companion, Farmer farmer, IContentLoader contentLoader)
        {
            this.Companion = companion;
            this.Farmer = farmer;
            this.ContentLoader = contentLoader;
        }

        public NPC Companion { get; }
        public Farmer Farmer { get; }
        private IContentLoader ContentLoader { get; }

        public bool HasAssignableBuffs()
        {
            return this.ContentLoader.CanLoad($"Buffs/{this.Companion.Name}");
        }

        public void AssignBuffs()
        {
            BuffInfo buffInfo = this.ContentLoader.Load<BuffInfo>($"Buffs/{this.Companion.Name}");
            if (buffInfo.attributes == null || buffInfo.attributes.Length < 12)
                throw new Exception("Invalid buff attributes!");

            int[] ft = buffInfo.attributes;
            // Main buff indicator
            Buff buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, this.Companion.Name, this.Companion.displayName)
            {
                millisecondsDuration = DURATION,
                description = buffInfo.description != null ? buffInfo.description.Replace("#", Environment.NewLine) : null,
            };
            // Stat buffs
            Buff statBuff = new Buff(
                ft[0], ft[1], ft[2], ft[3], ft[4], ft[5], ft[6], ft[7], ft[8], ft[9], ft[10], ft[11],
                30, this.Companion.Name, this.Companion.displayName)
            {
                millisecondsDuration = DURATION,
            };

            Game1.buffsDisplay.addOtherBuff(buff);
            buff.which = COMPANION_BUFF_ID;
            Game1.buffsDisplay.addOtherBuff(statBuff);
            statBuff.which = COMPANION_BUFF_ID;

            if (Helper.IsSpouseMarriedToFarmer(this.Companion, this.Farmer))
            {
                // Mariage additional buff (only farmer is married with this companion)
                Buff marriedLuckBuff = new Buff(0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 30, this.Companion.Name, this.Companion.displayName)
                {
                    millisecondsDuration = DURATION / 4,
                    description = this.ContentLoader.LoadString("Strings/Strings:spouseBuffDescription", this.Companion.displayName),
                    glow = new Color(114, 0, 0, 50), // Passion red lovely glowing
                };
                Game1.buffsDisplay.addOtherBuff(marriedLuckBuff);
                marriedLuckBuff.which = COMPANION_BUFF_ID;
            }

            Game1.buffsDisplay.syncIcons();
        }

        public void ReleaseBuffs()
        {
            List<Buff> otherBuffs = Game1.buffsDisplay.otherBuffs;
            List<Buff> toClean = new List<Buff>();

            for (int i = 0; i < otherBuffs.Count; i++)
            {
                if (otherBuffs[i] == null || otherBuffs[i].which != COMPANION_BUFF_ID)
                    continue;

                otherBuffs[i].removeBuff();
                toClean.Add(otherBuffs[i]);
            }

            // Clean icons in registry
            foreach (Buff buff in toClean)
                otherBuffs.Remove(buff);

            Game1.buffsDisplay.syncIcons();
        }
    }
}
