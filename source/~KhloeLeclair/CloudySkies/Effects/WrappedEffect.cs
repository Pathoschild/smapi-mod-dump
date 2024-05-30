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

namespace Leclair.Stardew.CloudySkies.Effects;

public class WrappedEffect : IWeatherEffect {

	internal static readonly Dictionary<Type, Func<object, uint>> RateAccessors = new();
	internal static readonly Dictionary<Type, Action<object, GameTime>> UpdateAccessors = new();

	private readonly IWeatherEffect Effect;
	private readonly object InnerEffect;

	private readonly Func<object, uint> RateAccessor;
	private readonly Action<object, GameTime> UpdateAccessor;

	public ulong Id { get; }

	public uint Rate => RateAccessor(InnerEffect);

	public WrappedEffect(ModEntry mod, IWeatherEffect inner) {
		Effect = inner;
		Id = inner.Id;

		if (!mod.TryUnproxy(inner, out object? unproxied))
			unproxied = inner;

		Type type = unproxied.GetType();

		if (!RateAccessors.TryGetValue(type, out var rateAccessor)) {
			var method = AccessTools.PropertyGetter(type, nameof(IWeatherEffect.Rate));
			rateAccessor = ReflectionHelper.CreateFunc<object, uint>(method);
			RateAccessors.Add(type, rateAccessor);
		}

		if (!UpdateAccessors.TryGetValue(type, out var updateAccessor)) {
			var method = AccessTools.Method(type, nameof(IWeatherEffect.Update));
			updateAccessor = ReflectionHelper.CreateAction<object, GameTime>(method);
			UpdateAccessors.Add(type, updateAccessor);
		}

		InnerEffect = unproxied;
		RateAccessor = rateAccessor;
		UpdateAccessor = updateAccessor;
	}

	public void ReloadAssets() {
		Effect.ReloadAssets();
	}

	public void Remove() {
		Effect.Remove();
	}

	public void Update(GameTime time) {
		UpdateAccessor(InnerEffect, time);
	}

}
