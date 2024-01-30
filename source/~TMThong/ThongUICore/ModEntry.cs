/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using ThongUICore.Framework.Manager;

namespace ThongUICore
{
    public class ModEntry : Mod
    {
        internal static Config config;
        internal ITranslationHelper i18n => Helper.Translation;

        public override void Entry(IModHelper helper)
        {
            config = helper.ReadConfig<Config>();
            ThongUIManager.ModHelper = helper;
            ThongUIManager.Monitor = this.Monitor;
        }
    }
}
