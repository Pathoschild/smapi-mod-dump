/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using System.Collections.Generic;

namespace SpousesIsland.Framework
{
    public class ContentPackData
    {
    
        public string Spousename { get; set; }
        public string ArrivalPosition { get; set; }
        public string ArrivalDialogue { get; set; }
        public Location1Data Location1 { get; set; }
        public Location2Data Location2 { get; set; }
        public Location3Data Location3 { get; set; }
        public List<DialogueTranslation> Translations { get; set; } = new List<DialogueTranslation>();

        public ContentPackData()
        {
        }
        public ContentPackData(ContentPackData cpd)
        {
            Spousename = cpd.Spousename;
            ArrivalPosition = cpd.ArrivalPosition;
            ArrivalDialogue = cpd.ArrivalDialogue;

            Location1.Name = cpd.Location1.Name;
            Location1.Time = cpd.Location1.Time;
            Location1.Position = cpd.Location1.Position;
            Location1.Dialogue = cpd.Location1.Dialogue;

            Location2.Name = cpd.Location2.Name;
            Location2.Time = cpd.Location2.Time;
            Location2.Position = cpd.Location2.Position;
            Location2.Dialogue = cpd.Location2.Dialogue;

            Location3.Name = cpd.Location3.Name;
            Location3.Time = cpd.Location3.Time;
            Location3.Position = cpd.Location3.Position;
            Location3.Dialogue = cpd.Location3.Dialogue;
            
            Translations = cpd.Translations;
        }
    }
}
