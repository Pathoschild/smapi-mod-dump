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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmartBuilding.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace SmartBuilding.UI
{
    public class ToolButton
    {
        private readonly ModState modState;

        /// <summary>
        ///     Everything can be derived from the button ID.
        /// </summary>
        /// <param name="button"></param>
        public ToolButton(ButtonId button, ButtonType type, Action action, string tooltip, Texture2D texture,
                          ModState modState, TileFeature? layerToTarget = null)

        {
            var sourceRect = Ui.GetButtonSourceRect(button);
            this.ButtonTooltip = tooltip;
            this.Id = button;
            this.Type = type;
            this.ButtonAction = action;
            this.LayerToTarget = layerToTarget;
            this.CurrentOverlayColour = Color.White;
            this.modState = modState;

            this.Component = new ClickableTextureComponent(
                new Rectangle(0, 0, 0, 0),
                texture,
                sourceRect,
                1f
            );
        }

        public ClickableTextureComponent Component { get; }

        public bool Enabled { get; set; }

        public bool IsHovered { get; set; }

        public string ButtonTooltip { get; }

        public Color CurrentOverlayColour { get; set; }

        public ButtonId Id { get; }

        public ButtonType Type { get; }

        public Action ButtonAction { get; }

        public TileFeature? LayerToTarget { get; }


        public void Draw(SpriteBatch b)
        {
            if (this.Type != ButtonType.Layer)
                b.Draw(this.Component.texture, new Vector2(this.Component.bounds.X, this.Component.bounds.Y),
                    this.Component.sourceRect, this.CurrentOverlayColour, 0f, Vector2.Zero, 4f, SpriteEffects.None,
                    1f);
            else
            {
                if (this.modState.ActiveTool != ButtonId.None)
                    if (this.modState.ActiveTool.Equals(ButtonId.Erase))
                        b.Draw(this.Component.texture, new Vector2(this.Component.bounds.X, this.Component.bounds.Y),
                            this.Component.sourceRect, this.CurrentOverlayColour, 0f, Vector2.Zero, 4f,
                            SpriteEffects.None, 1f);
                //b.Draw(Component.texture, new Vector2(Component.bounds.X, Component.bounds.Y), Component.sourceRect, Color.DarkSlateGray, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                //else
                //b.Draw(Component.texture, new Vector2(Component.bounds.X, Component.bounds.Y), Component.sourceRect, Color.DarkSlateGray, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            }

            if (this.Type == ButtonType.Tool)
                if (this.modState.ActiveTool != ButtonId.None)
                    if (this.Id.Equals(this.modState.ActiveTool))
                        b.Draw(
                            Game1.mouseCursors,
                            new Vector2(this.Component.bounds.X, this.Component.bounds.Y),
                            Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29),
                            Color.Black,
                            0.0f,
                            Vector2.Zero,
                            1f,
                            SpriteEffects.None,
                            0f);

            if (this.Type == ButtonType.Layer)
                if (this.modState.ActiveTool != ButtonId.None)
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
                    if (this.LayerToTarget.Equals(this.modState.SelectedLayer))
                    {
                        b.Draw(
                            Game1.mouseCursors,
                            new Vector2(this.Component.bounds.X + 80, this.Component.bounds.Y + 8),
                            new Rectangle(352, 495, 12, 11),
                            Color.White,
                            0.0f,
                            Vector2.Zero,
                            4f,
                            SpriteEffects.None,
                            0f);

                        Utility.drawTextWithColoredShadow(b, this.ButtonTooltip, Game1.dialogueFont,
                            new Vector2(this.Component.bounds.X + 96,
                                this.Component.bounds.Y + 64), Color.WhiteSmoke,
                            new Color(Color.Black, 0.75f));
                    }
        }
    }
}
