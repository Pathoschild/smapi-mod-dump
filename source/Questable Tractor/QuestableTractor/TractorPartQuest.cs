/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NermNermNerm/StardewMods
**
*************************************************/

using StardewValley;

namespace NermNermNerm.Stardew.QuestableTractor
{
    public abstract class TractorPartQuest<TQuestState> : BaseQuest<TQuestState> where TQuestState : struct
    {
        protected TractorPartQuest(TractorPartQuestController<TQuestState> controller) : base(controller) { }

        public new TractorPartQuestController<TQuestState> Controller => (TractorPartQuestController<TQuestState>)base.Controller;

        public abstract void GotWorkingPart(Item workingPart);

        public override bool IsItemForThisQuest(Item item)
        {
            return item.ItemId == this.Controller.BrokenAttachmentPartId || item.ItemId == this.Controller.WorkingAttachmentPartId;
        }
    }
}
