namespace QualityScrubber
{
	public class ModConfig
	{
		public int Duration { get; set; } = 30;
		public bool AllowPreserves { get; set; } = true;
		public bool AllowHoney { get; set; } = true;
		public bool TurnWineIntoGenericWine { get; set; } = false;
		public bool TurnHoneyIntoGenericHoney { get; set; } = false;
		public bool TurnJuiceIntoGenericJuice { get; set; } = false;
		public bool PicklesIntoGenericPickles { get; set; } = false;
		public bool TurnJellyIntoGenericJelly { get; set; } = false;
		public bool TurnRoeIntoGenericRoe { get; set; } = false;
	}
}
