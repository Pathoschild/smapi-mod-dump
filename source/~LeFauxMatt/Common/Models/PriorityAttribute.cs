/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

#if IS_FAUXCORE
namespace StardewMods.FauxCore.Common.Models;
#else
namespace StardewMods.Common.Models;
#endif

/// <summary>Represents an attribute used to specify the priority of a subscriber method.</summary>
[AttributeUsage(AttributeTargets.Method)]
public class PriorityAttribute : Attribute
{
    /// <summary>Initializes a new instance of the <see cref="PriorityAttribute" /> class.</summary>
    /// <param name="priority">The priority level for the subscriber.</param>
    public PriorityAttribute(int priority) => this.Priority = priority;

    public int Priority { get; private set; }
}