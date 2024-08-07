/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace CustomSpousePatioRedux
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;
        public int MaxSpousesPerPage { get; set; } = 6;
        public SButton PatioWizardKey { get; set; } = SButton.F8;
    }
}
