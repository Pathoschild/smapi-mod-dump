/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Let-Me-Rest
**
*************************************************/

using StardewValley;
using StardewModdingAPI;

namespace LetMeRest.Framework.Lists
{
    public class Buffs
    {
        public static void SetBuff(string buff)
        {
            switch (buff)
            {
                case "Restoring":
                    Buff restoring_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_RestoringStamina");
                    if (restoring_buff == null)
                    {
                        restoring_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.restoring.description"), 0, "LMR_RestoringStamina", 16);
                        restoring_buff.displaySource = ModEntry.instance.Helper.Translation.Get("buff.restoring.source");
                        Game1.buffsDisplay.addOtherBuff(restoring_buff);
                    }
                    restoring_buff.millisecondsDuration = 0;
                    break;

                case "Water":
                    Buff water_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Water");
                    if (water_buff == null)
                    {
                        water_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.water.description"), 0, "LMR_Water", 1);
                        water_buff.displaySource = ModEntry.instance.Helper.Translation.Get("buff.water.source");
                        Game1.buffsDisplay.addOtherBuff(water_buff);
                    }
                    water_buff.millisecondsDuration = 0;
                    break;

                case "Decoration":
                    Buff decoration_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Decoration");
                    if (decoration_buff == null)
                    {
                        decoration_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.decoration.description"), 0, "LMR_Decoration", 24);
                        decoration_buff.displaySource = ModEntry.instance.Helper.Translation.Get("buff.decoration.source");
                        Game1.buffsDisplay.addOtherBuff(decoration_buff);
                    }
                    decoration_buff.millisecondsDuration = 0;
                    break;

                case "Afraid":
                    Buff afraid_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Afraid");
                    if (afraid_buff == null)
                    {
                        afraid_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.afraid.description"), 0, "LMR_Afraid", 18);
                        afraid_buff.displaySource = ModEntry.instance.Helper.Translation.Get("buff.afraid.source");
                        Game1.buffsDisplay.addOtherBuff(afraid_buff);
                    }
                    afraid_buff.millisecondsDuration = 0;
                    break;

                case "Decoration2":
                    Buff decoration2_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Decoration2");
                    if (decoration2_buff == null)
                    {
                        decoration2_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.decoration2.description"), 0, "LMR_Decoration2", 24);
                        decoration2_buff.displaySource = ModEntry.instance.Helper.Translation.Get("buff.decoration2.source");
                        Game1.buffsDisplay.addOtherBuff(decoration2_buff);
                    }
                    decoration2_buff.millisecondsDuration = 0;
                    break;

                case "Calm":
                    Buff calm_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Calm");
                    if (calm_buff == null)
                    {
                        if (Context.IsMultiplayer && ModEntry.data.EnableBuffs)
                        {
                            calm_buff = new Buff(0, 0, 0, 0, 2, 0, 2, 15, 0, 0, 2, 2, 120 * 1000, "LMR_Calm", ModEntry.instance.Helper.Translation.Get("buff.calm.source"));
                            calm_buff.sheetIndex = 21;
                            calm_buff.description = ModEntry.instance.Helper.Translation.Get("buff.calm.description");
                        }
                        else
                        {
                            calm_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.calm.description"), 120 * 1000, "LMR_Calm", 21);
                            calm_buff.displaySource = ModEntry.instance.Helper.Translation.Get("buff.calm.source");
                        }
                        if (!Context.IsMultiplayer && ModEntry.config.EnableBuffs)
                        {
                            calm_buff = new Buff(0, 0, 0, 0, 2, 0, 2, 15, 0, 0, 2, 2, 120 * 1000, "LMR_Calm", ModEntry.instance.Helper.Translation.Get("buff.calm.source"));
                            calm_buff.sheetIndex = 21;
                            calm_buff.description = ModEntry.instance.Helper.Translation.Get("buff.calm.description");
                        }
                        else
                        {
                            calm_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.calm.description"), 120 * 1000, "LMR_Calm", 21);
                            calm_buff.displaySource = ModEntry.instance.Helper.Translation.Get("buff.calm.source");
                        }

                        Buff calm2_buff_ref = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Calm2");
                        if (calm2_buff_ref != null)
                        {
                            calm2_buff_ref.millisecondsDuration = 0;
                        }

                        Game1.buffsDisplay.addOtherBuff(calm_buff);
                    }
                    calm_buff.millisecondsDuration = 120 * 1000;
                    break;

                case "Calm2":
                    Buff calm2_buff = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Calm2");
                    if (calm2_buff == null)
                    {
                        if (Context.IsMultiplayer && ModEntry.data.EnableBuffs)
                        {
                            calm2_buff = new Buff(0, 0, 0, 0, 5, 0, 5, 35, 0, 0, 5, 5, 120 * 1000, "LMR_Calm2", ModEntry.instance.Helper.Translation.Get("buff.calm2.source"));
                            calm2_buff.sheetIndex = 21;
                            calm2_buff.description = ModEntry.instance.Helper.Translation.Get("buff.calm2.description");
                        }
                        else
                        {
                            calm2_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.calm2.description"), 120 * 1000, "LMR_Calm2", 21);
                            calm2_buff.displaySource = ModEntry.instance.Helper.Translation.Get("buff.calm2.source");
                        }
                        if (!Context.IsMultiplayer && ModEntry.config.EnableBuffs)
                        {
                            calm2_buff = new Buff(0, 0, 0, 0, 5, 0, 5, 35, 0, 0, 5, 5, 120 * 1000, "LMR_Calm2", ModEntry.instance.Helper.Translation.Get("buff.calm2.source"));
                            calm2_buff.sheetIndex = 21;
                            calm2_buff.description = ModEntry.instance.Helper.Translation.Get("buff.calm2.description");
                        }
                        else
                        {
                            calm2_buff = new Buff(ModEntry.instance.Helper.Translation.Get("buff.calm2.description"), 120 * 1000, "LMR_Calm2", 21);
                            calm2_buff.displaySource = ModEntry.instance.Helper.Translation.Get("buff.calm2.source");
                        }

                        Buff calm_buff_ref = Game1.buffsDisplay.otherBuffs.Find(i => i.source == "LMR_Calm");
                        if (calm_buff_ref != null)
                        {
                            calm_buff_ref.millisecondsDuration = 0;
                        }

                        Game1.buffsDisplay.addOtherBuff(calm2_buff);
                    }
                    calm2_buff.millisecondsDuration = 120 * 1000;
                    break;
            }
        }
    }
}
