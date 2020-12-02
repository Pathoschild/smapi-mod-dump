/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JacquePott/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace MassProduction
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the mass producer identification string set for this object, or null if none exists.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string GetMassProducerKey(this SObject o)
        {
            return ModEntry.MPMManager.GetUpgradeKey(o);
        }

        /// <summary>
        /// Sets the mass producer identification string for this object.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="s"></param>
        public static void SetMassProducerKey(this SObject o, string s)
        {
            ModEntry.MPMManager.OnMachineUpgradeKeyChanged(o, s);
        }
    }
}
