/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ShivaGuy/StardewMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using SDV_Object = StardewValley.Object;

namespace ShivaGuy.Stardew.FarmAnimalChoices.Patch
{
    internal static class PurchaseAnimalsMenuPatch
    {
        private static int MaxCols = 3;

        private static readonly Texture2D ChickenTexture_White = Game1.content.Load<Texture2D>("Animals/White Chicken");
        private static readonly Texture2D ChickenTexture_Brown = Game1.content.Load<Texture2D>("Animals/Brown Chicken");
        private static readonly Texture2D ChickenTexture_Blue = Game1.content.Load<Texture2D>("Animals/Blue Chicken");
        private static readonly Texture2D ChickenTexture_Void = Game1.content.Load<Texture2D>("Animals/Void Chicken");
        private static readonly Texture2D ChickenTexture_Golden = Game1.content.Load<Texture2D>("Animals/Golden Chicken");
        private static readonly Texture2D DuckTexture = Game1.content.Load<Texture2D>("Animals/Duck");

        private static readonly string Required_Coop = Game1.content.LoadString("Strings/StringsFromCSFiles:Utility.cs.5926");
        private static readonly string Required_Barn = Game1.content.LoadString("Strings/StringsFromCSFiles:Utility.cs.5931");
        private static readonly string Required_BigCoop = Game1.content.LoadString("Strings/StringsFromCSFiles:Utility.cs.5940");

        private static readonly string Description_Chicken = Game1.content.LoadString("Strings/StringsFromCSFiles:PurchaseAnimalsMenu.cs.11334") + Environment.NewLine + Game1.content.LoadString("Strings/StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335");
        private static readonly string Description_Cow = Game1.content.LoadString("Strings/StringsFromCSFiles:PurchaseAnimalsMenu.cs.11343") + Environment.NewLine + Game1.content.LoadString("Strings/StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344");
        private static readonly string Description_Secret = Game1.content.LoadString("Characters/Dialogue/Abigail:winter_Sat6").Split("#$b#")[1].Split('$')[0];

        private static readonly Dictionary<string, string> FarmAnimalsData = Game1.content.Load<Dictionary<string, string>>("Data/FarmAnimals");

        private static bool Has_Coop { get { return Game1.getFarm().isBuildingConstructed("Coop") || Has_BigCoop; } }
        private static bool Has_BigCoop { get { return Game1.getFarm().isBuildingConstructed("Big Coop") || Has_DeluxeCoop; } }
        private static bool Has_DeluxeCoop { get { return Game1.getFarm().isBuildingConstructed("Deluxe Coop"); } }

        private static bool Has_Barn { get { return Game1.getFarm().isBuildingConstructed("Barn") || Has_BigBarn; } }
        private static bool Has_BigBarn { get { return Game1.getFarm().isBuildingConstructed("Big Barn") || Has_DeluxeBarn; } }
        private static bool Has_DeluxeBarn { get { return Game1.getFarm().isBuildingConstructed("Deluxe Barn"); } }

        private static bool Disabled_ProgressionMode { get { return !ModEntry.Config?.ProgressionMode ?? false; } }
        private static bool Seen_Shane8HeartEvent { get { return Game1.player.eventsSeen.Contains(3900074); } }
        private static bool Unlocked_Sewers { get { return Game1.player.mailReceived.Contains("OpenedSewer"); } }
        private static bool Achieved_Perfection { get { return Game1.player.mailReceived.Contains("Farm_Eternal"); } }

        public static void ApplyPatch(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Constructor(typeof(PurchaseAnimalsMenu), new Type[] { typeof(List<SDV_Object>) }),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(PurchaseAnimalsMenuPatch), nameof(ctor_Prefix)), before: new string[] { "aedenthorn.BulkAnimalPurchase" }),
                postfix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(ctor_Postfix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.getAnimalTitle)),
                prefix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(getAnimalTitle_Prefix)));

            harmony.Patch(
                original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.getAnimalDescription)),
                prefix: new HarmonyMethod(typeof(PurchaseAnimalsMenuPatch), nameof(getAnimalDescription_Prefix)));
        }

        private static string AnimalDisplayName(string name)
        {
            var data = FarmAnimalsData.GetValueOrDefault(name, name);
            return data.Equals(name) ? name : data.Split('/')[25];
        }

        public static void ctor_Prefix(List<SDV_Object> stock)
        {
            PurchaseAnimalsMenu.menuHeight = ((stock.Count + 5) / MaxCols) * 85 + 64;
        }

        public static void ctor_Postfix(PurchaseAnimalsMenu __instance)
        {
            var animalsToPurchase = new List<ClickableTextureComponent>();
            var offsetFromDefault = new List<Point>();

            string name, unlocked, placeholder_price = "0";
            Rectangle placeholder_bounds = Rectangle.Empty;

            for (int i = 0; i < __instance.animalsToPurchase.Count; i++)
            {
                var cc = __instance.animalsToPurchase[i];

                switch (cc.item.Name)
                {
                    case "Chicken":

                        name = "White Chicken";
                        unlocked = Has_Coop ? null : Required_Coop;
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            name: placeholder_price,
                            bounds: placeholder_bounds,
                            label: null,
                            hoverText: name,
                            texture: ChickenTexture_White,
                            sourceRect: new Rectangle(0, 0, 16, 16),
                            scale: 4.0f,
                            unlocked == null)
                        {
                            item = new SDV_Object(100, 1, false, price: 400)
                            {
                                Name = name,
                                Type = unlocked,
                                displayName = name
                            }
                        });
                        offsetFromDefault.Add(new Point(32, 0));

                        name = "Brown Chicken";
                        unlocked = Has_Coop ? null : Required_Coop;
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            name: placeholder_price,
                            bounds: placeholder_bounds,
                            label: null,
                            hoverText: name,
                            texture: ChickenTexture_Brown,
                            sourceRect: new Rectangle(0, 0, 16, 16),
                            scale: 4.0f,
                            unlocked == null)
                        {
                            item = new SDV_Object(100, 1, false, price: 400)
                            {
                                Name = name,
                                Type = unlocked,
                                displayName = name
                            }
                        });
                        offsetFromDefault.Add(new Point(32, 0));

                        name = "Blue Chicken";
                        unlocked = Has_Coop && (Disabled_ProgressionMode || Seen_Shane8HeartEvent) ? null : Required_Coop + Environment.NewLine + "& " + Description_Secret;
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            name: placeholder_price,
                            bounds: placeholder_bounds,
                            label: null,
                            hoverText: name,
                            texture: ChickenTexture_Blue,
                            sourceRect: new Rectangle(0, 0, 16, 16),
                            scale: 4.0f,
                            unlocked == null)
                        {
                            item = new SDV_Object(100, 1, false, price: 400)
                            {
                                Name = name,
                                Type = unlocked,
                                displayName = name
                            }
                        });
                        offsetFromDefault.Add(new Point(32, 0));
                        break;

                    case "Dairy Cow":

                        name = "White Cow";
                        unlocked = Has_Barn ? null : Required_Barn;
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            name: placeholder_price,
                            bounds: placeholder_bounds,
                            label: null,
                            hoverText: name,
                            texture: Game1.mouseCursors,
                            sourceRect: new Rectangle(32, 448, 32, 16),
                            scale: 4.0f,
                            unlocked == null)
                        {
                            item = new SDV_Object(100, 1, false, price: 750)
                            {
                                Name = name,
                                Type = unlocked,
                                displayName = name
                            }
                        });
                        offsetFromDefault.Add(new Point(0, 0));

                        name = "Brown Cow";
                        unlocked = Has_Barn ? null : Required_Barn;
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            name: placeholder_price,
                            bounds: placeholder_bounds,
                            label: null,
                            hoverText: name,
                            texture: Game1.mouseCursors,
                            sourceRect: new Rectangle(64, 480, 32, 16),
                            scale: 4.0f,
                            unlocked == null)
                        {
                            item = new SDV_Object(100, 1, false, price: 750)
                            {
                                Name = name,
                                Type = unlocked,
                                displayName = name
                            }
                        });
                        offsetFromDefault.Add(new Point(0, 0));
                        break;

                    case "Duck":

                        // duck (without egg)
                        name = cc.item.Name;
                        unlocked = Has_BigCoop ? null : Required_BigCoop;
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            name: placeholder_price,
                            bounds: placeholder_bounds,
                            label: null,
                            hoverText: name,
                            texture: DuckTexture,
                            sourceRect: new Rectangle(0, 0, 16, 16),
                            scale: 4.0f,
                            unlocked == null)
                        {
                            item = new SDV_Object(100, 1, false, price: 600)
                            {
                                Name = name,
                                Type = unlocked,
                                displayName = name
                            }
                        });
                        offsetFromDefault.Add(new Point(32, 0));
                        break;

                    default:
                        animalsToPurchase.Add(cc);
                        offsetFromDefault.Add(new Point(0, 0));
                        break;
                }
            }

            name = "Void Chicken";
            unlocked = Has_BigCoop && (Disabled_ProgressionMode || Unlocked_Sewers) ? null : Required_BigCoop + Environment.NewLine + "& " + Description_Secret;
            animalsToPurchase.Add(new ClickableTextureComponent(
                name: placeholder_price,
                bounds: placeholder_bounds,
                label: null,
                hoverText: name,
                texture: ChickenTexture_Void,
                sourceRect: new Rectangle(0, 0, 16, 16),
                scale: 4.0f,
                unlocked == null)
            {
                item = new SDV_Object(100, 1, false, price: Math.Max(0, ModEntry.Config?.VoidChicken / 2 ?? 0))
                {
                    Name = name,
                    Type = unlocked,
                    displayName = name
                }
            });
            offsetFromDefault.Add(new Point(32, 0));

            name = "Golden Chicken";
            unlocked = Has_BigCoop && (Disabled_ProgressionMode || Achieved_Perfection) ? null : Required_BigCoop + Environment.NewLine + "& " + Description_Secret;
            animalsToPurchase.Add(new ClickableTextureComponent(
                name: placeholder_price,
                bounds: placeholder_bounds,
                label: null,
                hoverText: name,
                texture: ChickenTexture_Golden,
                sourceRect: new Rectangle(0, 0, 16, 16),
                scale: 4.0f,
                unlocked == null)
            {
                item = new SDV_Object(100, 1, false, price: Math.Max(0, ModEntry.Config?.GoldenChicken / 2 ?? 0))
                {
                    Name = name,
                    Type = unlocked,
                    displayName = name
                }
            });
            offsetFromDefault.Add(new Point(32, 0));

            for (int i = 0; i < animalsToPurchase.Count; i++)
            {
                animalsToPurchase[i].name = animalsToPurchase[i].item.salePrice().ToString();
                animalsToPurchase[i].bounds = new Rectangle(
                    __instance.xPositionOnScreen + IClickableMenu.borderWidth + ((i % MaxCols) * 128),
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (IClickableMenu.borderWidth / 2) + (i / MaxCols) * 85,
                    128,
                    64);
                animalsToPurchase[i].bounds.Offset(offsetFromDefault[i]);

                animalsToPurchase[i].myID = i;
                animalsToPurchase[i].leftNeighborID = (i % MaxCols) == 0 ? -1 : i - 1;
                animalsToPurchase[i].rightNeighborID = (i % MaxCols) == (MaxCols - 1) ? -1 : i + 1;
                animalsToPurchase[i].upNeighborID = i - MaxCols;
                animalsToPurchase[i].downNeighborID = i - MaxCols;
            }

            __instance.animalsToPurchase.Clear();
            animalsToPurchase.ForEach(item => __instance.animalsToPurchase.Add(item));
        }

        public static bool getAnimalTitle_Prefix(string name, ref string __result)
        {
            if (!name.EndsWith(" Chicken") && !name.EndsWith(" Cow"))
                return true;

            __result = AnimalDisplayName(name);
            return false;
        }

        public static bool getAnimalDescription_Prefix(string name, ref string __result)
        {
            if (name.EndsWith(" Chicken"))
                __result = Description_Chicken;
            else if (name.EndsWith(" Cow"))
                __result = Description_Cow;
            else
                return true;

            return false;
        }
    }
}
