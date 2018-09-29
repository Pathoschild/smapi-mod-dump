using StardewModdingAPI;
using StardewModdingAPI.Events;
using PelicanPostalService.Config;
using PelicanPostalService.Framework.Menu;
using PelicanPostalService.Framework.Player;
using StardewValley;

namespace PelicanPostalService
{
    public class ModEntry : Mod
    {
        private ModConfig config;

        public override void Entry(IModHelper arg)
        {
            config = arg.ReadConfig<ModConfig>();
            ControlEvents.KeyPressed += ControlEvents_KeyPressed;
        }

        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (Context.IsWorldReady)
            {
                if (e.KeyPressed.ToString() == config.MenuAccessKey)
                {
                    QuestData.Monitor = Monitor;
                
                    ActiveItem activeItem = new ActiveItem(Game1.player.ActiveObject);

                    PostalService postalService = new PostalService(activeItem, config.AllowQuestSubmissions);
                    postalService.Open(Game1.activeClickableMenu, Game1.player.CurrentTool);
                }
            }
        }
    }
}