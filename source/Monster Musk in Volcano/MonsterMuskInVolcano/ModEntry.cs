/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/BananaFruit1492/MonsterMuskInVolcano
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

namespace YourProjectName
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public static readonly int MonsterMuskBuffInteger = 24;

        public static readonly string VolcanoRockCrabName = "False Magma Cap";

        public static readonly Dictionary<string, int> VolcanoBatNameToMineLevel = new Dictionary<string, int>
        {
            {"Magma Sprite", -555},
            {"Magma Sparker", -556}
        };

        private static readonly ISet<int> MuskedVolcanoDungeonsToday = new HashSet<int>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += PlayerWarped;
            helper.Events.GameLoop.DayStarted += NewDay;
        }

        private void NewDay(object sender, DayStartedEventArgs e)
        {
            ModEntry.MuskedVolcanoDungeonsToday.Clear();
        }

        private void PlayerWarped(object sender, WarpedEventArgs e)
        {
            if(e.NewLocation is VolcanoDungeon dungeon)
            {
                var level = dungeon.level;
                var num_farmers_currently_in_dungeon = dungeon.farmers.Count;
                var entering_player_has_musk_buff = e.Player.hasBuff(ModEntry.MonsterMuskBuffInteger);
                // Only add monsters if musk is active, there is no other player in the level and level's monsters haven't been cloned yet today
                if (entering_player_has_musk_buff && !ModEntry.MuskedVolcanoDungeonsToday.Contains(level) && num_farmers_currently_in_dungeon <= 1)
                {
                    ModEntry.MuskedVolcanoDungeonsToday.Add(level);
                    this.DoubleMonstersInVolcanoDungeon(dungeon);
                }
            }
        }

        /// <summary>
        /// Clones all monsters in the given volcano dungeon and places them at the same place as their original
        /// </summary>
        /// <param name="dungeon"></param>
        private void DoubleMonstersInVolcanoDungeon(VolcanoDungeon dungeon)
        {
            var original_characters = new NetCollection<NPC>(dungeon.characters);
            foreach(var character in original_characters)
            {
                // exclude player, dwarf and whatever else might creep up here
                if(character is Monster monster)
                {
                    this.CloneVolcanoDungeonMonster(dungeon, monster);
                }
            }
        }

        /// <summary>
        /// Clones the given monster and puts it on the same place as the original
        /// </summary>
        /// <param name="dungeon"></param>
        /// <param name="monster"></param>
        private void CloneVolcanoDungeonMonster(VolcanoDungeon dungeon, Monster monster)
        {
            Monster clone = null;
            var originalPosition = monster.Position;
            
            if(monster is Duggy)
            {
                clone = new Duggy(originalPosition, magmaDuggy: true);
            }
            else if(monster is RockCrab)
            {
                clone = new RockCrab(originalPosition, ModEntry.VolcanoRockCrabName);
            }
            else if(monster is Bat bat)
            {
                var batName = bat.Name;
                if (ModEntry.VolcanoBatNameToMineLevel.ContainsKey(batName))
                {
                    clone = new Bat(originalPosition, ModEntry.VolcanoBatNameToMineLevel[batName]);
                }
            }
            else if (monster is LavaLurk)
            {
                clone = new LavaLurk(originalPosition);
            }
            else if(monster is HotHead)
            {
                clone = new HotHead(originalPosition);
            }
            else if(monster is GreenSlime)
            {
                var greenSlime = new GreenSlime(originalPosition, 0);
                greenSlime.makeTigerSlime();
                clone = greenSlime;
            }
            else if(monster is Spiker spike)
            {
                return;
            }
            if(clone == null)
            {
                this.Monitor.Log($"Cloning of monster \"{monster.Name}\" is not supported.", LogLevel.Warn);
            }
            else
            {
                dungeon.addCharacter(clone);
            }
        }
    }
}