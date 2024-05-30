/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/Ars-Venefici
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.Spells
{
    public class SavedSpellBook
    {
        private SavedSpellPage[] pages;
        private SavedSpellPage currentSpellPage;
        private int currentPageIndex;
        private int currentSpellIndex;

        int maxSpellPages = 10;

        private int manaCostReductionAmount = 1;

        public SavedSpellBook()
        {
            pages = new SavedSpellPage[10];

            for (int i = 0; i < maxSpellPages; i++)
            {
                pages[i] = new SavedSpellPage();
            }

            currentPageIndex = 0;
            currentSpellPage = pages[currentPageIndex];
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

            //currentSpell = spells[currentSpellIndex];
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

        public void SetPages(SavedSpellPage[] pages)
        {
            this.pages = pages;
        }

        public SavedSpellPage[] GetPages()
        {
            return pages;
        }

        public void SetCurrentSpellIndex(int spellIndex)
        {
            currentSpellIndex = spellIndex;
        }

        public int GetCurrentSpellIndex()
        {
            return currentSpellIndex;
        }

        public void SetCurrentSpellPageIndex(int pageIndex)
        {
            currentPageIndex = pageIndex;
        }

        public int GetCurrentSpellPageIndex()
        {
            return currentPageIndex;
        }

        public void SetCurrentSpellPage(SavedSpellPage spellPage)
        {
            currentSpellPage = spellPage;
        }

        public SavedSpellPage GetCurrentSpellPage()
        {
            return currentSpellPage;
        }

        public void SetManaCostReductionAmount(int value)
        {
            manaCostReductionAmount = value;
        }

        public int GetManaCostReductionAmount()
        {
            return manaCostReductionAmount;
        }
    }
}
