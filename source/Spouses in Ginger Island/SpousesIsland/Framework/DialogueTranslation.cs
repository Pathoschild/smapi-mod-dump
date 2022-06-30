/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

namespace SpousesIsland.Framework
{
    public class DialogueTranslation
    {
        public string Key { get; set; }
        public string Arrival { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public string Location3 { get; set; }
        
        public DialogueTranslation()
        {
        }

        public DialogueTranslation(DialogueTranslation d)
        {
            Key = d.Key;
            Arrival = d.Arrival;
            Location1 = d.Location1;
            Location2 = d.Location2;
            Location3 = d.Location3;
        }
    }
}
