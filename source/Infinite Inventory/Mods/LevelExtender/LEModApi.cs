/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/unidarkshin/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;

namespace LevelExtender
{
    public class LEModApi
    {
        public ModEntry ME;

        public LEModApi(ModEntry me)
        {
            ME = me;
        }

        public ModEntry GetMod()
        {
            return ME;
        }

        //This value will offset spawn-rate by the specified amount (1 second intervals)
        public double overSR = -1.0;

        public void Spawn_Rate(double osr)
        {
            overSR = osr;
        }

        /*public int[] CurrentXP()
        {
            return ME.GetCurXP();
        }

        public int[] RequiredXP()
        {
            return ME.GetReqXP();
        }*/

        public event EventHandler<EXPEventArgs> OnXPChanged
        {
            add => ME.LEE.OnXPChanged += value;
            remove => ME.LEE.OnXPChanged -= value;
        }

        //*** SKILL COMPATIBILITY ***//

        //Please use this function ONCE in the Save Loaded event (to be safe, PLEASE ADD A 5 SECOND DELAY BEFORE initialization):
        //
        //FORMAT:
        //initializeSkill(string <name of skill>, int <current xp value>, double? <xp_modifier, default = 1.0>, (optional) List<int> <current xp table>, (optional) int[] <numerical game categories the skill is related to, allows for LE skill modifiers/buffs>)
        //
        //EXAMPLE:
        //initializeSkill("cooking", 1305, 1.0, new List<int>() {100, 1000, 2000, 5000, 10000}, new int[] {-4 (fishing cat.), -105 (custom cat.)})
        public int initializeSkill(string name, int xp, double? xp_mod = null, List<int> xp_table = null, int[] cats = null)
        {
            return ME.initializeSkill(name, xp, xp_mod, xp_table, cats);
        }

        //Any requests to change a skill MUST FIRST check the current value of said variable and assign it to the corresponding variable
        //in your mod. (If this is not done, the skill variables across mods may not be synced)
        //
        //For getting data from a skill, use this parameter format:
        //args = {"get", "<name of skill>", "<name of variable>"} -> Example: args = {"get", "cooking", "xp"}
        //
        //For setting skill data, use this parameter format:
        //args = {"set", "<name of skill>", "<name of variable>", "<string representation of value>"} -> Example: args = {"set", "cooking", "level", "25"}
        public dynamic TalkToSkill(string[] args)
        {
            return ME.TalkToSkill(args);
        }

    }
}
