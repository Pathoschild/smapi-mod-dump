/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/DwarvishMattock
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace DwarvishMattock
{
	public class MeleeWeaponPatches
	{
		public static bool isScythe_Prefix(ref MeleeWeapon __instance, int index, ref bool __result)
		{
			// Mattocks are scythes.  Huh.
			if (__instance is Mattock)
			{
				__result = true;
				return false;
			}
			// Otherwise, just do the default functionality.
			return true;
		}

		public static bool DoDamage_Prefix(ref MeleeWeapon __instance, GameLocation location, int x, int y, int facingDirection, int power, Farmer who)
		{
			// Add functionality to be able to damage resource clumps with mattocks.
			if (__instance is Mattock mattock)
			{
				if (who.IsLocalPlayer)
				{
					Vector2 tileLocation3 = Vector2.Zero;
					Vector2 tileLocation2 = Vector2.Zero;
					Rectangle areaOfEffect = __instance.getAreaOfEffect(x, y, facingDirection, ref tileLocation3, ref tileLocation2, who.GetBoundingBox(), who.FarmerSprite.currentAnimationIndex);

					foreach (Vector2 v in Utility.removeDuplicates(Utility.getListOfTileLocationsForBordersOfNonTileRectangle(areaOfEffect)))
					{
						ResourceClump[] clumpCopy = new ResourceClump[location.resourceClumps.Count];
						location.resourceClumps.CopyTo(clumpCopy, 0);
						foreach (ResourceClump resourceClump in clumpCopy)
						{
							if (mattock.struckFeatures.Contains(resourceClump))
							{
								continue;
							}

							if (resourceClump.occupiesTile((int)v.X, (int)v.Y))
							{
								Tool standinTool = null;
								switch (resourceClump.parentSheetIndex.Get())
								{
									// Stumps
									case 600:
									case 602:
										standinTool = mattock.asAxe();
										break;

									// Boulders
									case 622:
									case 672:
									case 752:
									case 754:
									case 756:
									case 758:
										standinTool = mattock.asPickaxe();
										break;
								}

								if (standinTool != null)
								{
									mattock.struckFeatures.Add(resourceClump);
									if (resourceClump.performToolAction(standinTool, 1, resourceClump.tile.Get(), location))
									{
										location.resourceClumps.Remove(resourceClump);
									}
								}
								 
							}
						}
					}
				}
			}
			return true;
		}
	}
}