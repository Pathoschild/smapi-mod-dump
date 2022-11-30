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

using Microsoft.Xna.Framework;

using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.UI;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

namespace Leclair.Stardew.InputTest;

public class SButtonThing {

	public readonly SButton Button;
	private int Countdown;

	public SButtonThing(SButton button, int time) {
		Button = button;
		Countdown = time;
	}

	public bool Update() {
		Countdown--;
		return Countdown > 0;
	}

	public float Opacity {
		get {
			if (Countdown >= 30)
				return 1f;
			return Countdown / 30f;
		}
	}

}

public class ModEntry : ModSubscriber {

	readonly List<SButtonThing> Pressed = new();
	SButton[]? Held;
	readonly List<SButtonThing> Released = new();

	[Subscriber]
	private void OnRendered(object? sender, RenderedEventArgs e) {

		var builder = SimpleHelper.Builder();

		var b2 = FlowHelper.Builder();

		if (Pressed.Count > 0) {
			for(int i = 0; i < Pressed.Count; i++) {
				var thing = Pressed[i];
				if (thing.Update()) {
					b2.Text($"{thing.Button} ", color: new Color(0, 0, 0) * thing.Opacity);
				} else {
					Pressed.Remove(thing);
					i--;
				}
			}
		} else
			b2.Text("(none)");

		builder.Group(margin: 8)
			.Text("Pressed")
			.Divider()
			.Flow(b2.Build(), wrapText: false)
		.EndGroup()
		.Divider();

		builder.Group(margin: 8)
			.Text("Held")
			.Divider()
			.Text(Held is null ? "(none)" : string.Join(", ", Held))
		.EndGroup()
		.Divider();

		b2 = FlowHelper.Builder();

		if (Released.Count > 0) {
			for(int i = 0; i < Released.Count; i++) {
				var thing = Released[i];
				if (thing.Update()) {
					b2.Text($"{thing.Button} ", color: new Color(0, 0, 0) * thing.Opacity);
				} else {
					Released.Remove(thing);
					i--;
				}
			}
		} else
			b2.Text("(none)");

		builder.Group(margin: 8)
			.Text("Released")
			.Divider()
			.Flow(b2.Build(), wrapText: false)
		.EndGroup();

		builder.GetLayout().DrawHover(e.SpriteBatch, defaultFont: Game1.smallFont, overrideX: 8, overrideY: 8);
	}

	[Subscriber]
	private void OnChanged(object? sender, ButtonsChangedEventArgs e) {
		foreach (var button in e.Pressed) {
			Pressed.Add(new(button, 40));
		}

		var held = e.Held.ToArray();
		Held = held.Length > 0 ? held : null;

		foreach (var button in e.Released) {
			Released.Add(new(button, 40));
		}
	}

}
