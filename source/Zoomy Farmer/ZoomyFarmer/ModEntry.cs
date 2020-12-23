/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/geko_x/stardew-mods
**
*************************************************/

using System;
using System.Runtime.Remoting.Channels;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace Sprint {
	class ModEntry : Mod {

		public static Mod INSTANCE;
		public static IModHelper modhelper;
		public static ITranslationHelper i18n;

		public static ModConfig config;

		private bool isPlayerSprinting = false;
		private bool isPlayerToggleSprinting = false;

		private int actualSprintSpeed = 0;
		private float actualStaminaDrain = 0;
		private int actualSprintIncrease = 0;

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			INSTANCE = this;
			modhelper = helper;
			i18n = helper.Translation;

			Monitor.Log("Mod Entry", LogLevel.Trace);

			Monitor.Log("Reading config", LogLevel.Debug);
			ModEntry.config = helper.ReadConfig<ModConfig>();

			// Warn about potential clash with default run key
			if(config.sprintKey == SButton.LeftShift || config.alternateSprintKey == SButton.LeftShift) {
				Monitor.LogOnce("Potential button clash for running and sprinting! Consider changing the 'sprintKey' button in the config!", LogLevel.Warn);
			}

			modhelper.Events.GameLoop.DayStarted += OnDayStarted;

			modhelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			modhelper.Events.GameLoop.OneSecondUpdateTicked += OnSecondTicked;

			//modhelper.Events.Multiplayer.PeerContextReceived += OnPeerContextReceived;
			//modhelper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
		}

		private void OnDayStarted(object sender, DayStartedEventArgs e) {
			
			// Get the default player speed, just in case it's changed from the default

			//if (Context.IsMainPlayer) {
			ModConfig.defaultFarmerSpeed = Game1.player.speed;

			this.actualSprintSpeed = config.sprintSpeed;
			this.actualStaminaDrain = config.staminaDrainPerSecond;
			this.actualSprintIncrease = this.actualSprintSpeed - ModConfig.defaultFarmerSpeed;

			//modhelper.Multiplayer.SendMessage(this.actualSprintSpeed, "MessageTypeSprintSpeed", new[] { this.ModManifest.UniqueID });
			//modhelper.Multiplayer.SendMessage(this.actualStaminaDrain, "MessageTypeStamina", new[] { this.ModManifest.UniqueID });
			//}

			// If toggle sprinting and enableSprintWhenWaking, start sprinting now
			if(config.toggleSprint && config.enableSprintWhenWaking) {
				isPlayerToggleSprinting = true;
				StartSprint();
			}

		}

		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {

			if (!Context.IsWorldReady)
				return;

			if (!Context.IsPlayerFree)
				return;

			/*
			 * Toggle sprint
			 */
			if (config.toggleSprint) {
				if (modhelper.Input.GetState(config.sprintKey) == SButtonState.Released || modhelper.Input.GetState(config.alternateSprintKey) == SButtonState.Released) {

					isPlayerToggleSprinting = !isPlayerToggleSprinting;

					if (isPlayerToggleSprinting) {
						StartSprint();
					}

					else {
						StopSprint();
					}
				}
			}

			/*
			 * Hold to sprint
			 */
			else {
				if (modhelper.Input.GetState(config.sprintKey) == SButtonState.Pressed || modhelper.Input.GetState(config.alternateSprintKey) == SButtonState.Pressed) {
					StartSprint();
				}

				if (modhelper.Input.GetState(config.sprintKey) == SButtonState.Released || modhelper.Input.GetState(config.alternateSprintKey) == SButtonState.Released) {
					StopSprint();
				}
			}

			// addedSpeed gets set to 0 every 10minUpdate.
			// Re-add the sprint speed if we should be sprinting when this happens
			if(this.isPlayerSprinting && Game1.player.addedSpeed == 0) {
				Game1.player.addedSpeed += this.actualSprintIncrease;
			}

			// Exhausted, stop sprinting regardless
			if (Game1.player.stamina <= 0)
				StopSprint();
		}

		private void StartSprint() {

			if (this.isPlayerSprinting)
				return;

			if (Game1.player.stamina < this.actualStaminaDrain)
				return;

			this.isPlayerSprinting = true;
			Game1.player.addedSpeed += this.actualSprintIncrease;
		}

		private void StopSprint() {

			if (!this.isPlayerSprinting)
				return;

			this.isPlayerSprinting = false;
			this.isPlayerToggleSprinting = false;
			Game1.player.addedSpeed -= this.actualSprintIncrease;

			// If for whatever reason the added speed is negative, just set it to 0
			// This prevents crops and grass disappearing when sprinting,
			//  but will remove any negative speed modifiers (like long grass)
			if (Game1.player.addedSpeed < 0) {
				Monitor.Log("Player speed reset to 0", LogLevel.Trace);
				Game1.player.addedSpeed = 0;
			}
		}

		private void OnSecondTicked(object sender, OneSecondUpdateTickedEventArgs e) {
			if (!Context.IsWorldReady)
				return;

			if (!Context.IsPlayerFree)
				return;

			if (this.isPlayerSprinting && !Game1.paused && Game1.player.isMoving()) {
				float drain = Math.Min(Game1.player.Stamina, config.staminaDrainPerSecond);
				Game1.player.Stamina -= drain;
			}
		}

		/*
		 * Multiplayer config syncs
		 */

		//private void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs e) {
		//	if (!Context.IsMainPlayer)
		//		return;

		//	if (!config.multiplayerSync)
		//		return;

		//	modhelper.Multiplayer.SendMessage(this.actualSprintSpeed, "MessageTypeSprintSpeed", new[] { this.ModManifest.UniqueID }, new[] { e.Peer.PlayerID });
		//	modhelper.Multiplayer.SendMessage(this.actualStaminaDrain, "MessageTypeStamina", new[] { this.ModManifest.UniqueID }, new[] { e.Peer.PlayerID });
		//}

		//private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e) {
		//	if (e.FromModID == this.ModManifest.UniqueID) {
		//		if (e.Type == "MessageTypeSprintSpeed") {
		//			this.actualSprintSpeed = e.ReadAs<int>();
		//		}

		//		if (e.Type == "MessageTypeStamina") {
		//			this.actualStaminaDrain = e.ReadAs<float>();
		//		}
		//	}
		//}
	}

	class ModConfig {

		public static int defaultFarmerSpeed = 5;

		public int sprintSpeed { get; set; } = 7;
		public float staminaDrainPerSecond { get; set; } = 1f;

		public SButton sprintKey { get; set; } = SButton.LeftControl;
		public SButton alternateSprintKey { get; set; } = SButton.LeftStick;

		public bool toggleSprint { get; set; } = true;
		public bool enableSprintWhenWaking { get; set; } = true;
		//public bool multiplayerSync { get; set; } = true;
	}
}
