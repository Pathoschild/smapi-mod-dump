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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells
{
    public class SpellPage
    {
        public Spell spell;

        //private List<ShapeGroupArea> spellShapes;
        private ShapeGroupArea[] spellShapes;

        private List<SpellPartDraggable> spellGrammerList = new List<SpellPartDraggable>();

        private string name { get; set; }

        public SpellPage() 
        {
            //spellShapes = new List<ShapeGroupArea>();

            spellShapes = new ShapeGroupArea[5];

            for (int i = 0; i < spellShapes.Length; i++)
            {
                //spellShapes.Add(new ShapeGroupArea(0, 0, null, ""));

                spellShapes[i] = new ShapeGroupArea(0, 0, null, "");
            }
        }

        public void SetSpellShapes(ShapeGroupArea[] spellShapes)
        {
            this.spellShapes = spellShapes;
        }

        public ShapeGroupArea[] GetSpellShapeAreas()
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
