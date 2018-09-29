using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace SelfServe
{
    public class ModEntry : Mod
    {
        private List<Vector2> seedShopCounterTiles;
        private List<Vector2> animalShopCounterTiles;
        private List<Vector2> carpentersShopCounterTiles;
        private List<Vector2> fishShopShopCounterTiles;
        private Dictionary<String, NPC> npcRefs;

        private ITranslationHelper i18n;

        private bool inited = false;

        private void OnLoad(object Sender, EventArgs e)
        {
            // params reset
            seedShopCounterTiles = new List<Vector2>();
            animalShopCounterTiles = new List<Vector2>();
            carpentersShopCounterTiles = new List<Vector2>();
            fishShopShopCounterTiles = new List<Vector2>();

            npcRefs = new Dictionary<string, NPC>();
            inited = false;

            // params setup
            seedShopCounterTiles.Add(new Vector2(4f, 19f));
            seedShopCounterTiles.Add(new Vector2(5f, 19f));

            animalShopCounterTiles.Add(new Vector2(12f, 16f));
            animalShopCounterTiles.Add(new Vector2(13f, 16f));

            carpentersShopCounterTiles.Add(new Vector2(8f, 20f));

            fishShopShopCounterTiles.Add(new Vector2(4f, 6f));
            fishShopShopCounterTiles.Add(new Vector2(5f, 6f));
            fishShopShopCounterTiles.Add(new Vector2(6f, 6f));

            foreach (NPC npc in Utility.getAllCharacters())
            {
                switch (npc.Name)
                {
                    case "Pierre":
                    case "Robin":
                    case "Marnie":
                    case "Willy":
                       npcRefs[npc.Name] = npc;
                        break;
                }
            }

            foreach(var item in npcRefs)
            {
                Monitor.Log(item.ToString());
            }

            // done

            this.inited = true;
        }

        private void OnExit(object Sender, EventArgs e)
        {
            inited = false;
        }

        public override void Entry(IModHelper helper)
        {
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            SaveEvents.AfterLoad += this.OnLoad;
            SaveEvents.AfterReturnToTitle += this.OnExit;

            i18n = helper.Translation;
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (this.inited && this.OpenMenuHandler(e.IsActionButton))
                e.SuppressButton();
        }

        private bool OpenMenuHandler(bool isActionKey)
        {
            // returns true if menu is opened, otherwise false

            String locationString = Game1.player.currentLocation.Name;
            Vector2 playerPosition = Game1.player.getTileLocation();
            int faceDirection = Game1.player.getFacingDirection();

            bool result = false; // default

            if (ShouldOpen(isActionKey, Game1.player.getFacingDirection(), locationString, playerPosition))
            {
                result = true;
                switch (locationString)
                {
                    case "SeedShop":
                        Game1.player.currentLocation.createQuestionDialogue(
                            i18n.Get("SeedShop_Menu"),
                            new Response[2]
                            {
                                new Response("Shop", i18n.Get("SeedShopMenu_Shop")),
                                new Response("Leave", i18n.Get("SeedShopMenu_Leave"))
                            },
                            delegate(Farmer who, string whichAnswer)
                            {
                                switch (whichAnswer)
                                {
                                    case "Shop":
                                        Game1.activeClickableMenu = (IClickableMenu)new ShopMenu(Utility.getShopStock(true), 0, "Pierre");
                                        break;
                                    case "Leave":
                                        // do nothing
                                        break;
                                    default:
                                        Monitor.Log($"invalid dialogue answer: {whichAnswer}", LogLevel.Info);
                                        break;
                                }
                            }
                        );
                        break;
                    case "AnimalShop":
                        Game1.player.currentLocation.createQuestionDialogue(
                            i18n.Get("AnimalShop_Menu"),
                            new Response[3]
                            {
                                new Response("Supplies", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Supplies")),
                                new Response("Purchase", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Animals")),
                                new Response("Leave", Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Leave"))
                            },
                            "Marnie"
                        );
                        break;
                    case "ScienceHouse":
                        if (Game1.player.daysUntilHouseUpgrade < 0 && !Game1.getFarm().isThereABuildingUnderConstruction())
                        {
                            Response[] answerChoices;
                            if (Game1.player.HouseUpgradeLevel < 3)
                                answerChoices = new Response[4]
                                {
                                    new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")),
                                    new Response("Upgrade", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_UpgradeHouse")),
                                    new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")),
                                    new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
                                };
                            else
                                answerChoices = new Response[3]
                                {
                                    new Response("Shop", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Shop")),
                                    new Response("Construct", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Construct")),
                                    new Response("Leave", Game1.content.LoadString("Strings\\Locations:ScienceHouse_CarpenterMenu_Leave"))
                                };

                            Game1.player.currentLocation.createQuestionDialogue(i18n.Get("ScienceHouse_CarpenterMenu"), answerChoices, "carpenter");
                        }
                        else
                        {
                            Game1.activeClickableMenu = (IClickableMenu)new ShopMenu(Utility.getCarpenterStock(), 0, "Robin");
                        }
                        break;
                    case "FishShop":
                        Game1.player.currentLocation.createQuestionDialogue(
                            (SDate.Now() != new SDate(9, "spring")) ? i18n.Get("FishShop_Menu") : i18n.Get("FishShop_Menu_DocVisit"),
                            new Response[2]
                            {
                                new Response("Shop", i18n.Get("FishShopMenu_Shop")),
                                new Response("Leave", i18n.Get("FishShopMenu_Leave"))
                            },
                            delegate (Farmer who, string whichAnswer)
                            {
                                switch (whichAnswer)
                                {
                                    case "Shop":
                                        Game1.activeClickableMenu = (IClickableMenu)new ShopMenu(Utility.getFishShopStock(Game1.player), 0, "Willy");
                                        break;
                                    case "Leave":
                                        // do nothing
                                        break;
                                    default:
                                        Monitor.Log($"invalid dialogue answer: {whichAnswer}", LogLevel.Info);
                                        break;
                                }
                            }
                        );
                        break;
                    default:
                        Monitor.Log($"invalid location: {locationString}", LogLevel.Info);
                        break;
                }
            }

            return result;

        }

        private bool ShouldOpen(bool isActionKey, int facingDirection, String locationString, Vector2 playerLocation)
        {
            bool result = false;
            if (Game1.activeClickableMenu == null && isActionKey && facingDirection == 3) // somehow SMAPI doesn't provide enum for facing directions?
            {
                // TODO: refactor this part to avoid hard coded tile locations
                switch (locationString)
                {
                    case "SeedShop":
                        result = this.seedShopCounterTiles.Contains(playerLocation) && (npcRefs["Pierre"].currentLocation.Name != locationString || !npcRefs["Pierre"].getTileLocation().Equals(new Vector2(4f, 17f)));
                        break;
                    case "AnimalShop":
                        result = this.animalShopCounterTiles.Contains(playerLocation) && (npcRefs["Marnie"].currentLocation.Name != locationString || !npcRefs["Marnie"].getTileLocation().Equals(new Vector2(12f, 14f)));
                        break;
                    case "ScienceHouse":
                        result = this.carpentersShopCounterTiles.Contains(playerLocation) && (npcRefs["Robin"].currentLocation.Name != locationString || !npcRefs["Robin"].getTileLocation().Equals(new Vector2(8f, 18f)));
                        break;
                    case "FishShop":
                        result = this.fishShopShopCounterTiles.Contains(playerLocation) && (npcRefs["Willy"].currentLocation.Name != locationString || npcRefs["Willy"].getTileLocation().Y < 6f);
                        break;
                    default:
                        // Monitor.Log($"no shop at location {locationString}", LogLevel.Info);
                        break;
                }
            }

            return result;
        }
    }
}
