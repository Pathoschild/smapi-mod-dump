using System;
using StardewValley;

namespace FollowerNPC.Buffs
{
    class EmilyBuff : CompanionBuff
    {
        protected int buffOwnerLastFrameHealth;

        protected int buffOwnerMaxHealth;
        protected int healthThreshold;
        protected int frameTimer;

        public EmilyBuff(Farmer farmer, NPC npc, CompanionsManager manager) : base(farmer, npc, manager)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Emily and her powerful spirit will aid you while mining." +
                               Environment.NewLine +
                               "She grants you +2 mining, briefly shields you after being damaged," +
                               Environment.NewLine +
                               "and quickly regenerates your health when it's lower than 40%.";

            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            statBuffs[0].description = "+2 Mining" +
                                       Environment.NewLine +
                                       "Source: Emily";

            buffOwnerLastFrameHealth = buffOwner.health;

            buffOwnerMaxHealth = buffOwner.maxHealth;
            healthThreshold = (int)(buffOwnerMaxHealth * 0.4f);
        }

        public override void Update()
        {
            base.Update();

            // Tracks changes to the buff owner's max health and updates the health threshold accordingly
            if (buffOwner.maxHealth != buffOwnerMaxHealth)
            {
                buffOwnerMaxHealth = buffOwner.maxHealth;
                healthThreshold = (int)(buffOwnerMaxHealth * 0.4f);
            }

            // Shield buff owner if they've been damaged
            if (buffOwner.health < buffOwnerLastFrameHealth)
            {
                buffOwner.currentLocation.playSound("yoba");
                Game1.buffsDisplay.addOtherBuff(new Buff(21));
            }

            // Regenerate buff owner's health if they're the health threshold
            if (buffOwner.health < healthThreshold && ++frameTimer == 30)
            {
                frameTimer = 0;
                buffOwner.health++;
            }

            buffOwnerLastFrameHealth = buffOwner.health;
        }
    }
}
