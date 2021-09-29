/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.TerrainFeatures;
using StardewModdingAPI.Utilities;

namespace RidgesideVillage
{
    internal static class PaulaClinic
    {
        const int cost = 500;

        static IModHelper Helper;
        static IMonitor Monitor;
        internal static void Initialize(IMod ModInstance)
        {
            Helper = ModInstance.Helper;
            Monitor = ModInstance.Monitor;

            TileActionHandler.RegisterTileAction("PaulaCounter", OpenPaulaMenu);
        }

        private static void OpenPaulaMenu(string tileActionString, Vector2 position)
        {
            if (true || Game1.player.health < (Game1.player.maxHealth * 0.8) || Game1.player.stamina < (Game1.player.MaxStamina * 0.8))
            {
                ClinicChoices();
            }
            else
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("Clinic.Healthy"));
            }
        }
        private static void ClinicChoices()
        {
            var responses = new List<Response>
            {
                new Response("healthcheckup", Helper.Translation.Get("Clinic.Health") + $" : ${cost}"),
                new Response("staminacheckup", Helper.Translation.Get("Clinic.Stamina") + $" : ${cost}"),
                new Response("cancel", Helper.Translation.Get("Exit.Text"))
            };
            var responseActions = new List<Action>
            {
                delegate
                {
                    HealthCheckup();
                },
                delegate
                {
                    StaminaCheckup();
                },
                delegate{}
            };

            Game1.activeClickableMenu = new DialogueBoxWithActions(Helper.Translation.Get("Clinic.Choices"), responses, responseActions);
        }

        private static void HealthCheckup()
        {
            if(true || Game1.player.health < (Game1.player.maxHealth * 0.8) && Game1.player.Money >= cost)
            {
                var location = Game1.getLocationFromName("Custom_Ridgeside_PaulaClinic");
                
                var events = location.GetLocationEvents();///fade/message \"{{i18n: 87620002.3}}\"/warp farmer 8 21/pause 600/fade unfade
                //string eventString = $"none/15 15/farmer 16 15 0 Paula 16 13 2/skippable/pause 250/money -{cost}/globalFade 0.007 false/viewport -1000 -1000/pause 1000/pause 2000/playSound pickUpItem/pause 1500/playSound axe/pause 200/playSound healSound/pause 1500/globalFadeToClear 0.007 true/viewport 15 15/pause 1000/speak Paula \"All done!\"/pause 500/end";
                //string eventString = $"none/15 15/farmer 16 15 0 Paula 16 13 2/skippable/fade unfade/pause 250/money -{cost}/fade/pause 400/pause 1000/pause 2000/playSound pickUpItem/pause 1500/playSound axe/pause 200/playSound healSound/pause 1500/fade unfade/pause 1000/speak Paula \"All done!\"/pause 500/end";
                foreach(var key in events.Keys)
                {
                    Log.Debug($"{key}: {events[key]}");
                }
                string eventString = events["healthCheckup"].Replace("{cost}", cost.ToString());
                Log.Debug(eventString);
                Game1.globalFadeToBlack(delegate
                {
                    location.startEvent(new Event(eventString));

                    Game1.player.health = Game1.player.maxHealth;
                });
            }
            else if(Game1.player.health >= 100 && Game1.player.Money >= cost)
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("Clinic.HealthyHealth"));
            }
            else
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("NotEnoughMoney"));
            }
        }

        private static void StaminaCheckup()
        {
            if (true || Game1.player.stamina < (Game1.player.MaxStamina * 0.8) && Game1.player.Money >= cost)
            {
                var location = Game1.getLocationFromName("Custom_Ridgeside_PaulaClinic");

                var events = location.GetLocationEvents();///fade/message \"{{i18n: 87620002.3}}\"/warp farmer 8 21/pause 600/fade unfade
                //string eventString = $"none/15 15/farmer 16 15 0 Paula 16 13 2/skippable/pause 250/money -{cost}/globalFade 0.007 false/viewport -1000 -1000/pause 1000/pause 2000/playSound pickUpItem/pause 1500/playSound axe/pause 200/playSound healSound/pause 1500/globalFadeToClear 0.007 true/viewport 15 15/pause 1000/speak Paula \"All done!\"/pause 500/end";
                //string eventString = $"none/15 15/farmer 16 15 0 Paula 16 13 2/skippable/fade unfade/pause 250/money -{cost}/fade/pause 400/pause 1000/pause 2000/playSound pickUpItem/pause 1500/playSound axe/pause 200/playSound healSound/pause 1500/fade unfade/pause 1000/speak Paula \"All done!\"/pause 500/end";
                foreach (var key in events.Keys)
                {
                    Log.Debug($"{key}: {events[key]}");
                }
                string eventString = events["staminaCheckup"].Replace("{cost}", cost.ToString());
                Log.Debug(eventString);
                Game1.globalFadeToBlack(delegate
                {
                    location.startEvent(new Event(eventString));

                    Game1.player.stamina = Game1.player.MaxStamina;
                });
            }
            else if (Game1.player.stamina >= 100 && Game1.player.Money >= cost)
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("Clinic.HealthyStamina"));
            }
            else
            {
                Game1.activeClickableMenu = new DialogueBox(Helper.Translation.Get("NotEnoughMoney"));
            }
        }
    }
  
}
