/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace StardewSurvivalProject.source.effects
{
    public class EffectManager
    {
        public static int burnEffectIndex = 36;
        public static int starvationEffectIndex = 37;
        public static int hypothermiaEffectIndex = 38;
        public static int frostbiteEffectIndex = 39;
        public static int heatstrokeEffectIndex = 40;
        public static int dehydrationEffectIndex = 41;
        public static int feverEffectIndex = 42;
        public static int stomachacheEffectIndex = 43;
        public static int thirstEffectIndex = 44;
        public static int hungerEffectIndex = 45;
        public static int wellFedEffectIndex = 46;
        public static int refreshingEffectIndex = 47;

        //dictionary include effect index as key, a string int pair value for description and effect duration respectively
        public static Dictionary<int, KeyValuePair<string, int>> effectDescDictionary = new Dictionary<int, KeyValuePair<string, int>>();

        public static void initialize(int appendRow = 3)
        {
            effectDescDictionary.Clear();

            burnEffectIndex = appendRow * 12 + 0;
            starvationEffectIndex = appendRow * 12 + 1;
            hypothermiaEffectIndex = appendRow * 12 + 2;
            frostbiteEffectIndex = appendRow * 12 + 3;
            heatstrokeEffectIndex = appendRow * 12 + 4;
            dehydrationEffectIndex = appendRow * 12 + 5;
            feverEffectIndex = appendRow * 12 + 6;
            stomachacheEffectIndex = appendRow * 12 + 7;
            thirstEffectIndex = appendRow * 12 + 8;
            hungerEffectIndex = appendRow * 12 + 9;
            wellFedEffectIndex = appendRow * 12 + 10;
            refreshingEffectIndex = appendRow * 12 + 11;

            effectDescDictionary.Add(burnEffectIndex, new KeyValuePair<string, int>("Holy crap you are on fire! Get away from the heat source NOW", 1000));
            effectDescDictionary.Add(starvationEffectIndex, new KeyValuePair<string, int>("You're starving. Please eat something...", 1000));
            effectDescDictionary.Add(hypothermiaEffectIndex, new KeyValuePair<string, int>("Your skin is getting colder. Please seek a shelter and a campfire.", 1000));
            effectDescDictionary.Add(frostbiteEffectIndex, new KeyValuePair<string, int>("Your mind is getting numb. I hope your shelter is nearby...", 1000));
            effectDescDictionary.Add(heatstrokeEffectIndex, new KeyValuePair<string, int>("The heat is so bad, you begin to sweat non-stop", 1000));
            effectDescDictionary.Add(dehydrationEffectIndex, new KeyValuePair<string, int>("You are as dry as a kindle. Please get yourself something to drink", 1000));
            effectDescDictionary.Add(feverEffectIndex, new KeyValuePair<string, int>("Someday you just feeling sick. You'd better not doing something to heavy", 480000));
            effectDescDictionary.Add(stomachacheEffectIndex, new KeyValuePair<string, int>("Your gut felt some pain, maybe cook your food next time", 10000));
            effectDescDictionary.Add(thirstEffectIndex, new KeyValuePair<string, int>("Your throat is dried, it's begging for some liquid", 1000));
            effectDescDictionary.Add(hungerEffectIndex, new KeyValuePair<string, int>("Your stomach is growling, better get something to eat", 1000));
            effectDescDictionary.Add(wellFedEffectIndex, new KeyValuePair<string, int>("You feel fullfilled, life's good", 1000));
            effectDescDictionary.Add(refreshingEffectIndex, new KeyValuePair<string, int>("Lovely temperature, make you so ready for work", 1000));
        }

        public static void addEffect(int effectIndex)
        {
            Buff effect = null;
            if (effectIndex == hypothermiaEffectIndex)
                Game1.buffsDisplay.addOtherBuff(effect = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, -2, 0, 0, 0, "", ""));
            else if (effectIndex == hungerEffectIndex)
                Game1.buffsDisplay.addOtherBuff(effect = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, -1, 0, "", ""));
            else if (effectIndex == thirstEffectIndex)
                Game1.buffsDisplay.addOtherBuff(effect = new Buff(0, 0, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", ""));
            else if (effectIndex == wellFedEffectIndex)
                Game1.buffsDisplay.addOtherBuff(effect = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, "", ""));
            else if (effectIndex == refreshingEffectIndex)
                Game1.buffsDisplay.addOtherBuff(effect = new Buff(0, 0, 0, 0, 0, 0, 0, 20, 1, 1, 0, 0, 0, "", ""));
            else
                Game1.buffsDisplay.addOtherBuff(effect = new Buff(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", ""));

            effect.which = effectIndex;
            effect.sheetIndex = effectIndex;
            KeyValuePair<string, int> effectExtraInfo = new KeyValuePair<string, int>("Unknown Effect", 0);
            bool res = effectDescDictionary.TryGetValue(effectIndex, out effectExtraInfo);

            effect.millisecondsDuration = effectExtraInfo.Value;
            effect.description = effectExtraInfo.Key;
        }

        public static void applyEffect(int effectIndex)
        {
            Buff effect = Game1.buffsDisplay.otherBuffs.FirstOrDefault(e => e.which == effectIndex);
            if (effect != null)
            {
                renewEffect(effect);
            }
            else
            {
                addEffect(effectIndex);
            }
        }

        public static void renewEffect(Buff effect)
        {
            KeyValuePair<string, int> effectExtraInfo = new KeyValuePair<string, int>("Unknown Effect", 0);
            bool res = effectDescDictionary.TryGetValue(effect.which, out effectExtraInfo);
            effect.millisecondsDuration = effectExtraInfo.Value;
        }
    }
}
