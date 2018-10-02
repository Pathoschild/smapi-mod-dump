using System;
using System.Xml.Serialization;

namespace UiModSuite {
    public class ModData {

        // Dictionary of int => bool is used because OptionsCheckbox uses int as a stored value.
        // The int used here is actually a hash value of the npc name string
        public SerializableDictionary<int, bool> locationOfTownsfolkOptions = new SerializableDictionary<int, bool>();


        public SerializableDictionary<int, int> intSettings = new SerializableDictionary<int, int>();
        public SerializableDictionary<int, bool> boolSettings = new SerializableDictionary<int, bool>();

        // Currently unused
        public SerializableDictionary<int, string> stringSettings = new SerializableDictionary<int, string>();
    }
}
