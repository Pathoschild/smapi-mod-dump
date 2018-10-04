using StardewModdingAPI;
using StardewModdingAPI.Events;
using Project.Config;
using Project.Framework.Menus;
using Project.Framework.Player.Items;
using Project.Logging;
using StardewValley;

namespace Project
{
    public class ModEntry : Mod
    {
        private ModConfig settings;

        public override void Entry(IModHelper modHelper)
        {
            settings = modHelper.ReadConfig<ModConfig>();
            ControlEvents.KeyPressed += ControlEvents_KeyPressed;
        }

        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (Context.IsWorldReady)
            {
                if (e.KeyPressed.ToString() == settings.MenuAccessKey)
                {
                    Debug.Console = Monitor;
                    ItemHandler itemDetails = new ItemHandler(Game1.player.ActiveObject);
                    PostalService menu = new PostalService(itemDetails, settings);
                    menu.Open(Game1.activeClickableMenu, Game1.player.CurrentTool);
                }
            }
        }
    }
}