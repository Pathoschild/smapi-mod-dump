/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using System;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace Entoarox.Framework.Indev
{
    [Obsolete("Currently in development", true)]
    internal class VanillaProfession : IProfession
    {
        /*********
        ** Accessors
        *********/
        public int SkillLevel { get; }

        public string Parent { get; }

        public string Name { get; }

        public string DisplayName => LevelUpMenu.getProfessionTitleFromNumber(SkillHelper.VanillaProfessions[this.Name]);

        public string Description => string.Join("\n", LevelUpMenu.getProfessionDescription(SkillHelper.VanillaProfessions[this.Name]));

        public Texture2D Icon => throw new NotImplementedException();


        /*********
        ** Public methods
        *********/
        public VanillaProfession(string name, string parent, int level)
        {
            this.Name = name;
            this.Parent = parent;
            this.SkillLevel = level;
        }
    }
}
