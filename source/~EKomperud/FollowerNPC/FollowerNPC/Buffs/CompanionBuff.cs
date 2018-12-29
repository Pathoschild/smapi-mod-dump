using System;
using System.Collections.Generic;
using StardewValley;

namespace FollowerNPC
{
    public class CompanionBuff
    {
        public Farmer buffOwner;

        public Buff buff;
        public Buff[] statBuffs;

        public CompanionBuff(Farmer farmer)
        {
            buffOwner = farmer;
        }

        public static CompanionBuff InitializeBuffFromCompanionName(string companionName, Farmer farmer)
        {
            CompanionBuff companionBuff;
            switch (companionName)
            {
                case "Abigail":
                    companionBuff = new Buffs.AbigailBuff(farmer);
                    break;
                case "Alex":
                    companionBuff = new Buffs.AlexBuff(farmer);
                    break;
                case "Elliott":
                    companionBuff = new Buffs.ElliottBuff(farmer);
                    break;
                case "Emily":
                    companionBuff = new Buffs.EmilyBuff(farmer);
                    break;
                case "Haley":
                    companionBuff = new Buffs.HaleyBuff(farmer);
                    break;
                case "Harvey":
                    companionBuff = new Buffs.HarveyBuff(farmer);
                    break;
                case "Leah":
                    companionBuff = new Buffs.LeahBuff(farmer);
                    break;
                case "Maru":
                    companionBuff = new Buffs.MaruBuff(farmer);
                    break;
                case "Penny":
                    companionBuff = new Buffs.PennyBuff(farmer);
                    break;
                case "Sam":
                    companionBuff = new Buffs.SamBuff(farmer);
                    break;
                case "Sebastian":
                    companionBuff = new Buffs.SebastianBuff(farmer);
                    break;
                case "Shane":
                    companionBuff = new Buffs.ShaneBuff(farmer);
                    break;
                default:
                    companionBuff = new Buffs.AbigailBuff(farmer);
                    break;
            }

            companionBuff.buff.millisecondsDuration = 1200000;
            Game1.buffsDisplay.addOtherBuff(companionBuff.buff);
            companionBuff.buff.which = -420;

            if (companionBuff.statBuffs != null)
            {
                foreach (Buff b in companionBuff.statBuffs)
                {
                    if (b != null)
                    {
                        b.millisecondsDuration = 1200000;
                        Game1.buffsDisplay.addOtherBuff(b);
                        b.which = -420;
                    }
                }
            }

            return companionBuff;
        }

        public virtual void RemoveAndDisposeCompanionBuff()
        {
            List<Buff> otherBuffs = Game1.buffsDisplay.otherBuffs;
            for (int i = 0; i < otherBuffs.Count; i++)
            {
                if (otherBuffs[i] != null && otherBuffs[i].which == -420)
                {
                    otherBuffs[i].removeBuff();
                    otherBuffs.RemoveAt(i);
                    i--;
                }
            }
            Game1.buffsDisplay.syncIcons();
        }

        public virtual void Update()
        {

        }
    }
}
