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

using Microsoft.Xna.Framework;

using StardewValley;

using Leclair.Stardew.Common.Extensions;

namespace Leclair.Stardew.Common.UI.Widgets;

/// <summary>
/// The KWidget class is the base class of all UI objects.
/// </summary>
public class KWidget : KObject {

	#region Fields

	private bool _Enabled;

	private bool _Visible;

	private KMargins _ContentMargins;

	private bool _PositionDirty;

	private Point _Position;

	private bool _SizeDirty;

	private KSize _Size;

	#endregion

	#region Life Cycle

	public KWidget(KObject? parent = null) : base(parent: parent) {

	}

	#endregion

	#region Mode Properties

	/// <summary>
	/// Whether or not the widget is enabled.
	///
	/// Setting <see cref="Enabled"/> to <c>true</c> makes the widget enabled
	/// if all its parent widgets are enabled, up to the top-level widget. If
	/// one of its parents is not enabled, the widget will not be enabled even
	/// when explicitly enabled.
	///
	/// Setting <see cref="Enabled"/> to <c>false</c> disables a widget
	/// explicitly. An explicitly disabled widget will never become enabled,
	/// even if all its parent widgets are enabled.
	///
	/// By default, this property has a value of <c>true</c>.
	/// </summary>
	public bool Enabled {
		get {
			// If we are explicitly disabled, return false now.
			if (!_Enabled)
				return false;

			// Iterate through every parent. If we have a parent that
			// is disabled, then we are disabled.
			KObject? obj = Parent;
			while (obj is not null) {
				if (obj is KWidget widget && !widget._Enabled)
					return false;
				obj = obj.Parent;
			}

			// If we didn't have any disabled parents, we're enabled.
			return true;
		}
		set {
			if (_Enabled != value) {
				bool oldEnabled = Enabled;
				_Enabled = value;
				if (Enabled != oldEnabled) {
					// TODO: Dispatch events.
				}
			}
		}
	}

	/// <summary>
	/// Whether or not the widget is visible.
	///
	/// Setting <see cref="Visible"/> to <c>true</c> makes the widget visible
	/// if all its parent widgets are visible, up to the top-level widget. If
	/// one of its parents is not visible, the widget will not be visible even
	/// when explicitly shown.
	///
	/// If its size or position have changed, a widget will receive move and
	/// resize events before it is shown. If a widget has not been resized
	/// yet, the widget's size will be set appropriately using the
	/// <see cref="AdjustSize()"/> method.
	///
	/// Setting <see cref="Visible"/> to <c>false</c> hides a widget explicitly.
	/// An explicitly hidden widget will never become visible, even if all its
	/// parent widgets are visible.
	///
	/// By default, this property has a value of <c>true</c>.
	/// </summary>
	public bool Visible {
		get {
			// If we are explicitly not visible, return false now.
			if (!_Visible)
				return false;

			// Iterate through every parent. If we have a parent that
			// is not visible, then we are not visible.
			KObject? obj = Parent;
			while(obj is not null) {
				if (obj is KWidget widget && !widget._Visible)
					return false;
				obj = obj.Parent;
			}

			// If we didn't have any invisible parents, we're visible.
			return true;
		}
		set {
			if (_Visible != value) {
				bool oldVisible = Visible;
				_Visible = value;
				if (Visible != oldVisible) {
					// TODO: Dispatch events.
				}
			}
		}
	}

	#endregion

	#region Mode Methods

	/// <summary>
	/// Returns whether or not a widget is enabled all the way up until a
	/// specified parent. This can be used to determine if enabling a
	/// specific parent would cause this widget to become enabled.
	///
	/// This returns true if this widget has not been explicitly disabled,
	/// and no parents of this widget up to the <paramref name="other"/>
	/// object (but excluding that object) have been explicitly disabled.
	/// </summary>
	/// <param name="other">The parent object to check until.</param>
	public bool IsEnabledTo(KObject other) {
		// If we are explicitly disabled, return false now.
		if (!_Enabled)
			return false;

		// Iterate through every parent until we encounter the provided widget.
		// If we have a parent that is disabled, then we are disabled.
		KObject? obj = Parent;
		while (obj is not null && obj != other) {
			if (obj is KWidget widget && !widget._Enabled)
				return false;
			obj = obj.Parent;
		}

		// We did not encounter a disabled parent, so we are enabled.
		return true;
	}

	/// <summary>
	/// Returns whether or not a widget is visible all the way up until a
	/// specified parent. This can be used to determine if showing a
	/// specific parent would cause this widget to become visible.
	///
	/// This returns true if this widget has not been explicitly hidden,
	/// and no parents of this widget up to the <paramref name="other"/>
	/// object (but excluding that object) have been explicitly hidden.
	/// </summary>
	/// <param name="other">The parent object to check until.</param>
	public bool IsVisibleTo(KObject other) {
		// If we are explicitly hidden, return false now.
		if (!_Visible)
			return false;

		// Iterate through every parent until we encounter the provided widget.
		// If we have a parent that is hidden, then we are hidden.
		KObject? obj = Parent;
		while (obj is not null && obj != other) {
			if (obj is KWidget widget && !widget._Visible)
				return false;
			obj = obj.Parent;
		}

		// We did not encounter an invisible parent, so we are visible.
		return true;
	}

	#endregion

	#region Geometry Properties

	/// <summary>
	/// The X position of the widget within its parent widget.
	///
	/// If the widget is a top-level widget, the position is that of the
	/// widget within <see cref="Game1.uiViewport"/>, including its frame.
	/// 
	/// This value is equal to the <see cref="Point.X"/> property of the
	/// widget's <see cref="Position"/>.
	///
	/// By default, this property has a value of <c>0</c>.
	/// </summary>
	public int X {
		get => _Position.X;
		set {
			if (_Position.X != value) {
				_Position.X = value;
				// TODO: Trigger a move event.
			}
		}
	}

	/// <summary>
	/// The Y position of the widget within its parent widget.
	///
	/// If the widget is a top-level widget, the position is that of the
	/// widget within <see cref="Game1.uiViewport"/>, including its frame.
	/// 
	/// This value is equal to the <see cref="Point.Y"/> property of the
	/// widget's <see cref="Position"/>.
	///
	/// By default, this property has a value of <c>0</c>.
	/// </summary>
	public int Y {
		get => _Position.Y;
		set {
			if (_Position.Y != value) {
				_Position.Y = value;
				// TODO: Trigger a move event.
			}
		}
	}

	/// <summary>
	/// The position of the widget within its parent widget.
	///
	/// If the widget is a top-level widget, the position is that of the widget
	/// within <see cref="Game1.uiViewport"/>, including its frame.
	///
	/// When changing the position, the widget, if visible, receives a move
	/// event immediately. If the widget is not currently visible, it is
	/// guaranteed to receive an event before it is shown.
	///
	/// By default, this property has a value of <c>(0, 0)</c>.
	/// </summary>
	public Point Position {
		get => _Position;
		set {
			if (_Position != value) {
				_Position = value;
				// TODO: Trigger a move event.
			}
		}
	}

	public KSize MinimumSize {
		get => KSize.Zero;
	}

	public KSize MaximumSize {
		get => KSize.Zero;
	}

	/// <summary>
	/// The size of the widget, excluding any frame.
	///
	/// When changing the size, the widget, if visible, receives a resize
	/// event immediately. If the widget is not currently visible, it is
	/// guaranteed to receive an event before it is shown.
	///
	/// The size is adjusted if it lies outside the range defined by
	/// <see cref="MinimumSize"/> and <see cref="MaximumSize"/>.
	/// </summary>
	public KSize Size {
		get => _Size;
		set {
			if (value.Width < MinimumSize.Width)
				value.Width = MinimumSize.Width;
			if (value.Height < MinimumSize.Height)
				value.Height = MinimumSize.Height;
			if (value.Width > MaximumSize.Width)
				value.Width = MaximumSize.Width;
			if (value.Height > MaximumSize.Height)
				value.Height = MaximumSize.Height;

			if (_Size != value) {
				_Size = value;
				// TODO: Trigger a resize event.
			}
		}
	}

	/// <summary>
	/// The width of the widget, excluding any frame.
	///
	/// This value is equal to the <see cref="KSize.Width"/> property of the
	/// widget's <see cref="Size"/>.
	///
	/// When changing the size, the widget, if visible, receives a resize
	/// event immediately. If the widget is not currently visible, it is
	/// guaranteed to receive an event before it is shown.
	///
	/// The size is adjusted if it lies outside the range defined by
	/// <see cref="MinimumSize"/> and <see cref="MaximumSize"/>.
	/// </summary>
	public int Width {
		get => _Size.Width;
		set {
			if (value < MinimumSize.Width)
				value = MinimumSize.Width;
			if (value > MaximumSize.Width)
				value = MaximumSize.Width;

			if (_Size.Width != value) {
				_Size.Width = value;
				// TODO: Trigger a resize event.
			}
		}
	}

	/// <summary>
	/// The height of the widget, excluding any frame.
	///
	/// This value is equal to the <see cref="KSize.Height"/> property of the
	/// widget's <see cref="Size"/>.
	///
	/// When changing the size, the widget, if visible, receives a resize
	/// event immediately. If the widget is not currently visible, it is
	/// guaranteed to receive an event before it is shown.
	///
	/// The size is adjusted if it lies outside the range defined by
	/// <see cref="MinimumSize"/> and <see cref="MaximumSize"/>.
	/// </summary>
	public int Height {
		get => _Size.Height;
		set {
			if (value < MinimumSize.Height)
				value = MinimumSize.Height;
			if (value > MaximumSize.Height)
				value = MaximumSize.Height;

			if (_Size.Height != value) {
				_Size.Height = value;
				// TODO: Trigger a resize event.
			}
		}
	}

	public KMargins ContentMargins {
		get => _ContentMargins;
		set {
			if (_ContentMargins != value) {
				_ContentMargins = value;
				// TODO: Trigger a resize event.
			}
		}
	}

	#endregion

	#region Geometry Methods



	#endregion

	#region Layering Methods

	/// <summary>
	/// Raise this widget to the top (end) of the parent's children list.
	///
	/// This ensures that this widget will be drawn after all its siblings, and
	/// thus that it will appear visually in front of any overlapping siblings.
	/// </summary>
	public void Raise() {
		_Parent?.Children.MoveToEnd(this);
	}

	/// <summary>
	/// Lower this widget to the bottom (start) of the parent's children list.
	///
	/// This ensures that this widget will be drawn before all its siblings,
	/// and thus that it will appear visually in front of any
	/// overlapping siblings.
	/// </summary>
	public void Lower() {
		_Parent?.Children.MoveToStart(this);
	}

	/// <summary>
	/// Place this widget under <paramref name="other"/> in the parent's
	/// children list.
	///
	/// This will have no effect if this widget and <paramref name="other"/>
	/// do not share the same parent.
	/// </summary>
	/// <param name="other">The widget to move relative to.</param>
	public void StackUnder(KWidget other) {
		_Parent?.Children.MoveBefore(this, other);
	}

	#endregion

}
