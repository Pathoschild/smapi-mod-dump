/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;
using Magic.Framework.Schools;
using SpaceCore;
using StardewValley;

namespace Magic.Framework.Spells
{
    internal class LanternSpell : Spell
    {
        private readonly Func<long> GetNewId;

        public LanternSpell(Func<long> getNewId)
            : base(SchoolId.Nature, "lantern")
        {
            this.GetNewId = getNewId;
        }

        public override int GetManaCost(Farmer player, int level)
        {
            return level * 3;
        }

        public override IActiveEffect OnCast(Farmer player, int level, int targetX, int targetY)
        {
            if (player != Game1.player)
                return null;

            int power = level switch
            {
                1 => 8,
                2 => 16,
                _ => 4
            };

            player.currentLocation.sharedLights.Add(this.GetUnusedLightSourceId(player.currentLocation), new LightSource(1, Game1.player.position, power));
            player.AddCustomSkillExperience(Magic.Skill, level);

            return null;
        }

        private int GetUnusedLightSourceId(GameLocation location)
        {
            while (true)
            {
                int id = (int)this.GetNewId();
                if (!location.hasLightSource(id))
                    return id;
            }
        }
    }
}
