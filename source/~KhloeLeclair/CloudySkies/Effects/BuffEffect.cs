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

using Leclair.Stardew.CloudySkies.Models;
using Leclair.Stardew.Common.Serialization;
using Leclair.Stardew.Common.Serialization.Converters;
using Leclair.Stardew.Common.Types;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

using StardewValley;
using StardewValley.Buffs;
using StardewValley.GameData.Buffs;

namespace Leclair.Stardew.CloudySkies.Effects;

[DiscriminatedType("Buff")]
public record BuffEffectData : BaseEffectData, IBuffEffectData {

	public string? BuffId { get; set; }

	public string? DisplayName { get; set; }

	public string? Description { get; set; }

	public string? IconTexture { get; set; }

	public int IconSpriteIndex { get; set; } = -1;

	[JsonConverter(typeof(ColorConverter))]
	public Color? GlowColor { get; set; }

	public bool? IsDebuff { get; set; }

	public int LingerDuration { get; set; } = 0;

	public int LingerMaxDuration { get; set; } = 0;

	public BuffAttributesData? Effects { get; set; }

	[JsonConverter(typeof(AbstractConverter<ValueEqualityDictionary<string, string>, Dictionary<string, string>>))]
	public Dictionary<string, string> CustomFields { get; set; } = new ValueEqualityDictionary<string, string>();

}

public class BuffEffect : IWeatherEffect, IDisposable {

	private readonly ModEntry Mod;

	public ulong Id { get; }

	public uint Rate { get; }

	private readonly string? BuffId;

	private readonly BuffEffects Effects;

	private readonly string? DisplayName;
	private readonly string? Description;

	private readonly bool? IsDebuff;
	private readonly Color? GlowColor;

	private readonly int LingerDuration;
	private readonly int LingerMaxDuration;

	private readonly string? IconTexture;
	private readonly int IconSpriteIndex;
	private Texture2D? Texture;

	private readonly Dictionary<string, string>? CustomFields;

	private bool IsDisposed;
	private Buff? Buff;

	#region Life Cycle

	public BuffEffect(ModEntry mod, ulong id, IBuffEffectData data) {
		Mod = mod;
		Id = id;
		Rate = data.Rate;

		if (string.IsNullOrEmpty(data.BuffId))
			BuffId = null;
		else
			BuffId = data.BuffId;

		DisplayName = data.DisplayName;
		Description = data.Description;
		LingerDuration = data.LingerDuration;
		LingerMaxDuration = data.LingerMaxDuration;
		IconTexture = data.IconTexture;
		IconSpriteIndex = data.IconSpriteIndex;
		GlowColor = data.GlowColor;
		IsDebuff = data.IsDebuff;
		CustomFields = data.CustomFields;

		Effects = new BuffEffects(data.Effects);

		if (IconTexture != null) {
			Texture = Mod.Helper.GameContent.Load<Texture2D>(IconTexture);
			Mod.MarkLoadsAsset(Id, IconTexture);
		}
	}

	protected virtual void Dispose(bool disposing) {
		if (!IsDisposed) {

			Buff = null;
			Texture = null;
			Mod.RemoveLoadsAsset(Id);

			IsDisposed = true;
		}
	}

	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	#endregion

	public void ReloadAssets() {
		if (IconTexture != null) {
			Texture = Mod.Helper.GameContent.Load<Texture2D>(IconTexture);

			if (Buff != null)
				Buff.iconTexture = Texture;
		}
	}

	public void Remove() {
		if (BuffId is null)
			return;

		// Just make sure our buff is gone and leave.
		if (LingerDuration == 0) {
			Game1.player.buffs.Remove(BuffId);
			return;
		}

		if (Game1.player.buffs.AppliedBuffs.TryGetValue(BuffId, out var buff)) {
			int duration = LingerDuration;
			if (duration != -2 && LingerMaxDuration > duration)
				duration = Game1.random.Next(duration, LingerMaxDuration);

			buff.millisecondsDuration = duration;
		}
	}

	public void Update(GameTime time) {
		if (Game1.player.buffs.AppliedBuffs.TryGetValue(BuffId, out var buff)) {
			buff.millisecondsDuration = Buff.ENDLESS;
			return;
		}

		Buff = new Buff(
			BuffId,
			null,
			null,
			-2,
			Texture,
			IconSpriteIndex,
			Effects,
			IsDebuff,
			DisplayName,
			Description
		);

		// TODO: SpaceCore-ize the Buff

		if (GlowColor.HasValue)
			Buff.glow = GlowColor.Value;

		Game1.player.applyBuff(Buff);

	}

}
