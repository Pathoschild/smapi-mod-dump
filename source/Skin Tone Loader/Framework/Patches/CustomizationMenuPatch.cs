/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/SkinToneLoader
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Menus.CharacterCustomization;

namespace SkinToneLoader.Framework.Patches
{
    
    /// <summary>
    /// Class that patches the customization menu buttons
    /// </summary>
    public class CustomizationMenuPatch
    {
        private readonly Type _menu = typeof(CharacterCustomization);

        // Instance of ModEntry
        private static ModEntry modEntryInstance;

        /// <summary>
        /// CustomizationMenuPatch Constructor
        /// </summary>
        /// <param name="entry">The instance of ModEntry</param>
        public CustomizationMenuPatch(ModEntry entry)
        {
            // Set the field
            modEntryInstance = entry;
        }

        internal void Apply(Harmony harmony)
        {
            modEntryInstance.Monitor.Log("Patching CustomizationMenu", LogLevel.Info);

            harmony.Patch(
                original: AccessTools.Method(_menu, "selectionClick", new[] { typeof(string), typeof(int) }),
                prefix: new HarmonyMethod(GetType(), nameof(SelectionClickPrefix))
            );
        }

        private static bool SelectionClickPrefix(string name, int change, CharacterCustomization __instance, List<ClickableComponent> ___leftSelectionButtons, Farmer ____displayFarmer)
        {
            switch (name)
            {
                case "Skin":
                    Game1.player.changeSkinColor((int)Game1.player.skin + change);
                    Game1.playSound("skeletonStep");
                    break;
                case "Hair":
                    {
                        List<int> allHairstyleIndices = Farmer.GetAllHairstyleIndices();
                        int num2 = allHairstyleIndices.IndexOf(Game1.player.hair);
                        num2 += change;
                        if (num2 >= allHairstyleIndices.Count)
                        {
                            num2 = 0;
                        }
                        else if (num2 < 0)
                        {
                            num2 = allHairstyleIndices.Count - 1;
                        }

                        Game1.player.changeHairStyle(allHairstyleIndices[num2]);
                        Game1.playSound("grassyStep");
                        break;
                    }
                case "Shirt":
                    Game1.player.rotateShirt(change, __instance.GetValidShirtIds());
                    Game1.playSound("coin");
                    break;
                case "Pants Style":
                    Game1.player.rotatePantStyle(change, __instance.GetValidPantsIds());
                    Game1.playSound("coin");
                    break;
                case "Acc":
                    Game1.player.changeAccessory((int)Game1.player.accessory + change);
                    Game1.playSound("purchase");
                    break;
                case "Direction":
                    ____displayFarmer.faceDirection((____displayFarmer.FacingDirection - change + 4) % 4);
                    ____displayFarmer.FarmerSprite.StopAnimation();
                    ____displayFarmer.completelyStopAnimatingOrDoingAction();
                    Game1.playSound("pickUpItem");
                    break;
                case "Cabins":
                    if ((Game1.startingCabins != 0 || change >= 0) && (Game1.startingCabins != Game1.Multiplayer.playerLimit - 1 || change <= 0))
                    {
                        Game1.playSound("axchop");
                    }

                    Game1.startingCabins += change;
                    Game1.startingCabins = Math.Max(0, Math.Min(Game1.Multiplayer.playerLimit - 1, Game1.startingCabins));
                    break;
                case "Difficulty":
                    if (Game1.player.difficultyModifier < 1f && change < 0)
                    {
                        Game1.playSound("breathout");
                        Game1.player.difficultyModifier += 0.25f;
                    }
                    else if (Game1.player.difficultyModifier > 0.25f && change > 0)
                    {
                        Game1.playSound("batFlap");
                        Game1.player.difficultyModifier -= 0.25f;
                    }

                    break;
                case "Wallets":
                    if ((bool)Game1.player.team.useSeparateWallets)
                    {
                        Game1.playSound("coin");
                        Game1.player.team.useSeparateWallets.Value = false;
                    }
                    else
                    {
                        Game1.playSound("coin");
                        Game1.player.team.useSeparateWallets.Value = true;
                    }

                    break;
                case "Pet":
                    {
                        var getPetTypesAndBreeds = AccessTools.Method(typeof(CharacterCustomization), "GetPetTypesAndBreeds");

                        List<KeyValuePair<string, string>> petTypesAndBreeds = (List<KeyValuePair<string, string>>)getPetTypesAndBreeds.Invoke(__instance, new object[0]);
                        int num = petTypesAndBreeds.IndexOf(new KeyValuePair<string, string>(Game1.player.whichPetType, Game1.player.whichPetBreed));
                        num = ((num != -1) ? (num + change) : 0);
                        if (num < 0)
                        {
                            num = petTypesAndBreeds.Count - 1;
                        }
                        else if (num >= petTypesAndBreeds.Count)
                        {
                            num = 0;
                        }

                        KeyValuePair<string, string> keyValuePair = petTypesAndBreeds[num];
                        Game1.player.whichPetType = keyValuePair.Key;
                        Game1.player.whichPetBreed = keyValuePair.Value;
                        Game1.playSound("coin");
                        break;
                    }
            }

            return false;
        }
    }
}
