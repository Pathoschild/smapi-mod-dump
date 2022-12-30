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
using System.Text;
using System.Reflection;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using HarmonyLib;

using StardewModdingAPI;
using StardewValley;

namespace Leclair.Stardew.ThemeManager.Patches;

internal static class SpriteBatch_Patches {

	private static ModEntry? Mod;
	private static IMonitor? Monitor;

	private static bool _AlignText;

	internal static MethodInfo[]? Methods;
	internal static HarmonyMethod? Prefix;

	internal static bool AlignText {
		get => _AlignText;
		set {
			if (_AlignText == value) return;

			_AlignText = value;
			if (_AlignText)
				Apply();
			else
				Unapply();
		}
	}

	internal static void Patch(ModEntry mod) {
		Mod = mod;
		Monitor = mod.Monitor;

		Methods = new[] {
			AccessTools.Method(
				typeof(SpriteBatch),
				nameof(SpriteBatch.DrawString),
				new Type[] {
					typeof(SpriteFont),
					typeof(string),
					typeof(Vector2),
					typeof(Color)
				}
			),
			AccessTools.Method(
				typeof(SpriteBatch),
				nameof(SpriteBatch.DrawString),
				new Type[] {
					typeof(SpriteFont),
					typeof(StringBuilder),
					typeof(Vector2),
					typeof(Color)
				}
			),

			AccessTools.Method(
				typeof(SpriteBatch),
				nameof(SpriteBatch.DrawString),
				new Type[] {
					typeof(SpriteFont),
					typeof(string),
					typeof(Vector2),
					typeof(Color),
					typeof(float),
					typeof(Vector2),
					typeof(Vector2),
					typeof(SpriteEffects),
					typeof(float)
				}
			),
			AccessTools.Method(
				typeof(SpriteBatch),
				nameof(SpriteBatch.DrawString),
				new Type[] {
					typeof(SpriteFont),
					typeof(StringBuilder),
					typeof(Vector2),
					typeof(Color),
					typeof(float),
					typeof(Vector2),
					typeof(Vector2),
					typeof(SpriteEffects),
					typeof(float)
				}
			)
		};

		Prefix = new HarmonyMethod(typeof(SpriteBatch_Patches), nameof(DrawString_Prefix));

		if (_AlignText)
			Apply();
	}

	private static void Apply() {
		if (Mod?.Harmony is null || Methods is null || Prefix is null)
			return;

		try {
			foreach (MethodInfo? method in Methods) {
				if (method is null)
					continue;

				Mod.Harmony.Patch(method, prefix: Prefix);
			}

		} catch (Exception ex) {
			Mod.Log("Unable to apply SpriteText patches due to error.", LogLevel.Error, ex);
		}
	}

	private static void Unapply() {
		if (Mod?.Harmony is null || Methods is null || Prefix is null)
			return;

		try {
			foreach (MethodInfo? method in Methods) {
				if (method is null)
					continue;

				Mod.Harmony.Unpatch(method, Prefix.method);
			}

		} catch (Exception ex) {
			Mod.Log("Unable to unapply SpriteText patches due to error.", LogLevel.Error, ex);
		}
	}

	static bool DrawString_Prefix(ref Vector2 position) {
		position = new Vector2(
			MathF.Floor(position.X),
			MathF.Floor(position.Y)
		);

		return true;
	}

}
