/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewValley;
using DeluxeJournal.Events;
using DeluxeJournal.Framework.Serialization;

namespace DeluxeJournal.Tasks
{
    /// <summary>Task interface.</summary>
    /// <remarks>Note: This should not be implemented directly. Please subclass from TaskBase.</remarks>
    public interface ITask : IComparable<ITask>
    {
        /// <summary>Renew periods.</summary>
        public enum Period
        {
            Never,
            Daily,
            Weekly,
            Monthly,
            Annually
        }

        /// <summary>Uniquely identifies the class type for serialization.</summary>
        string ID { get; }

        /// <summary>The name of this task.</summary>
        string Name { get; set; }

        /// <summary>False if waiting for renewal, true otherwise.</summary>
        bool Active { get; set; }

        /// <summary>Is this task complete?</summary>
        /// <remarks>Note: Tasks marked as completed are removed at the end of the day if the period is "Never".</remarks>
        bool Complete { get; set; }

        /// <summary>Renew period. Tasks will be renewed at the end of the period.</summary>
        /// /// <remarks>Note: Tasks marked as completed are removed at the end of the day if the period is "Never".</remarks>
        [JsonConverter(typeof(StringEnumConverter))]
        Period RenewPeriod { get; set; }

        /// <summary>The reference date for the renew period.</summary>
        [JsonConverter(typeof(WorldDateConverter))]
        WorldDate RenewDate { get; set; }

        /// <summary>Generic value. Typically used for storing localized names.</summary>
        string TargetDisplayName { get; set; }

        /// <summary>Generic value.</summary>
        string TargetName { get; set; }

        /// <summary>Generic value.</summary>
        int TargetIndex { get; set; }

        /// <summary>Generic value. Typically used for storing a meta-type on another generic value.</summary>
        int Variant { get; set; }

        /// <summary>Current count. Used for progress.</summary>
        int Count { get; set; }

        /// <summary>Target count. Used for progress</summary>
        int MaxCount { get; set; }

        /// <summary>Starting price value.</summary>
        int BasePrice { get; set; }

        /// <summary>Days remaining until renew.</summary>
        int DaysRemaining();

        /// <summary>The total cost to complete this task at the current state.</summary>
        int GetPrice();

        /// <summary>Get the sorting index. Used to preserve ordering among tasks in the same state.</summary>
        int GetSortingIndex();

        /// <summary>Set the sorting index. Used to preserve ordering among tasks in the same state.</summary>
        void SetSortingIndex(int index);

        /// <summary>Dismiss the pulsing check after completion.</summary>
        void MarkAsViewed();

        /// <summary>Has this task been marked as viewed?</summary>
        bool HasBeenViewed();

        /// <summary>Show a progress bar on the task entry if true. Progress is defined as Count divided by MaxCount.</summary>
        bool ShouldShowProgress();

        /// <summary>Subscribe hook.</summary>
        void EventSubscribe(ITaskEvents events);

        /// <summary>Unsubscribe hook.</summary>
        void EventUnsubscribe(ITaskEvents events);
    }
}
