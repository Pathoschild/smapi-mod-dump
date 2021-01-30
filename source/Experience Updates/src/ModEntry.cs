/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Quipex/ExperienceUpdates
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace ExperienceUpdates
{
    public class ModEntry : Mod
    {
        private const int NUMBER_OF_SKILLS = 6;
        private ExperienceCalculator calculator;
        private TextRenderer renderer;
        public static Configuration Config;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<Configuration>();

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            helper.Events.Display.RenderedHud += this.OnRenderedHud;
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            Monitor.Log("Went to title, stopping");
            Calculator().Stop();
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            Monitor.Log("Save loaded, resetting counters");
            Calculator().Reset();
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            var textsToRender = Calculator().GetUpdatableTexts();
            Renderer().Render(textsToRender, NUMBER_OF_SKILLS);
        }

        private ExperienceCalculator Calculator()
        {
            if (calculator == null)
            {
                calculator = new ExperienceCalculator(Monitor);
            }
            return calculator;
        }

        private TextRenderer Renderer()
        {
            if (renderer == null)
            {
                renderer = new TextRenderer();
            }
            return renderer;
        }
    }
}
