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

using Leclair.Stardew.CloudySkies.Models;
using Leclair.Stardew.Common.Serialization.Converters;

using Microsoft.Xna.Framework;

using StardewValley;

namespace Leclair.Stardew.CloudySkies.Effects;

[DiscriminatedType("ModifyStamina")]
public record ModifyStaminaEffectData : BaseEffectData, IModifyStaminaEffectData {

	public float Chance { get; set; } = 1f;

	public int Amount { get; set; } = 0;

	public int MinValue { get; set; } = 0;

	public int MaxValue { get; set; } = int.MaxValue;

}

public class ModifyStaminaEffect : IWeatherEffect {

	public ulong Id { get; }

	public uint Rate { get; }

	private readonly float Chance;

	private readonly int Amount;

	private readonly int MinValue;

	private readonly int MaxValue;

	public ModifyStaminaEffect(ulong id, IModifyStaminaEffectData data) {
		Id = id;
		Rate = data.Rate;

		Chance = data.Chance;
		Amount = data.Amount;
		MinValue = data.MinValue;
		MaxValue = data.MaxValue;
	}

	public void Remove() {

	}

	public void Update(GameTime time) {

		if (Amount == 0)
			return;

		int max = Math.Min(Game1.player.MaxStamina, MaxValue);

		int current = (int) Game1.player.Stamina;
		int available = Amount < 0
			? current - MinValue
			: max - current;

		if (available <= 0)
			return;

		// We have a Chance% chance to apply this.
		if (Chance <= 0f || (Chance < 1f && Game1.random.NextDouble() > Chance))
			return;

		// Alright, do the stamina drain / fill.
		if (Amount < 0)
			Game1.player.Stamina -= Math.Min(available, -Amount);
		else
			Game1.player.Stamina += Math.Min(available, Amount);

	}

}
