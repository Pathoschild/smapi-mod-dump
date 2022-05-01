/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/SimpleHUD
**
*************************************************/

using System.Collections.Generic;
using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SimpleHUD.Framework;
using SimpleHUD.Framework.Gui;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace SimpleHUD
{
    public class ModEntry : Mod
    {
        public static Config Config;
        private static ModEntry _instance;

        public ModEntry()
        {
            _instance = this;
        }

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<Config>();
            Toolbar toolbar = null;

            helper.Events.Input.ButtonPressed += (sender, args) =>
            {
                if (args.Button == Config.OpenSetting)
                {
                    Game1.activeClickableMenu = new SettingGui();
                }
            };

            helper.Events.GameLoop.UpdateTicking += (sender, args) =>
            {
                if (toolbar == null)
                {
                    foreach (var onScreenMenu in Game1.onScreenMenus)
                    {
                        if (onScreenMenu is Toolbar menu)
                        {
                            toolbar = menu;
                        }
                    }
                }

                toolbar?.performHoverAction(Game1.getMouseX(), Game1.getMouseY());
            };

            helper.Events.Display.Rendered += (sender, args) =>
            {
                Game1.displayHUD = !Config.Enable;

                if (!Config.Enable)
                    return;
                if (!Context.IsWorldReady)
                    return;
                if (!Context.IsPlayerFree)
                    return;
                if (Game1.isWarping)
                    return;
                if (Game1.activeClickableMenu != null)
                    return;

                toolbar?.draw(args.SpriteBatch);

                Game1.game1.drawMouseCursor();


                var list = new List<string>();

                list.Add(Config.Title);
                list.Add("");
                list.Add($"Location:{Game1.currentLocation.Name}");
                list.Add(
                    $"TileX:{Game1.player.getTileX()}/TileY:{Game1.player.getTileY()}/StandingX:{Game1.player.getStandingX()}/StandingY:{Game1.player.getStandingY()}");
                list.Add($"Season:{Game1.currentSeason}");
                list.Add($"Date:{Game1.shortDayDisplayNameFromDayOfSeason(Game1.dayOfMonth)}({Game1.dayOfMonth})");
                list.Add($"Time:{Game1.getTimeOfDayString(Game1.timeOfDay)}");
                list.Add($"Weather:{Game1.weatherIcon}");
                list.Add($"Money:{Game1.player.Money}");
                list.Add($"ClubCoin:{Game1.player.clubCoins}");
                list.Add($"QiGems:{Game1.player.QiGems}");
                list.Add($"GoldPiece:{Game1.player.goldPieces}");
                list.Add($"Health:{Game1.player.health}/{Game1.player.maxHealth}");
                list.Add($"Strength:{Game1.player.stamina}/{Game1.player.maxStamina}");
                list.Add($"Item:{(Game1.player.CurrentItem == null ? "Blank" : Game1.player.CurrentItem.Name)}");
                var index = 0;
                foreach (var s in list)
                {
                    args.SpriteBatch.DrawString(Game1.dialogueFont, s, new Vector2(0, index), ColorUtils.Instance.Get(Config.TextColor));
                    index += 35;
                }
            };
        }

        public static ModEntry GetInstance()
        {
            return _instance;
        }
    }
}