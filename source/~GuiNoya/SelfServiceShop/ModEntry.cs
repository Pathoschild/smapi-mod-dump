using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using System.Collections.Generic;
using xTile.Dimensions;

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CC0021 // Use nameof
#pragma warning disable CC0091 // Use static method

namespace SelfServiceShop
{
    // ReSharper disable once UnusedMember.Global
    public class ModEntry : Mod
    {
        private static readonly Texture2D PortraitRobin = Game1.content.Load<Texture2D>("Portraits\\Robin");
        private static readonly Texture2D PortraitMarnie = Game1.content.Load<Texture2D>("Portraits\\Marnie");
        private ModConfig _config;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonPressed += Input_ButtonPressed;
        }

        private void Input_ButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsPlayerFree || Game1.activeClickableMenu != null ||
                !e.Button.IsActionButton() && e.Button != SButton.ControllerA)
                return;

            var property = Game1.currentLocation.doesTileHaveProperty((int)e.Cursor.GrabTile.X,
                (int)e.Cursor.GrabTile.Y, "Action", "Buildings");

            switch (property)
            {
                case "Buy General":
                    if (_config.Pierre && (_config.ShopsAlwaysOpen || IsNpcInLocation("Pierre")))
                    {
                        Helper.Input.Suppress(e.Button);
                        Game1.activeClickableMenu =
                            new ShopMenu(((SeedShop)Game1.currentLocation).shopStock(), 0, "Pierre");
                    }
                    break;

                case "Carpenter":
                    if (_config.Carpenter)
                    {
                        if (_config.ShopsAlwaysOpen)
                        {
                            Carpenters();
                        }
                        else
                        {
                            NPC robin;
                            if (Game1.currentLocation.characters.Find("Robin") is NPC npc)
                                robin = npc;
                            else
                                break;
                            var carpenters = Helper.Reflection.GetMethod(Game1.currentLocation, "carpenters");
                            var tileLocation = robin.getTileLocation();
                            carpenters.Invoke(new Location((int)tileLocation.X, (int)tileLocation.Y));
                        }
                        Helper.Input.Suppress(e.Button);
                    }
                    break;

                case "AnimalShop":
                    if (_config.Ranch)
                    {
                        if (_config.ShopsAlwaysOpen)
                        {
                            AnimalShop();
                        }
                        else
                        {
                            NPC marnie;
                            if (Game1.currentLocation.characters.Find("Marnie") is NPC npc)
                                marnie = npc;
                            else
                                break;
                            var animalShop = Helper.Reflection.GetMethod(Game1.currentLocation, "animalShop");
                            var tileLocation = marnie.getTileLocation();
                            animalShop.Invoke(new Location((int)tileLocation.X, (int)tileLocation.Y + 1));
                        }
                        Helper.Input.Suppress(e.Button);
                    }
                    break;

                case "Buy Fish":
                    if (_config.FishShop &&
                        (_config.ShopsAlwaysOpen || IsNpcInLocation("Willy") || IsNpcInLocation("Willy", "Beach")))
                    {
                        Helper.Input.Suppress(e.Button);
                        Game1.activeClickableMenu = new ShopMenu(Utility.getFishShopStock(Game1.player), 0, "Willy");
                    }
                    break;

                case "Blacksmith":
                    if (_config.Blacksmith)
                    {
                        if (_config.ShopsAlwaysOpen)
                        {
                            Blacksmith(Game1.getCharacterFromName("Clint"));
                        }
                        else
                        {
                            NPC clint;
                            if (Game1.currentLocation.characters.Find("Clint") is NPC npc)
                                clint = npc;
                            else
                                break;
                            Blacksmith(clint);
                        }
                        Helper.Input.Suppress(e.Button);
                    }
                    break;

                case "IceCreamStand":
                    if (_config.IceCreamStand &&
                        (_config.ShopsAlwaysOpen || _config.IceCreamInAllSeasons || SDate.Now().Season == "summer"))
                    {
                        var d = new Dictionary<Item, int[]>
                        {
                            {new Object(233, 1), new[] {250, int.MaxValue}}
                        };
                        Game1.activeClickableMenu = new ShopMenu(d);
                        Helper.Input.Suppress(e.Button);
                    }
                    break;

                default:
                    break;
            }
        }

        private bool IsNpcInLocation(string name, string locationName = "")
        {
            GameLocation location;
            location = locationName.Length == 0 ? Game1.currentLocation : Game1.locations.Find(locationName);

            return location.characters.Find(name) != null;
        }

        private void Carpenters()
        {
            if (Game1.player.daysUntilHouseUpgrade.Value < 0 && !Game1.getFarm().isThereABuildingUnderConstruction() &&
                Game1.player.currentUpgrade == null)
            {
                var responseList = new List<Response>(4)
                {
                    new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop"))
                };
                if (Game1.IsMasterGame)
                {
                    if (Game1.player.HouseUpgradeLevel < 3)
                    {
                        responseList.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")));
                    }
                    else if ((Game1.MasterPlayer.mailReceived.Contains("ccIsComplete") || Game1.MasterPlayer.mailReceived.Contains("JojaMember") || Game1.MasterPlayer.hasCompletedCommunityCenter())
                        && ((Game1.getLocationFromName("Town") as Town).daysUntilCommunityUpgrade.Value <= 0 && !Game1.MasterPlayer.mailReceived.Contains("pamHouseUpgrade")))
                    {
                        responseList.Add(new Response("CommunityUpgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_CommunityUpgrade")));
                    }
                }
                else if (Game1.player.HouseUpgradeLevel < 3)
                {
                    responseList.Add(new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeCabin")));
                }
                responseList.Add(new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")));
                responseList.Add(new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave")));
                Game1.currentLocation.createQuestionDialogue(
                    Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu"), responseList.ToArray(),
                    "carpenter");
                return;
            }
            Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
        }

        private void AnimalShop()
        {
            Game1.currentLocation.createQuestionDialogue("", new[]
            {
                new Response("Supplies", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Supplies")),
                new Response("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
                new Response("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
            }, "Marnie");
        }

        private void Blacksmith(NPC clint)
        {
            if (Game1.player.toolBeingUpgraded.Value == null)
            {
                Response[] answerChoices;
                answerChoices = Game1.player.hasItemInInventory(535, 1) || Game1.player.hasItemInInventory(536, 1) ||
                    Game1.player.hasItemInInventory(537, 1) || Game1.player.hasItemInInventory(749, 1) ? new[]
                    {
                        new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
                        new Response("Upgrade",
                            Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")),
                        new Response("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")),
                        new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
                    } : new[]
                    {
                        new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
                        new Response("Upgrade",
                            Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Upgrade")),
                        new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
                    };
                Game1.currentLocation.createQuestionDialogue("", answerChoices, "Blacksmith");
                return;
            }

            if (Game1.player.daysLeftForToolUpgrade.Value <= 0)
            {
                if (Game1.player.freeSpotsInInventory() > 0)
                {
                    Game1.player.holdUpItemThenMessage(Game1.player.toolBeingUpgraded.Value);
                    Game1.player.addItemToInventoryBool(Game1.player.toolBeingUpgraded.Value);
                    Game1.player.toolBeingUpgraded.Value = null;
                    return;
                }
                Game1.drawDialogue(clint, Game1.content.LoadString("Data\\ExtraDialogue:Clint_NoInventorySpace"));
                return;
            }
            Game1.drawDialogue(clint,
                Game1.content.LoadString("Data\\ExtraDialogue:Clint_StillWorking",
                    Game1.player.toolBeingUpgraded.Value.DisplayName));
            Helper.Events.Display.MenuChanged += Display_MenuChanged;
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu != null) return;

            if (Game1.player.hasItemInInventory(535, 1) || Game1.player.hasItemInInventory(536, 1) ||
                Game1.player.hasItemInInventory(537, 1) || Game1.player.hasItemInInventory(749, 1))
            {
                var answerChoices = new[]
                {
                    new Response("Shop", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Shop")),
                    new Response("Process", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Geodes")),
                    new Response("Leave", Game1.content.LoadString("Strings\\Locations:Blacksmith_Clint_Leave"))
                };
                Game1.currentLocation.createQuestionDialogue("", answerChoices, "Blacksmith");
            }
            else
            {
                Game1.activeClickableMenu = new ShopMenu(Utility.getBlacksmithStock(), 0, "Clint");
            }
            Helper.Events.Display.MenuChanged -= Display_MenuChanged;
        }
    }
}

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore CC0091 // Use static method
#pragma warning restore CC0021 // Use nameof