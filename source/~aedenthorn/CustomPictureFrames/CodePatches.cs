/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace CustomPictureFrames
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry
    {
        private static void GameLocation_draw_Postfix(GameLocation __instance, SpriteBatch b)
        {
            if (!Config.EnableMod)
                return;

            foreach (Furniture f in __instance.furniture)
            {
                if (!f.modData.ContainsKey("aedenthorn.CustomPictureFrames/index"))
                    continue;
                if (f.modData["aedenthorn.CustomPictureFrames/index"] == "-1" || !int.TryParse(f.modData["aedenthorn.CustomPictureFrames/index"], out int index))
                    continue;

                string key;
                if (pictureDict.ContainsKey(f.Name))
                    key = f.Name;
                else if (f.Name.Contains("/") && pictureDict.ContainsKey(f.Name.Split('/')[1]))
                    key = f.Name.Split('/')[1];
                else 
                    continue;
                if (pictureDict[key].Count <= index)
                {
                    f.modData["aedenthorn.CustomPictureFrames/index"] = "-1";
                    return;
                }
                Texture2D texture = pictureDict[key][index];

                b.Draw(texture, Game1.GlobalToLocal(Game1.viewport, SHelper.Reflection.GetField<NetVector2>(f, "drawPosition").GetValue() + ((f.shakeTimer > 0) ? new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) : Vector2.Zero)), new Rectangle(0, 0, texture.Width, texture.Height), Color.White, 0f, Vector2.Zero, 1f, f.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (f.furniture_type.Value == 12) ? (2E-09f + f.TileLocation.Y / 100000f) : ((f.boundingBox.Value.Bottom - ((f.furniture_type.Value == 6 || f.furniture_type.Value == 17 || f.furniture_type.Value == 13) ? 48 : 8)) / 10000f));
            }
        } 
        private static void Furniture_placementAction_Postfix(Furniture __instance)
        {
            if (!Config.EnableMod)
                return;
            SMonitor.Log($"furniture name {__instance.Name}");
        }
    }
}