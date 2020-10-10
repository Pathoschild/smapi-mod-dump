/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/demiacle/QualityOfLifeMods
**
*************************************************/

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
