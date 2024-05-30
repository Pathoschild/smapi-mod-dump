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

[DiscriminatedType("ModifyHealth")]
public record ModifyHealthEffectData : BaseEffectData, IModifyHealthEffectData {

	public float Chance { get; set; } = 1f;

	public int Amount { get; set; } = 0;

	public int MinValue { get; set; } = 0;

	public int MaxValue { get; set; } = int.MaxValue;

}

public class ModifyHealthEffect : IWeatherEffect {

	public ulong Id { get; }

	public uint Rate { get; }

	private readonly float Chance;

	private readonly int Amount;

	private readonly int MinValue;

	private readonly int MaxValue;

	public ModifyHealthEffect(ulong id, IModifyHealthEffectData data) {
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

		if (Amount == 0 || Amount < 0 && !Game1.player.CanBeDamaged())
			return;

		int max = Math.Min(Game1.player.maxHealth, MaxValue);

		int current = Game1.player.health;
		int available = Amount < 0
			? current - MinValue
			: max - current;

		if (available <= 0)
			return;

		// We have a Chance% chance to apply this.
		if (Chance <= 0f || (Chance < 1f && Game1.random.NextDouble() > Chance))
			return;

		// Alright, do the damage / healing.
		if (Amount < 0)
			Game1.player.takeDamage(Math.Min(available, -Amount), true, null);
		else
			Game1.player.health += Math.Min(available, Amount);

	}
}
