using System;

namespace Denifia.Stardew.SendItemsApi.Models
{
    // Zoolander :)
    public class CreateMailModel
    {
        public string ToFarmerId { get; set; }
        public string FromFarmerId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
