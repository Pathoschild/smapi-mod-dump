/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace RidgesideVillage
{
    internal class ShopTileAction
    {

        static IModHelper Helper;
        internal static void Initialize(IMod ModInstance)
        {
            Helper = ModInstance.Helper;

            TileActionHandler.RegisterTileAction("RSVShop", OpenShop);
        }

        private static void OpenShop(string tileActionString, Vector2 position)
        {
            var split = tileActionString.Split(' ');
            if (split.Length > 1)
            {
                ExternalAPIs.STFAPI.OpenItemShop(split[1]);
            }
        }

    }
}

