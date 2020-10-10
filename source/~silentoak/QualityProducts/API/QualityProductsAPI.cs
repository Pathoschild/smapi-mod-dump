/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/silentoak/StardewMods
**
*************************************************/

namespace SilentOak.QualityProducts.API
{
    public class QualityProductsAPI : IQualityProductsAPI
    {
        public QualityProductsConfig Config { get; }

        public QualityProductsAPI(QualityProductsConfig config)
        {
            Config = config;
        }
    }
}