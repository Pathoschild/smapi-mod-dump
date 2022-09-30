/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyRoguelike
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Reflection;

namespace StardewRoguelike.Patches
{
    internal class NoWeedDropsPatch : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new(typeof(StardewValley.Object), "cutWeed");

        public static bool Prefix(StardewValley.Object __instance, Farmer who, GameLocation location = null)
        {
            if (location is null && who is not null)
                location = who.currentLocation;

			Color c = Color.Green;
			string sound = "cut";
			int animation = 50;
			__instance.Fragility = 2;

			Multiplayer multiplayer = (Multiplayer)typeof(Game1).GetField("multiplayer", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

			switch (__instance.ParentSheetIndex)
			{
				case 678:
					c = new Color(228, 109, 159);
					break;
				case 679:
					c = new Color(253, 191, 46);
					break;
				case 313:
				case 314:
				case 315:
					c = new Color(84, 101, 27);
					break;
				case 316:
				case 317:
				case 318:
					c = new Color(109, 49, 196);
					break;
				case 319:
					c = new Color(30, 216, 255);
					sound = "breakingGlass";
					animation = 47;
					location.playSound("drumkit2");
					break;
				case 320:
					c = new Color(175, 143, 255);
					sound = "breakingGlass";
					animation = 47;
					location.playSound("drumkit2");
					break;
				case 321:
					c = new Color(73, 255, 158);
					sound = "breakingGlass";
					animation = 47;
					location.playSound("drumkit2");
					break;
				case 792:
				case 793:
				case 794:
					break;
				case 882:
				case 883:
				case 884:
					c = new Color(30, 97, 68);
					break;
			}

			location.playSound(sound);
			multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(animation, __instance.TileLocation * 64f, c));
			multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(animation, __instance.TileLocation * 64f + new Vector2(Game1.random.Next(-16, 16), Game1.random.Next(-48, 48)), c * 0.75f)
			{
				scale = 0.75f,
				flipped = true
			});

			multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(animation, __instance.TileLocation * 64f + new Vector2(Game1.random.Next(-16, 16), Game1.random.Next(-48, 48)), c * 0.75f)
			{
				scale = 0.75f,
				delayBeforeAnimationStart = 50
			});

			multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(animation, __instance.TileLocation * 64f + new Vector2(Game1.random.Next(-16, 16), Game1.random.Next(-48, 48)), c * 0.75f)
			{
				scale = 0.75f,
				flipped = true,
				delayBeforeAnimationStart = 100
			});

			if (!sound.Equals("breakingGlass"))
			{
				if (Game1.random.NextDouble() < 1E-05)
					location.debris.Add(new Debris(new Hat(40), __instance.TileLocation * 64f + new Vector2(32f, 32f)));
			}

			if (Game1.random.NextDouble() < 0.02)
				location.addJumperFrog(__instance.TileLocation);

			return false;
        }
    }
}
