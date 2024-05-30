/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using MoonShared;
using MoonShared.Command;
using StardewValley;
using StardewValley.Tools;

namespace ArchaeologySkill
{
    [CommandClass]
    public class Command
    {
        [CommandMethod("testing spawning a water shifter")]
        public static void Invokewater()
        {
            string Stringhere = "ArchaeologySkill.Objects.ShifterObject/stringHere";
            string type = Stringhere.Substring(0, Stringhere.IndexOf('/'));
            string arg = Stringhere.Substring(Stringhere.IndexOf('/') + 1);

            Log.Warn(type);
            Log.Warn(arg);


            int xLocation = (int)Game1.player.Position.Y;
            int yLocation = (int)Game1.player.Position.Y;
            Game1.createItemDebris(TestingThisThing(type, arg), new Vector2((float)xLocation + 0.5f, (float)yLocation + 0.5f) * 64f, -1);
        }

        public static Item TestingThisThing(string type, string arg)
        {
            var ctor = AccessTools.Constructor(AccessTools.TypeByName(type), new[] { typeof(string) });
            return (Item)ctor.Invoke(new object[] { arg });
        }


    }
}
