/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/SAML
**
*************************************************/

global using Object = StardewValley.Object;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using SAML.Utilities;
using StardewValley;
using SAML.Menus;

namespace SAML
{
    internal class ModEntry : Mod
    {
        internal static IMonitor IMonitor;
        internal static IModHelper IHelper;
        internal static PerScreen<EventManager> EventManager = new(() => new EventManager());

        public override void Entry(IModHelper helper)
        {
            IMonitor = Monitor;
            IHelper = Helper;

            Game1.game1.Window.TextInput += (_, e) =>
            {
                if (Game1.activeClickableMenu is Menu m)
                    m.TextInputReceived(e);
            };
        }
    }
}
