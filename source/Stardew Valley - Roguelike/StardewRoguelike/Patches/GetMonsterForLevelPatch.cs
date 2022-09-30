/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Monsters;
using System;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    internal class GetMonsterForLevelPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(MineShaft), "getMonsterForThisLevel");

        public static bool Prefix(ref Monster __result, MineShaft __instance, int level, int xTile, int yTile)
        {
			return true;  // we might not need this patch anymore. needs more testing to confirm

			Vector2 position = new Vector2(xTile, yTile) * 64f;
			float distanceFromLadder = __instance.getDistanceFromStart(xTile, yTile);
			Random mineRandom = (Random)__instance.GetType().GetField("mineRandom", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
			if (__instance.getMineArea() == 0 || __instance.getMineArea() == 10)
			{
				if (mineRandom.NextDouble() < 0.25 && !__instance.mustKillAllMonstersToAdvance())
				{
					__result = new Bug(position, mineRandom.Next(4), __instance);
					return false;
				}
				if (level < 15)
				{
					if (__instance.doesTileHaveProperty(xTile, yTile, "Diggable", "Back") is not null)
					{
						__result = new Duggy(position);
						return false;
					}
					if (mineRandom.NextDouble() < 0.15)
					{
						__result = new RockCrab(position);
						return false;
					}
					__result = new GreenSlime(position, level);
					return false;
				}
				if (level <= 30)
				{
					if (__instance.doesTileHaveProperty(xTile, yTile, "Diggable", "Back") is not null)
					{
						__result = new Duggy(position);
						return false;
					}
					if (mineRandom.NextDouble() < 0.15)
					{
						__result = new RockCrab(position);
						return false;
					}
					if (mineRandom.NextDouble() < 0.05 && distanceFromLadder > 10f && __instance.GetAdditionalDifficulty() <= 0)
					{
						__result = new Fly(position);
						return false;
					}
					if (mineRandom.NextDouble() < 0.45)
					{
						__result = new GreenSlime(position, level);
						return false;
					}
					if (__instance.GetAdditionalDifficulty() <= 0)
					{
						__result = new Grub(position);
						return false;
					}
					if (distanceFromLadder > 9f)
					{
						__result = new BlueSquid(position);
						__result.moveTowardPlayerThreshold.Value *= 2;
						return false;
					}
					if (mineRandom.NextDouble() < 0.01)
					{
						__result = new RockGolem(position, __instance);
						return false;
					}
					__result = new GreenSlime(position, level);
					return false;
				}
				if (level <= 40)
				{
					if (mineRandom.NextDouble() < 0.1 && distanceFromLadder > 10f)
					{
						__result = new Bat(position, level);
						return false;
					}
					if (__instance.GetAdditionalDifficulty() > 0 && mineRandom.NextDouble() < 0.1)
					{
						__result = new Ghost(position, "Carbon Ghost");
						return false;
					}
					__result = new RockGolem(position, __instance);
					return false;
				}
			}
			else if (__instance.getMineArea() == 40)
			{
				if (__instance.mineLevel >= 70 && (mineRandom.NextDouble() < 0.75 || __instance.GetAdditionalDifficulty() > 0))
				{
					if (mineRandom.NextDouble() < 0.75 || __instance.GetAdditionalDifficulty() <= 0)
					{
						__result = new Skeleton(position, __instance.GetAdditionalDifficulty() > 0 && mineRandom.NextDouble() < 0.5);
						return false;
					}
					__result = new Bat(position, 77377);
					return false;
				}
				if (mineRandom.NextDouble() < 0.3)
				{
					__result = new DustSpirit(position, mineRandom.NextDouble() < 0.8);
					return false;
				}
				if (mineRandom.NextDouble() < 0.3 && distanceFromLadder > 10f)
				{
					__result = new Bat(position, __instance.mineLevel);
					return false;
				}
				bool ghostAdded = (bool)__instance.GetType().GetField("ghostAdded", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
				if (!ghostAdded && __instance.mineLevel > 50 && mineRandom.NextDouble() < 0.3 && distanceFromLadder > 10f)
				{
					ghostAdded = true;
					__instance.GetType().GetField("ghostAdded", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, ghostAdded);
					if (__instance.GetAdditionalDifficulty() > 0)
					{
						__result = new Ghost(position, "Putrid Ghost");
						return false;
					}
					__result = new Ghost(position);
					return false;
				}
				if (__instance.GetAdditionalDifficulty() > 0)
				{
					if (mineRandom.NextDouble() < 0.01)
					{
						RockCrab rockCrab = new RockCrab(position);
						rockCrab.makeStickBug();
						__result = rockCrab;
						return false;
					}
					if (__instance.mineLevel >= 50)
					{
						__result = new Leaper(position);
						return false;
					}
					if (mineRandom.NextDouble() < 0.7)
					{
						__result = new Grub(position);
						return false;
					}
					__result = new GreenSlime(position, __instance.mineLevel);
					return false;
				}
			}
			else if (__instance.getMineArea() == 80)
			{
				if (__instance.isDarkArea() && mineRandom.NextDouble() < 0.25)
				{
					__result = new Bat(position, __instance.mineLevel);
					return false;
				}
				if (mineRandom.NextDouble() < ((__instance.GetAdditionalDifficulty() > 0) ? 0.05 : 0.15))
				{
					__result = new GreenSlime(position, __instance.getMineArea());
					return false;
				}
				if (mineRandom.NextDouble() < 0.15)
				{
					__result = new MetalHead(position, __instance.getMineArea());
					return false;
				}
				if (mineRandom.NextDouble() < 0.25)
				{
					__result = new ShadowBrute(position);
					return false;
				}
				if (__instance.GetAdditionalDifficulty() > 0 && mineRandom.NextDouble() < 0.25)
				{
					__result = new Shooter(position, "Shadow Sniper");
					return false;
				}
				if (mineRandom.NextDouble() < 0.25)
				{
					__result = new ShadowShaman(position);
					return false;
				}
				if (mineRandom.NextDouble() < 0.25)
				{
					__result = new RockCrab(position, "Lava Crab");
					return false;
				}
				if (mineRandom.NextDouble() < 0.2 && distanceFromLadder > 8f && __instance.mineLevel >= 90 && __instance.getTileIndexAt(xTile, yTile, "Back") != -1 && __instance.getTileIndexAt(xTile, yTile, "Front") == -1)
				{
					__result = new SquidKid(position);
					return false;
				}
			}
			else
			{
				if (__instance.getMineArea() == 121)
				{
					bool loadedDarkArea = (bool)__instance.GetType().GetField("loadedDarkArea", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
					if (loadedDarkArea)
					{
						if (mineRandom.NextDouble() < 0.18 && distanceFromLadder > 8f)
						{
							__result = new Ghost(position, "Carbon Ghost");
							return false;
						}
						__result = new Serpent(position);
						return false;
					}
					if (__instance.mineLevel % 20 == 0 && distanceFromLadder > 10f)
					{
						__result = new Bat(position, __instance.mineLevel);
						return false;
					}
					if (__instance.mineLevel % 16 == 0 && !__instance.mustKillAllMonstersToAdvance())
					{
						__result = new Bug(position, mineRandom.Next(4), __instance);
						return false;
					}
					if (mineRandom.NextDouble() < 0.33 && distanceFromLadder > 10f)
					{
						if (__instance.GetAdditionalDifficulty() <= 0)
						{
							__result = new Serpent(position);
							return false;
						}
						__result = new Serpent(position, "Royal Serpent");
						return false;
					}
					if (mineRandom.NextDouble() < 0.33 && distanceFromLadder > 10f && __instance.mineLevel >= 171)
					{
						__result = new Bat(position, __instance.mineLevel);
						return false;
					}
					if (__instance.mineLevel >= 126 && distanceFromLadder > 10f && mineRandom.NextDouble() < 0.04 && !__instance.mustKillAllMonstersToAdvance())
					{
						__result = new DinoMonster(position);
						return false;
					}
					if (mineRandom.NextDouble() < 0.33 && !__instance.mustKillAllMonstersToAdvance())
					{
						__result = new Bug(position, mineRandom.Next(4), __instance);
						return false;
					}
					if (mineRandom.NextDouble() < 0.25)
					{
						__result = new GreenSlime(position, level);
						return false;
					}
					if (__instance.mineLevel >= 146 && mineRandom.NextDouble() < 0.25)
					{
						__result = new RockCrab(position, "Iridium Crab");
						return false;
					}
					if (__instance.GetAdditionalDifficulty() > 0 && mineRandom.NextDouble() < 0.2 && distanceFromLadder > 8f && __instance.getTileIndexAt(xTile, yTile, "Back") != -1 && __instance.getTileIndexAt(xTile, yTile, "Front") == -1)
					{
						__result = new SquidKid(position);
						return false;
					}
					__result = new BigSlime(position, __instance);
					return false;
				}
			}
			__result = new GreenSlime(position, level);

			return false;
		}
    }
}
