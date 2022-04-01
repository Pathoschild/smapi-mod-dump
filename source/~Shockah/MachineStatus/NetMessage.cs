/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Shockah.CommonModCode;
using Shockah.CommonModCode.Stardew;
using StardewValley.Objects;
using GameLocation = StardewValley.GameLocation;
using SVObject = StardewValley.Object;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Shockah.MachineStatus
{
	internal static class NetMessage
	{
		private const int CrabPotID = 710;

		public static class Entity
		{
			public readonly struct Color
			{
				public readonly byte R { get; }
				public readonly byte G { get; }
				public readonly byte B { get; }
				public readonly byte A { get; }

				public Color(byte r, byte g, byte b, byte a)
				{
					this.R = r;
					this.G = g;
					this.B = b;
					this.A = a;
				}

				public static implicit operator Color(XnaColor c)
					=> new(c.R, c.G, c.B, c.A);

				public static implicit operator XnaColor(Color c)
					=> new(c.R, c.G, c.B, c.A);
			}

			public readonly struct SObject
			{
				public readonly int ParentSheetIndex { get; }
				public readonly string Name { get; }
				public readonly bool BigCraftable { get; }
				public readonly bool ShowNextIndex { get; }
				public readonly Color? Color { get; }
				public readonly bool ColorSameIndexAsParentSheetIndex { get; }

				public SObject(int parentSheetIndex, string name, bool bigCraftable, bool showNextIndex, Color? color, bool colorSameIndexAsParentSheetIndex)
				{
					this.ParentSheetIndex = parentSheetIndex;
					this.Name = name;
					this.BigCraftable = bigCraftable;
					this.ShowNextIndex = showNextIndex;
					this.Color = color;
					this.ColorSameIndexAsParentSheetIndex = colorSameIndexAsParentSheetIndex;
				}

				public static SObject Create(SVObject @object)
				{
					Color? color = null;
					bool colorSameIndexAsParentSheetIndex = false;
					if (@object is ColoredObject colored)
					{
						color = colored.color.Value;
						colorSameIndexAsParentSheetIndex = colored.ColorSameIndexAsParentSheetIndex;
					}

					return new(
						@object.ParentSheetIndex,
						@object.Name,
						@object.bigCraftable.Value,
						@object.showNextIndex.Value,
						color,
						colorSameIndexAsParentSheetIndex
					);
				}

				public bool Matches(SVObject @object)
					=> ParentSheetIndex == @object.ParentSheetIndex && BigCraftable == @object.bigCraftable.Value && Name == @object.Name;

				public SVObject Retrieve(IntPoint? tileLocation)
				{
					SVObject result;
					if (tileLocation is null)
					{
						result = Color.HasValue ? new ColoredObject(ParentSheetIndex, 1, Color.Value) : new SVObject(ParentSheetIndex, 1);
					}
					else
					{
						if (!BigCraftable && ParentSheetIndex == CrabPotID)
							result = new CrabPot(new Vector2(tileLocation.Value.X, tileLocation.Value.Y));
						else
							result = new SVObject(new Vector2(tileLocation.Value.X, tileLocation.Value.Y), ParentSheetIndex, 1);
					}

					result.Name = Name;
					result.bigCraftable.Value = BigCraftable;
					result.showNextIndex.Value = ShowNextIndex;
					if (result is ColoredObject colored)
						colored.ColorSameIndexAsParentSheetIndex = ColorSameIndexAsParentSheetIndex;
					return result;
				}

				public override string ToString()
					=> $"{ParentSheetIndex}:{Name}{(BigCraftable ? " (BigCraftable)" : "")}";
			}
		}
		
		public struct MachineUpsert
		{
			public LocationDescriptor Location { get; set; }
			public IntPoint TileLocation { get; set; }
			public Entity.SObject Machine { get; set; }
			public Entity.SObject? HeldObject { get; set; }
			public bool ReadyForHarvest { get; set; }
			public int MinutesUntilReady { get; set; }
			public MachineState State { get; set; }

			public MachineUpsert(
				LocationDescriptor location,
				IntPoint tileLocation,
				Entity.SObject machine,
				Entity.SObject? heldObject,
				bool readyForHarvest,
				int minutesUntilReady,
				MachineState state
			)
			{
				this.Location = location;
				this.TileLocation = tileLocation;
				this.Machine = machine;
				this.HeldObject = heldObject;
				this.ReadyForHarvest = readyForHarvest;
				this.MinutesUntilReady = minutesUntilReady;
				this.State = state;
			}

			public static MachineUpsert Create(LocationDescriptor location, SVObject machine, MachineState state)
			{
				Entity.SObject? heldObject = null;
				if (machine.TryGetAnyHeldObject(out var machineHeldObject))
					heldObject = Entity.SObject.Create(machineHeldObject);
				return new(
					location,
					new IntPoint((int)machine.TileLocation.X, (int)machine.TileLocation.Y),
					Entity.SObject.Create(machine),
					heldObject,
					machine.readyForHarvest.Value,
					machine.MinutesUntilReady,
					state
				);
			}

			public bool MatchesMachine(SVObject machine)
				=> Machine.Matches(machine) && TileLocation.X == (int)machine.TileLocation.X && TileLocation.Y == (int)machine.TileLocation.Y;

			public SVObject RetrieveMachine()
			{
				var machine = Machine.Retrieve(TileLocation);
				machine.TileLocation = new Vector2(TileLocation.X, TileLocation.Y);
				if (HeldObject is not null)
				{
					var retrievedHeldObject = HeldObject.Value.Retrieve(null);
					if (machine is CrabPot crabPot && retrievedHeldObject.Category == SVObject.baitCategory)
						crabPot.bait.Value = retrievedHeldObject;
					else
						machine.heldObject.Value = retrievedHeldObject;
				}
				machine.readyForHarvest.Value = ReadyForHarvest;
				machine.MinutesUntilReady = MinutesUntilReady;
				return machine;
			}
		}

		public struct MachineRemove
		{
			public LocationDescriptor Location { get; set; }
			public IntPoint TileLocation { get; set; }
			public int MachineParentSheetIndex { get; set; }
			public string MachineName { get; set; }

			public MachineRemove(LocationDescriptor location, IntPoint tileLocation, int machineParentSheetIndex, string machineName)
			{
				this.Location = location;
				this.TileLocation = tileLocation;
				this.MachineParentSheetIndex = machineParentSheetIndex;
				this.MachineName = machineName;
			}

			public static MachineRemove Create(GameLocation location, SVObject machine)
				=> new(
					LocationDescriptor.Create(location),
					new IntPoint((int)machine.TileLocation.X, (int)machine.TileLocation.Y),
					machine.ParentSheetIndex,
					machine.Name
				);

			public bool MatchesMachine(SVObject machine)
				=> MachineParentSheetIndex == machine.ParentSheetIndex && MachineName == machine.Name &&
				TileLocation.X == (int)machine.TileLocation.X && TileLocation.Y == (int)machine.TileLocation.Y;
		}
	}
}