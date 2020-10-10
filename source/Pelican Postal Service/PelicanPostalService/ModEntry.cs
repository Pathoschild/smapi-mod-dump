/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TobinWilson/PelicanPostalService
**
*************************************************/

using Pelican.Config;
using Pelican.Menus;
using Pelican.Items;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Pelican
{
    public class ModEntry : Mod
    {
        private IModHelper helper;

        public override void Entry(IModHelper helper)
        {
            this.helper = helper;
            ControlEvents.KeyPressed += ControlEvents_KeyPressed;
        }

        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (Context.IsWorldReady)
            {
                Meta.Config = helper.ReadConfig<ModConfig>();
                if (e.KeyPressed.ToString().Equals(Meta.Config.MenuAccessKey))
                {
                    Meta.Console = Monitor;
                    Meta.Lang = helper.Translation;

                    ItemHandler itemHandler = new ItemHandler();
                    PostalService menu = new PostalService(itemHandler);
                    menu.Open();
                }
            }
        }
    }
}