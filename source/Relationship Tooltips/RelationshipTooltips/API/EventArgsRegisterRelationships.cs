using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelationshipTooltips.Relationships;

namespace RelationshipTooltips.API
{
    public class EventArgsRegisterRelationships : EventArgs
    {
        public EventArgsRegisterRelationships()
        {
            RelationshipsOnHover = new List<IRelationship>();
            RelationshipsOnScreen = new List<IRelationship>();
        }
        public List<IRelationship> RelationshipsOnHover { get; }
        public List<IRelationship> RelationshipsOnScreen { get; }
    }
}
