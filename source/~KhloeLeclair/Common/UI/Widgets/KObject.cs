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
using System.Text;

using Leclair.Stardew.Common.Extensions;
using Leclair.Stardew.Common.Types;

namespace Leclair.Stardew.Common.UI.Widgets;

public class KObject {

	#region Fields

	private string? _ObjectName;

	protected internal KObject? _Parent;

	protected internal OrderedSet<KObject>? _Children;

	#endregion

	#region Life Cycle

	public KObject(KObject? parent = null) {
		Children = new(this);
		if (parent is not null)
			Parent = parent;
	}

	#endregion

	#region Identity

	/// <summary>
	/// The name of this object.
	/// </summary>
	public string? ObjectName {
		get => _ObjectName;
		set {
			if (_ObjectName != value) {
				string? oldName = _ObjectName;
				_ObjectName = value;
				ObjectNameChanged?.SafeInvoke(this, new ObjectNameChangedEventArgs(oldName, _ObjectName));
			}
		}
	}

	/// <summary>
	/// This event is fired whenever the <see cref="ObjectName"/> changes.
	/// </summary>
	public event EventHandler<ObjectNameChangedEventArgs>? ObjectNameChanged;

	#endregion

	#region Hierarchy

	/// <summary>
	/// A sorted set of <see cref="KObject"/>s that are children of
	/// this object.
	/// </summary>
	public KChildSet Children { get; }

	/// <summary>
	/// The parent <see cref="KObject"/> of this object. May be null if this
	/// object has no parent.
	/// </summary>
	public KObject? Parent {
		get => _Parent;
		set {
			// The logic for changing parents and children is all located
			// within the KChildList class for consistency.
			if (value is null)
				_Parent?.Children.Remove(this);
			else
				value.Children.Add(this);
		}
	}

	/// <summary>
	/// The root <see cref="KObject"/>. May be this object if this object has
	/// no parent object.
	/// </summary>
	public KObject RootObject {
		get {
			KObject @object = this;

			while (@object._Parent is not null)
				@object = @object._Parent;

			return @object;
		}
	}

	#endregion

	#region Hierarchy Events

	protected internal virtual void OnParentChanged(KObject? oldParent, KObject? newParent) {

	}

	#endregion

	#region Traversal

	public T? FindChild<T>(string? name, bool recursive = true) where T : KObject {
		if (_Children is null)
			return null;

		foreach (KObject child in _Children) {
			if (child is T tchild && (name is null || child.ObjectName == name))
				return tchild;
		}

		if (recursive)
			foreach (KObject child in _Children) {
				T? result = child.FindChild<T>(name, recursive);
				if (result is not null)
					return result;
			}

		return null;
	}

	public KObject? FindChild(string? name, bool recursive = true) {
		if (_Children is null)
			return null;

		foreach (KObject child in _Children) {
			if (name is null || child.ObjectName == name)
				return child;
		}

		if (recursive)
			foreach (KObject child in _Children) {
				KObject? result = child.FindChild(name, recursive);
				if (result is not null)
					return result;
			}

		return null;
	}

	public IEnumerable<T> FindChildren<T>(string? name, bool recursive = true) where T : KObject {
		if (_Children is null)
			yield break;

		foreach(KObject child in _Children) {
			if (child is T tchild && (name is null || child.ObjectName == name))
				yield return tchild;
		}

		if (recursive)
			foreach (KObject child in _Children) {
				foreach(T tchild in child.FindChildren<T>(name, recursive))
					yield return tchild;
			}
	}

	public IEnumerable<KObject> FindChildren(string? name, bool recursive = true) {
		if (_Children is null)
			yield break;

		foreach (KObject child in _Children) {
			if (name is null || child.ObjectName == name)
				yield return child;
		}

		if (recursive)
			foreach (KObject child in _Children) {
				foreach (KObject kchild in child.FindChildren(name, recursive))
					yield return kchild;
			}
	}

	#endregion

	#region Event Classes

	public class ObjectNameChangedEventArgs : EventArgs {

		public string? OldName { get; }
		public string? NewName { get; }

		public ObjectNameChangedEventArgs(string? oldName, string? newName) {
			OldName = oldName;
			NewName = newName;
		}
	}

	#endregion

}
