/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace GenericModConfigMenu.ModOption
{
    internal class ParagraphModOption : BaseModOption
    {

        public override void SyncToMod()
        {
        }

        public override void Save()
        {
        }

        public ParagraphModOption(string paragraph, IManifest mod)
            : base(paragraph, "", paragraph, mod)
        {
        }
    }
}
