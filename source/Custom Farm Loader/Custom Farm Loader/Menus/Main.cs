/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley-custom-farm-loader
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using StardewValley;
using System.Xml;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using System.Reflection;
using Custom_Farm_Loader.Lib;
using StardewValley.Network;
using StardewValley.GameData;
using Custom_Farm_Loader.Menus;

namespace Custom_Farm_Loader.Menus
{
    public class Main
    {
        public static Mod Mod;
        private static IMonitor Monitor;
        private static IModHelper Helper;

        public static void Initialize(Mod mod)
        {
            Mod = mod;
            Monitor = mod.Monitor;
            Helper = mod.Helper;

            _CharacterCustomizationMenu.Initialize(mod);
            _LoadGameMenu.Initialize(mod);
            CustomFarmSelection.Initialize(mod);
        }
    }
}