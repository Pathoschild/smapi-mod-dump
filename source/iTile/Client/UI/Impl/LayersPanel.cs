/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using System.Linq;
using iTile.Client.UI.Framework;
using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using iTile.Utils;

namespace iTile.Client.UI.Impl
{
    public class LayersPanel : UIPanel
    {
        public static readonly int btnWidth = 150;
        public static readonly int btnHeight = 40;
        public static readonly int layersCount = 5;
        public static readonly float fontScale = 0.6f;
        private Point defPos;
        public Dictionary<UIButton, UILabel> layerBtns;
        private string selectedLayer = string.Empty;
        public string SelectedLayer
        {
            get => selectedLayer;
            set
            {
                UIButton btn = GetLayerBtnByName(value).Key;
                if (btn == null)
                    return;

                if (btn.pressed)
                {
                    selectedLayer = string.Empty;
                }
                else
                {
                    selectedLayer = value;
                }
                UnpressAll(value);
            }
        }

        public LayersPanel(Point pos, UIElement parent)
            : base("LayersPanel", new Rectangle(pos.X, pos.Y, btnWidth, btnHeight * layersCount), parent/*, AssetsManager.defaultTexture*/)
        {
            defPos = pos;
            layerBtns = new Dictionary<UIButton, UILabel>();
            FillIn(Layers.layerIDs.ToArray());
        }

        protected override void Update()
        {
            CheckOutOfScreenBounds();
        }

        private void CheckOutOfScreenBounds()
        {
            Point defPosGlobal = parent.LocalToGlobalPos(defPos);
            bool flag1 = defPosGlobal.X + transform.Width > Game1.viewport.Width;
            bool flag2 = defPosGlobal.Y + transform.Height > Game1.viewport.Height;
            if (flag1 || flag2)
            {
                LocalPosition = new Point(-btnWidth, flag2 ? ControlPanel.btnDim - transform.Height : defPos.Y);
            }
            else
            {
                LocalPosition = defPos;
            }
        }

        private KeyValuePair<UIButton, UILabel> GetLayerBtnByName(string name)
        {
            return layerBtns.FirstOrDefault(pair => pair.Key.Name == name);
        }

        private void OnClick(string name)
        {
            SelectedLayer = name;
        }

        private void UnpressAll(string name)
        {
            layerBtns.ToList().ForEach(pair =>
            {
                UIButton btn = pair.Key;
                UILabel lbl = pair.Value;
                if (btn.Name != name)
                {
                    btn.pressed = false;
                }
                else if (!btn.pressed)
                {
                    lbl.color = defaultColor;
                    return;
                }
                lbl.color = Color.White;
            });
        }

        private void FillIn(params string[] layerNames)
        {
            if (layerNames.Length < layersCount)
                return;

            for (int i = 0; i < layersCount; i++)
            {
                InitPair(i, layerNames[i]);
            }
        }

        private void InitPair(int index, string name)
        {
            UIButton btn = (UIButton)new UIButton(
                name,
                new Rectangle(0, index * btnHeight, btnWidth, btnHeight),
                this,
                AssetsManager.defaultTexture).Show();
            btn.canBePressed = true;
            UILabel lbl = (UILabel)new UILabel(
                name + "_Icon",
                Rectangle.Empty,
                name,
                btn).Show();
            lbl.color = Color.White;
            lbl.Scale = fontScale;
            lbl.Center();
            btn.onClick += () => OnClick(btn.Name);
            layerBtns.Add(btn, lbl);
        }
    }
}