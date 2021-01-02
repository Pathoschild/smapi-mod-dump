/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using ExpandedPreconditionsUtility;
using Microsoft.Xna.Framework;
using NpcAdventure.Loader;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Story.Scenario
{
    class CompanionCutscenes : BaseScenario
    {
        private readonly IContentLoader contentLoader;
        private readonly CompanionManager companionManager;
        private readonly IConditionsChecker epu;

        public CompanionCutscenes(IContentLoader contentLoader, CompanionManager companionManager, IConditionsChecker epu)
        {
            this.contentLoader = contentLoader;
            this.companionManager = companionManager;
            this.epu = epu;
        }

        public override void Dispose()
        {
            this.GameMaster.Events.CheckEvent -= this.Events_CheckEvent;
        }

        public override void Initialize()
        {
            this.GameMaster.Events.CheckEvent += this.Events_CheckEvent;
        }

        private void Events_CheckEvent(object sender, ICheckEventEventArgs e)
        {
            if (Game1.CurrentEvent != null || Game1.eventUp)
                return;

            var recruitedCsm = this.companionManager
                    .PossibleCompanions
                    .Values
                    .FirstOrDefault(csm => csm.CurrentStateFlag == StateMachine.CompanionStateMachine.StateFlag.RECRUITED);

            if (recruitedCsm == null)
                return;

            var events = this.contentLoader.LoadStrings("Data/Events");
            foreach (var key in events.Keys)
            {
                string[] tokens = key.Split('/');

                if (tokens.Length >= 4 && tokens[0] == "companion" && recruitedCsm.Name == tokens[2] && e.Location.Name == tokens[3])
                {
                    int eventId = int.Parse(tokens[1]);
                    bool canPlay = tokens.Length == 4 || this.epu.CheckConditions(string.Join("/", tokens.Skip(4).ToArray()));

                    if (!Game1.player.eventsSeen.Contains(eventId) && canPlay)
                    {
                        var companionEvent = new Event(events[key], eventId);

                        companionEvent.onEventFinished += () =>
                        {
                            var afterEventPosition = Game1.player.positionBeforeEvent;

                            if (recruitedCsm.Companion.currentLocation is MineShaft mines)
                            {
                                afterEventPosition = recruitedCsm.Reflection.GetProperty<Vector2>(mines, "tileBeneathLadder").GetValue();
                                Game1.player.positionBeforeEvent = afterEventPosition;
                            }

                            recruitedCsm.Companion.setTileLocation(afterEventPosition);
                        };

                        e.Location.startEvent(companionEvent);
                        break;
                    }
                }
            }
        }
    }
}
