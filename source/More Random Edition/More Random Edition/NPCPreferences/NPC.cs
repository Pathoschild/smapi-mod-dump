using System.Collections.Generic;

namespace Randomizer
{
	public class NPC
	{
		public static List<Item> UniversalLoves = new List<Item>
		{
			ItemList.Items[(int)ObjectIndexes.GoldenPumpkin],
			ItemList.Items[(int)ObjectIndexes.Pearl],
			ItemList.Items[(int)ObjectIndexes.PrismaticShard],
			ItemList.Items[(int)ObjectIndexes.RabbitsFoot]
		};

		public static List<Item> UniversalHates;
		static NPC()
		{
			UniversalHates = new List<Item>
			{
				ItemList.Items[(int)ObjectIndexes.Bait],
				ItemList.Items[(int)ObjectIndexes.WildBait],
				ItemList.Items[(int)ObjectIndexes.Carp],
				ItemList.Items[(int)ObjectIndexes.CrabPot],
				ItemList.Items[(int)ObjectIndexes.DrumBlock],
				ItemList.Items[(int)ObjectIndexes.EnergyTonic],
				ItemList.Items[(int)ObjectIndexes.ExplosiveAmmo],
				ItemList.Items[(int)ObjectIndexes.FluteBlock],
				ItemList.Items[(int)ObjectIndexes.GrassStarter],
				ItemList.Items[(int)ObjectIndexes.GreenAlgae],
				ItemList.Items[(int)ObjectIndexes.Hay],
				ItemList.Items[(int)ObjectIndexes.IronOre],
				ItemList.Items[(int)ObjectIndexes.MermaidsPendant],
				ItemList.Items[(int)ObjectIndexes.MuscleRemedy],
				ItemList.Items[(int)ObjectIndexes.OilOfGarlic],
				ItemList.Items[(int)ObjectIndexes.Poppy],
				ItemList.Items[(int)ObjectIndexes.RainTotem],
				ItemList.Items[(int)ObjectIndexes.RedMushroom],
				ItemList.Items[(int)ObjectIndexes.Sap],
				ItemList.Items[(int)ObjectIndexes.WildBait],
				ItemList.Items[(int)ObjectIndexes.SeaUrchin],
				ItemList.Items[(int)ObjectIndexes.Seaweed],
				ItemList.Items[(int)ObjectIndexes.GreenSlimeEgg],
				ItemList.Items[(int)ObjectIndexes.BlueSlimeEgg],
				ItemList.Items[(int)ObjectIndexes.RedSlimeEgg],
				ItemList.Items[(int)ObjectIndexes.PurpleSlimeEgg],
				ItemList.Items[(int)ObjectIndexes.Snail],
				ItemList.Items[(int)ObjectIndexes.StrangeBun],
				ItemList.Items[(int)ObjectIndexes.Sugar],
				ItemList.Items[(int)ObjectIndexes.Torch],
				ItemList.Items[(int)ObjectIndexes.TreasureChest],
				ItemList.Items[(int)ObjectIndexes.VoidMayonnaise],
				ItemList.Items[(int)ObjectIndexes.WarpTotemBeach],
				ItemList.Items[(int)ObjectIndexes.WarpTotemFarm],
				ItemList.Items[(int)ObjectIndexes.WarpTotemMountains],
				ItemList.Items[(int)ObjectIndexes.WhiteAlgae],
				ItemList.Items[(int)ObjectIndexes.Slime],
				ItemList.Items[(int)ObjectIndexes.BugMeat],
				ItemList.Items[(int)ObjectIndexes.BatWing]
			};
			UniversalHates.AddRange(ItemList.GetTrash());
			UniversalHates.AddRange(ItemList.GetArtifacts());
		}

		public static List<string> QuestableNPCsList = new List<string>
		{ // Kent is not included because of him not appearing for awhile
			"Alex",
			"Elliot",
			"Harvey",
			"Sam",
			"Sebastian",
			"Shane",
			"Abigail",
			"Emily",
			"Haley",
			"Leah",
			"Maru",
			"Penny",
			"Caroline",
			"Clint",
			"Demetrius",
			"Evelyn",
			"George",
			"Gus",
			"Jas",
			"Jodi",
			"Lewis",
			"Linus",
			"Marnie",
			"Pam",
			"Pierre",
			"Robin",
			"Vincent",
			"Willy",
			"Wizard"
		};
	}
}
