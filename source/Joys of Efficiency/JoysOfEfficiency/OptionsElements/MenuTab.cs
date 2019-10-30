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
