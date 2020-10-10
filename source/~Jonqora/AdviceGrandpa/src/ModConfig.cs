/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Jonqora/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace AdviceGrandpa
{
    class ModConfig
    {
        public SButton debugKey { get; set; }

        public ModConfig()
        {
            debugKey = SButton.J;
        }
    }
}
