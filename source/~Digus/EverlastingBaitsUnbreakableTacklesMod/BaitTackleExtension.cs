/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Digus/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EverlastingBaitsAndUnbreakableTacklesMod
{
    public static class BaitTackleExtension
    {
        public static string GetDescription(this BaitTackle value)
        {
            var field = value.GetType().GetField(value.ToString());

            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetQuestName(this BaitTackle value)
        {

            return "Quest" + value.ToString();
        }

        public static BaitTackle? GetFromDescription(string value)
        {
            foreach (BaitTackle baitTackle in Enum.GetValues(typeof(BaitTackle)))
            {
                if (baitTackle.GetDescription().Equals(value))
                {
                    return baitTackle;
                }
            }
            return null;
        }
    }
}
