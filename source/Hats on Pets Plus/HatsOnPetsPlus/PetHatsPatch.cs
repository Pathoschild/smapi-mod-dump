/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SymaLoernn/Stardew_HatsOnPetsPlus
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using static HatsOnPetsPlus.HOPPHelperFunctions;
using StardewValley.Objects;

namespace HatsOnPetsPlus
{
    internal class PetHatsPatch
    {

        private static IMonitor Monitor;
        public static Dictionary<Tuple<string, string>, PetData> customPetsDict = new Dictionary<Tuple<string, string>, PetData>();

        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static void addPetToDictionnary(ExternalPetModData moddedPet)
        {
            addPetToDictionnary(moddedPet.Type, moddedPet.BreedId, new PetData(moddedPet.Sprites));
            
        }

        public static void addPetToDictionnary(string petType, string petBreed, PetData pet)
        {
            customPetsDict[new Tuple<string, string>(petType, petBreed)] = pet;
        }

        public static void resetDictionary()
        {
            customPetsDict.Clear();
        }

        internal static bool DrawHatPrefix(SpriteBatch b, Vector2 shake, StardewValley.Characters.Pet __instance)
        {
            try
            {
                //Monitor.Log("Entered draw hat prefix function", LogLevel.Debug);

                if (__instance.hat.Value == null)
                {
                    // This could return false since the original codes will also return without doing anything in this case, but I think it's best to let the vanilla code run
                    // in case something changes in the vanilla game in the future
                    return true;
                }

                // Check if the pet has custom hat data, if not default to vanilla logic

                Tuple<string, string> petTypeAndBreed = new Tuple<string, string>(__instance.petType, __instance.whichBreed);
                PetData customPet;
                if (!customPetsDict.TryGetValue(petTypeAndBreed, out customPet))
                {
                    //Monitor.Log("No modded data found for this pet ["+ petTypeAndBreed +"], defaulting to vanilla logic", LogLevel.Debug);
                    return true;
                }

                // Check if this sprite has custom data, if not use default data, or default to vanilla logic if there is no usable data

                bool flipped = __instance.flip || (__instance.sprite.Value.CurrentAnimation != null && __instance.sprite.Value.CurrentAnimation[__instance.sprite.Value.currentAnimationIndex].flip);
                SpriteData? customHatData;
                if (!
                    customPet.sprites.TryGetValue(new Tuple<int, bool>(__instance.Sprite.currentFrame, flipped), out customHatData)
                   )
                {
                    if (customPet.defaultSprite == null)
                    {
                        //Monitor.Log("No modded data found for this sprite on this otherwise custom pet and no default data, defaulting to vanilla logic", LogLevel.Debug);
                        return true;
                    } else
                    {
                        //Monitor.Log("No modded data found for this sprite, using default data for this custom pet, LogLevel.Debug);
                        customHatData = customPet.defaultSprite;
                    }
                    
                }

                // Check if DoNotDraw flag is set
                if (customHatData.doNotDraw.HasValue && customHatData.doNotDraw.Value)
                {
                    return false;
                }


                //Monitor.Log("Custom data found, starting logic for drawing a hat on a custom pet", LogLevel.Debug);

                Vector2 hatOffset = Vector2.Zero;
                hatOffset *= 4f;
                if (hatOffset.X <= -100f)
                {
                    return true;
                }
                float horse_draw_layer = Math.Max(0f, __instance.isSleepingOnFarmerBed.Value ? (((float)__instance.StandingPixel.Y + 112f) / 10000f) : ((float)__instance.StandingPixel.Y / 10000f));
                hatOffset.X = -2f;
                hatOffset.Y = -24f;
                horse_draw_layer += 1E-07f;
                int direction = 2;
                // flipped boolean is initialized earlier
                float scale = 1.3333334f;

                // Adjust hat placement, direction and scale with custom data when provided
                hatOffset.X += customHatData.hatOffsetX.HasValue ? customHatData.hatOffsetX.Value : 0;
                hatOffset.Y += customHatData.hatOffsetY.HasValue ? customHatData.hatOffsetY.Value : 0;
                direction = customHatData.direction.HasValue ? customHatData.direction.Value : direction;
                scale = customHatData.scale.HasValue ? customHatData.scale.Value : scale;

                hatOffset += shake; 

                // Not sure if the following lines from the vanilla logic should be implemented or not
                // In my opinion, it makes it more confusing so I'll leave it commented for now
                //if (flipped)
                //{
                //    hatOffset.X -= 4f;
                //}

                __instance.hat.Value.draw(b, __instance.getLocalPosition(Game1.viewport) + hatOffset + new Vector2(30f, -42f), scale, 1f, horse_draw_layer, direction, useAnimalTexture: true);

                return false;
            }
            catch(Exception ex)
            {
                Monitor.Log($"Failed in {nameof(DrawHatPrefix)}:\n{ex}", LogLevel.Error);
                return true; // run original logic in case of exception
            }
            
        }

        internal static void CheckActionPostfix(StardewValley.Characters.Pet __instance, Farmer who)
        {
            try
            {
                Tuple<string, string> petTypeAndBreed = new Tuple<string, string>(__instance.petType.Value, __instance.whichBreed.Value);

                // Mostly copied from the original game code, but check if the player is holding something and it's a hat
                if (who.Items.Count > who.CurrentToolIndex && who.Items[who.CurrentToolIndex] != null && who.Items[who.CurrentToolIndex] is Hat)
                {
                    // Always let the player remove hats!!
                    if (__instance.hat.Value != null)
                    {
                        Game1.createItemDebris(__instance.hat.Value, __instance.Position, __instance.FacingDirection);
                        __instance.hat.Value = null;
                    }
                    // If there's hat offset data for that animal and breed, then let the player put a hat on
                    // Cats and Dogs are already covered in the vanilla code, filtering them out to prevent double actions
                    else if (__instance.petType != "Cat" && __instance.petType != "Dog" && customPetsDict.TryGetValue(petTypeAndBreed, out _))
                    {
                        Hat hatItem = who.Items[who.CurrentToolIndex] as Hat;
                        who.Items[who.CurrentToolIndex] = null;
                        __instance.hat.Value = hatItem;
                        Game1.playSound("dirtyHit");
                    }
                    __instance.mutex.ReleaseLock();
                }
            }
            catch (Exception e)
            {
                Monitor.Log($"Failed in {nameof(CheckActionPostfix)}:\n{e}", LogLevel.Error);
            }
        }

    }
}
