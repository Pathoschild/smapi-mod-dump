/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using BZP_Allergies.Config;
using StardewModdingAPI;

namespace BZP_Allergies
{
    internal abstract class Initializable
    {
        protected internal static IMonitor Monitor;
        protected internal static IGameContentHelper GameContent;
        protected internal static IModContentHelper ModContent;

        // call in the Entry class
        public static void Initialize(IMonitor monitor,
            IGameContentHelper gameContentHelper, IModContentHelper modContentHelper)
        {
            Monitor = monitor;
            GameContent = gameContentHelper;
            ModContent = modContentHelper;
        }
    }
}
