/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Shockah.CommonModCode.GMCM;
using Shockah.CommonModCode.IL;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.DontStopMeNow
{
	public class DontStopMeNow: Mod
	{
		private static DontStopMeNow Instance { get; set; } = null!;

		internal ModConfig Config { get; private set; } = null!;

		private readonly IList<Farmer> NotRunningPlayers = new List<Farmer>();
		private readonly IList<Farmer> PlayersToStopMovingInTwoTicks = new List<Farmer>();
		private readonly IList<Farmer> PlayersToStopMovingNextTick = new List<Farmer>();
		private readonly PerScreen<SButton?> LastToolButton = new(null);

		public override void Entry(IModHelper helper)
		{
			Instance = this;

			Config = helper.ReadConfig<ModConfig>();

			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.Input.ButtonPressed += OnButtonPressed;
			helper.Events.Input.ButtonReleased += OnButtonReleased;

			var harmony = new Harmony(ModManifest.UniqueID);
			try
			{
				harmony.Patch(
					original: AccessTools.Method(typeof(Farmer), nameof(Farmer.setRunning)),
					postfix: new HarmonyMethod(typeof(DontStopMeNow), nameof(Farmer_setRunning_Postfix))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(Farmer), nameof(Farmer.BeginUsingTool)),
					postfix: new HarmonyMethod(typeof(DontStopMeNow), nameof(Farmer_BeginUsingTool_Postfix))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(MeleeWeapon), nameof(MeleeWeapon.leftClick)),
					postfix: new HarmonyMethod(typeof(DontStopMeNow), nameof(MeleeWeapon_leftClick_Postfix))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(MeleeWeapon), "beginSpecialMove"),
					postfix: new HarmonyMethod(typeof(DontStopMeNow), nameof(MeleeWeapon_beginSpecialMove_Postfix))
				);

				foreach (var nestedType in typeof(Game1).GetTypeInfo().DeclaredNestedTypes)
				{
					if (!nestedType.DeclaredFields.Where(f => f.FieldType == typeof(Game1) && f.Name.EndsWith("__this")).Any())
						continue;
					if (!nestedType.DeclaredFields.Where(f => f.FieldType == typeof(KeyboardState) && f.Name == "currentKBState").Any())
						continue;
					if (!nestedType.DeclaredFields.Where(f => f.FieldType == typeof(MouseState) && f.Name == "currentMouseState").Any())
						continue;
					if (!nestedType.DeclaredFields.Where(f => f.FieldType == typeof(GamePadState) && f.Name == "currentPadState").Any())
						continue;
					if (!nestedType.DeclaredFields.Where(f => f.FieldType == typeof(GameTime) && f.Name == "time").Any())
						continue;

					foreach (var method in nestedType.DeclaredMethods)
					{
						if (!method.Name.StartsWith("<UpdateControlInput>"))
							continue;

						harmony.Patch(
							original: method,
							transpiler: new HarmonyMethod(typeof(DontStopMeNow), nameof(Game1_UpdateControlInput_Transpiler))
						);
						goto done;
					}
				}

				Monitor.Log($"Could not patch methods - Don't Stop Me Now probably won't work.\nReason: Cannot patch Game1.UpdateControlInput/hooks/OnGame1_UpdateControlInput/Delegate.", LogLevel.Error);
				done:;
			}
			catch (Exception e)
			{
				Monitor.Log($"Could not patch methods - Don't Stop Me Now probably won't work.\nReason: {e}", LogLevel.Error);
			}
		}

		private void SetupConfig()
		{
			var api = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api is null)
				return;
			var helper = new GMCMI18nHelper(api, ModManifest, Helper.Translation);

			api.Register(
				ModManifest,
				reset: () => Config = new ModConfig(),
				save: () => Helper.WriteConfig(Config)
			);

			helper.AddSectionTitle("config.movement.section");
			helper.AddBoolOption("config.movement.slowMove", () => Config.SlowMove);
			helper.AddBoolOption("config.movement.tools", () => Config.MoveWhileSwingingTools);
			helper.AddBoolOption("config.movement.meleeWeapons", () => Config.MoveWhileSwingingMeleeWeapons);
			helper.AddBoolOption("config.movement.special", () => Config.MoveWhileSpecial);
			helper.AddBoolOption("config.movement.aimingSlingshot", () => Config.MoveWhileAimingSlingshot);
			helper.AddBoolOption("config.movement.chargingTools", () => Config.MoveWhileChargingTools);

			helper.AddSectionTitle("config.facing.section");
			helper.AddBoolOption("config.facing.tools", () => Config.FixToolFacing);
			helper.AddBoolOption("config.facing.meleeWeapons", () => Config.FixMeleeWeaponFacing);
			helper.AddBoolOption("config.facing.chargingTools", () => Config.FixChargingToolFacing);
			helper.AddBoolOption("config.facing.fishingRod", () => Config.FixFishingRodFacing);
			helper.AddBoolOption("config.facing.mouse", () => Config.FixFacingOnMouse);
			helper.AddBoolOption("config.facing.controller", () => Config.FixFacingOnController);
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			SetupConfig();
		}

		private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
		{
			LastToolButton.Value = null;
		}

		private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
		{
			if (!Context.IsPlayerFree)
				return;
			if (!Config.FixChargingToolFacing)
				return;
			var player = Game1.player;
			if (!player.UsingTool)
				return;

			if (ShouldFixFacing(player) && LastToolButton.Value is not null)
				FixFacingDirectionIfNeeded();
		}

		private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
		{
			foreach (var playerToStopMoving in PlayersToStopMovingNextTick)
			{
				if (playerToStopMoving.CanMove && !ShouldAllowMovement(playerToStopMoving, true))
					playerToStopMoving.CanMove = false;
			}
			PlayersToStopMovingNextTick.Clear();

			foreach (var playerToStopMoving in PlayersToStopMovingInTwoTicks)
				PlayersToStopMovingNextTick.Add(playerToStopMoving);
			PlayersToStopMovingInTwoTicks.Clear();

			foreach (var notRunningPlayer in NotRunningPlayers.ToList())
			{
				if (!notRunningPlayer.UsingTool)
				{
					NotRunningPlayers.Remove(notRunningPlayer);
					notRunningPlayer.setRunning(true);
				}
			}
		}

		private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
		{
			if (!Context.IsPlayerFree)
				return;
			if (!e.Button.IsUseToolButton() && !e.Button.IsActionButton())
				return;
			var player = Game1.player;
			if (!(player.CurrentTool is MeleeWeapon ? Config.FixMeleeWeaponFacing : Config.FixToolFacing))
				return;

			LastToolButton.Value = e.Button;
			if (ShouldFixFacing(player))
				FixFacingDirectionIfNeeded();
		}

		private void OnButtonReleased(object? sender, ButtonReleasedEventArgs e)
		{
			if (!Context.IsPlayerFree)
				return;
			if (!e.Button.IsUseToolButton() && !e.Button.IsActionButton())
				return;
			if (LastToolButton.Value != e.Button)
				return;
			LastToolButton.Value = null;
		}

		private void FixFacingDirectionIfNeeded()
		{
			if (LastToolButton.Value is null)
				return;
			switch (LastToolButton.Value.Value.GetButtonType())
			{
				case InputHelper.ButtonType.Gamepad:
					if (!Config.FixFacingOnController)
						return;
					FixControllerFacingDirection();
					break;
				case InputHelper.ButtonType.Keyboard:
				case InputHelper.ButtonType.Mouse:
					if (!Config.FixFacingOnMouse)
						return;
					FixMouseFacingDirection();
					break;
				default:
					throw new ArgumentException($"{nameof(InputHelper.ButtonType)} has an invalid value.");
			}
		}

		private void FixControllerFacingDirection()
		{
			var thumbStickDirection = Game1.oldPadState.ThumbSticks.Left;
			if (Math.Abs(thumbStickDirection.X) < 0.2)
				thumbStickDirection.X = 0;
			if (Math.Abs(thumbStickDirection.Y) < 0.2)
				thumbStickDirection.Y = 0;
			if (Game1.oldPadState.IsButtonDown(Buttons.DPadLeft))
				thumbStickDirection.X = -1;
			if (Game1.oldPadState.IsButtonDown(Buttons.DPadRight))
				thumbStickDirection.X = 1;
			if (Game1.oldPadState.IsButtonDown(Buttons.DPadUp))
				thumbStickDirection.Y = 1;
			if (Game1.oldPadState.IsButtonDown(Buttons.DPadDown))
				thumbStickDirection.Y = -1;

			thumbStickDirection.Y *= -1;
			if (thumbStickDirection.LengthSquared() > 0.2f)
				FixFacingDirection(thumbStickDirection);
		}

		private void FixMouseFacingDirection()
		{
			var player = Game1.player;
			var cursor = new Vector2(Game1.viewport.X + Game1.getOldMouseX(), Game1.viewport.Y + Game1.getOldMouseY());
			var direction = cursor - player.GetBoundingBox().Center.ToVector2();
			FixFacingDirection(direction);
		}

		private void FixFacingDirection(Vector2 direction)
		{
			var player = Game1.player;
			if (Math.Abs(direction.X) > Math.Abs(direction.Y))
				player.FacingDirection = direction.X >= 0 ? Game1.right : Game1.left;
			else
				player.FacingDirection = direction.Y >= 0 ? Game1.down : Game1.up;
		}

		private bool? IsUsingPoweredUpOnHoldTool(Farmer player)
		{
			if (!player.UsingTool)
				return false;
			if (player.toolHold == 0 && player.toolPower == 0)
				return null;
			return player.toolHold > 0 || player.toolPower > 0;
		}

		private bool ShouldAllowMovement(Farmer player, bool isSecondTick = false)
		{
			if (player.CurrentTool is MeleeWeapon weapon)
			{
				return weapon.isOnSpecial ? Instance.Config.MoveWhileSpecial : Instance.Config.MoveWhileSwingingMeleeWeapons;
			}
			else if (player.CurrentTool is Slingshot)
			{
				return Instance.Config.MoveWhileAimingSlingshot;
			}
			else
			{
				switch (IsUsingPoweredUpOnHoldTool(player))
				{
					case true:
						return Instance.Config.MoveWhileChargingTools;
					case false:
						return Instance.Config.MoveWhileSwingingTools;
					case null:
						return isSecondTick ? Instance.Config.MoveWhileSwingingTools : Instance.Config.MoveWhileChargingTools || Instance.Config.MoveWhileSwingingTools;
				}
			}
		}

		private bool ShouldFixFacing(Farmer player)
		{
			if (player.CurrentTool is MeleeWeapon weapon)
			{
				return !weapon.isOnSpecial && Config.FixMeleeWeaponFacing;
			}
			else if (player.CurrentTool is Slingshot)
			{
				return false;
			}
			else if (player.CurrentTool is FishingRod)
			{
				return Config.FixFishingRodFacing;
			}
			else
			{
				switch (IsUsingPoweredUpOnHoldTool(player))
				{
					case true:
						return Config.FixChargingToolFacing;
					case false:
					case null:
						return Config.FixToolFacing;
				}
			}
		}

		private static bool Game1_UpdateControlInput_Transpiler_UsingToolReplacement()
		{
			var player = Game1.player;
			return player.UsingTool && !Instance.ShouldAllowMovement(player);
		}

		private static IEnumerable<CodeInstruction> Game1_UpdateControlInput_Transpiler(IEnumerable<CodeInstruction> enumerableInstructions)
		{
			var instructions = enumerableInstructions.ToList();

			// IL to find:
			// IL_15d5: call class StardewValley.Farmer StardewValley.Game1::get_player()
			// IL_15da: callvirt instance bool StardewValley.Farmer::get_UsingTool()
			// IL_15df: brtrue IL_17a0
			var worker = TranspileWorker.FindInstructions(instructions, new Func<CodeInstruction, bool>[]
			{
				i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == AccessTools.Method(typeof(Game1), "get_player"),
				i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == AccessTools.Method(typeof(Farmer), "get_UsingTool"),
				i => i.opcode == OpCodes.Brtrue
			});
			if (worker is null)
			{
				Instance.Monitor.Log($"Could not patch methods - Don't Stop Me Now probably won't work.\nReason: Could not find IL to transpile.", LogLevel.Error);
				return instructions;
			}

			worker[0] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DontStopMeNow), nameof(Game1_UpdateControlInput_Transpiler_UsingToolReplacement)));
			worker[1] = new CodeInstruction(OpCodes.Nop);

			return instructions;
		}

		private static void Farmer_setRunning_Postfix(Farmer __instance)
		{
			if (Instance.NotRunningPlayers.Contains(__instance))
			{
				__instance.running = false;
				__instance.speed = 2;
			}
		}

		private static void Farmer_BeginUsingTool_Postfix(Farmer __instance)
		{
			if (!__instance.CanMove && Instance.ShouldAllowMovement(__instance))
			{
				__instance.CanMove = true;
				Instance.PlayersToStopMovingInTwoTicks.Add(__instance);
				if (Instance.Config.SlowMove)
				{
					__instance.setRunning(false);
					Instance.NotRunningPlayers.Add(__instance);
				}
			}
		}

		private static void MeleeWeapon_leftClick_Postfix(Farmer who)
		{
			if (!who.CanMove && Instance.ShouldAllowMovement(who))
			{
				who.CanMove = true;
				Instance.PlayersToStopMovingInTwoTicks.Add(who);
			}
		}

		private static void MeleeWeapon_beginSpecialMove_Postfix(Farmer who)
		{
			if (!who.CanMove && Instance.ShouldAllowMovement(who))
			{
				who.CanMove = true;
				Instance.PlayersToStopMovingInTwoTicks.Add(who);
			}
		}
	}
}