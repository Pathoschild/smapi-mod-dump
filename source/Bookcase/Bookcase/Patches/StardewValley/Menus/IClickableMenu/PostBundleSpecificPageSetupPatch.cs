/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bookcase.Events;

namespace Bookcase.Patches {
    /// <summary>
    /// Code run everytime a bundle is 'setup', essentially allowing you the ability to inject information into an already created bundle's display.
    /// </summary>
    public class PostBundleSpecificPageSetupPatch : IGamePatch {
        public Type TargetType => typeof(JunimoNoteMenu);

        public MethodBase TargetMethod => TargetType.GetMethod("setUpBundleSpecificPage", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Postfix(JunimoNoteMenu __instance, Bundle b) {
            BookcaseEvents.PostBundleSpecificPageSetup.Post(new PostBundleSetupEvent(__instance.ingredientList, __instance.ingredientSlots, __instance.inventory, b));
        }
    }
}
