/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

using MoreMultiplayerInfo.EventHandlers;
using MoreMultiplayerInfo.Helpers;
using StardewModdingAPI;
using StardewValley;

namespace MoreMultiplayerInfo
{
    public class ShowPlayerIconHandler
    {
        private readonly IMonitor _monitor;
        private readonly IModHelper _helper;
        private readonly ReadyCheckHandler _readyCheckHandler;
        
        private PlayerIconMenu _iconMenu;

        public ShowPlayerIconHandler(IMonitor monitor, IModHelper helper)
        {
            _monitor = monitor;
            _helper = helper;

            _readyCheckHandler = new ReadyCheckHandler(monitor, helper);

            _iconMenu = new PlayerIconMenu(_readyCheckHandler, monitor, helper);
            _iconMenu.PlayerIconClicked += PlayerIconClicked;
            StardewModdingAPI.Events.SaveEvents.AfterLoad += SaveEvents_AfterLoad;
        }

        private void SaveEvents_AfterLoad(object sender, System.EventArgs e)
        {
            if (Context.IsMultiplayer || !ConfigHelper.GetOptions().HideInSinglePlayer)
            {
                Game1.onScreenMenus.Add(_iconMenu);
            }
        }

        private void PlayerIconClicked(object sender, PlayerIconClickedArgs input)
        {
            var player = PlayerHelpers.GetPlayerWithUniqueId(input.PlayerId);

            Game1.activeClickableMenu = new PlayerInformationMenu(player.UniqueMultiplayerID, _helper);


        }

    }
}
