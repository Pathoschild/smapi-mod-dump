/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/DwarvishMattock
**
*************************************************/


using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace DwarvishMattock
{
	/// <summary>The mod entry point.</summary>
	public class AssetLoader : IAssetLoader
	{
		public readonly static String blacksmithEvent = String.Join("/", new string[] {
			"playful",
			"8 15",
			"farmer 6 15 1 Clint 8 12 2 Dwarf 10 13 3",
			"addObject 9 11 934 0",
			"addTemporaryActor Clint2 16 32 -1 -1 4 false Character",
			"pause 800",
			"emote Clint 16",
			"jump Clint 4",
			"speak Clint \"There you are, @! Gunther just brought this over, he said you donated it?  Where did you find this?$l\"",
			"advancedMove farmer false 2 0 0 -1",
			"faceDirection farmer 1",
			"pause 1600",
			"stopAnimation farmer",
			"speak Clint \"I've read about these in Metallurgist Monthly, this is quite a find!  It's called a mattock, they haven't been seen in years!$h\"",
			"faceDirection Clint 1",
			"jump Clint 4",
			"showFrame Clint 7",
			"speak Clint \"Look over here, one end has a sharp blade to cut through thick roots and plants and the other end is a sturdy point, strong as any pickaxe!  What exquisite craftsmanship, you could probably chop a tree down with this thing!$h\"",
			"emote Clint 20",
			"faceDirection Clint 2",
			"showFrame Clint 0",
			"pause 800",
			"faceDirection farmer 1",
			"speak Dwarf \"This is all that remains of an ancient tool used by my ancestors. It is said that every dwarf was given one when they came of age, and this is what gave us our reputation as the world's best miners.  Unfortunately the tradition was lost over the ages...\"",
			"speak Dwarf \"It has been a long time since tools like this were made, seeing this brings happiness to my heart.  Thank you, human.\"",
			"pause 1000",
			"jump Clint 8",
			"speak Clint \"Wait a second, I've been stockpiling some rare materials for a special occasion... @ is the best miner I know, why don't we bring the tradition back to life!$h\"",
			"emote Dwarf 32",
			"friendship Dwarf 250",

			// *** Fade out ***
			"globalFade",
			"viewport -1000 -1000",
			// ***          ***

			"playMusic none",

			// Move the mattock to the table.
			"removeObject 9 11",
			"addObject 10 16 934 0",

			// Remove the bowl from the table.
			"removeTile 10 16 Front",

			// Warp everyone around the table to examine.
			"warp Clint 10 15",
			"faceDirection Clint 2",
			"warp Dwarf 8 17",
			"faceDirection Dwarf 1",
			"warp farmer 8 16",
			"faceDirection farmer 1",

			"playMusic sappypiano",

			// *** Fade in ***
			"pause 400",
			"viewport 6 15 true",
			"globalFadeToClear 0.015",
			// ***         ***

			"showFrame Clint 43",
			"textAboveHead Clint \"If I attach the head here...\"",
			"pause 4000",
			"showFrame Clint 0",
			"pause 3000",
			"textAboveHead Clint \"... I could forge the edges with dragontooth...\"",
			"pause 2000",
			"jump Dwarf 4",
			"pause 250",
			"faceDirection farmer 2",
			"pause 150",
			"faceDirection Dwarf 0",
			"pause 1750",
			"faceDirection Dwarf 1",
			"pause 100",
			"faceDirection farmer 1",
			"pause 1250",
			"shake Clint 2000",
			"textAboveHead Clint \"Let's get started!\"",
			"pause 200",
			"jump Dwarf 4",
			"pause 50",
			"jump farmer 4",
			"pause 750",
			"jump Dwarf 4",
			"pause 1000",
			"jump Dwarf 6",
			"pause 1500",

			// *** Fade out ***
			"globalFade 0.015",
			"viewport -1000 -1000",
			// ***          ***

			"warp Dwarf 9 15",
			"warp farmer 7 15",
			"warp Clint 8 12",
			"warp Clint2 9 12",
			"addObject 9 11 852 0",
			"addObject 11 12 337 0",
			"faceDirection Dwarf 3",
			"faceDirection farmer 1",
			"showFrame Clint 16",
			"showFrame Clint2 17",
			"pause 400",

			// Fade in
			"viewport 6 15 true",
			"globalFadeToClear 0.015",

			"pause 600",
			"showFrame Clint 22", "showFrame Clint2 23",
			
			"pause 150",
			"showFrame Clint 20", "showFrame Clint2 21",
			"pause 250",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 150",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 150",
			"playSound clank",
			"showFrame Clint 18", "showFrame Clint2 19",
			"pause 150",
			"showFrame Clint 16", "showFrame Clint2 17",

			"emote farmer 8",
			
			"pause 750",

			"emote Dwarf 40",

			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 150",
			"showFrame Clint 20", "showFrame Clint2 21",
			"pause 250",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 150",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 150",
			"playSound clank",
			"showFrame Clint 18", "showFrame Clint2 19",
			"pause 150",
			"showFrame Clint 16", "showFrame Clint2 17",

			"emote Dwarf 52",

			"pause 200",
			"jump farmer 4",
			"emote farmer 16",
			"pause 400",

			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 150",
			"showFrame Clint 20", "showFrame Clint2 21",
			"pause 250",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 100",
			"playSound clank",
			"showFrame Clint 18", "showFrame Clint2 19",
			"pause 250",
			"showFrame Clint 16", "showFrame Clint2 17",

			"emote Dwarf 32",
			"emote farmer 32",

			"pause 200",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 100",
			"showFrame Clint 20", "showFrame Clint2 21",

			// *** Fade out ***
			"globalFade 0.015",
			"viewport -1000 -1000",
			// ***          ***
			"playSound clank",

			"addProp 690 5 10 2 1 1",
			"addProp 692 5 11 2 1 1",
			"addProp 694 5 11 2 1 1",
			
			"changeMapTile Front 6 12 815",

			"removeObject 9 11",
			"removeObject 11 12",
			"addObject 9 11 788 0",
			"addObject 9 11 67 1",
			"addObject 11 16 656 1",

			"warp Dwarf 6 14",
			"faceDirection Dwarf 0",
			"positionOffset Dwarf 2 -8",
			"warp farmer 5 14",
			"faceDirection farmer 0",
			"positionOffset farmer 10 0",
			"warp Clint2 -1 -1",
			"faceDirection Clint 2",

			"pause 450",
			"viewport 6 15 true",
			"globalFadeToClear 0.015",

			"advancedMove Clint false 0 2 4 0 0 2",
			"pause 600",
			"temporarySprite 5 11 3 3 250 false",
			"playSound shwip",
			"pause 1200",
			"temporarySprite 6 11 1 4 200 false",
			"playSound debuffSpell",
			"shake Dwarf 1150",
			"pause 1000",
			"showFrame farmer 15",
			"jump farmer 4",
			"pause 1500",
			"showFrame farmer 12",
			"addProp 694 5 11 2 1 1",
			"temporarySprite 6 11 3 2 300 false",
			"playSound grunt",
			"positionOffset Dwarf 0 -1",
			"faceDirection Clint 3",
			"pause 1250",
			"addProp 694 5 11 2 1 1",
			"temporarySprite 5 11 1 4 200 false",
			"playSound fireball",
			"shake Dwarf 1200",
			"pause 1500",
			"positionOffset Dwarf 0 -1",
			"pause 600",
			"showFrame farmer 65",
			"pause 500",
			"showFrame Clint 13",
			"jump farmer 4",
			"pause 750",
			"positionOffset Dwarf 0 1",
			"pause 950",
			"temporarySprite 5 11 1 4 200 false",
			"playSound grunt",
			"pause 1000",
			"showFrame Clint 12",
			"jump Dwarf 2",
			"pause 250",
			"showFrame farmer 12",
			"temporarySprite 6 11 5 2 250 false",
			"playSound explosion",
			"pause 1000",

			// *** Fade out ***
			"globalFade",
			"viewport -1000 -1000",
			// ***          ***

			"removeObject 9 11",
			"removeObject 9 11",
			"addObject 9 11 935 0",
			"pause 500",
			"warp Clint 8 12",
			"warp Clint2 9 12",
			"warp Dwarf 10 14",
			"warp farmer 9 14",

			"faceDirection Dwarf 0",
			"faceDirection farmer 0",
			"showFrame Clint 16",
			"showFrame Clint2 17",
			"positionOffset Clint 0 -14",
			"positionOffset Clint2 0 -14",

			"changeMapTile Front 6 12 48",
			"changeMapTile Front 5 10 218",
			"changeMapTile Front 6 10 112",
			"changeMapTile Front 5 11 994",
			"changeMapTile Front 6 11 997",
			"removeObject 11 16",

			// *** Fade in ***
			"pause 800",
			"viewport 6 15 true",
			"globalFadeToClear 0.005",
			// ***         ***
			
			"pause 600",
			"showFrame Clint 22", "showFrame Clint2 23",
			
			"pause 100",
			"showFrame Clint 20", "showFrame Clint2 21",
			"pause 200",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 100",
			"playSound clank",
			"showFrame Clint 18", "showFrame Clint2 19",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"textAboveHead Clint \"Just a few more finishing touches...\"",
			"showFrame Clint 22", "showFrame Clint2 23",
			
			"pause 1100",
			"showFrame Clint 20", "showFrame Clint2 21",
			"pause 200",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 100",
			"playSound clank",
			"showFrame Clint 18", "showFrame Clint2 19",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 200",
			"showFrame Clint 22", "showFrame Clint2 23",
			
			"pause 200",
			"showFrame Clint 20", "showFrame Clint2 21",
			"pause 800",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 100",
			"playSound clank",
			"showFrame Clint 18", "showFrame Clint2 19",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 200",
			"showFrame Clint 22", "showFrame Clint2 23",

			"pause 200",
			"showFrame Clint 20", "showFrame Clint2 21",
			"pause 600",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 100",
			"playSound clank",
			"showFrame Clint 18", "showFrame Clint2 19",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 200",
			"showFrame Clint 22", "showFrame Clint2 23",

			"pause 200",
			"showFrame Clint 20", "showFrame Clint2 21",
			"pause 600",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 100",
			"playSound clank",
			"showFrame Clint 18", "showFrame Clint2 19",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 200",
			"showFrame Clint 22", "showFrame Clint2 23",

			"pause 200",
			"showFrame Clint 20", "showFrame Clint2 21",
			"pause 1400",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 100",
			"playSound clank",
			"showFrame Clint 18", "showFrame Clint2 19",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 200",
			"showFrame Clint 22", "showFrame Clint2 23",

			"pause 200",
			"showFrame Clint 20", "showFrame Clint2 21",
			"pause 600",
			"showFrame Clint 22", "showFrame Clint2 23",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 100",
			"playSound clank",
			"showFrame Clint 18", "showFrame Clint2 19",
			"pause 100",
			"showFrame Clint 16", "showFrame Clint2 17",
			"pause 1500",

			"warp Clint2 -1 -1",
			"faceDirection Clint 2",
			"showFrame Clint 0",
			"pause 500",
			"textAboveHead Clint \"It's done!\"",
			"removeObject 9 11",
			"advancedMove Clint false 0 2",
			"pause 500",
			"faceDirection Dwarf 3",
			"pause 500",
			"playMusic none",
			"faceDirection Clint 1",
			"showFrame Clint 7",
			"pause 400",
			"playSound getNewSpecialItem",
			"faceDirection farmer 2",
			"showFrame farmer 57",
			"addObject 9 12 935 1",
			"message \"You received the Dwarven Mattock!\"",
			"removeObject 9 12",
			"showFrame Clint 4",
			"faceDirection farmer 3",

			"speak Clint \"This has to be some of my best work, I don't think I'll be able to upgrade it any further. I stayed true to the original dwarf design, so this tool can destroy plants, trees and rocks.  Plus I used a dragontooth alloy so it's practically indestructible!$h\"",
			"pause 500",
			"faceDirection farmer 1",
			"pause 750",
			"speak Dwarf \"You have done dwarf kind a service. Thank you, both. Wield it with pride, @!\"",
			"pause 2500",
			"globalFade",
			"viewport -1000 -1000",
			"end"
		});

		/// <summary>Get whether this instance can load the initial version of the given asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public bool CanLoad<T>(IAssetInfo asset)
		{
			return asset.AssetNameEquals("Data/Events/Blacksmith");
		}

		/// <summary>Load a matched asset.</summary>
		/// <param name="asset">Basic metadata about the asset being loaded.</param>
		public T Load<T>(IAssetInfo asset)
		{
			// Add the new event.
			if (asset.AssetNameEquals("Data/Events/Blacksmith"))
			{
				return (T)(object)new Dictionary<string, string>
				{
					["9684001/r 0"] = blacksmithEvent
				};
			}

			throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
		}
	}
}