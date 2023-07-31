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
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewArchipelago.Goals;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;

namespace StardewArchipelago.Items
{
    public class BabyBirther
    {
        private static readonly string[] _babyNames = new[]
        {
            "Albrekka", "Alchav", "axe_y", "beauxq", "Berserker", "Black Sliver", "blastron", "CaitSith2", "Celeste", "Damocles",
            "dewin", "el_", "Espeon", "eudaimonistic", "Exempt-Medic", "Farrak Kilhn", "Figment", "Fly Sniper",
            "Frazzleduck", "Heinermann", "JaredWeakStrike", "Jarno", "jat2980", "Jouramie", "Justice", "Kaito Kid",
            "KelioMZX", "Kono Tyran", "lordlou", "Magnemania", "Marech", "Mati", "Mav", "mewlif", "N00byKing", "Phar",
            "Porygone", "ProfBytes", "RaspberrySpaceJam", "Rosalie", "Salzkorn", "Scipio", "Sneaki", "Snow", "SunnyBat",
            "TheCondor", "toaster", "Trev", "Violet", "Yellow_Meep", "zig", "Ziktofel",
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
                Position = new Vector2(16f, 4f) * 64f + new Vector2(0.0f, -24f)
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
