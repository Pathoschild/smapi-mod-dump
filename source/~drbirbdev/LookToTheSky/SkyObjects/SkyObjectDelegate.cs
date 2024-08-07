/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.Collections.Generic;
using System.Reflection;
using BirbCore.Attributes;
using StardewValley;

namespace LookToTheSky.SkyObjects;

public delegate void SkyObjectHandlerDelegate(SkyObject skyObject, string[] args);

public class SkyObjectEvent
{
    private static readonly Dictionary<string, SkyObjectHandlerDelegate> HANDLERS = new();

    private static void SetupSkyObjectHandlers()
    {
        if (HANDLERS.Count != 0)
        {
            return;
        }

        MethodInfo[] array = typeof(DefaultEvents).GetMethods(BindingFlags.Static | BindingFlags.Public);
        foreach (MethodInfo method in array)
        {
            HANDLERS.Add(method.Name, method.CreateDelegate<SkyObjectHandlerDelegate>());
        }
    }

    private class DefaultEvents
    {
        public static void SpawnItem(string[] args)
        {
            if (args.Length < 1)
            {
                ArgUtility.TryGetOptionalInt(args, 1, out int stackSize, out string error1, 1);
                ArgUtility.TryGetOptionalInt(args, 2, out int quality, out string error2);
                if (error1 is not null || error2 is not null)
                {
                    Log.Error($"SpawnItem got errors {error1}, {error2}");
                }
                Item item = ItemRegistry.Create(args[0], stackSize, quality);
                Game1.createItemDebris(item, Game1.player.getStandingPosition(), 2, Game1.currentLocation);
            }
        }

        public static void PlaySound(TemporaryAnimatedSprite sprite, string[] args)
        {

        }
    }
}
