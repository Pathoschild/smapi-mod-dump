using System;
using System.Collections.Generic;
using StardewValley;

namespace FollowerNPC
{
    public class CompanionBuff
    {
        public Farmer buffOwner;
        public NPC buffGranter;
        public CompanionsManager manager;

        public Buff buff;
        public Buff[] statBuffs;

        public CompanionBuff(Farmer farmer, NPC npc, CompanionsManager manager)
        {
            buffOwner = farmer;
            buffGranter = npc;
            this.manager = manager;
        }

        public static CompanionBuff InitializeBuffFromCompanionName(string companionName, Farmer farmer, CompanionsManager cm)
        {
            NPC n = Game1.getCharacterFromName(companionName);
            CompanionBuff companionBuff;
            switch (companionName)
            {
                case "Abigail":
                    companionBuff = new Buffs.AbigailBuff(farmer, n, cm);
                    break;
                case "Alex":
                    companionBuff = new Buffs.AlexBuff(farmer, n, cm);
                    break;
                case "Elliott":
                    companionBuff = new Buffs.ElliottBuff(farmer, n, cm);
                    break;
                case "Emily":
                    companionBuff = new Buffs.EmilyBuff(farmer, n, cm);
                    break;
                case "Haley":
                    companionBuff = new Buffs.HaleyBuff(farmer, n, cm);
                    break;
                case "Harvey":
                    companionBuff = new Buffs.HarveyBuff(farmer, n, cm);
                    break;
                case "Leah":
                    companionBuff = new Buffs.LeahBuff(farmer, n, cm);
                    break;
                case "Maru":
                    companionBuff = new Buffs.MaruBuff(farmer, n, cm);
                    break;
                case "Penny":
                    companionBuff = new Buffs.PennyBuff(farmer, n, cm);
                    break;
                case "Sam":
                    companionBuff = new Buffs.SamBuff(farmer, n, cm);
                    break;
                case "Sebastian":
                    companionBuff = new Buffs.SebastianBuff(farmer, n, cm);
                    break;
                case "Shane":
                    companionBuff = new Buffs.ShaneBuff(farmer, n, cm);
                    break;
                default:
                    companionBuff = new Buffs.AbigailBuff(farmer, n, cm);
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

        public virtual void SpecialAction()
        {

        }
    }
}
