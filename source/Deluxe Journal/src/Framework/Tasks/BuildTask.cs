/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Buildings;
using DeluxeJournal.Events;
using DeluxeJournal.Tasks;

namespace DeluxeJournal.Framework.Tasks
{
    internal class BuildTask : TaskBase
    {
        public class Factory : DeluxeJournal.Tasks.TaskFactory
        {
            [TaskParameter("building", Tag = "building")]
            public string BuildingType { get; set; } = string.Empty;

            [TaskParameter("cost", Tag = "cost", Hidden = true)]
            public int Cost { get; set; } = 0;

            public override string? SmartIconName()
            {
                return BuildingType;
            }

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                BuildingType = task.TargetDisplayName;
                Cost = task.BasePrice;
            }

            public override ITask? Create(string name)
            {
                return new BuildTask(name, BuildingType, Cost);
            }
        }

        /// <summary>Serialization constructor.</summary>
        public BuildTask() : base(TaskTypes.Build)
        {
        }

        public BuildTask(string name, string buildingType, int cost) : base(TaskTypes.Build, name)
        {
            TargetDisplayName = buildingType;
            BasePrice = cost;
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.ModEvents.World.BuildingListChanged += OnBuildingListChanged;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.ModEvents.World.BuildingListChanged -= OnBuildingListChanged;
        }

        private void OnBuildingListChanged(object? sender, BuildingListChangedEventArgs e)
        {
            if (!CanUpdate())
            {
                return;
            }

            foreach (Building building in e.Added)
            {
                if (building.buildingType.Value == TargetDisplayName || (building.isCabin && TargetDisplayName == "Cabin"))
                {
                    MarkAsCompleted();
                    return;
                }
            }
        }
    }
}
