using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI.Framework;
using StardewModdingAPI.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Starvation
{
    class ModEntry : Mod
    {
        private Texture2D HungerBarStatus;
        private Texture2D HungerBarBack;

        public override void Entry(IModHelper helper)
        {
            HungerBarBack = this.Helper.Content.Load<Texture2D>("assets/HungerBarBack.png");
            HungerBarStatus = this.Helper.Content.Load<Texture2D>("assets/HungerBarStatus.png");

            helper.Events.Display.RenderingHud += this.DisplayRenderingHud;
        }

        private void DisplayRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.activeClickableMenu != null)
            {
                return;
            }

            else
            {
                e.SpriteBatch.Draw(HungerBarBack, new Rectangle(0 ,0 ,12 ,56), Color.White);
            }
        }
    }
}
