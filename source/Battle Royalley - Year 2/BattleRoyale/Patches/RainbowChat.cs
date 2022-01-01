/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using System;

namespace BattleRoyale.Patches
{
    class RainbowChat : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(ChatMessage), "draw");

        public static void Prefix(ref Color ___color, ChatMessage __instance)
        {
            try
            {
                if (__instance.message.Count > 0)
                {
                    string msg = __instance.message[0].message;
                    if (msg.Contains(":"))
                    {
                        string name = msg.Split(':')[0];
                        foreach (Farmer f in Game1.getAllFarmers())
                        {
                            if (f != null && f.Name == name && f.favoriteThing.Value == "chairity")
                            {
                                float speed = 3;

                                float r, g, b;
                                double t = speed * 2 * Math.PI / 600 * __instance.timeLeftToDisplay;
                                r = (float)Math.Cos(t);
                                g = (float)Math.Sin(t);
                                b = 1 - r - g;

                                ___color = new Color(r, g, b);

                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
