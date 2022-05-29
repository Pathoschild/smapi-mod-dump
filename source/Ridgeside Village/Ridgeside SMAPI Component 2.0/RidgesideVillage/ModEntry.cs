/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace RidgesideVillage
{
    public class ModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; set; }
        internal new static IModHelper Helper { get; set; }

        internal static ModConfig Config;

        private ConfigMenu ConfigMenu;
        private CustomCPTokens CustomCPTokens;
        private Patcher Patcher;

        private SpiritShrine SpiritShrine;

        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            Helper = helper;

            if (!new InstallationChecker().checkInstallation(helper))
            {
                return;
            }

            ConfigMenu = new ConfigMenu(this);
            CustomCPTokens = new CustomCPTokens(this);

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            
            BgUtils.Initialize(this);

            TortsBackground.Initialize(this);

            BloomProjectile.Initialize(this);
            MistProjectile.Initialize(this);
            Mistblade.Initialize(this);

            Patcher = new Patcher(this);
            Patcher.PerformPatching();

            HotelMenu.Initialize(this);

            Minecarts.Initialize(this);

            SpiritRealm.Initialize(this);

            SpecialOrders.Initialize(this);

            IanShop.Initialize(this);

            Elves.Initialize(this);

            Greenhouses.Initialize(this);

            Loan.Initialize(this);

            //SummitHouse.Initialize(this);

            WarpTotem.Initialize(this);

            PaulaClinic.Initialize(this);

            Offering.OfferingTileAction.Initialize(this);

            NightlyEvent.Initialize(this);

            NinjaBooks.Initialize(this);

            Foxbloom.Initialize(this);

            //not done (yet?)
            //new CliffBackground();

            Helper.ConsoleCommands.Add("LocationModData", "show ModData of given location", printLocationModData);
            Helper.ConsoleCommands.Add("remove_equipment", "Remove all clothes and equipment from farmer", RemoveEquipment);
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            forgetRepeatableEvents();
        }

        private void forgetRepeatableEvents()
        {
            string path = PathUtilities.NormalizePath("assets/RepeatableEvents.json");
            var data = Helper.Content.Load<Dictionary<string, List<int>>>(path);
            if(data.TryGetValue("RepeatEvents", out List<int> repeatableEvents))
            {
                foreach(var entry in repeatableEvents)
                {
                    Game1.player.eventsSeen.Remove(entry);
                }
            }
            if (data.TryGetValue("RepeatResponses", out List<int> repeatableResponses))
            {
                foreach (var entry in repeatableResponses)
                {
                    Game1.player.dialogueQuestionsAnswered.Remove(entry);
                }
            }
            Log.Trace("Removed all repeatable events");
        }

        private void printLocationModData(string arg1, string[] arg2)
        {
            if (arg2.Length < 1)
            {
                Log.Info("Location parameter needed");
                return;
            }
            GameLocation location = Game1.getLocationFromName(arg2[0]);
            if (location != null)
            {
                foreach(var key in location.modData.Keys)
                {
                    Log.Info($"{key}: {location.modData[key]}");
                }
            }
            Log.Info("Done");
        }

        private void OnGameLaunched(object sender, EventArgs e)
        {
            TileActionHandler.Initialize(Helper);
            ImageMenu.Setup(Helper);
            MapMenu.Setup(Helper);
            TrashCans.Setup(Helper);
            RSVWorldMap.Setup(Helper);
            ExternalAPIs.Initialize(Helper);

            Config = Helper.ReadConfig<ModConfig>();

            if (!Helper.ModRegistry.IsLoaded("spacechase0.JsonAssets"))
            {
                Log.Error("JSON Assets is not loaded! This mod *requires* JSON Assets!");
                return;
            }

            // Custom CP Token Set-up
            CustomCPTokens.RegisterTokens();

            Helper.ConsoleCommands.Add("RSV_reset_pedestals", "", ResetPedestals);
            Helper.ConsoleCommands.Add("RSV_open_portal", "", OpenPortal);
            // RSV_rivera_secret in Patches/WalletItem

            // Generic Mod Config Menu setup
            ConfigMenu.RegisterMenu();
        }

        private void OpenPortal(string arg1, string[] arg2)
        {
            if (Context.IsWorldReady && this.SpiritShrine != null)
            {
                this.SpiritShrine.OpenPortal(arg1, arg2);
            }
        }

        private void ResetPedestals(string arg1, string[] arg2)
        {
            if (Context.IsWorldReady && this.SpiritShrine != null)
            {
                this.SpiritShrine.ResetPedestals(arg1, arg2);
            }
        }

        private void RemoveEquipment(string arg1, string[] arg2)
        {
            Game1.player.hat.Value = null;
            Game1.player.shirt.Value = -1;
            Game1.player.changeShirt(-1);
            Game1.player.shirtItem.Value = null;
            Game1.player.pants.Value = -1;
            Game1.player.changePants(Color.White);
            Game1.player.pantsItem.Value = null;
            Game1.player.UpdateClothing();

            try { Game1.player.boots?.Value.onUnequip(); } catch {}
            Game1.player.boots.Value = null;
            Game1.player.changeShoeColor(12);

            try { Game1.player.leftRing?.Value.onUnequip(Game1.player, Game1.currentLocation); } catch {}
            Game1.player.leftRing.Value = null;
            try { Game1.player.rightRing?.Value.onUnequip(Game1.player, Game1.currentLocation); } catch {}
            Game1.player.rightRing.Value = null;

            /*
            if (!Helper.ModRegistry.IsLoaded("bcmpinc.WearMoreRings"))
                return;

            int chest_id = int.Parse(Game1.player.modData["bcmpinc.WearMoreRings/chest-id"]);
            Chest chest = (Chest)Game1.getFarm().objects[new Vector2(chest_id, -50)];
            foreach(var item in chest.items)
            {
                Ring ring = (Ring)item;
                try { ring.onUnequip(Game1.player, Game1.currentLocation); } catch {};
            }
            Chest new_chest = new(true);
            Game1.getFarm().objects[new Vector2(chest_id, -50)] = new_chest;
            */
        }

        private void OnSaveLoaded(object sender, EventArgs ex)
        {
            try
            {
                Config = Helper.ReadConfig<ModConfig>();
            }
            catch (Exception e)
            {
                Log.Debug($"Failed to load config settings. Will use default settings instead. Error: {e}");
                Config = new ModConfig();
            }


            SpiritShrine = new SpiritShrine(this);


            //mark greenhouses as greenhouses, so trees can be planted
            List<string> locationsNames = new List<string>() { "Custom_Ridgeside_AguarCaveTemporary", "Custom_Ridgeside_RSVGreenhouse1", "Custom_Ridgeside_RSVGreenhouse2" };
            foreach (var name in locationsNames)
            {
                GameLocation location = Game1.getLocationFromName(name);
                if (location == null)
                {
                    Log.Trace($"{name} is null");
                    continue;
                }
                location.isGreenhouse.Value = true;
                Log.Trace($"{name} set to greenhouse");
            }

            //remove corrupted Fire SO if the player shouldnt have it
            var team = Game1.player.team;
            if (Game1.player.IsMainPlayer && team.SpecialOrderActive(("RSV.UntimedSpecialOrder.SpiritRealmFlames")))
            {
                //if player has NOT seen portal opening or HAS seen the cleansing event remove the fire quest
                if (!Game1.player.eventsSeen.Contains(75160256) || Game1.player.eventsSeen.Contains(75160263))
                {
                    for (int i = 0; i<team.specialOrders.Count; i++)
                    {
                        if (team.specialOrders[i].questKey.Equals("RSV.UntimedSpecialOrder.SpiritRealmFlames"))
                        {
                            team.specialOrders.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

    }
}
