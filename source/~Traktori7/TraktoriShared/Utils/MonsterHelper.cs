/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;


namespace TraktoriShared.Utils
{
	/// <summary>
	/// NOTE: Look into Characters/Monsters for some you've possibly missed.
	/// Also https://stardewvalleywiki.com/Monsters has very nice information.
	/// Look into MineShaft.BuffMonsterIfNecessary. Buffs them and handles switching the monsters to the dangerous sprite variants.
	/// 
	/// Supported monster types and their respective names:
	/// 
	/// #Slimes
	/// 
	/// GreenSlime
	/// -Spawns a green slime seen on the first levels of the mine.
	/// -Using mineLevel 1 to spawn a normal one. If Changed to 2,3 or 4 it seems to have a 1% chance to spawn a special one.
	/// 
	/// FrostJelly
	/// -Spawns a blue slime seen on levels 40-79 of the mine.
	/// -Using mineLevel 1 to spawn a normal one. If Changed to 2,3 or 4 it seems to have a 1% chance to spawn a special one.
	/// 
	/// Sludge
	/// -Spawns a red slime seen on levels 80-120 of the mine.
	/// -Using mineLevel 1 to spawn a normal one. If Changed to 2,3 or 4 it seems to have a 1% chance to spawn a special one.
	/// 
	/// BigSlime
	/// -mineArea (10, 40, 80, 121) changes the color and health
	/// 
	/// There seems to also be a mineLevel 120+ Sludge. Also there seems to be a slime.makePrismatic() method for super tanky slime.
	/// 
	/// 
	/// #Bats
	/// 
	/// Bat
	/// -Spawns the normal bat.
	/// 
	/// FrostBat
	/// -Spawns the bat from the levels 40-79 of the mine.
	/// 
	/// LavaBat
	/// -Spawns the bat from the level 80-120 of the mine.
	/// 
	/// IridiumBat
	/// -Spawns the hard bat from the skull cavern.
	/// 
	/// 
	/// #Bat-like creatures
	/// 
	/// HauntedSkull
	/// -The floating skull found especially in the quarry mine.
	/// 
	/// MagmaSprite
	/// -I don't know what this is.
	/// 
	/// MagmaSparker
	/// -I don't know what this is.
	/// 
	/// CursedDoll
	/// -A floating cursed doll. Seems hard to kill.
	/// 
	/// 
	/// #Ghosts
	/// 
	/// Ghost
	/// 
	/// CarbonGhost
	/// 
	/// PutridGhost
	/// </summary>
	internal class MonsterHelper
	{
		/// <summary>
		/// Gets the monster type from the given name.
		/// </summary>
		/// <param name="monsterName">The monster name in Data/Monsters.json without the space.</param>
		/// <returns>The monster type.</returns>
		internal static MonsterType GetMonsterTypeFromName(string monsterName)
		{
			// TryParse seems to do something weird sometimes with the values it parses succesfully,
			// so double check the parsing worked with IsDefined.
			if (Enum.TryParse(monsterName, out MonsterType monsterType) && Enum.IsDefined(monsterType))
			{
				return monsterType;
			}

			return MonsterType.None;
		}


		/// <summary>
		/// Creates a monster instance that matches the given name.
		/// </summary>
		/// <param name="monsterName">The name of the monster.</param>
		/// <param name="spawnPoint">The tile to spawn the monster on.</param>
		/// <returns>The monster instance, or null if the given type doesn't match any.</returns>
		internal static Monster? GetMonsterFromName(string monsterName, Point spawnPoint)
		{
			MonsterType type = GetMonsterTypeFromName(monsterName);

			return (type is MonsterType.None) ? null : GetMonsterFromType(type, spawnPoint);
		}


		/// <summary>
		/// Creates a monster instance that matches the given type.
		/// </summary>
		/// <param name="monsterType">The monster type to create.</param>
		/// <param name="point">The tile to spawn the monster on.</param>
		/// <returns>The monster instance, or null if the given type doesn't match any.</returns>
		internal static Monster? GetMonsterFromType(MonsterType monsterType, Point point)
		{
			Monster? monster = null;
			// The game uses tile coordinates * 64 for the real spawn location.
			// It's probably pixel coordinates since a tile of 16 pixels gets scaled by 4.
			// Interestingly the monsters get autokilled if they touch the negative coordinates,
			// or atleast that seems to be the case since if you spawn them on (0, 0) and they walk of the edge get removed.
			Vector2 spawnPoint = point.ToVector2() * 64;

			switch (monsterType)
			{
				case MonsterType.None:
					break;
				case MonsterType.GreenSlime:
					monster = new GreenSlime(spawnPoint, 1);
					break;
				case MonsterType.DustSpirit:
					break;
				case MonsterType.Bat:
					monster = new Bat(spawnPoint, 1);
					break;
				case MonsterType.FrostBat:
					monster = new Bat(spawnPoint, 40);
					break;
				case MonsterType.LavaBat:
					monster = new Bat(spawnPoint, 80);
					break;
				case MonsterType.IridiumBat:
					monster = new Bat(spawnPoint, 171);
					break;
				case MonsterType.StoneGolem:
					break;
				case MonsterType.WildernessGolem:
					break;
				case MonsterType.Grub:
					break;
				case MonsterType.Fly:
					break;
				case MonsterType.FrostJelly:
					monster = new GreenSlime(spawnPoint, 40);
					break;
				case MonsterType.Sludge:
					monster = new GreenSlime(spawnPoint, 80);
					break;
				case MonsterType.ShadowGuy:
					break;
				case MonsterType.Ghost:
					monster = new Ghost(spawnPoint);
					break;
				case MonsterType.CarbonGhost:
					string carbonGhostName = "Carbon Ghost";
					monster = new Ghost(spawnPoint, carbonGhostName);
					break;
				case MonsterType.Duggy:
					break;
				case MonsterType.RockCrab:
					break;
				case MonsterType.LavaCrab:
					break;
				case MonsterType.IridiumCrab:
					break;
				case MonsterType.Fireball:
					break;
				case MonsterType.SquidKid:
					break;
				case MonsterType.SkeletonWarrior:
					break;
				case MonsterType.Crow:
					break;
				case MonsterType.Frog:
					break;
				case MonsterType.Cat:
					break;
				case MonsterType.ShadowBrute:
					break;
				case MonsterType.ShadowShaman:
					break;
				case MonsterType.Skeleton:
					break;
				case MonsterType.SkeletonMage:
					break;
				case MonsterType.MetalHead:
					break;
				case MonsterType.Spiker:
					break;
				case MonsterType.Bug:
					break;
				case MonsterType.Mummy:
					break;
				case MonsterType.BigSlime:
					int difficulty = 10;
					monster = new BigSlime(spawnPoint, difficulty);
					break;
				case MonsterType.Serpent:
					break;
				case MonsterType.PepperRex:
					break;
				case MonsterType.TigerSlime:
					GreenSlime slime = new GreenSlime(spawnPoint);
					slime.makeTigerSlime();
					monster = slime;
					break;
				case MonsterType.LavaLurk:
					break;
				case MonsterType.HotHead:
					break;
				case MonsterType.MagmaSprite:
					int magmaSpriteMineLevel = -555;
					monster = new Bat(spawnPoint, magmaSpriteMineLevel);
					break;
				case MonsterType.MagmaDuggy:
					break;
				case MonsterType.MagmaSparker:
					int magmaSparkerMineLevel = -556;
					monster = new Bat(spawnPoint, magmaSparkerMineLevel);
					break;
				case MonsterType.FalseMagmaCap:
					break;
				case MonsterType.DwarvishSentry:
					break;
				case MonsterType.PutridGhost:
					string putridGhostName = "Putrid Ghost";
					monster = new Ghost(spawnPoint, putridGhostName);
					break;
				case MonsterType.ShadowSniper:
					break;
				case MonsterType.Spider:
					break;
				case MonsterType.RoyalSerpent:
					break;
				case MonsterType.BlueSquid:
					break;
				case MonsterType.HauntedSkull:
					int hauntedSkullMineLevel = 77377;
					monster = new Bat(spawnPoint, hauntedSkullMineLevel);
					break;
				case MonsterType.CursedDoll:
					int cursedDollMineLevel = -666;
					monster = new Bat(spawnPoint, cursedDollMineLevel);
					break;
			}

			return monster;
		}


		internal enum MonsterType
		{
			None,
			GreenSlime,
			DustSpirit,
			Bat,
			FrostBat,
			LavaBat,
			IridiumBat,
			StoneGolem,
			WildernessGolem,
			Grub,
			Fly,
			FrostJelly,
			Sludge,
			ShadowGuy,
			Ghost,
			CarbonGhost,
			Duggy,
			RockCrab,
			LavaCrab,
			IridiumCrab,
			Fireball,
			SquidKid,
			SkeletonWarrior,
			Crow,
			Frog,
			Cat,
			ShadowBrute,
			ShadowShaman,
			Skeleton,
			SkeletonMage,
			MetalHead,
			Spiker,
			Bug,
			Mummy,
			BigSlime,
			Serpent,
			PepperRex,
			TigerSlime,
			LavaLurk,
			HotHead,
			MagmaSprite,
			MagmaDuggy,
			MagmaSparker,
			FalseMagmaCap,
			DwarvishSentry,
			PutridGhost,
			ShadowSniper,
			Spider,
			RoyalSerpent,
			BlueSquid,
			// These aren't in the names in Data/Monsters.json
			HauntedSkull,
			CursedDoll
		}
	}
}
