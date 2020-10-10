/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/maxvollmer/LeftyMode
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Reflection;

namespace LeftyMode
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            harmony.Patch(
               original: AccessTools.Method(typeof(Mouse), nameof(Mouse.GetState)),
               postfix: new HarmonyMethod(typeof(LeftyMouse), nameof(LeftyMouse.GetState))
            );
        }
    }
}
