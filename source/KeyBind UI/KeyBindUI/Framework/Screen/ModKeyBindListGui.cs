/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/KeyBindUI
**
*************************************************/

using System.Text;
using System.Text.RegularExpressions;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Components;
using EnaiumToolKit.Framework.Utils;
using KeyBindUI.Framework.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewValley;

namespace KeyBindUI.Framework.Screen;

public class ModKeyBindListGui : GuiScreen
{
    private readonly Slot<KeyBindSlot> _slot;

    private readonly Button _add;
    private readonly Button _remove;

    private static readonly List<string> KeyList = new();


    private static readonly int X = Game1.uiViewport.Width / 2 - 100;
    private static readonly int Y = Game1.uiViewport.Height / 2 - 35;

    private static bool _record;

    private readonly Button _done;

    private readonly Button _cancel;

    public ModKeyBindListGui(KeyBindListInfo keyBindListInfo, Tuple<string, string> keyBind)
    {
        var keyBinds = Regex.Split(keyBind.Item2, ", ").ToList();
        keyBinds.Sort((s1, s2) => FontUtils.GetWidth(s2) - FontUtils.GetWidth(s1));
        var w = FontUtils.GetWidth(keyBinds[0]) + 200;
        _slot = new Slot<KeyBindSlot>("", "", Game1.viewport.Width / 2 - w / 2, 50, w,
            (Game1.viewport.Height - 400) / 80 * 80, 80);
        foreach (var key in keyBinds)
        {
            _slot.AddEntry(new KeyBindSlot(key));
        }


        _add = new Button(
            ModEntry.GetInstance().Helper.Translation.Get("KeyBindUI.Framework.Screen.ModKeyBindListGui.add"), "",
            _slot.X, _slot.Height + 100, _slot.Width, 80)
        {
            OnLeftClicked = () => { _record = true; }
        };
        _remove = new Button(
            ModEntry.GetInstance().Helper.Translation.Get("KeyBindUI.Framework.Screen.ModKeyBindListGui.remove"),
            "",
            _slot.X, _slot.Height + 200, _slot.Width, 80)
        {
            OnLeftClicked = () =>
            {
                if (_slot.SelectedEntry == null) return;
                keyBinds.Remove(_slot.SelectedEntry.KeyBind);
                _slot.RemoveEntry(_slot.SelectedEntry);
                _slot.SelectedEntry = null;

                SaveConfig(keyBindListInfo, keyBind, keyBinds);
            }
        };
        AddComponent(_slot);
        AddComponentRange(_add, _remove);

        _done = new Button(
            ModEntry.GetInstance().Helper.Translation.Get("KeyBindUI.Framework.Screen.ModKeyBindListGui.done"), "",
            X,
            Y + 100, 200, 80)
        {
            OnLeftClicked = () =>
            {
                var k = string.Join(" + ", KeyList);
                keyBinds.Add(k);
                _slot.AddEntry(new KeyBindSlot(k));

                SaveConfig(keyBindListInfo, keyBind, keyBinds);

                _record = false;
                KeyList.Clear();
                Game1.playSound("coin");
            }
        };

        _cancel = new Button(
            ModEntry.GetInstance().Helper.Translation.Get("KeyBindUI.Framework.Screen.ModKeyBindListGui.cancel"),
            "",
            X,
            Y + 200, 200, 80)
        {
            OnLeftClicked = () =>
            {
                _record = false;
                KeyList.Clear();
                Game1.playSound("bigDeSelect");
            }
        };

        AddComponent(_done);
        AddComponent(_cancel);
    }

    private static void SaveConfig(KeyBindListInfo keyBindListInfo, Tuple<string, string> keyBind,
        IEnumerable<string> keyBinds)
    {
        keyBindListInfo.KeyBindList[keyBind.Item1] = string.Join(", ", keyBinds);

        var read = new JsonTextReader(keyBindListInfo.ConfigFileInfo.OpenText());
        var readFrom = (JObject)JToken.ReadFrom(read);
        read.Close();

        foreach (var keyValuePair in keyBindListInfo.KeyBindList)
        {
            readFrom[keyValuePair.Key] = keyValuePair.Value;
        }

        System.IO.File.WriteAllText(keyBindListInfo.ConfigFileInfo.FullName, readFrom.ToString(),
            Encoding.UTF8);
    }

    public override void draw(SpriteBatch b)
    {
        var background = new Rectangle(_slot.X - 13, _slot.Y - 10, _slot.Width + 15,
            _slot.Height + 20);
        Render2DUtils.DrawBound(b, background.X, background.Y, background.Width, background.Height, Color.White);

        _remove.Visibled = _slot.SelectedEntry != null && !_record;

        _add.Visibled = !_record;
        _done.Visibled = _record;
        _cancel.Visibled = _record;

        base.draw(b);

        if (_record)
        {
            b.DrawString(Game1.dialogueFont,
                KeyList.Count == 0
                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsElement.cs.11225")
                    : string.Join(" + ", KeyList.ToArray()),
                Utility.getTopLeftPositionForCenteringOnScreen(192, 64), Color.White, 0.0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0.9999f);
        }
    }


    public override void receiveKeyPress(Keys key)
    {
        if (_record)
        {
            if (!KeyList.Contains(key.ToString()))
            {
                KeyList.Add(key.ToString());
            }
        }

        base.receiveKeyPress(key);
    }

    private class KeyBindSlot : Slot<KeyBindSlot>.Entry
    {
        public readonly string KeyBind;

        public KeyBindSlot(string keyBind)
        {
            KeyBind = keyBind;
        }

        public override void Render(SpriteBatch b, int x, int y)
        {
            Hovered = Render2DUtils.IsHovered(Game1.getMouseX(), Game1.getMouseY(), x, y, Width, Height);
            FontUtils.DrawHvCentered(b, KeyBind, x + Width / 2, y + Height / 2);
        }
    }
}