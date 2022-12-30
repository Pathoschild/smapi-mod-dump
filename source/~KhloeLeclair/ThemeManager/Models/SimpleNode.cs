/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leclair.Stardew.ThemeManager.Models;

internal record SimpleNode<T> {

	public T Value { get; }

	public SimpleNode<T>? Parent { get; }

	public List<SimpleNode<T>> Children { get; } = new();

	public SimpleNode(T value, SimpleNode<T>? parent) {
		Value = value;
		Parent = parent;
	}

}
