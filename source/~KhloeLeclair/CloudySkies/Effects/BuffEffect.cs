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

using Leclair.Stardew.Common.Serialization.Converters;

using Leclair.Stardew.CloudySkies.Models;
using Microsoft.Xna.Framework;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buffs;
using Newtonsoft.Json;
using StardewValley.GameData.Buffs;

namespace Leclair.Stardew.CloudySkies.Effects;

[DiscriminatedType("Buff")]
public record BuffEffectData : BaseEffectData {

	public string? BuffId { get; set; }

	public string? IconTexture { get; set; }

	public int IconSpriteIndex { get; set; } = -1;

	public bool? IsDebuff { get; set; }

	[JsonConverter(typeof(ColorConverter))]
	public Color? GlowColor { get; set; }

	public int LingerDuration { get; set; } = 0;

	public BuffAttributesData? Effects { get; set; }

	public string? DisplayName { get; set; }

	public string? Description { get; set; }

}

public class BuffEffect : IEffect, IDisposable {

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

	private readonly string? IconTexture;
	private readonly int IconSpriteIndex;
	private Texture2D? Texture;

	private bool IsDisposed;

	#region Life Cycle

	public BuffEffect(ModEntry mod, ulong id, BuffEffectData data) {
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
		IconTexture = data.IconTexture;
		IconSpriteIndex = data.IconSpriteIndex;
		GlowColor = data.GlowColor;
		IsDebuff = data.IsDebuff;

		Effects = new BuffEffects(data.Effects);

		if (IconTexture != null) { 
			Texture = Mod.Helper.GameContent.Load<Texture2D>(IconTexture);
			Mod.MarkLoadsAsset(Id, IconTexture);
		}
	}

	protected virtual void Dispose(bool disposing) {
		if (!IsDisposed) {

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
		if (IconTexture != null)
			Texture = Mod.Helper.GameContent.Load<Texture2D>(IconTexture);
	}

	public void Remove() {
		if (BuffId is null)
			return;

		// Just make sure our buff is gone and leave.
		if (LingerDuration == 0) {
			Game1.player.buffs.Remove(BuffId);
			return;
		}

		if (Game1.player.buffs.AppliedBuffs.TryGetValue(BuffId, out var buff))
			buff.millisecondsDuration = LingerDuration;
	}

	public void Update(GameTime time) {
		if (BuffId is null)
			return;

		if (Game1.player.buffs.AppliedBuffs.TryGetValue(BuffId, out var buff)) {
			buff.millisecondsDuration = Buff.ENDLESS;
			return;
		}

		buff = new Buff(
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

		if (GlowColor.HasValue)
			buff.glow = GlowColor.Value;

		Game1.player.applyBuff(buff);

	}

}
