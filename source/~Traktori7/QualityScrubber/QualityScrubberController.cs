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
using SObject = StardewValley.Object;
using StardewValley.Objects;


namespace QualityScrubber
{
	public class QualityScrubberController
	{
		private IMonitor Monitor { get; set; }
		private ModConfig Config { get; set; }

		public int Duration
		{
			get { return Config.Duration; }
		}


		public QualityScrubberController(IMonitor monitor, ModConfig config)
		{
			this.Monitor = monitor;
			this.Config = config;
		}


		public bool CanProcess(Item inputItem, SObject machine)
		{
			if (inputItem is null)
				return false;

			if (machine.heldObject.Value != null)
			{
				//Monitor.Log("The machine is already scrubbing!", LogLevel.Debug);
				return false;
			}

			if (!(inputItem is SObject inputObject))
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
			if (!Config.AllowPreserves && inputObject.preserve.Value != null)
			{
				//Monitor.Log("You can't scrub these yet!", LogLevel.Debug);
				return false;
			}

			// Ignore honey...
			if (!Config.AllowHoney && inputObject.ParentSheetIndex == 340)
			{
				//Monitor.Log("You can't scrub honey!", LogLevel.Debug);
				return false;
			}

			return true;
		}


		public SObject GetOutputObject(Item inputItem)
		{
			if (inputItem is SObject inputObject)
			{
				bool turnIntoGeneric = false;

				SObject outputObject;

				// Handle roe/aged roe
				if (inputObject is ColoredObject)
				{
					outputObject = new ColoredObject(inputObject.ParentSheetIndex, 1, ((ColoredObject)inputObject).color.Value);
				}
				else
				{
					outputObject = new StardewValley.Object(Vector2.Zero, inputObject.ParentSheetIndex, 1);

					// If input is honey, copy honey type
					if (outputObject.Name.Contains("Honey"))
					{
						outputObject.honeyType.Value = inputObject.honeyType.Value;

						if (Config.TurnHoneyIntoGenericHoney)
							turnIntoGeneric = true;
					}
				}

				outputObject.Quality = SObject.lowQuality;

				switch (inputObject.preserve.Value)
				{
					case SObject.PreserveType.AgedRoe:
					case SObject.PreserveType.Roe:
						if (Config.TurnRoeIntoGenericRoe)
							turnIntoGeneric = true;
						break;
					case SObject.PreserveType.Jelly:
						if (Config.TurnJellyIntoGenericJelly)
							turnIntoGeneric = true;
						break;
					case SObject.PreserveType.Juice:
						if (Config.TurnJuiceIntoGenericJuice)
							turnIntoGeneric = true;
						break;
					case SObject.PreserveType.Pickle:
						if (Config.PicklesIntoGenericPickles)
							turnIntoGeneric = true;
						break;
					case SObject.PreserveType.Wine:
						if (Config.TurnWineIntoGenericWine)
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
			SObject outputObject = GetOutputObject(inputObject);

			//this.Monitor.Log("Machine starts to scrub the item", LogLevel.Debug);
			machine.heldObject.Value = outputObject;
			machine.MinutesUntilReady = Config.Duration;

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
