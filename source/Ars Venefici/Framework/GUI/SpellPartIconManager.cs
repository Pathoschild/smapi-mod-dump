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
using ArsVenefici.Framework.Spells;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArsVenefici.Framework.GUI
{
    public class SpellPartIconManager
    {
        public Dictionary<string, Texture2D> spellPartSprites = new Dictionary<string, Texture2D>();

        public SpellPartIconManager(ModEntry modEntry)
        {
            foreach (KeyValuePair<string, ISpellPart> item in modEntry.spellPartManager.spellParts)
            {
                PoplulateSprites(item.Value, modEntry);
            }
        }

        public void PoplulateSprites(ISpellPart spellPart, ModEntry modEntry)
        {
            try
            {
                spellPartSprites.Add(spellPart.GetId(), modEntry.Helper.ModContent.Load<Texture2D>("assets/icon/spellpart/" + spellPart.GetId() + ".png"));
            }
            catch (ContentLoadException e)
            {
                modEntry.Monitor.Log("Failed to load icon for spell " + spellPart.GetId() + ": " + e, LogLevel.Warn);
            }
        }

        public Texture2D GetSprite(string name)
        {
            Texture2D sprite = null;
            return spellPartSprites.TryGetValue(name, out sprite) ? sprite : null;
        }
    }
}
