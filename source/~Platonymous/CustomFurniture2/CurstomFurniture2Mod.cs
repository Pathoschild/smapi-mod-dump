/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace CustomFurniture2
{
    public class CurstomFurniture2Mod : Mod
    {
        internal Config config;
        internal ITranslationHelper i18n => Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            string startingMessage = i18n.Get("template.start", new { mod = helper.ModRegistry.ModID, folder = helper.DirectoryPath });
            Monitor.Log(startingMessage, LogLevel.Trace);

            config = helper.ReadConfig<Config>();

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
