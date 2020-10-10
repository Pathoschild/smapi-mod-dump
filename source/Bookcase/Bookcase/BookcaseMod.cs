/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using StardewModdingAPI;
using Bookcase.Lib;
using Bookcase.Patches;
using System;

namespace Bookcase {

    public class BookcaseMod : Mod {

        internal static IModHelper modHelper;
        internal static IReflectionHelper reflection;
        internal static Log logger;
        internal static Random random;

        public override void Entry(IModHelper helper) {

            modHelper = helper;
            reflection = helper.Reflection;
            logger = new Log(this);
            random = new Random();

            StardewModHooksWrapper.CreateWrapper();
            PatchManager patchManager = new PatchManager();
            Events.SMAPIEventWrapper.SubscribeToSMAPIEvents();
        }
    }
}