/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/1Avalon/Avas-Stardew-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Network;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using static StardewValley.Menus.SocialPage;

namespace FriendshipStreaks
{
    public static class Patches
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            // Obtain the necessary property information for Rectangle::Bottom
            PropertyInfo bottomProperty = typeof(Rectangle).GetProperty("Bottom");
            MethodInfo bottomGetter = bottomProperty?.GetGetMethod(true);

            if (bottomGetter == null)
            {
                // Log a warning if the Bottom property is not found, and return the original instructions
                ModEntry.instance.Monitor.Log("Warning: Could not find the Bottom property on the Rectangle type.", StardewModdingAPI.LogLevel.Warn);
                return code;
            }

            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].opcode == OpCodes.Ldflda && code[i + 1].opcode == OpCodes.Ldfld && code[i + 2].opcode == OpCodes.Conv_R4 && code[i + 3].opcode == OpCodes.Newobj && code[i + 4].opcode == OpCodes.Ldsfld)
                {
                    FieldInfo yField = typeof(Rectangle).GetField("Y");
                    if (code[i + 1].operand == yField)
                    {
                        code.Insert(i + 2, new CodeInstruction(OpCodes.Ldc_I4, 64)); // Load constant 50
                        code.Insert(i + 3, new CodeInstruction(OpCodes.Add)); // Add 50 to the Y value
                    }
                }
            }
            return code;
        }

        public static void Postfix_drawNPCSlot(SocialPage __instance, SpriteBatch b, int i)
        {
            List<ClickableTextureComponent> sprites = ModEntry.instance.Helper.Reflection.GetField<List<ClickableTextureComponent>>(__instance, "sprites").GetValue();
            ClickableTextureComponent sprite = sprites[i];
            SocialEntry entry = __instance.GetSocialEntry(i);


            NPC npc = entry.Character as NPC;

            if (!ModEntry.streaks.TryGetValue(npc.Name, out FriendshipStreak streak))
                return;

            //Talking Streak
            Vector2 textPosition = new Vector2(sprite.bounds.Left + 60, sprite.bounds.Top - 10);
            float speechBubbleScale = 2.5f;
            string currentTalkingStreak = streak.CurrentTalkingStreak.ToString();
            b.Draw(ModEntry.gameCursors,new Vector2(textPosition.X + 40, textPosition.Y + 5), new Rectangle(66, 4, 14, 12), Color.White, 0f, Vector2.Zero, speechBubbleScale, SpriteEffects.None, 333f);
            b.DrawString(Game1.dialogueFont, currentTalkingStreak, textPosition + new Vector2(10f - Game1.dialogueFont.MeasureString(currentTalkingStreak).X / 2 * 0.8f, 0), Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);

            //Gift Streak
            textPosition = textPosition + new Vector2(100, 0);
            float giftIconScale = 2.5f;
            string currentGiftStreak = streak.CurrentGiftStreak.ToString();
            b.Draw(ModEntry.gameCursors, new Vector2(textPosition.X + 40, textPosition.Y + 5), new Rectangle(229, 410, 14, 14), Color.White, 0f, Vector2.Zero, giftIconScale, SpriteEffects.None, 333f);
            b.DrawString(Game1.dialogueFont, currentGiftStreak, textPosition + new Vector2(10f - Game1.dialogueFont.MeasureString(currentGiftStreak).X / 2 * 0.8f, 0), Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
        }

        public static void Postfix_receiveGift(NPC __instance)
        {
            if (!__instance.CanReceiveGifts())
                return;

            ModEntry.instance.Monitor.Log($"Adding +1 to {__instance.Name}'s gift streak");
            ModEntry.streaks[__instance.Name].UpdateGiftStreak();
        }

        public static bool Prefix_grantConversationFriendship(NPC __instance, Farmer who,  int amount)
        {
            if (!who.hasPlayerTalkedToNPC(__instance.Name) && ModEntry.streaks.ContainsKey(__instance.Name))
                ModEntry.streaks[__instance.Name].UpdateTalkingStreak();

            return true;
        }

        public static bool Prefix_changeFriendship(Farmer __instance, ref int amount, NPC n)
        {
            if (!ModEntry.streaks.TryGetValue(n.Name, out FriendshipStreak streak) || amount < 0 || !ModEntry.Config.enableBonus)
                return true;
            float bonus = streak.EvaluateFriendshipBonus();
            int _amount = amount + (int)(amount * bonus / 100);
            amount = _amount;
            return true;
        }

        public static void Postfix_drawProfileMenu(ProfileMenu __instance, SpriteBatch b)
        {
            //Gift
            Vector2 positionMaxGiftStreak = __instance.nextCharacterButton.getVector2() + new Vector2(15, -90);
            float giftIconScale = 2.5f;
            string characterName = __instance.Current.Character.Name;
            FriendshipStreak streak = ModEntry.streaks[characterName];
            streak.EvaluateFriendshipBonus();
            string highestStreak = streak.HighestGiftStreak.ToString();
            string multiplierText = I18n.Multiplier();
            string multiplier = $"+{streak.Multiplier}%";
            //b.DrawString(Game1.dialogueFont, multiplier, positionMaxGiftStreak + new Vector2(17 - SpriteText.getWidthOfString(multiplier) / 2, 170), Color.Black, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1f);
            b.DrawString(Game1.dialogueFont, I18n.Max(), positionMaxGiftStreak, Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
            if (ModEntry.Config.enableBonus)
            {
                b.DrawString(Game1.dialogueFont, multiplier, positionMaxGiftStreak + new Vector2(11f - Game1.dialogueFont.MeasureString(multiplier).X * 0.6f / 2, 170), Color.Black, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1f);
                b.DrawString(Game1.dialogueFont, multiplierText, positionMaxGiftStreak + new Vector2(70 - SpriteText.getWidthOfString(multiplierText) / 2, 140), Color.Black, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 1f);
            }
            b.DrawString(Game1.dialogueFont, highestStreak, positionMaxGiftStreak + new Vector2(20 - SpriteText.getWidthOfString(highestStreak) / 2 * 0.8f, 40), Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
            b.Draw(ModEntry.gameCursors, new Vector2(positionMaxGiftStreak.X - 40, positionMaxGiftStreak.Y), new Rectangle(229, 410, 14, 14), Color.White, 0f, Vector2.Zero, giftIconScale, SpriteEffects.None, 333f);

            //Bubble
            streak = ModEntry.streaks[characterName];
            Vector2 positionMaxTalkingStreak = __instance.previousCharacterButton.getVector2() + new Vector2(15, -90);
            float speechBubbleScale = 2.5f;
            highestStreak = streak.HighestTalkingStreak.ToString();
            b.DrawString(Game1.dialogueFont, I18n.Max(), positionMaxTalkingStreak, Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
            b.DrawString(Game1.dialogueFont, highestStreak, positionMaxTalkingStreak + new Vector2(15f - Game1.dialogueFont.MeasureString(highestStreak).X / 2 * 0.8f, 40), Color.Black, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1f);
            b.Draw(ModEntry.gameCursors, new Vector2(positionMaxTalkingStreak.X - 40, positionMaxTalkingStreak.Y), new Rectangle(66, 4, 14, 12), Color.White, 0f, Vector2.Zero, speechBubbleScale, SpriteEffects.None, 333f);
        }
    }
}
