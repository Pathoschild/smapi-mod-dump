using System;
using System.Collections.Generic;

namespace Randomizer
{
	/// <summary>
	/// Modifies the strings in the game
	/// </summary>
	public class StringsRandomizer
	{
		/// <summary>
		/// Fixes the strings to use the random values generated for the game's seed
		/// </summary>
		/// <returns />
		public static Dictionary<string, string> Randomize()
		{
			Dictionary<string, string> stringReplacements = new Dictionary<string, string>();

			// Fix the "Parsnip" string at the start of the game
			string parsnipSeedName = ItemList.Items[(int)ObjectIndexes.ParsnipSeeds].Name;
			stringReplacements["Farmer.cs.1918"] = $"You received 15 {parsnipSeedName}!^^'I found these seeds - I think they're Parsnip seeds but they probably aren't!^-Mayor Lewis'";

			return stringReplacements;
		}

		/// <summary>
		/// Taken from the original code
		/// Randomizes the story that Grandpa tells you at the start of the game
		/// </summary>
		public static Dictionary<string, string> RandomizeGrandpasStory()
		{
			Dictionary<string, string> stringReplacements = new Dictionary<string, string>();
			if (!Globals.Config.RandomizeIntroStory) { return stringReplacements; }

			string[] Adjective = new string[30];
			Adjective[0] = $"angry"; Adjective[1] = $"arrogant"; Adjective[2] = $"bored"; Adjective[3] = $"clumsy"; Adjective[4] = $"confused"; Adjective[5] = $"creepy"; Adjective[6] = $"cruel"; Adjective[7] = $"fierce";
			Adjective[8] = $"mysterious"; Adjective[9] = $"adorable"; Adjective[10] = $"handsome"; Adjective[11] = $"confident"; Adjective[12] = $"glamorous"; Adjective[13] = $"kind"; Adjective[14] = $"pretty"; Adjective[15] = $"calm";
			Adjective[16] = $"peaceful"; Adjective[17] = $"tranquil"; Adjective[18] = $"fat"; Adjective[19] = $"gigantic"; Adjective[20] = $"immense"; Adjective[21] = $"miniature"; Adjective[22] = $"gigantic";
			Adjective[23] = $"petite"; Adjective[24] = $"tiny"; Adjective[25] = $"brave"; Adjective[26] = $"charming"; Adjective[27] = $"energetic"; Adjective[28] = $"proud"; Adjective[29] = $"lazy";

			string[] Verb = new string[30];
			Verb[0] = $"bite"; Verb[1] = $"break"; Verb[2] = $"burn"; Verb[3] = $"dig"; Verb[4] = $"dream"; Verb[5] = $"drink"; Verb[6] = $"fight"; Verb[7] = $"freeze";
			Verb[8] = $"hide"; Verb[9] = $"hurt"; Verb[10] = $"lose"; Verb[11] = $"read"; Verb[12] = $"sell"; Verb[13] = $"swim"; Verb[14] = $"throw"; Verb[15] = $"understand";
			Verb[16] = $"write"; Verb[17] = $"lead"; Verb[18] = $"fly"; Verb[19] = $"forget"; Verb[20] = $"dive"; Verb[21] = $"choose"; Verb[22] = $"catch";
			Verb[23] = $"buy"; Verb[24] = $"bend"; Verb[25] = $"stab"; Verb[26] = $"make"; Verb[27] = $"run"; Verb[28] = $"see"; Verb[29] = $"shred";

			string[] PastVerb = new string[20];
			PastVerb[0] = $"beat"; PastVerb[1] = $"broke"; PastVerb[2] = $"burned"; PastVerb[3] = $"cut"; PastVerb[4] = $"dug"; PastVerb[5] = $"dove"; PastVerb[6] = $"dreamed"; PastVerb[7] = $"fell";
			PastVerb[8] = $"fought"; PastVerb[9] = $"froze"; PastVerb[10] = $"grew"; PastVerb[11] = $"hurt"; PastVerb[12] = $"laid"; PastVerb[13] = $"paid"; PastVerb[14] = $"sold"; PastVerb[15] = $"showed";
			PastVerb[16] = $"threw"; PastVerb[17] = $"woke"; PastVerb[18] = $"swam"; PastVerb[19] = $"tore";

			string[] Noun = new string[30];
			Noun[0] = $"oven mitt"; Noun[1] = $"Canadian"; Noun[2] = $"dank weed"; Noun[3] = $"American"; Noun[4] = $"concerned ape"; Noun[5] = $"dragon"; Noun[6] = $"cold-hearted eskimo"; Noun[7] = $"doge";
			Noun[8] = $"kappa"; Noun[9] = $"twitch chat"; Noun[10] = $"spaceship"; Noun[11] = $"gift"; Noun[12] = $"cowbell"; Noun[13] = $"shark"; Noun[14] = $"Spiderweb"; Noun[15] = $"canoe";
			Noun[16] = $"cardigan"; Noun[17] = $"tornado"; Noun[18] = $"underwear"; Noun[19] = $"airplane"; Noun[20] = $"toenail"; Noun[21] = $"pathoschild"; Noun[22] = $"mosquito"; Noun[23] = $"missile";
			Noun[24] = $"land mine"; Noun[25] = $"hamburger"; Noun[26] = $"gorilla"; Noun[27] = $"noob"; Noun[28] = $"dinosaur"; Noun[29] = "particle accelerator";

			string farmerNameTemp = "{0}";
			string farmNameTemp = "{1}";

			Random rng = new Random();
			stringReplacements["GrandpaStory.cs.12026"] = $"...and for my very {Adjective[rng.Next(0, 30)]} grandson:";
			stringReplacements["GrandpaStory.cs.12028"] = $"...and for my very {Adjective[rng.Next(0, 30)]} granddaughter:";
			stringReplacements["GrandpaStory.cs.12029"] = $"I want you to have this {PastVerb[rng.Next(0, 20)]} envelope.";
			stringReplacements["GrandpaStory.cs.12030"] = $"No, no, don't {Verb[rng.Next(0, 30)]} it yet... have patience.";
			stringReplacements["GrandpaStory.cs.12034"] = $"There will come a day when you feel {PastVerb[rng.Next(0, 20)]} by the burden of modern life...";
			stringReplacements["GrandpaStory.cs.12035"] = $"...and your {Adjective[rng.Next(0, 30)]} spirit will fade before a growing emptiness.";
			stringReplacements["GrandpaStory.cs.12036"] = $"When that happens, my boy, you'll be ready for this {Noun[rng.Next(0, 30)]}.";
			stringReplacements["GrandpaStory.cs.12038"] = $"When that happens, my dear, you'll be ready for this {Noun[rng.Next(0, 30)]}.";
			stringReplacements["GrandpaStory.cs.12040"] = $"Now, let Grandpa {Verb[rng.Next(0, 30)]}...";
			stringReplacements["GrandpaStory.cs.12051"] = $"Dear {farmerNameTemp},^^If you're reading this, you must be in dire need of a {Noun[rng.Next(0, 30)]}.^^The same thing happened to me, long ago. I'd lost sight of what mattered most in life... {Noun[rng.Next(0, 30)]}s. So I {PastVerb[rng.Next(0, 20)]} everything and moved to the place I truly belong.^^^I've enclosed the deed to that place... my pride and joy: {farmNameTemp} Farm. It's located in Stardew Valley, on the {Adjective[rng.Next(0, 30)]} coast. It's the {Adjective[rng.Next(0, 30)]} place to start your new life.^^This was my most precious gift of all, and now it's yours. I know you'll honor the family name, my boy. Good luck.^^Love, Grandpa^^P.S. If Lewis is still alive say hi to the {Adjective[rng.Next(0, 30)]} guy for me, will ya?";
			stringReplacements["GrandpaStory.cs.12055"] = $"Dear {farmerNameTemp},^^If you're reading this, you must be in dire need of a {Noun[rng.Next(0, 30)]}.^^The same thing happened to me, long ago. I'd lost sight of what mattered most in life... {Noun[rng.Next(0, 30)]}s. So I {PastVerb[rng.Next(0, 20)]} everything and moved to the place I truly belong.^^^I've enclosed the deed to that place... my pride and joy: {farmNameTemp} Farm. It's located in Stardew Valley, on the {Adjective[rng.Next(0, 30)]} coast. It's the {Adjective[rng.Next(0, 30)]} place to start your new life.^^This was my most precious gift of all, and now it's yours. I know you'll honor the family name, my boy. Good luck.^^Love, Grandpa^^P.S. If Lewis is still alive say hi to the {Adjective[rng.Next(0, 30)]} guy for me, will ya?";

			return stringReplacements;
		}
	}
}
