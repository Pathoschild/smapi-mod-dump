/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ameisen/SV-SpriteMaster
**
*************************************************/

using Microsoft.Xna.Framework.Input;
using SpriteMaster.Configuration;
using SpriteMaster.Extensions;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpriteMaster.Types;

[StructLayout(LayoutKind.Auto)]
internal readonly struct ExtendedButton : IConfigSerializable<ExtendedButton> {
	[Flags]
	internal enum ModifierFlags {
		None = 0,
		Alt = 1 << 0,
		Control = 1 << 1,
		Shift = 1 << 2,
		Command = 1 << 3,
		PrimaryMask = (Command << 1) - 1,
		// Sub-modifiers
		LeftAlt = 1 << 4,
		RightAlt = 1 << 5,
		LeftControl = 1 << 6,
		RightControl = 1 << 7,
		LeftShift = 1 << 8,
		RightShift = 1 << 9,
		LeftCommand = 1 << 10,
		RightCommand = 1 << 11
	}

	internal readonly SButton Button;
	internal readonly ModifierFlags Modifiers;

	internal readonly ExtendedButton Primary => new(Button, Modifiers & ModifierFlags.PrimaryMask);

	private static readonly (Keys, ModifierFlags)[] ModifierMappings = new[] {
		(Keys.LeftAlt, ModifierFlags.LeftAlt | ModifierFlags.Alt),
		(Keys.RightAlt, ModifierFlags.RightAlt | ModifierFlags.Alt),
		(Keys.LeftControl, ModifierFlags.LeftControl | ModifierFlags.Control),
		(Keys.RightControl, ModifierFlags.RightControl | ModifierFlags.Control),
		(Keys.LeftShift, ModifierFlags.LeftShift | ModifierFlags.Shift),
		(Keys.RightShift, ModifierFlags.RightShift | ModifierFlags.Shift),
		(Keys.LeftWindows, ModifierFlags.LeftCommand | ModifierFlags.Command),
		(Keys.RightWindows, ModifierFlags.RightCommand | ModifierFlags.Command),
	};

	internal ExtendedButton(SButton button) {
		Button = button;

		var keyboardState = Game1.input.GetKeyboardState();
		ModifierFlags modifiers = ModifierFlags.None;

		foreach (var (key, flags) in ModifierMappings) {
			if (keyboardState[key] == KeyState.Down) {
				modifiers |= flags;
			}
		}

		Modifiers = modifiers;
	}

	private ExtendedButton(SButton button, ModifierFlags modifiers) {
		Button = button;
		Modifiers = modifiers;
	}

	public readonly bool TrySerialize(out string serialized) {
		if (Modifiers == ModifierFlags.None) {
			serialized = Button.ToString();
		}
		else {
			using var enumNamesPooled = ObjectPoolExt.Take<List<string>>(list => list.Clear());
			var enumNames = enumNamesPooled.Value;
			foreach (var enumPairs in EnumExt.Get<ModifierFlags>()) {
				if (Modifiers.HasFlag(enumPairs.Value)) {
					enumNames.Add(enumPairs.Key);
				}
			}

			var enumString = string.Join('|', enumNames);

			serialized = $"{enumString}+{Button}";
		}

		return true;
	}

	public readonly bool TryDeserialize(string serialized, out ExtendedButton deserialized) {
		try {


			int plusIndex = serialized.IndexOf('+');

			if (plusIndex < 0) {
				if (!Enum.TryParse<SButton>(serialized, out var button)) {
					Debug.Error($"Unknown SButton: {serialized}");
					deserialized = default;
					return false;
				}

				deserialized = new(button, ModifierFlags.None);
				return true;
			}
			else {
				string modifierString = serialized.Substring(0, plusIndex);
				string buttonString = serialized.Substring(plusIndex + 1);

				if (!Enum.TryParse<SButton>(buttonString, out var button)) {
					Debug.Error($"Unknown SButton: {buttonString}");
					deserialized = default;
					return false;
				}

				ModifierFlags modifiers = ModifierFlags.None;

				var modifierStrings = modifierString.Split('|');
				foreach (var modifierName in modifierStrings) {
					if (!Enum.TryParse<ModifierFlags>(modifierName, out var modifier)) {
						Debug.Error($"Unknown ModifierFlags: {modifierName}");
						deserialized = default;
						return false;
					}

					modifiers |= modifier;
				}

				deserialized = new(button, modifiers);
				return true;
			}
		}
		catch {
			deserialized = default;
			return false;
		}
	}
}
