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

using HarmonyLib;

using Leclair.Stardew.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Leclair.Stardew.CloudySkies.Layers;

public class WrappedLayer : IWeatherLayer {

	internal static readonly Dictionary<Type, Action<object, SpriteBatch, GameTime, RenderTarget2D>> DrawActions = new();
	internal static readonly Dictionary<Type, Action<object, GameTime>> UpdateActions = new();
	internal static readonly Dictionary<Type, Action<object, int, int>> MoveWithViewportActions = new();

	private readonly IWeatherLayer Layer;
	private readonly object InnerLayer;

	private readonly Action<object, SpriteBatch, GameTime, RenderTarget2D> DrawAction;
	private readonly Action<object, GameTime> UpdateAction;
	private readonly Action<object, int, int> MoveWithViewportAction;

	public ulong Id { get; }

	public LayerDrawType DrawType { get; }

	public WrappedLayer(ModEntry mod, IWeatherLayer inner) {
		Layer = inner;
		Id = inner.Id;
		DrawType = inner.DrawType;

		if (!mod.TryUnproxy(inner, out object? unproxied))
			unproxied = inner;

		Type type = unproxied.GetType();

		if (!DrawActions.TryGetValue(type, out var drawAction)) {
			var method = AccessTools.Method(type, nameof(IWeatherLayer.Draw));
			drawAction = ReflectionHelper.CreateAction<object, SpriteBatch, GameTime, RenderTarget2D>(method);
			DrawActions.Add(type, drawAction);
		}

		if (!UpdateActions.TryGetValue(type, out var updateAction)) {
			var method = AccessTools.Method(type, nameof(IWeatherLayer.Update));
			updateAction = ReflectionHelper.CreateAction<object, GameTime>(method);
			UpdateActions.Add(type, updateAction);
		}

		if (!MoveWithViewportActions.TryGetValue(type, out var moveWithViewportAction)) {
			var method = AccessTools.Method(type, nameof(IWeatherLayer.MoveWithViewport));
			moveWithViewportAction = ReflectionHelper.CreateAction<object, int, int>(method);
			MoveWithViewportActions.Add(type, moveWithViewportAction);
		}

		InnerLayer = unproxied;
		DrawAction = drawAction;
		UpdateAction = updateAction;
		MoveWithViewportAction = moveWithViewportAction;
	}

	public void Draw(SpriteBatch batch, GameTime time, RenderTarget2D targetScreen) {
		DrawAction(InnerLayer, batch, time, targetScreen);
	}

	public void MoveWithViewport(int offsetX, int offsetY) {
		MoveWithViewportAction(InnerLayer, offsetX, offsetY);
	}

	public void ReloadAssets() {
		Layer.ReloadAssets();
	}

	public void Resize(Point newSize, Point oldSize) {
		Layer.Resize(newSize, oldSize);
	}

	public void Update(GameTime time) {
		UpdateAction(InnerLayer, time);
	}
}
