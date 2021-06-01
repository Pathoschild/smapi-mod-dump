/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using QuestEssentials.Framework;
using QuestEssentials.Messages;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Tasks
{
    class TileActionTask : QuestTask<TileActionTask.TileActionData>
    {
        public struct TileActionData
        {
            public string Location { get; set; }
            public Point Tile { get; set; }
            public string ItemAcceptedContextTags { get; set; }
            public string ItemRequiredMessage { get; set; }
            public string Message { get; set; }
            public string StartEvent { get; set; }
            public bool ConsumeItem { get; set; }
        }

        public override bool OnCheckProgress(IStoryMessage message)
        {
            if (this.IsCompleted())
                return false;

            if (message is TileActionMessage tileActionMessage && this.IsWhenMatched())
            {
                if (this.Data.Location != tileActionMessage.Location?.Name || this.Data.Tile != tileActionMessage.TilePosition)
                    return false;

                if (this.Data.ItemAcceptedContextTags == null)
                {
                    this.IncrementCount(this.Goal);

                    if (this.Data.Message != null)
                    {
                        Game1.drawObjectDialogue(this.Data.Message);
                    }

                    return true;
                }

                Farmer farmer = tileActionMessage.Farmer;

                if (Helper.CheckItemContextTags(farmer.ActiveObject, this.Data.ItemAcceptedContextTags))
                {
                    int requiredAmount = this.Goal - this.CurrentCount;
                    int donatedAmount = Math.Min(farmer.ActiveObject.Stack, requiredAmount);

                    if (donatedAmount < requiredAmount)
                    {
                        if (this.Data.ItemRequiredMessage != null)
                            Game1.drawObjectDialogue(string.Format(this.Data.ItemRequiredMessage, requiredAmount));

                        return true;
                    }

                    if (this.Data.ConsumeItem)
                    {
                        farmer.ActiveObject.Stack -= donatedAmount;

                        if (farmer.ActiveObject.Stack <= 0)
                        {
                            farmer.ActiveObject = null;
                            farmer.showNotCarrying();
                        }
                    }

                    this.IncrementCount(donatedAmount);

                    if (this.Data.StartEvent != null)
                        tileActionMessage.Location.StartEventFrom(this.Data.StartEvent);
                    else if (this.Data.Message != null)
                        Game1.drawObjectDialogue(this.Data.Message);

                    return true;
                }
            }

            return false;
        }

        public override bool ShouldShowProgress()
        {
            return this.Data.ItemAcceptedContextTags != null;
        }
    }
}
