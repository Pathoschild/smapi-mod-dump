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
using Newtonsoft.Json;
using QuestEssentials.Framework;
using QuestEssentials.Messages;
using QuestEssentials.Quests.Messages;
using StardewValley;

namespace QuestEssentials.Tasks
{
    class EnterSpotTask : QuestTask<EnterSpotTask.EnterSpotData>
    {
        public struct EnterSpotData
        {
            [JsonConverter(typeof(PointConverter))]
            public Point? Tile { get; set; }
            [JsonConverter(typeof(RectangleConverter))]
            public Rectangle? Area { get; set; }
            public string Location { get; set; }
            public string EventOnComplete { get; set; }
        }

        protected override void OnTaskComplete()
        {
            if (this.Data.EventOnComplete == null)
                return;

            if (this.Data.Location == null || Game1.player.currentLocation.Name != this.Data.Location)
                return;

            Game1.player.currentLocation.StartEventFrom(this.Data.EventOnComplete);
        }

        public override bool ShouldShowProgress()
        {
            return false;
        }

        public override bool OnCheckProgress(IStoryMessage message)
        {
            if (message.Trigger != "PlayerMoved" || !this.IsWhenMatched() || this.IsCompleted())
                return false;

            if (message is PlayerMovedMessage movedMessage)
            {
                if (movedMessage.Location.Name != this.Data.Location)
                    return false;

                if (this.Data.Tile.HasValue && this.Data.Tile.Value == movedMessage.TilePosition)
                {
                    this.IncrementCount(this.Count);
                    return true;
                }

                if (this.Data.Area.HasValue && this.Data.Area.Value.Contains((int)movedMessage.Position.X, (int)movedMessage.Position.Y))
                {
                    this.IncrementCount(this.Count);
                    return true;
                }
            }

            return false;
        }
    }
}
