/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace Survivalistic.Framework.Common
{
    public class Buffs
    {
        public const string FullnessBuffName = "Fullness";

        public const string HydratedBuffName = "Hydrated";

        public const string HungerBuffName = "Hunger";

        public const string ThirstyBuffName = "Thirsty";

        public const string FaintingBuffName = "Fainting";

        public static void SetBuff(string name)
        {
            switch (name)
            {
                case FullnessBuffName:
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

                    break;

                case HydratedBuffName:
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

                    break;

                case HungerBuffName:
                    Buff hunger_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Hunger");

                    if (hunger_buff == null)
                    {
                        hunger_buff = new Buff(ModEntry.instance.Helper.Translation.Get("hunger-warning"), 0, "SURV_Hunger", 6)
                        {
                            displaySource = ModEntry.instance.Helper.Translation.Get("hunger-source")
                        };

                        Game1.buffsDisplay.addOtherBuff(hunger_buff);
                    }
                    hunger_buff.millisecondsDuration = 10 * 1000;

                    break;

                case ThirstyBuffName:
                    Buff thirsty_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Thirsty");

                    if (thirsty_buff == null)
                    {
                        thirsty_buff = new Buff(ModEntry.instance.Helper.Translation.Get("thirsty-warning"), 0, "SURV_Thirsty", 7)
                        {
                            displaySource = ModEntry.instance.Helper.Translation.Get("thirsty-source")
                        };

                        Game1.buffsDisplay.addOtherBuff(thirsty_buff);
                    }
                    thirsty_buff.millisecondsDuration = 10 * 1000;

                    break;

                case FaintingBuffName:
                    Buff fainting_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Fainting");

                    if (fainting_buff == null)
                    {
                        fainting_buff = new Buff(ModEntry.instance.Helper.Translation.Get("pass-out"), 0, "SURV_Fainting", 26)
                        {
                            displaySource = ModEntry.instance.Helper.Translation.Get("pass-out-source")
                        };

                        Game1.buffsDisplay.addOtherBuff(fainting_buff);
                    }
                    fainting_buff.millisecondsDuration = 10 * 1000;

                    break;

                default:
                    ModEntry.instance.Monitor.Log($"Unknown buff name send to 'SetBuff' function. Value: {name}.", LogLevel.Error);

                    break;
            }
        }

        public static void RemoveBuff(string name)
        {
            switch (name)
            {
                case FullnessBuffName:
                    Buff fullness_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Fullness");

                    if (fullness_buff != null)
                    {
                        fullness_buff.millisecondsDuration = 0;
                    }

                    break;

                case HydratedBuffName:
                    Buff hydrated_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Hydrated");

                    if (hydrated_buff != null)
                    {
                        hydrated_buff.millisecondsDuration = 0;
                    }

                    break;

                case HungerBuffName:
                    Buff hunger_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Hunger");

                    if (hunger_buff != null)
                    {
                        hunger_buff.millisecondsDuration = 0;
                    }

                    break;

                case ThirstyBuffName:
                    Buff thirsty_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Thirsty");

                    if (thirsty_buff != null)
                    {
                        thirsty_buff.millisecondsDuration = 0;
                    }

                    break;

                case FaintingBuffName:
                    Buff fainting_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "SURV_Fainting");

                    if (fainting_buff != null)
                    {
                        fainting_buff.millisecondsDuration = 0;
                    }

                    break;

                default:
                    ModEntry.instance.Monitor.Log($"Unknown value send to 'RemoveBuff' function. Value: {name}.", LogLevel.Error);

                    break;
            }
        }
    }
}
