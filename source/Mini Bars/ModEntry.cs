/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Mini-Bars
**
*************************************************/

using StardewModdingAPI;
using MiniBars.Framework.Rendering;
using MiniBars.Framework;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewValley;

namespace MiniBars
{
    public class ModEntry : Mod
    {
        public static ModEntry instance;
        public static Config config;

        public override void Entry(IModHelper helper)
        {
            instance = this;
            config = helper.ReadConfig<Config>();

            Textures.LoadTextures();

            helper.Events.Display.RenderedWorld += Renderer.OnRendered;
            helper.ConsoleCommands.Add("minibars_theme", "Change the bars theme.\nUsage: minibars_theme '1 or 2'", Commands.Theme);
            helper.ConsoleCommands.Add("minibars_showfull", "Enable or disable showing healthbars when monsters full health.\nUsage: minibars_showfull 'true or false'", Commands.ShowFullLife);
            helper.ConsoleCommands.Add("minibars_range", "Enable or disable the range verification.\nUsage: minibars_range 'true or false'", Commands.RangeVerification);
        }
    }
}
