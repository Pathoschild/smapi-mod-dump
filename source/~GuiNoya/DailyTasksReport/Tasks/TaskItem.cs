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