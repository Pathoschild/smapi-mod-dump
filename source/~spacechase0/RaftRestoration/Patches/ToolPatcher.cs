/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spacechase.Shared.Harmony;
using SpaceShared;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace RaftRestoration.Patches
{
    /// <summary>Applies Harmony patches to <see cref="Tool"/>.</summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = DiagnosticMessages.NamedForHarmony)]
    internal class ToolPatcher : BasePatcher
    {
        /*********
        ** Public methods
        *********/
        /// <inheritdoc />
        public override void Apply(HarmonyInstance harmony, IMonitor monitor)
        {
            harmony.Patch(
                original: this.RequireMethod<Tool>(nameof(Tool.drawInMenu), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }),
                prefix: this.GetHarmonyMethod(nameof(Before_DrawInMenu))
            );
        }


        /*********
        ** Private methods
        *********/
        /// <summary>The method to call before <see cref="Tool.drawInMenu(SpriteBatch,Vector2,float,float,float,StackDrawType,Color,bool)"/>.</summary>
        private static bool Before_DrawInMenu(Tool __instance, SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            if (!(__instance is Raft))
                return true;

            spriteBatch.Draw(Game1.toolSpriteSheet, location + new Vector2(32f, 32f), new Rectangle(16, 0, 16, 16), color * transparency, 0.0f, new Vector2(8f, 8f), 4f * scaleSize, SpriteEffects.None, layerDepth);
            return false;
        }
    }
}
