using System;
using StardewValley;
using StardewValley.Monsters;

namespace FollowerNPC.Buffs
{
    class AlexBuff : CompanionBuff
    {
        protected uint buffOwnerLastMonstersSlain;

        public AlexBuff(Farmer farmer) : base(farmer)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Alex's fighting spirit seems to awaken yours as well." + 
                               Environment.NewLine + 
                               "You get +20% attack power and gain 'Warrior Energy' after slaying an enemy.";

            buffOwnerLastMonstersSlain = buffOwner.stats.MonstersKilled;
            OnAddAction();
        }

        public override void Update()
        {
            base.Update();

            if (buffOwner.stats.MonstersKilled > buffOwnerLastMonstersSlain)
            {
                buffOwnerLastMonstersSlain = buffOwner.stats.MonstersKilled;
                Game1.buffsDisplay.addOtherBuff(new Buff(20));
                buffOwner.currentLocation.playSound("warrior");
            }
        }

        public override void RemoveAndDisposeCompanionBuff()
        {
            base.RemoveAndDisposeCompanionBuff();
            OnRemoveAction();
        }

        protected void OnAddAction()
        {
            Game1.player.attackIncreaseModifier += 0.2f;
        }

        protected void OnRemoveAction()
        {
            Game1.player.attackIncreaseModifier -= 0.2f;
        }
    }
}