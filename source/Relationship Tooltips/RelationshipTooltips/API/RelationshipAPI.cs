using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelationshipTooltips.Relationships;
using StardewModdingAPI;

namespace RelationshipTooltips.API
{
    public class RelationshipAPI : IRelationshipAPI
    {
        private IMonitor Monitor;
        public RelationshipAPI(IMonitor m)
        {
            Monitor = m;
        }
        public event EventHandler<EventArgsRegisterRelationships> RegisterRelationships;
        public event EventHandler<EventArgsRegisterRelationships> OnRegisterRelationshipsComplete;
        internal EventArgsRegisterRelationships FireRegistrationEvent()
        {
            EventArgsRegisterRelationships toRegister = new EventArgsRegisterRelationships();
            try
            {
                RegisterRelationships?.Invoke(null, toRegister);
            OnRegisterRelationshipsComplete?.Invoke(null, toRegister);
            }
            catch (Exception e)
            {
                Monitor.Log(e.ToString(), LogLevel.Error);
            }
            return toRegister;
        }
    }
}