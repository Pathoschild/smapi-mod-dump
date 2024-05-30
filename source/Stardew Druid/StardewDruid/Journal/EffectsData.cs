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
using StardewDruid.Cast.Mists;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.Objects;
using System;
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
            Effect herbalism = new()
            {
                title = "Herbalism",
                icon = IconData.displays.weald,
                description = "The offerings of the valley sustain me.",
                instruction = "Learn how to brew potions and tonics at the herbalism bench in the secluded farm grove. There are three lines of potion to brew, each with their own ingredients and special effects.",
                details = new()
                {
                    "Ligna: special ingredients include roots, seeds and roughage, and higher potency will provide regeneration and better outcomes from rites.",
                    "Impes: special ingredients include fungi, and higher potency will raise special attack and critical hits.",
                    "Celeri: special ingredients include fish oil and seaweeds, and higher potency will raise movement speed, cast speed and also lower cooldowns."
                }
            };

            Effect clear = new()
            {
                title = "Clearance",
                icon = IconData.displays.weald,
                description = "When I inherited the farm from my Grandfather, it had become almost completely overrun with thicket. The Effigy has shown me how to clear way for new growth.",
                instruction = "Rite of the Weald will explode nearby weeds, twigs and artifact spots.",
                details = new()
                {   
                    "Mastery: Will also explode gem nodes and breakable containers.",
                    "Stamina Cost: 2 per explosion",
                    "Scaling: 2 + Druid Level tile explosion radius",
                    
                }
            };

            Effect attunement = new()
            {
                title = "Attunement",
                icon = IconData.displays.weald,
                description = "I can dedicate even the most rudimentary farm implement to the work of the Two Kings.",
                instruction = "The energies of the weald at the grove standing stones can attune additional melee weapons and farm tools to the Rite of the Weald.",
                details = new()
                {
                    "Farm tools: Pickaxe, Axe, Hoe, Scythe, Watering Can, Fishing Rods, Pan",
                    "Melee Weapons: Any that aren't reserved for specific rites",
                    "Note: More patrons will unlock throughout the mod progression with additional attunement choices"

                }
            };

            effects[QuestHandle.wealdOne] = new() { ritesOfTheDruids, herbalism,  clear, attunement, };

            Effect wildbounty = new()
            {
                title = "Wild Bounty",
                icon = IconData.displays.weald,
                description = "The Effigy has shown me how to gather the bounty of the wild.",
                instruction = "Rite of the Weald will extract foragables from large bushes, timber and moss from trees, fibre from grass and small fish from water.",
                details = new()
                {
                    "Mastery: Wild seeds are gathered from grass.",
                    "Target: Each cast targets a 9x9 square tile grid near farmer, each 9sqt can be targetted once a day",
                    "Extract(Fibre,Timber): 1 + RNG based on DruidLevel and professions",
                    "Extract(Bush): seasonal berry, with chance for mushroom",
                    "Extract(Water): low tier fish, with chance for high quality",
                    "Cost (Grass,Tree): 1 stamina per extract",
                    "Cost (Bush): 6 stamina per extract, halved at Foraging Level 6",
                    "Cost (Water): 8 stamina per extract, halved at Fishing Level 6",
                    "Experience (Bush): 4 to Foraging per extract",
                    "Experience (Water): 8 to Fishing per extract",
                    
                }

            };

            Effect community = new()
            {
                title = "Community",
                icon = IconData.displays.weald,
                description = "The druids of antiquity played an important role in civil matters, as ceremonial leaders, mediators and physicians. The Rite of the Weald appears to have a positive effect on those who witness the rite.",
                instruction = "Rite of the Weald: Bounty, when cast near NPCs and Farm animals, will trigger daily dialogue counters and generate unique reactions.",
                details = new()
                {
                    "Note: Disabled for villagers who have not been introduced to farmer and registered in the in-game friendship directory",
                    "Villagers: Raises friendship by 25, adds custom dialogue, ticks 'talked to today' value",
                    "Animals: Applies petting to animal",
                }
            };

            effects[QuestHandle.wealdTwo] = new() { wildbounty, community, };

            Effect wildgrowth = new()
            {
                title = "Wild Growth",
                icon = IconData.displays.weald,
                description = "The Druid fills the barren spaces with life. Seeds, sewn everywhere, freely, ready to sprout into tomorrow's wilderness.",
                instruction = "Rite of the Weald: Bounty will sprout trees, grass, seasonal forage and flowers in empty spaces.",
                details = new()
                {
                    "Mastery: Chance to sprout wild flowers.",
                    "Cost: 6 stamina per forageable spawned, halved at Foraging Level 6",
                    
                }
            };

            effects[QuestHandle.wealdThree] = new() { wildgrowth };

            Effect cultivate = new()
            {
                title = "Cultivate",
                icon = IconData.displays.weald,
                description = "I have learned that the Farmer and the Druid share the same vision for a prosperous and well fed community, and so the wild seed is domesticated.",
                instruction = "Farm/Greenhouse only. Cast and hold Rite of the Weald for 3-4 seconds while standing still, until the essence fills the rite indicator. When released, the cultivation effect will radiate in increasing magnitude from your position.",
                details = new()
                {
                    "Mastery: Wild seeds have a chance to convert into quality crops.",
                    "Effect: Seasonal wild seeds sewn into tilled dirt are converted into domestic crops.",
                    "Effect: Fertilise and update the growth cycle of all crop seeds and tree seeds",
                    "Effect: Progress the growth rate of maturing fruit trees and tea bushes by one day (once per day)",
                    "Note: Also triggers Rite of the Weald: Community effect",
                    "Cost: 2 stamina, halved at Farming Level 6",
                    

                }

            };

            effects[QuestHandle.wealdFour] = new() { cultivate };

            Effect rockfall = new()
            {
                title = "Rockfall",
                icon = IconData.displays.weald,
                description = "The power of the two Kings resonates through the deep earth.",
                instruction = "Cast in mineshafts to cause stones to fall from the ceiling.",
                details = new()
                {
                    "Mastery: Falling rocks inflict small damage to monsters within 3 tiles.",
                    "Cost: 2 stamina",
                    "Debris (Stone): 1 + RNG based on DruidLevel and professions",
                    
                }

            };

            Effect sap = new()
            {
                title = "Charge: Sap",
                icon = IconData.displays.weald,
                description = "I am energised with every blow I inflict against the enemies of the valley.",
                instruction = "While charged, melee attacks against monsters will replenish stamina.",
                details = new()
                {
                    "Activate: Press the special button while casting Rite of the Weald to charge.",
                    "Cooldown: 500ms (half a second)",
                    "Drain: 4 + DruidLevel X 2"
                }

            };

            effects[QuestHandle.wealdFive] = new() { rockfall, sap,};


            // ====================================================================
            // Mists effects

            Effect cursorTargetting = new()
            {
                title = "Cursor Targetting",
                icon = IconData.displays.mists,
                description = "The mists gather in front of me.",
                instruction = "The Rite of Mists uses directional and cursor based targetting to effect a point ahead of or away from the player, as opposed to centered-on-player targetting, so the direction and position of the farmer and/or mouse cursor is important to get precise hits.",

            };

            Effect sunder = new()
            {
                title = "Sunder",
                icon = IconData.displays.mists,
                description = "The Lady Beyond the Shore has granted me the power to remove common obstacles. Now I can be her representative to the further, wilder spaces of the valley.",
                instruction = "Rite of Mists will instantly destroy boulders, large stumps and hollow logs.",
                details = new()
                {
                    "Mastery: Increases the resource acquired from obstacle destruction",
                    "Cost: 32 stamina - (Player Foraging level x 3)",
                }
            };

            effects[QuestHandle.mistsOne] = new() { cursorTargetting,sunder };

            Effect campfire = new()
            {
                title = "Campfires",
                icon = IconData.displays.mists,
                description = "Druids were masters of the hearth and bonfire, often a central point for festive occasions. The raw energy from Rite of Mists is precise enough to spark a controlled flame.",
                instruction = "Strike crafted campfires and firepits throughout the valley to create cookouts.",
                details = new()
                {
                    "Mastery: Unlocks up to 16 base recipes.",
                    "Limit: One instance in one location per day",
                    "Note: Crafted campfires are consumed",
                    "Firepits include the beach firepit, Linus's firepit, and a secret spot on the cliffs east of the poke' mouse house.",
                }
            };

            Effect totemShrines = new()
            {
                title = "Warp Totems",
                icon = IconData.displays.mists,
                description = "The old circle of druids left traces of their presence. Their work is visible in the delipidated structures and grime covered statues of the valley. Some residual power remains.",
                instruction = "Strike warp statues once a day to extract totems. Includes Farm, Beach, Mountain and Desert statues.",
                details = new()
                {
                    "Random chance for extra totems",
                }
            };


            Effect artifice = new()
            {
                title = "Artifice",
                icon = IconData.displays.mists,
                description = "The Lady is fascinated by the industriousness of humanity, and incorporating common farm constructs into the Rite of Mists produces interesting results",
                instruction = "Strike scarecrows to produce a radial crop watering effect. Radius increases after certain quest milestones. Strike lightning rods and mushroom logs once a day to charge it.",
                details = new()
                {
                    "Radius (scarecrow): 2 tiles + Druid Power Level",
                    "Cooldown: Each target can be charged once a day",
                    "Limit: 5 each of rods and logs, and unlimited for scarecrows.",
                    "Cost (scarecrow): 32 stamina - (Player Farming level x 3)",

                }
            };

            effects[QuestHandle.mistsTwo] = new() { campfire, totemShrines, artifice,};

            Effect rodMaster = new()
            {
                title = "Fishspots",
                icon = IconData.displays.mists,
                description = "The denizens of the deep water serve the Lady Beyond the Shore. Rarer, bigger fish will gather where the Rite of Mists strikes the open water.",
                instruction = "Strike water at least three tiles away from water edge with Rite of Mists to produce a fishing-spot that yields rare species of fish. Cast the fishing line and wait for the fish mini-game to trigger automatically, then reel the fish in.",
                details = new()
                {
                    "Mastery: Rarer fish become available to catch.",
                    "Trigger: about 3 seconds, reduced to approxiamately 1 second if bait and tackle are attached",
                    "Sidequest: creating and using fishspots in different parts of the valley may yield special relics that can be inspected by the servants of the Lady (the atoll statue)"
                }
            };

            effects[QuestHandle.mistsThree] = new() { rodMaster, };

            Effect smite = new()
            {
                title = "Smite",
                icon = IconData.displays.mists,
                description = "I now have an answer for some of the more terrifying threats I've encountered in my adventures. Bolts of lightning strike at my foes.",
                instruction = "Expend stamina to hit enemies with bolts of lightning.",
                details = new()
                {
                    "Mastery: Critical hit chance greatly increased",
                }
            };

            Effect veilCharge = new()
            {
                title = "Veil of Mist",
                icon = IconData.displays.mists,
                description = "The discharge of lightning has the strange effect of drawing in mist that's imbued with the Lady's benevolence, and I feel myself invigorated when immersed in it.",
                instruction = "While charged, melee attacks against monsters will create a veil of mist with healing properties.",
                details = new()
                {
                    "Activate: Press the special button while casting Rite of Mists to charge.",
                    "Cooldown: 1 second",
                    "Heal: up to 10% of max health",
                }
            };

            effects[QuestHandle.mistsFour] = new() { smite, veilCharge, };

            Effect summonWisps = new()
            {
                title = "Wisps",
                icon = IconData.displays.mists,
                description = "The Effigy continued to practice and modify the techniques taught to him by his creators. One technique that developed from his independent scholarship of the otherworld is the summoning of the valley wisps into a temporary material form.",
                instruction = "Channel (press and hold) Rite of the Mists to summon wisps. Wisps cast a smaller version of Smite that stuns nearby enemies.",
                details = new()
                {
                    "Number: One wisp per 12x12 tile square on valid ground around the player, going clockwise, for every 1 second held once triggered."
                }
            };

            Effect summonEffigy = new()
            {
                title = "Gardener of the Circle",
                icon = IconData.displays.effigy,
                description = "It is time for the Effigy to frolic the furrows and fields of former friend first farmer's home.",
                instruction = "Approach the Effigy and select (adventure) from the dialogue menu to give him instructions.",
                details = new()
                {
                    "The 'Enameled Star Crest' can be used in the relics menu to summon the Effigy.",
                    "The Effigy can accompany you to other locations, and will perform it's own attacks against nearby monsters.",
                    "The Effigy can be invited to roam the farm, and will perform Weald: Cultivate and Mists: Scarecrows where scarecrows have been placed. ",
                }
            };

            /*Effect ritualSummon = new()
            {
                title = "Ritual of Summoning",
                icon = IconData.displays.mists,
                description = "The druids would attempt to commune with spirits at times when the barrier between the material and ethereal world had waned. The Lady's power can punch right through the veil.",
                instruction = "Perform a ritual of summoning by channeling (press and hold) Rite of the Mists in specific locations. Fight off the monsters that step through the veil to receive a reward.",
                details = new()
                {
                    "Locations: Druid Grove, Druid Atoll (far eastern beach), and other Druid specific sites.",
                    "Trigger: Stand within the summoning circle and hold Rite of the Mists to cast",
                    "Difficulty: The longer the summoning rite is held (up to five levels, capped by Druid level) the stronger the summoning. " +
                    "Rounds: The number of rounds is determined by summoning strength and Druid level up to ten",
                }
            };*/

            //effects[QuestHandle.questEffigy] = new() { wispSummon, ritualSummon, };

            effects[QuestHandle.questEffigy] = new() { summonWisps, summonEffigy };

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
