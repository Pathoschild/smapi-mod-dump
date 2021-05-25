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
    abstract class CustomBuff : Buff, ICustomBuff
    {
        public CustomBuff(string description, string source, int durationMinutes)
            : base(-1)
        {
            this.description = description;
            this.source = source;
            this.millisecondsDuration = durationMinutes * 1000;
            this.totalMillisecondsDuration = durationMinutes * 1000;
            this.displaySource = source;
        }

        public abstract void ApplyCustomEffect();

        public abstract List<ClickableTextureComponent> GetCustomBuffIcons();

        public abstract void RemoveCustomEffect(bool clearingAllBuffs);        
    }
}
