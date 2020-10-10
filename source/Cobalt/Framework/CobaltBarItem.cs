/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Cobalt
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using SObject = StardewValley.Object;

namespace Cobalt.Framework
{
    internal class CobaltBarItem
    {
        public const int INDEX = 900;
        public const string NAME = "Cobalt Bar";
        public const string DESCRIPTION = "A fabled ore, usable for amazing things.";
        public const int PRICE = 10000;
        public const string TYPE = "Basic";
        public const int CATEGORY = SObject.metalResources;
        public const int EDIBILITY = -300;

        private Texture2D icon = ModEntry.instance.Helper.Content.Load<Texture2D>("assets/cobalt-bar.png");
    }
}
