using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace FollowerNPC.Buffs
{
    class SamBuff : CompanionBuff
    {
        public SamBuff(Farmer farmer, NPC npc, CompanionsManager manager) : base(farmer, npc, manager)
        {
            buff = new Buff(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 30, "", "");
            buff.description = "Sam likes to live fast (but hopefully he won't die TOO young)."+
                               System.Environment.NewLine+
                               "You gain +2 speed while in his stead.";

            statBuffs = new Buff[1];
            statBuffs[0] = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 30, "", "");
            statBuffs[0].description = "+2 Speed" +
                                       System.Environment.NewLine +
                                       "Source: Sam";
        }

        public override void SpecialAction()
        {
            base.SpecialAction();
            Game1.activeClickableMenu = new ChooseFromListMenu(Game1.player.songsHeard.Distinct<string>().ToList<string>(), new ChooseFromListMenu.actionOnChoosingListOption(ChooseFromListMenu.playSongAction), true);
        }
    }
}
