using MoreMultiplayerInfo.EventHandlers;
using MoreMultiplayerInfo.Helpers;
using StardewModdingAPI;
using StardewValley;
using System.Linq;

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
            if ((Context.IsMultiplayer || !ConfigHelper.GetOptions().HideInSinglePlayer) && Game1.onScreenMenus.All(t => t.GetType() != typeof(PlayerIconMenu)))
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
