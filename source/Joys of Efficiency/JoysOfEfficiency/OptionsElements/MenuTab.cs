/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pomepome/JoysOfEfficiency
**
*************************************************/

using System.Collections.Generic;
using StardewValley.Menus;

namespace JoysOfEfficiency.OptionsElements
{
    internal class MenuTab
    {
        private readonly List<OptionsElement> _optionsElements = new List<OptionsElement>();

        public void AddOptionsElement(OptionsElement element)
        {
            _optionsElements.Add(element);
        }

        public List<OptionsElement> GetElements()
        {
            return new List<OptionsElement>(_optionsElements);
        }
    }
}
