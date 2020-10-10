/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using System.ComponentModel;
using TehPers.CoreMod.Api.Json;

namespace TehPers.CoreMod.Config {
    [JsonDescribe]
    internal class ModConfig {

        [Description("When crafting items, allows multiple recipes to be crafted at once, storing all the results under the cursor.")]
        public bool AllowMultiCrafting { get; set; } = false;
    }
}