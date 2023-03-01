/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JhonnieRandler/TVBrasileira
**
*************************************************/

using StardewModdingAPI;
using TVBrasileira.frameworks;
using TVBrasileira.channels;

namespace TVBrasileira
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            var configMenu = new CreateMenu(helper, ModManifest, Monitor);
            var ednaldoPereira = new EdnaldoPereira(helper, Monitor);
            var palmirinha = new Palmirinha(helper, Monitor);
            var globoRural = new GloboRural(helper, Monitor);
        }
    }
}