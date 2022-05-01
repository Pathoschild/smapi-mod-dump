/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ShivaGuy/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewObject = StardewValley.Object;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace ShivaGuy.Stardew.FarmAnimalChoices
{
    public class ModEntry : Mod
    {
        private static readonly int MaxCols = 3;

        private static readonly Dictionary<string, string> FarmAnimalsData = Game1.content.Load<Dictionary<string, string>>("Data/FarmAnimals");

        private static readonly Texture2D WhiteChickenTexture = Game1.content.Load<Texture2D>("Animals/White Chicken");
        private static readonly Texture2D BrownChickenTexture = Game1.content.Load<Texture2D>("Animals/Brown Chicken");
        private static readonly Texture2D BlueChickenTexture = Game1.content.Load<Texture2D>("Animals/Blue Chicken");
        private static readonly Texture2D VoidChickenTexture = Game1.content.Load<Texture2D>("Animals/Void Chicken");
        private static readonly Texture2D GoldenChickenTexture = Game1.content.Load<Texture2D>("Animals/Golden Chicken");

        private static readonly Texture2D DuckTexture = Game1.content.Load<Texture2D>("Animals/Duck");

        private static readonly string CoopIsRequired = Game1.content.LoadString("Strings/StringsFromCSFiles:Utility.cs.5926");
        private static readonly string BigCoopIsRequired = Game1.content.LoadString("Strings/StringsFromCSFiles:Utility.cs.5940");
        private static readonly string BarnIsRequired = Game1.content.LoadString("Strings/StringsFromCSFiles:Utility.cs.5931");

        private static readonly string ChickenDescription = (
            Game1.content.LoadString("Strings/StringsFromCSFiles:PurchaseAnimalsMenu.cs.11334") +
            Environment.NewLine +
            Game1.content.LoadString("Strings/StringsFromCSFiles:PurchaseAnimalsMenu.cs.11335"));

        private static readonly string CowDescription = (
            Game1.content.LoadString("Strings/StringsFromCSFiles:PurchaseAnimalsMenu.cs.11343") +
            Environment.NewLine +
            Game1.content.LoadString("Strings/StringsFromCSFiles:PurchaseAnimalsMenu.cs.11344")
        );

        private static readonly string ItIsSecret = /* ...It's a secret. */
            Game1.content.LoadString("Characters/Dialogue/Abigail:winter_Sat6").Split("#$b#")[1].Split('$')[0];

        private static ModConfig Config { get; set; }

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Constructor(typeof(PurchaseAnimalsMenu), new Type[] { typeof(List<StardewObject>) }),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PurchaseAnimalsMenu__Prefix)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PurchaseAnimalsMenu__Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.getAnimalTitle)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PurchaseAnimalsMenu_getAnimalTitle__Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(PurchaseAnimalsMenu), nameof(PurchaseAnimalsMenu.getAnimalDescription)),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.PurchaseAnimalsMenu_getAnimalDescription__Prefix))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(FarmAnimal), new Type[] { typeof(string), typeof(long), typeof(long) }),
                prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.FarmAnimal__Prefix)),
                postfix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.FarmAnimal__Postfix))
            );

            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs evt)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu == null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.SetTitleScreenOnlyForNextOptions(ModManifest, true);

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Void Chicken Cost",
                tooltip: () => "Cost of Void Chicken in Marnie's shop",
                getValue: () => Config.VoidChicken,
                setValue: value => Config.VoidChicken = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Golden Chicken Cost",
                tooltip: () => "Cost of Golden Chicken in Marnie's shop",
                getValue: () => Config.GoldenChicken,
                setValue: value => Config.GoldenChicken = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Progression Mode",
                tooltip: () => "Disabling progression mode will unlock all chickens in the game. Keep it enabled to avoid any spoilers.",
                getValue: () => Config.ProgressionMode,
                setValue: value => Config.ProgressionMode = value
            );
        }

        private static int CalcLeftNeighborId(int id) { return (id % MaxCols) == 0 ? -1 : id - 1; }
        private static int CalcRightNeighborId(int id) { return (id % MaxCols) == (MaxCols - 1) ? -1 : id + 1; }
        private static int CalcTopNeighborId(int id) { return id - MaxCols; }
        private static int CalcBottomNeighborId(int id) { return id + MaxCols; }

        private static string GetAnimalName(string key)
        {
            FarmAnimalsData.TryGetValue(key, out string rawData);
            if (rawData != null)
            {
                return rawData.Split('/')[25];
            }
            return null;
        }

        public static void PurchaseAnimalsMenu__Prefix(List<StardewObject> stock)
        {
            int totalItems = stock.Count;
            foreach (var item in stock)
            {
                switch (item.Name)
                {
                    case "Chicken":
                        totalItems += 2; // brown, blue chicken
                        break;
                    case "Dairy Cow":
                        totalItems += 1; // brown cow
                        break;
                }
            }
            totalItems += 2; // void, golden chicken
            PurchaseAnimalsMenu.menuHeight = (totalItems / MaxCols) * 85 + 64;
        }

        public static void PurchaseAnimalsMenu__Postfix(PurchaseAnimalsMenu __instance)
        {
            List<ClickableTextureComponent> animalsToPurchase = new();
            List<Point> textureOffset = new();

            bool deluxeCoopConstructed = Game1.getFarm().isBuildingConstructed("Deluxe Coop");
            bool bigCoopConstructed = Game1.getFarm().isBuildingConstructed("Big Coop") || deluxeCoopConstructed;
            bool coopConstructed = Game1.getFarm().isBuildingConstructed("Coop") || bigCoopConstructed;

            bool deluxeBarnConstructed = Game1.getFarm().isBuildingConstructed("Deluxe Barn");
            bool bigBarnConstructed = Game1.getFarm().isBuildingConstructed("Big Barn") || deluxeBarnConstructed;
            bool barnConstructed = Game1.getFarm().isBuildingConstructed("Barn") || bigBarnConstructed;

            Rectangle calculatedLater = new Rectangle();
            bool unlockAll = !(Config?.ProgressionMode ?? true);

            for (int i = 0; i < __instance.animalsToPurchase.Count; i++)
            {
                var cc = __instance.animalsToPurchase[i];

                switch (cc.item.Name)
                {
                    case "Chicken":

                        // white chicken
                        StardewObject whiteChicken = new(100, 1, false, price: 400)
                        {
                            Name = "White Chicken",
                            Type = coopConstructed ? null : CoopIsRequired,
                            displayName = "White Chicken"
                        };
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            whiteChicken.salePrice().ToString(),
                            bounds: calculatedLater,
                            label: null,
                            hoverText: whiteChicken.Name,
                            texture: WhiteChickenTexture,
                            sourceRect: new Rectangle(0, 0, 16, 16),
                            scale: 4.0f,
                            whiteChicken.Type == null)
                        {
                            item = whiteChicken
                        });
                        textureOffset.Add(new Point(32, 0));

                        // brown chicken
                        StardewObject brownChicken = new(100, 1, false, price: 400)
                        {
                            Name = "Brown Chicken",
                            Type = coopConstructed ? null : CoopIsRequired,
                            displayName = "Brown Chicken"
                        };
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            brownChicken.salePrice().ToString(),
                            bounds: calculatedLater,
                            label: null,
                            hoverText: brownChicken.Name,
                            texture: BrownChickenTexture,
                            sourceRect: new Rectangle(0, 0, 16, 16),
                            scale: 4.0f,
                            brownChicken.Type == null)
                        {
                            item = brownChicken
                        });
                        textureOffset.Add(new Point(32, 0));

                        // blue chicken
                        StardewObject blueChicken = new(100, 1, false, price: 400)
                        {
                            Name = "Blue Chicken",
                            Type = coopConstructed && (unlockAll || Game1.player.eventsSeen.Contains(3900074)) // Shane's 8 Heart Event
                                    ? null : CoopIsRequired + Environment.NewLine + "& " + ItIsSecret,
                            displayName = "Blue Chicken"
                        };
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            blueChicken.salePrice().ToString(),
                            bounds: calculatedLater,
                            label: null,
                            hoverText: blueChicken.Name,
                            texture: BlueChickenTexture,
                            sourceRect: new Rectangle(0, 0, 16, 16),
                            scale: 4.0f,
                            blueChicken.Type == null)
                        {
                            item = blueChicken
                        });
                        textureOffset.Add(new Point(32, 0));
                        break;

                    case "Dairy Cow":

                        // white cow
                        StardewObject whiteCow = new(100, 1, false, 750)
                        {
                            Name = "White Cow",
                            Type = barnConstructed ? null : BarnIsRequired,
                            displayName = "White Cow"
                        };
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            whiteCow.salePrice().ToString(),
                            bounds: calculatedLater,
                            label: null,
                            hoverText: whiteCow.Name,
                            texture: Game1.mouseCursors,
                            sourceRect: new Rectangle(32, 448, 32, 16),
                            scale: 4.0f,
                            whiteCow.Type == null)
                        {
                            item = whiteCow
                        });
                        textureOffset.Add(new Point(0, 0));

                        // brown cow
                        StardewObject brownCow = new(100, 1, false, 750)
                        {
                            Name = "Brown Cow",
                            Type = barnConstructed ? null : BarnIsRequired,
                            displayName = "Brown Cow"
                        };
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            brownCow.salePrice().ToString(),
                            bounds: calculatedLater,
                            label: null,
                            hoverText: brownCow.Name,
                            texture: Game1.mouseCursors,
                            sourceRect: new Rectangle(64, 480, 32, 16),
                            scale: 4.0f,
                            brownCow.Type == null)
                        {
                            item = brownCow
                        });
                        textureOffset.Add(new Point(0, 0));
                        break;

                    case "Duck":

                        // duck (without egg)
                        StardewObject duck = new(100, 1, false, price: 600)
                        {
                            Name = "Duck",
                            Type = bigCoopConstructed ? null : BigCoopIsRequired,
                            displayName = "Duck"
                        };
                        animalsToPurchase.Add(new ClickableTextureComponent(
                            duck.salePrice().ToString(),
                            bounds: calculatedLater,
                            label: null,
                            hoverText: duck.Name,
                            texture: DuckTexture,
                            sourceRect: new Rectangle(0, 0, 16, 16),
                            scale: 4.0f,
                            duck.Type == null)
                        {
                            item = duck
                        });
                        textureOffset.Add(new Point(32, 0));
                        break;

                    default:
                        animalsToPurchase.Add(cc);
                        textureOffset.Add(new Point(0, 0));
                        break;
                }
            }

            // void chicken
            StardewObject void_chicken = new(100, 1, false, price: Math.Max(0, Config?.VoidChicken / 2 ?? 0))
            {
                Name = "Void Chicken",
                Type = bigCoopConstructed && (unlockAll || Game1.player.mailReceived.Contains("OpenedSewer")) // Sewer unlocked
                        ? null : BigCoopIsRequired + Environment.NewLine + "& " + ItIsSecret,
                displayName = "Void Chicken"
            };
            animalsToPurchase.Add(new ClickableTextureComponent(
                void_chicken.salePrice().ToString(),
                bounds: calculatedLater,
                label: null,
                hoverText: void_chicken.Name,
                texture: VoidChickenTexture,
                sourceRect: new Rectangle(0, 0, 16, 16),
                scale: 4.0f,
                void_chicken.Type == null)
            {
                item = void_chicken
            });
            textureOffset.Add(new Point(32, 0));

            // golden chicken
            StardewObject golden_chicken = new(100, 1, false, price: Math.Max(0, Config?.GoldenChicken / 2 ?? 0))
            {
                Name = "Golden Chicken",
                Type = bigCoopConstructed && (unlockAll || Game1.player.mailReceived.Contains("Farm_Eternal")) // Perfection
                        ? null : BigCoopIsRequired + Environment.NewLine + "& " + ItIsSecret,
                displayName = "Golden Chicken"
            };
            animalsToPurchase.Add(new ClickableTextureComponent(
                golden_chicken.salePrice().ToString(),
                bounds: calculatedLater,
                label: null,
                hoverText: golden_chicken.Name,
                texture: GoldenChickenTexture,
                sourceRect: new Rectangle(0, 0, 16, 16),
                scale: 4.0f,
                golden_chicken.Type == null)
            {
                item = golden_chicken
            });
            textureOffset.Add(new Point(32, 0));

            for (int i = 0; i < animalsToPurchase.Count; i++)
            {
                animalsToPurchase[i].bounds = new Rectangle(
                    __instance.xPositionOnScreen + IClickableMenu.borderWidth + ((i % MaxCols) * 128),
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + (IClickableMenu.borderWidth / 2) + (i / MaxCols) * 85,
                    128,
                    64);
                animalsToPurchase[i].bounds.Offset(textureOffset[i]);

                animalsToPurchase[i].myID = i;
                animalsToPurchase[i].leftNeighborID = CalcLeftNeighborId(i);
                animalsToPurchase[i].rightNeighborID = CalcRightNeighborId(i);
                animalsToPurchase[i].upNeighborID = CalcTopNeighborId(i);
                animalsToPurchase[i].downNeighborID = CalcBottomNeighborId(i);
            }

            __instance.animalsToPurchase.Clear();
            animalsToPurchase.ForEach(item => __instance.animalsToPurchase.Add(item));
        }

        public static bool PurchaseAnimalsMenu_getAnimalTitle__Prefix(string name, ref string __result)
        {
            if (!name.EndsWith(" Chicken") && !name.EndsWith(" Cow"))
                return true;

            __result = GetAnimalName(name) ?? name;
            return false;
        }

        public static bool PurchaseAnimalsMenu_getAnimalDescription__Prefix(string name, ref string __result)
        {
            if (name.EndsWith(" Chicken"))
                __result = ChickenDescription;
            else if (name.EndsWith(" Cow"))
                __result = CowDescription;
            else
                return true;

            return false;
        }

        public static void FarmAnimal__Prefix(string type, out string __state)
        {
            __state = type;
        }
        public static void FarmAnimal__Postfix(string type, FarmAnimal __instance, string __state)
        {
            if (__state == type || (!type.EndsWith(" Chicken") && !type.EndsWith(" Cow")))
                return;

            __instance.type.Value = __state;
            __instance.reloadData();
        }
    }
}
