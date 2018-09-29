using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Demiacle.ImprovedQualityOfLife {
    public class ModData {

        // Todo refactor this into the working back end
        public SerializableDictionary<string, bool> boolOptions = new SerializableDictionary<string, bool>();
        public SerializableDictionary<string, int> intOptions = new SerializableDictionary<string, int>();

    }
}
