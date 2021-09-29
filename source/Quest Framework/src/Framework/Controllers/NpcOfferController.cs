/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/QuestFramework
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuestFramework.Events;
using QuestFramework.Framework;
using QuestFramework.Framework.Helpers;
using QuestFramework.Offers;
using QuestFramework.Quests;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestFramework.Framework.Controllers
{
    internal class NpcOfferController
    {
        private readonly IMonitor _monitor;
        private readonly QuestOfferManager _offerManager;
        private readonly QuestManager _questManager;
        protected readonly PerScreen<List<QuestOffer<NpcOfferAttributes>>> _npcQuestOffers;
        protected readonly PerScreen<HashSet<string>> _activeIndicators;

        protected List<QuestOffer<NpcOfferAttributes>> QuestOffers => this._npcQuestOffers.Value;
        protected HashSet<string> ActiveIndicators => this._activeIndicators.Value;

        public NpcOfferController(QuestOfferManager offerManager, QuestManager questmanager, IModEvents modEvents, IQuestFrameworkEvents qfEvents, IMonitor monitor)
        {
            this._npcQuestOffers = new PerScreen<List<QuestOffer<NpcOfferAttributes>>>(CreateOfferList);
            this._activeIndicators = new PerScreen<HashSet<string>>(CreateActiveIndicatorList);
            this._offerManager = offerManager;
            this._questManager = questmanager;
            this._monitor = monitor;

            modEvents.GameLoop.DayStarted += this.OnDayStarted;
            modEvents.GameLoop.TimeChanged += this.OnTimeChanged;
            modEvents.Player.Warped += this.OnPlayerWarped;
            modEvents.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            modEvents.Display.RenderedWorld += this.OnRenderedWorld;
            qfEvents.QuestAccepted += this.OnQuestAccepted;
        }

        private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
        {
            if (!QuestFrameworkMod.Instance.Config.ShowNpcQuestIndicators)
                return;

            if (!Context.IsWorldReady || this.ActiveIndicators.Count == 0)
                return;

            if (Game1.player?.currentLocation == null)
                return;

            foreach (NPC npc in Game1.player.currentLocation.characters)
            {
                if (!this.ShouldDrawIndicator(npc))
                    continue;

                float yOffset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                e.SpriteBatch.Draw(Game1.mouseCursors,
                    Game1.GlobalToLocal(
                        Game1.viewport, new Vector2(npc.Position.X + npc.Sprite.SpriteWidth * 2, npc.Position.Y + yOffset - 92)),
                    new Rectangle(395, 497, 3, 8),
                    Color.White, 0f,
                    new Vector2(1f, 4f), 4f + Math.Max(0f, 0.25f - yOffset / 16f),
                    SpriteEffects.None, 0.6f);
            }
        }

        private bool ShouldDrawIndicator(NPC npc)
        {
            IReflectionHelper reflection = QuestFrameworkMod.Instance.Helper.Reflection;

            return npc.isVillager()
                && this.ActiveIndicators.Contains(npc.Name)
                && !npc.IsInvisible 
                && !npc.IsEmoting 
                && !npc.isSleeping.Value 
                && !Game1.eventUp
                && reflection.GetField<int>(npc, "textAboveHeadTimer").GetValue() <= 0
                && reflection.GetField<string>(npc, "textAboveHead").GetValue() == null;
        }

        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            this._npcQuestOffers.ResetAllScreens();
            this._activeIndicators.ResetAllScreens();
        }

        private void OnQuestAccepted(object sender, QuestEventArgs e)
        {
            if (!e.IsManaged || this._npcQuestOffers.Value.Count == 0)
                return;

            CustomQuest quest = e.GetManagedQuest();
            QuestOffer<NpcOfferAttributes> offer = this.QuestOffers.FirstOrDefault(o => o.QuestName == quest.GetFullName());

            if (offer != null)
            {
                this.QuestOffers.Remove(offer);
                this.RefreshActiveIndicators();
            }
        }

        private void OnPlayerWarped(object sender, WarpedEventArgs e)
        {
            this.RefreshNpcOffers();
        }

        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            this.RefreshNpcOffers();
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            this.RefreshNpcOffers();
        }

        public void RefreshActiveIndicators()
        {
            if (!QuestFrameworkMod.Instance.Config.ShowNpcQuestIndicators)
                return;

            this.ActiveIndicators.Clear();

            foreach (var offer in this.QuestOffers)
            {
                if (!offer.OfferDetails.Secret)
                    this.ActiveIndicators.Add(offer.OfferDetails.NpcName);
            }

            foreach (var requestor in GetNpcOfferedOrders().Select(o => o.requester.Value))
                this.ActiveIndicators.Add(requestor);
        }

        public void RefreshNpcOffers()
        {
            if (!Context.IsWorldReady)
                return;

            this._monitor.Log("Refreshing NPC quest offers");

            var offers = from o in this._offerManager.GetMatchedOffers<NpcOfferAttributes>("NPC")
                         where !Game1.player.hasQuest(this._questManager.ResolveGameQuestId(o.QuestName))
                            && !string.IsNullOrEmpty(o.OfferDetails.DialogueText)
                         select o;

            this.QuestOffers.Clear();
            this.QuestOffers.AddRange(offers);
            this.RefreshActiveIndicators();
        }

        public bool TryOfferNpcQuest(NPC npc, out QuestOffer<NpcOfferAttributes> offer)
        {
            offer = this.QuestOffers.FirstOrDefault(o => o.OfferDetails.NpcName == npc.Name);

            this.RefreshActiveIndicators();
            return offer != null;
        }

        public bool TryOfferNpcSpecialOrder(NPC npc, out SpecialOrder specialOrder)
        {
            specialOrder = GetNpcOfferedOrders()
                .FirstOrDefault(o => o.requester.Value == npc.Name);

            this.RefreshActiveIndicators();
            return specialOrder != null;
        }

        private static List<QuestOffer<NpcOfferAttributes>> CreateOfferList()
        {
            return new List<QuestOffer<NpcOfferAttributes>>();
        }

        private static HashSet<string> CreateActiveIndicatorList()
        {
            return new HashSet<string>();
        }

        private static IEnumerable<SpecialOrder> GetNpcOfferedOrders()
        {
            return from order in Game1.player.team.availableSpecialOrders
                   where order.orderType.Value == "QF_NPC"
                     && !Utils.IsSpecialOrderAccepted(order.questKey.Value)
                   select order;
        }
    }
}
