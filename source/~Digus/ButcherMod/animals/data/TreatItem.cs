using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AnimalHusbandryMod.animals.data
{
    public interface TreatItem
    {
        int MinimumDaysBetweenTreats { get; set; }
        object[] LikedTreats { get; set; }
        [JsonIgnore]
        ISet<int> LikedTreatsId { get; set; }
    }
}
