/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using EnhancedSlingshots.Enchantments;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancedSlingshots.Patch
{
	[HarmonyPatch(typeof(GameLocation))]
	public static class GameLocationPatchs
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

		[HarmonyPostfix]
		[HarmonyPatch(nameof(GameLocation.monsterDrop))]
		public static void monsterDrop_Postfix(GameLocation __instance, int x, int y, Monster monster)
		{
			if (Game1.player.CurrentTool is Slingshot sling && sling.hasEnchantmentOfType<HunterEnchantment>())
			{ 
				IList<int> objects = monster.objectsToDrop;
				Vector2 playerPosition = new Vector2(Game1.player.GetBoundingBox().Center.X, Game1.player.GetBoundingBox().Center.Y);
				
				Game1.content.Load<Dictionary<string, string>>("Data\\Monsters").TryGetValue(monster.Name, out string result);
				if (result != null && result.Length > 0)
				{
					string[] objectsSplit = result.Split('/')[6].Split(' ');					
					for (int l = 0; l < objectsSplit.Length; l += 2)
					{
						if (Game1.random.NextDouble() < Convert.ToDouble(objectsSplit[l + 1]))													
							objects.Add(Convert.ToInt32(objectsSplit[l]));	
					}
				}

				for (int k = 0; k < objects.Count; k++)
				{
					int objectToAdd = objects[k];
					if (objectToAdd < 0)
					{
						__instance.debris.Add(monster.ModifyMonsterLoot(new Debris(Math.Abs(objectToAdd), Game1.random.Next(1, 4), new Vector2(x, y), playerPosition)));
					}
					else
					{
						__instance.debris.Add(monster.ModifyMonsterLoot(new Debris(objectToAdd, new Vector2(x, y), playerPosition)));
					}
				}
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch("breakStone")]
		public static void breakStone_Postfix(GameLocation __instance, int indexOfStone, int x, int y, Farmer who, Random r)
        {
			if (who.CurrentTool is Slingshot sling && sling.hasEnchantmentOfType<MinerEnchantment>())
            {
				switch (indexOfStone)
                {
					case 95:
						Game1.createObjectDebris(909, x, y, who.UniqueMultiplayerID, __instance);
						break;
					case 75:
						Game1.createObjectDebris(535, x, y, who.UniqueMultiplayerID, __instance);						
						break;
					case 76:
						Game1.createObjectDebris(536, x, y, who.UniqueMultiplayerID, __instance);						
						break;
					case 77:
						Game1.createObjectDebris(537, x, y, who.UniqueMultiplayerID, __instance);						
						break;
					case 819:
						Game1.createObjectDebris(749, x, y, who.UniqueMultiplayerID, __instance);						
						break;
					case 8:
						Game1.createObjectDebris(66, x, y, who.UniqueMultiplayerID, __instance);						
						break;
					case 10:
						Game1.createObjectDebris(68, x, y, who.UniqueMultiplayerID, __instance);						
						break;
					case 12:
						Game1.createObjectDebris(60, x, y, who.UniqueMultiplayerID, __instance);					
						break;
					case 14:
						Game1.createObjectDebris(62, x, y, who.UniqueMultiplayerID, __instance);						
						break;
					case 6:
						Game1.createObjectDebris(70, x, y, who.UniqueMultiplayerID, __instance);						
						break;
					case 4:
						Game1.createObjectDebris(64, x, y, who.UniqueMultiplayerID, __instance);						
						break;
					case 2:
						Game1.createObjectDebris(72, x, y, who.UniqueMultiplayerID, __instance);						
						break;
					case 751:
					case 849:
						Game1.createObjectDebris(378, x, y, who.UniqueMultiplayerID, __instance);
						break;
					case 290:
					case 850:
						Game1.createObjectDebris(380, x, y, who.UniqueMultiplayerID, __instance);
						break;
					case 764:
						Game1.createObjectDebris(384, x, y, who.UniqueMultiplayerID, __instance);
						break;
					case 765:
						Game1.createObjectDebris(386, x, y, who.UniqueMultiplayerID, __instance);
						break;
				}
			}
        }

	}
}
