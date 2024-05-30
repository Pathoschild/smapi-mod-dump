/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using StardewModdingAPI;
using IJsonAssetsApi = MoonShared.APIs.IJsonAssetsApi;
using BirbCore.Attributes;
using CookingSkill.Core;
using MoonShared.APIs;

namespace CookingSkill
{
    public class ModEntry : Mod
    {
        [SMod.Instance]
        internal static ModEntry Instance;
        internal static Config Config;
        internal static Assets Assets;

        internal static bool JALoaded => ModEntry.Instance.Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets");
        internal static bool BCLoaded => ModEntry.Instance.Helper.ModRegistry.IsLoaded("leclair.bettercrafting");

        internal static IJsonAssetsApi JsonAssets;
        internal static IBetterCrafting BetterCrafting;

        internal ITranslationHelper I18n => this.Helper.Translation;


        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Parser.ParseAll(this);
        }

        public override object GetApi()
        {
            try
            {
                return new CookingAPI();
            }
            catch
            {
                return null;
            }

        }
    }
}
