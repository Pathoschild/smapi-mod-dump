/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using DeluxeAutoPetter.helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace DeluxeAutoPetter
{
    internal sealed class DeluxeAutoPetter : Mod
    {
        private static bool IS_DATA_LOADED = false;

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);

            helper.Events.Multiplayer.ModMessageReceived += OnModMessageReceived;
            helper.Events.Multiplayer.PeerConnected += OnPeerConnected;

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;

            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.Player.InventoryChanged += OnPlayerInventoryChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
        }

        private void OnModMessageReceived(object? sender, ModMessageReceivedEventArgs e)
        {
            if (e.FromModID.Equals(ModManifest.UniqueID))
            {
                if (e.Type.Equals(nameof(MultiplayerHandler.QuestData)))
                {
                    if (Context.IsMainPlayer)
                    {
                        MultiplayerHandler.SetPlayerQuestData(e.FromPlayerID, e.ReadAs<MultiplayerHandler.QuestData>());
                        MultiplayerHandler.SavePerPlayerQuestData(Helper);
                    }
                    else
                    {
                        MultiplayerHandler.SetPlayerQuestData(Game1.player.UniqueMultiplayerID, e.ReadAs<MultiplayerHandler.QuestData>());
                        QuestDetails.LoadQuestData(Game1.player.UniqueMultiplayerID);
                    }
                }
            }
        }

        private void OnPeerConnected(object? sender, PeerConnectedEventArgs e)
        {
            if (!Context.IsMainPlayer) return;

            Helper.Multiplayer.SendMessage(MultiplayerHandler.GetPlayerQuestData(e.Peer.PlayerID), nameof(MultiplayerHandler.QuestData), new[] { ModManifest.UniqueID }, new[] { e.Peer.PlayerID });
        }

        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            IS_DATA_LOADED = false;
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            DeluxeAutoPetterDrawPatcher.Initialize(Monitor);
            Harmony harmony = new(ModManifest.UniqueID);
            DeluxeAutoPetterDrawPatcher.ApplyPatch(harmony);
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (Game1.player.hasQuest(QuestDetails.GetQuestID()))
            {
                QuestDetails.ShowDropboxLocator(true);
                if (Game1.activeClickableMenu is null && QuestDetails.IsMouseOverDropbox(Game1.currentCursorTile)) Game1.mouseCursor = Game1.cursor_grab;
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!(Context.IsWorldReady &&
                Game1.currentLocation.Equals(Game1.getLocationFromName(QuestDetails.GetDropBoxGameLocationString())) &&
                Game1.player.hasQuest(QuestDetails.GetQuestID()) &&
                e.Button.IsActionButton()))
                return;

            Vector2 distanceVector = QuestDetails.GetInteractionDistanceFromDropboxVector(Game1.player.GetToolLocation());

            if (Math.Abs(distanceVector.X) < (Game1.tileSize * 1.5) && Math.Abs(distanceVector.Y) < Game1.tileSize / 2)
                Game1.activeClickableMenu ??= QuestDetails.CreateQuestContainerMenu();
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            QuestDetails.Initialize(ModManifest.UniqueID);
            QuestMail.Initialize(ModManifest.UniqueID);
            ObjectDetails.Initialize(ModManifest.UniqueID, Helper.DirectoryPath);
            ShopDetails.Initialize();
            MultiplayerHandler.Initialize(ModManifest.UniqueID);

            QuestDetails.LoadQuest();
            QuestMail.LoadMail(QuestMail.GetQuestMailID(), QuestMail.GetQuestMailDetails());
            QuestMail.LoadMail(QuestMail.GetQuestRewardMailID(), QuestMail.GetQuestRewardMailDetails());
            ObjectDetails.LoadObject();
            ShopDetails.LoadShopItem();

            if (Context.IsMainPlayer)
            {
                MultiplayerHandler.LoadPerPlayerQuestData(Helper, Game1.player.UniqueMultiplayerID);
                QuestDetails.LoadQuestData(Game1.player.UniqueMultiplayerID);
                IS_DATA_LOADED = true;
            }
            else Monitor.Log("You are not the host. If the host does not have this mod, then all data for this mod will be ignored.", LogLevel.Info);
        }

        private void OnPlayerInventoryChanged(object? sender, InventoryChangedEventArgs e)
        {
            if (QuestDetails.IsQuestDataNull() && !Context.IsMainPlayer) return;

            if (!QuestDetails.GetIsTriggered() && e.Added.Any(item => item.QualifiedItemId.Equals(QuestDetails.GetAutoPetterID())))
            {
                e.Player.mailForTomorrow.Add(QuestMail.GetQuestMailID());
                QuestDetails.SetIsTriggered(true);
            }
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (!Context.IsMainPlayer) return;

            foreach (StardewValley.Buildings.Building building in Game1.getFarm().buildings)
            {
                if (building.GetIndoors()?.Objects.Values.FirstOrDefault(sObject => sObject?.QualifiedItemId.Equals($"(BC){ObjectDetails.GetDeluxeAutoPetterID()}") ?? false, null) is not null)
                {
                    foreach (FarmAnimal animal in  building.GetIndoors().Animals.Values)
                    {
                        animal.pet(Game1.getFarmer(animal.ownerID.Value), true);
                        animal.pet(Game1.getFarmer(animal.ownerID.Value));
                    }
                }
            }
        }

        private void OnDayEnding(object? sender, DayEndingEventArgs e)
        {
            if (!IS_DATA_LOADED && Context.IsMainPlayer) return;
            else if (Context.IsMainPlayer) MultiplayerHandler.SavePerPlayerQuestData(Helper);
            else Helper.Multiplayer.SendMessage(MultiplayerHandler.GetPlayerQuestData(Game1.player.UniqueMultiplayerID), nameof(MultiplayerHandler.QuestData), new[] { ModManifest.UniqueID });
        }
    }
}
