using System;
using StardewValley;

namespace FollowerNPC.Buffs
{
    class SebastianBuff : CompanionBuff
    {
        public SebastianBuff(Farmer farmer, NPC npc, CompanionsManager manager) : base(farmer, npc, manager)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Hmmmm. Something about Sebastian's buff seems familiar..." +
                               Environment.NewLine +
                               "You gain +1 speed, +1 luck, and +10% attack power.";

            statBuffs = new Buff[2];
            statBuffs[0] = new Buff(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, "", "");
            statBuffs[0].description = "+1 Luck" +
                                       Environment.NewLine +
                                       "Source: Sebastian";

            statBuffs[1] = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, "", "");
            statBuffs[1].description = "+1 Speed" +
                                       Environment.NewLine +
                                       "Source: Sebastian";

            OnAddAction();
        }

        public override void RemoveAndDisposeCompanionBuff()
        {
            base.RemoveAndDisposeCompanionBuff();
            OnRemoveAction();
        }

        protected void OnAddAction()
        {
            Game1.player.attackIncreaseModifier += 0.1f;
        }

        protected void OnRemoveAction()
        {
            Game1.player.attackIncreaseModifier -= 0.1f;
        }
    }
}
