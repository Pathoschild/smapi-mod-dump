using System.Collections.Generic;
using StardewValley.Menus;

namespace JoysOfEfficiency.OptionsElements
{
    internal class MenuTab
    {
        public int Count => _optionsElements.Count;

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
