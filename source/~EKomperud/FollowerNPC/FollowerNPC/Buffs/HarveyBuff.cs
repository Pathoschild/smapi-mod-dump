using System;
using StardewValley;

namespace FollowerNPC.Buffs
{
    class HarveyBuff : CompanionBuff
    {
        protected int buffOwnerMaxHealth;
        protected int healthThreshold;
        protected int frameTimer;

        public HarveyBuff(Farmer farmer, NPC npc, CompanionsManager manager) : base(farmer, npc, manager)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Dr. Harvey is the master of both preventative and reactive medicine."+
                               Environment.NewLine+
                               "You gain defense equal to 1.5x your combat level and your health" +
                               Environment.NewLine+
                               "slowly regenerates while below 80%.";

            int defense = (int)(buffOwner.CombatLevel * 1.5f);
            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, defense, 0, 30, "", "");
            statBuffs[0].description = "+"+defense+" Defense" +
                                       Environment.NewLine +
                                       "Source: Harvey";

            buffOwnerMaxHealth = buffOwner.maxHealth;
            healthThreshold = (int)(buffOwnerMaxHealth * 0.8f);
        }

        public override void Update()
        {
            base.Update();

            // Tracks changes to the buff owner's max health and updates the health threshold accordingly
            if (buffOwner.maxHealth != buffOwnerMaxHealth)
            {
                buffOwnerMaxHealth = buffOwner.maxHealth;
                healthThreshold = (int)(buffOwnerMaxHealth * 0.8f);
            }

            // Regenerate buff owner's health if they're the health threshold
            if (buffOwner.health < healthThreshold && ++frameTimer == 90)
            {
                frameTimer = 0;
                buffOwner.health++;
            }
        }
    }
}
