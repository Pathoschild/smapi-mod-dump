/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Ping
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.RenderedHud += OnRenderedHud;
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            if(Game1.IsClient)
            {
                if(Game1.client != null)
                {
                    SpriteFont smallFont = Game1.smallFont;
                    string txt = $"Ping: {(int)Game1.client.GetPingToHost()} ms";
                    Vector2 size = smallFont.MeasureString(txt);
                    Game1.DrawBox(8, 8, (int)size.X, (int)size.Y);
                    e.SpriteBatch.DrawString(smallFont, txt, new Vector2(8, 8), Color.Black);
                }
            }
        }
    }
}
