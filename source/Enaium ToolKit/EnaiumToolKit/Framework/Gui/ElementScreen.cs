/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/EnaiumToolKit
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using EnaiumToolKit.Framework.Utils;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace EnaiumToolKit.Framework.Gui;

internal class ElementScreen : ScreenGui
{
    public ElementScreen() : base("Element")
    {
        AddElement(new ComboBox<Model>("ComboBox1", "ComboBox1")
        {
            Options = new List<Model>
            {
                new("Enaium", 18),
                new("Enaium1", 19),
                new("Enaium2", 20),
                new("Enaium3", 21)
            },
            OnCurrentChanged = value => { ModEntry.GetInstance().Monitor.Log(value.ToString(), LogLevel.Debug); }
        });
        AddElement(new ComboBox<Model>("ComboBox2", "ComboBox2")
        {
            Options = new List<Model>
            {
                new("Enaium", 18),
                new("Enaium1", 19),
            },
            OnCurrentChanged = value => { ModEntry.GetInstance().Monitor.Log(value.ToString(), LogLevel.Debug); }
        });
        AddElement(new Button("Button", "It is Button")
        {
            OnLeftClicked = () => { ModEntry.GetInstance().Monitor.Log("Clicked", LogLevel.Debug); }
        });
        var toggleButton = new ToggleButton("ToggleButton", "It is Toggle")
        {
            OnCurrentChanged = value => { ModEntry.GetInstance().Monitor.Log(value.ToString(), LogLevel.Debug); }
        };
        AddElement(toggleButton);
        AddElement(new ModeButton("ModeButton", "Left key plus right key minus")
        {
            Modes = new List<string> { "Mode1", "Mode2", "Mode3", "Mode4" }, Current = "Mode1",
            OnCurrentChanged = current => { ModEntry.GetInstance().Monitor.Log(current, LogLevel.Debug); }
        });
        AddElementRange(new Label("Label1", "It is Label1"), new Label("Label2", "It is Label2"),
            new Label("Label3", "It is Label3"), new Label("Label4", "It is Label4"));
        AddElement(new ValueButton("ValueButton", "It is ValueButton")
        {
            Current = 1, Min = 1, Max = 10,
            OnCurrentChanged = value => { ModEntry.GetInstance().Monitor.Log(value.ToString(), LogLevel.Debug); }
        });
        AddElement(new SliderBar("Slider1", "Slider1", 0, 100)
        {
            OnCurrentChanged = value => ModEntry.GetInstance().Monitor.Log(value.ToString(), LogLevel.Debug)
        });
        AddElement(new SliderBar("Slider2", "Slider2", 0, 100)
        {
            OnCurrentChanged = value => ModEntry.GetInstance().Monitor.Log(value.ToString(), LogLevel.Debug)
        });
        AddElement(new ColorPicker("Color Picker", "Color Picker", Color.White)
        {
            OnCurrentChanged = value => { ModEntry.GetInstance().Monitor.Log(value.ToString(), LogLevel.Debug); }
        });
        AddElement(new CheckBox("CheckBox", "CheckBox")
        {
            Current = true,
            OnCurrentChanged = value => { ModEntry.GetInstance().Monitor.Log(value.ToString(), LogLevel.Debug); }
        });
        AddElement(new SetButton("SetButton", "SetButton")
        {
            OnLeftClicked = () => { ModEntry.GetInstance().Monitor.Log("Clicked", LogLevel.Debug); }
        });
    }

    private record Model(string Name, int Age)
    {
        public override string ToString()
        {
            return $"{Name} {Age}";
        }
    }
}