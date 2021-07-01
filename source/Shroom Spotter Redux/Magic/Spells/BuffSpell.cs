/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using Magic.Schools;
using Netcode;
using SpaceCore;
using StardewValley;

namespace Magic.Spells
{
    public class BuffSpell : Spell
    {
        public BuffSpell() : base(SchoolId.Life, "buff")
        {
        }

        public override bool canCast(Farmer player, int level)
        {
            if (player == Game1.player)
            {
                foreach (var buff in Game1.buffsDisplay.otherBuffs)
                {
                    if (buff.source == "spell:life:buff")
                        return false;
                }
            }

            return base.canCast(player, level);
        }

        public override int getManaCost(Farmer player, int level)
        {
            return 25;
        }

        public override IActiveEffect onCast(Farmer player, int level, int targetX, int targetY)
        {
            if (player != Game1.player)
                return null;

            foreach ( var buff in Game1.buffsDisplay.otherBuffs )
            {
                if (buff.source == "spell:life:buff")
                    return null;
            }

            //Game1.buffsDisplay.clearAllBuffs();
            Mod.instance.Helper.Reflection.GetField<NetArray<int, NetInt>>(Game1.player, "appliedBuffs").GetValue().Clear();
            Game1.player.attack = 0;

            int l = level + 1;
            int farm = l, fish = l, mine = l, luck = l, forage = l, def = 0 /*1*/, atk = 2;
            if (l == 2)
            {
                //def = 3;
                atk = 5;
            }
            else if (l == 3)
            {
                //def = 5;
                atk = 10;
            }

            Game1.buffsDisplay.addOtherBuff(new Buff(farm, fish, mine, 0, luck, forage, 0, 0, 0, 0, def, atk, 60 + level * 120, "spell:life:buff", "Buff (spell)"));
            player.AddCustomSkillExperience(Magic.Skill,10);
            return null;
        }
    }
}
