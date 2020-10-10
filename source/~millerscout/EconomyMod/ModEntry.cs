/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using EconomyMod;
using EconomyMod.Helpers;
using EconomyMod.Interface;
using EconomyMod.Interface.PageContent;
using EconomyMod.Interface.Submenu;
using EconomyMod.Multiplayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace EconomyMod
{
    public class ModEntry : Mod
    {
        private TaxationService taxation;
        //public EconomyInterfaceHandler Interface { get; set; }
        public MessageBroadcastService messageBroadcast { get; set; }
        public override void Entry(IModHelper helper)
        {
            Util.Config = helper.ReadConfig<ModConfig>();
            Util.ModManifest = this.ModManifest;
            Util.Monitor = this.Monitor;
            Util.Helper = helper;

            this.taxation = new TaxationService();
            var framework = new UIFramework();
            framework.AddNewPage(() => new EconomyPage(
                framework,
                Util.Helper.Content.Load<Texture2D>($"assets/Interface/tabIcon.png"),
                Util.Helper.Translation.Get("BalanceReportText"), taxation), 0, new SidetabData(Util.Helper.Content.Load<Texture2D>($"assets/Interface/sidebarButtonReport.png"), Util.Helper.Translation.Get("Sidetab_TaxPaymentReportText"), 0));

            taxation.OnTaxScheduleListUpdated += (object sender, IEnumerable<Model.TaxSchedule> collection) => framework.UpdateListDataList(collection);

            //framework.AddNewPage(() => new LoanPage(framework, taxation), 0, new SidetabData(Util.Helper.Content.Load<Texture2D>($"assets/Interface/LoanButton.png"), "Loan", 0));



            this.messageBroadcast = new MessageBroadcastService(taxation);

#if DEBUG
            Util.IsDebug = true;
            Monitor.Log("-----------------------DEBUG MODE-----------------------", LogLevel.Alert);
            this.Helper.Events.GameLoop.DayStarted += (object sender, StardewModdingAPI.Events.DayStartedEventArgs e) => Game1.addHUDMessage(new HUDMessage("DEBUG MODE", 2));

            helper.Events.Input.ButtonPressed += (object sender, ButtonPressedEventArgs e) =>
             {
                 if (!Context.IsWorldReady)
                     return;

                 if (e.Button == SButton.F)
                 {
                     InterfaceHelper.DrawGuidelines = !InterfaceHelper.DrawGuidelines;
                 }
                 if (e.Button == SButton.G)
                 {
                     var message = new Multiplayer.Messages.BroadcastMessage();
                     Util.Helper.Multiplayer.SendMessage(message, "nil", modIDs: new[] { Util.ModManifest.UniqueID });

                 }
             };
#endif
        }

    }


}
