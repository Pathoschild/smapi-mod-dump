/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using DecidedlyShared.Logging;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Characters;

namespace CarryYourPet.Patches
{
    public class Patches
    {
        private static CarriedCharacter carriedCharacter;
        private static ModConfig config;
        private static Logger logger;

        public Patches(ModConfig c, CarriedCharacter p, Logger l)
        {
            carriedCharacter = p;
            config = c;
            logger = l;
        }

        public static bool ShouldAlwaysDraw { get; }

        public static void PetCheckAction_Postfix(Pet __instance, Farmer who, GameLocation l, bool __result)
        {
            try
            {
                // if carriedCharacter.Npc is null, that means we're not currently carrying an NPC...
                if (carriedCharacter.Npc == null)
                {
                    // ...so we check to see if the appropriate hotkey is held down...
                    if (config.HoldToCarryNpc.IsDown())
                        // ...and set this instance as the carried NPC.
                        carriedCharacter.Npc = __instance;
                }
                else
                {
                    // Otherwise, since we are carrying an NPC, we set the carried value to null if the hotkey is held down.
                    if (config.HoldToCarryNpc.IsDown()) carriedCharacter.Npc = null;
                }
            }
            catch (Exception e)
            {
                logger.Exception(e);
            }
        }

        public static bool HorseCheckAction_Prefix(Horse __instance, Farmer who, GameLocation l, bool __result)
        {
            try
            {
                // if carriedCharacter.Npc is null, that means we're not currently carrying an NPC...
                if (carriedCharacter.Npc == null)
                {
                    // ...so we check to see if the appropriate hotkey is held down...
                    if (config.HoldToCarryNpc.IsDown())
                    {
                        // ...and set this instance as the carried NPC.
                        carriedCharacter.Npc = __instance;
                        return false;
                    }
                }
                else
                {
                    // Otherwise, since we are carrying an NPC, we set the carried value to null if the hotkey is held down.
                    if (config.HoldToCarryNpc.IsDown()) carriedCharacter.Npc = null;
                }
            }
            catch (Exception e)
            {
                logger.Exception(e);

                return true;
            }

            return true;
        }

        public static bool FarmerIsCarrying_Postfix(bool __result)
        {
            if (carriedCharacter.Npc != null)
                __result = true;

            return __result;
        }

        public static bool PetDraw_Prefix(Pet __instance, SpriteBatch b)
        {
            try
            {
                // If the carried NPC is not null, we need to control the drawing.
                if (carriedCharacter?.Npc != null)
                {
                    // If this is the Pet instance we're carrying, we need to handle the drawing.
                    if (__instance == carriedCharacter?.Npc)
                    {
                        // If ShouldDraw is true, it means we're manually drawing for this frame, so we should return true to allow the method to run.
                        if (carriedCharacter.ShouldDraw)
                            return true;
                        return false;
                    }
                    else // If it isn't, we allow the game to draw as per usual.
                        return true;
                }
            }
            catch (Exception e)
            {
                logger.Exception(e);

                // Return true after the exception is thrown so the original method runs.
                return true;
            }

            return true;
        }

        public static bool HorseDraw_Prefix(Horse __instance, SpriteBatch b)
        {
            try
            {
                // If the carried NPC is not null, we need to control the drawing.
                if (carriedCharacter?.Npc != null)
                {
                    // If this is the Pet instance we're carrying, we need to handle the drawing.
                    if (__instance == carriedCharacter?.Npc)
                    {
                        // If ShouldDraw is true, it means we're manually drawing for this frame, so we should return true to allow the method to run.
                        if (carriedCharacter.ShouldDraw)
                            return true;
                        return false;
                    }
                    else // If it isn't, we allow the game to draw as per usual.
                        return true;
                }
            }
            catch (Exception e)
            {
                logger.Exception(e);

                // Return true after the exception is thrown so the original method runs.
                return true;
            }

            return true;
        }

        public static bool FarmAnimalPet_Prefix(FarmAnimal __instance, Farmer who, bool is_auto_pet)
        {
            if (is_auto_pet)
                return true;

            try
            {
                // if carriedCharacter.Npc is null, that means we're not currently carrying an NPC...
                if (carriedCharacter.Npc == null)
                {
                    // ...so we check to see if the appropriate hotkey is held down...
                    if (config.HoldToCarryNpc.IsDown())
                    {
                        // ...and set this instance as the carried NPC.
                        carriedCharacter.Npc = __instance;

                        return false;
                    }
                }
                else
                {
                    // Otherwise, since we are carrying an NPC, we set the carried value to null if the hotkey is held down.
                    if (config.HoldToCarryNpc.IsDown())
                    {
                        carriedCharacter.Npc = null;

                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Exception(e);

                return true;
            }

            return true;
        }

        public static bool FarmAnimalDraw_Prefix(FarmAnimal __instance, SpriteBatch b)
        {
            try
            {
                // If the carried NPC is not null, we need to control the drawing.
                if (carriedCharacter?.Npc != null)
                    // But only if this is the NPC we're carrying.
                    if (carriedCharacter.Npc == __instance)
                    {
                        // If ShouldDraw is true, it means we're manually drawing for this frame, so we should return true to allow the method to run.
                        if (carriedCharacter.ShouldDraw)
                            return true;
                        return false;
                    }
            }
            catch (Exception e)
            {
                logger.Exception(e);

                // Return true after the exception is thrown so the original method runs.
                return true;
            }

            return true;
        }

        // These are the method patches for NPC carrying.
        // public static void NpcCheckAction_Postfix(NPC __instance, Farmer who, GameLocation l, bool __result)
        // {
        //     if (carriedCharacter.Npc == null)
        //     {
        //         if (config.HoldToCarryNpc.IsDown())
        //         {
        //             carriedCharacter.Npc = __instance;
        //             carriedCharacter.IsPet = false;
        //         }
        //     }
        //     else
        //     {
        //         if (config.HoldToCarryNpc.IsDown())
        //         {
        //             carriedCharacter.Npc = null;
        //             carriedCharacter.IsPet = false;
        //         }
        //     }
        // }
        //
        // public static bool NpcDraw_Prefix(NPC __instance, SpriteBatch b)
        // {
        //     if (carriedCharacter != null)
        //     {
        //         if (carriedCharacter.Npc != null)
        //         {
        //             if (!carriedCharacter.IsPet)
        //             {
        //                 if (carriedCharacter.ShouldDraw)
        //                     return true;
        //                 else
        //                     return false;
        //             }
        //         }
        //     }
        //
        //     return true;
        // }
    }
}
