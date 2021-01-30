/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using CombineMachines.Patches;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace CombineMachines.Helpers
{
    public static class Extensions
    {
        /// <summary>Returns true if the output stack has already been modified during the current processing cycle, and should not be changed again until the next processing cycle</summary>
        public static bool HasModifiedOutput(this SObject Item)
        {
            if (Item.modData != null && Item.modData.TryGetValue(ModEntry.ModDataOutputModifiedKey, out string HasModifiedString))
            {
                bool Result = bool.Parse(HasModifiedString);
                return Result;
            }
            else
                return false;
        }

        public static void SetHasModifiedOutput(this SObject Item, bool Value)
        {
            Item.modData[ModEntry.ModDataOutputModifiedKey] = Value.ToString();
        }

        public static bool IsCombinedMachine(this SObject Item)
        {
            return Item.modData != null && Item.modData.TryGetValue(ModEntry.ModDataQuantityKey, out _);
        }

        public static bool TryGetCombinedQuantity(this SObject Item, out int Quantity)
        {
            Quantity = 0;

            if (Item.modData != null && Item.modData.TryGetValue(ModEntry.ModDataQuantityKey, out string QuantityString))
            {
                Quantity = int.Parse(QuantityString);
                return true;
            }
            else
                return false;
        }

        public static void SetCombinedQuantity(this SObject Item, int Quantity)
        {
            if (!IsCombinableObject(Item))
                throw new InvalidOperationException("Only machines can be combined in mod: ." + nameof(CombineMachines));

            if (!TryGetCombinedQuantity(Item, out int PreviousValue))
                PreviousValue = 0;

            Item.modData[ModEntry.ModDataQuantityKey] = Quantity.ToString();
            int PreviousStack = Item.Stack;
            Item.Stack = 1;
            ModEntry.Logger.Log(string.Format("Set combined quantity on {0} (Stack={1}) from {2} to {3}", Item.DisplayName, PreviousStack, PreviousValue, Quantity), ModEntry.InfoLogLevel);
        }

        public static bool TryGetProcessingInterval(this CrabPot Item, out double Power, out double IntervalDecimalHours, out int IntervalMinutes)
        {
            if (Item.TryGetCombinedQuantity(out int Quantity))
            {
                Power = ModEntry.UserConfig.ComputeProcessingPower(Quantity);

                IntervalDecimalHours = MinutesElapsedPatch.CrabPotHoursPerDay / Power;
                IntervalMinutes = (int)(IntervalDecimalHours * MinutesElapsedPatch.MinutesPerHour);

                //  Round the minutes up to the nearest multiple of 10
                if (IntervalMinutes % 10 != 0)
                {
                    IntervalMinutes += 10 - IntervalMinutes % 10;
                    IntervalDecimalHours = IntervalMinutes * 1.0 / MinutesElapsedPatch.MinutesPerHour;
                }

                return true;
            }
            else
            {
                Power = 1.0;
                IntervalDecimalHours = -1;
                IntervalMinutes = -1;
                return false;
            }
        }

        public static double GetProcessingPower(this SObject Obj)
        {
            if (Obj.TryGetCombinedQuantity(out int Quantity))
            {
                double Power = ModEntry.UserConfig.ComputeProcessingPower(Quantity);
                return Power;
            }
            else
                return 1.0;
        }

        /// <summary>The item Ids of machines that are just regular objects, rather than BigCraftable item types.</summary>
        public static readonly ReadOnlyCollection<int> NonBigCraftableMachineIds = new List<int>() {
            710 // Crab Pot
        }.AsReadOnly();

        /// <summary>The item Ids of BigCraftables objects that are not machines.</summary>
        public static readonly ReadOnlyCollection<int> NonMachineBigCraftableIds = new List<int>()
        {
            //https://stardewcommunitywiki.com/Modding:Big_craftables_data
            0, 1, 2, 3, 4, 5, 6, 7, // House Plants
            8, // Scarecrow
            22, 23, // Table Pieces L/R
            26, 27, // Wood Chair
            28, // Skeleton Model
            29, // Obelisk
            31, // Chicken Statue
            32, // Stone Cairn
            33, // Suit of Armor
            34, // Sign of the Vessel
            35, // Basic Log
            36, // Lawn Flamingo
            37, 38, 39, // Wood/Stone/Dark Signs
            40, 41, 42, 43, 44, // Big Green Cane, Green Canes, Mixed Cane, Red Canes, Big Red Cane
            45, // Ornamental Hay Bale
            46, // Log Section
            47, // Grave Stone
            48, // Seasonal Decor
            52, 53, 54, 55, // Stone Frog/Parrot/Owl/Junimo
            56, // Slime Ball
            62, // Garden Pot
            64, // Bookcase
            65, 66, 67, // Fancy Table, Ancient Table, Ancient Stool
            68, // Grandfather clock
            69, // Teddy Timer
            70, // Dead tree
            71, // Staircase
            72, // Tall Torch
            73, // Ritual Mask
            74, // Bonfire
            75, // Bongo
            76, // Decorative Spears
            78, // Boulder
            79, 80, 81, 82, // Door / Locked Door
            83, 84, // Wicked Statue
            85, 86, 87, // Sloth Skeleton L/M/R
            88, // Standing Geode
            89, // Obsidian Vase
            94, // Singing Stone
            95, // Stone Owl
            96, 98, // Strange / Empty Capsules
            99, // Feed Hopper
            104, // Heater
            106, // Camera
            107, // Plush Bunny
            108, 109, // Tub o' flowers
            110, 113, 126, 136, 137, 138, 139, 140, // Rarecrows
            111, // Decorative Pitcher
            112, // Dried sunflowers
            116, //Stardew Hero Trophy
            118, 119, 120, 121, 122, 123, 124, 125, 174, 175, // Create / Barrel
            130, 232, 256, // Chest, Stone Chest, Junimo Chest
            141, // Prairie King Arcade System
            143, 144, 145, 147, 148, 149, 150, 151, // Wooden/Stone/Gold/Stump/Carved/Skull/Barrel/Marble Brazier
            146, // Campfire
            152, 153, // Wood/Iron Lamp-posts
            155, 161, 162, // ???
            159, // Junimo Kart Arcade System
            164, // Solid Gold Lewis
            165, // Auto-grabber
            167, // Deluxe Scarecrow
            184, 188, 192, 196, 200, 204, // Seasonal Plant
            208, // Workbench
            209, // Mini-jukebox
            214, // Telephone
            216, // Mino-Fridge
            219, // Cursed P.K. Arcade System
            238, // Mini-Obelisk
            239, // Farm Computer
            247, // Sewing Machine
            248, // Mini-Shipping Bin
            272, // Auto-Petter
            275, // Hopper
            278 // Campfire
        }.AsReadOnly();

        public static ReadOnlyCollection<int> ScarecrowIds = new List<int>()
        {
            8, // Scarecrow
            110, 113, 126, 136, 137, 138, 139, 140, // Rarecrows
            167 // Deluxe Scarecrow
        }.AsReadOnly();

        public static bool IsScarecrow(this SObject Obj)
        {
            return Obj != null && Obj.bigCraftable.Value && ScarecrowIds.Contains(Obj.ParentSheetIndex);
        }

        private static readonly ReadOnlyCollection<int> OreIds = new List<int>() {
            378, 380, 384, 386, 909 // Copper Ore, Iron Ore, Gold Ore, Iridium Ore, Radioactive Ore
        }.AsReadOnly();
        public static bool IsOre(this Item Item)
        {
            return Item != null && OreIds.Contains(Item.ParentSheetIndex) && Item is SObject Obj && !Obj.GetType().IsSubclassOf(typeof(SObject)) && !Obj.IsRecipe && !Obj.bigCraftable.Value;
        }
        public const int CoalId = 382;
        public static bool IsCoal(this Item Item)
        {
            return Item != null && Item.ParentSheetIndex == CoalId && Item is SObject Obj && !Obj.GetType().IsSubclassOf(typeof(SObject)) && !Obj.IsRecipe && !Obj.bigCraftable.Value;
        }

        public static bool IsCombinableObject(this SObject Item)
        {
            return 
                (Item.bigCraftable.Value && !NonMachineBigCraftableIds.Contains(Item.ParentSheetIndex)) || // All BigCraftable Machines, such as Kegs, Mayonnaise Machines, Tappers, Furnaces etc
                NonBigCraftableMachineIds.Contains(Item.ParentSheetIndex) || // All non-BigCraftable Machines, such as Crab Pots
                IsScarecrow(Item); // Scarecrow, Rarecrows, and Deluxe Scarecrow
        }

        //Taken from: https://stackoverflow.com/questions/521146/c-sharp-split-string-but-keep-split-chars-separators
        public static IEnumerable<string> SplitAndKeepDelimiter(this string s, char[] delims)
        {
            int start = 0, index;

            while ((index = s.IndexOfAny(delims, start)) != -1)
            {
                if (index - start > 0)
                    yield return s.Substring(start, index - start);
                yield return s.Substring(index, 1);
                start = index + 1;
            }

            if (start < s.Length)
            {
                yield return s.Substring(start);
            }
        }

        public static Rectangle GetOffseted(this Rectangle value, Point Offset)
        {
            return new Rectangle(value.X + Offset.X, value.Y + Offset.Y, value.Width, value.Height);
        }

        public static Point AsPoint(this Vector2 value)
        {
            return new Point((int)value.X, (int)value.Y);
        }

        /// <summary>Returns <see cref="ICursorPosition.ScreenPixels"/>, always adjusted for UI Scaling. SMAPI version 3.8.1 and earlier used to always do this, but changes were made to SMAPI 3.8.2+.</summary>
        public static Vector2 LegacyScreenPixels(this ICursorPosition value)
        {
            if (!Constants.ApiVersion.IsNewerThan("3.8.1"))
                return value.ScreenPixels;
            else
                return Utility.ModifyCoordinatesForUIScale(value.ScreenPixels);
        }
    }
}
