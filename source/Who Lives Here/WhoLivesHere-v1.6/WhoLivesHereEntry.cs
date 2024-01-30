/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/WhoLivesHere
**
*************************************************/

using StardewModdingAPI.Utilities;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using Prism99_Core.PatchingFramework;
using Prism99_Core.Utilities;

namespace WhoLivesHere
{
    internal class WhoLivesHereEntry : Mod
    {
        public IModHelper Helper ;
         private SDVLogger logger;
        private static bool Visible = false;
        private readonly static Dictionary<string, Tuple<Rectangle, Texture2D>> animalCache = new Dictionary<string, Tuple<Rectangle, Texture2D>>();
        private WhoLivesHereLogic whoLogic;
        public override void Entry(IModHelper helper)
        {
            logger = new SDVLogger(Monitor, helper.DirectoryPath, helper);
            Helper=helper;

            whoLogic = new WhoLivesHereLogic();

            whoLogic.Initialize(helper, logger);
        }

    }
}
