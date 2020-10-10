/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

namespace Igorious.StardewValley.DynamicAPI.Extensions
{
    public static class BoolExtensions
    {
        public static string Serialize(this bool e)
        {
            return e.ToString().ToLower();
        }
    }
}
