namespace Omegasis.SaveAnywhere.Framework.Models
{
    /// <summary>The data for the current player.</summary>
    public class PlayerData
    {
        /// <summary>The current time.</summary>
        public int Time { get; set; }

        /// <summary>The saved character data.</summary>
        public CharacterData[] Characters { get; set; }

        /// <summary>Checks if the character was swimming at save time.</summary>
        public bool IsCharacterSwimming { get; set; }
    }
}
