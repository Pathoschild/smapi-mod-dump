/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using System;
using System.Text;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;

namespace BoatSpriteMode
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public static ModEntry context;//single instance
        public static Harmony harmony;
		public static IModHelper Helper;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
			Helper = helper;
			context = this;

			harmony = new Harmony(ModManifest.UniqueID);

			//Before drawing replace how it draws the boat
			harmony.Patch(
                original: AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.draw), new Type[] { typeof(SpriteBatch) }),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(pre_draw_boattunnel))
            );

			//Allow calling of the base GameLocation
			harmony.CreateReversePatcher(AccessTools.Method(typeof(GameLocation), nameof(GameLocation.draw), new[] { typeof(SpriteBatch) }),
				new HarmonyMethod(GetType(), nameof(ExecuteGameLocationReversePatch))
				).Patch();

			//Adjust willy's walking path
			harmony.Patch(
				original: AccessTools.Method(typeof(BoatTunnel), nameof(BoatTunnel.StartDeparture)),
				postfix: new HarmonyMethod(typeof(ModEntry), nameof(post_StartDeparture))
			);

			//Before drawing replace how it draws
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandSouth), nameof(IslandSouth.draw), new Type[] { typeof(SpriteBatch) }),
				prefix: new HarmonyMethod(typeof(ModEntry), nameof(pre_draw_islandsouth))
			);

			//Allow calling of the base file
			harmony.CreateReversePatcher(AccessTools.Method(typeof(IslandLocation), nameof(IslandLocation.draw), new[] { typeof(SpriteBatch) }),
				new HarmonyMethod(GetType(), nameof(ExecuteIslandLocationReversePatch))
				).Patch();

			//Adjust willy and players position on the boat
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandSouth), nameof(IslandSouth.Depart)),
				prefix: new HarmonyMethod(typeof(ModEntry), nameof(pre_Depart_islandsouth))
			);

			//Adjust smoke/bubbles position
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandSouth), nameof(IslandSouth.UpdateWhenCurrentLocation)),
				prefix: new HarmonyMethod(typeof(ModEntry), nameof(pre_UpdateWhenCurrentLocation))
			);

			//Adjust lights on bots position
			harmony.Patch(
				original: AccessTools.Method(typeof(IslandSouth), nameof(IslandSouth.UpdateWhenCurrentLocation)),
				postfix: new HarmonyMethod(typeof(ModEntry), nameof(post_UpdateWhenCurrentLocation))
			);
		}

        public static bool pre_draw_boattunnel(BoatTunnel __instance, Texture2D ___boatTexture, Vector2 ___boatPosition, float ____plankShake, int ____gateFrame, SpriteBatch b)
        {
			ExecuteGameLocationReversePatch((GameLocation)__instance, b);//base.draw(b);

			float[] offsets = new float[] { 0, 1, 1, 2, 1, 1, 0, -1, -1, 0 };
			Vector2 waterbob = Vector2.Zero;
			if (__instance.currentEvent == null)
            {
				waterbob = new Vector2(0, offsets[__instance.waterAnimationIndex]);//10 frames before it warps, and takes 2 seconds
			}

			Vector2 boat_position = __instance.GetBoatPosition();
			//Useable boat
			if (Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed") && Game1.farmEvent == null)
			{
				//Draw base boat
				b.Draw(___boatTexture, Game1.GlobalToLocal(boat_position+new Vector2(-4f, 0) * 4f)+ waterbob, new Microsoft.Xna.Framework.Rectangle(0, 0, 181, 118), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ___boatPosition.Y / 10000f);
				//Draw overlay
				b.Draw(___boatTexture, Game1.GlobalToLocal(boat_position + new Vector2(8f, 0) * 4f) + waterbob, new Microsoft.Xna.Framework.Rectangle(0, 160, 116, 96), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (___boatPosition.Y + 408f) / 10000f);
				Vector2 plank_shake = Vector2.Zero;
				if (!__instance.PlankFinishedAnimating() || ____plankShake > 0f)
				{
					plank_shake = new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
				}
				//Plank
				Vector2 tileposition = new Vector2(6f, 8f) * 64f;
				b.Draw(___boatTexture, Game1.GlobalToLocal(tileposition + new Vector2(0f, (int)__instance._plankPosition + 4) * 4f + plank_shake), new Microsoft.Xna.Framework.Rectangle(128, 176, 17, 33), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (512f + __instance._plankPosition * 4f) / 10000f);
				
				//Gate
				Microsoft.Xna.Framework.Rectangle gate_draw_rect = __instance.gateRect;
				gate_draw_rect.X = ____gateFrame * __instance.gateRect.Width;
				b.Draw(___boatTexture, Game1.GlobalToLocal(boat_position + new Vector2(35f, 81f) * 4f)+ waterbob, gate_draw_rect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (___boatPosition.Y + 428f) / 10000f);
			}
			else
			{
				//Draw base boat
				b.Draw(___boatTexture, Game1.GlobalToLocal(boat_position + new Vector2(-4f, 0) * 4f) + waterbob, new Microsoft.Xna.Framework.Rectangle(0, 259, 181, 122), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, ___boatPosition.Y / 10000f);
				//Draw chain, with half bobbing
				b.Draw(___boatTexture, Game1.GlobalToLocal(boat_position + new Vector2(86f, 88f) * 4f) + new Vector2(0, (int)waterbob.Y/2), new Microsoft.Xna.Framework.Rectangle(156, 236, 5, 20), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (___boatPosition.Y+88) / 10000f);
				//Draw anchor, no bobbing
				b.Draw(___boatTexture, Game1.GlobalToLocal(boat_position + new Vector2(78f, 104f) * 4f), new Microsoft.Xna.Framework.Rectangle(116+((int)__instance.waterAnimationIndex/5)*20, 238, 20, 18), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (___boatPosition.Y+104f) / 10000f);
				//Draw plank
				b.Draw(___boatTexture, Game1.GlobalToLocal(new Vector2(6f, 9f) * 64f + new Vector2(0f, (int)__instance._plankPosition) * 4f), new Microsoft.Xna.Framework.Rectangle(128, 176, 17, 33), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (512f + __instance._plankPosition * 4f) / 10000f);
				float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds / 250.0), 2);
				if (!Game1.eventUp)
				{
					if (!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatHull"))
					{
						b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(416f, 456f + yOffset)), new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8), Color.White, 0f, new Vector2(1f, 4f), 4f + Math.Max(0f, 0.25f - yOffset / 4f), SpriteEffects.None, 1f);
					}
					if (!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatTicketMachine"))
					{
						b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(288f, 520f + yOffset)), new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8), Color.White, 0f, new Vector2(1f, 4f), 4f + Math.Max(0f, 0.25f - yOffset / 4f), SpriteEffects.None, 1f);
					}
					if (!Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatAnchor"))
					{
						b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(560f, 520f + yOffset)), new Microsoft.Xna.Framework.Rectangle(395, 497, 3, 8), Color.White, 0f, new Vector2(1f, 4f), 4f + Math.Max(0f, 0.25f - yOffset / 4f), SpriteEffects.None, 1f);
					}
				}
			}
			b.Draw(___boatTexture, Game1.GlobalToLocal(new Vector2(4f, 8f) * 64f), new Microsoft.Xna.Framework.Rectangle(160, 192, 16, 32), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0512f);

			//prevent further rendering
			return false;
		}

		public static void ExecuteGameLocationReversePatch(GameLocation __instance, SpriteBatch b)
		{
			new NotImplementedException("It's a stub!");
		}

		public static void post_StartDeparture(BoatTunnel __instance, Event ____boatEvent)
		{
			//Stop will from walking so low
			for(int i = 0; i < ____boatEvent.eventCommands.Length; i++)
            {
				if(____boatEvent.eventCommands[i] == "move Willy 0 1 2")
                {
					____boatEvent.eventCommands[i] = "move Willy 0 0 2";
				}
			}
		}

		public static bool pre_draw_islandsouth(IslandSouth __instance, SpriteBatch b)
		{
			ExecuteIslandLocationReversePatch((IslandLocation)__instance, b); //base.draw(b);

			float[] offsets = new float[] { 0, 1, 1, 2, 1, 1, 0, -1, -1, 0 };
			Vector2 waterbob = Vector2.Zero;
			if (__instance.currentEvent == null || __instance.currentEvent.id != -157039427)
			{
				waterbob = new Vector2(0, offsets[__instance.waterAnimationIndex]);//10 frames before it warps, and takes 2 seconds
			}

			Vector2 boat_position = __instance.GetBoatPosition();
			b.Draw(__instance.boatTexture, Game1.GlobalToLocal(boat_position)+ waterbob, new Microsoft.Xna.Framework.Rectangle(192, 0, 96, 208), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (__instance.boatPosition.Y + 320f) / 10000f);
			b.Draw(__instance.boatTexture, Game1.GlobalToLocal(boat_position)+ waterbob, new Microsoft.Xna.Framework.Rectangle(288, 0, 96, 208), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, (__instance.boatPosition.Y + 616f) / 10000f);
			if (__instance.currentEvent == null || __instance.currentEvent.id != -157039427)
			{
				b.Draw(__instance.boatTexture, Game1.GlobalToLocal(new Vector2(1184f, 2752f)), new Microsoft.Xna.Framework.Rectangle(192, 208, 32, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.272f);
			}

			//prevent further rendering
			return false;
		}

		public static void ExecuteIslandLocationReversePatch(IslandLocation __instance, SpriteBatch b)
		{
			new NotImplementedException("It's a stub!");
		}

		public static bool pre_Depart_islandsouth(IslandSouth __instance)
		{
			Event departEvent = new Event(Game1.content.LoadString("Data\\Events\\IslandSouth:IslandDepart"), -157039427, Game1.player);
			
			for (int i = 0; i < departEvent.eventCommands.Length; i++)
			{
				if (departEvent.eventCommands[i] == "farmer 15 43 2 Willy 17 45 2")
				{
					departEvent.eventCommands[i] = "farmer 16 42 2 Willy 17 46 2";
				}
			}

			Game1.globalFadeToBlack(delegate
			{
				__instance.currentEvent = departEvent;
				Game1.eventUp = true;
			});

			//prevent further the default action
			return false;
		}

		public static void pre_UpdateWhenCurrentLocation(IslandSouth __instance, int ____boatDirection, ref float ____nextBubble, ref float ____nextSmoke, GameTime time)
		{
			//Create bubbles in the right spot
			if (____boatDirection != 0)
			{
				if(____nextBubble - (float)time.ElapsedGameTime.TotalSeconds <= 0f){
					Microsoft.Xna.Framework.Rectangle back_rectangle = new Microsoft.Xna.Framework.Rectangle(64, 128, 192, 64);
					back_rectangle.X += (int)__instance.GetBoatPosition().X;
					back_rectangle.Y += (int)__instance.GetBoatPosition().Y;
					Vector2 position2 = Utility.getRandomPositionInThisRectangle(back_rectangle, Game1.random);
					TemporaryAnimatedSprite sprite2 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 50f, 9, 1, position2, flicker: false, flipped: false, 0f, 0.025f, Color.White, 1f, 0f, 0f, 0f);
					sprite2.acceleration = new Vector2(0f, -0.25f * (float)Math.Sign(____boatDirection));
					__instance.temporarySprites.Add(sprite2);
					____nextBubble = 0.01f;
				}
			}

			//Create the smoke before and put it in the right spot
			if (__instance.currentEvent != null && __instance.currentEvent.id == -157039427)
			{
				//Stop willy from breathing
				foreach (NPC actor in __instance.currentEvent.actors)
				{
					actor.breather.Set(false);
				}

				if (____nextSmoke > 0f)
				{
					____nextSmoke -= (float)time.ElapsedGameTime.TotalSeconds;
				}
				else
				{
					Vector2 position = new Vector2(3f, 2.5f) * 64f + __instance.GetBoatPosition();
					TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 1600, 64, 128), 200f, 9, 1, position, flicker: false, flipped: false, 1f, 0.025f, Color.White, 1f, 0.025f, 0f, 0f);
					sprite.acceleration = new Vector2(-0.25f, -0.15f);
					__instance.temporarySprites.Add(sprite);
					____nextSmoke = 0.2f;
				}
			}
		}

		public static void post_UpdateWhenCurrentLocation(IslandSouth __instance, GameTime time)
		{
			//light sources for the boat
			if (__instance.boatLight != null)
			{
				__instance.boatLight.position.Value = new Vector2(3f, 1f+6f) * 64f + __instance.GetBoatPosition();
			}
			if (__instance.boatStringLight != null)
			{
				__instance.boatStringLight.position.Value = new Vector2(3f, 4f+4f) * 64f + __instance.GetBoatPosition();
			}
		}
	}
}
