using System;
using StardewValley;

namespace FollowerNPC.Buffs
{
    class HarveyBuff : CompanionBuff
    {
        protected int buffOwnerMaxHealth;
        protected int healthThreshold;
        protected int frameTimer;

        public HarveyBuff(Farmer farmer, NPC npc) : base(farmer, npc)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Dr. Harvey is the master of both preventative and reactive medicine."+
                               Environment.NewLine+
                               "You gain +10 defense and your health slowly regenerates while below 80%.";

            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10, 0, 30, "", "");
            statBuffs[0].description = "+10 Defense" +
                                       Environment.NewLine +
                                       "Source: Harvey";

            buffOwnerMaxHealth = buffOwner.maxHealth;
            healthThreshold = (int)(buffOwnerMaxHealth * 0.7f);
        }

        public override void Update()
        {
            base.Update();

            // Tracks changes to the buff owner's max health and updates the health threshold accordingly
            if (buffOwner.maxHealth != buffOwnerMaxHealth)
            {
                buffOwnerMaxHealth = buffOwner.maxHealth;
                healthThreshold = (int)(buffOwnerMaxHealth * 0.7f);
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
