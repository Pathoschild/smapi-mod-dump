/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Skill;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpaceCore.Skills;

namespace ArsVenefici.Framework.Commands
{
    internal class SpellPartsCommand : Command
    {

        public SpellPartsCommand(ModEntry modEntry) : base(modEntry)
        {

        }

        public void LearnSpellPart(string command, string[] args)
        {
            string value = args[0];

            if (modEntry.spellPartSkillManager.spellPartSkills.ContainsKey(value))
            {
                SpellPartSkillHelper helper = SpellPartSkillHelper.Instance();

                string spellPartNameText = modEntry.Helper.Translation.Get($"spellpart.{value}.name");

                helper.Learn(modEntry, Game1.player, value);

                modEntry.Monitor.Log($"You learned the spell part {spellPartNameText}!", LogLevel.Info);
            }
            else
            {
                modEntry.Monitor.Log($"The name {value} is not a valid spell part id!", LogLevel.Info);
            }
            
        }

        public void LearnAllSpellParts(string command, string[] args)
        {
            SpellPartSkillHelper helper = SpellPartSkillHelper.Instance();
            helper.LearnAll(modEntry, Game1.player);

            modEntry.Monitor.Log($"You learned all the spell parts!", LogLevel.Info);
        }

        public void ForgetSpellPart(string command, string[] args)
        {
            string value = args[0];

            if (modEntry.spellPartSkillManager.spellPartSkills.ContainsKey(value))
            {
                SpellPartSkillHelper helper = SpellPartSkillHelper.Instance();

                string spellPartNameText = modEntry.Helper.Translation.Get($"spellpart.{value}.name");

                helper.Forget(modEntry, Game1.player, value);

                modEntry.Monitor.Log($"You forgot the spell part {spellPartNameText}!", LogLevel.Info);
            }
            else
            {
                modEntry.Monitor.Log($"The name {value} is not a valid spell part id!", LogLevel.Info);
            }
        }

        public void ForgetAllSpellParts(string command, string[] args)
        {
            SpellPartSkillHelper helper = SpellPartSkillHelper.Instance();
            helper.ForgetAll(modEntry, Game1.player);

            modEntry.Monitor.Log($"You forgot all the spell parts!", LogLevel.Info);
        }

        public void KnowsSpellPart(string command, string[] args)
        {
            string value = args[0];

            if (modEntry.spellPartSkillManager.spellPartSkills.ContainsKey(value))
            {
                SpellPartSkillHelper helper = SpellPartSkillHelper.Instance();

                string spellPartNameText = modEntry.Helper.Translation.Get($"spellpart.{value}.name");

                if (helper.Knows(modEntry, Game1.player, value))
                {
                    modEntry.Monitor.Log($"You know the spell part {spellPartNameText}!", LogLevel.Info);
                }
                else
                {
                    modEntry.Monitor.Log($"You do not know the spell part {spellPartNameText}!", LogLevel.Info);
                }
            }
            else
            {
                modEntry.Monitor.Log($"The name {value} is not a valid spell part id!", LogLevel.Info);
            }
        }
    }
}
