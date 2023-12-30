/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewArchipelago.Goals;
using StardewArchipelago.Items.Traps;
using StardewValley;
using StardewValley.Characters;

namespace StardewArchipelago.Items
{
    public class BabyBirther
    {
        private static readonly string[] _babyNames = new[]
        {
            "Albrekka", "Alchav", "Angela", "axe_y", "beauxq", "Berserker", "Black Sliver", "blastron", "BootsinSoots", "CaitSith2", "Captain Canadia", "Celeste", "Damocles",
            "dewin", "el_", "emmalution", "Espeon", "eudaimonistic", "Exempt-Medic", "Farrak Kilhn", "Figment", "Fly Sniper",
            "Frazzleduck", "Heinermann", "Ishigh", "JaredWeakStrike", "Jarno", "jat2980", "jennnifer", "Jouramie", "Justice", "Kaito Kid",
            "KelioMZX", "KittyAshy", "Kono Tyran", "Lady Redacted", "lordlou", "Magnemania", "Marech", "Margot", "Mati", "Mav", "MeleneV", "mewlif",
            "Noca", "N00byKing"/*, "NinoKito"*/, "Phar",
            "PoryGone", "ProfBytes", "RaspberrySpaceJam", "Rosalie", "Salzkorn", "SamanthaCoffin", "Scipio", "Sneaki", "Snow", "Sraw", "SunnyBat",
            "TheCondor", "toaster", "Trev", "Violet", "WhySoThirstyy", "Yellow_Meep", "zig", "Ziktofel",
        };

        public void SpawnNewBaby()
        {
            var seed = (int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed);
            var random = new Random(seed);
            var babyGender = random.NextDouble() < 0.5;
            var babyColor = random.NextDouble() < 0.5;
            var babyName = ChooseBabyName(random);

            var baby = new Child(babyName, babyGender, babyColor, Game1.player)
            {
                Age = 0,
                Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f),
            };
            Utility.getHomeOfFarmer(Game1.player).characters.Add(baby);
            Game1.playSound("smallSelect");
            var spouse = Game1.player.getSpouse();
            if (Game1.player.getChildrenCount() >= 2)
            {
                GoalCodeInjection.CheckFullHouseGoalCompletion();
            }

            if (spouse != null)
            {
                spouse.shouldSayMarriageDialogue.Value = true;
                spouse.currentMarriageDialogue.Insert(0, new MarriageDialogueReference("Data\\ExtraDialogue", "NewChild_Adoption", true, babyName));
            }
        }

        public void SpawnTemporaryBaby(int seedOffset)
        {
            var seed = (int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed) + seedOffset;
            var random = new Random(seed);
            var babyGender = random.NextDouble() < 0.5;
            var babyColor = random.NextDouble() < 0.5;
            var babyName = ChooseBabyName(random);

            var currentMap = Game1.currentLocation;
            var tile = currentMap.getRandomTile() * 64f;
            var age = random.Next(4);
            var baby = new TemporaryBaby(babyName, babyGender, babyColor, Game1.player, age)
            {
                Position = tile,
            };
            Game1.currentLocation.characters.Add(baby);
        }

        private string ChooseBabyName(Random random)
        {
            var npcNames = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions").Keys.ToHashSet();
            foreach (var npc in Utility.getAllCharacters())
            {
                npcNames.Add(npc.Name);
            }
            string babyName;
            var maxAttempts = _babyNames.Length * 10;
            var attempt = 0;
            do
            {
                attempt++;
                babyName = _babyNames[random.Next(0, _babyNames.Length)];
                if (attempt >= maxAttempts)
                {
                    while (npcNames.Contains(babyName))
                    {
                        babyName += " ";
                    }
                }
            }
            while (npcNames.Contains(babyName));

            return babyName;
        }
    }
}
