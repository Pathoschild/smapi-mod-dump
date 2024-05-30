/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using System;

namespace HappyHomeDesigner.Framework
{
	public interface IUndoRedoState<T> : IEquatable<T> where T : IUndoRedoState<T>
	{
		public bool Apply(bool forward);
	}
}
