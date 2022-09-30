/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using StardewValley;
using StardewValley.Monsters;

namespace StardewRoguelike.Enchantments
{
    public class WeaponStatTrack : BaseEnchantment
    {
        protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
        {
            ModEntry.Stats.DamageDealt += amount / 2;
        }

        protected override void _OnMonsterSlay(Monster monster, GameLocation location, Farmer who)
        {
            ModEntry.Stats.MonstersKilled++;
        }
    }
}
