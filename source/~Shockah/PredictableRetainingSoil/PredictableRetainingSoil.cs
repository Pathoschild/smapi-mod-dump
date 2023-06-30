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
using Netcode;
using Shockah.Kokoro;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Shockah.PredictableRetainingSoil;

public class PredictableRetainingSoil : BaseMod<ModConfig>, IPredictableRetainingSoilApi
{
	private const int BasicRetainingSoilID = 370;
	private const int QualityRetainingSoilID = 371;
	private const int DeluxeRetainingSoilID = 920;

	private static readonly string MultiFertilizerModQualifiedName = "MultiFertilizer.Mod, MultiFertilizer";
	private static readonly string MultiFertilizerDirtHelperQualifiedName = "MultiFertilizer.Framework.DirtHelper, MultiFertilizer";
	private static readonly string MultiFertilizerFertilizerDataQualifiedName = "MultiFertilizer.Framework.FertilizerData, MultiFertilizer";

	internal static PredictableRetainingSoil Instance { get; private set; } = null!;
	private IFluent<string> Fluent { get; set; } = null!;

	private bool IsMultiFertilizerLoaded { get; set; } = false;
	private Func<string?> MultiFertilizerKeyRetain { get; set; } = null!;
	private Func<HoeDirt, int?> GetMultiFertilizerRetainingSoilType { get; set; } = null!;

	private bool IsStayingWateredViaRetainingSoil = false;
	private List<(WeakReference<NetInt> State, WeakReference<HoeDirt> Soil)> TrackedSoilStateValues { get; set; } = new();

	public override void MigrateConfig(ISemanticVersion? configVersion, ISemanticVersion modVersion)
	{
		// do nothing, for now
	}

	public override void OnEntry(IModHelper helper)
	{
		Instance = this;

		Config = helper.ReadConfig<ModConfig>();

		helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		helper.Events.GameLoop.DayStarted += OnDayStarted;

		var harmony = new Harmony(ModManifest.UniqueID);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.Constructor(typeof(HoeDirt)),
			postfix: new HarmonyMethod(typeof(PredictableRetainingSoil), nameof(HoeDirt_ctor_Postfix))
		);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.dayUpdate)),
			prefix: new HarmonyMethod(typeof(PredictableRetainingSoil), nameof(HoeDirt_dayUpdate_Prefix)),
			postfix: new HarmonyMethod(typeof(PredictableRetainingSoil), nameof(HoeDirt_dayUpdate_Postfix))
		);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.Method(typeof(HoeDirt), nameof(HoeDirt.plant)),
			postfix: new HarmonyMethod(typeof(PredictableRetainingSoil), nameof(HoeDirt_plant_Postfix))
		);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.Method(typeof(NetInt), nameof(NetInt.Set)),
			postfix: new HarmonyMethod(typeof(PredictableRetainingSoil), nameof(NetInt_Set_Postfix))
		);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.Constructor(typeof(CraftingRecipe), new Type[] { typeof(string), typeof(bool) }),
			postfix: new HarmonyMethod(typeof(PredictableRetainingSoil), nameof(CraftingRecipe_Constructor_Postfix))
		);
		harmony.TryPatch(
			monitor: Monitor,
			original: () => AccessTools.Method(typeof(SObject), nameof(SObject.getDescription)),
			postfix: new HarmonyMethod(typeof(PredictableRetainingSoil), nameof(Object_getDescription_Postfix))
		);
	}

	public override object GetApi()
		=> this;

	private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
	{
		var fluentApi = Helper.ModRegistry.GetApi<IFluentApi>("Shockah.ProjectFluent")!;
		Fluent = fluentApi.GetLocalizationsForCurrentLocale(ModManifest);

		if (Helper.ModRegistry.IsLoaded("spacechase0.MultiFertilizer"))
		{
			IsMultiFertilizerLoaded = true;

			var multiFertilizerKeyRetainPropertyGetter = AccessTools.PropertyGetter(Type.GetType(MultiFertilizerModQualifiedName), "KeyRetain");
			MultiFertilizerKeyRetain = () =>
			{
				if (!IsMultiFertilizerLoaded)
					return null;

				try
				{
					return (string?)multiFertilizerKeyRetainPropertyGetter.Invoke(null, null);
				}
				catch (Exception e)
				{
					Monitor.Log($"There was a problem with MultiFertilizer compatibility.\nReason: {e}", LogLevel.Error);
					IsMultiFertilizerLoaded = false;
					return null;
				}
			};

			var tryGetFertilizerMethod = AccessTools.DeclaredMethod(Type.GetType(MultiFertilizerDirtHelperQualifiedName), "TryGetFertilizer", new Type[] { typeof(HoeDirt), typeof(string), Type.GetType(MultiFertilizerFertilizerDataQualifiedName)!.MakeByRefType() });
			var levelPropertyGetter = AccessTools.PropertyGetter(Type.GetType(MultiFertilizerFertilizerDataQualifiedName), "Level");
			GetMultiFertilizerRetainingSoilType = soil =>
			{
				if (!IsMultiFertilizerLoaded)
					return null;

				try
				{
					var parameters = new object?[] { soil, MultiFertilizerKeyRetain(), null };
					if ((bool?)tryGetFertilizerMethod.Invoke(null, parameters) is true)
					{
						var fertilizerData = parameters[2];
						var level = (int?)levelPropertyGetter.Invoke(fertilizerData, null);
						return level switch
						{
							1 => BasicRetainingSoilID,
							2 => QualityRetainingSoilID,
							3 => DeluxeRetainingSoilID,
							_ => null,
						};
					}
					return null;
				}
				catch (Exception e)
				{
					Monitor.Log($"There was a problem with MultiFertilizer compatibility.\nReason: {e}", LogLevel.Error);
					IsMultiFertilizerLoaded = false;
					return null;
				}
			};
		}

		IsMultiFertilizerLoaded = Helper.ModRegistry.IsLoaded("spacechase0.MultiFertilizer");

		var sc = Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore")!;
		sc.RegisterCustomProperty(
			typeof(HoeDirt),
			"RetainingSoilDaysLeft",
			typeof(int),
			AccessTools.Method(typeof(HoeDirtExtensions), nameof(HoeDirtExtensions.GetRetainingSoilDaysLeft)),
			AccessTools.Method(typeof(HoeDirtExtensions), nameof(HoeDirtExtensions.SetRetainingSoilDaysLeft))
		);

		SetupConfig();
	}

	private void OnDayStarted(object? sender, DayStartedEventArgs e)
	{
		TrackedSoilStateValues = TrackedSoilStateValues.Where(e => e.State.TryGetTarget(out _) && e.Soil.TryGetTarget(out _)).ToList();
	}

	private void SetupConfig()
	{
		var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

		configMenu?.Register(
			ModManifest,
			reset: () => Config = new ModConfig(),
			save: () => Helper.WriteConfig(Config)
		);

		configMenu?.AddSectionTitle(
			mod: ModManifest,
			text: () => Fluent["config-daysToRetain-section"],
			tooltip: () => Fluent["config-daysToRetain-section.tooltip"]
		);

		configMenu?.AddNumberOption(
			mod: ModManifest,
			name: () => Fluent["config-daysToRetain-basic"],
			getValue: () => Config.BasicRetainingSoilDays,
			setValue: value => Config.BasicRetainingSoilDays = value,
			min: -1, interval: 1
		);

		configMenu?.AddNumberOption(
			mod: ModManifest,
			name: () => Fluent["config-daysToRetain-quality"],
			getValue: () => Config.QualityRetainingSoilDays,
			setValue: value => Config.QualityRetainingSoilDays = value,
			min: -1, interval: 1
		);

		configMenu?.AddNumberOption(
			mod: ModManifest,
			name: () => Fluent["config-daysToRetain-deluxe"],
			getValue: () => Config.DeluxeRetainingSoilDays,
			setValue: value => Config.DeluxeRetainingSoilDays = value,
			min: -1, interval: 1
		);
	}

	private static void HoeDirt_ctor_Postfix(HoeDirt __instance)
	{
		__instance.NetFields.AddFields(__instance.GetRetainingSoilDaysLeftNetField());
		Instance.TrackedSoilStateValues.Add((State: new(__instance.state), Soil: new(__instance)));
	}

	private static void HoeDirt_dayUpdate_Prefix(HoeDirt __instance, ref int __state)
	{
		__state = __instance.state.Value;
	}

	private static void HoeDirt_dayUpdate_Postfix(HoeDirt __instance, ref int __state)
	{
		if (__instance.hasPaddyCrop())
			return;
		if (Instance.HasRetainingSoil(__instance))
		{
			if (__instance.state.Value == 0)
			{
				Instance.IsStayingWateredViaRetainingSoil = true;
				__instance.state.Value = __state;
				Instance.IsStayingWateredViaRetainingSoil = false;
			}

			var retainingSoilDaysLeft = __instance.GetRetainingSoilDaysLeft();
			if (retainingSoilDaysLeft == -1)
				return;
			__instance.SetRetainingSoilDaysLeft(retainingSoilDaysLeft - 1);
			if (retainingSoilDaysLeft == 0)
				__instance.state.Value = 0;
		}
	}

	private static void HoeDirt_plant_Postfix(HoeDirt __instance, bool isFertilizer)
	{
		if (!isFertilizer)
			return;
		if (__instance.state.Value == 0)
			return;
		Instance.RefreshRetainingSoilDaysLeft(__instance);
	}

	private static void NetInt_Set_Postfix(NetInt __instance, int newValue)
	{
		if (Instance.IsStayingWateredViaRetainingSoil)
			return;
		if (newValue <= 0)
			return;

		var entry = Instance.TrackedSoilStateValues.FirstOrNull(e => e.State.TryGetTarget(out var state) && ReferenceEquals(state, __instance));
		if (entry is null)
			return;
		if (!entry.Value.Soil.TryGetTarget(out var soil))
			return;

		Instance.RefreshRetainingSoilDaysLeft(soil);
	}

	private static void CraftingRecipe_Constructor_Postfix(CraftingRecipe __instance)
	{
		if (Instance.Fluent is null)
			return;

		if (__instance.bigCraftable)
			return;
		var retainingSoilDays = Instance.GetRetainingSoilDays(__instance.itemToProduce[0]);
		if (retainingSoilDays is null)
			return;

		__instance.description = Instance.Fluent.Get("retainingSoil-tooltip", new { Days = retainingSoilDays.Value });
	}

	private static void Object_getDescription_Postfix(SObject __instance, ref string __result)
	{
		if (Instance.Fluent is null)
			return;

		if (__instance.Category != SObject.fertilizerCategory)
			return;
		var retainingSoilDays = Instance.GetRetainingSoilDays(__instance.ParentSheetIndex);
		if (retainingSoilDays is null)
			return;

		__result = Instance.Fluent.Get("retainingSoil-tooltip", new { Days = retainingSoilDays.Value });
	}

	#region API

	#region HoeDirt
	public bool HasRetainingSoil(HoeDirt soil)
		=> GetRetainingSoilType(soil) != null;

	public int? GetRetainingSoilType(HoeDirt soil)
	{
		if (IsMultiFertilizerLoaded)
			return GetMultiFertilizerRetainingSoilType(soil);
		else
			return soil.fertilizer.Value is BasicRetainingSoilID or QualityRetainingSoilID or DeluxeRetainingSoilID ? soil.fertilizer.Value : null;
	}

	public int? GetRetainingSoilDaysLeft(HoeDirt soil)
		=> HasRetainingSoil(soil) ? soil.GetRetainingSoilDaysLeft() : null;

	public void SetRetainingSoilDaysLeft(HoeDirt soil, int days)
	{
		if (!HasRetainingSoil(soil))
			return;
		soil.SetRetainingSoilDaysLeft(days);
	}

	public void RefreshRetainingSoilDaysLeft(HoeDirt soil)
	{
		var retainingSoilType = GetRetainingSoilType(soil);
		if (retainingSoilType is null)
			return;

		var retainingSoilDays = GetRetainingSoilDays(retainingSoilType.Value);
		if (retainingSoilDays is not null)
			soil.SetRetainingSoilDaysLeft(retainingSoilDays.Value);
	}
	#endregion

	#region Object
	public bool IsRetainingSoil(int index)
		=> GetRetainingSoilDays(index) != null;

	public int? GetRetainingSoilDays(int index)
	{
		// TODO: maybe add some API for other mods to add their own, but no idea what to do about config then (probably also make it part of the API)
		return index switch
		{
			BasicRetainingSoilID => Config.BasicRetainingSoilDays,
			QualityRetainingSoilID => Config.QualityRetainingSoilDays,
			DeluxeRetainingSoilID => Config.DeluxeRetainingSoilDays,
			_ => null,
		};
	}
	#endregion

	#endregion
}