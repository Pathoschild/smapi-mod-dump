/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewModdingAPI;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Nodes;
using Netcode;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using ItemPipes.Framework.Util;

namespace ItemPipes.Framework.Patches
{
	[HarmonyPatch(typeof(Fence))]
	public static class FencePatcher
    {


		public static void Apply(Harmony harmony)
		{
			try
			{
				harmony.Patch(
					original: AccessTools.Method(typeof(Fence), nameof(Fence.checkForAction)),
					prefix: new HarmonyMethod(typeof(FencePatcher), nameof(FencePatcher.Fence_checkForAction_Prefix))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(StardewValley.Object), nameof(StardewValley.Object.clicked)),
					prefix: new HarmonyMethod(typeof(FencePatcher), nameof(FencePatcher.Fence_clicked_Prefix))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(Fence), nameof(Fence.getDrawSum)),
					prefix: new HarmonyMethod(typeof(FencePatcher), nameof(FencePatcher.Fence_getDrawSum_Prefix))
				);
				/*harmony.Patch(
					original: AccessTools.Method(typeof(Fence), nameof(Fence.drawInMenu), new Type[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }),
					prefix: new HarmonyMethod(typeof(FencePatcher), nameof(FencePatcher.Fence_drawInMenu_Prefix))
				);*/
				harmony.Patch(
					original: AccessTools.Method(typeof(Fence), nameof(Fence.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
					prefix: new HarmonyMethod(typeof(FencePatcher), nameof(FencePatcher.Fence_draw_Prefix))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(Fence), nameof(Fence.isPassable)),
					prefix: new HarmonyMethod(typeof(FencePatcher), nameof(FencePatcher.Fence_isPassable_Prefix))
				);
				harmony.Patch(
					original: AccessTools.Method(typeof(Fence), nameof(Fence.countsForDrawing)),
					prefix: new HarmonyMethod(typeof(FencePatcher), nameof(FencePatcher.Fence_countsForDrawing_Prefix))
				);

			}
			catch (Exception ex)
			{
				if (Globals.UltraDebug) { Printer.Info($"Failed to add fence patch: {ex}"); }
			}
		}
		
		private static bool Fence_checkForAction_Prefix(Fence __instance)
		{
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true) && __instance.Name.Equals("FilterPipe"))
			{
				List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
				FilterPipeNode pipe = (FilterPipeNode)nodes.Find(n => n.Position.Equals(__instance.TileLocation));
				pipe.Chest.ShowMenu();
				return false;
			}
			else if (DataAccess.IOPipeNames.Contains(__instance.Name))
			{
				List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
				IOPipeNode pipe = (IOPipeNode)nodes.Find(n => n.Position.Equals(__instance.TileLocation));
				//Add state display
				return false;
			}
			else
			{
				return true;
			}
		}

		private static bool Fence_clicked_Prefix(StardewValley.Object __instance)
		{
			if(__instance is Fence)
            {
				DataAccess DataAccess = DataAccess.GetDataAccess();
				if (DataAccess.IOPipeNames.Contains(__instance.Name))
				{
					List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
					IOPipeNode pipe = (IOPipeNode)nodes.Find(n => n.Position.Equals(__instance.TileLocation));
					switch (pipe.State)
					{
						case "off":
							if (pipe.ConnectedContainer != null)
							{
								pipe.State = "on";
							}
							else
							{
								pipe.State = "unconnected";
							}
							break;
						case "on":
							pipe.State = "off";
							break;
						case "unconnected":
							pipe.State = "off";
							break;
					}
					return false;
				}
				else
				{
					return true;
				}
			}
			else
            {
				return true;
            }

		}

		private static bool Fence_countsForDrawing_Prefix(Fence __instance, ref bool __result, int type)
		{
			__result = false;
			DataAccess DataAccess = DataAccess.GetDataAccess();
			//Add when JA obj IDs is done, make it not.
			//if (DataAccess.ValidPipeNames.Contains(__instance.Name) && DataAccess.ValidPipeIDs.Contains(type))
			if (DataAccess.PipeNames.Contains(__instance.Name) && IsDefaultFence(type))
			{
				__result = true;
				return false;
			}
			else
            {
				return true;
            }
		}

		private static bool IsDefaultFence(int type)
        {
			if(type == 1 || type == 2 || type == 3 || type == 4 || type == 5)
            {
				return false;
            }
			else
            {
				return true;
            }
        }

		private static bool Fence_isPassable_Prefix(ref bool __result, Fence __instance)
		{
			__result = false;
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (DataAccess.PipeNames.Contains(__instance.Name))
			{
				List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
				Node node = nodes.Find(n => n.Position.Equals(__instance.TileLocation));
				if (node != null)
                {
					if (node.ParentNetwork.IsPassable)
					{
						__result = true;
					}
				}
				return false;
			}
			else
            {
				return true;
			}
		}

		private static bool Fence_getDrawSum_Prefix(ref int __result, Fence __instance, GameLocation location)
		{
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (DataAccess.PipeNames.Contains(__instance.Name))
			{
				bool CN = false;
				bool CS = false;
				bool CW = false;
				bool CE = false;
				int drawSum = 0;
				Vector2 surroundingLocations = __instance.TileLocation;
				surroundingLocations.X += 1f;
				//0 = 6
				//West = 100 = 11
				if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Fence && ((Fence)location.objects[surroundingLocations]).countsForDrawing(__instance.whichType.Value))
				{
					drawSum += 100;
				}
				else if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Chest)
				{
					CW = true;
				}
				//East = 10 = 10
				//W + E = 110 = 8
				surroundingLocations.X -= 2f;
				if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Fence && ((Fence)location.objects[surroundingLocations]).countsForDrawing(__instance.whichType.Value))
				{
					drawSum += 10;
				}
				else if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Chest)
				{
					CE = true;
				}
				//South = 500 = 6
				//S + E = 600 = 1
				//S + W = 510 = 3
				//S + E + W = 610
				surroundingLocations.X += 1f;
				surroundingLocations.Y += 1f;
				if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Fence && ((Fence)location.objects[surroundingLocations]).countsForDrawing(__instance.whichType.Value))
				{
					drawSum += 500;
				}
				else if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Chest)
				{
					CS = true;
				}
				surroundingLocations.Y -= 2f;
				//North = 1000 = 4
				//N + E = 1100 = 7
				//N + W = 1010 = 9
				//N + S = 1500 = 4
				//N + E + W = 1110 = 8
				//N + E + S = 1600 = 1
				//N + S + W = 1510 = 3
				//N + E + W  + S = 1610 = 5
				if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Fence && ((Fence)location.objects[surroundingLocations]).countsForDrawing(__instance.whichType.Value))
				{
					drawSum += 1000;
				}
				else if (location.objects.ContainsKey(surroundingLocations) && location.objects[surroundingLocations] is Chest)
				{
					CN = true;
				}
				if (DataAccess.IOPipeNames.Contains(__instance.Name))
				{
					if (CN || CS || CW || CE)
					{
						drawSum = GetAdjChestsSum(drawSum, CN, CS, CW, CE);
					}
				}
				if (__instance.Name.Equals("ConnectorPipe"))
				{
					List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
					Node node = nodes.Find(n => n.Position.Equals(__instance.TileLocation));
					if (node is ConnectorNode)
                    {
						ConnectorNode connector = (ConnectorNode)node;
						if(connector.PassingItem)
                        {
							drawSum += 5;
						}

					}
				}
				__result = drawSum;
				return false;
			}
			else
            {
				return true;
			}
		}

		private static int GetAdjChestsSum(int drawSum, bool CN, bool CS, bool CW, bool CE)
        {
			switch(drawSum)
            {
				case 0:
					if (CN) { drawSum = 500; }
					else if (CS) { drawSum = 1000; }
					else if (CW) { drawSum = 10; }
					else if (CE) { drawSum = 100; }
					break;
				case 1000:
					if (CS){drawSum += 2;}
					else if (CW){drawSum += 3;}
					else if (CE) { drawSum += 4; }
					break;
				case 500:
					if (CN) { drawSum += 1; }
					else if (CW) { drawSum += 3; }
					else if (CE) { drawSum += 4; }
					break;
				case 100:
					if (CN) { drawSum += 1; }
					else if (CS) { drawSum += 2; }
					else if (CE) { drawSum += 4; }
					break;
				case 10:
					if (CN) { drawSum += 1; }
					else if (CS) { drawSum += 2; }
					else if (CW) { drawSum += 3; }
					break;
				case 1500:
					if (CW) { drawSum += 3; }
					else if (CE) { drawSum += 4; }
					break;
				case 110:
					if (CN){drawSum += 1;}
					else if (CS){drawSum += 2;}
					break;
				case 1100:
					if (CS){drawSum += 2;}
					else if (CE){drawSum += 4;}
					break;
				case 1010:
					if (CS){drawSum += 2;}
					else if (CW){drawSum += 3;}
					break;
				case 600:
					if (CN){drawSum += 1;}
					else if (CE){drawSum += 4;}
					break;
				case 510:
					if (CN){drawSum += 1;}
					else if (CW){drawSum += 3;}
					break;
			}
			return drawSum;
		}
		
		private static bool Fence_drawInMenu_Prefix(Fence __instance, SpriteBatch spriteBatch, Vector2 location, float scale, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (DataAccess.PipeNames.Contains(__instance.Name))
			{
				location.Y -= 64f * scale;
				int sourceRectPosition = 1;
				int drawSum = __instance.getDrawSum(Game1.currentLocation);
				sourceRectPosition = GetNewDrawGuide(__instance)[drawSum];
				spriteBatch.Draw(__instance.fenceTexture.Value, location + new Vector2(32f, 32f) * scale, Game1.getArbitrarySourceRect(__instance.fenceTexture.Value, 64, 128, sourceRectPosition), color * transparency, 0f, new Vector2(32f, 32f) * scale, scale, SpriteEffects.None, layerDepth);
				return false;
			}
			else
			{
				return true;
			}

		}


		private static bool Fence_draw_Prefix(Fence __instance, SpriteBatch b, int x, int y, float alpha = 1f)
		{
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (DataAccess.PipeNames.Contains(__instance.Name))
			{
				int sourceRectPosition = 1;
				int drawSum = __instance.getDrawSum(Game1.currentLocation);
				sourceRectPosition = GetNewDrawGuide(__instance)[drawSum];
				List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
				Node node = nodes.Find(n => n.Position.Equals(__instance.TileLocation));
				if (node != null)
				{
					Texture2D signalTexture = Helper.GetHelper().Content.Load<Texture2D>($"assets/Pipes/{node.GetName()}/{node.GetState()}.png");
					b.Draw(signalTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64)), new Rectangle(sourceRectPosition * Fence.fencePieceWidth % __instance.fenceTexture.Value.Bounds.Width, sourceRectPosition * Fence.fencePieceWidth / __instance.fenceTexture.Value.Bounds.Width * Fence.fencePieceHeight, Fence.fencePieceWidth, Fence.fencePieceHeight), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 32) / 10000f);
				}
				else
                {
					b.Draw(__instance.fenceTexture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64)), new Rectangle(sourceRectPosition * Fence.fencePieceWidth % __instance.fenceTexture.Value.Bounds.Width, sourceRectPosition * Fence.fencePieceWidth / __instance.fenceTexture.Value.Bounds.Width * Fence.fencePieceHeight, Fence.fencePieceWidth, Fence.fencePieceHeight), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (float)(y * 64 + 32) / 10000f);
				}
				
				return false;
			}
			else
            {
				return true;
            }
			
		}

		private static Dictionary<int, int> GetNewDrawGuide(Fence fence)
        {
			Dictionary<int, int> DrawGuide = new Dictionary<int, int>();
			DataAccess DataAccess = DataAccess.GetDataAccess();
			if (DataAccess.NetworkItems.Contains(fence.Name))
			{
				if (fence.Name.Equals("ConnectorPipe"))
				{
					DrawGuide.Add(0, 0);
					DrawGuide.Add(1000, 1);
					DrawGuide.Add(500, 2); ;
					DrawGuide.Add(100, 3);
					DrawGuide.Add(10, 4);
					DrawGuide.Add(1500, 5);
					DrawGuide.Add(1100, 6);
					DrawGuide.Add(1010, 7);
					DrawGuide.Add(600, 8);
					DrawGuide.Add(510, 9);
					DrawGuide.Add(110, 10);
					DrawGuide.Add(1600, 11);
					DrawGuide.Add(1510, 12);
					DrawGuide.Add(1110, 13);
					DrawGuide.Add(610, 14);
					DrawGuide.Add(1610, 15);
					DrawGuide.Add(5, 0);
					DrawGuide.Add(1005, 16);
					DrawGuide.Add(505, 17); ;
					DrawGuide.Add(105, 18);
					DrawGuide.Add(15, 19);
					DrawGuide.Add(1505, 20);
					DrawGuide.Add(1105, 21);
					DrawGuide.Add(1015, 22);
					DrawGuide.Add(605, 23);
					DrawGuide.Add(515, 24);
					DrawGuide.Add(115, 25);
					DrawGuide.Add(1605, 26);
					DrawGuide.Add(1515, 27);
					DrawGuide.Add(1115, 28);
					DrawGuide.Add(615, 29);
					DrawGuide.Add(1615, 30);
				}
				else
				{
					DrawGuide.Clear();
					DrawGuide.Add(0, 0);
					DrawGuide.Add(1000, 1);
					DrawGuide.Add(1002, 1);
					DrawGuide.Add(1003, 2);
					DrawGuide.Add(1004, 3);
					DrawGuide.Add(500, 4);
					DrawGuide.Add(501, 4);
					DrawGuide.Add(503, 5);
					DrawGuide.Add(504, 6);
					DrawGuide.Add(100, 9);
					DrawGuide.Add(101, 7);
					DrawGuide.Add(102, 8);
					DrawGuide.Add(104, 9);
					DrawGuide.Add(10, 12);
					DrawGuide.Add(11, 10);
					DrawGuide.Add(12, 11);
					DrawGuide.Add(13, 12);
					DrawGuide.Add(1500, 13);
					DrawGuide.Add(1503, 13);
					DrawGuide.Add(1504, 14);
					DrawGuide.Add(1100, 15);
					DrawGuide.Add(1102, 15);
					DrawGuide.Add(1104, 16);
					DrawGuide.Add(1010, 17);
					DrawGuide.Add(1012, 17);
					DrawGuide.Add(1013, 18);
					DrawGuide.Add(600, 19);
					DrawGuide.Add(601, 19);
					DrawGuide.Add(604, 20);
					DrawGuide.Add(510, 21);
					DrawGuide.Add(511, 21);
					DrawGuide.Add(513, 22);
					DrawGuide.Add(110, 23);
					DrawGuide.Add(111, 23);
					DrawGuide.Add(112, 24);
					DrawGuide.Add(1600, 25);
					DrawGuide.Add(1510, 26);
					DrawGuide.Add(1110, 27);
					DrawGuide.Add(610, 28);
					DrawGuide.Add(1610, 29);
				}
			}
			return DrawGuide;
		}




		/*		private static bool Fence_isPassable_Prefix(ref bool __result, Fence __instance)
		{
			SGraphDB DataAccess = SGraphDB.GetSGraphDB();
			if (DataAccess.ValidItemNames.Contains(__instance.Name))
			{

				return false;
			}
			else
            {
				return true;
			}
		}
		 */
	}
}
