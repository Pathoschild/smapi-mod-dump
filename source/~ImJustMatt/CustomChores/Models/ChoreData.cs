/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/SDVCustomChores
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace LeFauxMatt.CustomChores.Models
{
    public class ChoreData
    {
        public string ChoreName { get; }
        public IDictionary<string, object> Config { get; }
        public IList<TranslationData> Translations { get; }
        public Texture2D Image { get; }

        public ChoreData(string choreName, IDictionary<string, object> config, IList<TranslationData> translations, Texture2D image)
        {
            ChoreName = choreName;
            Config = config;
            Translations = translations;
            Image = image;
        }
    }
}
