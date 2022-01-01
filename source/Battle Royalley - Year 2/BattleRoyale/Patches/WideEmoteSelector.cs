/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using BattleRoyale.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Linq;

namespace BattleRoyale.Patches
{
    class WideEmoteSelector : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(EmoteMenu), "_RepositionButtons");

        public static bool Prefix(EmoteSelector __instance)
        {
            if (!Game1.onScreenMenus.OfType<VictoryRoyale>().Any())
                return true;

            __instance.xPositionOnScreen = Game1.viewport.Width / 2 - __instance.width / 2;
            __instance.yPositionOnScreen = Game1.viewport.Height / 2 - __instance.height / 2 - 96;

            ModEntry.BRGame.Helper.Reflection.GetField<int>(__instance, "_expandedButtonRadius").SetValue(84);

            return true;
        }
    }

    class EmoteScrollLower : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(EmoteMenu), "draw");

        public static bool Prefix(EmoteSelector __instance, SpriteBatch b)
        {
            if (!Game1.onScreenMenus.OfType<VictoryRoyale>().Any())
                return true;


            float _alpha = ModEntry.BRGame.Helper.Reflection.GetField<float>(__instance, "_alpha").GetValue();
            List<ClickableTextureComponent> emoteButtons = ModEntry.BRGame.Helper.Reflection.GetField<List<ClickableTextureComponent>>(__instance, "_emoteButtons").GetValue();
            string selectedEmote = ModEntry.BRGame.Helper.Reflection.GetField<string>(__instance, "_selectedEmote").GetValue();
            int selectedIndex = ModEntry.BRGame.Helper.Reflection.GetField<int>(__instance, "_selectedIndex").GetValue();
            int selectedTime = ModEntry.BRGame.Helper.Reflection.GetField<int>(__instance, "_selectedTime").GetValue();
            Texture2D menuBackgroundTexture = ModEntry.BRGame.Helper.Reflection.GetField<Texture2D>(__instance, "menuBackgroundTexture").GetValue();
            bool gamepadMode = ModEntry.BRGame.Helper.Reflection.GetField<bool>(__instance, "gamepadMode").GetValue();

            bool drewName = true;

            Game1.StartWorldDrawInUI(b);
            Color background_color = Color.White;
            background_color.A = (byte)Utility.Lerp(0f, 255f, _alpha);
            foreach (ClickableTextureComponent emoteButton in emoteButtons)
            {
                emoteButton.draw(b, background_color, 0.86f);
            }
            if (selectedEmote != null)
            {
                Farmer.EmoteType[] eMOTES = Farmer.EMOTES;
                foreach (Farmer.EmoteType emote_type in eMOTES)
                {
                    if (emote_type.emoteString == selectedEmote)
                    {
                        SpriteText.drawStringWithScrollCenteredAt(b, emote_type.displayName, __instance.xPositionOnScreen + __instance.width / 2, __instance.yPositionOnScreen + __instance.height + 64);
                        drewName = true;
                        break;
                    }
                }
            }
            if (selectedIndex >= 0 && selectedTime >= 250)
            {
                Vector2 draw_position = Utility.PointToVector2(emoteButtons[selectedIndex].bounds.Center);
                draw_position.X += 16f;
                if (!gamepadMode)
                {
                    draw_position = Utility.PointToVector2(Game1.getMousePosition(ui_scale: false)) + new Vector2(32f, 32f);
                    b.Draw(menuBackgroundTexture, draw_position, new Rectangle(64, 0, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.99f);
                }
                else
                {
                    b.Draw(Game1.controllerMaps, draw_position, Utility.controllerMapSourceRect(new Rectangle(625, 260, 28, 28)), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.99f);
                }
                draw_position.X += 32f;
                b.Draw(menuBackgroundTexture, draw_position, new Rectangle(64, 16, 16, 16), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.99f);
            }
            Game1.EndWorldDrawInUI(b);

            return !drewName;
        }
    }
}
