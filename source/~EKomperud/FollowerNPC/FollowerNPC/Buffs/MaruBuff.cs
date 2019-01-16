using System;
using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FollowerNPC.Buffs
{
    class MaruBuff : CompanionBuff
    {
        protected int currentProstheticIndex;
        protected Buff prosthetic;
        protected Buff[] prosthetics;

        List<Buff> otherBuffs;

        public MaruBuff(Farmer farmer, NPC npc, CompanionsManager manager) : base(farmer, npc, manager)
        {
            prosthetics = new Buff[6];
            prosthetics[0] = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 30, "", "");
            prosthetics[0].description = "Hypersonic Locomo-tronic Leg Braces! +1 Speed"+
                                         Environment.NewLine+
                                         "Source: Maru";
            prosthetics[1] = new Buff(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            prosthetics[1].description = "SPF 200 Farmer's Tan Skin Shield! +1 Farming" +
                                         Environment.NewLine +
                                         "Source: Maru";
            prosthetics[2] = new Buff(0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            prosthetics[2].description = "Tension-Sensing Reflex Enhancer! +1 Fishing" +
                                         Environment.NewLine +
                                         "Source: Maru";
            prosthetics[3] = new Buff(0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            prosthetics[3].description = "Black-hole Pickaxe Grippers! +1 Mining" +
                                         Environment.NewLine +
                                         "Source: Maru";
            prosthetics[4] = new Buff(0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 30, "", "");
            prosthetics[4].description = "Quantum-Computational Reality Modulator? +1 Luck" +
                                         Environment.NewLine +
                                         "Source: Maru";
            prosthetics[5] = new Buff(0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 30, "", "");
            prosthetics[5].description = "Olfactory-Occipital-Undergrowth Unionizer! +1 Foraging" +
                                         Environment.NewLine +
                                         "Source: Maru";
            
            currentProstheticIndex = 0;
            prosthetic = prosthetics[0];
            prosthetic.millisecondsDuration = 1200000;
            otherBuffs = Game1.buffsDisplay.otherBuffs;
            Game1.buffsDisplay.addOtherBuff(prosthetic);
            foreach (Buff p in prosthetics)
                p.which = -69;

            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Maru's been working on some prosthetics that can enhance various abilities!"+
                               Environment.NewLine+
                               "You gain +1 to any stat. Use the 'G' key to cycle your current prosthetic.";

            ModEntry.modHelper.Events.Input.ButtonReleased += Input_ButtonReleased;
        }

        public override void RemoveAndDisposeCompanionBuff()
        {
            base.RemoveAndDisposeCompanionBuff();
            ModEntry.modHelper.Events.Input.ButtonReleased += Input_ButtonReleased;

            for (int i = 0; i < otherBuffs.Count; i++)
            {
                if (otherBuffs[i] != null && otherBuffs[i].which == -69)
                {
                    otherBuffs[i].removeBuff();
                    otherBuffs.RemoveAt(i);
                    i--;
                }
            }
            Game1.buffsDisplay.syncIcons();
        }

        private void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (e.Button == Microsoft.Xna.Framework.Input.Keys.G.ToSButton())
            {
                for (int i = 0; i < otherBuffs.Count; i++)
                {
                    if (otherBuffs[i] != null && otherBuffs[i].which == -69)
                    {
                        otherBuffs[i].removeBuff();
                        otherBuffs.RemoveAt(i);
                        Game1.buffsDisplay.syncIcons();
                    }
                }

                currentProstheticIndex = ++currentProstheticIndex > 5 ? 0 : currentProstheticIndex;
                prosthetic = prosthetics[currentProstheticIndex];
                prosthetic.millisecondsDuration = 1200000;
                prosthetic.which = -1;
                Game1.buffsDisplay.addOtherBuff(prosthetic);
                prosthetic.which = -69;
            }
        }
    }
}
