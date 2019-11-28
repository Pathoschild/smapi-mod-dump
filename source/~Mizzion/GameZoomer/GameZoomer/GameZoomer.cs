using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace GameZoomer
{
    public class GameZoomer : Mod
    {
        private GameZoomConfig _config;
        private bool debugging = false;

        //Mods Entry method
        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<GameZoomConfig>();

            //Helper Events
            helper.Events.Input.ButtonPressed += ButtonPressed;
        }
        
        //Method that runs when buttons are pressed.
        private void ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if(e.IsDown(_config.ZoomInKey) || e.IsDown(_config.ZoomButtonIn))
                IncreaseZoom();
            if(e.IsDown(_config.ZoomOutKey) || e.IsDown(_config.ZoomButtonOut))
                DecreaseZoom();
        }

        private void IncreaseZoom()
        {
            Game1.options.zoomLevel += .05f;
            /*
            if (Game1.options.zoomLevel > 1.25f)
                Game1.options.zoomLevel = 1.25f;*/
            Program.gamePtr.refreshWindowSettings();
            if(debugging)
                Monitor.Log($"Current Zoom: {Game1.options.zoomLevel}");

        }

        private void DecreaseZoom()
        {
            Game1.options.zoomLevel -= .05f;
            /*
            if (Game1.options.zoomLevel < 0.35f)
                Game1.options.zoomLevel = 0.35f;*/
            Program.gamePtr.refreshWindowSettings();
            if (debugging)
                Monitor.Log($"Current Zoom: {Game1.options.zoomLevel}");
        }
    }
}
