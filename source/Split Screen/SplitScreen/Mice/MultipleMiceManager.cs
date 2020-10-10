/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ilyaki/SplitScreen
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RawInputSharp;
using StardewValley;
using System;
using System.Collections.Generic;

namespace SplitScreen.Mice
{
	//Uses RawInputSharp from http://jstookey.com/arcade/rawmouse/

	class MultipleMiceManager
	{
		private static RawMouse attachedMouse;
		private static Vector2 totalAttachedDelta = new Vector2(0, 0);
		
		private MouseDisabler mouseDisabler;

		private static RawMouseInput rawMouseInput;

		public static string AttachedMouseID
		{
			get {
				if (attachedMouse != null) return attachedMouse.Handle.ToString();
				else return "ANY";
			}
		}

		public static bool HasAttachedMouse() => attachedMouse != null;

		//Vector2 is total delta
		private static List<RawMouse> mice = new List<RawMouse>();

		public MultipleMiceManager()
		{
			mouseDisabler = new MouseDisabler();

			RegisterMice();
		}

		#region Registering mice
		private void RegisterMice()
		{
			rawMouseInput = new RawMouseInput();
			rawMouseInput.RegisterForWM_INPUT(System.Windows.Forms.Control.FromHandle(Game1.game1.Window.Handle).FindForm().Handle);//form.handle
			System.Windows.Forms.Application.AddMessageFilter(new PreMessageFilter());

			foreach (object rawMouseObj in rawMouseInput.Mice)
			{
				if (rawMouseObj != null)
					mice.Add((RawMouse)rawMouseObj);
			}
			Console.WriteLine($"Registered mouse driver, found {mice.Count} mice");
		}

		private class PreMessageFilter : System.Windows.Forms.IMessageFilter
		{
			public bool PreFilterMessage(ref System.Windows.Forms.Message m)
			{
				if (m != null && m.Msg == 0x00FF && rawMouseInput != null)
					rawMouseInput.UpdateRawMouse(m.LParam);
				return false;
			}
		}
		#endregion

		#region SMAPI events
		
		public void OnUpdateTick(object sender, EventArgs e)
		{
			if (attachedMouse != null && !MouseDisabler.IsAutoHotKeyNull)
			{
				System.Windows.Forms.Cursor.Position = new System.Drawing.Point(0, 0);
				System.Windows.Forms.Cursor.Clip = new System.Drawing.Rectangle(System.Windows.Forms.Cursor.Position, new System.Drawing.Size(1, 1));
			}

			if (attachedMouse != null)
			{
				totalAttachedDelta.X = Math.Max(0, Math.Min(totalAttachedDelta.X + attachedMouse.XDelta, Game1.game1.GraphicsDevice.PresentationParameters.Bounds.Width - 1));
				totalAttachedDelta.Y = Math.Max(0, Math.Min(totalAttachedDelta.Y + attachedMouse.YDelta, Game1.game1.GraphicsDevice.PresentationParameters.Bounds.Height - 1));
			}

			if (Game1.quit)
				DetatchMouse();
		}

		public void OnAfterReturnToTitle(object sender, EventArgs e)
		{
			DetatchMouse();
		}
		#endregion

		#region Attach/Detach buttons
		public void AttachMouseButtonClicked()
		{
			foreach (RawMouse mouse in mice)
			{
				if (attachedMouse != mouse && mouse.Buttons[0])
				{
					attachedMouse = mouse;
					totalAttachedDelta = new Vector2(0, 0);
					Monitor.Log($"Set attached mouse to {mouse.Handle.ToString()}", StardewModdingAPI.LogLevel.Trace);

					mouseDisabler.Lock();
					break;
				}
			}
		}

		public void DetachMouseButtonClicked()
		{
			DetatchMouse();
		}
		#endregion

		private void DetatchMouse()
		{
			if (attachedMouse != null)
			{
				attachedMouse = null;
				System.Windows.Forms.Cursor.Clip = new System.Drawing.Rectangle();
				mouseDisabler.Unlock();

				Monitor.Log("Detached mouse", StardewModdingAPI.LogLevel.Trace);
			}
		}

		public static MouseState? GetAttachedMouseState()
		{
			if (attachedMouse == null) return null; 
			
			int leftButtonState = attachedMouse.Buttons[0] ? 1 : 0;
			int rightButtonState = attachedMouse.Buttons[2] ? 1 : 0;
			int middleButtonState = attachedMouse.Buttons[1] ? 1 : 0;
			return new MouseState((int)totalAttachedDelta.X, (int)totalAttachedDelta.Y, attachedMouse.Z, (ButtonState)leftButtonState, (ButtonState)rightButtonState, (ButtonState)middleButtonState, ButtonState.Released, ButtonState.Released);
		}
	}
}
