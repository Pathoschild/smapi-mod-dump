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

namespace DeluxeJournal.Task
{
    /// <summary>Task interface.</summary>
    /// <remarks>Note: This should not be implemented directly. Please inherit from <see cref="TaskBase"/>.</remarks>
    public interface ITask : IComparable<ITask>
    {
        /// <summary>Renew periods.</summary>
        public enum Period
        {
            Never,
            Daily,
            Weekly,
            Monthly,
            Annually,
            Custom
        }

        /// <summary>Uniquely identifies the class type for serialization.</summary>
        [JsonProperty(Order = -5)]
        string ID { get; }

        /// <summary>The name of this task.</summary>
        [JsonProperty(Order = -4)]
        string Name { get; set; }

        /// <summary><c>false</c> if waiting for renewal, <c>true</c> otherwise.</summary>
        [JsonProperty(Order = -3)]
        bool Active { get; set; }

        /// <summary>Is this task complete?</summary>
        /// <remarks>Note: Tasks marked as completed are removed at the end of the day if the period is "Never".</remarks>
        [JsonProperty(Order = -2)]
        bool Complete { get; set; }

        /// <summary>UMID of the player that owns this task.</summary>
        [JsonIgnore]
        long OwnerUMID { get; set; }

        /// <summary>Renew period. Tasks will be renewed at the end of the period.</summary>
        /// <remarks>Note: Tasks marked as completed are removed at the end of the day if set to <see cref="Period.Never"/>.</remarks>
        [JsonConverter(typeof(StringEnumConverter))]
        Period RenewPeriod { get; set; }

        /// <summary>The reference date for the renewal period.</summary>
        [JsonConverter(typeof(WorldDateConverter))]
        WorldDate RenewDate { get; set; }

        /// <summary>The renew interval (in days) for a <see cref="Period.Custom"/> renewal period.</summary>
        int RenewCustomInterval { get; set; }

        /// <summary>Current count. Used for tracking progress.</summary>
        int Count { get; set; }

        /// <summary>Target count. Used for tracking progress.</summary>
        int MaxCount { get; set; }

        /// <summary>Starting price value.</summary>
        int BasePrice { get; set; }

        /// <summary>Create a copy of this task.</summary>
        /// <remarks>Ensure any mutable data is deep copied.</remarks>
        ITask Copy();

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

        /// <summary>Show a custom status on the task entry if true. Status is obtained via GetCustomStatusKey().</summary>
        bool ShouldShowCustomStatus();

        /// <summary>Get the translation key for current status of the task.</summary>
        string GetCustomStatusKey();

        /// <summary>Does this player own this task?</summary>
        bool IsTaskOwner(Farmer player);

        /// <summary>Does the player with this UMID own this task?</summary>
        bool IsTaskOwner(long umid);

        /// <summary>Subscribe hook.</summary>
        void EventSubscribe(ITaskEvents events);

        /// <summary>Unsubscribe hook.</summary>
        void EventUnsubscribe(ITaskEvents events);

        /// <summary>Validate the state of this task.</summary>
        /// <remarks>
        /// Called when the state of a task may be invalidated, such as after making changes via the
        /// options menu or after the start of a new day.
        /// </remarks>
        void Validate();
    }
}
