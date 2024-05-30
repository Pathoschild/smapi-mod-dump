/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.GUI.DragNDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells
{
    public class SavedSpellPage
    {
        private SavedShapeGroupArea<SpellPartDraggable>[] spellShapes = new SavedShapeGroupArea<SpellPartDraggable>[5];

        private List<SpellPartDraggable> spellGrammerList = new List<SpellPartDraggable>();

        private string name { get; set; }

        public void SetSpellShapes(SavedShapeGroupArea<SpellPartDraggable>[] spellShapes)
        {
            this.spellShapes = spellShapes;
        }

        public SavedShapeGroupArea<SpellPartDraggable>[] GetSpellShapes()
        {
            return this.spellShapes;
        }

        public void SetSpellGrammerList(List<SpellPartDraggable> spellGrammerList)
        {
            this.spellGrammerList = spellGrammerList;
        }

        public List<SpellPartDraggable> GetSpellGrammerList()
        {
            return this.spellGrammerList;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
    }
}
