/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using Microsoft.Xna.Framework;

namespace iTile.Client.UI.Impl
{
    public class NotificationPanel : UIPanel
    {
        // 0.0 is absolute top, 1.0 is absolute bottom
        public float verticalPos = 0.8f;
        public float showTime;
        public UILabel label;

        public string Text
        {
            get => label?.Text;
            set
            {
                label.Text = value;
                label.Center();
            }
        }

        public NotificationPanel(Vector2 dim, string text)
            : base("NotificationPanel", new Rectangle(0, 0, (int)dim.X, (int)dim.Y), null, AssetsManager.defaultTexture)
        {
            label = new UILabel("NotificationPanelLabel", Rectangle.Empty, text, this)
            {
                color = Color.White,
                Scale = 0.7f
            };
            label.Center();
            label.Show();
        }

        public void ShowTemp(int time, string text)
        {
            Text = text;
            showTime = time;
        }

        protected override void Update()
        {
            if (showTime > 0)
            {
                showTime--;
                show = true;
            }
            else
            {
                showTime = 0;
                show = false;
                return;
            }

            Center();
            label.Center();
        }

        public override void Center(bool horizontally = true, bool vertically = true)
        {
            base.Center(horizontally, vertically);
            transform.Y = (int)(ParentTransform.Height * verticalPos);
        }
    }
}
