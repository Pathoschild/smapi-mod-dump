/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Cobalt
**
*************************************************/

using Harmony;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;
using Cobalt.Framework;

namespace Cobalt
{
    internal class ModEntry : Mod
    {
        public static ModEntry instance;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;

            Items.init(helper.Events);

            helper.Content.AssetEditors.Add(new ItemInjector());
            helper.Content.AssetEditors.Add(new CobaltInjector());
            Game1.ResetToolSpriteSheet();

            var harmony = HarmonyInstance.Create("spacechase0.Cobalt");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        /// <summary>Get an API that other mods can access. This is always called after <see cref="Entry" />.</summary>
        public override object GetApi()
        {
            return new CobaltApi();
        }
    }
}
