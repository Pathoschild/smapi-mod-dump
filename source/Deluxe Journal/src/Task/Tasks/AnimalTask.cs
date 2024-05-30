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
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Task;

using static DeluxeJournal.Task.TaskParameterAttribute;

namespace DeluxeJournal.Task.Tasks
{
    internal class AnimalTask : TaskBase
    {
        public class Factory : TaskFactory
        {
            [TaskParameter(TaskParameterNames.FarmAnimal, TaskParameterTag.FarmAnimalList)]
            public IList<string>? FarmAnimalTypes { get; set; }

            [TaskParameter(TaskParameterNames.Count, TaskParameterTag.Count, Constraints = Constraint.GE1)]
            public int Count { get; set; } = 1;

            public override SmartIconFlags EnabledSmartIcons => SmartIconFlags.Animal;

            public override bool EnableSmartIconCount => true;

            public override void Initialize(ITask task, ITranslationHelper translation)
            {
                if (task is AnimalTask animalTask)
                {
                    FarmAnimalTypes = animalTask.FarmAnimalTypes;
                    Count = animalTask.MaxCount;
                }
            }

            public override ITask? Create(string name)
            {
                return FarmAnimalTypes != null && FarmAnimalTypes.Count > 0
                    ? new AnimalTask(name, FarmAnimalTypes, Count)
                    : null;
            }
        }

        /// <summary>
        /// The farm animal types associated with a shop name. The animal types are the internal names
        /// used as the keys in <see cref="Game1.farmAnimalData"/>.
        /// </summary>
        public IList<string> FarmAnimalTypes { get; set; }

        /// <summary>Serialization constructor.</summary>
        public AnimalTask() : base(TaskTypes.Animal)
        {
            FarmAnimalTypes = Array.Empty<string>();
        }

        public AnimalTask(string name, IList<string> farmAnimalTypes, int count) : base(TaskTypes.Animal, name)
        {
            FarmAnimalTypes = farmAnimalTypes;
            MaxCount = count;

            if (Game1.farmAnimalData.TryGetValue(farmAnimalTypes.First(), out var data))
            {
                BasePrice = data.PurchasePrice * 2;
            }
        }

        public override bool ShouldShowProgress()
        {
            return MaxCount > 1;
        }

        public override void EventSubscribe(ITaskEvents events)
        {
            events.FarmAnimalPurchased += OnFarmAnimalPurchased;
        }

        public override void EventUnsubscribe(ITaskEvents events)
        {
            events.FarmAnimalPurchased -= OnFarmAnimalPurchased;
        }

        private void OnFarmAnimalPurchased(object? sender, FarmAnimalEventArgs e)
        {
            if (CanUpdate() && IsTaskOwner(e.Player) && FarmAnimalTypes.Contains(e.FarmAnimalType))
            {
                IncrementCount();
            }
        }
    }
}
