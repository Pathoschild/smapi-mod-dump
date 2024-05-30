/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chiccenFL/StardewValleyMods
**
*************************************************/

using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarbageCanTweaks
{
    public partial class ModEntry
    {
        public static string ApplyLootChance(string condition)
        {
            if (string.IsNullOrEmpty(condition)) return condition;
            Log($"Condition before: {condition}", debugOnly: true);
            string[] conditions = condition.Split(',');
            bool replaced = false;
            string random = string.Empty;
            string chance = string.Empty;
            foreach (string str in conditions)
            {
                if (str.Contains("RANDOM"))
                {
                    random = str;
                    condition = condition.Replace(random, string.Empty);
                }
            }
            if (string.IsNullOrEmpty(random)) return condition;
            string[] condition1 = random.Split(" ");
            foreach (string word in condition1)
            {
                if (word.Contains('.'))
                {
                    chance = (float.Parse(word) * Config.LootChance).ToString();
                    random = random.Replace(word, chance);
                    replaced = true;
                    break;
                }
            }
            string newCondition = (condition.TrimEnd().EndsWith(',')) ? condition + $"{random}" : condition + $", {random}";
            Log($"Condition after: {newCondition}", debugOnly: true);
            return (replaced) ? newCondition : condition;
        }
    }
}
