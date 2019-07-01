using System;

namespace Denifia.Stardew.SendItems.Domain
{
    public class BasePerson : IEquatable<BasePerson>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FarmName { get; set; }
        public string DisplayText => $"{Name} ({FarmName} Farm)";
        public string ConsoleSafeName => MakeConsoleSave(Name);
        public string ConsoleSaveFarmName => MakeConsoleSave(FarmName);

        private string MakeConsoleSave(string text)
        {
            if (text.Contains(" "))
            {
                text = $"\"{text}\"";
            }
            return text;
        }

        public bool Equals(BasePerson other)
        {
            return Id == other.Id;
        }
    }
}
