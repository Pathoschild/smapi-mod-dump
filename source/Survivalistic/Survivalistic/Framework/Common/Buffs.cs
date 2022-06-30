/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using Survivalistic.Framework.Bars;
using Survivalistic.Framework.Databases;

namespace Survivalistic.Framework.Common
{
    public class Buffs
    {
        public static void SetBuff(string name)
        {
            if (name == "Fullness")
            {
                Buff fullness_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Fullness");
                if (fullness_buff == null)
                {
                    fullness_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.fullness.description"), 0, "SURV_Fullness", 28)
                    {
                        displaySource = ModEntry.instance.Helper.Translation.Get("buff.fullness.source")
                    };
                    Game1.buffsDisplay.addOtherBuff(fullness_buff);
                }
                fullness_buff.millisecondsDuration = 60 * 1000;
                ApplyFullness();
            }

            if (name == "Hydrated")
            {
                Buff hydrated_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Hydrated");
                if (hydrated_buff == null)
                {
                    hydrated_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.hydrated.description"), 0, "SURV_Hydrated", 19)
                    {
                        displaySource = ModEntry.instance.Helper.Translation.Get("buff.hydrated.source")
                    };
                    Game1.buffsDisplay.addOtherBuff(hydrated_buff);
                }
                hydrated_buff.millisecondsDuration = 30 * 1000;
                ApplyHydrated();
            }

            if (name == "Hunger")
            {
                Buff hunger_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Hunger");
                if (hunger_buff == null)
                {
                    hunger_buff = new Buff("You're hungry!", 0, "SURV_Hunger", 6)
                    {
                        displaySource = "Hunger"
                    };
                    Game1.buffsDisplay.addOtherBuff(hunger_buff);
                }
                hunger_buff.millisecondsDuration = 10 * 1000;
            }

            if (name == "Thirsty")
            {
                Buff thirsty_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Thirsty");
                if (thirsty_buff == null)
                {
                    thirsty_buff = new Buff("You're thirsty!", 0, "SURV_Thirsty", 7)
                    {
                        displaySource = "Thirsty"
                    };
                    Game1.buffsDisplay.addOtherBuff(thirsty_buff);
                }
                thirsty_buff.millisecondsDuration = 10 * 1000;
            }

            if (name == "Fainting")
            {
                Buff fainting_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Fainting");
                if (fainting_buff == null)
                {
                    fainting_buff = new Buff("You're passing out!", 0, "SURV_Fainting", 26)
                    {
                        displaySource = "Passing Out"
                    };
                    Game1.buffsDisplay.addOtherBuff(fainting_buff);
                }
                fainting_buff.millisecondsDuration = 10 * 1000;
            }
        }

        public static void RemoveBuff(string name)
        {
            if (name == "Fullness")
            {
                Buff fullness_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Fullness");
                if (fullness_buff != null)
                {
                    fullness_buff.millisecondsDuration = 0;
                }
            }

            if (name == "Hydrated")
            {
                Buff hydrated_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Hydrated");
                if (hydrated_buff != null)
                {
                    hydrated_buff.millisecondsDuration = 0;
                }
            }

            if (name == "Hunger")
            {
                Buff hunger_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Hunger");
                if (hunger_buff != null)
                {
                    hunger_buff.millisecondsDuration = 0;
                }
            }

            if (name == "Thirsty")
            {
                Buff thirsty_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Thirsty");
                if (thirsty_buff != null)
                {
                    thirsty_buff.millisecondsDuration = 0;
                }
            }

            if (name == "Fainting")
            {
                Buff fainting_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Fainting");
                if (fainting_buff != null)
                {
                    fainting_buff.millisecondsDuration = 0;
                }
            }
        }

        public static void ApplyFullness()
        {

        }

        public static void ApplyHydrated()
        {

        }
    }
}
