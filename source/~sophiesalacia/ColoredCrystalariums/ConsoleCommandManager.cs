/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace ColoredCrystalariums;

internal class ConsoleCommandManager
{
    private static readonly List<int> PossibleGems = new()
    {
        60, 62, 64, 66, 68, 70, 72, 80, 82, 84, 86, 538, 539, 540, 541, 542, 543, 544, 545, 546, 547, 548, 549, 550,
        551, 552, 553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 565, 566, 567, 568, 569, 570, 571, 572,
        573, 574, 575, 576, 577, 578
    };

    internal static void InitializeConsoleCommands()
    {
        Globals.CCHelper.Add(
            "sophie.ccc.setup",
            "Sets up a large patch of crystalariums on the farm. WARNING: Will destroy anything where it sets up.",
            SetUpCrystalariums
        );
    }

    private static void SetUpCrystalariums(string command, string[] args)
    {
        if (!Context.IsWorldReady)
        {
            Log.Info("This command should only be used in a loaded save.");
            return;
        }

        Farm farm = Game1.getFarm();

        if (farm is null)
            return;

        if (args.Length == 0)
        {
            for (int x = 20; x < 121; x++)
            {
                for (int y = 30; y < 71; y += 3)
                {
                    farm.removeEverythingExceptCharactersFromThisTile(x, y);
                    Vector2 position = new(x, y);
                    Object crystalarium = new(position, 21);
                    Object gem = GetRandomGem();
                    farm.Objects.Add(new Vector2(x, y), crystalarium);
                    crystalarium.performObjectDropInAction(gem, true, Game1.player);
                }
            }
        }
        else
        {
            int gemNum = 0;
            for (int x = 20; x < 37; x++)
            {
                for (int y = 30; y < 39; y += 3)
                {
                    farm.removeEverythingExceptCharactersFromThisTile(x, y);
                    Vector2 position = new(x, y);
                    Object crystalarium = new(position, 21);
                    Object gem = (Object)new Object(PossibleGems[gemNum], 1).getOne();
                    farm.Objects.Add(new Vector2(x, y), crystalarium);
                    crystalarium.performObjectDropInAction(gem, true, Game1.player);
                    gemNum++;
                }
            }
        }
    }

    private static Object GetRandomGem()
    {
        int numGems = PossibleGems.Count;
        Random rand = new();
        int index = rand.Next(numGems);
        return (Object)new Object(PossibleGems[index], 1).getOne();
    }
}
