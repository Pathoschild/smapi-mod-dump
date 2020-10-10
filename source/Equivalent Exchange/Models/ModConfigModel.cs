/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MercuriusXeno/EquivalentExchange
**
*************************************************/

using Microsoft.Xna.Framework.Input;

namespace EquivalentExchange.Models
{
    class ConfigurationModel
    {
        public string TransmuteKey { get; set; }
        public string NormalizeKey { get; set; }
        public bool DisableRecipeItems { get; set; }

        public ConfigurationModel()
        {
            TransmuteKey = Keys.V.ToString();
            NormalizeKey = Keys.N.ToString();
            DisableRecipeItems = false;
        }
    }
}