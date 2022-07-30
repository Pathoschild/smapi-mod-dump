/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using System.Net.WebSockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmartBuilding.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace SmartBuilding.UI
{
    public class ToolButton
    {
        private ClickableTextureComponent buttonComponent;
        private ButtonId buttonId;
        private ButtonType buttonType;
        private Action buttonAction;
        private TileFeature? layerToTarget;
        private Color currentOverlayColour;
        private bool isHovered;
        private bool enabled;
        private string buttonTooltip;
        private ModState modState;

        public ClickableTextureComponent Component
        {
            get => buttonComponent;
        }

        public bool Enabled
        {
            get => enabled;
            set { enabled = value; }
        }

        public bool IsHovered
        {
            get => isHovered;
            set { isHovered = value; }
        }

        public string ButtonTooltip
        {
            get => buttonTooltip;
        }

        public Color CurrentOverlayColour
        {
            get => currentOverlayColour;
            set { currentOverlayColour = value; }
        }

        public ButtonId Id
        {
            get => buttonId;
        }

        public ButtonType Type
        {
            get => buttonType;
        }

        public Action ButtonAction
        {
            get => buttonAction;
        }

        public TileFeature? LayerToTarget
        {
            get => layerToTarget;
        }

        
        public void Draw(SpriteBatch b)
        {
            if (Type != ButtonType.Layer)
                b.Draw(Component.texture, new Vector2(Component.bounds.X, Component.bounds.Y), Component.sourceRect, CurrentOverlayColour, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            else
            {
                if (modState.ActiveTool != ButtonId.None)
                {
                    if (modState.ActiveTool.Equals(ButtonId.Erase))
                    {
                        b.Draw(Component.texture, new Vector2(Component.bounds.X, Component.bounds.Y), Component.sourceRect, CurrentOverlayColour, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    }
                    else
                    {
                        //b.Draw(Component.texture, new Vector2(Component.bounds.X, Component.bounds.Y), Component.sourceRect, Color.DarkSlateGray, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    }
                }
                //else
                    //b.Draw(Component.texture, new Vector2(Component.bounds.X, Component.bounds.Y), Component.sourceRect, Color.DarkSlateGray, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            }

            if (Type == ButtonType.Tool)
            {
                if (modState.ActiveTool != ButtonId.None)
                {
                    if (Id.Equals(modState.ActiveTool))
                    {
                        b.Draw(
                            Game1.mouseCursors,
                            new Vector2(Component.bounds.X, Component.bounds.Y),
                            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29),
                            Color.Black,
                            0.0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            0f);
                    }
                }
            }

            if (Type == ButtonType.Layer)
            {
                if (modState.ActiveTool != ButtonId.None)
                {
                    // b.Draw(
                    //     Game1.mouseCursors,
                    //     new Vector2(Component.bounds.X, Component.bounds.Y),
                    //     Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29),
                    //     Color.White,
                    //     0.0f,
                    //     Vector2.Zero,
                    //     1f,
                    //     SpriteEffects.None,
                    //     0f);

                    if (LayerToTarget.Equals(modState.SelectedLayer))
                    {
                        b.Draw(
                            Game1.mouseCursors,
                            new Vector2(Component.bounds.X + 80, Component.bounds.Y + 8),
                            new Rectangle(352, 495, 12, 11),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            0f);

                        Utility.drawTextWithColoredShadow(b, ButtonTooltip, Game1.dialogueFont, new Vector2(Component.bounds.X + 96, Component.bounds.Y + 64), Color.WhiteSmoke, new Color(Color.Black, 0.75f));
                    }
                }
            }
        }

        /// <summary>
        /// Everything can be derived from the button ID.
        /// </summary>
        /// <param name="button"></param>
        public ToolButton(ButtonId button, ButtonType type, Action action, string tooltip, Texture2D texture, ModState modState, TileFeature? layerToTarget = null)

        {
            Rectangle sourceRect = Ui.GetButtonSourceRect(button);
            buttonTooltip = tooltip;
            buttonId = button;
            buttonType = type;
            buttonAction = action;
            this.layerToTarget = layerToTarget;
            currentOverlayColour = Color.White;
            this.modState = modState;

            buttonComponent = new ClickableTextureComponent(
                new Rectangle(0, 0, 0, 0),
                texture,
                sourceRect,
                1f
            );
        }
    }
}