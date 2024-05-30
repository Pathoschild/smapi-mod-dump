/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lisyce/SDV_Allergies_Mod
**
*************************************************/

using StardewModdingAPI;

namespace BarleyCore
{
    internal sealed class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            
        }

        public override object GetApi(IModInfo mod)
        {
            return new BarleyCoreApi(mod);
        }
    }
}