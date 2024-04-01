/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-EqualMoneySplit
**
*************************************************/

namespace EqualMoneySplit.Models
{
    /// <summary>
    /// Type of event triggering an action
    /// </summary>
    public enum EventContext
    {
        None,
        InventoryChanged,
        EndOfDay
    }
}
