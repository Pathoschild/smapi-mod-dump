/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/seferoni/EnergyRework
**
*************************************************/

namespace EnergyRework.Common;

#region using directives

using StardewValley;

#endregion

internal static class Utilities
{
	internal static float GetEnergyLoss()
	{
		float energyLoss = ModEntry.Config.BaseEnergyLoss;

		if (Game1.player.running && Game1.player.isMoving())
		{
			energyLoss += ModEntry.Config.MovingEnergyOffset;
		}

		return energyLoss;
	}

	internal static float GetMaximumEnergy()
	{
		return Math.Min(Game1.player.MaxStamina, ModEntry.Config.SittingEnergyCeiling);
	}

	internal static void IncreaseEnergy(float energyChange)
	{
		if (Game1.player.Stamina >= ModEntry.Config.SittingEnergyCeiling)
		{
			return;
		}

		Game1.player.Stamina = Math.Min(Game1.player.Stamina + energyChange, GetMaximumEnergy());
	}

	internal static bool IsGameStateViable()
	{
		if (!Context.IsPlayerFree)
		{
			return false;
		}

		if (Game1.player.swimming.Value)
		{
			return false;
		}

		return true;
	}

	internal static void ReduceEnergy(float energyChange)
	{
		if (Game1.player.Stamina <= ModEntry.Config.EnergyFloor)
		{
			return;
		}

		Game1.player.Stamina = Math.Clamp(Game1.player.Stamina - energyChange, ModEntry.Config.EnergyFloor, Game1.player.MaxStamina);
	}

	internal static void UpdateEnergy()
	{
		if (!IsGameStateViable())
		{
			return;
		}

		if (Game1.player.IsSitting())
		{
			IncreaseEnergy(ModEntry.Config.SittingEnergyOffset);
			return;
		}

		ReduceEnergy(GetEnergyLoss());
	}
}