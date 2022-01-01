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
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Sprint {
	class ModEntry : Mod {

		public static Mod INSTANCE;
		public static IModHelper modhelper;
		public static ITranslationHelper i18n;

		public static ModConfig config;

		//private bool isPlayerSprinting = false;
		//private bool isPlayerToggleSprinting = false;

		private readonly PerScreen<bool> isPlayerSprinting = new PerScreen<bool>(createNewState: () => false);
		private readonly PerScreen<bool> isPlayerToggleSprinting = new PerScreen<bool>(createNewState: () => false);

		private int actualSprintSpeed = 0;
		private float actualStaminaDrain = 0;
		private int actualSprintIncrease = 0;

		private bool enableFloorSpeed = true;
		private int additionalFloorSpeed = 0;

		//private Vector2 currentTile = new Vector2(0, 0);
		//private bool wasLastTilePath = false;

		private readonly PerScreen<Vector2> currentTile = new PerScreen<Vector2>(createNewState: () => new Vector2(0, 0));
		private readonly PerScreen<bool> wasLastTilePath = new PerScreen<bool>(createNewState: () => false);

		private const string MESSAGE_SPRINT_SPEED = "SprintSpeed";
		private const string MESSAGE_SPRINT_STAMINA = "Stamina";
		private const string MESSAGE_ENABLE_FLOOR = "EnableFloor";
		private const string MESSAGE_FLOOR_SPEED = "FloorSpeed";

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

			// Get the default player speed, just in case it's changed from the default

			ModConfig.defaultFarmerSpeed = 5;

			this.actualSprintSpeed = config.sprintSpeed;
			this.actualStaminaDrain = config.staminaDrainPerSecond;
			this.actualSprintIncrease = this.actualSprintSpeed - ModConfig.defaultFarmerSpeed;

			this.enableFloorSpeed = config.fasterOnFloorTiles;
			this.additionalFloorSpeed = config.additionalFloorSpeed;

			modhelper.Events.GameLoop.DayStarted += OnDayStarted;

			modhelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			modhelper.Events.GameLoop.OneSecondUpdateTicked += OnSecondTicked;

			modhelper.Events.Multiplayer.PeerContextReceived += OnPeerContextReceived;
			modhelper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;

			modhelper.Events.Input.ButtonPressed += OnButtonPressed;
			modhelper.Events.Input.ButtonReleased += OnButtonReleased;
		}

		private void OnDayStarted(object sender, DayStartedEventArgs e) {			

			if (Context.IsMainPlayer && !Game1.hasLocalClientsOnly) {
				SendConfigSyncMessages(null);
			}

			// If toggle sprinting and enableSprintWhenWaking, start sprinting now
			if (config.toggleSprint && config.enableSprintWhenWaking) {
				isPlayerToggleSprinting.Value = true;
				StartSprint();
			}

		}

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
			if (!(e.Button == config.sprintKey || e.Button == config.alternateSprintKey))
				return;

			//Monitor.Log($"{e.Button} pressed by player {Context.ScreenId}", LogLevel.Warn);

			if (!config.toggleSprint) {
				StartSprint();
			}

		}

		private void OnButtonReleased(object sender, ButtonReleasedEventArgs e) {


			if (!(e.Button == config.sprintKey || e.Button == config.alternateSprintKey))
				return;

			/*
			 * Toggle sprint
			 */
			if (config.toggleSprint) {
				//if (modhelper.Input.GetState(config.sprintKey) == SButtonState.Released || modhelper.Input.GetState(config.alternateSprintKey) == SButtonState.Released) {

					isPlayerToggleSprinting.Value = !isPlayerToggleSprinting.Value;

					if (isPlayerToggleSprinting.Value) {
						StartSprint();
					} else {
						StopSprint();
					}
				//}
			}

			/*
			 * Hold to sprint - stop sprinting here
			 */
			else {
				//if (modhelper.Input.GetState(config.sprintKey) == SButtonState.Pressed || modhelper.Input.GetState(config.alternateSprintKey) == SButtonState.Pressed) {
				//	StartSprint();
				//}

				//if (modhelper.Input.GetState(config.sprintKey) == SButtonState.Released || modhelper.Input.GetState(config.alternateSprintKey) == SButtonState.Released) {
					StopSprint();
				//}
			}

		}

		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {

			if (!Context.IsWorldReady)
				return;

			if (!Context.IsPlayerFree)
				return;

			/*
			 * Floor tile speed boost
			 */

			if (this.isPlayerSprinting.Value && this.enableFloorSpeed) {

				if(currentTile.Value != Game1.player.getTileLocation()) {

					currentTile.Value = Game1.player.getTileLocation();
					GameLocation location = Game1.player.currentLocation;

					bool isThisTilePath = isTileAPath(location, currentTile.Value);

					Monitor.Log($"Stepping in new tile: {currentTile}. Is path: {isThisTilePath}", LogLevel.Trace);

					if (isThisTilePath && wasLastTilePath.Value) {

						// Reset the current added speed and re-add, but with the tile mod
						Game1.player.addedSpeed -= this.actualSprintIncrease;
						this.actualSprintIncrease = this.actualSprintSpeed - ModConfig.defaultFarmerSpeed + this.additionalFloorSpeed;

						Game1.player.addedSpeed += this.actualSprintIncrease;
					}

					else {
						// Reset the current added speed and re-add, but without the tile mod
						Game1.player.addedSpeed -= this.actualSprintIncrease;
						this.actualSprintIncrease = this.actualSprintSpeed - ModConfig.defaultFarmerSpeed;

						Game1.player.addedSpeed += this.actualSprintIncrease;
					}

					wasLastTilePath.Value = isThisTilePath;
				}
			}

			// addedSpeed gets set to 0 every 10minUpdate.
			// Re-add the sprint speed if we should be sprinting when this happens
			if (this.isPlayerSprinting.Value && Game1.player.addedSpeed == 0) {
				Game1.player.addedSpeed += this.actualSprintIncrease;
			}

			// Exhausted, stop sprinting regardless
			if (Game1.player.stamina <= 0)
				StopSprint();
		}

		private void StartSprint() {

			if (this.isPlayerSprinting.Value)
				return;

			if (Game1.player.stamina < this.actualStaminaDrain)
				return;

			this.isPlayerSprinting.Value = true;
			Game1.player.addedSpeed += this.actualSprintIncrease;
		}

		private void StopSprint() {

			if (!this.isPlayerSprinting.Value)
				return;

			this.isPlayerSprinting.Value = false;
			this.isPlayerToggleSprinting.Value = false;
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

			if (this.isPlayerSprinting.Value && !Game1.paused && Game1.player.isMoving()) {
				float drain = Math.Min(Game1.player.Stamina, this.actualStaminaDrain);
				Game1.player.Stamina -= drain;
			}
		}

		/*
		 * Multiplayer config syncs
		 */

		private void OnPeerContextReceived(object sender, PeerContextReceivedEventArgs e) {
			if (!Context.IsMainPlayer)
				return;

			if (!config.multiplayerSync)
				return;


			SendConfigSyncMessages(new[] { e.Peer.PlayerID });
		}
		
		private void SendConfigSyncMessages(long[] playerIds ) {

			if (!Context.IsMainPlayer)
				return;

			if (!config.multiplayerSync)
				return;

			Monitor.Log("Sending config sync messages", LogLevel.Info);

			modhelper.Multiplayer.SendMessage<int>(this.actualSprintSpeed, MESSAGE_SPRINT_SPEED, new[] { this.ModManifest.UniqueID }, playerIds);
			modhelper.Multiplayer.SendMessage<float>(this.actualStaminaDrain, MESSAGE_SPRINT_STAMINA, new[] { this.ModManifest.UniqueID }, playerIds);
			modhelper.Multiplayer.SendMessage<bool>(this.enableFloorSpeed, MESSAGE_ENABLE_FLOOR, new[] { this.ModManifest.UniqueID }, playerIds);
			modhelper.Multiplayer.SendMessage<int>(this.additionalFloorSpeed, MESSAGE_FLOOR_SPEED, new[] { this.ModManifest.UniqueID }, playerIds);
		}

		private void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e) {

			//Monitor.Log($"Received mod message: {e.FromModID}:{e.Type}:{e.ReadAs<string>()}", LogLevel.Info);

			if (e.FromModID == this.ModManifest.UniqueID) {
				if (e.Type == MESSAGE_SPRINT_SPEED) {
					this.actualSprintSpeed = e.ReadAs<int>();
				}

				if (e.Type == MESSAGE_SPRINT_STAMINA) {
					this.actualStaminaDrain = e.ReadAs<float>();
				}

				if(e.Type == MESSAGE_ENABLE_FLOOR) {
					this.enableFloorSpeed = e.ReadAs<bool>();
				}

				if(e.Type == MESSAGE_FLOOR_SPEED) {
					this.additionalFloorSpeed = e.ReadAs<int>();
				}
			}
		}

		private bool isTileAPath(GameLocation location, Vector2 tile) {
			if(location.terrainFeatures.ContainsKey(tile)) {
				TerrainFeature tf = location.terrainFeatures[tile];
				if(tf is Flooring) {
					return true;
				}
			}

			return false;
		}
	}

	class ModConfig {

		public static int defaultFarmerSpeed = 5;

		public int sprintSpeed { get; set; } = 7;
		public float staminaDrainPerSecond { get; set; } = 1f;

		public SButton sprintKey { get; set; } = SButton.LeftControl;
		public SButton alternateSprintKey { get; set; } = SButton.LeftStick;

		public bool toggleSprint { get; set; } = true;
		public bool enableSprintWhenWaking { get; set; } = true;

		public bool fasterOnFloorTiles { get; set; } = true;
		public int additionalFloorSpeed { get; set; } = 1;

		public bool multiplayerSync { get; set; } = true;
	}
}
