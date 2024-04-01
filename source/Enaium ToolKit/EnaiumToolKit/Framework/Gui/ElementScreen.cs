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
using StardewModdingAPI;

namespace EnaiumToolKit.Framework.Gui;

internal class ElementScreen : ScreenGui
{
    public ElementScreen() : base("Element")
    {
        AddElement(new Button("Button", "It is Button")
        {
            OnLeftClicked = () => { ModEntry.GetInstance().Monitor.Log("Clicked", LogLevel.Debug); }
        });

        var toggleButton = new ToggleButton("ToggleButton", "It is Toggle");
        toggleButton.OnLeftClicked = () =>
        {
            ModEntry.GetInstance().Monitor.Log(toggleButton.Toggled + "", LogLevel.Debug);
        };
        AddElement(toggleButton);
        var modeButton = new ModeButton("ModeButton", "Left key plus right key minus")
        {
            modes = new List<string> { "Mode1", "Mode2", "Mode3", "Mode4" }, Current = "Mode1"
        };
        modeButton.OnLeftClicked = () => { ModEntry.GetInstance().Monitor.Log(modeButton.Current, LogLevel.Debug); };
        AddElement(modeButton);
        AddElementRange(new Label("Label1", "It is Label1"), new Label("Label2", "It is Label2"),
            new Label("Label3", "It is Label3"), new Label("Label4", "It is Label4"));
        AddElement(new ValueButton("ValueButton", "It is ValueButton")
        {
            Current = 1, Min = 1, Max = 10
        });
        AddElement(new Button("Colors", "Colors")
        {
            OnLeftClicked = () => { OpenScreenGui(new Colors()); }
        });
        AddElement(new SliderBar("Slider", "Slider", 0, 100));
    }

    private class Colors : ScreenGui
    {
        public Colors()
        {
            foreach (var variable in ColorUtils.Instance.Colors)
            {
                AddElement(new ColorButton(variable.Name.ToString(), "Color")
                {
                    Color = ColorUtils.Instance.Get(variable.Name)
                });
            }
        }
    }
}