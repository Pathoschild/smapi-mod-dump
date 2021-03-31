/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AndyCrocker/StardewMods
**
*************************************************/

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
        /// <summary>The prefix for the <see cref="AnimatedSprite.loadTexture())"/> method.</summary>
        /// <param name="__instance">The <see cref="AnimatedSprite"/> instance being patched.</param>
        /// <returns><see langword="false"/>, meaning the original method will not get ran.</returns>
        /// <remarks>This is used to load the sprite sheets of custom animals.</remarks>
        internal static bool LoadTexturePrefix(AnimatedSprite __instance)
        {
            if (__instance.loadedTexture == __instance.textureName.Value)
                return false;
            __instance.loadedTexture = __instance.textureName.Value;

            var contentManager = (ContentManager)typeof(AnimatedSprite).GetField("contentManager", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

            // check if the texture to load is an animal
            if (__instance.textureName.Value != null && __instance.textureName.Value.Contains("Animals\\"))
            {
                var animalName = __instance.textureName.Value.Split('\\')[1];
                var subtypeInternalName = animalName.ToLower().Replace("baby", "").Replace("sheared", "");
                
                var animal = ModEntry.Instance.Api.GetAnimalByInternalSubtypeName(subtypeInternalName);
                var animalSubtype = ModEntry.Instance.Api.GetAnimalSubtypeByInternalName(subtypeInternalName);

                // if animalSubtype is null, it means the sprite sheet being loaded is of an animal but a non farm animal (horse, dog, cat)
                var textureName = __instance.textureName.Value;
                if (animalSubtype != null)
                    textureName = $"favr{animal.InternalName},{animalSubtype.InternalName},{animalName.ToLower().Contains("baby")},{animalName.ToLower().Contains("sheared")},{Game1.currentSeason}";

                __instance.spriteTexture = contentManager.Load<Texture2D>(textureName);
            }
            else
                __instance.spriteTexture = (__instance.textureName.Value != null) 
                    ? contentManager.Load<Texture2D>(__instance.textureName.Value) 
                    : null;

            if (__instance.spriteTexture == null)
                return false;

            __instance.UpdateSourceRect();
            return false;
        }
    }
}
