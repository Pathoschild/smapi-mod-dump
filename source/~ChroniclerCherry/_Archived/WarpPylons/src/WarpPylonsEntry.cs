/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ChroniclerCherry/stardew-valley-mods
**
*************************************************/

using StardewModdingAPI;

namespace WarpPylons
{
    public partial class WarpPylonsEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            PylonsManager.Initialize(Monitor,Helper);

            Helper.ConsoleCommands.Add("Pylons","Opens the Warp Pylons menu",this.OpenWarPylonsMenuCommand);
        }
    }
}
