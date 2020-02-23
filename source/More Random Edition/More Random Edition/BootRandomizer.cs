using System.Collections.Generic;

namespace Randomizer
{
	public class BootRandomizer
	{
		public static Dictionary<int, BootItem> Boots = new Dictionary<int, BootItem>();

		/// <summary>
		/// Randomizes boots - currently only changes defense and immunity
		/// </summary>
		/// <returns />
		public static Dictionary<int, string> Randomize()
		{
			Boots.Clear();
			WeaponAndArmorNameRandomizer nameRandomizer = new WeaponAndArmorNameRandomizer();

			Dictionary<int, string> bootReplacements = new Dictionary<int, string>();
			List<BootItem> bootsToUse = new List<BootItem>();
			foreach (BootItem originalBoot in BootData.AllBoots)
			{
				int statPool = Globals.RNGGetIntWithinPercentage(originalBoot.Defense + originalBoot.Immunity, 30);
				int defense = Range.GetRandomValue(0, statPool);
				int immunity = statPool - defense;

				BootItem newBootItem = new BootItem(
					originalBoot.Id,
					nameRandomizer.GenerateRandomBootName(),
					originalBoot.NotActuallyPrice,
					defense,
					immunity,
					originalBoot.ColorSheetIndex
				);

				bootsToUse.Add(newBootItem);
				Boots.Add(newBootItem.Id, newBootItem);
			}

			foreach (BootItem bootToAdd in bootsToUse)
			{
				bootReplacements.Add(bootToAdd.Id, bootToAdd.ToString());
			}

			WriteToSpoilerLog(bootsToUse);
			return bootReplacements;
		}

		/// <summary>
		/// Writes the boots to the spoiler log
		/// </summary>
		/// <param name="bootsToUse">The boot data that was used</param>
		public static void WriteToSpoilerLog(List<BootItem> bootsToUse)
		{
			if (!Globals.Config.RandomizeBoots) { return; }

			Globals.SpoilerWrite("==== BOOTS ====");
			foreach (BootItem bootToAdd in bootsToUse)
			{
				Globals.SpoilerWrite($"{bootToAdd.Name}: +{bootToAdd.Defense} defense; +{bootToAdd.Immunity} immunity");
			}
			Globals.SpoilerWrite("");
		}
	}
}
