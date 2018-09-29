
using System.Reflection;
using Harmony;
using StardewValley;
using StardewValley.Network;
using System;
using System.IO;
using CustomEmojis.Framework.Patches;
using CustomEmojis.Framework.Extensions;
using CustomEmojis.Framework.Constants;
using StardewModdingAPI;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using CustomEmojis.Framework.Menu;

namespace CustomEmojis.Patches {

	internal static class Game1Patch {

		internal class DrawOverlaysPatch : ClassPatch {

			public override MethodInfo Original => AccessTools.Method(typeof(Game1), "drawOverlays", new Type[] { typeof(SpriteBatch) });
			public override MethodInfo Postfix => AccessTools.Method(this.GetType(), nameof(DrawOverlaysPatch.DrawOverlays_Postfix));

			private static void DrawOverlays_Postfix(Game1 __instance, ref SpriteBatch spriteBatch) {

				if(Game1.chatBox != null) {

					spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
					
					foreach(IClickableMenu menu in Game1.onScreenMenus) {
						if(menu is CachedMessageEmojis cachedMessageEmojis) {
							cachedMessageEmojis.DrawMessages(spriteBatch);
						}
					}

					//if((Game1.displayHUD || Game1.eventUp) && (Game1.currentBillboard == 0 && Game1.gameMode == (byte)3) && (!Game1.freezeControls && !Game1.panMode)) {
					//	__instance.drawMouseCursor();
					//}

					spriteBatch.End();

				}

			}

		}

	}

}
