/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;


namespace QualityScrubber
{
	public class QualityScrubberController
	{
		private readonly IMonitor monitor;
		private readonly ModConfig config;

		public int Duration
		{
			get { return config.Duration; }
		}


		public QualityScrubberController(IMonitor monitor, ModConfig config)
		{
			this.monitor = monitor;
			this.config = config;
		}


		public bool CanProcess(Item inputItem, SObject machine)
		{
			if (inputItem is null)
				return false;

			if (machine.heldObject.Value is not null)
			{
				//Monitor.Log("The machine is already scrubbing!", LogLevel.Debug);
				return false;
			}

			if (inputItem is not SObject inputObject)
			{
				//Monitor.Log("You can't scrub this!", LogLevel.Debug);
				return false;
			}

			if (inputObject.Quality == SObject.lowQuality)
			{
				//Monitor.Log("You can't scrub this any more!", LogLevel.Debug);
				return false;
			}

			// Ignore roe/wine/juice/jelly/pickles
			if (!config.AllowPreserves && inputObject.preserve.Value is not null)
			{
				//Monitor.Log("You can't scrub these yet!", LogLevel.Debug);
				return false;
			}

			// Ignore honey...
			if (!config.AllowHoney && inputObject.ParentSheetIndex == 340)
			{
				//Monitor.Log("You can't scrub honey!", LogLevel.Debug);
				return false;
			}

			return true;
		}


		public SObject? GetOutputObject(Item inputItem)
		{
			if (inputItem is SObject inputObject)
			{
				bool turnIntoGeneric = false;

				SObject outputObject;

				// Handle roe/aged roe
				if (inputObject is ColoredObject coloredObject)
				{
					outputObject = new ColoredObject(inputObject.ParentSheetIndex, 1, coloredObject.color.Value);
				}
				else
				{
					outputObject = new SObject(Vector2.Zero, inputObject.ParentSheetIndex, 1);

					// If input is honey, copy honey type
					if (outputObject.Name.Contains("Honey"))
					{
						outputObject.honeyType.Value = inputObject.honeyType.Value;

						if (config.TurnHoneyIntoGenericHoney)
							turnIntoGeneric = true;
					}
				}

				outputObject.Quality = SObject.lowQuality;

				switch (inputObject.preserve.Value)
				{
					case SObject.PreserveType.AgedRoe:
					case SObject.PreserveType.Roe:
						if (config.TurnRoeIntoGenericRoe)
							turnIntoGeneric = true;
						break;
					case SObject.PreserveType.Jelly:
						if (config.TurnJellyIntoGenericJelly)
							turnIntoGeneric = true;
						break;
					case SObject.PreserveType.Juice:
						if (config.TurnJuiceIntoGenericJuice)
							turnIntoGeneric = true;
						break;
					case SObject.PreserveType.Pickle:
						if (config.PicklesIntoGenericPickles)
							turnIntoGeneric = true;
						break;
					case SObject.PreserveType.Wine:
						if (config.TurnWineIntoGenericWine)
							turnIntoGeneric = true;
						break;
					default:
						break;
				}

				if (!turnIntoGeneric)
				{
					outputObject.Name = inputObject.Name;
					outputObject.Price = inputObject.Price;
					// Preserve value has to be defined here, otherwise the output will be named "Weeds {preservetype}"
					outputObject.preserve.Value = inputObject.preserve.Value;
					// Handle preserves (Wine, Juice, Jelly, Pickle)
					// preservedParentSheetIndex + preserve type define the object's DisplayName
					outputObject.preservedParentSheetIndex.Value = inputObject.preservedParentSheetIndex.Value;
				}

				return outputObject;
			}

			return null;
		}


		public void StartProcessing(SObject inputObject, SObject machine, Farmer who)
		{
			SObject? outputObject = GetOutputObject(inputObject);

			//this.Monitor.Log("Machine starts to scrub the item", LogLevel.Debug);
			machine.heldObject.Value = outputObject;
			machine.MinutesUntilReady = config.Duration;

			// Remove the item from inventory, if everything was successful
			if (who.ActiveObject.Stack == 1)
			{
				//who.Items.Remove(who.ActiveObject);
				who.ActiveObject = null;
			}
			else
			{
				who.ActiveObject.Stack -= 1;
			}
		}
	}
}
