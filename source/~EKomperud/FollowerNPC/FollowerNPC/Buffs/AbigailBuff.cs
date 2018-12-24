using System;
using StardewValley;

namespace FollowerNPC.Buffs
{
    class AbigailBuff: CompanionBuff
    {
        public AbigailBuff(Farmer farmer) : base(farmer)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Hanging out with Abigail get's your adventurer's blood pumping!"+
                               Environment.NewLine+
                               "You gain +1 speed, +1 luck, and +10% attack power.";

            statBuffs = new Buff[2];
            statBuffs[0] = new Buff(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, "", "");
            statBuffs[0].description = "+1 Luck" +
                                       Environment.NewLine +
                                       "Source: Abigail";

            statBuffs[1] = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, "", "");
            statBuffs[1].description = "+1 Speed" +
                                       Environment.NewLine +
                                       "Source: Abigail";

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
