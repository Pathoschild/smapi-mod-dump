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
using EnhancedSlingshots.Framework.Enchantments;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancedSlingshots.Framework.Patch
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
				Vector2 playerPosition = Game1.player.GetBoundingBox().Center.ToVector2();	
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
				for(int i = 0; i < ModEntry.Instance.config.HunterEnchantment_ExtraDropsAmount; i++)
					__instance.debris.Add(monster.ModifyMonsterLoot(new Debris(Math.Abs(objects[Game1.random.Next(0, objects.Count)]), new Vector2(x, y), playerPosition)));
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch("breakStone")]
		public static void breakStone_Postfix(GameLocation __instance, int indexOfStone, int x, int y, Farmer who, Random r)
        {
			if (who.CurrentTool is Slingshot sling && sling.hasEnchantmentOfType<MinerEnchantment>())
            {
				for(int i = 0; i < ModEntry.Instance.config.MinerEnchantment_ExtraDropsAmount; x++)
                {
					switch (indexOfStone)
					{
						case 2: //Diamond
							Game1.createObjectDebris(72, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 4: //Ruby
							Game1.createObjectDebris(64, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 6: //Jade
							Game1.createObjectDebris(70, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 8: //Amethyst
							Game1.createObjectDebris(66, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 10: //Topaz
							Game1.createObjectDebris(68, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 12: //Emerald
							Game1.createObjectDebris(60, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 14: //Aquamarine
							Game1.createObjectDebris(62, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 75: //Geode
							Game1.createObjectDebris(535, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 76: //Frozen Geode
							Game1.createObjectDebris(536, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 77: //Magma Geode
							Game1.createObjectDebris(537, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 95: //Radioactive ore
							Game1.createObjectDebris(909, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 290: //Iron ore
						case 850: //Iron ore (volcano version)
							Game1.createObjectDebris(380, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 751: //Copper ore
						case 849: //Copper ore (volcano version)  
							Game1.createObjectDebris(378, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 764: //Gold ore
							Game1.createObjectDebris(384, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 765: //Iridium ore
							Game1.createObjectDebris(386, x, y, who.UniqueMultiplayerID, __instance);
							return;
						case 819: //Omnigeode
							Game1.createObjectDebris(749, x, y, who.UniqueMultiplayerID, __instance);
							return;
					}
				}				
			}
        }

	}
}
