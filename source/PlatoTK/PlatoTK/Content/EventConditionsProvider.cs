/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlatoTK.Content
{
    internal class EventConditionsProvider : IConditionsProvider
    {
        public string Id => "Default";

        public bool CanHandleConditions(string conditions)
        {
            return true;
        }

        public bool CheckConditions(string conditions, object caller)
        {
            GameLocation location = caller is GameLocation gl ? gl : Game1.currentLocation;

            if (location == null)
                return false;

            var m = AccessTools.Method(typeof(GameLocation),"checkEventPrecondition");
            return (int)m.Invoke(location, new string[] { ("9999999/" + conditions) }) > 0;
        }
    }
}
