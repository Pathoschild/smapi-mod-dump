/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Windmill-City/InputFix
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace InputFix
{
    #region KeyboardDispatcher

    [HarmonyPatch(typeof(KeyboardDispatcher))]
    [HarmonyPatch("Subscriber", MethodType.Setter)]
    internal class PatchSubScriber
    {
        private static void Postfix()
        {
            //Dont change except mainthread
            if (ModEntry.isMainThread() && !Compatibility.ignore && KeyboardInput_.iMEControl != null)
                if ((Game1.keyboardDispatcher.Subscriber is ITextBox && (Game1.keyboardDispatcher.Subscriber as ITextBox).AllowIME)
                    || (Game1.keyboardDispatcher.Subscriber is TextBox && !(Game1.keyboardDispatcher.Subscriber as TextBox).numbersOnly))
                {
                    KeyboardInput_.iMEControl.EnableIME();
                }
                else
                {
                    KeyboardInput_.iMEControl.DisableIME();
                }
        }
    }

    #endregion KeyboardDispatcher

    #region ChatBox

    [HarmonyPatch(typeof(ChatBox), MethodType.Constructor)]
    internal class PatchChatBox_ctor
    {
        private static void Postfix(ChatBox __instance)
        {
            //replace ChatTextBox
            Texture2D texture2D = Game1.content.Load<Texture2D>("LooseSprites\\chatBox");
            ChatTextBox_ chatTextBox_ = new ChatTextBox_(texture2D, null, Game1.smallFont, Color.White);
            chatTextBox_.OnEnterPressed += new TextBoxEvent(__instance.textBoxEnter);
            chatTextBox_.X = __instance.chatBox.X;
            chatTextBox_.Y = __instance.chatBox.Y;
            chatTextBox_.Width = __instance.chatBox.Width;
            chatTextBox_.Height = __instance.chatBox.Height;
            __instance.chatBox = chatTextBox_;
        }
    }

    #endregion ChatBox

    #region AnimalQueryMenu

    [HarmonyPatch(typeof(AnimalQueryMenu), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(FarmAnimal) })]
    internal class PatchAnimalQueryMenu_ctor
    {
        private static void Postfix(AnimalQueryMenu __instance, ref TextBox ___textBox)
        {
            //replace TextBox
            TextBox_ textBox_ = new TextBox_(null, null, Game1.dialogueFont, Game1.textColor);
            textBox_.X = ___textBox.X;
            textBox_.Y = ___textBox.Y;
            textBox_.Width = ___textBox.Width;
            textBox_.Height = ___textBox.Height;
            textBox_.SetText(___textBox.Text);
            ___textBox = textBox_;
        }
    }

    #endregion AnimalQueryMenu

    #region NamingMenu

    [HarmonyPatch(typeof(NamingMenu), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(NamingMenu.doneNamingBehavior), typeof(string), typeof(string) })]
    internal class PatchNamingMenu_ctor
    {
        private static void Postfix(NamingMenu __instance, ref TextBox ___textBox)
        {
            //replace TextBox
            TextBox_ textBox_ = new TextBox_(null, null, Game1.dialogueFont, Game1.textColor);
            textBox_.OnEnterPressed += new TextBoxEvent(__instance.textBoxEnter);
            textBox_.X = ___textBox.X;
            textBox_.Y = ___textBox.Y;
            textBox_.Width = ___textBox.Width;
            textBox_.Height = ___textBox.Height;
            textBox_.SetText(___textBox.Text);
            ___textBox = textBox_;
        }
    }

    #endregion NamingMenu

    #region NumberSelectionMenu

    [HarmonyPatch(typeof(NumberSelectionMenu), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(string),
        typeof(NumberSelectionMenu.behaviorOnNumberSelect),
        typeof(int),
        typeof(int),
        typeof(int),
        typeof(int) })]
    internal class PatchNumberSelectionMenu_ctor
    {
        private static void Postfix(NumberSelectionMenu __instance, ref TextBox ___numberSelectedBox)
        {
            //replace TextBox
            TextBox_ textBox_ = new TextBox_(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor);
            textBox_.X = ___numberSelectedBox.X;
            textBox_.Y = ___numberSelectedBox.Y;
            textBox_.Width = ___numberSelectedBox.Width;
            textBox_.Height = ___numberSelectedBox.Height;
            textBox_.numbersOnly = true;
            textBox_.textLimit = ___numberSelectedBox.textLimit;
            textBox_.SetText(___numberSelectedBox.Text);
            ___numberSelectedBox = textBox_;
        }
    }

    #endregion NumberSelectionMenu

    #region PurchaseAnimalsMenu

    [HarmonyPatch(typeof(PurchaseAnimalsMenu), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(List<StardewValley.Object>) })]
    internal class PatchPurchaseAnimalsMenu_ctor
    {
        private static void Postfix(PurchaseAnimalsMenu __instance, ref TextBox ___textBox)
        {
            //replace TextBox
            TextBox_ textBox_ = new TextBox_(null, null, Game1.dialogueFont, Game1.textColor);
            textBox_.OnEnterPressed += new TextBoxEvent(__instance.textBoxEnter);
            textBox_.X = ___textBox.X;
            textBox_.Y = ___textBox.Y;
            textBox_.Width = ___textBox.Width;
            textBox_.Height = ___textBox.Height;
            textBox_.SetText(___textBox.Text);
            ___textBox = textBox_;
        }
    }

    #endregion PurchaseAnimalsMenu

    #region CharacterCustomization

    [HarmonyPatch(typeof(CharacterCustomization), "setUpPositions")]
    internal class PatchCharacterCustomization_setUpPositions
    {
        private static void Postfix(ref TextBox ___nameBox, ref TextBox ___farmnameBox, ref TextBox ___favThingBox)
        {
            //replace TextBox
            TextBox_ textbox_1 = new TextBox_(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor);
            TextBox_ textbox_2 = new TextBox_(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor);
            TextBox_ textbox_3 = new TextBox_(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont, Game1.textColor);
            //nameBox
            textbox_1.X = ___nameBox.X;
            textbox_1.Y = ___nameBox.Y;
            textbox_1.Width = ___nameBox.Width;
            textbox_1.Height = ___nameBox.Height;
            textbox_1.SetText(___nameBox.Text);
            ___nameBox = textbox_1;
            //farm name
            textbox_2.X = ___farmnameBox.X;
            textbox_2.Y = ___farmnameBox.Y;
            textbox_2.Width = ___farmnameBox.Width;
            textbox_2.Height = ___farmnameBox.Height;
            textbox_2.SetText(___farmnameBox.Text);
            ___farmnameBox = textbox_2;
            //fav
            textbox_3.X = ___favThingBox.X;
            textbox_3.Y = ___favThingBox.Y;
            textbox_3.Width = ___favThingBox.Width;
            textbox_3.Height = ___favThingBox.Height;
            textbox_3.SetText(___favThingBox.Text);
            ___favThingBox = textbox_3;
        }
    }

    #endregion CharacterCustomization

    #region ChatTextBox

    [HarmonyPatch(typeof(ChatTextBox))]
    [HarmonyPatch("reset")]
    internal class PatchChatBox_reset
    {
        private static bool Prefix(ChatTextBox __instance)
        {
            if (__instance is ChatTextBox_)
            {
                (__instance as ChatTextBox_).reset();
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ChatTextBox))]
    [HarmonyPatch("setText")]
    internal class PatchChatBox_setText
    {
        private static bool Prefix(ChatTextBox __instance, string text)
        {
            if (__instance is ChatTextBox_)
            {
                (__instance as ChatTextBox_).setText(text);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(ChatTextBox))]
    [HarmonyPatch("receiveEmoji")]
    internal class PatchChatBox_receiveEmoji
    {
        private static bool Prefix(ChatTextBox __instance, int emoji)
        {
            if (__instance is ChatTextBox_)
            {
                (__instance as ChatTextBox_).receiveEmoji(emoji);
                return false;
            }
            return true;
        }
    }

    #endregion ChatTextBox

    #region TextBox

    [HarmonyPatch(typeof(TextBox))]
    [HarmonyPatch("RecieveSpecialInput")]
    internal class Patch_TextBox_RecieveSpecialInput
    {
        private static bool Prefix(TextBox __instance, Keys key)
        {
            if (__instance is ChatTextBox_)
            {
                (__instance as ChatTextBox_).RecieveSpecialInput(key);
                return false;
            }
            else if (__instance is TextBox_)
            {
                (__instance as TextBox_).RecieveSpecialInput(key);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TextBox))]
    [HarmonyPatch("Text", MethodType.Setter)]
    internal class Patch_set_Text
    {
        private static void Postfix(TextBox __instance, string value)
        {
            if (__instance is ITextBox)
            {
                (__instance as ITextBox).SetText(value);
            }
        }
    }

    [HarmonyPatch(typeof(TextBox))]
    [HarmonyPatch("Text", MethodType.Getter)]
    internal class Patch_get_Text
    {
        private static void Postfix(TextBox __instance, ref string __result)
        {
            if (__instance is ITextBox)
            {
                __result = (__instance as ITextBox).GetText();
            }
        }
    }

    [HarmonyPatch(typeof(TextBox))]
    [HarmonyPatch("X", MethodType.Setter)]
    internal class Patch_set_X
    {
        private static void Postfix(TextBox __instance, int value)
        {
            if (__instance is ChatTextBox_)
            {
                (__instance as ChatTextBox_).X = value;
            }
            else if (__instance is TextBox_)
            {
                (__instance as TextBox_).X = value;
            }
        }
    }

    [HarmonyPatch(typeof(TextBox))]
    [HarmonyPatch("Y", MethodType.Setter)]
    internal class Patch_set_Y
    {
        private static void Postfix(TextBox __instance, int value)
        {
            if (__instance is ChatTextBox_)
            {
                (__instance as ChatTextBox_).Y = value;
            }
            else if (__instance is TextBox_)
            {
                (__instance as TextBox_).Y = value;
            }
        }
    }

    #endregion TextBox
}