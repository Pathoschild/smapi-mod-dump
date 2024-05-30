/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/KeyBindUI
**
*************************************************/

using System.Reflection;
using EnaiumToolKit.Framework.Extensions;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace KeyBindUI.Framework.Gui;

public class KeyListScreen : GuiScreen
{
    private Slot<KeyBindSlot> _slot = null!;
    private Button _add = null!;
    private Button _remove = null!;
    private Button _back = null!;
    private Button _done = null!;
    private Button _confirm = null!;
    private Button _cancel = null!;

    private static readonly List<string> KeyList = new();

    private static bool _record;

    private readonly (string name, IMod instance) _mod;
    private readonly (PropertyInfo propertyInfo, object config) _info;

    private readonly List<string> _keyBinds = new();

    public KeyListScreen((string Name, IMod instance) mod, (PropertyInfo propertyInfo, object config) info)
    {
        _mod = mod;
        _info = info;
        var value = info.propertyInfo.GetValue(info.config);
        if (value != null)
        {
            if (_info.propertyInfo.PropertyType == typeof(KeybindList))
            {
                _keyBinds = ((KeybindList)value).Keybinds.Select(it => it.ToString()).ToList();
            }
            else if (_info.propertyInfo.PropertyType == typeof(SButton))
            {
                _keyBinds = new List<string> { ((SButton)value).ToString() };
            }
        }
    }

    protected override void Init()
    {
        var w = (int)(Game1.graphics.GraphicsDevice.Viewport.Width / 1.5);
        var keyHeight = (int)Game1.dialogueFont.MeasureString("A").Y * 2;
        var h = Game1.graphics.GraphicsDevice.Viewport.Height / 2 / keyHeight * keyHeight;
        var centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(w, h);
        _slot = new Slot<KeyBindSlot>("", "", (int)centeringOnScreen.X, (int)centeringOnScreen.Y, w, h,
            keyHeight);
        foreach (var key in _keyBinds)
        {
            _slot.AddEntry(new KeyBindSlot(key));
        }

        var addTitle = ModEntry.GetInstance().Helper.Translation
            .Get("KeyBindUI.Framework.Screen.ModKeyBindListGui.add");
        _add = new Button(addTitle, "", _slot.Bounds.X, _slot.Bounds.Bottom + 20,
            (int)Game1.dialogueFont.MeasureString(addTitle).X + 20, 80)
        {
            OnLeftClicked = () => { _record = true; }
        };
        var removeTitle = ModEntry.GetInstance().Helper.Translation
            .Get("KeyBindUI.Framework.Screen.ModKeyBindListGui.remove");
        _remove = new Button(removeTitle, "", _add.Bounds.Right, _add.Bounds.Y,
            (int)Game1.dialogueFont.MeasureString(removeTitle).X + 20, 80)
        {
            OnLeftClicked = () =>
            {
                if (_slot.SelectedEntry == null) return;
                _keyBinds.Remove(_slot.SelectedEntry.KeyBind);
                _slot.RemoveEntry(_slot.SelectedEntry);
                _slot.SelectedEntry = null;
            }
        };
        var confirmTitle = ModEntry.GetInstance().Helper.Translation
            .Get("KeyBindUI.Framework.Screen.ModKeyBindListGui.confirm");
        _confirm = new Button(confirmTitle, "", _remove.Bounds.Right, _add.Bounds.Y,
            (int)Game1.dialogueFont.MeasureString(confirmTitle).X + 20, 80)
        {
            OnLeftClicked = () =>
            {
                Confirm();
                Game1.playSound("coin");
                if (PreviousMenu != null)
                {
                    OpenScreenGui(PreviousMenu);
                }
            }
        };
        var backTitle = ModEntry.GetInstance().Helper.Translation
            .Get("KeyBindUI.Framework.Screen.ModKeyBindListGui.back");
        _back = new Button(backTitle, "", _remove.Bounds.Right, _add.Bounds.Y,
            (int)Game1.dialogueFont.MeasureString(backTitle).X + 20, 80)
        {
            OnLeftClicked = () =>
            {
                if (PreviousMenu != null)
                {
                    OpenScreenGui(PreviousMenu);
                }
            }
        };

        var x = Game1.uiViewport.Width / 2 - 100;
        var y = Game1.uiViewport.Height / 2 - 35;

        _done = new Button(
            ModEntry.GetInstance().Helper.Translation.Get("KeyBindUI.Framework.Screen.ModKeyBindListGui.done"), "",
            x,
            y + 100, 200, 80)
        {
            OnLeftClicked = () =>
            {
                var k = string.Join(" + ", KeyList);
                _keyBinds.Add(k);
                _slot.AddEntry(new KeyBindSlot(k));
                _record = false;
                KeyList.Clear();
                Game1.playSound("coin");
            }
        };

        _cancel = new Button(
            ModEntry.GetInstance().Helper.Translation.Get("KeyBindUI.Framework.Screen.ModKeyBindListGui.cancel"),
            "",
            x,
            y + 200, 200, 80)
        {
            OnLeftClicked = () =>
            {
                _record = false;
                KeyList.Clear();
                Game1.playSound("bigDeSelect");
            }
        };
        AddComponentRange(_slot, _add, _remove, _back, _done, _confirm, _cancel);
    }

    private void Confirm()
    {
        var k = string.Join(" + ", _keyBinds);
        if (_info.propertyInfo.PropertyType == typeof(KeybindList))
        {
            _info.propertyInfo.SetValue(_info.config,
                new KeybindList(_keyBinds.Select(it => KeybindList.Parse(it).Keybinds[0]).ToArray()));
        }
        else if (_info.propertyInfo.PropertyType == typeof(SButton))
        {
            _info.propertyInfo.SetValue(_info.config, Enum.Parse<SButton>(k));
        }

        _mod.instance.Helper.WriteConfig(_info.config);
    }

    public override void draw(SpriteBatch b)
    {
        var background =
            new Rectangle(_slot.X - 10, _slot.Y - 10, _slot.Width + 20,
                _slot.Height + 20);
        b.DrawWindowTexture(background, Color.White);

        _add.X = _slot.Bounds.X;
        _remove.X = _add.Visibled ? _add.Bounds.Right + 120 : _add.Bounds.X;
        _confirm.X = _remove.Visibled ? _remove.Bounds.Right + 120 : _remove.Bounds.X;
        _back.X = _confirm.Visibled ? _confirm.Bounds.Right + 120 : _confirm.Bounds.X;

        _add.Visibled = !_record && (_info.propertyInfo.PropertyType == typeof(KeybindList) || !_keyBinds.Any());
        _remove.Visibled = _slot.SelectedEntry != null && !_record;

        _done.Visibled = _record && KeyList.Any();
        _cancel.Visibled = _record;
        base.draw(b);
        if (_record)
        {
            b.DrawStringCenter(KeyList.Count == 0
                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:OptionsElement.cs.11225")
                    : string.Join(" + ", KeyList.ToArray()),
                new Rectangle(0, 0, Game1.graphics.GraphicsDevice.Viewport.Width,
                    Game1.graphics.GraphicsDevice.Viewport.Height), color: Color.White);
        }
    }

    public override void receiveKeyPress(Keys key)
    {
        if (_record)
        {
            if (!KeyList.Contains(key.ToString()))
            {
                if (_info.propertyInfo.PropertyType == typeof(KeybindList))
                {
                    KeyList.Add(key.ToString());
                }
                else if (_info.propertyInfo.PropertyType == typeof(SButton))
                {
                    KeyList.Clear();
                    KeyList.Add(key.ToString());
                }
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
            Hovered = new Rectangle(x, y, Width, Height).Contains(Game1.getMouseX(), Game1.getMouseY());
            b.DrawStringCenter(KeyBind, x, y, Width, Height);
        }
    }
}