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
using Nanoray.Shrike;
using Nanoray.Shrike.Harmony;
using Newtonsoft.Json.Linq;
using Shockah.CommonModCode.GMCM;
using Shockah.Kokoro;
using Shockah.Kokoro.GMCM;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Shockah.DontStopMeNow;

public class ModEntry : BaseMod<ModConfig>
{
	private static ModEntry Instance { get; set; } = null!;
	private static bool IsDuringUpdateControlInputDelegate = false;
	private static bool IsDuringGetMovementSpeed = false;

	public override void MigrateConfig(ISemanticVersion? configVersion, ISemanticVersion modVersion)
	{
		if (configVersion is null)
			return;

		if (configVersion.IsOlderThan("2.0.0"))
		{
			if (Config.ExtensionData.TryGetValue("SlowMove", out var token) && token.Type == JTokenType.Boolean)
				Config.MoveSpeed = token.Value<bool>() ? 0.5f : 1f;
		}
	}

	public override void OnEntry(IModHelper helper)
	{
		Instance = this;
		var harmony = new Harmony(ModManifest.UniqueID);

		helper.Events.GameLoop.GameLaunched += (_, _) => OnGameLaunched(harmony);
		helper.Events.GameLoop.UpdateTicking += OnUpdateTicking;

		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.DeclaredMethod(typeof(Farmer), nameof(Farmer.getMovementSpeed)),
			prefix: new HarmonyMethod(GetType(), nameof(Farmer_getMovementSpeed_Prefix)),
			postfix: new HarmonyMethod(GetType(), nameof(Farmer_getMovementSpeed_Postfix)),
			finalizer: new HarmonyMethod(GetType(), nameof(Farmer_getMovementSpeed_Finalizer))
		);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.DeclaredMethod(typeof(Farmer), nameof(Farmer.updateMovementAnimation)),
			transpiler: new HarmonyMethod(GetType(), nameof(Farmer_updateMovementAnimation_Transpiler))
		);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => typeof(Game1).GetNestedTypes(AccessTools.all).SelectMany(t => t.GetMethods(AccessTools.all)).First(m => m.Name.StartsWith("<UpdateControlInput>") && m.ReturnType == typeof(void)),
			prefix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Game1_UpdateControlInput_Delegate_Prefix))),
			finalizer: new HarmonyMethod(AccessTools.Method(GetType(), nameof(Game1_UpdateControlInput_Delegate_Finalizer)))
		);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.DeclaredMethod(typeof(Farmer), nameof(Farmer.canStrafeForToolUse)),
			postfix: new HarmonyMethod(GetType(), nameof(Farmer_canStrafeForToolUse_Postfix))
		);
	}

	private void OnGameLaunched(Harmony harmony)
	{
		SetupConfig();

		harmony.TryPatchVirtual(
			monitor: Monitor,
			original: () => AccessTools.DeclaredMethod(typeof(Tool), nameof(Tool.beginUsing)),
			prefix: new HarmonyMethod(GetType(), nameof(Tool_beginUsingTool_Prefix)),
			postfix: new HarmonyMethod(GetType(), nameof(Tool_beginUsingTool_Postfix))
		);
		harmony.TryPatchVirtual(
			monitor: Monitor,
			original: () => AccessTools.DeclaredMethod(typeof(MeleeWeapon), nameof(MeleeWeapon.leftClick)),
			postfix: new HarmonyMethod(GetType(), nameof(MeleeWeapon_leftClick_Postfix))
		);
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
			save: () =>
			{
				WriteConfig();
				LogConfig();
			}
		);

		helper.AddSectionTitle("config.movement.section");
		helper.AddBoolOption("config.movement.overrideSpeed", () => Config.OverrideMoveSpeed);
		helper.AddNumberOption("config.movement.speed", () => Config.MoveSpeed, min: 0f, max: 2f, interval: 0.05f);
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

	private void OnUpdateTicking(object? sender, UpdateTickingEventArgs e)
	{
		if (!Context.IsPlayerFree)
			return;
		if (!Config.FixChargingToolFacing)
			return;
		if (!Game1.player.UsingTool)
			return;
		if (!Game1.player.IsLocalPlayer)
			return;
		if (Game1.player.CurrentTool is not { } tool)
			return;

		if (ShouldFixFacing(Game1.player, tool))
			FixFacingDirectionIfNeeded();
	}

	private void FixFacingDirectionIfNeeded()
	{
		if (Game1.options.gamepadControls)
		{
			if (Config.FixFacingOnController)
				FixControllerFacingDirection();
		}
		else
		{
			if (Config.FixFacingOnMouse)
				FixMouseFacingDirection();
		}
	}

	private static void FixControllerFacingDirection()
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

	private static void FixMouseFacingDirection()
	{
		var player = Game1.player;
		var cursor = new Vector2(Game1.viewport.X + Game1.getOldMouseX(), Game1.viewport.Y + Game1.getOldMouseY());
		var direction = cursor - player.GetBoundingBox().Center.ToVector2();
		FixFacingDirection(direction);
	}

	private static void FixFacingDirection(Vector2 direction)
	{
		var player = Game1.player;
		if (Math.Abs(direction.X) > Math.Abs(direction.Y))
			player.FacingDirection = direction.X >= 0 ? Game1.right : Game1.left;
		else
			player.FacingDirection = direction.Y >= 0 ? Game1.down : Game1.up;
	}

	private static bool? IsUsingPoweredUpOnHoldTool(Farmer player)
	{
		if (!player.UsingTool)
			return false;
		if (player.toolHold.Value == 0 && player.toolPower.Value == 0)
			return null;
		return player.toolHold.Value > 0 || player.toolPower.Value > 0;
	}

	private bool ShouldAllowMovement(Farmer player, Tool tool)
	{
		if (tool is MeleeWeapon weapon)
			return weapon.isOnSpecial ? Config.MoveWhileSpecial : Config.MoveWhileSwingingMeleeWeapons;
		if (tool is Slingshot)
			return Config.MoveWhileAimingSlingshot;

		return IsUsingPoweredUpOnHoldTool(player) switch
		{
			true => Config.MoveWhileChargingTools,
			false => Config.MoveWhileSwingingTools,
			null => Config.MoveWhileChargingTools || Config.MoveWhileSwingingTools
		};
	}

	private bool ShouldFixFacing(Farmer player, Tool tool)
	{
		if (tool is MeleeWeapon weapon)
			return !weapon.isOnSpecial && Config.FixMeleeWeaponFacing;
		if (tool is Slingshot)
			return false;
		if (tool is FishingRod)
			return Config.FixFishingRodFacing;

		return IsUsingPoweredUpOnHoldTool(player) switch
		{
			true => Config.FixChargingToolFacing,
			false or null => Config.FixToolFacing
		};
	}

	private static void Farmer_getMovementSpeed_Prefix()
		=> IsDuringGetMovementSpeed = true;

	private static void Farmer_getMovementSpeed_Finalizer()
		=> IsDuringGetMovementSpeed = false;

	private static void Farmer_getMovementSpeed_Postfix(Farmer __instance, ref float __result)
	{
		if (__instance.UsingTool)
			__result *= Instance.Config.MoveSpeed;
	}

	private static void Game1_UpdateControlInput_Delegate_Prefix()
		=> IsDuringUpdateControlInputDelegate = true;

	private static void Game1_UpdateControlInput_Delegate_Finalizer()
		=> IsDuringUpdateControlInputDelegate = false;

	private static void Farmer_canStrafeForToolUse_Postfix(Farmer __instance, ref bool __result)
	{
		if (Instance.Config.OverrideMoveSpeed && IsDuringGetMovementSpeed)
		{
			__result = false;
			return;
		}

		if (__result)
			return;

		if (__instance.CurrentTool is { } tool && Instance.ShouldAllowMovement(__instance, tool))
			__result = true;
	}

	private static IEnumerable<CodeInstruction> Farmer_updateMovementAnimation_Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase originalMethod)
	{
		try
		{
			return new SequenceBlockMatcher<CodeInstruction>(instructions)
				.Find(
					ILMatches.Ldarg(0).ExtractLabels(out var startLabels),
					ILMatches.Call("get_FarmerSprite"),
					ILMatches.Call("get_PauseForSingleAnimation"),
					ILMatches.Brtrue,
					ILMatches.Ldarg(0),
					ILMatches.Call("get_UsingTool"),
					ILMatches.Brfalse.GetBranchTarget(out var allowMovementBranchTarget),
					ILMatches.Instruction(OpCodes.Ret)
				)
				.Replace(
					new CodeInstruction(OpCodes.Ldarg_0).WithLabels(startLabels),
					new CodeInstruction(OpCodes.Call, AccessTools.DeclaredMethod(MethodBase.GetCurrentMethod()!.DeclaringType!, nameof(Farmer_updateMovementAnimation_Transpiler_ShouldAllowMovement))),
					new CodeInstruction(OpCodes.Brtrue, allowMovementBranchTarget.Value),
					new CodeInstruction(OpCodes.Ret)
				)
				.AllElements();
		}
		catch (Exception ex)
		{
			Instance.Monitor.Log($"Could not patch method {originalMethod} - {Instance.ModManifest.Name} probably won't work.\nReason: {ex}", LogLevel.Error);
			return instructions;
		}
	}

	private static bool Farmer_updateMovementAnimation_Transpiler_ShouldAllowMovement(Farmer who)
	{
		var result = !(who.FarmerSprite.PauseForSingleAnimation || who.UsingTool);
		if (result)
			return true;

		if (who.CurrentTool is { } tool && Instance.ShouldAllowMovement(who, tool))
			result = true;
		return result;
	}

	private static void Tool_beginUsingTool_Prefix(Tool __instance, Farmer __3 /* who */)
	{
		if (Instance.ShouldFixFacing(__3, __instance) && __3.IsLocalPlayer)
			Instance.FixFacingDirectionIfNeeded();
	}

	private static void Tool_beginUsingTool_Postfix(Tool __instance, Farmer __3 /* who */)
	{
		if (Instance.ShouldAllowMovement(__3, __instance))
			__3.CanMove = true;
	}

	private static void MeleeWeapon_leftClick_Postfix(MeleeWeapon __instance, Farmer __0 /* who */)
	{
		if (Instance.ShouldAllowMovement(__0, __instance))
			__0.CanMove = true;
	}
}