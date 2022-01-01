/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/urbanyeti/stardew-better-friendship
**
*************************************************/

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BetterFriendship
{
    internal class ModConfig : INotifyPropertyChanged
    {
        public bool DisplayTalkPrompts { get; set; } = true;
        public int GiftCycleDelay { get; set; } = 2000;
        public bool IgnoreMaxedFriendships { get; set; } = false;
        public bool DisplayBubbles { get; set; } = true;

        private string _giftPreference = "like";
        private int _giftCycleCount = 5;
        private bool _onlyHighestQuality;

        public string GiftPreference
        {
            get => _giftPreference;
            set
            {
                _giftPreference = value;
                OnPropertyChanged();
            }
        }

        public int GiftCycleCount
        {
            get => _giftCycleCount;
            set
            {
                _giftCycleCount = value;
                OnPropertyChanged();
            }
        }

        public bool OnlyHighestQuality
        {
            get => _onlyHighestQuality;
            set
            {
                _onlyHighestQuality = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}