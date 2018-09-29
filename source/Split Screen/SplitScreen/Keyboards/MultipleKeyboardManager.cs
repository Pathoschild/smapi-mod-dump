using Harmony;
using Microsoft.Xna.Framework.Input;
using RawInput_dll;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SplitScreen.Keyboards
{
	//https://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard

	public class MultipleKeyboardManager
	{
		//KeyPressEvent.DeviceName is the key
		static private readonly Dictionary<string, KeyStateStore> keyboardsKeyStores = new Dictionary<string, KeyStateStore>();
		static private Dictionary<string, KeyboardState> oldKeyboardStates = new Dictionary<string, KeyboardState>();

		static string attachedKeyboardID = "";
		public static string AttachedKeyboardID
		{ 
			get
			{	if (attachedKeyboardID != "") return attachedKeyboardID;
				else return "ANY";
			}
		}
		public static bool HasAttachedKeyboard() => !String.IsNullOrWhiteSpace(attachedKeyboardID);

		private static KeyboardState? oldAttachedKeyboardState;

		public MultipleKeyboardManager(HarmonyInstance harmony)
		{
			RawInput rawinput = new RawInput(Game1.game1.Window.Handle, false);
			rawinput.KeyPressed += OnKeyPressed;
		}
		
		#region Events
		private void OnKeyPressed(object sender, RawInputEventArg e)
		{
			string deviceUniqueID = e.KeyPressEvent.DeviceHandle.ToString();

			Keys key = GetKey(e.KeyPressEvent.VKey);
			if (key != Keys.None)
			{
				if (!keyboardsKeyStores.ContainsKey(deviceUniqueID))
					keyboardsKeyStores.Add(deviceUniqueID, new KeyStateStore());

				if (keyboardsKeyStores.TryGetValue(deviceUniqueID, out KeyStateStore keyStateStore))
					keyStateStore.SetKeyState((int)key, e.KeyPressEvent.IsPressed());
			}
			else
			{
				Console.WriteLine($"Unknown key \"{KeyMapper.GetMicrosoftKeyName(e.KeyPressEvent.VKey)}\"");
			}

			//Fixes input not working inside of text boxes and chat boxes
			if (e.KeyPressEvent.IsPressed() && (deviceUniqueID == attachedKeyboardID || ( String.IsNullOrWhiteSpace(attachedKeyboardID)   && Game1.game1.IsActive  ) ))
			{
				char c = KeyToChar(key, GetAnyPressedKeys().Contains(Keys.LeftShift));

				if ((int)c != 0)//Prevents sending strange keys
					Game1.keyboardDispatcher.Subscriber?.RecieveTextInput(c);

				switch (key)
				{
					case (Keys.Back): Game1.keyboardDispatcher.Subscriber?.RecieveCommandInput('\b'); break;
					case (Keys.Enter): Game1.keyboardDispatcher.Subscriber?.RecieveCommandInput('\r'); break;
					case (Keys.Tab): Game1.keyboardDispatcher.Subscriber?.RecieveCommandInput('\t'); break;
				}
			}
		}
		
		public void OnUpdate(object sender, EventArgs args)
		{
			//Update old keyboard states
			oldKeyboardStates.Clear();
			foreach (var k in keyboardsKeyStores) oldKeyboardStates.Add(k.Key, k.Value.GetKeyboardState());

			oldAttachedKeyboardState = Keyboard.GetState();
		}

		public void OnAfterReturnToTitle(object sender, EventArgs args) => DetachKeyboard();
		#endregion

		private void DetachKeyboard()
		{
			if (!String.IsNullOrWhiteSpace(attachedKeyboardID))
			{
				attachedKeyboardID = "";
				Monitor.Log("Detached keyboard", StardewModdingAPI.LogLevel.Trace);
			}
		}

		private Keys GetKey(int key_int)
		{
			//VKey seems to (mostly) correspond to Microsoft.XNA.Framework.Input.Keys
			
			if (Enum.IsDefined(typeof(Keys), key_int))
				return (Keys)key_int;
			else
			{
				//https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys
				switch (key_int)
				{
					case 16: return Keys.LeftShift;
					case 17: return Keys.LeftControl;
					case 18: return Keys.LeftAlt;
					default: return Keys.None;
				}
			}
		}

		public static KeyboardState? GetAttachedKeyboardState()
		{
			if (keyboardsKeyStores != null && keyboardsKeyStores.TryGetValue(attachedKeyboardID, out KeyStateStore attachedKeyStateStore))
				return attachedKeyStateStore.GetKeyboardState();
			return null;
		}

		public static IEnumerable<Keys> GetAnyPressedKeys()
		{
			IEnumerable<Keys> keys = new List<Keys>();

			foreach (KeyStateStore keyStateStore in keyboardsKeyStores.Values)
				keys = keys.Concat(keyStateStore.GetPressedKeys());

			return keys;
		}

		public static bool WasKeyJustPressed(Keys key)
		{
			if (String.IsNullOrWhiteSpace(attachedKeyboardID))
				return Game1.game1.IsActive && Keyboard.GetState().IsKeyDown(key) && oldAttachedKeyboardState?.IsKeyUp(key) == true;
			else
				return oldKeyboardStates.TryGetValue(attachedKeyboardID, out KeyboardState oldKeyboardState) && (GetAttachedKeyboardState() ?? new KeyboardState()).IsKeyDown(key) && oldKeyboardState.IsKeyUp(key);
		}

		/// <summary>
		/// Returns true if found a keyboard
		/// </summary>
		/// <returns></returns>
		public bool CheckKeyboardsToAttach()
		{
			foreach (string keyboardID in keyboardsKeyStores.Keys)
			{
				keyboardsKeyStores.TryGetValue(keyboardID, out KeyStateStore keyStateStore); var keyboardState = keyStateStore.GetKeyboardState();
				oldKeyboardStates.TryGetValue(keyboardID, out KeyboardState oldKeyboardState);

				if (keyboardState.GetPressedKeys().Length > 0)
				{
					attachedKeyboardID = keyboardID;
					Console.WriteLine("Set new attached keyboard to " + keyboardID);
					return true;
				}
			}

			return false;
		}

		public void OnDetachButtonClicked() => DetachKeyboard();
		

		/// <summary>
		/// Converts a key to a char. Uses US QWERTY layout
		/// https://www.reddit.com/r/monogame/comments/70j259/code_how_to_convert_an_xnamonogame_key_to_a/
		/// </summary>
		/// <param name="Key">They key to convert.</param>
		/// <param name="Shift">Whether or not shift is pressed.</param>
		/// <returns>The key in a char.</returns>
		private static Char KeyToChar(Keys Key, bool Shift = false)
		{
			/* It's the space key. */
			if (Key == Keys.Space)
			{
				return ' ';
			}
			else
			{
				string String = Key.ToString();

				/* It's a letter. */
				if (String.Length == 1)
				{
					Char Character = Char.Parse(String);
					byte Byte = Convert.ToByte(Character);

					if (
						(Byte >= 65 && Byte <= 90) ||
						(Byte >= 97 && Byte <= 122)
						)
					{
						return (!Shift ? Character.ToString().ToLower() : Character.ToString())[0];
					}
				}

				/* 
				 * 
				 * The only issue is, if it's a symbol, how do I know which one to take if the user isn't using United States international?
				 * Anyways, thank you, for saving my time
				 * down here:
				 */

				#region Credits :  http://roy-t.nl/2010/02/11/code-snippet-converting-keyboard-input-to-text-in-xna.html for saving my time.
				switch (Key)
				{
					case Keys.D0:
						if (Shift) { return ')'; } else { return '0'; }
					case Keys.D1:
						if (Shift) { return '!'; } else { return '1'; }
					case Keys.D2:
						if (Shift) { return '@'; } else { return '2'; }
					case Keys.D3:
						if (Shift) { return '#'; } else { return '3'; }
					case Keys.D4:
						if (Shift) { return '$'; } else { return '4'; }
					case Keys.D5:
						if (Shift) { return '%'; } else { return '5'; }
					case Keys.D6:
						if (Shift) { return '^'; } else { return '6'; }
					case Keys.D7:
						if (Shift) { return '&'; } else { return '7'; }
					case Keys.D8:
						if (Shift) { return '*'; } else { return '8'; }
					case Keys.D9:
						if (Shift) { return '('; } else { return '9'; }

					case Keys.NumPad0: return '0';
					case Keys.NumPad1: return '1';
					case Keys.NumPad2: return '2';
					case Keys.NumPad3: return '3';
					case Keys.NumPad4: return '4';
					case Keys.NumPad5: return '5';
					case Keys.NumPad6: return '6';
					case Keys.NumPad7: return '7'; ;
					case Keys.NumPad8: return '8';
					case Keys.NumPad9: return '9';

					case Keys.OemTilde:
						if (Shift) { return '~'; } else { return '`'; }
					case Keys.OemSemicolon:
						if (Shift) { return ':'; } else { return ';'; }
					case Keys.OemQuotes:
						if (Shift) { return '"'; } else { return '\''; }
					case Keys.OemQuestion:
						if (Shift) { return '?'; } else { return '/'; }
					case Keys.OemPlus:
						if (Shift) { return '+'; } else { return '='; }
					case Keys.OemPipe:
						if (Shift) { return '|'; } else { return '\\'; }
					case Keys.OemPeriod:
						if (Shift) { return '>'; } else { return '.'; }
					case Keys.OemOpenBrackets:
						if (Shift) { return '{'; } else { return '['; }
					case Keys.OemCloseBrackets:
						if (Shift) { return '}'; } else { return ']'; }
					case Keys.OemMinus:
						if (Shift) { return '_'; } else { return '-'; }
					case Keys.OemComma:
						if (Shift) { return '<'; } else { return ','; }
				}
				#endregion

				return (Char)0;

			}
		}

	}

	static class KeyboardExtensions
	{
		public static bool IsPressed(this KeyPressEvent keyPressEvent) => keyPressEvent.KeyPressState == "MAKE";

		public static bool AreKeysDown(this KeyboardState keyboardState, params Keys[] keys) => keys.All(x => keyboardState.IsKeyDown(x));		
	}
}
