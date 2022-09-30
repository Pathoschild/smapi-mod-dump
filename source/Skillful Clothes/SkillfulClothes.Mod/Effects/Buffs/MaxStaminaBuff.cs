/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SkillfulClothes
**
*************************************************/

using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillfulClothes.Effects.Buffs
{
    /// <summary>
    /// Extends the base game's stamina buff so that it can be added as "other buff" and not only
    /// as drink or food (when addign the normal stamian buff as other buff it won't get removed correctly when day ends)
    /// </summary>
    class MaxStaminaBuff : Buff, ICustomBuff
    {
        int amount;

        public MaxStaminaBuff(int amount, int durationMinutes, string source)
            : base(0, 0, 0, 0, 0, 0, 0, amount, 0, 0, 0, 0, durationMinutes, source, source)
        {
            this.amount = amount;
        }

        public void ApplyCustomEffect()
        {
            if (Game1.player.stamina == Game1.player.maxStamina.Value - amount)
            {
                Game1.player.stamina = Game1.player.maxStamina.Value;
            }
        }

        public List<ClickableTextureComponent> GetCustomBuffIcons()
        {
            // icon will be handled by the base game
            return null;
        }

        public void RemoveCustomEffect(bool clearingAllBuffs)
        {
            if (clearingAllBuffs)
            {
                Game1.player.MaxStamina = Math.Max(1, Game1.player.MaxStamina - amount);
                Game1.player.stamina = Math.Min(Game1.player.stamina, Game1.player.MaxStamina);
            }
        }
    }
}
