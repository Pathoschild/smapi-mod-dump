/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using System.Collections.Generic;

namespace Survivalistic.Framework.Databases
{
    public class Tools
    {
        public static Dictionary<string, string> GetToolDatabase()
        {
            Dictionary<string, string> holder = new Dictionary<string, string>();

            holder.Add("Axe", "0.25/0.5");
            holder.Add("Pickaxe", "0.25/0.5");
            holder.Add("Hoe", "0.25/0.5");
            holder.Add("Scythe", "0.1/0.2");
            holder.Add("Fishing Rod", "0.15/0.3");
            holder.Add("Watering Can", "0.1/0.2");
            holder.Add("Shears", "0.15/0.3");
            holder.Add("Milk Pail", "0.15/0.3");

            return holder;
        }
    }
}
