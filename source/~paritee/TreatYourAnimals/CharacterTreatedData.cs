using System;
using System.Collections.Generic;

namespace TreatYourAnimals
{
    class CharacterTreatedData
    {
        public List<string> Characters = new List<string>();

        public string FormatEntry(string type, string id)
        {
            return String.Join("_", new string[] { type, id });
        }
    }
}
