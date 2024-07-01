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
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using static StardewValley.Minigames.TargetGame;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Channels;

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
                    "Along your journey you will attain relics that will unlock higher levels of potency, up to level 5",
                    "Ligna: special ingredients include roots, seeds and roughage, and higher potency will increase the damage and success-rate (better chance based outcomes) of rites.",
                    "Impes: special ingredients include tubers, fungi and spices, and higher potency will strengthen charge effects and critical hits.",
                    "Celeri: special ingredients include fish oil and seaweeds, and higher potency will raise movement speed, cast speed and lower charge effect cooldowns."
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
                instruction = "Farm/Greenhouse only. Cast and hold Rite of the Weald for 2-3 seconds while standing still, until the essence fills the rite indicator. When released, the cultivation effect will radiate in increasing magnitude from your position.",
                details = new()
                {
                    "Mastery: Wild seeds have a chance to convert into quality crops.",
                    "Effect: Seasonal wild seeds sewn into tilled dirt are converted into domestic crops.",
                    "Effect: Fertilise and update the growth cycle of all crop seeds and tree seeds",
                    "Effect: Progress the growth rate of maturing fruit trees and tea bushes by one day (once per day)",
                    "Note: Also triggers Rite of the Weald: Community effect",
                    "Cost: 2 stamina, halved at Farming Level 6",
                    "Crop effect can be further adjusted in the configuration options",
                    "Option 1: Major boost to growth rate, minor boost to quality",
                    "Option 2: Balanced boost to growth rate and quality",
                    "Option 3: Minor boost to growth rate, major boost to quality",
                   
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
                instruction = "The Rite of Mists uses directional and cursor based logic to select a target point ahead of or away from the player",
                details = new()
                {
                    "This is different to the centered-on-player targetting logic of Rite of the Weald",
                    "Controllers: Will use directional targetting based on the direction the player is facing.",
                    "Cursor Proximity: If the cursor is too close to the farmer position on screen the logic will default to directional targetting. " +
                        "This is because it's too difficult for the logic to discern the vector of the cursor relative to the player.",
                    "The direction and position of the farmer and/or mouse cursor is important to get precise hits."
                }

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

            // ====================================================================
            // Stars Effects

            effects[QuestHandle.questEffigy] = new() { summonWisps, summonEffigy };

            Effect meteorRain = new()
            {
                title = "Meteor Rain",
                icon = IconData.displays.stars,
                description = "If nature extends to the celestial realm, then the stars themselves are it's greatest force, a force now granted to me in order to burn away the taint and decay of a stagnated world.",
                instruction = "Cast Rite of the Stars to produce a meteor that strikes objects and monsters within the impact radius of a random point near the Farmer. Will dislodge most set down objects.",
                details = new()
                {
                    "Cost: 12 - Player Combat Level, minimum 5",
                    "Activate: Hold the rite button to continue to produce a shower of meteors around the Farmer",
                    "Mastery: Chance for an additional meteor to be produced per cast cycle",
                    "The damage and behaviour of meteor rain can be changed in the configuration options",
                    "A melee weapon is required to cast meteors on farmland",
                    "Option 1. Prioritises Monsters and Stone Nodes. If no target is available, will utilise a regular spatial pattern for optimal coverage. Normal damage. (default)",
                    "Option 2. Prioritises Monsters, then follows spatial pattern, 1.1x damage",
                    "Option 3. Prioritises Stone nodes, then follows spatial pattern, 1.1x damage",
                    "Option 4. No prioritisation, follows spatial pattern, 1.25x damage, 33 % chance for large meteor",
                    "Option 5. Completely random targetting, 1.6x damage, 50 % chance for large meteor",
                }
            };

            Effect starBurst = new()
            {
                title = "Starburst",
                icon = IconData.displays.stars,
                description = "My weapons gleam with starlight.",
                instruction = "While charged, melee attacks against monsters will deal a small measure of burst damage and knock them back.",
                details = new()
                {
                    "Activate: Press the special button while casting Rite of the Stars to charge.",
                    "Cooldown: 2 second",
                    "Damage: Druid Level * 10",
                    "Stun: knocks down smaller monsters for 2 seconds, stuns larger monsters for 0.5 seconds",
                }
            };

            effects[QuestHandle.starsOne] = new() { meteorRain, starBurst, };

            Effect gravityWell = new()
            {
                title = "Gravity Well",
                icon = IconData.displays.stars,
                description = "I have become familiar with the dark side of celestial magic, the anti-star, the ravenous void that pulls at the fabric of the material plane",
                instruction = "Cast and hold the rite button to channel a gravity well at the cursor location.",
                details = new()
                {
                    "Activate: Hold the rite button until the channel cursor expands past the rite indicator",
                    "Mastery: A massive instance of meteor rain will be drawn to the black hole.",
                    "Nearby harvestable crops will be harvested and pulled towards the scarecrow, with boosts to quantity and quality based on Weald:Cultivation settings.",
                    "Monsters will be stunned and pulled in. Uses directional and cursor targetting.",
                }
            };

            effects[QuestHandle.starsTwo] = new() { gravityWell, };

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

            // ====================================================================
            // Stars Effects

            Effect summonJester = new()
            {
                title = "Adventures with Jester",
                icon = IconData.displays.jester,
                description = "Jester believes that his great purpose is intertwined with my own story. Despite the resoluton of our bargain, he will remain close by while he searches for his missing kin.",
                instruction = "Approach the Jester of Fate and select (adventure) from the dialogue menu to give him instructions.",
                details = new()
                {
                    "The 'Embossed Die of the Fates' can be used in the relics menu to summon Jester",
                    "Jester can accompany you to other locations, and will perform his own attacks against nearby monsters.",
                    "Jester can be invited to roam the farm, and will perform his version of Weald:Community when in proximity to villagers and farm animals, which provides a friendship boost to the target. ",
                }
            };

            effects[QuestHandle.approachJester] = new() { summonJester, };            

            Effect whisk = new()
            {
                title = "Whisk",
                icon = IconData.displays.fates,
                description = "The Fates do not constrain themselves to the physical laws of this world.",
                instruction = "Cast Rite of the Fates to fire a warp projectile, then warp along its path by pressing the rite button (again) or action button before the projectile expires.",
                details = new()
                {
                    "Range: up to 16 tiles",
                    "Mastery: Extends range by 8",
                    "Rite Button (Second press): warps caster to warp terminal (furthest point) instantly",
                    "Action button: warps caster to the current position of the warp projectile",
                    "Cost: 12 stamina",
                }
            };

            effects[QuestHandle.fatesOne] = new() { whisk, };

            Effect warpstrike = new()
            {
                title = "Warpstrike",
                icon = IconData.displays.fates,
                description = "Meow. Nothing escapes fate. Meow. Jester's word's reverberate in my head as I dominate my fates-stricken foes.",
                instruction = "Rite of the Fates: Whisk applies curses to monsters in the vicinity of the casting cursor. A whisk activation will trigger an extended warpstrike against any nearby cursed monsters.",
                details = new()
                {
                    "Activation: Replaces whisk with a warpstrike if nearby cursed monsters are detected",
                    "Mastery: Critical hits will trigger additional strikes against the same target.",
                    "Cost (Curse): 12 per target cursed",
                    "Cost (Warpstrike: Free",
                }
            };

            Effect curses = new()
            {
                title = "Curses",
                icon = IconData.displays.fates,
                description = "I interact with the otherworld in ways that would frighten the Druids of antiquity.",
                instruction = "While charged, attacks against monsters will apply one of four effects that correspond to the various factions of the Fae court.",
                details = new()
                {
                    "Polymorph: Converts the monster into a thrall of the Artisans of Fate, with a random rescaling.",
                    "Pickpocket: The touch of Chaos Causes the monster to drop a random item.",
                    "Dazzle: Slows the monster and confuses their targetting as they grapple with the great mysteries of the Priesthood of Fate.",
                    "Doom / Instant Death: The Morticians of Fate deal lethal damage to monsters after a countdown elapse or if they are under 7% of max health.",
                    "Companion Combo: Jester's attacks will also apply these curses.",
                }
            };

            effects[QuestHandle.fatesTwo] = new() { warpstrike, curses, };

            Effect tricks = new()
            {
                title = "Tricks",
                icon = IconData.displays.fates,
                description = "Druids train for many years to master the oral tradition, not only to safeguard esoteric knowledge, but to entertain and inspire their communities. Now, with Jester's help, I can add special effects.",
                instruction = "Villagers can be targetted with Rite of the Fates. A randomised effect will play that will either be disliked, liked or loved by the villager.",
                details = new()
                {
                    "Targetting: Cursor and Directional Targetting",
                    "Mastery: Unlocks more effects.",
                    "Friendship: Provides -25 to +75 friendship based on whether the villager disliked or liked the effect.",
                    "Cost: Free",
                }
            };

            effects[QuestHandle.fatesThree] = new() { tricks, };

            Effect enchant = new()
            {
                title = "Enchant",
                icon = IconData.displays.fates,
                description = "The otherworld has it's own devices of faye design, powered by Faeth, an essence distilled from light and void. Now I can experiment with using Faeth to power my own artifice.",
                instruction = "Enchant one of various types of farm machine to produce a randomised product without standard inputs. Each enchantment consumes one Faeth.",
                details = new()
                {
                    "Requires: 1 Faeth, brewable in the herbalism journal (page 2)",
                    "Mastery: Extracts existing products from machines.",
                    "Channel: Cast and hold Rite of the Fates for 2-3 seconds while standing still, until the essence fills the rite indicator. When released, the enchantment effect will radiate in increasing magnitude.",
                    "Works on Deconstructors, Bone Mills, Kegs, Preserves Jars, Cheese Presses, Mayonnaise Machines, Looms, Oil Makers, Furnaces and Geode Crushers.",
                    "Gathers outputs from surrounding machines.",
                    "Cost: 24 stamina",
                }
            };

            effects[QuestHandle.fatesFour] = new() { enchant, };

            Effect summonShadowtin = new()
            {
                title = "The Shadow Scholar",
                icon = IconData.displays.shadowtin,
                description = "The figure in the tin bear mask is a legend amongst the Shadowfolk. He has a fetish for rare collectibles, especially from the age of Dragons, and relishes the chance to further explore the forgotten history of the valley.",
                instruction = "Approach Shadowtin Bear and select (adventure) from the dialogue menu to give him instructions.",
                details = new()
                {
                    "The 'Compendium on the Ancient War' can be used in the relics menu to summon Shadowtin",
                    "Shadowtin can join your other allies on the farm, grove or at your side. He can use his Carnyx to stun enemies, or twirl it in a spin attack.",
                    "Shadowtin will pick up nearby forageables during idle moments when at work or when following the player.",
                }
            };

            effects[QuestHandle.challengeFates] = new() { summonShadowtin, };

            // ====================================================
            // Dragon Effects

            Effect dragonForm = new()
            {
                title = "Dragon Form",
                icon = IconData.displays.ether,
                description = "The Dragons have long been venerated as masters of the ether, the quintessential fifth element that defines the shape and nature of the spiritual domain. With the Dragontooth of Tyrannus Prime, I can adopt the guise of an Ancient One, and learn to master the ether myself.",
                instruction = "Press the rite button to transform into a Dragon. Press the rite button again to detransform.",
                details = new()
                {
                    "Dragon form should maintain through map transitions, but will exit for events or locations that are restricted.",
                }
            };

            Effect dragonFlight = new()
            {
                title = "Dragon Flight",
                icon = IconData.displays.ether,
                description = "I can feel the streams of ether rushing softly under my finger tips, and then my wing tips as I allow them to lift me into the sky.",
                instruction = "While in dragon form, press the action button/left click to perform a sweeping flight.",
                details = new()
                {
                    "Targetting: Flies in the direction the farmer is facing.",
                    "Monster Proximity: Will instead perform a sweeping tail attack when in close proximity to monsters.",
                    "Direction: Uses isometric lines by default, can be changed to strictly cardinal lines in the config.",
                    "Mastery: Nearby foes are damaged on take off and landing.",
                }
            };

            effects[QuestHandle.etherOne] = new() { dragonForm, dragonFlight, };

            Effect dragonBreath = new()
            {
                title = "Dragon Breath",
                icon = IconData.displays.ether,
                description = "I draw the ether in, then expel it as a torrent of violent energy.",
                instruction = "While in dragon form, Press the special button/right click to perform a dragon breath attack.",
                details = new()
                {
                    "Targetting: Uses cursor and directional targetting.",
                    "Potency: Powerful enough to destroy terrain and reave dirt",
                    "Mastery: Damage effect creates a zone of fire that applies the 'Burn' status" +
                    "Mastery (Burn): has a chance to immolate enemies and convert them into cooking items."
                }
            };

            effects[QuestHandle.etherTwo] = new() { dragonBreath, };

            Effect dragonDive = new()
            {
                title = "Dragon Dive",
                icon = IconData.displays.ether,
                description = "It is time to bring blessings to the world under the waters.",
                instruction = "While in dragon form, use Dragon Flight near water to fly into and out of swim mode. ",
                details = new()
                {
                    "Dive: Press the special button/right click while swimming to dive for treasure." ,
                    "Mastery: Dives have a chance for higher quality fish",
                }
            };

            effects[QuestHandle.etherThree] = new() { dragonDive, };

            Effect dragonTreasure = new()
            {
                title = "Dragon Treasure",
                icon = IconData.displays.ether,
                description = "The treasures of antiquity were sealed away with ethereal binds, all in the hope that a master of the ether would arise from the ranks of the Druids to reclaim them.",
                instruction = "While in dragon form, treasure markers will appear once day on large maps, these can be activated with the Special button/ right click to claim the treasure. ",
                details = new()
                {
                    "Location: Search for the ether symbol on large map locations (Forest, Beach, Mountain, RailRoad, Desert, Island etc).",
                    "Activate: Move over the spot and either dig or dive (special/right click button) to claim the dragon treasure.",
                    "Thieves: There is a chance a monster will attempt to abscond with the treasure.",
                    "Mastery: Treasure caches provide Ether",
                }
            };

            effects[QuestHandle.etherFour] = new() { dragonTreasure, };

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
