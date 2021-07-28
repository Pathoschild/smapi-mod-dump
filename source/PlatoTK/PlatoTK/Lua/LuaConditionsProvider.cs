/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using PlatoTK.Content;
using System.Collections.Generic;

namespace PlatoTK.Lua
{
    internal class LuaConditionsProvider : IConditionsProvider
    {
        public string Id => "Lua";

        private readonly IPlatoHelper Helper;

        public LuaConditionsProvider(IPlatoHelper helper)
        {
            Helper = helper;
        }

        public bool CanHandleConditions(string trigger)
        {
            return trigger == "L#";
        }

        public bool CheckConditions(string conditions, object caller)
        {
            Dictionary<string, object> dict = null;
            if (caller != null)
                dict = new Dictionary<string, object>() { { "caller", caller } };

            return Helper.Lua.CallLua<bool>(conditions.Substring("L# ".Length),dict);
        }
    }
}
