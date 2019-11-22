using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelationshipTooltips.Relationships;

namespace RelationshipTooltips.API
{
    public interface IRelationshipAPI
    {
        event EventHandler<EventArgsRegisterRelationships> RegisterRelationships;
        event EventHandler<EventArgsRegisterRelationships> OnRegisterRelationshipsComplete;
    }
}
