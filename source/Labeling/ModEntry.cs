/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/Labeling
**
*************************************************/

using System;
using System.Collections.Generic;
using EnaiumToolKit.Framework.Utils;
using Labeling.Framework;
using Labeling.Framework.Gui;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace Labeling
{
    public class ModEntry : Mod
    {
        private static ModEntry _instance;
        public Config Config;

        public Vector2 FirstObjectTile = Vector2.Zero;
        public Vector2 SecondObjectTile = Vector2.Zero;

        public ModEntry()
        {
            _instance = this;
        }

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<Config>();
            helper.Events.Input.ButtonReleased += OnButtonReleased;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Display.Rendered += OnRender;
        }


        private void OnRender(object sender, RenderedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (!Context.IsPlayerFree)
                return;
            if (Game1.isWarping)
                return;
            if (Game1.activeClickableMenu != null)
                return;


            foreach (var variable in Config.Labelings)
            {
                var firstLabelingPosition = GetLocalPosition(Game1.viewport, variable.FirstObjectTile);
                var secondLabelingPosition = GetLocalPosition(Game1.viewport, variable.SecondObjectTile);
                var color = ColorUtils.Instance.Get(variable.Color);
                color.A = 1;
                if (variable.Display && variable.CurrentGameLocation.Equals(Game1.player.currentLocation.Name))
                {
                    e.SpriteBatch.Draw(Game1.staminaRect, new Rectangle((int) firstLabelingPosition.X,
                            (int) firstLabelingPosition.Y,
                            (int) (secondLabelingPosition.X - firstLabelingPosition.X) + Game1.tileSize,
                            (int) (secondLabelingPosition.Y - firstLabelingPosition.Y) + Game1.tileSize),
                        color);
                    FontUtils.Draw(e.SpriteBatch, variable.Name, (int) firstLabelingPosition.X,
                        (int) firstLabelingPosition.Y);
                }
            }
            
            if (FirstObjectTile.Equals(Vector2.Zero) || SecondObjectTile.Equals(Vector2.Zero)) return;

            var firstObjectPosition = GetLocalPosition(Game1.viewport, FirstObjectTile);
            var secondObjectPosition = GetLocalPosition(Game1.viewport, SecondObjectTile);

            if (firstObjectPosition.X > secondObjectPosition.X)
            {
                var pervFirstObjectTile = FirstObjectTile;
                FirstObjectTile = SecondObjectTile;
                SecondObjectTile = pervFirstObjectTile;
            }

            if (firstObjectPosition.Y > secondObjectPosition.Y)
            {
                FirstObjectTile = Vector2.Zero;
                SecondObjectTile = Vector2.Zero;
            }

            e.SpriteBatch.Draw(Game1.staminaRect, new Rectangle((int) firstObjectPosition.X,
                (int) firstObjectPosition.Y,
                (int) (secondObjectPosition.X - firstObjectPosition.X) + Game1.tileSize,
                (int) (secondObjectPosition.Y - firstObjectPosition.Y) + Game1.tileSize), new Color(0, 255, 0, 1));
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (!Context.IsPlayerFree)
                return;

            if (e.Button == Config.OpenLabelScreen)
            {
                Game1.activeClickableMenu = new LabelGui();
            }
        }

        private void OnButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;
            if (!Context.IsPlayerFree)
                return;

            var toolLocationVector = Game1.currentCursorTile;

            if (Game1.player.CurrentItem != null && Game1.player.CurrentItem.Name.Equals("Wood"))
            {
                foreach (var variable in Game1.options.useToolButton)
                {
                    if (e.Button == variable.ToSButton())
                    {
                        FirstObjectTile = new Vector2((int) toolLocationVector.X, (int) toolLocationVector.Y);
                        Game1.addHUDMessage(new HUDMessage(GetTranslation("labeling.message.firstPos"), 4));
                    }
                }

                foreach (var variable in Game1.options.actionButton)
                {
                    if (e.Button == variable.ToSButton())
                    {
                        SecondObjectTile = new Vector2((int) toolLocationVector.X, (int) toolLocationVector.Y);
                        Game1.addHUDMessage(new HUDMessage(GetTranslation("labeling.message.secondPos"), 4));
                    }
                }
            }
        }

        public string GetTranslation(string key)
        {
            return GetInstance().Helper.Translation.Get(key);
        }

        public void ConfigReload()
        {
            GetInstance().Helper.WriteConfig(Config);
            Config = GetInstance().Helper.ReadConfig<Config>();
        }

        private Vector2 GetLocalPosition(xTile.Dimensions.Rectangle viewport, Vector2 vector2) => new Vector2(
            vector2.X * 64f - viewport.X, vector2.Y * 64f - viewport.Y);

        public static ModEntry GetInstance()
        {
            return _instance;
        }
    }
}