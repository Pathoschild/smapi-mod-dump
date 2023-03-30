/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using CommunityToolkit.Diagnostics;

namespace AtraCore.Framework.DialogueManagement;

/// <summary>
/// A dialogue to delay.
/// </summary>
public readonly struct DelayedDialogue : IComparable<DelayedDialogue>, IEquatable<DelayedDialogue>
{
    private readonly int time;
    private readonly Dialogue dialogue;
    private readonly NPC npc;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelayedDialogue"/> struct.
    /// </summary>
    /// <param name="time">Time to delay to.</param>
    /// <param name="dialogue">Dialogue to delay.</param>
    /// <param name="npc">Speaking NPC.</param>
    public DelayedDialogue(int time, Dialogue dialogue, NPC npc)
    {
        Guard.IsNotNull(dialogue);
        Guard.IsNotNull(npc);

        this.time = time;
        this.dialogue = dialogue;
        this.npc = npc;
    }

    #region operators

    public static bool operator ==(DelayedDialogue left, DelayedDialogue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DelayedDialogue left, DelayedDialogue right)
    {
        return !(left == right);
    }

    public static bool operator <(DelayedDialogue left, DelayedDialogue right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(DelayedDialogue left, DelayedDialogue right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(DelayedDialogue left, DelayedDialogue right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(DelayedDialogue left, DelayedDialogue right)
    {
        return left.CompareTo(right) >= 0;
    }

    #endregion

    /// <inheritdoc />
    public int CompareTo(DelayedDialogue other)
        => this.time - other.time;

    /// <inheritdoc />
    public bool Equals(DelayedDialogue other)
        => this.time == other.time && this.npc.Name == other.npc.Name && this.dialogue.dialogues.SequenceEqual(other.dialogue.dialogues);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is DelayedDialogue dialogue && this.Equals(dialogue);

    /// <summary>
    /// Pushes the delayed dialogue onto the NPC's stack if it's past time to do so..
    /// </summary>
    /// <param name="currenttime">The current in-game time.</param>
    /// <returns>True if pushed, false otherwise.</returns>
    public bool PushIfPastTime(int currenttime)
    {
        if (currenttime >= this.time)
        {
            this.npc.CurrentDialogue.Push(this.dialogue);
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            const int factor = -0x5AAA_AAD7;

            int ret = (EqualityComparer<int>.Default.GetHashCode(this.time) * factor) + EqualityComparer<string>.Default.GetHashCode(this.npc.Name);
            foreach (string? s in this.dialogue.dialogues)
            {
                ret *= factor;
                ret += EqualityComparer<string>.Default.GetHashCode(s);
            }

            return ret;
        }
    }
}