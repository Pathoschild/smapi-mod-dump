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
using StardewDruid.Character;
using StardewDruid.Data;
using StardewDruid.Dialogue;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using static StardewDruid.Journal.HerbalData;
using static StardewDruid.Journal.RelicData;
using static System.Formats.Asn1.AsnWriter;

namespace StardewDruid.Journal
{
    public class RelicData
    {

        public enum relicsets
        {
            
            none,
            companion,
            wayfinder,
            herbalism,
            runestones,
            avalant,
            books,
            courtesan,
        }

        public Dictionary<relicsets, List<string>> titles = new()
        {

            [relicsets.companion] = new() { "Companion Tokens", "Things to remember my new friends by", },
            [relicsets.wayfinder] = new() { "Mystic Wayfinders", "Used to reveal hidden passageways", },
            [relicsets.herbalism] = new() { "Apothecary's Tools", "Used to create potent herbal tonics and potions", },
            [relicsets.runestones] = new() { "Old Runestones", "Recovered from foes of the circle", },
            [relicsets.avalant] = new() { "Bronze Artifacts", "Retrieved from fishing spots around the valley", },
            [relicsets.books] = new() { "Preserved Texts", "Writing from the time of dragons", },
            [relicsets.books] = new() { "Courtesan Ornaments", "Left behind by envoys of the Fae Court", },
        };

        public Dictionary<relicsets, List<IconData.relics>> lines = new()
        {
            [relicsets.companion] = new() {
                IconData.relics.effigy_crest,
                IconData.relics.jester_dice,
                IconData.relics.shadowtin_tome,
            },
            [relicsets.wayfinder] = new() {
                IconData.relics.wayfinder_lantern,
                IconData.relics.wayfinder_water,
                IconData.relics.wayfinder_eye,
                IconData.relics.wayfinder_ceremonial,
            },
            [relicsets.herbalism] = new() {
                IconData.relics.herbalism_mortar,
                IconData.relics.herbalism_pan,
                IconData.relics.herbalism_still,
                IconData.relics.herbalism_crucible,
            },
            [relicsets.runestones] = new() {
                IconData.relics.runestones_spring,
                IconData.relics.runestones_farm,
                IconData.relics.runestones_moon,
                IconData.relics.runestones_cat,
            },
            [relicsets.avalant] = new() {
                IconData.relics.avalant_disc,
                IconData.relics.avalant_chassis,
                IconData.relics.avalant_gears,
                IconData.relics.avalant_casing,
                IconData.relics.avalant_needle,
                IconData.relics.avalant_measure,
            },
            [relicsets.books] = new() {
                IconData.relics.book_wyrven,
            },
            [relicsets.courtesan] = new() {
                IconData.relics.courtesan_pin,
            },
        };

        public Dictionary<string, Relic> reliquary = new();

        public RelicData()
        {

            reliquary = RelicsList();

            CheckRelics();

        }

        public void CheckRelics()
        {

            for(int i = Mod.instance.save.reliquary.Count - 1; i >= 0; i--)
            {

                string relic = Mod.instance.save.reliquary.ElementAt(i).Key;

                if (!reliquary.ContainsKey(relic))
                {

                    Mod.instance.save.reliquary.Remove(relic);

                }

            }

        }

        public List<List<string>> OrganiseRelics()
        {

            List<List<string>> source = new();

            foreach(relicsets set in lines.Keys)
            {

                List<string> add = new();

                for(int i = 0; i < lines[set].Count; i++)
                {
                    
                    if (Mod.instance.save.reliquary.ContainsKey(lines[set][i].ToString()))
                    {

                        add.Add(lines[set][i].ToString());

                    }

                }

                if(add.Count == 0)
                {
                    
                    continue;

                }

                if (source.Count == 0 || source.Last().Count() == 18)
                {
                    
                    source.Add(new List<string>());
                
                }

                for (int i = 0; i < 6; i++)
                {

                    if(i < add.Count)
                    {

                        source.Last().Add(add[i]);


                    }
                    else
                    {

                        source.Last().Add("blank");

                    }

                }

            }

            return source;

        }

        public void ReliquaryUpdate(string id, int update = 0)
        {

            if (!Mod.instance.save.reliquary.ContainsKey(id))
            {

                Mod.instance.save.reliquary[id] = update;

            }
            else if(Mod.instance.save.reliquary[id] < update)
            {

                Mod.instance.save.reliquary[id] = update;

            }

        }

        public static Dictionary<string, Relic> RelicsList()
        {

            Dictionary<string, Relic> relics = new();

            // ====================================================================
            // Companion Tokens

            relics[IconData.relics.effigy_crest.ToString()] = new()
            {
                title = "Enameled Star Crest",
                relic = IconData.relics.effigy_crest,
                line = RelicData.relicsets.companion,
                function = true,
                description = "Given to me by the Forgotten Effigy, the only surviving member of the old Circle of Druids.",
                details = new()
                {
                    "It is untarnished by time, and connected to my companion by some mystery.",
                    "Click to summon the Effigy to you."
                },
                heldup = "You received The Effigy's Enameled Star Crest! It can be used to summon the Effigy to accompany you on your adventures. Check the relics journal to view further details.",
            };

            relics[IconData.relics.jester_dice.ToString()] = new()
            {
                title = "Embossed Die of the Fates",
                relic = IconData.relics.jester_dice,
                line = RelicData.relicsets.companion,
                function = true,
                description = "Given to me by the Jester of Fate, envoy of the Fae Court.",
                details = new()
                {
                    "There's some kind of mechanism inside that jingles when the die is rolled.",
                    "Click to summon Jester to you."
                },
                heldup = "You received The Jester of Fate's Embossed Die of the Fates! It can be used to summon Jester to accompany you on your adventures. Check the relics journal to view further details.",
            };


            relics[IconData.relics.shadowtin_tome.ToString()] = new()
            {
                title = "Compendium on the Ancient War",
                relic = IconData.relics.shadowtin_tome,
                line = RelicData.relicsets.companion,
                function = true,
                description = "Given to me by Shadowtin Bear, treasure hunter and aspirant scholar.",
                details = new()
                {
                    "It's a comprehensive collection of Shadowtin's knowledge on the War for the Fallen Star. It is written in shadowtongue.",
                    "Click to summon Shadowtin to you."
                },
                heldup = "You received Shadowtin Bear's Compendium on the Ancient War! It can be used to summon Shadowtin to accompany you on your adventures. Check the relics journal to view further details.",
            };


            // ====================================================================
            // Wayfinder relics

            relics[IconData.relics.wayfinder_lantern.ToString()] = new()
            {
                title = "Star Guardian Lantern",
                relic = IconData.relics.wayfinder_lantern,
                line = RelicData.relicsets.wayfinder,
                function = true,
                description = "The order of Star Guardians preceded the Church of Bats, and placed hundreds of lanterns throughout their cavernholds in the mountain",
                details = new()
                {
                    "Click when on level 60 of the mines to open the passage way to the Chapel of the Stars.",
                },
                heldup = "You received the Star Guardian Lantern! It can be used to access the Chapel of the Stars on level 60 of the mines. Check the relics journal to view further details.",
            };

            relics[IconData.relics.wayfinder_water.ToString()] = new()
            {
                title = "Luminous Water of the Sacred Spring",
                relic = IconData.relics.wayfinder_water,
                line = RelicData.relicsets.wayfinder,
                function = true,
                description = "The waters of the sacred spring resist the brazen heat of the lavaflows deep under the mountain, where the terror beneath resides",
                details = new()
                {
                    "Click when on level 100 of the mines to open the passage way to the Molten Lair.",
                },
                heldup = "You received the Luminous Water of the Sacred Spring! It can be used to access the Molten Lair on level 100 of the mines. Check the relics journal to view further details.",
            };

            relics[IconData.relics.wayfinder_eye.ToString()] = new()
            {
                title = "Eye of the Warble",
                relic = IconData.relics.wayfinder_eye,
                line = RelicData.relicsets.wayfinder,
                function = true,
                description = "A Warble can peer beyond the bounds of space and time.",
                details = new()
                {
                    "Squeeze the eye and all the frustrations of the current moment will seem to fade away.",
                    "Click to warp to the map entrance, prioritising the direction you're facing."
                },
                heldup = "You received the Eye of the Warble! It can be used to warp to map entrances. Check the relics journal to view further details.",
            };

            relics[IconData.relics.wayfinder_ceremonial.ToString()] = new()
            {
                title = "Calico Ceremonial Lamp",
                relic = IconData.relics.wayfinder_ceremonial,
                line = RelicData.relicsets.wayfinder,
                function = true,
                description = "The civilisation of the calico shamans centered on reverence for a dragon. When the ancient being perished, so did the shaman's customs.",
                details = new()
                {
                    "Click when in the entrance to the skull caverns to open the passage way to the Tomb of Tyrannus .",
                },
                heldup = "You received the Calico Ceremonial Lamp! It can be used to access the Tomb of Tyrannus in the skull caverns. Check the relics journal to view further details.",
            };

            // ====================================================================
            // Herbalism Relics

            relics[IconData.relics.herbalism_mortar.ToString()] = new()
            {
                title = "Circle Mortar and Pestle",
                relic = IconData.relics.herbalism_mortar,
                line = RelicData.relicsets.herbalism,
                description = "One of the tools left behind by the old circle of Druids.",
                details = new()
                {
                    "Provides the means to brew basic potions.",
                    "The bowl is pleased."
                },
                heldup = "You received the Circle Mortar and Pestle! You are now able to brew basic potions. Check the potions journal to view further details.",
            };

            relics[IconData.relics.herbalism_pan.ToString()] = new()
            {
                title = "The First Pan",
                relic = IconData.relics.herbalism_pan,
                line = RelicData.relicsets.herbalism,
                description = "It belonged to the first farmer, but is useless to the Effigy, who does not consume organic matter",
                details = new()
                {
                    "Provides the means to brew better potions.",
                },
                heldup = "You received The First Pan! You are now able to brew better potions. Check the potions journal to view further details.",
            };

            relics[IconData.relics.herbalism_still.ToString()] = new()
            {
                title = "Sacred Water Still",
                relic = IconData.relics.herbalism_still,
                line = RelicData.relicsets.herbalism,
                description = "Before they were reformed into the Order of the Guardians of the Star, some amongst the human custodians of the sacred spring were hobby brewers.",
                details = new()
                {
                    "Provides the means to brew good potions.",
                },
                heldup = "You received the Sacred Water Still! You are now able to brew good potions. Check the potions journal to view further details.",
            };

            relics[IconData.relics.herbalism_crucible.ToString()] = new()
            {
                title = "Dark Crucible of Yoba",
                relic = IconData.relics.herbalism_crucible,
                line = RelicData.relicsets.herbalism,
                description = "From the smelter-halls of the Artisans of the Fates, who delight in drawing things down to their very essence.",
                details = new()
                {
                    "Provides the means to brew great potions.",
                },
                heldup = "You received the Dark Crucible Of Yoba! You are now able to brew great potions and a special resource; Faeth! Check page 2 of the potions journal for more details.",
            };

            // ====================================================================
            // Circle Runestones

            relics[IconData.relics.runestones_spring.ToString()] = new()
            {
                title = "Runestone of the Sacred Spring",
                relic = IconData.relics.runestones_spring,
                line = RelicData.relicsets.runestones,
                description = "Carried by the high cleric of bats.",
                details = new()
                {
                    "The bearer of this runestone adopted the customs and zealotry of those that once guarded the sacred mountain spring.",
                    "First of Four",
                },
                heldup = "You received the Runestone of the Sacred Spring! The energies of the weald might know more about this artifact. Check the relics journal to view further details.",
            };

            relics[IconData.relics.runestones_moon.ToString()] = new()
            {
                title = "Runestone of the Crooked Moon",
                relic = IconData.relics.runestones_moon,
                line = RelicData.relicsets.runestones,
                description = "Carried by the Jellyking.",
                details = new()
                {
                    "This runestone was once covered in pumpkin muck and slime. It had been swallowed by a foul spirit of the forest, the Jellyking, who took great delight in spiting the old circle of druids",
                    "Second of Four",
                },
                heldup = "You received the Runestone of the Crooked Moon! The energies of the weald might know more about this artifact. Check the relics journal to view further details.",
            };

            relics[IconData.relics.runestones_farm.ToString()] = new()
            {
                title = "Runestone of the New Farm",
                relic = IconData.relics.runestones_farm,
                line = RelicData.relicsets.runestones,
                description = "Carried by a Seafaring Phantom.",
                details = new()
                {
                    "The Effigy believes this runestone symbolises the hopes of those that restored the farmholds that were torched during the old war. One of those farmers must have lost it to the sea.",
                    "Third of Four",
                },
                heldup = "You received the Runestone of the New Farm! The energies of the weald might know more about this artifact. Check the relics journal to view further details.",
            };

            relics[IconData.relics.runestones_cat.ToString()] = new()
            {
                title = "Runestone of the Old Cat",
                relic = IconData.relics.runestones_cat,
                line = RelicData.relicsets.runestones,
                description = "Carried by a Lesser Dragon.",
                details = new()
                {
                    "This runestone was carried by a champion of the Star Guardians who was also a member of the Circle of Druids, until they ran foul of a powerful dragon in the depths of the lava caverns.",
                    "Fourth of Four",
                },
                heldup = "You received the Runestone of the Old Cat! The energies of the weald might know more about this artifact. Check the relics journal to view further details.",
            };

            // ====================================================================
            // Avalant Relics

            relics[IconData.relics.avalant_disc.ToString()] = new()
            {
                title = "Ancient Bronze Disc",
                relic = IconData.relics.avalant_disc,
                line = RelicData.relicsets.avalant,
                description = "The base of an ancient device, fished out of the waters of the local forest.",
                details = new()
                {
                    "First of Six",
                },
                heldup = "You received a Ancient Bronze Disc! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            relics[IconData.relics.avalant_chassis.ToString()] = new()
            {
                title = "Ancient Bronze Chassis",
                relic = IconData.relics.avalant_chassis,
                line = RelicData.relicsets.avalant,
                description = "The structural core of an ancient device, fished out of the mountain lake.",
                details = new()
                {
                    "Second of Six",
                },
                heldup = "You received a Ancient Bronze Chassis! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            relics[IconData.relics.avalant_gears.ToString()] = new()
            {
                title = "Ancient Bronze Gears",
                relic = IconData.relics.avalant_gears,
                line = RelicData.relicsets.avalant,
                description = "A mechanical component fished out of the local beach.",
                details = new()
                {
                    "Third of Six",
                },
                heldup = "You received a set of Ancient Bronze Gears! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            relics[IconData.relics.avalant_casing.ToString()] = new()
            {
                title = "Ancient Bronze Casement",
                relic = IconData.relics.avalant_casing,
                line = RelicData.relicsets.avalant,
                description = "The top casement of an ancient device, found in the cavern waters.",
                details = new()
                {
                    "Fourth of Six",
                },
                heldup = "You received a Ancient Bronze Casement! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            relics[IconData.relics.avalant_needle.ToString()] = new()
            {
                title = "Ancient Bronze Needle",
                relic = IconData.relics.avalant_needle,
                line = RelicData.relicsets.avalant,
                description = "A measuring needle fished out of the town river.",
                details = new()
                {
                    "Fifth of Six",
                },
                heldup = "You received a Strange Bronze Needle! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            relics[IconData.relics.avalant_measure.ToString()] = new()
            {
                title = "Ancient Bronze Measure",
                relic = IconData.relics.avalant_measure,
                line = RelicData.relicsets.avalant,
                description = "A measuring wheel found in the waters of a secluded atoll.",
                details = new()
                {
                    "Sixth of Six",
                },
                heldup = "You received an Ancient Bronze Measure! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            // ====================================================================
            // Preserved Texts

            relics[IconData.relics.book_wyrven.ToString()] = new()
            {
                title = "Knight Wyrven's Journal",
                relic = IconData.relics.book_wyrven,
                line = RelicData.relicsets.books,
                function = true,
                description = "A journal that serves an annal in the lost chronicles of the Guardians of the Star, recovered by Shadowtin's mercenaries from a repository in the deep quarry tunnels.",
                details = new()
                {
                    "Click to read.",
                    "The bounding resembles the colour and iconography of the Order of the Guardians of the Star",
                    "There's a graphite engraving of the Old Cat Runestone inside.",
                    "First of Five",
                    
                },
                heldup = "You received the first part of Knight Wyrven's Journal! It appears to have belonged to a Guardian of the Star. The Revenant might know more about this artifact.",

                narrative = new()
                {
                    
                    "Summer, Second Year",
                    "A low court of the Fates has been established to administer the valley in the aftermath of the war. " +
                    "It sits on the banks of the mountain stream that flows from the sacred spring, and the Justiciar of the Fates resides over the proceedings. " +
                    "The Justiciar's hound, the reaper, continues to grow in reverence with the assembly. " +
                    "His obsession with the recovery of the hidden Star-born is on full display, and the refugees listen fervently to his rhetoric. " +
                    "They applaud the viciousness with which the Mortician demonstrates against the surviving elderborn. " +
                    "They desire justice for the destruction of lives and families, but what comfort can be found in the humiliation of an endangered race. " +
                    "So few civilisations remain after the devastation of war.",
                    
                    "Winter, Second Year",
                    "The calico shamans have paid the final tributes to their draconic master, the Tyrant Djinn, and discarded the yoke of their servitude with the ancient one. " +
                    "I was invited to play a symbolic role in the ceremony. So I travelled to the skull caverns, to a mausoleum big enough to house the wasted bones of a dragon. " +
                    "We exchanged gifts of sacred water at the dias, to represent the reconciliation of the Guardians of the Star with our once great adversaries. " +
                    "Their water was pure, but I knew it was doused in their tears, and I drank to share their pain. " +
                    "For I played my part in the destruction wrought by the Bull of the Heavens in his vendetta against the dragons. " +
                    "As I watched them bury their old traditions, I felt a breeze sweeping through the cavern, an impossible breeze, as we were still far under the desolation that claimed the lush nation of Calico. " +
                    "I know now it was a stream of ether. " +
                    "The shamans, either knowingly or unknowingly, revealed it to me, the ephemeral, transformational substance that flows between realms. " +
                    "The legacy of Dragons. " +
                    "I intend to explore the possibilities of this purchase, and perhaps transform the Guardians into a force to rival the unchallenged authority of the Fates.",
                    
                    "Spring, Third year",
                    "The Justiciar intends to close the low court at the end of summer. " +
                    "The crowmother has already lost interest in hearing appeals and accusations, and has convinced her peers to vest authority in the Circle of Druids, " +
                    "to take responsibility for the renewal of the valley and the protection of the sacred spaces. ",
                    
                    "Summer, Third Year",
                    "I have incorporated the remainder of my order into the circle. It was the prudent choice considering our fragmented morale and numbers. " +
                    "The court has been disestablished, but the unresolved issue of the missing Star-born does not sit well with the Justiciar. " +
                    "They are desperate to bring the Bull of the Heavens before the high Fae Court to answer for his actions. " +
                    "As far as I'm aware, our great general vanished with the Elderborn prince, and that is the story we have stuck too. " +
                    "Still the reaper's acolytes continue to harrass us in the public forums. " +
                    "I fear an escalation.",

                }

            };


            // ====================================================================
            // Fates Relics

            relics[IconData.relics.courtesan_pin.ToString()] = new()
            {
                title = "Courtesan's Hair Pin",
                relic = IconData.relics.courtesan_pin,
                line = RelicData.relicsets.courtesan,
                description = "A hair pin that belonged to a courtisan of the Fates, held close by Thanatoshi, the Twilight Reaper.",
                details = new()
                {
                    "First of Four",
                },
                heldup = "You received a Courtesan's Hair Pin! This might have belonged to a member of the Fae Court. Buffin should know more about it.",
            };

            return relics;

        }

        public int RelicFunction(string id)
        {

            Relic relic = reliquary[id];

            switch (relic.relic)
            {

                case IconData.relics.effigy_crest:

                    if (!Context.IsMainPlayer)
                    {

                        break;

                    }

                    if (Mod.instance.characters.ContainsKey(CharacterHandle.characters.Effigy))
                    {

                        List<StardewDruid.Character.Character.mode> reservedModes = new() { Character.Character.mode.scene, Character.Character.mode.track, };

                        if (!reservedModes.Contains(Mod.instance.characters[CharacterHandle.characters.Effigy].modeActive))
                        {

                            Mod.instance.characters[CharacterHandle.characters.Effigy].SwitchToMode(Character.Character.mode.track, Game1.player);

                            Mod.instance.CastMessage("The Effigy has joined you on your adventures");

                            return 1;

                        }

                    }

                    break;

                case IconData.relics.jester_dice:

                    if (!Context.IsMainPlayer)
                    {

                        break;

                    }

                    if (Mod.instance.characters.ContainsKey(CharacterHandle.characters.Jester))
                    {

                        List<StardewDruid.Character.Character.mode> reservedModes = new() { Character.Character.mode.scene, Character.Character.mode.track, };

                        if (!reservedModes.Contains(Mod.instance.characters[CharacterHandle.characters.Jester].modeActive))
                        {

                            Mod.instance.characters[CharacterHandle.characters.Jester].SwitchToMode(Character.Character.mode.track, Game1.player);

                            Mod.instance.CastMessage("The Jester of Fates follows you about");

                            return 1;

                        }

                    }

                    break;


                case IconData.relics.shadowtin_tome:

                    if (!Context.IsMainPlayer)
                    {

                        break;

                    }

                    if (Mod.instance.characters.ContainsKey(CharacterHandle.characters.Shadowtin))
                    {

                        List<StardewDruid.Character.Character.mode> reservedModes = new() { Character.Character.mode.scene, Character.Character.mode.track, };

                        if (!reservedModes.Contains(Mod.instance.characters[CharacterHandle.characters.Shadowtin].modeActive))
                        {

                            Mod.instance.characters[CharacterHandle.characters.Shadowtin].SwitchToMode(Character.Character.mode.track, Game1.player);

                            Mod.instance.CastMessage("Shadowtin Bear has joined you for a treasure hunt");

                            return 1;

                        }

                    }

                    break;

                case IconData.relics.wayfinder_lantern:

                    if(Game1.player.currentLocation.Name == "UndergroundMine60")
                    {

                        Event.Access.AccessHandle lanternAccess = new();

                        lanternAccess.AccessSetup("UndergroundMine60", Location.LocationData.druid_chapel_name, new(24, 13), new(27, 30));

                        lanternAccess.AccessCheck(Game1.player.currentLocation);

                        return 1;

                    }

                    break;

                case IconData.relics.wayfinder_water:

                    if (Game1.player.currentLocation.Name == "UndergroundMine100")
                    {

                        Event.Access.AccessHandle lanternAccess = new();

                        lanternAccess.AccessSetup("UndergroundMine100", Location.LocationData.druid_vault_name, new(24, 13), new(27, 30));

                        lanternAccess.AccessCheck(Game1.player.currentLocation);

                        return 1;

                    }

                    break;

                case IconData.relics.wayfinder_eye:


                    Vector2 destination;

                    if (Game1.player.currentLocation is MineShaft)
                    {

                        destination = WarpData.WarpXZone(Game1.player.currentLocation);


                    }
                    else
                    {
                        destination = WarpData.WarpEntrance(Game1.player.currentLocation, Game1.player.Position);

                    }

                    if(destination != Vector2.Zero)
                    {

                        SpellHandle teleport = new(Game1.player.currentLocation, destination, Game1.player.Position);

                        teleport.type = SpellHandle.spells.teleport;

                        Mod.instance.spellRegister.Add(teleport);

                        Mod.instance.AbortAllEvents();

                    }
                    else
                    {

                        Mod.instance.CastMessage("Unable to find a valid warp point");

                    }

                    return 1;

                case IconData.relics.wayfinder_ceremonial:

                    if (Game1.player.currentLocation.Name == "SkullCave")
                    {

                        Event.Access.AccessHandle lanternAccess = new();

                        lanternAccess.AccessSetup("SkullCave", Location.LocationData.druid_tomb_name, new(10, 5), new(27, 30));

                        lanternAccess.AccessCheck(Game1.player.currentLocation);

                        return 1;

                    }

                    break;

                case IconData.relics.book_wyrven:

                    return 2;


            }

            return 0;


        }

        public IconData.relics RelicMistsLocations()
        {

            if(Game1.player.currentLocation is Forest)
            {

                return IconData.relics.avalant_disc;

            }
            else if (Game1.player.currentLocation is Mountain)
            {

                return IconData.relics.avalant_chassis;

            }
            else if (Game1.player.currentLocation is Beach)
            {

                return IconData.relics.avalant_gears;

            }
            else if (Game1.player.currentLocation is MineShaft)
            {

                return IconData.relics.avalant_casing;

            }
            else if (Game1.player.currentLocation is Town)
            {

                return IconData.relics.avalant_needle;

            }
            else if (Game1.player.currentLocation is StardewDruid.Location.Atoll)
            {
                
                return IconData.relics.avalant_measure;

            }

            return IconData.relics.none;

        }

        public int ProgressRelicQuest(relicsets relicset, bool update = false)
        {
            int count = 0;

            foreach (IconData.relics relic in lines[relicset])
            {

                if (Mod.instance.save.reliquary.ContainsKey(relic.ToString()))
                {

                    if (update)
                    {

                        Mod.instance.save.reliquary[relic.ToString()] = 1;

                        continue;

                    }

                    if (Mod.instance.save.reliquary[relic.ToString()] == 1)
                    {

                        return -1;

                    }

                    count++;

                }

            }

            return count;

        }

        public void FinishRelicQuest(relicsets relicset)
        {

            foreach (IconData.relics relic in lines[relicset])
            {

                if (Mod.instance.save.reliquary.ContainsKey(relic.ToString()))
                {

                    Mod.instance.save.reliquary[relic.ToString()] = 1;

                }

            }

        }


    }

    public class Relic
    {

        // -----------------------------------------------
        // journal

        public string title;

        public IconData.relics relic = IconData.relics.none;

        public RelicData.relicsets line = RelicData.relicsets.none;

        public bool function;

        public bool activatable;

        public string description;

        public string heldup;

        public List<string> details = new();

        public List<string> narrative;

    }

}
