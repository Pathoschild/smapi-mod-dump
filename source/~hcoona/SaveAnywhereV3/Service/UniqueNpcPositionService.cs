using System.Collections.Generic;
using System.Linq;
using SaveAnywhereV3.DataContract;
using StardewValley;

namespace SaveAnywhereV3.Service
{
    public class UniqueNpcPositionService : SaveLoadServiceBase<List<NpcPosition>>
    {
        public UniqueNpcPositionService()
            : base(model => model.NpcPositionList)
        { }

        protected override List<NpcPosition> DumpSaveModel()
        {
            return UniqueNPCs.Select(npc => new NpcPosition
            {
                Name = npc.name,
                X = npc.getTileX(),
                Y = npc.getTileY(),
                Location = npc.currentLocation.name,
                Direction = npc.facingDirection,
                SchedulePathDescriptionKey = LookupVillagerSchedulePathDescriptionKey(npc),
                PathToEndPointCount = npc.controller?.pathToEndPoint?.Count
            }).ToList();
        }

        protected override void DoLoad(List<NpcPosition> model)
        {
            var positions = model.ToDictionary(p => p.Name);
            foreach (var npc in UniqueNPCs)
            {
                if (positions.TryGetValue(npc.name, out var p))
                {
                    Game1.warpCharacter(npc, p.Location, new Microsoft.Xna.Framework.Vector2(p.X, p.Y), false, true);
                    if (npc.isVillager())
                    {
                        RestoreVillagerSchedule(npc, p);
                    }
                }
            }
        }

        private IEnumerable<NPC> UniqueNPCs
        {
            get
            {
                return from location in Game1.locations
                       from npc in location.characters
                       where !npc.IsMonster
                       group npc by npc.name into g
                       where g.Count() == 1
                       select g.Single();
            }
        }

        private static void RestoreVillagerSchedule(NPC npc, NpcPosition p)
        {
            var schedule = npc.getSchedule(Game1.dayOfMonth);
            npc.Schedule = schedule;

            if (schedule == null)
            {
                return;
            }

            var key = p.SchedulePathDescriptionKey;
            if (key.HasValue && schedule.TryGetValue(key.Value, out var schedulePathDescription))
            {
                npc.DirectionsToNewLocation = schedulePathDescription;
                npc.controller = new PathFindController(schedulePathDescription.route, npc, Utility.getGameLocationOfCharacter(npc))
                {
                    finalFacingDirection = schedulePathDescription.facingDirection,
                    endBehaviorFunction = Global.Helper.Reflection.GetPrivateMethod(npc, "getRouteEndBehaviorFunction")
                        .Invoke<PathFindController.endBehavior>(schedulePathDescription.endOfRouteBehavior, schedulePathDescription.endOfRouteMessage)
                };

                if (p.PathToEndPointCount.HasValue)
                {
                    int n = npc.controller.pathToEndPoint.Count - p.PathToEndPointCount.Value;
                    while (n-- != 0)
                    {
                        npc.controller.pathToEndPoint.Pop();
                    }
                }
            }
        }

        private int? LookupVillagerSchedulePathDescriptionKey(NPC npc)
        {
            if (npc.isVillager() && npc.Schedule != null && npc.DirectionsToNewLocation != null)
            {
                return npc.Schedule.Single(p => p.Value == npc.DirectionsToNewLocation).Key;
            }
            else
            {
                return null;
            }
        }
    }
}
