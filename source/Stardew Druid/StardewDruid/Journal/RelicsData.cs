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
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.Locations;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewDruid.Journal
{
    public class RelicsData
    {

        public enum relicsets
        {
            
            none,
            companion,
            minister,
            avalant,

        }

        public Dictionary<string, Relic> reliquary = new();

        public RelicsData()
        {

            reliquary = RelicsList();

        }

        public List<List<string>> OrganiseRelics()
        {

            List<List<string>> source = new();

            foreach (KeyValuePair<string, Relic> pair in Mod.instance.relicsData.reliquary)
            {

                if (Mod.instance.save.reliquary.ContainsKey(pair.Key))
                {

                    if (source.Count == 0 || source.Last().Count() == 15)
                    {
                        source.Add(new List<string>());
                    }

                    source.Last().Add(pair.Key);

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
            // Effigy crest

            relics[IconData.relics.effigy_crest.ToString()] = new()
            {
                title = "Enameled Star Crest",
                relic = IconData.relics.effigy_crest,
                line = RelicsData.relicsets.companion,
                function = true,
                description = "Given to me by the Forgotten Effigy, last remnant of the old circle of druids",
                details = new()
                {
                    "It is untarnished by time, and connected to my companion by some mystery.",
                    "Click to summon the Effigy to you."
                },
                heldup = "You received the Enameled Star Crest! It can be used to summon the Effigy to accompany you on your adventures. Check the relics journal to view further details.",
            };

            // ====================================================================
            // Cleric Bat Relic

            relics[IconData.relics.minister_mitre.ToString()] = new()
            {
                title = "Mitre Of The Mountain",
                relic = IconData.relics.minister_mitre,
                line = RelicsData.relicsets.minister,
                description = "Worn by the high cleric of bats.",
                details = new()
                {
                    "The wearer adopted the customs and zealotry of those that once guarded the sacred mountain spring.",
                },
                heldup = "You received the Mitre of the Mountain! The energies of the weald might know more about this artifact. Check the relics journal to view further details.",
            };

            // ====================================================================
            // Bronze disc

            relics[IconData.relics.avalant_disc.ToString()] = new()
            {
                title = "Ancient Bronze Disc",
                relic = IconData.relics.avalant_disc,
                line = RelicsData.relicsets.avalant,
                function = true,
                description = "The base of an ancient device, fished out of the waters of the local forest.",
                details = new()
                {
                    "1st of 6",
                },
                heldup = "You received a Ancient Bronze Disc! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            relics[IconData.relics.avalant_chassis.ToString()] = new()
            {
                title = "Ancient Bronze Chassis",
                relic = IconData.relics.avalant_chassis,
                line = RelicsData.relicsets.avalant,
                function = true,
                description = "The structural core of an ancient device, fished out of the mountain lake.",
                details = new()
                {
                    "2nd of 6",
                },
                heldup = "You received a Ancient Bronze Chassis! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            relics[IconData.relics.avalant_gears.ToString()] = new()
            {
                title = "Ancient Bronze Gears",
                relic = IconData.relics.avalant_gears,
                line = RelicsData.relicsets.avalant,
                function = true,
                description = "A mechanical component fished out of the local beach.",
                details = new()
                {
                    "3rd of 6",
                },
                heldup = "You received a set of Ancient Bronze Gears! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            relics[IconData.relics.avalant_casing.ToString()] = new()
            {
                title = "Ancient Bronze Casement",
                relic = IconData.relics.avalant_casing,
                line = RelicsData.relicsets.avalant,
                function = true,
                description = "The top casement of an ancient device, found in the cavern waters.",
                details = new()
                {
                    "4th of 6",
                },
                heldup = "You received a Ancient Bronze Casement! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            relics[IconData.relics.avalant_needle.ToString()] = new()
            {
                title = "Ancient Bronze Needle",
                relic = IconData.relics.avalant_needle,
                line = RelicsData.relicsets.avalant,
                function = true,
                description = "A measuring needle fished out of the town river.",
                details = new()
                {
                    "5th of 6",
                },
                heldup = "You received a Strange Bronze Needle! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            relics[IconData.relics.avalant_measure.ToString()] = new()
            {
                title = "Ancient Bronze Measure",
                relic = IconData.relics.avalant_measure,
                line = RelicsData.relicsets.avalant,
                function = true,
                description = "A measuring wheel found in the waters of a secluded atoll.",
                details = new()
                {
                    "6th of 6",
                },
                heldup = "You received an Ancient Bronze Measure! The servants of the Lady Beyond the Shore might know more about this artifact.",
            };

            return relics;

        }

        public void RelicFunction(string id)
        {

            Relic relic = reliquary[id];

            switch (relic.relic)
            {

                case IconData.relics.effigy_crest:

                    if (!Context.IsMainPlayer)
                    {

                        break;

                    }

                    if (Mod.instance.characters.ContainsKey(Data.CharacterData.characters.Effigy))
                    {

                        List<StardewDruid.Character.Character.mode> reservedModes = new() { Character.Character.mode.scene, Character.Character.mode.track, };

                        if (!reservedModes.Contains(Mod.instance.characters[Data.CharacterData.characters.Effigy].modeActive))
                        {

                            Mod.instance.characters[Data.CharacterData.characters.Effigy].SwitchToMode(Character.Character.mode.track, Game1.player);

                            Mod.instance.CastMessage("The Effigy has joined you on your adventures");

                            Game1.activeClickableMenu.exitThisMenu();

                        }

                    }

                    break;

            }

        }

        public IconData.relics RelicsFishQuest()
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

        public int ProgressFishQuest(bool update = false)
        {
            int avalant = 0;

            foreach (IconData.relics relic in new List<IconData.relics>()
            {

                IconData.relics.avalant_disc,
                IconData.relics.avalant_chassis,
                IconData.relics.avalant_gears,
                IconData.relics.avalant_casing,
                IconData.relics.avalant_needle,
                IconData.relics.avalant_measure,

            })
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

                    avalant++;

                }

            }

            return avalant;

        }


    }

    public class Relic
    {

        // -----------------------------------------------
        // journal

        public string title;

        public IconData.relics relic = IconData.relics.none;

        public RelicsData.relicsets line = RelicsData.relicsets.none;

        public bool function;

        public bool activatable;

        public string description;

        public string heldup;

        public List<string> details = new();

    }

}
