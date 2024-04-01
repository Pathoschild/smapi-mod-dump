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
using Force.DeepCloner;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.Locations;

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
        public static int sprintingEffectIndex = 48;

        //dictionary include effect index as key, a string int pair value for description and effect duration respectively
        public static Dictionary<int, CustomEffect> effectDictionary = new Dictionary<int, CustomEffect>();

        public static void initialize(Dictionary<string, Texture2D> effectIcons)
        {
            const int appendRow = 3;
            effectDictionary.Clear();

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
            sprintingEffectIndex = appendRow * 12 + 12;

            effectDictionary.Add(burnEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/burn",
                displayName: "Burn",
                description: "Holy crap you are on fire! Get away from the heat source NOW",
                iconTexture: effectIcons.GetValueSafe("Burn"),
                duration: 1_000
            ));

            // starvation effect, desc: You're starving. Please eat something...
            effectDictionary.Add(starvationEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/starvation",
                displayName: "Starvation",
                description: "You're starving. Please eat something...",
                iconTexture: effectIcons.GetValueSafe("Starvation"),
                duration: 1_000
            ));

            // hypothermia effect, desc: Your skin is getting colder. Please seek a shelter and a campfire.
            effectDictionary.Add(hypothermiaEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/hypothermia",
                displayName: "Hypothermia",
                description: "Your skin is getting colder. Please seek a shelter and a campfire.",
                iconTexture: effectIcons.GetValueSafe("Hypothermia"),
                duration: 1_000,
                effects: new BuffEffects
                {
                    Speed = { -2 }
                }
            ));

            // frostbite effect, desc: Your mind is getting numb. I hope your shelter is nearby...
            effectDictionary.Add(frostbiteEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/frostbite",
                displayName: "Frostbite",
                description: "Your mind is getting numb. I hope your shelter is nearby...",
                iconTexture: effectIcons.GetValueSafe("Frostbite"),
                duration: 1_000,
                effects: new BuffEffects
                {
                    Speed = { -2 }
                }
            ));
            
            // heatstroke effect, desc: The heat is so bad, you begin to sweat non-stop
            effectDictionary.Add(heatstrokeEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/heatstroke",
                displayName: "Heatstroke",
                description: "The heat is so bad, you begin to sweat non-stop",
                iconTexture: effectIcons.GetValueSafe("Heatstroke"),
                duration: 1_000
            ));

            // dehydration effect, desc: You are as dry as a kindle. Please get yourself something to drink
            effectDictionary.Add(dehydrationEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/dehydration",
                displayName: "Dehydration",
                description: "You are as dry as a kindle. Please get yourself something to drink",
                iconTexture: effectIcons.GetValueSafe("Dehydration"),
                duration: 1_000
            ));

            // fever effect, desc: Someday you just feeling sick. You'd better not doing something to heavy
            effectDictionary.Add(feverEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/fever",
                displayName: "Fever",
                description: "Someday you just feeling sick. You'd better not doing something to heavy",
                iconTexture: effectIcons.GetValueSafe("Fever"),
                isDebuff: true,
                duration: 4_800_000
            ));

            // stomachache effect, desc: Your gut felt some pain, maybe cook your food next time
            effectDictionary.Add(stomachacheEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/stomachache",
                displayName: "Stomachache",
                description: "Your gut felt some pain, maybe cook your food next time",
                iconTexture: effectIcons.GetValueSafe("Stomachache"),
                isDebuff: true,
                duration: 10_000
            ));

            // thirst effect, desc: Your throat is dried, it's begging for some liquid
            effectDictionary.Add(thirstEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/thirst",
                displayName: "Thirst",
                description: "Your throat is dried, it's begging for some liquid",
                iconTexture: effectIcons.GetValueSafe("Thirst"),
                duration: 1_000,
                effects: new BuffEffects
                {
                    MiningLevel = { -1 },
                    ForagingLevel = { -1 },
                }
            ));

            // hunger effect, desc: Your stomach is growling, better get something to eat
            effectDictionary.Add(hungerEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/hunger",
                displayName: "Hunger",
                description: "Your stomach is growling, better get something to eat",
                iconTexture: effectIcons.GetValueSafe("Hunger"),
                duration: 1_000,
                effects: new BuffEffects
                {
                    Attack = { -1 },
                    Defense = { -1 },
                }
            ));

            // well fed effect, desc: You feel fullfilled, life's good
            effectDictionary.Add(wellFedEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/wellfed",
                displayName: "Well Fed",
                description: "You feel fullfilled, life's good",
                iconTexture: effectIcons.GetValueSafe("WellFed"),
                duration: 1_000,
                effects: new BuffEffects
                {
                    Attack = { 1 },
                    Defense = { 1 },
                }
            ));

            // refreshing effect, desc: Lovely temperature, make you so ready for work
            effectDictionary.Add(refreshingEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/refreshing",
                displayName: "Refreshing",
                description: "Lovely temperature, make you so ready for work",
                iconTexture: effectIcons.GetValueSafe("Refreshing"),
                duration: 1_000,
                effects: new BuffEffects
                {
                    Speed = { 1 },
                    MagneticRadius = { 1 },
                    MaxStamina = { 20 }
                }
            ));

            effectDictionary.Add(sprintingEffectIndex, new CustomEffect(
                id: "neroyuki.rlvalley/sprinting",
                displayName: "Sprinting",
                description: "Zoooooommmmmmm",
                iconTexture: effectIcons.GetValueSafe("Sprinting"),
                duration: 100,
                effects: new BuffEffects
                {
                    Speed = { 2 }
                }
            ));    
        }

        public static void addEffect(int effectIndex)
        {
            var customEff = effectDictionary.GetValueSafe(effectIndex);
            Game1.player.applyBuff(new Buff(
                id: customEff.id,
                displayName: customEff.displayName,
                description: customEff.description,
                iconTexture: customEff.iconTexture,
                duration: customEff.duration,
                isDebuff: customEff.isDebuff,
                effects: customEff.effects
            ));
        }

        public static void applyEffect(int effectIndex)
        {
            addEffect(effectIndex);
        }

        //public static void renewEffect(Buff effect)
        //{
        //    KeyValuePair<string, int> effectExtraInfo = new KeyValuePair<string, int>("Unknown Effect", 0);
        //    bool res = effectDescDictionary.TryGetValue(effect.which, out effectExtraInfo);
        //    effect.millisecondsDuration = effectExtraInfo.Value;
        //}
    }
}
