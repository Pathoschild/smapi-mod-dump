/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlatoUI.UI.Components;
using StardewValley;
using StardewValley.Menus;
using System;

namespace PlatoUI.UI
{
    class UIMenu : IClickableMenu, IUIMenu
    {
        public virtual IWrapper Menu { get; }

        protected readonly IPlatoUIHelper Helper;

        public Func<bool> ShouldDraw { get; set; } = () => true;

        private float LastUIZoom { get; set; } = 1f ;

        public UIMenu(IPlatoUIHelper helper, IWrapper menu)
            :  base(0,0,Game1.viewport.Width, Game1.viewport.Height, false)
        {
#if ANDROID
#else
            LastUIZoom = Game1.options.desiredUIScale;
            Game1.options.desiredUIScale = Game1.options.desiredBaseZoomLevel;
#endif
            Helper = helper;
            Menu = menu;
            Helper.ModHelper.Events.GameLoop.UpdateTicked += ResetZoom;
        }

        private void ResetZoom(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if (!(Game1.activeClickableMenu is UIMenu))
            {
#if ANDROID
#else
                Game1.options.desiredUIScale = LastUIZoom;
#endif
                Helper.ModHelper.Events.GameLoop.UpdateTicked -= ResetZoom;
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (ShouldDraw?.Invoke() ?? true)
            {
                Menu.Draw(b);
                drawMouse(b);
            }
        }

        public override void update(GameTime time)
        {
            Menu.Update(null);
        }
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            Menu.Repopulate();
            Menu.Recompose();
        }

        public override void draw(SpriteBatch b, int red = -1, int green = -1, int blue = -1)
        {
            this.draw(b);
        }

        public override void emergencyShutDown()
        {
            base.emergencyShutDown();
        }

    }
}
