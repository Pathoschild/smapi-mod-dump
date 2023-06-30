/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.Kokoro.Stardew;
using Shockah.Kokoro.UI;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace Shockah.SeasonAffixes;

internal sealed class MediocrityAffix : BaseSeasonAffix, ISeasonAffix
{
	private static string ShortID => "Mediocrity";
	public string LocalizedDescription => Mod.Helper.Translation.Get($"{I18nPrefix}.description");
	public TextureRectangle Icon => new(Game1.objectSpriteSheet, new(368, 96, 16, 16));

	private List<WeakReference<SObject>> AffixApplied = new();

	public MediocrityAffix() : base(ShortID, "negative") { }

	public int GetPositivity(OrdinalSeason season)
		=> 0;

	public int GetNegativity(OrdinalSeason season)
		=> 1;

	public void OnActivate(AffixActivationContext context)
	{
		AffixApplied.Clear();
		Mod.Helper.Events.GameLoop.DayEnding += OnDayEnding;
		MachineTracker.MachineChangedEvent += OnMachineChanged;
	}

	public void OnDeactivate(AffixActivationContext context)
	{
		Mod.Helper.Events.GameLoop.DayEnding -= OnDayEnding;
		MachineTracker.MachineChangedEvent -= OnMachineChanged;
	}

	private void OnDayEnding(object? sender, DayEndingEventArgs e)
	{
		AffixApplied = AffixApplied
			.Where(r => r.TryGetTarget(out _))
			.ToList();
	}

	private void OnMachineChanged(GameLocation location, SObject machine, MachineProcessingState? oldState, MachineProcessingState? newState)
	{
		if (!Context.IsMainPlayer)
			return;
		if (oldState is null || newState is null)
			return;
		if (!newState.Value.ReadyForHarvest || newState.Value.HeldObject is null)
		{
			AffixApplied.RemoveAll(weakMachine => weakMachine.TryGetTarget(out var appliedMachine) && appliedMachine == machine);
			return;
		}
		if (AffixApplied.Any(weakMachine => weakMachine.TryGetTarget(out var appliedMachine) && ReferenceEquals(machine, appliedMachine)))
			return;

		AffixApplied.Add(new(machine));
		if (newState.Value.HeldObject.Quality == SObject.lowQuality)
			return;
		machine.heldObject.Value.Quality = SObject.lowQuality;
	}
}