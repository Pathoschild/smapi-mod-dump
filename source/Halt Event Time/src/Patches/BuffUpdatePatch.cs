/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chawolbaka/HaltEventTime
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using HarmonyLib;


namespace HaltEventTime.Patches
{
    [HarmonyPatch(typeof(Buff), "update")]
    public class BuffUpdatePatch
    {
        public static bool Disable = true;
        public static bool Prefix(ref bool __result, ref GameTime time)
        {
            //这边return true代表不阻止源方法
            if (!Disable)
                __result = false;
            return Disable;
        }
    }
}
