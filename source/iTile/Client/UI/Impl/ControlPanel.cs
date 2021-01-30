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
using iTile.Core.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using static iTile.Client.UI.UIButton;

namespace iTile.Client.UI.Impl
{
    public class ControlPanel : UIPanel
    {
        public static readonly int btnDim = 50;
        public UIButton moveBtn;
        public UIPanel moveBtnIcon;
        public UIButton tileModeBtn;
        public UIPanel tileModeBtnIcon;
        public UIButton settingsBtn;
        public UIPanel settingsBtnIcon;
        public UIButton actionBtn;
        public UIPanel actionBtnIcon;
        public UIButton layersBtn;
        public UIPanel layersBtnIcon;
        public LayersPanel layersPanel;

        public Texture2D copyAction;
        public Texture2D pasteAction;
        public Texture2D deleteAction;
        public Texture2D restoreAction;

        private Rectangle iconRect = new Rectangle(0, 0, btnDim, btnDim);

        public ControlPanel(Point pos) : base("ControlPanel", new Rectangle(pos.X, pos.Y, btnDim * 2, btnDim * 3), null)
        {
            InitMoveBtn();
            InitTileModeBtn();
            //InitSettingsBtn(); //WIP
            InitActionBtn();
            InitLayersBtn();
        }

        public void InitButton(
            ref UIButton button,
            ref UIPanel icon,
            string name,
            string pattern,
            Texture2D iconTexture,
            OnClickEvent onClick = null,
            UIElement parent = null,
            bool pressable = false,
            bool movable = false,
            bool show = true)
        {
            if (string.IsNullOrEmpty(pattern) || pattern.Length != 2)
                return;

            int x = Convert.ToInt32(pattern[0].ToString());
            int y = Convert.ToInt32(pattern[1].ToString());
            int posX = (parent != null ? 0 : transform.X) + btnDim * x;
            int posY = (parent != null ? 0 : transform.Y) + btnDim * y;
            button = new UIButton(name, new Rectangle(posX, posY, btnDim, btnDim), parent, AssetsManager.defaultTexture);
            if (show)
                button.Show();
            if (pressable)
                button.canBePressed = true;
            if (movable)
                button.canBeMoved = true;
            icon = (UIPanel)new UIPanel(name + "_Icon", iconRect, button, iconTexture).Show();
            icon.color = Color.White;
            if (onClick != null)
                button.onClick += onClick;
        }

        private void OnTileMode()
        {
            bool state = !tileModeBtn.pressed;
            CoreManager.Instance.tileMode.OnTileModeControl(state);
            tileModeBtnIcon.color = state ? defaultColor : Color.White;
            layersBtnIcon.color = Color.White;
            actionBtn.show = layersBtn.show = state;
            layersBtn.pressed = layersPanel.show = false;
        }

        private void OnTileModeAction()
        {
            CoreManager.Instance.tileMode.OnTileModeActionControl(this);
        }

        private void OnLayers()
        {
            bool state = !layersBtn.pressed;
            layersBtnIcon.color = state ? defaultColor : Color.White;
            layersPanel.show = state;
        }

        public void InitMoveBtn()
        {
            InitButton(ref moveBtn, ref moveBtnIcon, "ControlPanelMoveBtn", "10", AssetsManager.LoadTexture("MoveButton.png"), movable: true, show: false);
            SetParent(moveBtn);
        }

        public void InitTileModeBtn()
        {
            InitButton(ref tileModeBtn, ref tileModeBtnIcon, "TileModeBtn", "00", AssetsManager.LoadTexture("TileModeButton.png"), OnTileMode, this, true);
        }

        public void InitSettingsBtn()
        {
            InitButton(ref settingsBtn, ref settingsBtnIcon, "SettingsBtn", /*This is actual 01, don't forget to change*/"01", AssetsManager.LoadTexture("SettingsButton.png"), parent: this);
        }

        public void InitActionBtn()
        {
            copyAction = AssetsManager.LoadTexture("ActionCopy.png");
            pasteAction = AssetsManager.LoadTexture("ActionPaste.png");
            deleteAction = AssetsManager.LoadTexture("ActionDelete.png");
            restoreAction = AssetsManager.LoadTexture("ActionRestore.png");
            InitButton(ref actionBtn, ref actionBtnIcon, "ActionBtn", "01", copyAction, OnTileModeAction, this, show: false);
        }

        public void InitLayersBtn()
        {
            InitButton(ref layersBtn, ref layersBtnIcon, "LayersBtn", "02", AssetsManager.LoadTexture("LayersButton.png"), OnLayers, this, true, show: false);
            layersPanel = new LayersPanel(new Point(btnDim, 0), layersBtn);
        }

        protected override void OnButtonPressed(ButtonPressedEventArgs e)
        {
            if (e.Button == CoreManager.Instance.config.TogglePanelKey)
            {
                tileModeBtn.pressed = true;
                OnTileMode();
                tileModeBtn.pressed = false;
                moveBtn.Toggle();
            }
        }
    }
}