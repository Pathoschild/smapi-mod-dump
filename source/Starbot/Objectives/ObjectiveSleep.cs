using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starbot.Objectives
{
    class ObjectiveSleep : Objective
    {
        public override string AnnounceMessage => "Going to sleep";
        public override string UniquePoolId => "sleep";
        public override bool Cooperative => true; //not exclusive to a single player

        public ObjectiveSleep()
        {
            IsComplete = false;
        }

        public override void Reset()
        {
            base.Reset();
            IsComplete = false;
        }

        public override void Step()
        {
            base.Step();

            //step one: route to the homelocation
            if (Game1.player.homeLocation != Game1.player.currentLocation.NameOrUniqueName)
            {
                Core.RouteTo(Game1.player.homeLocation, critical: true);
                return;
            }

            //step two: to bed!
            if(!(Game1.player.currentLocation is StardewValley.Locations.FarmHouse))
            {
                Mod.instance.Monitor.Log("This is home but not a FarmHouse?!", StardewModdingAPI.LogLevel.Error);
                Fail();
                return;
            }

            var fh = Game1.player.currentLocation as StardewValley.Locations.FarmHouse;
            var bed = fh.getBedSpot();

            Core.RouteTo(Game1.player.currentLocation.NameOrUniqueName, bed.X, bed.Y, true);
        }

        public override void CantMoveUpdate()
        {
            base.CantMoveUpdate();
            if (Game1.dialogueUp)
            {
                Mod.instance.Monitor.Log("Bed prompt activated. Choosing yes...", StardewModdingAPI.LogLevel.Info);
                Core.AnswerGameLocationDialogue(0);
                Core.IsSleeping = true;
                IsComplete = true;
            }
        }
    }
}
