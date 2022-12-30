/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.OrdinaryCapsule.Framework.Models;

using System.Collections;
using System.Collections.Generic;
using StardewMods.Common.Integrations.OrdinaryCapsule;

/// <inheritdoc />
internal sealed class CapsuleItems : IList<ICapsuleItem>
{
    private readonly List<ICapsuleItem> _capsuleItems = new();

    /// <inheritdoc />
    public int Count => this._capsuleItems.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public ICapsuleItem this[int index]
    {
        get => this._capsuleItems[index];
        set => this._capsuleItems[index] = value;
    }

    /// <inheritdoc />
    public void Add(ICapsuleItem item)
    {
        this._capsuleItems.Add(item);
    }

    /// <inheritdoc />
    public void Clear()
    {
        this._capsuleItems.Clear();
    }

    /// <inheritdoc />
    public bool Contains(ICapsuleItem item)
    {
        return this._capsuleItems.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(ICapsuleItem[] array, int arrayIndex)
    {
        this._capsuleItems.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public IEnumerator<ICapsuleItem> GetEnumerator()
    {
        return this._capsuleItems.GetEnumerator();
    }

    /// <inheritdoc />
    public int IndexOf(ICapsuleItem item)
    {
        return this._capsuleItems.IndexOf(item);
    }

    /// <inheritdoc />
    public void Insert(int index, ICapsuleItem item)
    {
        this._capsuleItems.Insert(index, item);
    }

    /// <inheritdoc />
    public bool Remove(ICapsuleItem item)
    {
        return this._capsuleItems.Remove(item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        this._capsuleItems.RemoveAt(index);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this._capsuleItems.GetEnumerator();
    }
}