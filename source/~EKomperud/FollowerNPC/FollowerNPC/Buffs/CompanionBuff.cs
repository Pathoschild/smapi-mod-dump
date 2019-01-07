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

        public CompanionBuff(Farmer farmer, NPC npc)
        {
            buffOwner = farmer;
            buffGranter = npc;
        }

        public static CompanionBuff InitializeBuffFromCompanionName(string companionName, Farmer farmer, CompanionsManager cm)
        {
            NPC n = Game1.getCharacterFromName(companionName);
            CompanionBuff companionBuff;
            switch (companionName)
            {
                case "Abigail":
                    companionBuff = new Buffs.AbigailBuff(farmer, n);
                    break;
                case "Alex":
                    companionBuff = new Buffs.AlexBuff(farmer, n);
                    break;
                case "Elliott":
                    companionBuff = new Buffs.ElliottBuff(farmer, n);
                    break;
                case "Emily":
                    companionBuff = new Buffs.EmilyBuff(farmer, n);
                    break;
                case "Haley":
                    companionBuff = new Buffs.HaleyBuff(farmer, n);
                    break;
                case "Harvey":
                    companionBuff = new Buffs.HarveyBuff(farmer, n);
                    break;
                case "Leah":
                    companionBuff = new Buffs.LeahBuff(farmer, n);
                    (companionBuff as Buffs.LeahBuff).SetForageFoundDialogue(new string[]
                    {
                        cm.npcDialogueScripts["Leah"]["companionPerk1"],
                        cm.npcDialogueScripts["Leah"]["companionPerk2a"],
                        cm.npcDialogueScripts["Leah"]["companionPerk2b"],
                    });
                    break;
                case "Maru":
                    companionBuff = new Buffs.MaruBuff(farmer, n);
                    break;
                case "Penny":
                    companionBuff = new Buffs.PennyBuff(farmer, n);
                    break;
                case "Sam":
                    companionBuff = new Buffs.SamBuff(farmer, n);
                    break;
                case "Sebastian":
                    companionBuff = new Buffs.SebastianBuff(farmer, n);
                    break;
                case "Shane":
                    companionBuff = new Buffs.ShaneBuff(farmer, n);
                    break;
                default:
                    companionBuff = new Buffs.AbigailBuff(farmer, n);
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

            companionBuff.manager = cm;

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
