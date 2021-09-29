/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.ComponentModel;

namespace DynamicGameAssets.PackData
{
    public class GiftTastePackData : BasePackData
    {
        public string ObjectId { get; set; }
        public string Npc { get; set; }

        public int Amount { get; set; }

        [DefaultValue(null)]
        public string NormalTextTranslationKey { get; set; }

        [DefaultValue(null)]
        public string BirthdayTextTranslationKey { get; set; }

        [DefaultValue(null)]
        public int? EmoteId { get; set; }
    }
}
