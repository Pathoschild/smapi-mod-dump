/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/EpicBellyFlop45/StardewMods
**
*************************************************/

using FarmAnimalVarietyRedux.Enums;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Reflection;

namespace FarmAnimalVarietyRedux.Patches
{
    /// <summary>Contains patches for patching game code in the <see cref="AnimatedSprite"/> class.</summary>
    internal class AnimatedSpritePatch
    {
        /*********
        ** Internal Methods
        *********/
        /// <summary>The prefix for the LoadTexture method.</summary>
        /// <param name="textureName">The name of the texture to load.</param>
        /// <param name="__instance">The <see cref="AnimatedSprite"/> instance being patched.</param>
        /// <returns>False meaning the original method won't get ran.</returns>
        internal static bool LoadTexturePrefix(AnimatedSprite __instance)
        {
            var loadedTexture = (string)typeof(AnimatedSprite).GetField("loadedTexture", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            if (loadedTexture == __instance.textureName.Value)
                return false;

            var spriteTexture = (Texture2D)typeof(AnimatedSprite).GetField("spriteTexture", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            var contentManager = (ContentManager)typeof(AnimatedSprite).GetField("contentManager", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // check if the texture to load is an animal
            if (__instance.textureName.Value != null && __instance.textureName.Value.Contains("Animals\\"))
            {
                var animalName = __instance.textureName.Value.Split('\\')[1];
                var animalSubType = ModEntry.Instance.Api.GetAnimalSubTypeByName(animalName.Replace("Baby", "").Replace("Sheared", ""));

                if (animalSubType != null) // if animalSubType is null, it means the sprite sheet being loaded is of an animal but a non farm animal (horse, dog, cat)
                {
                    // get the current season
                    var currentSeason = Season.Spring;
                    switch (Game1.currentSeason)
                    {
                        case "spring":
                            currentSeason = Season.Spring;
                            break;
                        case "summer":
                            currentSeason = Season.Summer;
                            break;
                        case "fall":
                            currentSeason = Season.Fall;
                            break;
                        case "winter":
                            currentSeason = Season.Winter;
                            break;
                    }

                    // load the texture through FAVR, not the content pipeline
                    spriteTexture = animalSubType.Sprites.GetSpriteSheet(
                        isBaby: animalName.Contains("Baby"),
                        isHarvested: animalName.Contains("Sheared"),
                        season: currentSeason
                    );
                }
                else
                    spriteTexture = contentManager.Load<Texture2D>(__instance.textureName.Value);
            }
            else
                spriteTexture = __instance.textureName.Value != null ? contentManager.Load<Texture2D>(__instance.textureName.Value) : null;

            loadedTexture = __instance.textureName.Value;

            typeof(AnimatedSprite).GetField("spriteTexture", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, spriteTexture);
            typeof(AnimatedSprite).GetField("loadedTexture", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(__instance, loadedTexture);

            if (spriteTexture == null)
                return false;

            __instance.UpdateSourceRect();
            return false;
        }
    }
}
