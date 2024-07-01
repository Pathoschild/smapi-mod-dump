/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ArsVenefici.Framework.Util;
using SpaceCore;
using SpaceShared;
using SpaceShared.ConsoleCommands;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;

namespace ArsVenefici.Framework.Commands
{
    public class ToggleWizardryCommand : Command
    {

        public ToggleWizardryCommand(ModEntry modEntry) : base(modEntry) 
        {

        }

        public void ToggleWizardry(string command, string[] args)
        {
            bool value;

            string s = $"{ModEntry.ArsVenificiContentPatcherId}_MagicAltar";

            if (bool.TryParse(args[0], out value))
            {
                if (value)
                {
                    if(Game1.player.GetCustomSkillLevel(ModEntry.Skill) < 1)
                    {
                        Game1.player.AddCustomSkillExperience(ModEntry.Skill, ModEntry.Skill.ExperienceCurve[0]);
                        modEntry.FixManaPoolIfNeeded(Game1.player, overrideWizardryLevel: 1);
                    }
                    else if (Game1.player.GetCustomSkillLevel(ModEntry.Skill) >= 1)
                    {
                        modEntry.FixManaPoolIfNeeded(Game1.player, overrideWizardryLevel: Game1.player.GetCustomSkillLevel(ModEntry.Skill));
                    }

                    Game1.player.eventsSeen.Add(modEntry.LearnedWizardryEventId.ToString());
                    
                    CraftingRecipe craftingRecipe = new CraftingRecipe(s);
                   
                    if (!Game1.player.craftingRecipes.Keys.Contains(s))
                        Game1.player.craftingRecipes.Add(s, 0);

                    modEntry.Monitor.Log("You have learned wizardry!", LogLevel.Info);
                }
                else if (value == false)
                {
                    Game1.player.SetMaxMana(0);
                    Game1.player.eventsSeen.Remove(modEntry.LearnedWizardryEventId.ToString());

                    //if (Game1.player.craftingRecipes.ContainsKey(s))
                    //    Game1.player.craftingRecipes.Remove(s);

                    modEntry.Monitor.Log("You have forgotten wizardry!", LogLevel.Info);
                }
            }
        }
    }
}
