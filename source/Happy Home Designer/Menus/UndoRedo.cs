/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Framework;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace HappyHomeDesigner.Menus
{
	public partial class UndoRedoButton<T> : ClickableComponent where T : IUndoRedoState<T>
	{
		private readonly Stack<T> backwards = new();
		private readonly Stack<T> forwards = new();

		/// <summary>Apply a new state</summary>
		public void Push(T state)
		{
			forwards.Clear();
			backwards.Push(state);
		}

		public bool Undo(bool playSound)
			=> Do(backwards, forwards, playSound, false);

		public bool Redo(bool playSound)
			=> Do(forwards, backwards, playSound, true);

		private static bool Do(Stack<T> from, Stack<T> to, bool playSound, bool forward)
		{
			if (from.Count is 0)
				return false;

			var state = from.Pop();

			if (state.Apply(forward))
			{
				if (playSound)
					Game1.playSound("Cowboy_gunshot");

				to.Push(state);
				return true;
			}

			ModEntry.monitor.Log((forward ? "Redo" : "Undo") + " failed: could not apply.");
			from.Push(state);
			return false;
		}

		public void Clear()
		{
			backwards.Clear();
			forwards.Clear();
		}
	}
}
