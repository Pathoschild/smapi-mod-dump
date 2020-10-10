/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GuiNoya/SVMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace DailyTasksReport.Tasks
{
    public class SimpleTaskItem
    {
        public GameLocation Location { get; set; }
        public Vector2 Position { get; set; }
    }

    public class TaskItem<TObject> : SimpleTaskItem
    {
        public TaskItem(GameLocation location, Vector2 position, TObject @object)
        {
            Location = location;
            Position = position;
            Object = @object;
        }

        public TObject Object { get; set; }
    }
}