/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using SObject = StardewValley.Object;

namespace Shockah.Kokoro.Stardew
{
	public readonly struct MachineProcessingState
	{
		public bool ReadyForHarvest { get; init; }
		public int MinutesUntilReady { get; init; }
		public SObject? HeldObject { get; init; }

		public MachineProcessingState(bool readyForHarvest, int minutesUntilReady, SObject? heldObject)
		{
			this.ReadyForHarvest = readyForHarvest;
			this.MinutesUntilReady = minutesUntilReady;
			this.HeldObject = heldObject;
		}

		public MachineProcessingState(SObject machine)
		{
			this.ReadyForHarvest = machine.readyForHarvest.Value;
			this.MinutesUntilReady = machine.MinutesUntilReady;
			this.HeldObject = machine.GetAnyHeldObject();
		}

		public override bool Equals(object? obj)
			=> obj is MachineProcessingState state
			&& ReadyForHarvest == state.ReadyForHarvest
			&& MinutesUntilReady == state.MinutesUntilReady
			&& (HeldObject is null) == (state.HeldObject is null)
			&& (HeldObject is null || HeldObject.IsSameItem(state.HeldObject!));

		public override int GetHashCode()
			=> (ReadyForHarvest ? 31 : 0) + MinutesUntilReady;

		public static bool operator ==(MachineProcessingState left, MachineProcessingState right)
			=> left.Equals(right);

		public static bool operator !=(MachineProcessingState left, MachineProcessingState right)
			=> !left.Equals(right);
	}
}