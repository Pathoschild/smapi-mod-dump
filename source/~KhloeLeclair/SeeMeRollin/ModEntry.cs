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

using Leclair.Stardew.Common.Events;
using Leclair.Stardew.Common.Integrations.GenericModConfigMenu;

using StardewValley;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

#if DEBUG
using Leclair.Stardew.Common.UI;
#endif

namespace Leclair.Stardew.SeeMeRollin {
	public class ModEntry : ModSubscriber {

		public readonly static byte[][][] Frames = new byte[][][] {
			// Normal
			new byte[][] {
				// Left Foot
				new byte[] {
					13,
					17,
					1,
					11
				},
				// Right Foot
				new byte[] {
					14,
					11,
					2,
					17
				}
			},

			// Bathing
			new byte[][] {
				// Left Foot
				new byte[] {
					121, // Up
					116, // Right
					109, // Down
					115  // Left
				},
				// Right Foot
				new byte[] {
					122, // Up
					115, // Right
					110, // Down
					116  // Left
				}
			}
		};

		public static readonly int BUFF = -9999;

		public static ModEntry instance;

		private Harmony Harmony;

		public ModConfig Config;

		private GMCMIntegration<ModConfig, ModEntry> GMCMIntegration;

		private PerScreen<bool> IsRollin = new(() => false);
		private PerScreen<bool> IsAnimating = new(() => false);
		private PerScreen<float> Speed = new(() => 0f);
		private PerScreen<bool> OtherFoot = new(() => false);

#if DEBUG
		private PerScreen<bool> Debugging = new(() => false);
		private PerScreen<Tuple<int, float, float>> Speeds = new(() => new(0, 0, 0));
#endif

		public override void Entry(IModHelper helper) {
			base.Entry(helper);
			instance = this;

			I18n.Init(Helper.Translation);

			Harmony = new Harmony(ModManifest.UniqueID);
			Harmony.PatchAll();

			// Read Config
			Config = Helper.ReadConfig<ModConfig>();
		}

		#region Rolling

		public bool CanRoll(Farmer who) {
			if (!who.CanMove && !who.isRidingHorse())
				return false;

			if (Game1.CurrentEvent != null && !Game1.CurrentEvent.playerControlSequence)
				return false;

			if (Game1.eventUp)
				return false;

			if (!Config.AllowWhenSwimming && who.swimming.Value)
				return false;

			if (!Config.AllowWhenSlowed) {
				int speed = who.addedSpeed;
				if (who.hasBuff(BUFF))
					speed -= Config.SpeedModifier;

				if (speed < 0)
					return false;
			}

			return true;
		}

		private void ApplyBuff() {
			if (!Game1.buffsDisplay.hasBuff(BUFF))
				Game1.buffsDisplay.addOtherBuff(new RollinBuff(Config.SpeedModifier));
		}

		private void RemoveBuff() {
			if (Game1.buffsDisplay.hasBuff(BUFF))
				Game1.buffsDisplay.removeOtherBuff(BUFF);
		}

		public void StartRolling(Farmer who) {
			if (IsRollin.Value)
				return;

			IsRollin.Value = true;
			Speed.Value = who.temporarySpeedBuff;

			ApplyBuff();
		}

		public void StopRolling(Farmer who) {
			if (!IsRollin.Value)
				return;

			IsRollin.Value = false;

			RemoveBuff();

			if (IsAnimating.Value && Game1.player.Equals(who)) {
				IsAnimating.Value = false;
				if (!who.UsingTool)
					who.stopShowingFrame();
			}
		}

		public void FixAnimation(Farmer who) {
			if (IsAnimating.Value && Game1.player.Equals(who)) {
				IsAnimating.Value = false;
				who.stopShowingFrame();
			}
		}

		#endregion

		#region Config

		public void SaveConfig() {
			Helper.WriteConfig(Config);
		}

		public void ResetConfig() {
			Config = new();
		}

		public bool HasGMCM() {
			return GMCMIntegration?.IsLoaded ?? false;
		}

		public void OpenGMCM() {
			if (HasGMCM())
				GMCMIntegration.OpenMenu();
		}

		private void RegisterConfig() {
			GMCMIntegration = new(this, () => Config, ResetConfig, SaveConfig);
			if (!GMCMIntegration.IsLoaded)
				return;

			GMCMIntegration.Register(true);

			GMCMIntegration
				.Add(
					I18n.Setting_EnableAnimation,
					I18n.Setting_EnableAnimation_Desc,
					c => c.EnableAnimation,
					(c, v) => c.EnableAnimation = v
				)
				.Add(
					I18n.Setting_UseKey,
					I18n.Setting_UseKey_Desc,
					c => c.UseKey,
					(c, v) => c.UseKey = v
				)
				.Add(
					I18n.Setting_ShowBuff,
					I18n.Setting_ShowBuff_Desc,
					c => c.ShowBuff,
					(c, v) => c.ShowBuff = v
				)
				.Add(
					I18n.Setting_SpeedMod,
					I18n.Setting_SpeedMod_Desc,
					c => c.SpeedModifier,
					(c, v) => c.SpeedModifier = v,
					1, 15
				)
				.AddChoice(
					I18n.Setting_FallOff,
					I18n.Setting_FallOff_Desc,
					c => c.FalloffMode,
					(c, v) => c.FalloffMode = v,
					new Dictionary<Func<string>, FalloffMode>() {
						{I18n.Setting_FallOff_None, FalloffMode.None},
						{I18n.Setting_FallOff_Slight, FalloffMode.Slight},
						{I18n.Setting_FallOff_TillZero, FalloffMode.TillZero}
					}
				)
				.Add(
					I18n.Setting_AllowSwim,
					I18n.Setting_AllowSwim_Desc,
					c => c.AllowWhenSwimming,
					(c, v) => c.AllowWhenSwimming = v
				)
				.Add(
					I18n.Setting_AllowSlowed,
					I18n.Setting_AllowSlowed_Desc,
					c => c.AllowWhenSlowed,
					(c, v) => c.AllowWhenSlowed = v
				);
		}

		#endregion

		#region Events

#if DEBUG

		[Subscriber]
		private void DebugRender(object sender, RenderedHudEventArgs e) {

			if (Debugging.Value) {
				int? direction = Game1.player.movementDirections.Count > 0 ? Game1.player.movementDirections[0] : null;

				SimpleHelper.DrawHover(
					node: SimpleHelper.Builder()
						.Text($"X: {Game1.player.position.X}, Y: {Game1.player.position.Y}")
						.Text($"Facing: {direction}")
						.Text($"Bathing: {Game1.player.bathingClothes.Value}")
						.Text($"Speed: {Speeds.Value.Item3}")
						.Text($"Can Move: {Game1.player.CanMove}")
						.Text($"Using Tool: {Game1.player.UsingTool}")
						.Divider()
						.Text($"Foot: {OtherFoot.Value}")
						.Text($"Rollin: {IsRollin.Value}")
						.Text($"Added Speed: {Speeds.Value.Item1}")
						.Text($"Temp. Speed: {Speeds.Value.Item2}")
						.GetLayout(),
					batch: e.SpriteBatch,
					defaultFont: Game1.smallFont,
					overrideX: 4,
					overrideY: 4
				);
			}
		}

#endif

		[Subscriber]
		private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
			RegisterConfig();
		}

		[Subscriber]
		private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e) {
			if (!Context.IsWorldReady)
				return;

#if DEBUG

			if (KeybindList.Parse("F8").JustPressed()) {
				Debugging.Value = !Debugging.Value;
				return;
			}

			if (KeybindList.Parse("F5").IsDown()) {
				if (IsRollin.Value)
					StopRolling(Game1.player);

				RemoveBuff();

				Game1.player.addedSpeed = 0;
				Game1.player.temporarySpeedBuff = 0f;

				return;
			}

			if (KeybindList.Parse("F6").JustPressed()) {
				if (Game1.player.bathingClothes.Value)
					Game1.player.changeOutOfSwimSuit();
				else
					Game1.player.changeIntoSwimsuit();
			}

#endif

			if (IsRollin.Value) {
				if (!Config.UseKey.IsDown())
					StopRolling(Game1.player);

			} else {
				if (Config.UseKey.JustPressed() && CanRoll(Game1.player))
					StartRolling(Game1.player);
			}
		}

		[Subscriber]
		private void OnUpdate(object sender, UpdateTickingEventArgs e) {

			if (!Context.IsWorldReady || !IsRollin.Value) {
#if DEBUG
				if (Context.IsWorldReady)
					Speeds.Value = new(Game1.player.addedSpeed, Speed.Value, Game1.player.getMovementSpeed());
#endif
				return;
			}

			if (!CanRoll(Game1.player)) {
				StopRolling(Game1.player);
#if DEBUG
				Speeds.Value = new(Game1.player.addedSpeed, Speed.Value, Game1.player.getMovementSpeed());
#endif
				return;
			}

			if (Config.FalloffMode != FalloffMode.None)
				Game1.player.temporarySpeedBuff = Speed.Value;

			float speed = Game1.player.getMovementSpeed();
			if (speed <= 1) {
				StopRolling(Game1.player);
#if DEBUG
				Speeds.Value = new(Game1.player.addedSpeed, Speed.Value, Game1.player.getMovementSpeed());
#endif
				return;
			}

#if DEBUG
			Speeds.Value = new(Game1.player.addedSpeed, Speed.Value, speed);
#endif

			if (Config.FalloffMode != FalloffMode.TillZero && Speed.Value < -2) {
				Speed.Value = 0f;
				OtherFoot.Value = !OtherFoot.Value;
			}

			Speed.Value -= 0.02f;

			// Show a static animation frame, to make us look fly.
			if (Game1.player.movementDirections.Count <= 0 || !Game1.player.CanMove || Game1.player.UsingTool || !Config.EnableAnimation) {
				if (IsAnimating.Value) {
					IsAnimating.Value = false;
					Game1.player.stopShowingFrame();
				}

				return;
			}

			int direction = Game1.player.movementDirections[0];
			if (direction < 0 || direction > 3)
				return;

			// Bathe responsibly. Don't randomly turn into a man
			// by showing the wrong animation frame.
			// int offset = Game1.player.bathingClothes.Value ? 108 : 0;

			IsAnimating.Value = true;

			Game1.player.faceDirection(direction);
			Game1.player.showFrame(
				frame: Frames[Game1.player.bathingClothes.Value ? 1 : 0][OtherFoot.Value ? 1 : 0][direction],
				flip: direction == 3
			);
		}

		#endregion
	}
}
