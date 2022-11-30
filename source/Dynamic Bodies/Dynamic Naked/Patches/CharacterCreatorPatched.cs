/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using StardewValley.Locations;
using StardewValley.Characters;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley.Objects;
using System.IO;
using DynamicBodies.Data;

namespace DynamicBodies.Patches
{
    internal class CharacterCreatorPatched
    {
        static bool NewFarmHand = false;
        static Farmer player = null;
        static int hairCache = -1;
        static List<int> defaultHairStyles;
        static List<string> all_hairs;
        static Color hairColorCache;
        public CharacterCreatorPatched(Harmony harmony)
        {
            //Check if we need to 'new game' patch
            harmony.Patch(
                original: AccessTools.Constructor(typeof(CharacterCustomization), new[] { typeof(CharacterCustomization.Source) }),
                postfix: new HarmonyMethod(GetType(), nameof(post_CharacterCustomization_setup))
            );

            //Update the way it handles the intro
            harmony.Patch(
                original: AccessTools.Method(typeof(CharacterCustomization), nameof(CharacterCustomization.update), new[] { typeof(GameTime) }),
                prefix: new HarmonyMethod(GetType(), nameof(pre_Update))
            );
        }
        public static void post_CharacterCustomization_setup(CharacterCustomization __instance, CharacterCustomization.Source source)
        {
            NewFarmHand = (source == CharacterCustomization.Source.NewGame) || (source == CharacterCustomization.Source.NewFarmhand);
            if (NewFarmHand)
            {
                player = Game1.player;
                defaultHairStyles = Farmer.GetAllHairstyleIndices();
                all_hairs = ModEntry.getContentPackOptions(ModEntry.hairOptions).ToList();
                hairColorCache = player.hairstyleColor.Value;
            }
        }

        private static void pre_Update(CharacterCustomization __instance, GameTime time)
        {
            if (NewFarmHand)
            {
                //No shirt equipped, so equip one
                if(player.shirtItem.Value == null)
                {
                    player.shirtItem.Set(new Clothing(player.shirt.Value));
                }

                //No boots equipped, so equip one
                if(player.boots.Value == null)
                {
                    Boots boot = new Boots(506);//Leather boots
                    //As the game doesn't make dummy boots
                    boot.displayName = ModEntry.translate("shoes");
                    boot.description = ModEntry.translate("shoes_description");
                    boot.defenseBonus.Set(0);
                    boot.immunityBonus.Set(0);
                    player.boots.Set(boot);
                }

                //Fix hair style
                if(hairCache != player.hair.Value)
                {
                    PlayerBaseExtended pbe = PlayerBaseExtended.Get(player);

                    int i = 0;
                    for(int j = 0; j < defaultHairStyles.Count; j++)
                    {
                        i = j;
                        if (defaultHairStyles[j] == player.hair.Value)
                        {
                            break;
                        }
                    }


                    if (i < all_hairs.Count)
                    {
                        pbe.SetModData(player, "DB.hairStyle", all_hairs[i]);

                        ModEntry.MakePlayerDirty();
                    } else
                    {
                        //Some add 100 for some reason
                        ModEntry.debugmsg("Hair error for: " + player.hair.Value+" - index out of bounds, not applied", LogLevel.Error);
                    }
                    
                    hairCache = player.hair.Value;
                }

                //Darken the hair colour
                if(!hairColorCache.Equals(player.hairstyleColor.Value))
                {
                    player.modData["DB.darkHair"] = new Color(player.hairstyleColor.Value.R / 2, player.hairstyleColor.Value.G / 2, player.hairstyleColor.Value.B / 2).PackedValue.ToString();
                    hairColorCache = player.hairstyleColor.Value;
                }
            }
        }
    }
}
