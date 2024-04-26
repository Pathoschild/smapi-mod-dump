/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/


using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewValley.GameData.Characters;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace StardewDruid.Journal
{
    public static class EffectsData
    {

        public static Dictionary<string,List<Effect>> EffectList()
        {

            Dictionary<string, List<Effect>> effects = new();

            // ====================================================================
            // Weald effects

            Effect ritesOfTheDruids = new()
            {
                title = "Rites of the Druids",
                icon = IconData.displays.weald,
                description = "When I perform a rite once practiced by the valley druids, I feel the essence of the wild being drawn under me with each step.",
                instruction = "Press and/or hold the rite button to cast a rite. More and stronger effects will be unlocked further along in progress.",
                details = new()
                {
                    "Druid Level: Number of quests completed / 5",
                    "Indicator: The decorative rite circle, cursor and HUD-buff icon should indicate the currently selected rite, which can vary due to weapon, attunement, progress and patron served.",
                    "Otherworldly connection: Some of the game maps (mostly interiors) have effect restrictions that can be disabled in the config.",
                    "Hold: You can run and ride a horse while holding the rite button.",
                    "Grass: Performing a rite provides faster movement through grass.",
                }
            };
            Effect autoConsume = new()
            {
                title = "Auto Consume",
                icon = IconData.displays.weald,
                description = "The offerings of the valley sustain me.",
                instruction = "When enabled in the configuration, auto-consume will trigger when health or stamina falls below a certain threshold, and consume items from the inventory to provide a stamina/health boost and even a temporary speed buff.",
                details = new()
                {
                    "Roughage items: Sap, Tree seeds, Slime, Batwings, Red mushrooms.",
                    "Lunch items: SpringOnion, Snackbar, Mushrooms, Algae, Seaweed, Carrots, Sashimi, Salmonberry, Cheese, Salad, Tonic.",
                    "Coffee/Speed items: Cola, Tea Leaves, Tea, Coffee Bean, Coffee, Triple Espresso, Energy Tonic."
                }
            };
            Effect community = new()
            {
                title = "Community",
                icon = IconData.displays.weald,
                description = "The druids of antiquity played an important role in civil matters, as ceremonial leaders, mediators and physicians. The Rite of the Weald appears to have a positive effect on those who witness the rite.",
                instruction = "Cast Rite of the Weald near NPCs and Farm animals to trigger daily dialogue counters and generate unique reactions.",
                details = new()
                {
                    "Note: Disabled for villagers who have not been introduced to farmer and registered in the farmer friendship directory",
                    "Mastery: Small boost to friendship with villagers and farm animals who witness casts"
                }
            };
            Effect clear = new()
            {
                title = "Clearance",
                icon = IconData.displays.weald,
                description = "When I inherited the farm from my Grandfather, it had become almost completely overrun with thicket. The Effigy has shown me how to clear way for new growth.",
                instruction = "Rite of the Weald will explode nearby weeds and twigs.",
                details = new()
                {
                    "Stamina Cost: 2 per explosion",
                    "Scaling: 2 + Druid Level tile explosion radius"

                }
            };

            effects[QuestHandle.clearLesson] = new() { ritesOfTheDruids, autoConsume, community, clear };

            Effect extraction = new()
            {
                title = "Extraction",
                icon = IconData.displays.weald,
                description = "The Effigy has shown me how to gather the bounty of the wild.",
                instruction = "Rite of the Weald will extract foragables from large bushes, wood from trees, fibre from grass and small fish from water.",
                details = new()
                {
                    "Target: Each cast targets 1-3 6x6 square tile grids near farmer, each 6sqt can be targetted once a day",
                    "Debris (Timber): 1 + RNG based on DruidLevel + professions",
                    "Cost (Grass,Tree): Free",
                    "Cost (Bush): 6 stamina per extract, halved at Foraging Level 6",
                    "Cost (Water): 8 stamina per extract, halved at Fishing Level 6",
                    "Experience (Bush, Tree): 2 to Foraging",
                    "Experience (Water): 8 to Fishing per extract",
                    "Mastery: Wild seeds are gathered from grass."
                }

            };

            effects[QuestHandle.bushLesson] = new() { extraction, };

            Effect wildgrowth = new()
            {
                title = "Wildgrowth",
                icon = IconData.displays.weald,
                description = "Sprout trees, grass and seasonal forage in empty spaces.",
                instruction = "Rite of the Weald will explode nearby weeds and twigs.",
                details = new()
                {
                    "Target: Each cast targets 1-3 6x6 square tile grids near farmer, each 6sqt can be targetted once a day",
                    "Cost: 6 stamina per 6sqt, halved at Foraging Level 6",
                    "Mastery: Chance to sprout wild flowers."
                }
            };

            effects[QuestHandle.spawnLesson] = new() { wildgrowth };

            Effect cultivate = new()
            {
                title = "Cultivate",
                icon = IconData.displays.weald,
                description = "I have learned that the Farmer and the Druid share the same vision for a prosperous and well fed community, and so the wild seed is domesticated.",
                instruction = "Cast Rite of the Weald over seasonal wild seeds sewn into tilled dirt to convert them into domestic crops.",
                details = new()
                {
                    "Target: Cast effect radiates in increasing magnitude from the farmer.",
                    "Plus: Fertilise and update the growth cycle of all crop seeds and tree seeds",
                    "Plus: Progress the growth rate of maturing fruit trees and tea bushes by one day (once per day)",
                    "Cost: 2 stamina, halved at Farming Level 6",
                    "Mastery: Wild seeds have a chance to convert into quality crops.",

                }

            };

            effects[QuestHandle.cropLesson] = new() { cultivate };

            Effect rockfall = new()
            {
                title = "Rockfall",
                icon = IconData.displays.weald,
                description = "The power of the two Kings resonates through the deep earth.",
                instruction = "Cast in mineshafts to cause stones to fall from the ceiling.",
                details = new()
                {
                    "Cost: 1 stamina",
                    "Debris (Stone): 1 + RNG based on DruidLevel + professions",
                    "Mastery: Falling rocks damage monsters.",
                }

            };

            Effect drain = new()
            {
                title = "Drain",
                icon = IconData.displays.weald,
                description = "I am energised with every blow I inflict against the enemies of the valley.",
                instruction = "While charged, melee attacks against monsters will replenish stamina.",
                details = new()
                {
                    "Activate: Press the special button while the rite button is also pressed to charge.",
                    "Cooldown: 500ms (half a second)",
                    "Drain: DruidLevel * 3"
                }

            };

            effects[QuestHandle.rockLesson] = new() { rockfall, drain };

            return effects;

        }

    }
    public class Effect
    {

        // -----------------------------------------------
        // journal

        public string title;

        public IconData.displays icon = IconData.displays.none;

        public string description;

        public string instruction;

        public List<string> details;


    }

}
