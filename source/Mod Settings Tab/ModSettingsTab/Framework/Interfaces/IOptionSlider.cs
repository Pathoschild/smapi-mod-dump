/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/GilarF/SVM
**
*************************************************/

namespace ModSettingsTab.Framework.Interfaces
{
    public interface IOptionSlider : IModOption
    {
        int SliderValue { get; set; }

        int SliderMaxValue { get; set; }

        int SliderMinValue { get; set; }

        int SliderStep { get; set; }
    }
}