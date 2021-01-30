/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Newtonsoft.Json.Linq;
using QuestFramework.Offers;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.ContentPacks.Model
{
    internal class Content : ITranslatable<Content>
    {
        public ISemanticVersion Format { get; set; }
        public List<QuestData> Quests { get; set; }
        public List<QuestOffer<JObject>> Offers { get; set; } = new List<QuestOffer<JObject>>();
        public List<CustomBoardData> CustomBoards { get; set; } = new List<CustomBoardData>();
        public List<CustomDropBoxData> CustomDropBoxes { get; set; }
        internal IContentPack Owner { get; set; }

        public Content Translate(ITranslationHelper translation)
        {
            return new Content()
            {
                Format = this.Format,
                Quests = this.Quests?.Select(q => q.Translate(translation)).ToList(),
                Offers = this.Offers?.Select(o => TranslationUtils.TranslateOffer(translation, o)).ToList(),
                CustomBoards = this.CustomBoards?.Select(b => TranslationUtils.Translate(translation, b)).ToList(),
                CustomDropBoxes = this.CustomDropBoxes?.Select(b => TranslationUtils.Translate(translation, b)).ToList(),
                Owner = this.Owner,
            };
        }
    }
}
