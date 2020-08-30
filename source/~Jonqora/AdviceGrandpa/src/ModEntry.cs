using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace AdviceGrandpa
{
    public class ModEntry : Mod
    {
        internal ModConfig config;
        internal ITranslationHelper i18n => Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            string startingMessage = i18n.Get("template.start", new { mod = helper.ModRegistry.ModID, folder = helper.DirectoryPath });
            Monitor.Log(startingMessage, LogLevel.Trace);

            config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            e.Button.TryGetKeyboard(out Keys keyPressed);

            if (keyPressed.Equals(config.debugKey))
                Monitor.Log(i18n.Get("template.key"), LogLevel.Info);
        }
    }
}
