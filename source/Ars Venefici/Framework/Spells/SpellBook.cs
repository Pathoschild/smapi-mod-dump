/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using ArsVenefici.Framework.Interfaces.Spells;
using ArsVenefici.Framework.Interfaces;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using ArsVenefici.Framework.Util;
using xTile;
using ArsVenefici.Framework.GUI.DragNDrop;

namespace ArsVenefici.Framework.Spells
{
    public class SpellBook
    {
        /// <summary>The underlying player.</summary>
        public Farmer Player => this.Data.Player;

        /// <summary>The player's underlying data.</summary>
        private readonly SpellBookData Data;

        /// <summary>The list of spell pages for a player's spell book.</summary>
        private List<SpellPage> pages;

        /// <summary>The current spell page.</summary>
        private SpellPage currentSpellPage;

        /// <summary>The current spell page as an index.</summary>
        private int currentPageIndex;

        int maxSpellPages = 10;

        /// <summary>The list of spells for a player's spell book.</summary>
        private List<ISpell> spells;

        /// <summary>The current spell.</summary>
        private ISpell currentSpell;

        /// <summary>The current spell as an index.</summary>
        private int currentSpellIndex;

        /// <summary>The reduction amount to appy to the total mana cost. Will be divided by the mana cost and the result will be subtracted from the cost</summary>
        private int manaCostReductionAmount = 1;

        public SpellBook(Farmer player)
        {
            this.Data = new SpellBookData(player);

            pages = new List<SpellPage>();
            spells = new List<ISpell>();

            for (int i = 0; i < maxSpellPages; i++)
            {
                pages.Add(new SpellPage());
            }

            for (int i = 0; i < maxSpellPages; i++)
            {
                spells.Add(new Spell(null, null, null));
            }

            currentPageIndex = 0;
            currentSpellPage = pages[currentPageIndex];

            currentSpellIndex = 0;
            currentSpell = spells[currentSpellIndex];
        }

        /// <summary>Change the underlying spell data and save when done.</summary>
        /// <param name="mutate">Apply changes to the spell data.</param>
        public void Mutate(Action<SpellBookData> mutate)
        {
            mutate(this.Data);
        }

        public void SaveSpellBook(ModEntry modEntry)
        {
            Data.ManaCostReductionAmount = manaCostReductionAmount;
            Data.SpellPages = pages;
            Data.CurrentSpellPage = currentSpellPage;
            Data.CurrentSpellPageIndex = currentPageIndex;
            Data.CurrentSpellIndex = currentSpellIndex;

            // save changes
            Data.Save(modEntry);
        }

        public SavedSpellBook ReadSpellBook(ModEntry modEntry)
        {
            return GetData().ReadSpellBook(modEntry);
        }

        public void SyncSpellBook(ModEntry modEntry)
        {
            SavedSpellBook loadedSpellBook = ReadSpellBook(modEntry);

            SavedSpellPage[] loadedSpellPages = loadedSpellBook.GetPages();
            SavedSpellPage savedCurrentSpellPage = loadedSpellBook.GetCurrentSpellPage();
            currentPageIndex = loadedSpellBook.GetCurrentSpellPageIndex();
            manaCostReductionAmount = loadedSpellBook.GetManaCostReductionAmount();

            for (int i = 0; i < GetPages().Count; i++)
            {
                SpellPage page = GetPages()[i];
                SavedSpellPage loadedSpellPage = loadedSpellBook.GetPages()[i];

                for (int j = 0; j < page.GetSpellShapeAreas().Length; j++)
                {
                    ShapeGroupArea area = page.GetSpellShapeAreas()[j];
                    SavedShapeGroupArea<SpellPartDraggable> spellShapeArea = loadedSpellPage.GetSpellShapes()[j];

                    if (spellShapeArea != null)
                    {
                        List<SpellPartDraggable> draggedShapes = spellShapeArea.GetAll();
                        area.SetAll(draggedShapes);
                    }
                }

                page.SetName(loadedSpellPages[i].GetName());
                page.SetSpellGrammerList(loadedSpellPages[i].GetSpellGrammerList());
            }

            SetCurrentSpellPageIndex(currentPageIndex);
            TurnToSpellPage();

            SetCurrentSpellIndex(loadedSpellBook.GetCurrentSpellIndex());
            TurnToSpell();
        }

        public void TurnToSpell()
        {

            if (currentSpellIndex < 0)
            {
                currentSpellIndex = 9;
            }
            else if (currentSpellIndex >= 10)
            {
                currentSpellIndex = 0;
            }

            currentSpell = spells[currentSpellIndex];
        }

        public void TurnToSpellPage()
        {

            if (currentPageIndex < 0)
            {
                currentPageIndex = 9;
            }
            else if (currentPageIndex >= 10)
            {
                currentPageIndex = 0;
            }

            currentSpellPage = pages[currentPageIndex];
        }

        public void CreateSpells(ModEntry modEntry)
        {
            spells = new List<ISpell>();

            for (int i = 0; i < maxSpellPages; i++)
            {
                spells.Add(new Spell(null, null, null));
            }

            for (int i = 0; i < spells.Count; i++)
            {
                spells[i] = CreateSpell(modEntry, i);
            }
        }

        public Spell CreateSpell(ModEntry modEntry, int spellPageIndex)
        {
            List<ShapeGroup> shapes = new List<ShapeGroup>();

            for (int i = 0; i < GetPages()[spellPageIndex].GetSpellShapeAreas().Length; i++)
            {
                ShapeGroupArea area = GetPages()[spellPageIndex].GetSpellShapeAreas()[i];
                List<SpellPartDraggable> spellPartDraggables = area.GetAll();

                List<ISpellPart> spellParts = new List<ISpellPart>();

                foreach (SpellPartDraggable spellPartDraggable in spellPartDraggables)
                {
                    spellParts.Add(spellPartDraggable.GetPart());
                }

                ShapeGroup shapeGroup = ShapeGroup.Of(spellParts);
                shapes.Add(shapeGroup);
            }

            List<SpellPartDraggable> spellGrammarPartDraggables = GetPages()[spellPageIndex].GetSpellGrammerList();
            List<ISpellPart> spellGrammarParts = new List<ISpellPart>();

            foreach (SpellPartDraggable spellPartDraggable in spellGrammarPartDraggables)
            {
                spellGrammarParts.Add(spellPartDraggable.GetPart());
            }

            Spell spell = new Spell(modEntry, shapes, SpellStack.Of(spellGrammarParts));

            spell.SetName(GetPages()[spellPageIndex].GetName());

            return spell;
        }

        public void SetPages(List<SpellPage> pages)
        {
            this.pages = pages;
        }

        public List<SpellPage> GetPages() 
        {  
            return pages; 
        }

        public List<ISpell> GetSpells()
        {
            return spells;
        }

        public void SetCurrentSpellPageIndex(int pageIndex)
        {
            currentPageIndex = pageIndex;
        }

        public int GetCurrentSpellPageIndex()
        {
            return currentPageIndex;
        }

        public void SetCurrentSpellIndex(int spellIndex)
        {
            currentSpellIndex = spellIndex;
        }

        public int GetCurrentSpellIndex()
        {
            return currentSpellIndex;
        }

        public void SetCurrentSpellPage(SpellPage spellPage)
        {
            currentSpellPage = spellPage;
        }

        public SpellPage GetCurrentSpellPage()
        {
            return currentSpellPage;
        }

        public void SetCurrentSpell(ISpell spell)
        {
            currentSpell = spell;
        }

        public ISpell GetCurrentSpell()
        {
            return currentSpell;
        }

        public void SetManaCostReductionAmount(int value)
        {
            manaCostReductionAmount = value;
        }

        public int GetManaCostReductionAmount()
        {
            return manaCostReductionAmount;
        }

        public SpellBookData GetData()
        {
            return this.Data;
        }
    }
}
