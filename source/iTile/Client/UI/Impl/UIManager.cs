/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using iTile.Client.UI.Framework;
using iTile.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace iTile.Client.UI.Impl
{
    public class UIManager : Manager
    {
        public Texture2D defTex;
        public ControlPanel controlPanel;
        public NotificationPanel notifPanel;

        public UIManager()
        {
            Init();
        }

        public override void Init()
        {
            defTex = CoreManager.Instance.assetsManager.defaultTexture;
            InitControlPanel();
            InitNotifPanel();
            Helper.Events.Display.RenderedHud += Display_RenderedHud;
        }

        public void InitControlPanel()
        {
            controlPanel = (ControlPanel)new ControlPanel(new Point(200, 200)).Show();
            controlPanel.processClicks = true;
        }

        public void InitNotifPanel()
        {
            notifPanel = new NotificationPanel(new Vector2(400, 80), string.Empty);
        }

        private void Display_RenderedHud(object sender, StardewModdingAPI.Events.RenderedHudEventArgs e)
        {
            controlPanel?.parent?.MaybeDraw();
            notifPanel?.MaybeDraw();
        }

        public void ShowNotification(int time, string text)
        {
            notifPanel?.ShowTemp(time, text);
        }
    }
}