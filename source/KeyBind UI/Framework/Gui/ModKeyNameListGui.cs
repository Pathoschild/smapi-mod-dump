/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/KeyBindUI
**
*************************************************/

using System;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using KeyBindUI.Framework.Entity;
using KeyBindUI.Framework.Screen;

namespace KeyBindUI.Framework.Gui
{
    public class KeyBindListGui : ScreenGui
    {
        public KeyBindListGui(KeyBindListInfo keyBindListInfo) : base(keyBindListInfo.Name)
        {
            foreach (var keyValuePair in keyBindListInfo.KeyBindList)
            {
                AddElement(new Button(keyValuePair.Key,
                    ModEntry.GetInstance().Helper.Translation.Get("KeyBindUI.Framework.Screen.ModsGui.KeySetting"))
                {
                    OnLeftClicked = () =>
                    {
                        OpenScreenGui(
                            new ModKeyBindListGui(keyBindListInfo,
                                new Tuple<string, string>(keyValuePair.Key, keyValuePair.Value)));
                    }
                });
            }
        }
    }
}