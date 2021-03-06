/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/theLion/smapi-mods
**
*************************************************/

using TheLion.AwesomeProfessions.Configs;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod user-defined settings.</summary>
	public class ModConfig
	{
		// farming
		public bool EnableHarvester { get; set; } = true;
		public bool EnableAgriculturist { get; set; } = true;
		public bool EnableOenologist { get; set; } = true;

		public bool EnableRancher { get; set; } = true;
		public bool EnableBreeder { get; set; } = true;
		public bool EnableProducer { get; set; } = true;

		// foraging
		public bool EnableForager { get; set; } = true;
		public bool EnableEcologist { get; set; } = true;
		public bool EnableScavenger { get; set; } = true;

		public bool EnableLumberjack { get; set; } = true;
		public bool EnableArborist { get; set; } = true;
		public bool EnableTapper { get; set; } = true;

		// mining
		public bool EnableMiner { get; set; } = true;
		public bool EnableSpelunker { get; set; } = true;
		public bool EnableProspector { get; set; } = true;

		public bool EnableBlaster { get; set; } = true;
		public bool EnableDemolitionist { get; set; } = true;
		public bool EnableGemologist { get; set; } = true;

		// fishing
		public bool EnableFisher { get; set; } = true;
		public bool EnableAngler { get; set; } = true;
		public bool EnableAquarist { get; set; } = true;

		public bool EnableTrapper { get; set; } = true;
		public bool EnableMariner { get; set; } = true;
		public bool EnableConservationist { get; set; } = true;

		// combat
		public bool EnableFighter { get; set; } = true;
		public bool EnableBrute { get; set; } = true;
		public bool EnableGambit { get; set; } = true;

		public bool EnableRascal { get; set; } = true;
		public bool EnableMarksman { get; set; } = true;
		public bool EnableSlimemaster { get; set; } = true;

		public BreederConfig BreederConfig { get; set; } = new();
		public EcologistConfig EcologistConfig { get; set; } = new();
		public GemologistConfig GemologistConfig { get; set; } = new();
	}
}
