/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

using MoreMultiplayerInfo.Helpers;
using StardewModdingAPI;

namespace MoreMultiplayerInfo
{
    public class ModEntry : Mod
    {
        private ModEntryHelper _baseHandler;

        public override void Entry(IModHelper helper)
        {
            ConfigHelper.Helper = helper;

            _baseHandler = new ModEntryHelper(Monitor, helper);
        }
    }
}
